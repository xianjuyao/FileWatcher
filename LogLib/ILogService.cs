using System.Collections.ObjectModel;

namespace LogLib
{
    public interface ILogService
    {
       //普通
        void Info(string message);
        //警告
        void Warn(string message);
        //错误
        void Error(string message);
        //debug
        void Debug(string message);
        //释放
        void Release();
        ObservableCollection<LogItem> GetLogList();
    }
}