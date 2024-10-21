using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

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
        //全局日志集合
        private static ObservableCollection<LogItem> _logList;
        private static ConsoleManager _cm;

        public ObservableCollection<LogItem> GetLogList() => _logList;

        //必须在主线程（ui）中才可以操作ObservableCollection
        private readonly SynchronizationContext _syncContext;

        //是否显示在控制台（用于调试程序）
        private readonly bool _isShowConsole;
        private readonly int _maxLogCount;

        public LogService(bool isShowConsole = false, int maxLogCount = 1000)
        {
            _logList = new ObservableCollection<LogItem>();
            _logList.CollectionChanged += OnCollectionChanged;
            _isShowConsole = isShowConsole;
            _maxLogCount = maxLogCount;
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
            if (!isShowConsole) return;
            _cm = new ConsoleManager();
            _cm.ShowConsole();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;
            // 使用调度器来推迟集合修改操作
            //删除第一个元素 注意不能使用同步方法操作，因为不支持在OnCollectionChanged方法过程中操作Collection
            if (_logList.Count >= _maxLogCount)
            {
                _syncContext.Post(_ => { _logList.RemoveAt(0); }, null);
            }
        }

        private static string GetCurrentTime() => DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        private void AddLog(LogItem item)
        {
            //相当于App.Current.Dispatcher.InvokeAsync
            _syncContext.Post(_ => { _logList.Add(item); }, null);
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

        ~LogService()
        {
            Release();
        }
    }
}