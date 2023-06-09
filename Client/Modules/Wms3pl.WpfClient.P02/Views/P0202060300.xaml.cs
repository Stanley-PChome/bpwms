using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.Datas.F02;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
    /// <summary>
    /// P0202060300.xaml 的互動邏輯
    /// </summary>
    public partial class P0202060300 : Wms3plWindow
    {
        String TYPE_CODE;
        public P0202060300(F0205 f0205data)
        {
            InitializeComponent();
            TYPE_CODE = f0205data.TYPE_CODE;
            if (f0205data.TYPE_CODE == "A")
            {
                txtContainerCode.Visibility = Visibility.Visible;
                txtContainerCode2.Visibility = Visibility.Collapsed;
                //SetFocusedElement(txtContainerCode);
            }
            else
            {
                txtContainerCode.Visibility = Visibility.Collapsed;
                txtContainerCode2.Visibility = Visibility.Visible;
                //SetFocusedElement(txtContainerCode2);

            }
            Vm.SetInitValue(f0205data);

            Vm.DoFocusContanerCode = () =>
            {
                if (f0205data.TYPE_CODE == "A")
                    SetFocusedElement(txtContainerCode, true);
                else
                    SetFocusedElement(txtContainerCode2);
            };

            Vm.DoCloseAction = () => { this.Close(); };
        }

        private void txtQty_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtbox = sender as TextBox;
            CheckQTYValue(txtbox.Text);
        }

        private void TxtContainerCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SetFocusedElement(txtItemQty, true);
        }

        private void TxtItemQty_KeyDown(object sender, KeyEventArgs e)
        {
            var txtbox = sender as TextBox;
            if (e.Key == Key.Enter && txtbox.Name == "txtItemQty")
                Vm.AddCommand.Execute(null);

            var keycode = (int)e.Key;
            if (!((keycode >= 34 && keycode <= 43) || (keycode >= 74 && keycode <= 83)))
            {
                e.Handled = true;
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Vm.DeleteCommand.Execute(null);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (CheckQTYValue(Vm.ItemQty.ToString()))
                Vm.AddCommand.Execute(null);
        }

        private Boolean CheckQTYValue(String Value)
        {
            int tmpValue;
            if (!int.TryParse(Value, out tmpValue))
            {
                Vm.ShowWarningMessage("放入數量請輸入正確的數值");
                return false;
            }
            if (Vm.IsNeedCheckQTY(tmpValue) && tmpValue < 1)
            {
                Vm.ShowWarningMessage("放入數量請輸入大於0數值");
                return false;
            }

            return true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (TYPE_CODE == "A")
                SetFocusedElement(txtContainerCode);
            else
                SetFocusedElement(txtContainerCode2);
            this.Show();
        }

        private void txtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            Vm.UpdateMemoryF0205A_QTY();
        }
    }
}
