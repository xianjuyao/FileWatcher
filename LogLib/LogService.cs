using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LogLib
{
    /**
     *
     * 自定义日志库
     * 2021/10/07
     *
     * */
    public class LogService : ILogService, IDisposable
    {
        //全局日志集合
        private static ObservableCollection<LogItem> _logList;
        private static ConsoleManager _cm;
        public ObservableCollection<LogItem> GetLogList() => _logList;

        //是否显示在控制台（用于调试程序）
        private readonly bool _isShowConsole;

        public LogService(bool isShowConsole = false)
        {
            _logList = new ObservableCollection<LogItem>();
            _logList.CollectionChanged += OnCollectionChanged;
            _isShowConsole = isShowConsole;
            if (!isShowConsole) return;
            _cm = new ConsoleManager();
            _cm.ShowConsole();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //删除第一个元素 跨度为1000
            if (_logList.Count >= 1000)
            {
                _logList.RemoveAt(0);
            }
        }

        private static string GetCurrentTime() => DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        private void AddLog(LogItem item)
        {
            _logList.Add(item);
            if (_isShowConsole)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public void Debug(string message) => AddLog(new LogItem(GetCurrentTime(), message, LogLevel.Debug));
        public void Error(string message) => AddLog(new LogItem(GetCurrentTime(), message, LogLevel.Error));
        public void Info(string message) => AddLog(new LogItem(GetCurrentTime(), message, LogLevel.Info));
        public void Warn(string message) => AddLog(new LogItem(GetCurrentTime(), message, LogLevel.Warning));

        public void Release()
        {
            if (_isShowConsole)
            {
                Console.WriteLine("logger closed...");
            }

            _logList.Clear();
            _logList = null;
            _cm = null;
        }

        public void Dispose()
        {
            Release();
        }
    }
}