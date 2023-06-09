using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
    /// <summary>
    /// Interaction logic for P0201050000.xaml
    /// </summary>
    public partial class P0201050000 : Wms3plWindow
    {
        /// <summary>
        /// txtMemo編輯前內容，判斷是否要觸發後續內容用
        /// </summary>
        private string strBeforeEdittxtMemo;
        /// <summary>
        /// txtBOX_CNT編輯前內容，判斷是否要觸發後續內容用
        /// </summary>
        private string strBeforeEdittxtBOX_CNT;
        /// <summary>
        /// txtReceiptBoxCount資料修改前內容，輸入異常時還原用
        /// </summary>
        private string strBeforeEditReceiptBoxCount;
        /// <summary>
        /// 是否還原內容狀態(避免數值還原後還跑去執行後續內容)
        /// </summary>
        private bool IsRestoreValueMode = false;

        public P0201050000()
        {
            InitializeComponent();
            SetFocusedElement(DcComboBox);
            Vm.OnCheckEmpIDComplete = () =>
            {
                SetFocusedElement(TxtFreightNo);
            };
            Vm.OnFocusEmpID = () =>
            {
                TxtEmpID.IsFoucus = true;
            };
            Vm.OnInsertScanCargoDataComplete = () =>
            {
                DispatcherAction(() =>
                {
                    dgScanCargo.Items.MoveCurrentToLast();
                    if (dgScanCargo.Items.CurrentItem != null)
                    {
                        dgScanCargo.ScrollIntoView(dgScanCargo.Items.CurrentItem);
                        dgScanCargo.SelectedItem = dgScanCargo.Items.CurrentItem;
                    }
                    SetFocusedElement(TxtFreightNo, true);
                });

            };

            Vm.OnInsertScanReceiptDataComplete = () =>
            {
                DispatcherAction(() =>
                {

                    dgScanReceipt.Items.MoveCurrentToLast();
                    if (dgScanReceipt.Items.CurrentItem != null)
                    {
                        dgScanReceipt.ScrollIntoView(dgScanReceipt.Items.CurrentItem);
                        dgScanReceipt.SelectedItem = dgScanReceipt.Items.CurrentItem;
                    }

                    SetFocusedElement(txtReceiptFreightNo, true);
                });
            };
        }

        //private void TxtEmpID_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        if (string.IsNullOrWhiteSpace(TxtEmpID.Text))
        //        {
        //            Vm.EmpName = "";
        //            return;
        //        }
        //        if (!Vm.SetEmpIDInfo())
        //            return;
        //        Vm.LoadTodayUncheckedCargo(true);
        //        Vm.IsQueryMode = false;
        //        Vm.TabControlSelectedIndex = 0;
        //        SetFocusedElement(TxtFreightNo, true);
        //    }
        //    else
        //    {
        //        return;
        //    }
        //}

        private void btnHistorySearch_Click(object sender, RoutedEventArgs e)
        {
            var win = new P0201050100();
            win.ShowDialog();
        }

        private void TxtFreightNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Vm.InsertScanCargoDataCommand.Execute(null);
            }
            else
                return;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetFocusedElement(TxtEmpID);
        }

        private void txtMemo_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox))
                return;
            var tmpTextbox = sender as TextBox;

            strBeforeEdittxtMemo = tmpTextbox.Text;
        }

        private void txtMemo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox))
                return;
            var tmpTextbox = sender as TextBox;
            if (strBeforeEdittxtMemo == tmpTextbox.Text)
                return;
            Vm.UpdateScanCargoMemoDataCommand.Execute(tmpTextbox.Text);
        }

        private void txtBOX_CNT_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox))
                return;
            var tmpTextbox = sender as TextBox;

            strBeforeEdittxtBOX_CNT = tmpTextbox.Text;

        }

        private void txtBOX_CNT_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox))
                return;
            var tmpTextbox = sender as TextBox;
            if (strBeforeEdittxtBOX_CNT == tmpTextbox.Text)
                return;
            string msg = null;
            short tmpValue = 0;
            if (!short.TryParse(tmpTextbox.Text, out tmpValue))
                msg = "請輸入數值";
            else if (tmpValue <= 0)
                msg = "請輸入大於0數值";

            if (!string.IsNullOrWhiteSpace(msg))
            {
                Vm.ShowWarningMessage(msg);
                tmpTextbox.Text = strBeforeEdittxtBOX_CNT;
                return;
            }

            Vm.UpdateScanCargoBoxCntDataCommand.Execute(Int16.Parse(tmpTextbox.Text));
        }

        private void txtReceiptBoxCount_LostFocus(object sender, RoutedEventArgs e)
        {
            string msg = null;
            if (!(sender is TextBox))
                return;
            var tmpTextbox = sender as TextBox;
            long testConveter;
            short tmpValue = 0;

            if (tmpTextbox.Text == strBeforeEditReceiptBoxCount)
                return;
            else if (!long.TryParse(tmpTextbox.Text, out testConveter))
                msg = "請輸入數值";
            else if (testConveter > short.MaxValue)
                msg = "輸入之數值過大";
            else if (testConveter <= 0)
                msg = "請輸入大於0數值";
            else if (!short.TryParse(tmpTextbox.Text, out tmpValue))
                msg = "請輸入數值"; //避免user輸入過大的數值，因此上面先用比較大的long去測試轉換，才實際轉成後續要使用的short
            else if (tmpValue > Vm.SelectScanReceiptData.CHECK_BOX_CNT)
                msg = "不可輸入大於核對箱數之數值";

            if (!string.IsNullOrWhiteSpace(msg))
            {
                Vm.ShowWarningMessage(msg);
                tmpTextbox.Text = strBeforeEditReceiptBoxCount;
                IsRestoreValueMode = true;
                return;
            }
            var editRecord = Vm.SelectScanReceiptData;
            editRecord.SHIP_BOX_CNT = tmpValue;
            if (!IsRestoreValueMode)
                Vm.UpdateScanReceiptShipBoxCntCommand.Execute(editRecord);

            IsRestoreValueMode = false;
        }

        private void txtReceiptFreightNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Vm.InsertScanReceiptData();
            }
            else
            { return; }
        }

        private void txtReceiptBoxCount_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox))
                return;
            var tmpTextbox = sender as TextBox;

            strBeforeEditReceiptBoxCount = tmpTextbox.Text;
        }

        private void txtBOX_CNT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("^[1-9]?[0-9]*$");
            var txt = ((TextBox)sender).Text + e.Text;


            e.Handled = !re.IsMatch(txt);
        }

        private void txtReceiptBoxCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("^[1-9]?[0-9]*$");
            var txt = ((TextBox)sender).Text + e.Text;
            e.Handled = !re.IsMatch(txt);
        }
    }
}
