using System.ComponentModel;
using System.Data.Services.Client;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.UILib.Views
{
  /// <summary>
  /// Interaction logic for UcProductAutoComplete.xaml
  /// </summary>
  public partial class UcProductAutoComplete : UserControl
  {
    public UcProductAutoComplete()
    {
      InitializeComponent();
      if (!DesignerProperties.GetIsInDesignMode(this))
        acbItemCode.Populating += OnPopulating;
      this.GotFocus += new RoutedEventHandler(UcProductAutoComplete_GotFocus);
    }

    void UcProductAutoComplete_GotFocus(object sender, RoutedEventArgs e)
    {
      var textBox = acbItemCode.Template.FindName("Text", acbItemCode) as TextBox;
      if (textBox != null) textBox.Focus();
    }

    private void OnPopulating(object sender, PopulatingEventArgs e)
    {
      var source = sender as AutoCompleteBox;
			var proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "ProductAutoComplete");
      //rep.SearchByKeyAsync(source.Text, "19",
      //  r =>
      //  {
      //    Action action = delegate { source.ItemsSource = r; source.PopulateComplete();};
      //    Dispatcher.BeginInvoke(action);

      //    Debug.WriteLine(r.Count.ToString());
      //  });
      var results = (from i in proxy.F1903s
                 where i.ITEM_CODE.StartsWith(source.Text) && i.GUP_CODE == ItemGroupCode && i.CUST_CODE == ItemCustCode
                 select i).Take(10).ToList();
      if (results.Count == 1) source.ItemsSource = null;
      else
        source.ItemsSource = results;
    }

    public static readonly DependencyProperty ItemCodeProperty =
      DependencyProperty.Register("ItemCode", typeof (string), typeof (UcProductAutoComplete), new PropertyMetadata(default(string)));

    public string ItemCode
    {
      get { return (string) GetValue(ItemCodeProperty); }
      set { SetValue(ItemCodeProperty, value); }
    }

    public static readonly DependencyProperty ItemGroupCodeProperty =
      DependencyProperty.Register("ItemGroupCode", typeof (string), typeof (UcProductAutoComplete), new PropertyMetadata(default(string)));

    public string ItemGroupCode
    {
      get { return (string) GetValue(ItemGroupCodeProperty); }
      set { SetValue(ItemGroupCodeProperty, value); }
    }

    public static readonly DependencyProperty ItemCustCodeProperty =
      DependencyProperty.Register("ItemCustCode", typeof(string), typeof(UcProductAutoComplete), new PropertyMetadata(default(string)));
    public string ItemCustCode
    {
      get { return (string)GetValue(ItemCustCodeProperty); }
      set { SetValue(ItemCustCodeProperty, value); }
    }


    }
}
