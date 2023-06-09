using System.Windows;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P19.Views
{
    /// <summary>
    /// P1901810100.xaml 的互動邏輯
    /// </summary>
    public partial class P1901810000 : Wms3plUserControl
    {
        public P1901810000()
        {
            InitializeComponent();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Vm.PPKAreaValueChangeCommand.Execute("PK_AREA");
        }
    }
}
