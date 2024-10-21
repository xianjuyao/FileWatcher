using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileWatcher.Models;
using FileWatcher.Services;
using LogLib;
using System.Windows.Forms;

namespace FileWatcher.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private static System.Threading.Timer _timer;
        private string _lastFilePath;
        private const string NotInitString = "Target directory or source directory could not be initialized";
        private static Dictionary<string, DateTime> fileWriteTimes = new Dictionary<string, DateTime>();
        private string _sourceFilePath;
        private string _targetFilePath;
        private ObservableCollection<LogItem> _logList;
        private ObservableCollection<FileOperationTimes> _fileOperationTimes;
        private ObservableCollection<FileOperationLog> _fileOperationLogs;

        //服务相关
        private LogService Log;
        private IDialogService dialogService;
        private readonly IDbService _dbService;
        private FileSystemWatcher fileSystemWatcher;

        private bool _isWatching;

        //分页相关
        private int _operationLogCurrentPage = 1;
        private int _operationLogPageSize = 10;
        private int _operationLogTotalPage = 0;
        private int _operationLogTotalCount = 0;
        private int _operationTimesCurrentPage = 1;
        private int _operationTimesPageSize = 5;
        private int _operationTimesTotalPage = 0;
        private int _operationTimesTotalCount = 0;

        //延迟时间
        private static readonly int dueTime = 500;
        public RelayCommand LoadSourceFilePathCommand { get; }
        public RelayCommand LoadTargetFilePathCommand { get; }
        public RelayCommand StartListeningCommand { get; }
        public RelayCommand StopListeningCommand { get; }
        public RelayCommand LoadFirstPageFileOperationLogCommand { get; }
        public RelayCommand LoadFileOperationLogNextPageCommand { get; }
        public RelayCommand LoadFileOperationLogPreviousCommand { get; }
        public RelayCommand LoadLastPageFileOperationLogCommand { get; }
        public RelayCommand LoadFirstPageFileOperationTimesCommand { get; }
        public RelayCommand LoadFileOperationTimesNextPageCommand { get; }
        public RelayCommand LoadFileOperationTimesPreviousCommand { get; }
        public RelayCommand LoadLastPageFileOperationTimesCommand { get; }

        public string SourceFilePath
        {
            get => _sourceFilePath;
            set => SetProperty(ref _sourceFilePath, value);
        }

        public string TargetFilePath
        {
            get => _targetFilePath;
            set => SetProperty(ref _targetFilePath, value);
        }


        private bool IsWatching
        {
            get => _isWatching;
            set
            {
                _isWatching = value;
                if (fileSystemWatcher == null) return;
                fileSystemWatcher.EnableRaisingEvents = _isWatching;
            }
        }

        public int OperationLogCurrentPage
        {
            get => _operationLogCurrentPage;
            set => SetProperty(ref _operationLogCurrentPage, value);
        }

        public int OperationLogPageSize
        {
            get => _operationLogPageSize;
            set => SetProperty(ref _operationLogPageSize, value);
        }


        public int OperationLogTotalPage
        {
            get => _operationLogTotalPage;
            set => SetProperty(ref _operationLogTotalPage, value);
        }

        public int OperationTimesCurrentPage
        {
            get => _operationTimesCurrentPage;
            set => SetProperty(ref _operationTimesCurrentPage, value);
        }

        public int OperationTimesPageSize
        {
            get => _operationTimesPageSize;
            set => SetProperty(ref _operationTimesPageSize, value);
        }

        public int OperationTimesTotalPage
        {
            get => _operationTimesTotalPage;
            set => SetProperty(ref _operationTimesTotalPage, value);
        }

        public int OperationLogTotalCount
        {
            get => _operationLogTotalCount;
            set => SetProperty(ref _operationLogTotalCount, value);
        }

        public int OperationTimesTotalCount
        {
            get => _operationTimesTotalCount;
            set => SetProperty(ref _operationTimesTotalCount, value);
        }

        public ObservableCollection<LogItem> LogList
        {
            get => _logList;
            set => SetProperty(ref _logList, value);
        }

        public ObservableCollection<FileOperationTimes> FileOperationTimes
        {
            get => _fileOperationTimes;
            set => SetProperty(ref _fileOperationTimes, value);
        }

        public ObservableCollection<FileOperationLog> FileOperationLogs
        {
            get => _fileOperationLogs;
            set => SetProperty(ref _fileOperationLogs, value);
        }

        //初始化
        public MainViewModel(IDialogService dialogService, LogService logService, IDbService dbService)
        {
            Log = logService;
            this.dialogService = dialogService;
            _dbService = dbService;
            LogList = Log.GetLogList();
            Log.Info("Application Init Complete");
            Log.Debug($"main thread id is {Thread.CurrentThread.ManagedThreadId}");
            fileSystemWatcher = new FileSystemWatcher();
            FileOperationLogs = new ObservableCollection<FileOperationLog>();
            FileOperationTimes = new ObservableCollection<FileOperationTimes>();
            fileSystemWatcher.Created += OnFileSystemWatcherCreated;
            fileSystemWatcher.Changed += OnFileSystemWatcherChanged;
            fileSystemWatcher.Deleted += OnFileSystemWatcherDeleted;
            fileSystemWatcher.Renamed += OnFileSystemWatcherRenamed;
            LoadSourceFilePathCommand = new RelayCommand(OnLoadSourceFilePath);
            LoadTargetFilePathCommand = new RelayCommand(OnLoadTargetFilePath);
            StartListeningCommand = new RelayCommand(OnStartListening);
            StopListeningCommand = new RelayCommand(OnStopListening);
            LoadFileOperationLogNextPageCommand = new RelayCommand(ToFileOperationLogNextPage);
            LoadFileOperationLogPreviousCommand = new RelayCommand(ToFileOperationLogPreviousPage);
            LoadFirstPageFileOperationLogCommand = new RelayCommand(ToFirstOperationLogPage);
            LoadLastPageFileOperationLogCommand = new RelayCommand(ToLastOperationLogPag);
            LoadFirstPageFileOperationTimesCommand = new RelayCommand(ToFirstFileOperationTimesPage);
            LoadFileOperationTimesPreviousCommand = new RelayCommand(ToFileOperationTimesPreviousPage);
            LoadFileOperationTimesNextPageCommand = new RelayCommand(ToFileOperationTimesNextPage);
            LoadLastPageFileOperationTimesCommand = new RelayCommand(ToLastFileOperationTimesPage);
            InitData();
        }

        private async void InitData()
        {
            await LoadFileOperationLogsAsync();
            await LoadFileOperationTimesAsync();
        }

        private async void ToFileOperationTimesPreviousPage()
        {
            if (OperationTimesCurrentPage <= 1) return;
            OperationTimesCurrentPage--;
            await LoadFileOperationTimesAsync();
        }

        private async void ToFileOperationTimesNextPage()
        {
            if (OperationTimesCurrentPage >= OperationTimesTotalPage) return;
            OperationTimesCurrentPage++;
            await LoadFileOperationTimesAsync();
        }

        private async void ToFirstFileOperationTimesPage()
        {
            if (OperationTimesCurrentPage == OperationTimesTotalPage) return;
            OperationTimesCurrentPage = 1;
            await LoadFileOperationTimesAsync();
        }

        private async void ToLastFileOperationTimesPage()
        {
            if (OperationTimesCurrentPage == OperationTimesTotalPage) return;
            OperationTimesCurrentPage = 1;
            await LoadFileOperationTimesAsync();
        }

        private async void ToLastOperationLogPag()
        {
            if (OperationLogCurrentPage == OperationLogTotalPage) return;
            OperationLogCurrentPage = OperationLogTotalPage;
            await LoadFileOperationLogsAsync();
        }

        private async void ToFirstOperationLogPage()
        {
            if (OperationLogCurrentPage == 1) return;
            OperationLogCurrentPage = 1;
            await LoadFileOperationLogsAsync();
        }
        //文件操作日志下一页

        private async void ToFileOperationLogNextPage()
        {
            //去后一页
            if (OperationLogCurrentPage >= OperationLogTotalPage) return;
            OperationLogCurrentPage++;
            await LoadFileOperationLogsAsync();
        }

        //文件操作日志上一页
        private async void ToFileOperationLogPreviousPage()
        {
            if (OperationLogCurrentPage <= 1) return;
            OperationLogCurrentPage--;
            await LoadFileOperationLogsAsync();
        }

        private void OnFileSystemWatcherRenamed(object sender, RenamedEventArgs e)
        {
            HandleError(() =>
            {
                var message = $@"File {e.OldFullPath} renamed to {e.FullPath}";
                Log.Info(message);
                _dbService.InsertFileOperationLogAsync(new FileOperationLog(OperationType.renamed, message));
                InitData();
            });
        }

        private void OnFileSystemWatcherDeleted(object sender, FileSystemEventArgs e)
        {
            HandleError(() =>
            {
                var message = $@"File {e.FullPath} deleted";
                Log.Info(message);
                _dbService.InsertFileOperationLogAsync(new FileOperationLog(OperationType.deleted, message));
                InitData();
            });
        }

        //文件改变事件（复制粘贴修改等）
        /**
         *会多次触发，必须利用timer定时器实现延迟调用执行到最后一次
         *思路如下：
         * 1.第一次调用OnChanged方法时定义一个定时器延迟0.5s执行回调方法（需要执行的操作）
         * 2.当OnChanged方法第二次或者多次执行的时候，会刷新（改变）定时器的延迟执行时间
         * 3.当OnChanged方法完成调用时，定时器不再刷新延迟执行时间，就会直接调用1中的回调函数。
         *
         */
        private void OnFileSystemWatcherChanged(object sender, FileSystemEventArgs e)
        {
            _lastFilePath = e.FullPath;
            if (_timer == null)
            {
                _timer = new System.Threading.Timer(OnTimerElapsed, null, dueTime, Timeout.Infinite);
            }
            else
            {
                _timer.Change(dueTime, Timeout.Infinite);
            }
        }

        //定时器回调
        private void OnTimerElapsed(object state)
        {
            try
            {
                var sourceFileInfo = new FileInfo(_lastFilePath);
                var targetFileInfo = new FileInfo(Path.Combine(_targetFilePath, sourceFileInfo.Name));
                //执行文件操作
                if (sourceFileInfo.Extension != ".txt") return;
                //开始复制文件
                File.Copy(_lastFilePath, targetFileInfo.FullName, true);
                var message = $"{sourceFileInfo.FullName} was changed copy to {targetFileInfo.FullName}.";
                //插入数据库
                 Log.Info(message);
                _dbService.InsertFileOperationLogAsync(new FileOperationLog(OperationType.changed, message));
                InitData();
            }
            catch (Exception ex)
            {
                    Log.Error(ex.Message);
            }
            finally
            {
                _timer?.Dispose();
                _timer = null;
            }
        }

        //文件创建事件（复制粘贴等）
        private void OnFileSystemWatcherCreated(object sender, FileSystemEventArgs e)
        {
            HandleError(() =>
            {
                if (!CheckPathInit()) return;
                var targetFilePath = Path.Combine(_targetFilePath, e.Name);
                var message = $"Copying file {e.FullPath} into {targetFilePath}";
                Log.Info(message);
                _dbService.InsertFileOperationLogAsync(new FileOperationLog(OperationType.created, message));
                InitData();
                File.Copy(e.FullPath, targetFilePath, true);
            });
        }

        //从文件中读取text，并解析至界面显示（该方法在异步线程中调用，不用使用异步操作）
        // private void ReadTextFromFile(string path)
        // {
        //     try
        //     {
        //         using var reader = new StreamReader(path);
        //         App.Current.Dispatcher.InvokeAsync(() => PersonList.Clear());
        //         while (reader.ReadLine() is { } line)
        //         {
        //             var values = line.Split(',');
        //             var person = new Person(values[0], values[1], values[2], values[3], values[4]);
        //             App.Current.Dispatcher.InvokeAsync(() => PersonList.Add(person));
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         App.Current.Dispatcher.InvokeAsync(() => Log.Error($"{ex.Message}"));
        //     }
        //     HandleError(() =>
        //     {
        //         
        //         
        //     });
        // }

        private bool IsPathInit()
        {
            return !(string.IsNullOrEmpty(SourceFilePath) && string.IsNullOrEmpty(TargetFilePath));
        }

        private void OnLoadSourceFilePath()
        {
            var path = dialogService.ShowOpenFileDialog();
            if (!string.IsNullOrEmpty(path))
            {
                Log.Info($"Loading source image from: {path}");
                SourceFilePath = path;
            }
        }

        private void OnLoadTargetFilePath()
        {
            var path = dialogService.ShowOpenFileDialog();
            if (string.IsNullOrEmpty(path)) return;
            Log.Info($"Loading target image from: {path}");
            TargetFilePath = path;
        }

        //开始监听文件改变
        private void OnStartListening()
        {
            if (!CheckPathInit()) return;
            if (fileSystemWatcher.EnableRaisingEvents)
            {
                Log.Warn("Already listening.....");
                return;
            }

            Log.Info("Starting Listening");
            fileSystemWatcher.Path = SourceFilePath;
            IsWatching = true;
        }

        private void OnStopListening()
        {
            if (!fileSystemWatcher.EnableRaisingEvents)
            {
                Log.Warn("Already Stopped Listening.....");
                return;
            }

            IsWatching = false;
            Log.Info("Stopping Listening");
        }

        //检查目标路径和源路径是否初始化
        private bool CheckPathInit()
        {
            //初始化路径
            if (IsPathInit()) return true;
            //显示信息
            Log.Error(NotInitString);
            dialogService.ShowMessage(NotInitString);
            return false;
        }

        //通用异常处理
        private void HandleError(Action function)
        {
            try
            {
                function();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }


        private async Task LoadFileOperationLogsAsync()
        {
            var logs = await _dbService.GetFileOperationItemsAsync(OperationLogCurrentPage, OperationLogPageSize);
            App.Current.Dispatcher.Invoke(() => {
                FileOperationLogs.Clear();
                foreach (var log in logs)
                {
                    FileOperationLogs.Add(log);
                }
            });
            OperationLogTotalCount = await _dbService.GetFileOperationItemCountsAsync();
            OperationLogTotalPage = (OperationLogTotalCount + OperationLogPageSize - 1) / OperationLogPageSize;
        }


        private async Task LoadFileOperationTimesAsync()
        {
            var times = await _dbService.GetFileOperationTimesAsync(OperationTimesCurrentPage, OperationTimesPageSize);
            App.Current.Dispatcher.Invoke(() => {
                FileOperationTimes.Clear();
                foreach (var time in times)
                {
                    FileOperationTimes.Add(time);
                }
            });
            OperationTimesTotalCount = await _dbService.GetFileOperationTimeCountsAsync();
            OperationTimesTotalPage = (OperationTimesTotalCount + OperationTimesPageSize - 1) / OperationTimesPageSize;
        }
    }
}