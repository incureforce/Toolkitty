using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using ToolKitty;

namespace System.Windows
{
    public class UICollection<T> : ObservableCollection<T>
    {
        public UICollection()
        {
            View = GO.Dispatch(delegate {
                return CollectionViewSource.GetDefaultView(this);
            });
        }

        public ICollectionView View { get; }
    }
}
