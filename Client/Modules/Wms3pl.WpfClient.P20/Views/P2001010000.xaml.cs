using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using F1913Data = Wms3pl.WpfClient.ExDataServices.P20ExDataService.F1913Data;

namespace Wms3pl.WpfClient.P20.Views
{
    /// <summary>
    /// P2001010000.xaml 的互動邏輯
    /// </summary>
    public partial class P2001010000 : Wms3plUserControl
    {
        public P2001010000()
        {
            InitializeComponent();
            Vm.ItemAddClick += ItemAddClick;
            Vm.ItemEditClick += ItemEditClick;
            Vm.ResetAddDcCode1Selected += ResetAddDcCode1Selected;
            Vm.ExcelImport += ExcelImport;
        }

        private void ExcelImport()
        {
            bool ImportResultData = false;
            var win = new WinImportSample(string.Format("{0},{1}", Vm.CustCode, "P2001010000"));
            win.ImportResult = (t) => { ImportResultData = t; };
            win.ShowDialog();
            Vm.ImportFilePath = null;
            if (ImportResultData)
                Vm.ImportFilePath = OpenFileDialogFun();
        }
        private string OpenFileDialogFun()
        {
            if (Vm.F1913Datas != null && Vm.F1913Datas.Any())
            {
                var message = new MessagesStruct
                {
                    Button = DialogButton.YesNo,
                    Image = DialogImage.Warning,
                    Message = Properties.Resources.P2001010000_WarningDeleteGridData,
                    Title = Properties.Resources.P2001010000_Warning
                };
                var response = ShowMessage(message);
                if (response == DialogResponse.No)
                    return "";
            }

            var dlg = new OpenFileDialog { DefaultExt = ".xls", Filter = "excel files (*.xls,*.xlsx)|*.xls*" };
            if (dlg.ShowDialog() == true)
            {
                String[] ex = dlg.SafeFileName.Split('.');

                //防止*.*的判斷式
                if (ex[ex.Length - 1] != "xls" && ex[ex.Length - 1] != "xlsx")
                {
                    DialogService.ShowMessage("匯入檔必須為Excel檔案，總共有10欄");
                    dlg = null;
                    return "";
                }
                return dlg.FileName;
            }
            return "";
        }

        private void AddItemCode_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            Vm.GetItemName();
        }

        private void ItemAddClick()
        {
            var item = Vm.AddDcList1.Find(o => o.Value == Vm.SelectedAddDcCode1);

            var win = new P2001010200(item.Value, item.Name, Vm.SerialNoResults.SelectMany(x => x.Value))
            {
                Owner = System.Windows.Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            win.ShowDialog();
            if (win.DialogResult == true)
            {
                win.Vm.F1913Data.IsSelected = true;
                var seq = (Vm.F1913Datas != null && Vm.F1913Datas.Any()) ? Vm.F1913Datas.Max(o => o.ROWNUM) + 1 : 1;
                win.Vm.F1913Data.ROWNUM = seq;
                if (Vm.F1913Datas == null)
                    Vm.F1913Datas = new List<F1913Data>();
                Vm.F1913Datas.Add(win.Vm.F1913Data);
                Vm.F1913Datas = Vm.F1913Datas.ToList();
                Vm.SelectedF1913Data = Vm.F1913Datas.Find(o => o.ROWNUM == seq);
                var list = win.Vm.DgList;
                if (list != null && list.Any())
                {
                    Vm.SerialNoResults.Add(Vm.SelectedF1913Data.ROWNUM, win.Vm.DgList);

                    if (Vm.SelectedF1913Data.BUNDLE_SERIALNO == "1" && win.Vm.AdjustQty == win.Vm.DgList.Count(o => o.Checked))
                        Vm.SelectedF1913Data.SERIALNO_SCANOK = "1";
                    else
                        Vm.SelectedF1913Data.SERIALNO_SCANOK = "0";
                }
                else if (Vm.SelectedF1913Data.BUNDLE_SERIALNO == "1")
                {
                    Vm.SelectedF1913Data.SERIALNO_SCANOK = "0";
                }
            }

        }

        private void ItemEditClick()
        {
            var item = Vm.SerialNoResults.FirstOrDefault(o => o.Key == Vm.SelectedF1913Data.ROWNUM);

            var win = new P2001010100(Vm.SelectedF1913Data, item, Vm.SerialNoResults.Where(x => x.Key != Vm.SelectedF1913Data.ROWNUM).SelectMany(x => x.Value))
            {
                Owner = System.Windows.Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            win.ShowDialog();
            if (win.DialogResult == true)
            {
                Vm.SelectedF1913Data.IsSelected = true;
                Vm.SelectedF1913Data.WORK_TYPE = win.Vm.WorkType;
                if (win.Vm.WorkType == "0")
                    Vm.SelectedF1913Data.ADJ_QTY_IN = win.Vm.AdjustQty;
                else
                    Vm.SelectedF1913Data.ADJ_QTY_IN = null;
                if (win.Vm.WorkType == "1")
                    Vm.SelectedF1913Data.ADJ_QTY_OUT = win.Vm.AdjustQty;
                else
                    Vm.SelectedF1913Data.ADJ_QTY_OUT = null;
                Vm.SelectedF1913Data.CAUSE = win.Vm.Cause;
                Vm.SelectedF1913Data.CAUSENAME = win.Vm.CauseName;
                Vm.SelectedF1913Data.CAUSE_MEMO = win.Vm.Cause == "999" ? win.Vm.CauseMemo : string.Empty;
                var list = win.Vm.DgList;
                if (list != null && list.Any())
                {
                    if (item.Key == 0)
                        Vm.SerialNoResults.Add(Vm.SelectedF1913Data.ROWNUM, win.Vm.DgList);
                    else
                    {
                        Vm.SerialNoResults.Remove(item.Key);
                        if (win.Vm.DgList != null)
                            Vm.SerialNoResults.Add(Vm.SelectedF1913Data.ROWNUM, win.Vm.DgList);
                    }
                    if (Vm.SelectedF1913Data.BUNDLE_SERIALNO == "1" && win.Vm.AdjustQty == win.Vm.DgList.Count(o => o.Checked))
                        Vm.SelectedF1913Data.SERIALNO_SCANOK = "1";
                    else
                        Vm.SelectedF1913Data.SERIALNO_SCANOK = "0";
                }
                else
                {
                    if (Vm.SelectedF1913Data.BUNDLE_SERIALNO == "1")
                        Vm.SelectedF1913Data.SERIALNO_SCANOK = "0";
                    if (item.Key != 0)
                        Vm.SerialNoResults.Remove(item.Key);
                }
            }
        }

        private void ResetAddDcCode1Selected()
        {
            ControlView(() => CmbxAddDcCode1.SelectedValue = Vm.SelectedAddDcCode1);
        }

        private void btnSerachProduct_Click(object sender, RoutedEventArgs e)
        {
            WinSearchProduct winSearchProduct = new WinSearchProduct();
            var globalInfo = Wms3plSession.Get<GlobalInfo>();
            winSearchProduct.GupCode = globalInfo.GupCode;
            winSearchProduct.CustCode = globalInfo.CustCode;
            winSearchProduct.ShowDialog();

            if (winSearchProduct.SelectData != null)
            {
                F1903 f1903 = winSearchProduct.SelectData;
                Vm.AppendItemCode(f1903);
            }
        }

        private void AddItemCode_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty((sender as TextBox).Text))
                Vm.GetItemName();
        }
    }
}
