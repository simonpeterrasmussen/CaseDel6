using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElprisProducer
{
    internal class SimpleLogging
    {
        private string _path;
        private string _logFileName;
        private string _logfile;

        public SimpleLogging()
        {
            _path = Environment.GetEnvironmentVariable("HOMEDRIVE") +
                    Environment.GetEnvironmentVariable("HOMEPATH") + 
                    @"\AppLogs";
            if (!Directory.Exists(_path))
            {
                _path = Environment.CurrentDirectory;
            }
            //_logFileName = "logs.txt";
            _logFileName = System.Diagnostics.Process.GetCurrentProcess().ProcessName + 
                DateTime.Now.ToString("yyyyMMddHHmmss") +
                "Log.txt";
            _logfile = _path + @"\" + _logFileName;
            
        }
        public void SetFile(string logFileName)
        {
            _logFileName = logFileName;
        }
        public void SetPath(string logPath)
        {
            _path = logPath;
        }

        public void Log(string logItem)
        {
            File.AppendAllText(_logfile, DateTime.Now.ToString() + ": " + logItem + "\r\n");
        }
    }
}
