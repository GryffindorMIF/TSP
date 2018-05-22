using EShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Business
{
    public interface IAttributeService
    {
        ICollection<EShop.Models.Attribute> GetAllAttributes();
        ICollection<AttributeValue> GetAllAttributeValues();
        ICollection<ProductAttributeValue> GetAllProductAttributeValues();

        EShop.Models.Attribute FindAttributeById(int id);
        AttributeValue FindAttributeValueById(int id);
        ProductAttributeValue FindProductAttributeValueById(int id);

        Task<int> AddAttribute(EShop.Models.Attribute attribute);
        Task<int> AddAttributeRange(ICollection<Models.Attribute> attributes);
        Task<int> RemoveAttribute(EShop.Models.Attribute attribute);
        Task<int> RemoveAttributeRange(ICollection<EShop.Models.Attribute> attributes);
        Task<int> UpdateAttribute(EShop.Models.Attribute attribute);

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
