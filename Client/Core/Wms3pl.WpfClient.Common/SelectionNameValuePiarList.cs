using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common
{
    public class SelectionNameValuePiarList<TItem, TValue> : SelectionList<TItem, List<NameValuePair<TValue>>>
    {
        public SelectionNameValuePiarList()
        {
        }
        public SelectionNameValuePiarList(IEnumerable<TItem> items, Func<TItem, string> funcName, Func<TItem, TValue> funcValue)
        {
            foreach (var item in items)
            {
                var itemSource = new List<NameValuePair<TValue>> { new NameValuePair<TValue>(funcName.Invoke(item), funcValue.Invoke(item)) };
                this.Add(new SelectionItem<TItem, List<NameValuePair<TValue>>>(item, itemSource));
            }
        }
    }
}
