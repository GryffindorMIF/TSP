using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Util
{
    public static class FileManagerMethods
    {
        public static string UploadImage(this IHostingEnvironment appEnvironment, IFormFile file)
        {
            var uploads = Path.Combine(appEnvironment.WebRootPath, "images\\products");
            if (file.Length > 0 && (Path.GetExtension(file.FileName).ToLower() == ".jpg" || Path.GetExtension(file.FileName).ToLower() == ".png" || 
                Path.GetExtension(file.FileName).ToLower() == ".gif" || Path.GetExtension(file.FileName).ToLower() == ".jpeg"))
            {
                var fileName = Guid.NewGuid().ToString().Replace("-","") + Path.GetExtension(file.FileName).ToLower();
                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                return fileName;
            }
            else
            {
                return null;
            }
        }

        public static void DeleteImage(this IHostingEnvironment appEnvironment, string fileName)
        {
            var uploads = Path.Combine(appEnvironment.WebRootPath, "images\\products");
            var filePath = Path.Combine(uploads, fileName);
            File.Delete(filePath);
        }
    }
}
