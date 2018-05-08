﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EShop.Data;
using EShop.Models;
using EShop.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.WebRequestMethods;

namespace EShop.Controllers
{
    public class AttributeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _appEnvironment;

        public AttributeController(ApplicationDbContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
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
                if(attribute.IconUrl != null) await _appEnvironment.DeleteImageAsync(attribute.IconUrl, "attribute-icons");
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
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddIcon(int attributeId, IFormFile file)
        {
            Debug.WriteLine("$$$ " + attributeId);
            string iconImagePath = await _appEnvironment.UploadImageAsync(file, "attribute-icons", 2097152);

            
                Models.Attribute attr = await _context.Attribute.FindAsync(attributeId);
                if(attr.IconUrl != null)
                {
                    await _appEnvironment.DeleteImageAsync(attr.IconUrl, "attribute-icons");
                }
                attr.IconUrl = iconImagePath;
                _context.Update(attr);
                await _context.SaveChangesAsync();
    
            return RedirectToAction("Index", "Home");
        }
    }
}