using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShop.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public List<SelectListItem> SavedAddresses;
        public IEnumerable<ProductInCartViewModel> Products { get; set; }
        public bool Checkbox { get; set; }

        //Add custom error messages

        [Required] public string ZipConfirmation { get; set; }

        [CreditCard]
        [Required]
        [Display(Prompt = "16 digit card number")]
        public string CardNumber { get; set; }

        [RegularExpression(@"^[0-9]{3}$")]
        [Required]
        public string Cvv { get; set; }

        [Required] public string ExpYear { get; set; }

        [Required] public string ExpMonth { get; set; }

        [StringLength(20, MinimumLength = 2)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(20, MinimumLength = 2)]
        [Required]
        public string LastName { get; set; }

        public string StatusMessage { get; set; }
    }
}