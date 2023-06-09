using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Wms3pl.WpfClient.UILib.Controls
{
  public partial class PinButton : UserControl
  {
    public PinButton()
    {
      InitializeComponent();
      this.Cursor = Cursors.Hand;
      this.Pin.DataContext = this;
      this.UnPin.DataContext = this;
    }

    public static readonly DependencyProperty IsPinnedProperty =
      DependencyProperty.Register("IsPinned", typeof (bool), typeof (PinButton), new PropertyMetadata(default(bool)));

    public bool IsPinned
    {
      get { return (bool) GetValue(IsPinnedProperty); }
      set { SetValue(IsPinnedProperty, value); }
    }

    private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      IsPinned = !IsPinned;
      e.Handled = true;
    }
  }
}
