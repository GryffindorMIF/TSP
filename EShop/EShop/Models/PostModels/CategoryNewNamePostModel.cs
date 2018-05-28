namespace EShop.Models.PostModels
{
    public class CategoryNewNamePostModel
    {
        public int CategoryId { get; set; }
        public string NewName { get; set; }
        public string NewDescription { get; set; }
        public string RowVersion { get; set; }
    }
}