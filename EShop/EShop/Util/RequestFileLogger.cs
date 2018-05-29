using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EShop.Util
{
    public class RequestFileLogger : IDisposable
    {
        private static RequestFileLogger _logger;
        private static readonly object Lock = new object();
        private readonly string _fileName;
        private readonly string _folderPath;

        private readonly StringBuilder _sb;
        private readonly string _timeFormatting;

        private RequestFileLogger(IConfiguration configuration)
        {
            _sb = new StringBuilder(512 * 1024);

            _folderPath = configuration["RequestLoggingConfig:FolderPath"];
            if (string.IsNullOrWhiteSpace(_folderPath))
                _folderPath = AppDomain.CurrentDomain.BaseDirectory + '\\';

            _fileName = configuration["RequestLoggingConfig:FileName"];
            if (string.IsNullOrWhiteSpace(_fileName))
                _fileName = Assembly.GetExecutingAssembly().GetName().Name + " LOG " +
                           DateTime.Now.ToString("yyyy-MM-dd HH;mm;ss") + ".txt";

            _timeFormatting = configuration["RequestLoggingConfig:TimeFormatting"];
            if (string.IsNullOrWhiteSpace(_timeFormatting))
                _timeFormatting = "MMMdd HH:mm:ss.fff";
        }

        public void Dispose()
        {
            lock (Lock)
            {
                Flush();
            }
        }

        public static RequestFileLogger GetRequestFileLogger(IConfiguration configuration)
        {
            lock (Lock)
            {
                if (_logger == null)
                    _logger = new RequestFileLogger(configuration);
            }

            return _logger;
        }

        public void WriteLine(string userName, string method, string path)
        {
            lock (Lock)
            {
                _sb.Append(DateTime.Now.ToString(_timeFormatting)).Append('|').Append(userName).Append(',').Append(method)
                    .Append(',').AppendLine(path);
                if (_sb.Length * 2 > _sb.Capacity)
                    Flush();
            }
        }

        private void Flush()
        {
            Directory.CreateDirectory(_folderPath);
            using (var writer = File.AppendText(_folderPath + _fileName))
            {
                writer.Write(_sb.ToString());
            }

            _sb.Length = 0;
        }
    }
}