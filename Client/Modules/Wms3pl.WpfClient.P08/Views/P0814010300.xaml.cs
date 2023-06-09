using System;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.Views
{
    /// <summary>
    /// P0814010300.xaml 的互動邏輯
    /// </summary>
    public partial class P0814010300 : Wms3plWindow
    {
        String _dcCode, _gupCode, _custCode, _WmsOrdNo, _ContainerCode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="WmsOrdNo">出貨單號</param>
        /// <param name="ContainerCode">容器條碼</param>
        public P0814010300(String dcCode, String gupCode, String custCode, String WmsOrdNo, String ContainerCode)
        {
            InitializeComponent();
            Vm.OnSetFocuseContainerCode = () => { SetFocusedElement(txtContainerCode, true); };
            Vm.OnWindowsClose = o => {
                //ScanResult = o;
                DialogResult = o;
                this.Close(); };
            Vm.DataInit(dcCode, gupCode, custCode, WmsOrdNo, ContainerCode);
        }

        private void txtContainerCode_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                Vm.CheckCommand.Execute(null);
            }
        }

        //public Boolean ScanResult { get; set; }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetFocusedElement(txtContainerCode);
        }

    }
}
