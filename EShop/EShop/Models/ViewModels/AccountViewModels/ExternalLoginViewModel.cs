using System.ComponentModel.DataAnnotations;

namespace EShop.Models.ViewModels.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required] [EmailAddress] public string Email { get; set; }
    }
}