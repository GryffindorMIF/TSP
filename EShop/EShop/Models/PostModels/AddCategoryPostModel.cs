using EShop.Models.EFModels.Category;

namespace EShop.Models.PostModels
{
    public class AddCategoryPostModel
    {
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? ExistingCategoryId { get; set; }
    }
}