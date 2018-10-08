using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace System.Windows
{
    public class UICollection<T> : ObservableCollection<T>
    {
        public UICollection()
        {
            View = CollectionViewSource.GetDefaultView(this);
        }

        public ICollectionView View { get; }
    }
}
