using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common
{
    public class SelectionList<TItem, TItemSource> : ObservableCollection<SelectionItem<TItem, TItemSource>>
    {
        public SelectionList() : base() { }

        /// <summary>
        /// 若預先設定 IsSelected 等屬性，可直接帶 SelectionItem 型別初始化
        /// </summary>
        /// <param name="selectionItems"></param>
        public SelectionList(IEnumerable<SelectionItem<TItem, TItemSource>> selectionItems)
            : base(selectionItems)
        {
        }
    }
}
