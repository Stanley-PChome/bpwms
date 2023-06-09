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

namespace Wms3pl.WpfClient.P02.Views
{
    /// <summary>
    /// P0202060100.xaml 的互動邏輯
    /// </summary>
    public partial class P0202060100 : Wms3plWindow
    {
        String dcCode, gupCode, custCode, RTNo, RTSEQ;

        public P0202060100(String dcCode, String gupCode, String custCode, String RTNo, String RTSEQ)
        {
            InitializeComponent();
            Vm.DoShowP0202060300 += DoShowP0202060300;
            Vm.DoExitWin = () => this.Close();
            this.dcCode = dcCode;
            this.gupCode = gupCode;
            this.custCode = custCode;
            this.RTNo = RTNo;
            this.RTSEQ = RTSEQ;
        }

        private void DoShowP0202060300()
        {
            var win = new P0202060300(Vm.SendToP0202060300Data);
            win.ShowDialog();
            Vm.SetInitValue(dcCode, gupCode, custCode, RTNo, RTSEQ);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Vm.SetInitValue(dcCode, gupCode, custCode, RTNo, RTSEQ))
            {
                Vm.ShowWarningMessage("找不到資料");
                this.Close();
            }
        }
    }
}
