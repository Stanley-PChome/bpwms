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
        /// txtMemo�s��e���e�A�P�_�O�_�nĲ�o���򤺮e��
        /// </summary>
        private string strBeforeEdittxtMemo;
        /// <summary>
        /// txtBOX_CNT�s��e���e�A�P�_�O�_�nĲ�o���򤺮e��
        /// </summary>
        private string strBeforeEdittxtBOX_CNT;
        /// <summary>
        /// txtReceiptBoxCount��ƭק�e���e�A��J���`���٭��
        /// </summary>
        private string strBeforeEditReceiptBoxCount;
        /// <summary>
        /// �O�_�٭줺�e���A(�קK�ƭ��٭���ٶ]�h������򤺮e)
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
                msg = "�п�J�ƭ�";
            else if (tmpValue <= 0)
                msg = "�п�J�j��0�ƭ�";

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
                msg = "�п�J�ƭ�";
            else if (testConveter > short.MaxValue)
                msg = "��J���ƭȹL�j";
            else if (testConveter <= 0)
                msg = "�п�J�j��0�ƭ�";
            else if (!short.TryParse(tmpTextbox.Text, out tmpValue))
                msg = "�п�J�ƭ�"; //�קKuser��J�L�j���ƭȡA�]���W�����Τ���j��long�h�����ഫ�A�~����ন����n�ϥΪ�short
            else if (tmpValue > Vm.SelectScanReceiptData.CHECK_BOX_CNT)
                msg = "���i��J�j��ֹ�c�Ƥ��ƭ�";

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
