using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.Common
{
  public class AddedPropertyItemList<T, S> : ObservableCollection<AddedPropertyItem<T, S>> 
  {
    public AddedPropertyItemList(IEnumerable<T> list) :
      base(ToItemEnumerable(list))
    {
    }

    private static IEnumerable<AddedPropertyItem<T, S>> ToItemEnumerable(IEnumerable<T> items)
    {
      List<AddedPropertyItem<T, S>> list = new List<AddedPropertyItem<T, S>>();
      foreach (var item in items)
        list.Add(new AddedPropertyItem<T, S>(item));
      return list;
    }
  }

  public static class AddedPropertyItemListExtension
  {
    public static AddedPropertyItemList<T, S> ToAddedPropertyItemList<T, S>(this IEnumerable<T> items)
    {
      AddedPropertyItemList<T, S> list = new AddedPropertyItemList<T, S>(items);
      return list;
    }
  }
}
