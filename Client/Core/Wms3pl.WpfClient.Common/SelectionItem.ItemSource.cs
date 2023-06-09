using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common
{
    public class SelectionItem<TItem, TItemSource> : SelectionItem<TItem>
    {
        private TItemSource _itemSource;

        public TItemSource ItemSource
        {
            get { return _itemSource; }
            set
            {
                _itemSource = value;
                RaisePropertyChanged("ItemSource");
            }
        }

        public SelectionItem(TItem item, bool isSelected)
            : base(item, isSelected)
        {
        }

        public SelectionItem(TItem item, TItemSource source)
            : base(item)
        {
            this.ItemSource = source;
        }

        public SelectionItem(TItem item)
            : base(item)
        {
        }
    }
}
