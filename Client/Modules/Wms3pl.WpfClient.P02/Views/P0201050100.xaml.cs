using System.Windows.Input;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
    /// <summary>
    /// P0201050100.xaml 的互動邏輯
    /// </summary>
    public partial class P0201050100 : Wms3plWindow
    {
        public P0201050100()
        {
            InitializeComponent();
            Vm.OnSearchEmpIDNotFound = () =>
            {
                SetFocusedElement(txtEmpID); 
                txtEmpID.SelectAll();
            };
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtEmpID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                Vm.SetEmpIDInfo();
            }
            else
            {
                return;
            }
        }

        private void dgRecordDeatil_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //foreach (var item in dgRecordDeatil.)
            //{

            //}
        }
    }
}
