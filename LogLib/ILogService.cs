using System.Collections.ObjectModel;

namespace LogLib
{
    public interface ILogService
    {
       //普通
        public void Info(string message);
        //警告
        public void Warn(string message);
        //错误
        public void Error(string message);
        //debug
        public void Debug(string message);
        //释放
        public void Release();
        public ObservableCollection<LogItem> GetLogList();
    }
}