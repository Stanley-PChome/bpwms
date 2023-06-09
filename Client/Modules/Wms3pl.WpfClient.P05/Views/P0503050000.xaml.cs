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
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P05.Views
{
    /// <summary>
    /// P0503050000.xaml 的互動邏輯
    /// </summary>
    public partial class P0503050000 : Wms3plUserControl
    {
        public P0503050000()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var row = (sender as Button).DataContext;
            if (row != null) //取得按鈕按下那一筆資料列資料
            {
                var win = new P0503040100("1") { Owner = Wms3plViewer.GetWindow(this) };
                win.Vm.DcCode = Vm.DcCode;
                win.Vm.CalNo = Vm.SelectedMainItem.CAL_NO;
                win.Vm.StartLoad();

                if (!string.IsNullOrEmpty(win.Vm.CalNo))
                {
                    win.ShowDialog();
                }
            }
        }
    }
}
