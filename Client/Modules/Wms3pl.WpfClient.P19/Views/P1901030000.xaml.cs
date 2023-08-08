using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.P19.ViewModel;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P19.Views
{
    /// <summary>
    /// P1901030000.xaml 的互動邏輯
    /// </summary>
    public partial class P1901030000 : Wms3plUserControl
    {
        public P1901030000()
        {
            InitializeComponent();
        }

        public bool ShowItem(string gupCode, string custCode, string itemCode)
        {
            Vm._gupCode = gupCode;
            Vm._custCode = custCode;
            Vm.SearchItemCode = itemCode;
            Vm.SearchCommand.Execute(null);

            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            var win = new WinSearchProduct();
            win.GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
            win.CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;
            win.Owner = this.Parent as Window;
            win.ShowDialog();
            if (win.DialogResult.HasValue && win.DialogResult.Value)
            {
                SetSearchData(win.SelectData);
            }
        }

        private void SetSearchData(F1903 f1903)
        {
            if (f1903 != null)
            {
                Vm.SearchItemCode = f1903.ITEM_CODE;
                Vm.SearchItemName = f1903.ITEM_NAME;
            }
            else
            {
                Vm.SearchItemCode = string.Empty;
                Vm.SearchItemName = string.Empty;
            }
        }
    }
}
