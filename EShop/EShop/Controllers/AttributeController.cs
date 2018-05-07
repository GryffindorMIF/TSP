using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShop.Controllers
{
    public class AttributeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttributeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            ViewBag.AttributeCategories = _context.Attribute.ToList();

            ManageAttributesViewModel mavm = new ManageAttributesViewModel()
            {
                AttributeSelectList = new SelectList(_context.Attribute.ToList(), "Id", "Name"),
                AttributeyMultiSelectList = new MultiSelectList(_context.Attribute.ToList(), "Id", "Name"),
                AttributeValuesMultiSelectList = new MultiSelectList((from a in _context.AttributeValue.ToList()
                                                                select new
                                                                {
                                                                    a.Id,
                                                                    AttributeInCategory = _context.Attribute.Find(a.AttributeId).Name + "/" + a.Name
                                                                }),
                                                                "Id",
                                                                "AttributeInCategory"
                                                                ),
                ProductMultiSelectList = new MultiSelectList(_context.Product.ToList(), "Id", "Name"),
                LinksMultiList = new MultiSelectList((from pc in _context.ProductAttributeValue.ToList()
                                                                     select new
                                                                     {
                                                                         pc.Id,
                                                                         Association = _context.AttributeValue.Find(pc.AttributeValueId).Name + " -> " +
                                                                                       _context.Product.Find(pc.ProductId).Name
                                                                     }),
                                                                     "Id",
                                                                     "Association"
                                                                     )                                                                    
            };
            return View(mavm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAttribute(string name)
        {
            Models.Attribute attribute = new Models.Attribute(){ Name = name };
            _context.Attribute.Add(attribute);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAttributes(ManageAttributesViewModel model)
        {
            foreach(var id in model.IdsOfSelectedAttributesToRemove)
            {
                Models.Attribute attribute = await _context.Attribute.FindAsync(id);
                _context.Remove(attribute);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAttributeValue(string name, ManageAttributesViewModel model)
        {
            AttributeValue attrVal = new AttributeValue()
            {
                Name = name,
                AttributeId = model.SelectedAttributeId
            };
            _context.Add(attrVal);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAttributeValues(ManageAttributesViewModel model)
        {
            foreach (var id in model.IdsOfSelectedAttributeValues)
            {
                AttributeValue attrVal = await _context.AttributeValue.FindAsync(id);
                _context.Remove(attrVal);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LinkAttributeValuesToProducts(ManageAttributesViewModel model)
        {         
            foreach (var attrValId in model.IdsOfSelectedAttributeValues)
            {
                foreach (var productId in model.IdsOfSelectedProducts)
                {
                    /*
                    Product product = await _context.Product.FindAsync(productId);
                    ICollection<Category> productCategories = (from c in _context.Category
                                                                join pc in _context.ProductCategory on product.Id equals pc.ProductId
                                                                where pc.CategoryId == c.Id
                                                                select c).ToList();

                    AttributeValue attrVal = await _context.AttributeValue.FindAsync(attrValId);
                    foreach (var productCategory in productCategories)
                    {
                        CategoryAttribute ca = new CategoryAttribute();
                        ca.AttributeValueId = attrVal.Id;
                        ca.CategoryId = productCategory.Id;

                        try
                        {
                            _context.Add(ca);
                            await _context.SaveChangesAsync();
                        }
                        catch
                        {
                            _context.Remove(ca);
                            await _context.SaveChangesAsync();
                        }
                    }
                    */

                    ProductAttributeValue pav = new ProductAttributeValue()
                    {
                        AttributeValueId = attrValId,
                        ProductId = productId
                    };
                    _context.Add(pav);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnlinkAttributeValues(ManageAttributesViewModel model)
        {
            foreach(var productAttributeValueId in model.IdsOfSelectedLinks)
            {
                ProductAttributeValue pav = await _context.ProductAttributeValue.FindAsync(productAttributeValueId);
                _context.Remove(pav);

                /*
                AttributeValue attrVal = await _context.AttributeValue.FindAsync(pav.AttributeValueId);
                Product product = await _context.Product.FindAsync(pav.ProductId);

                IList<Category> productCategories = (from c in _context.Category
                                                      join pc in _context.ProductCategory on product.Id equals pc.ProductId
                                                      select c).ToList();

                int productAttributeValuesCount = (from paval in _context.ProductAttributeValue
                                                    where paval.AttributeValueId == productAttributeValueId
                                                    select paval).Count();

                if(productAttributeValuesCount == 1)
                {
                    foreach (var productCategory in productCategories)
                    {
                        CategoryAttribute categoryAttribute = (from cac in _context.CategoryAttribute
                                                               where cac.CategoryId == productCategory.Id
                                                               where cac.AttributeValueId == attrVal.Id
                                                               select cac).FirstOrDefault();

                        if (categoryAttribute != null)
                        {
                            _context.Remove(categoryAttribute);
                        }
                    }
                }
                */
                /*
                foreach (var productCategory in productCategories)
                {
                    CategoryAttribute categoryAttribute = (from cac in _context.CategoryAttribute
                                                                      where cac.CategoryId == productCategory.Id
                                                                      where cac.AttributeValueId == attrVal.Id
                                                                      select cac).FirstOrDefault();
                
                    if(categoryAttribute != null) 
                    {
                        _context.Remove(categoryAttribute);// trinam jei tai paskutine atribute reiksme, susieta su kategorija
                    }
                }
                */
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}