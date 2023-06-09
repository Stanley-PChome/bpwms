using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Wms3pl.WpfClient.UILib.Controls
{
  /// <summary>
  /// 當 ComboBox 只有一個選項時，直接選這個選項
  /// </summary>
  public class SelectFirstItemComboBoxBehavior : Behavior<ComboBox>
  {
    protected override void OnAttached()
    {
      base.OnAttached();
      (AssociatedObject.Items as INotifyCollectionChanged).CollectionChanged += HandleCollectionChanged;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      (AssociatedObject.Items as INotifyCollectionChanged).CollectionChanged -= HandleCollectionChanged;
    }

    private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (AssociatedObject.Items.Count == 1)
      {
        AssociatedObject.SelectedIndex = 0;
      }
    } 

  }
}
