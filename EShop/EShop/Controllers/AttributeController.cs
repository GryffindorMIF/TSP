﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Models.EFModels.Attribute;
using EShop.Models.EFModels.Product;
using EShop.Models.ViewModels;
using EShop.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShop.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class AttributeController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IAttributeService _attributeService;
        private readonly IProductService _productService;

        public AttributeController(IHostingEnvironment appEnvironment, IAttributeService attributeService,
            IProductService productService)
        {
            _appEnvironment = appEnvironment;
            _attributeService = attributeService;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.AttributeCategories = _attributeService.GetAllAttributes();

            var mavm = new ManageAttributesViewModel
            {
                AttributeSelectList = new SelectList(_attributeService.GetAllAttributes(), "Id", "Name"),
                AttributeyMultiSelectList = new MultiSelectList(_attributeService.GetAllAttributes(), "Id", "Name"),
                AttributeValuesMultiSelectList = new MultiSelectList(
                    from av in _attributeService.GetAllAttributeValues()
                    select new
                    {
                        av.Id,
                        AttributeInCategory = _attributeService.FindAttributeById(av.AttributeId).Name + "/" + av.Name
                    },
                    "Id",
                    "AttributeInCategory"
                ),
                ProductMultiSelectList = new MultiSelectList(await _productService.GetAllProducts(), "Id", "Name"),
                LinksMultiList = new MultiSelectList(from pav in _attributeService.GetAllProductAttributeValues()
                    select new
                    {
                        pav.Id,
                        Association = _attributeService.FindAttributeValueById(pav.AttributeValueId).Name + " -> " +
                                      _productService.FindProductById(pav.ProductId).Name
                    },
                    "Id",
                    "Association"
                )
            };
            return View(mavm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttribute(string name)
        {
            var attribute = new Attribute {Name = name};
            await _attributeService.AddAttribute(attribute);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAttributes(ManageAttributesViewModel model)
        {
            ICollection<Attribute> attributesToRemove = new List<Attribute>();

            foreach (var id in model.IdsOfSelectedAttributesToRemove)
            {
                var attribute = _attributeService.FindAttributeById(id);
                if (attribute.IconUrl != null)
                    await _appEnvironment.DeleteImageAsync(attribute.IconUrl, "attribute-icons");
                attributesToRemove.Add(attribute);
            }

            await _attributeService.RemoveAttributeRange(attributesToRemove);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttributeValue(string name, ManageAttributesViewModel model)
        {
            var attrVal = new AttributeValue
            {
                Name = name,
                AttributeId = model.SelectedAttributeId
            };
            await _attributeService.AddAttributeValue(attrVal);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAttributeValues(ManageAttributesViewModel model)
        {
            ICollection<AttributeValue> attributeValuesToRemove = new List<AttributeValue>();

            foreach (var id in model.IdsOfSelectedAttributeValues)
            {
                var attrVal = _attributeService.FindAttributeValueById(id);
                attributeValuesToRemove.Add(attrVal);
            }

            await _attributeService.RemoveAttributeValueRange(attributeValuesToRemove);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> LinkAttributeValuesToProducts(ManageAttributesViewModel model)
        {
            ICollection<ProductAttributeValue> pavs = new List<ProductAttributeValue>();

            foreach (var attrValId in model.IdsOfSelectedAttributeValues)
            foreach (var productId in model.IdsOfSelectedProducts)
            {
                var pav = new ProductAttributeValue
                {
                    AttributeValueId = attrValId,
                    ProductId = productId
                };
                pavs.Add(pav);
            }

            await _attributeService.AddProductAttributeValueRange(pavs);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UnlinkAttributeValues(ManageAttributesViewModel model)
        {
            ICollection<ProductAttributeValue> pavsToRemove = new List<ProductAttributeValue>();

            foreach (var productAttributeValueId in model.IdsOfSelectedLinks)
            {
                var pav = _attributeService.FindProductAttributeValueById(productAttributeValueId);
                pavsToRemove.Add(pav);
            }

            await _attributeService.RemoveProductAttributeValueRange(pavsToRemove);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddIcon(int attributeId, IFormFile file)
        {
            var iconImagePath = await _appEnvironment.UploadImageAsync(file, "attribute-icons", 2097152);

            var attr = _attributeService.FindAttributeById(attributeId);
            if (attr.IconUrl != null) await _appEnvironment.DeleteImageAsync(attr.IconUrl, "attribute-icons");
            attr.IconUrl = iconImagePath;
            await _attributeService.UpdateAttribute(attr);

            return RedirectToAction("Index", "Home");
        }
    }
}