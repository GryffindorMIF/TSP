using System.ComponentModel.DataAnnotations;

namespace EShop.Models.ViewModels.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required] [EmailAddress] public string Email { get; set; }
    }
}