using System.Windows;

namespace FileWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static readonly DependencyProperty MyHeightProperty =
            DependencyProperty.Register("MyHeight", typeof(string), typeof(MainWindow),
                new PropertyMetadata("test"));

        public string MyHeight
        {
            get { return (string)GetValue(MyHeightProperty); }
            set { SetValue(MyHeightProperty, value); }
        }
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}