using System.Windows;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.Views
{
    /// <summary>
    /// P0814010200.xaml 的互動邏輯
    /// </summary>
    public partial class P0814010200 : Wms3plWindow
    {
        string _dcCode = null;
        string _gupCode = null;
        string _custCode = null;
        string _wmsOrdNo = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="wmsOrdNo">出貨單號</param>
        public P0814010200(string dcCode = null, string gupCode = null, string custCode = null, string wmsOrdNo = null)
        {
            InitializeComponent();
            Vm.OnCloseClick = () => this.Close();
            _dcCode = dcCode;
            _gupCode = gupCode;
            _custCode = custCode;
            _wmsOrdNo = wmsOrdNo;
        }

        private void Windows_Loaded(object sender, RoutedEventArgs e)
        {
            Vm.SetInitValue(_dcCode, _gupCode, _custCode, _wmsOrdNo);
        }

        private void txtWmsOrdNo_LostFocus(object sender, RoutedEventArgs e)
        {
            Vm.WmsOrdNo = Vm.WmsOrdNo?.ToUpper() ?? null;
        }

        private void txtWmsOrdNo_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Vm.SearchCommand.Execute(null);
        }
    }
}
