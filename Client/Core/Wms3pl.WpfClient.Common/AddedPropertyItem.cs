using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Wms3pl.WpfClient.Common
{
  public class AddedPropertyItem<T, S> : INotifyPropertyChanged
  {
   
    public AddedPropertyItem(T item, S addedProp)
    {
      _item = item;
      _AddedProp = addedProp;
    }

    public AddedPropertyItem(T item)
    {
      this._item = item;
    }


    /// <summary>
    /// The <see cref="AddedProp" /> property's name.
    /// </summary>
    public const string AddedPropPropertyName = "AddedProp";

    private S _AddedProp = default(S);

    /// <summary>
    /// Gets the IsSelected property.
    /// </summary>
    public S AddedProp
    {
      get
      {
        return _AddedProp;
      }

      set
      {
        if ((_AddedProp != null) &&_AddedProp.Equals(value))
          return;

        _AddedProp = value;
        RaisePropertyChanged(AddedPropPropertyName);
      }
    }

    private T _item;

    public T Item
    {
      get { return _item; }
      set { _item = value; }
    }



    public event PropertyChangedEventHandler PropertyChanged = delegate { };
    

    private void RaisePropertyChanged(string propertyName)
    {
      PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
