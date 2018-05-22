using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Util
{
    public class RequestFileLogger : IDisposable
    {
        private static RequestFileLogger logger = null;
        private static readonly object _lock = new object();

        public static RequestFileLogger GetRequestFileLogger(IConfiguration configuration)
        {
            lock (_lock)
            {
                if (logger == null)
                    logger = new RequestFileLogger(configuration);
            }
            return logger;
        }
        
        private StringBuilder sb;
        private string folderPath;
        private string fileName;
        private string timeFormatting;

        private RequestFileLogger(IConfiguration configuration)
        {
            sb = new StringBuilder(512 * 1024);

            folderPath = configuration["RequestLoggingConfig:FolderPath"];
            if (string.IsNullOrWhiteSpace(folderPath))
                folderPath = AppDomain.CurrentDomain.BaseDirectory + '\\';

            fileName = configuration["RequestLoggingConfig:FileName"];
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + " LOG " + DateTime.Now.ToString("yyyy-MM-dd HH;mm;ss") + ".txt";

            timeFormatting = configuration["RequestLoggingConfig:TimeFormatting"];
            if (string.IsNullOrWhiteSpace(timeFormatting))
                timeFormatting = "MMMdd HH:mm:ss.fff";
        }

        public void WriteLine(string userName, string method, string path)
        {
            lock (_lock)
            {
                sb.Append(DateTime.Now.ToString(timeFormatting)).Append('|').Append(userName).Append(',').Append(method).Append(',').AppendLine(path);
                if (sb.Length * 2 > sb.Capacity)
                    Flush();
            }
        }

        private void Flush()
        {
            Directory.CreateDirectory(folderPath);
            using (var writer = File.AppendText(folderPath + fileName))
                writer.Write(sb.ToString());
            sb.Length = 0;
        }

        public void Dispose()
        {
            lock (_lock)
                Flush();
        }
    }
}
