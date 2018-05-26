using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EShop.Models
{
    public class OrderViewModel
    {
        public IEnumerable<ProductInCartViewModel> Products { get; set; }
        public List<SelectListItem> savedAddresses;
        public bool checkbox { get; set; }

        //Add custom error messages

        [Required]
        public string ZipConfirmation { get; set; }

        [CreditCard, Required]
        public string CardNumber { get; set; }

        [RegularExpression(@"^[0-9]{3}$"), Required]
        public string CVV { get; set; }

        [Required]
        public string Exp_Year { get; set; }

        [Required]
        public string Exp_Month { get; set; }

        [StringLength(20, MinimumLength = 2), Required]
        public string FirstName { get; set; }

        [StringLength(20, MinimumLength = 2), Required]
        public string LastName { get; set; }

        public string StatusMessage { get; set; }
    }
}
