using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class DiscountListViewModel
    {
        public ICollection<bool> HasDiscountList { get; set; }
        public ICollection<Decimal?> DiscountPriceList { get; set; }
        public ICollection<DateTime?> DiscountEndDateList { get; set; }
    }
}
