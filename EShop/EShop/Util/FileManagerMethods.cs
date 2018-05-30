using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace EShop.Util
{
    public static class FileManagerMethods
    {
        public static async Task<string> UploadImageAsync(this IHostingEnvironment appEnvironment, IFormFile file,
            string dirName = null, int uploadMaxByteSize = -1)
        {
            string uploads;
            if (dirName == null) uploads = Path.Combine(appEnvironment.WebRootPath, "images");
            else uploads = Path.Combine(appEnvironment.WebRootPath, "images\\" + dirName);

            if (file.Length > 0 && (file.Length < uploadMaxByteSize || uploadMaxByteSize < 0) &&
                (Path.GetExtension(file.FileName).ToLower() == ".jpg" ||
                 Path.GetExtension(file.FileName).ToLower() == ".png" ||
                 Path.GetExtension(file.FileName).ToLower() == ".gif" ||
                 Path.GetExtension(file.FileName).ToLower() == ".jpeg"))
            {
                var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName).ToLower();
                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return fileName;
            }

            return null;
        }

        public static async Task DeleteImageAsync(this IHostingEnvironment appEnvironment, string fileName,
            string dirName = null)
        {
            try
            {
                string uploads;
                if (dirName == null) uploads = Path.Combine(appEnvironment.WebRootPath, "images");
                else uploads = Path.Combine(appEnvironment.WebRootPath, "images\\" + dirName);

                var filePath = Path.Combine(uploads, fileName);
                await Task.Run(() => { File.Delete(filePath); });
            }
            catch(Exception)
            {
                //Ignored
            }
        }
    }
}