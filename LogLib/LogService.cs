using System;
using System.Collections.ObjectModel;

namespace LogLib
{
    /**
     * 
     * 自定义日志库
     * 2021/10/07
     * 
     * */
    public class LogService : ILogService
    {
        private static ObservableCollection<LogItem> logList;
        private static ConsoleManager cm;
        public ObservableCollection<LogItem> GetLogs() => logList;  
        //是否显示在控制台（用于调试程序）
        private bool isShowConsole;
        public LogService(bool isShowConsole=false) { 
            logList=new ObservableCollection<LogItem>();
            this.isShowConsole = isShowConsole;
            if (isShowConsole)
            {
                cm = new ConsoleManager();
                cm.ShowConsole();
            }
        }
        private string GetCurrentTime() => DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        public void Debug(string message)
        {
            var item = new LogItem(GetCurrentTime(), message, LogLevel.Debug);
            logList.Add(item);
            if (isShowConsole) {
                Console.WriteLine(item.ToString());
            }
        }

        public void Error(string message)
        {
            var item = new LogItem(GetCurrentTime(), message, LogLevel.Error);
            logList.Add(item);
            if (isShowConsole)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public void Info(string message)
        {
            var item = new LogItem(GetCurrentTime(), message, LogLevel.Info);
            logList.Add(item);
            if (isShowConsole)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public void Warn(string message)
        {
            var item = new LogItem(GetCurrentTime(), message, LogLevel.Warning);
            logList.Add(item);
            if (isShowConsole)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public void Release()
        {
            if (isShowConsole)
            {
                Console.WriteLine(@"logger closed...");
            }
            logList.Clear();
            logList = null;
            cm=null;
        }

        public ObservableCollection<LogItem> GetLogList() => logList;
      
        ~ LogService() { 
            Release();
        }
    }
}