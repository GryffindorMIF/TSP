using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Models.EFModels.Order;
using EShop.Models.EFModels.User;
using EShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShop.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CheckoutController : Controller
    {
        private readonly IAddressManager _addressManager;
        private readonly ICardInfoService _cardInfoService;
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController
        (
            UserManager<ApplicationUser> userManager,
            IShoppingCartService shoppingCartService,
            IAddressManager addressManager,
            IOrderService orderService,
            ICardInfoService cardInfoService
        )
        {
            _userManager = userManager;
            _shoppingCartService = shoppingCartService;
            _addressManager = addressManager;
            _orderService = orderService;
            _cardInfoService = cardInfoService;
        }

        [TempData] public string StatusMessage { get; set; }

        public async Task<IActionResult> Checkout()
        {
            var model = new CheckoutViewModel {StatusMessage = StatusMessage};
            var user = await _userManager.GetUserAsync(User);
            var savedAddresses = await _addressManager.QueryAllSavedDeliveryAddresses(user);
            if (user.ShoppingCartId != null)
            {
                var shoppingCart = await _shoppingCartService.FindShoppingCartByIdAsync((int) user.ShoppingCartId);

                model.Products =
                    await _shoppingCartService.QueryAllShoppingCartProductsAsync(shoppingCart, HttpContext.Session);
            }

            using (var enumerator = model.Products.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return RedirectToAction("Index", "Home");
            }

            model.SavedAddresses = new List<SelectListItem>();

            var savedCardInfo = await _cardInfoService.GetCardInfoByUserId(user.Id);

            if (savedCardInfo != null)
            {
                model.CardNumber = savedCardInfo.CardNumber;
                model.ExpYear = savedCardInfo.ExpYear;
                model.ExpMonth = savedCardInfo.ExpMonth;
            }

            foreach (var da in savedAddresses)
                model.SavedAddresses.Add(new SelectListItem
                {
                    Text = da.Zipcode,
                    Value = da.Zipcode
                });

            return View(model);
        }

        [EnableCors("EShopCorsPolicy")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> MakePurchase(CheckoutViewModel model)
        {
            /*
            if (!ModelState.IsValid)
            {
                model.StatusMessage = "Please fill out all fields";
                //return View(model);
                return new NotFoundResult();
            }
            */

            var user = await _userManager.GetUserAsync(HttpContext.User);

            //Total cost is recalculated separately to prevent user meddling
            var totalCost = await _shoppingCartService.CalculateTotalPriceCents(user, HttpContext.Session);

            var jsonPost =
                "{\"amount\":" + totalCost +
                ",\"number\": \"" + model.CardNumber +
                "\",\"holder\": \"" + model.FirstName + " " + model.LastName +
                "\",\"exp_year\": " + model.ExpYear +
                ",\"exp_month\": " + model.ExpMonth +
                ",\"cvv\": \"" + model.Cvv + "\"}";

            var confirmAddress = await _addressManager.FindAddressByZipcodeAsync(model.ZipConfirmation);

            if (confirmAddress != null) //If address exists
            {
                var purchaseResult = await _orderService.Purchase(totalCost, jsonPost);

                if (purchaseResult == 201) //Payment successful
                {
                    if (user.ShoppingCartId != null)
                    {
                        var shoppingCart = await _shoppingCartService.FindShoppingCartByIdAsync((int) user.ShoppingCartId);
                        await _shoppingCartService.AssignNewShoppingCart(user);

                        var newOrder = new Order
                        {
                            ShoppingCartId = shoppingCart.Id,
                            UserId = user.Id,
                            TotalPrice = (decimal) totalCost / 100,
                            Address = confirmAddress.Country + ", " + confirmAddress.County + " county, " +
                                      confirmAddress.City + " - " + confirmAddress.Address + ", " + confirmAddress.Zipcode,
                            CardNumber = model.CardNumber,
                            PurchaseDate = DateTime.Now,
                            StatusCode = 1
                        };

                        await _orderService.CreateOrderAsync(newOrder);
                        await _shoppingCartService.AddShoppingCartToHistory(shoppingCart);
                    }
                }
                else if (purchaseResult == 400)
                {
                    StatusMessage = "Purchase failed - Invalid input data";
                    return RedirectToAction(nameof(Checkout));
                }
                else if (purchaseResult == 402)
                {
                    StatusMessage = "Purchase failed - Out of funds";
                    return RedirectToAction(nameof(Checkout));
                }
                else
                {
                    StatusMessage = "Purchase failed - Internal server error";
                    //401 - could not auth user on mock-payment server-side
                    //404 - operation not found on mock-payment server-side
                    return RedirectToAction(nameof(Checkout));
                }
            }

            if (model.Checkbox)
            {
                var savedCardInfo = await _cardInfoService.GetCardInfoByUserId(user.Id);
                if (savedCardInfo == null)
                {
                    savedCardInfo = new CardInfo();
                    savedCardInfo.UserId = user.Id;
                    savedCardInfo.CardNumber = model.CardNumber;
                    savedCardInfo.ExpYear = model.ExpYear;
                    savedCardInfo.ExpMonth = model.ExpMonth;
                    await _cardInfoService.CreateCardInfo(savedCardInfo);
                }
                else
                {
                    savedCardInfo.CardNumber = model.CardNumber;
                    savedCardInfo.ExpYear = model.ExpYear;
                    savedCardInfo.ExpMonth = model.ExpMonth;
                    await _cardInfoService.UpdateCardInfo(savedCardInfo);
                }
            }

            return RedirectToAction("Index", "Order");
        }
    }
}