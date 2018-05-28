namespace EShop.Models.PostModels
{
    public class CategoryPostModel
    {
        public bool Cascade { get; set; }
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool ReferenceOnly { get; set; }
    }
}