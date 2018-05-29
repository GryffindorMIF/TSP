using System;
using System.Collections.Generic;

namespace EShop.Models.ViewModels.Home
{
    public class DiscountListViewModel
    {
        public ICollection<bool> HasDiscountList { get; set; }
        public ICollection<decimal?> DiscountPriceList { get; set; }
        public ICollection<DateTime?> DiscountEndDateList { get; set; }
    }
}