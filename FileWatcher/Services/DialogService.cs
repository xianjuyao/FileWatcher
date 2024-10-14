using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace FileWatcher.Services
{
    public class DialogService : IDialogService
    {
        private readonly FolderBrowserDialog _folderBrowserDialog = new FolderBrowserDialog
            { ShowNewFolderButton = true };

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public string ShowOpenFileDialog()
        {
            if (_folderBrowserDialog == null) return string.Empty;
            var result = _folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                return _folderBrowserDialog.SelectedPath;
            }

            ShowMessage("Please select a folder.");
            return string.Empty;
        }

        public void CloseFileDialog()
        {
            _folderBrowserDialog.Dispose();
        }

        ~DialogService()
        {
            CloseFileDialog();
        }
    }
}