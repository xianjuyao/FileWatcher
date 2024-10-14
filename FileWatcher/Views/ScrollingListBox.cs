using System.Collections.Specialized;
using System.Windows.Controls;

namespace FileWatcher.Views
{
    public class ScrollingListBox : ListBox
    {
        //重写实现自动下滑动功能
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;
            var newItemsCount = e.NewItems.Count;
            if (newItemsCount > 0)
            {
                ScrollIntoView(e.NewItems[newItemsCount - 1]);
                SelectedValue = e.NewItems[newItemsCount - 1];
            }

            base.OnItemsChanged(e);
        }
    }
}