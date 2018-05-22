using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business.Services
{
    public class AttributeService : IAttributeService
    {
        private readonly ApplicationDbContext _context;

        public AttributeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICollection<EShop.Models.Attribute> GetAllAttributes()
        {
            return _context.Attribute.ToList();
        }

        public ICollection<AttributeValue> GetAllAttributeValues()
        {
            return _context.AttributeValue.ToList();
        }

        public ICollection<ProductAttributeValue> GetAllProductAttributeValues()
        {
            return _context.ProductAttributeValue.ToList();
        }

        public EShop.Models.Attribute FindAttributeById(int id)
        {
            return _context.Attribute.Find(id);
        }

        public AttributeValue FindAttributeValueById(int id)
        {
            return _context.AttributeValue.Find(id);
        }

        public ProductAttributeValue FindProductAttributeValueById(int id)
        {
            return _context.ProductAttributeValue.Find(id);
        }

        public async Task<int> AddAttribute(EShop.Models.Attribute attribute)
        {
            try
            {
                _context.Attribute.Add(attribute);
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> AddAttributeRange(ICollection<Models.Attribute> attributes)
        {
            try
            {
                await _context.Attribute.AddRangeAsync(attributes);
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> RemoveAttribute(EShop.Models.Attribute attribute)
        {
            try
            {
                _context.Attribute.Remove(attribute);
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> RemoveAttributeRange(ICollection<Models.Attribute> attributes)
        {
            try
            {
                foreach (var attribute in attributes)
                {
                    _context.Attribute.Remove(attribute);
                }
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> UpdateAttribute(EShop.Models.Attribute attribute)
        {
            try
            {
                _context.Attribute.Update(attribute);
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> AddAttributeValue(AttributeValue attributeValue)
        {
            try
            {
                _context.AttributeValue.Add(attributeValue);
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> AddAttributeValueRange(ICollection<AttributeValue> attributeValues)
        {
            try
            {
                await _context.AttributeValue.AddRangeAsync(attributeValues);
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> RemoveAttributeValue(AttributeValue attributeValue)
        {
            try
            {
                _context.AttributeValue.Remove(attributeValue);
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> RemoveAttributeValueRange(ICollection<AttributeValue> attributeValues)
        {
            try
            {
                foreach (var attrValue in attributeValues)
                {
                    _context.AttributeValue.Remove(attrValue);
                }
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> UpdateAttributeValue(AttributeValue attributeValue)
        {
            try
            {
                _context.AttributeValue.Update(attributeValue);
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> AddProductAttributeValue(ProductAttributeValue pav)
        {
            try
            {
                _context.ProductAttributeValue.Add(pav);
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> AddProductAttributeValueRange(ICollection<ProductAttributeValue> pavs)
        {
            try
            {
                await _context.ProductAttributeValue.AddRangeAsync(pavs);
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> RemoveProductAttributeValue(ProductAttributeValue pav)
        {
            try
            {
                _context.ProductAttributeValue.Remove(pav);
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> RemoveProductAttributeValueRange(ICollection<ProductAttributeValue> pavs)
        {
            try
            {
                foreach (var pav in pavs)
                {
                    _context.ProductAttributeValue.Remove(pav);
                }
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<int> UpdateProductAttributeValue(ProductAttributeValue pav)
        {
            try
            {
                _context.ProductAttributeValue.Update(pav);
                await _context.SaveChangesAsync();

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<IList<AttributeValue>> GetProductAttributeValues(int productId)
        {
            List<AttributeValue> values = await (from a in _context.AttributeValue
                                                 join pa in _context.ProductAttributeValue on productId equals pa.ProductId
                                                 where a.Id == pa.AttributeValueId
                                                 select a).ToListAsync();
            return values;
        }

        public async Task<IList<AttributeValue>> GetAttributeValuesInCategory(int categoryId)
        {
            return await (from a in _context.AttributeValue
                          join pc in _context.ProductCategory on categoryId equals pc.CategoryId
                          join p in _context.Product on pc.ProductId equals p.Id
                          join pav in _context.ProductAttributeValue on p.Id equals pav.ProductId
                          join av in _context.AttributeValue on pav.AttributeValueId equals av.Id
                          where a.Id == av.Id
                          select a).Distinct().ToListAsync();
        }
    }
}
