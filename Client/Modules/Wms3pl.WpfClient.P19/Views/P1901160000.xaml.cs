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

namespace Wms3pl.WpfClient.P19.Views
{
    /// <summary>
    /// P1901160000.xaml 的互動邏輯
    /// </summary>
    public partial class P1901160000 : Wms3plUserControl
    {

        public P1901160000()
        {
            InitializeComponent();
            Vm.AddAction += AddDetailAction;
        }

        private void AddDetailAction()
        {
            if(dgItemList.SelectedItem!=null)
                dgItemList.ScrollIntoView(dgItemList.SelectedItem);
        }

        private void btnSerachStore_Click(object sender, RoutedEventArgs e)
        {
            if(Vm.SelectedData != null)
            {
                Vm.GetF1910Data();
            }
        }

        private void DELV_NOEditTxb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Vm.IsChinceString(this.DELV_NOEditTxb.Text))
                this.DELV_NOEditTxb.Text = "";
        }

        private void DRIVER_IDEditTxb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Vm.IsChinceString(this.DRIVER_IDEditTxb.Text))
                this.DRIVER_IDEditTxb.Text = "";
        }

        private void PACK_FIELDEditTxb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Vm.IsChinceString(this.PACK_FIELDEditTxb.Text))
                this.PACK_FIELDEditTxb.Text = "";
        }
    }
}
