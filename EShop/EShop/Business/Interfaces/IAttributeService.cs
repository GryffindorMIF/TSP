using System.Collections.Generic;
using System.Threading.Tasks;
using EShop.Models.EFModels.Attribute;
using EShop.Models.EFModels.Product;

namespace EShop.Business.Interfaces
{
    public interface IAttributeService
    {
        ICollection<Attribute> GetAllAttributes();
        ICollection<AttributeValue> GetAllAttributeValues();
        ICollection<ProductAttributeValue> GetAllProductAttributeValues();

        Attribute FindAttributeById(int id);
        AttributeValue FindAttributeValueById(int id);
        ProductAttributeValue FindProductAttributeValueById(int id);

        Task<int> AddAttribute(Attribute attribute);
        Task<int> AddAttributeRange(ICollection<Attribute> attributes);
        Task<int> RemoveAttribute(Attribute attribute);
        Task<int> RemoveAttributeRange(ICollection<Attribute> attributes);
        Task<int> UpdateAttribute(Attribute attribute);

        Task<int> AddAttributeValue(AttributeValue attributeValue);
        Task<int> AddAttributeValueRange(ICollection<AttributeValue> attributeValues);
        Task<int> RemoveAttributeValue(AttributeValue attributeValue);
        Task<int> RemoveAttributeValueRange(ICollection<AttributeValue> attributeValues);
        Task<int> UpdateAttributeValue(AttributeValue attributeValue);

        Task<int> AddProductAttributeValue(ProductAttributeValue pav);
        Task<int> AddProductAttributeValueRange(ICollection<ProductAttributeValue> pavs);
        Task<int> RemoveProductAttributeValue(ProductAttributeValue pav);
        Task<int> RemoveProductAttributeValueRange(ICollection<ProductAttributeValue> pavs);
        Task<int> UpdateProductAttributeValue(ProductAttributeValue pav);

        Task<IList<AttributeValue>> GetProductAttributeValues(int productId);
        Task<IList<AttributeValue>> GetAttributeValuesInCategory(int categoryId);
    }
}