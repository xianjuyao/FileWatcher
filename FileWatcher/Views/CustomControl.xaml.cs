using System.Windows;
using System.Windows.Controls;

namespace FileWatcher.Views
{
    public partial class CustomControl : UserControl
    {
        public static readonly DependencyProperty MyHeightProperty =
            DependencyProperty.Register("MyHeight", typeof(int), typeof(CustomControl),
                new PropertyMetadata(0));

        public int MyHeight
        {
            get { return (int)GetValue(MyHeightProperty); }
            set { SetValue(MyHeightProperty, value); }
        }

        public CustomControl()
        {
            InitializeComponent();
        }
    }
}