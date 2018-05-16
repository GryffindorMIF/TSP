using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class DataImportViewModel
    {
        public IFormFile ImportFile { get; set; }
    }
}
