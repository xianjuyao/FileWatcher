namespace FileWatcher.Services
{
    public interface IDialogService
    {
        void ShowMessage(string message);
        string ShowOpenFileDialog();
        void CloseFileDialog();
    }
}