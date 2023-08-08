using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P19.Views
{
    /// <summary>
    /// P1901920000.xaml 的互動邏輯
    /// </summary>
    public partial class P1901920000 : Wms3plUserControl
    {
        private string tmpBefEditCellNum;
        public P1901920000()
        {
            InitializeComponent();
        }

        private void BtnDeleteCell_OnClick(object sender, RoutedEventArgs e)
        {
            Vm.DeleteItmeCommand.Execute(null);
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex("^[1-9]?[0-9]*$");
            var txt = ((TextBox)sender).Text + e.Text;

            e.Handled = !re.IsMatch(txt);
        }

        private void txtCellNum_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Vm.UserOperateMode != OperateMode.Edit)
                return;
            TextBox txtbox = sender as TextBox;
            if (tmpBefEditCellNum != txtbox.Text)
                Vm.ChangeModifyModeToC();
        }

        private void txtCellNum_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Vm.UserOperateMode != OperateMode.Edit)
                return;
            TextBox txtbox = sender as TextBox;
            tmpBefEditCellNum = txtbox.Text;
        }

    }
}
