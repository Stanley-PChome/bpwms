using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.P91.Report;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P91.Views
{
	/// <summary>
	/// P9101010000.xaml 的互動邏輯
	/// </summary>
	public partial class P9101010000 : Wms3plUserControl
	{
		public P9101010000()
		{
			InitializeComponent();
			Vm.DoPrintReport += DoPrint;
			Vm.DoPrintPickTicketReport += DoPrintPickTicket;
			Vm.actionForCreatePickTicket += DoOpenCreatePickTicket;
			Vm.actionForAfterUpdate += ActionAfterUpdate;
		}

		private void BtnPrintLBSet_OnClick(object sender, RoutedEventArgs e)
		{
			// 檢查是否已開立揀料單, 否的話則無法進行動作
			if (Vm.EditableF910201.PROC_STATUS == "0")
			{
				ShowMessage(new MessagesStruct() { Button = UILib.Services.DialogButton.OK, Image = UILib.Services.DialogImage.Information, Message = Properties.Resources.P9101010000xamlcs_StartPickORD_To_PreSelectLabel, Title = Properties.Resources.P9101010000xamlcs_Message });
			}
			else
			{
				var win = new P9101010300(Vm.EditableF910201) { Owner = System.Windows.Window.GetWindow(this) };
				win.ShowDialog();
			}
		}

		private void BtnBOM_OnClick(object sender, RoutedEventArgs e)
		{
			var function = FormService.GetFunctionFromSession("P9103010000");
			if (function == null)
			{
				DialogService.ShowMessage(Properties.Resources.P9101010000xamlcs_UnAuthentication);
				return;
			}
			var win = new P9103010000()
			{
				Function = function
			};

			win.Show();
		}

		private void BtnLine_OnClick(object sender, RoutedEventArgs e)
		{
			// 檢查是否已開立揀料單, 否的話則無法進行動作
			if (Vm.EditableF910201.PROC_STATUS == "0")
			{
				ShowMessage(new MessagesStruct() { Button = UILib.Services.DialogButton.OK, Image = UILib.Services.DialogImage.Information, Message = Properties.Resources.P9101010000xamlcs_StartPickORD_To_PreSelectLabel, Title = Properties.Resources.P9101010000xamlcs_Message });
			}
			else
			{
				var win = new P9101010200(Vm.EditableF910201) { Owner = System.Windows.Window.GetWindow(this) };
				win.ShowDialog();
			}

		}

		private void BtnFinish_OnClick(object sender, RoutedEventArgs e)
		{
			if (!Vm.CheckCanEdit("1", Properties.Resources.P9101010000xamlcs_Process_Status_CantEdit))
				return;
			
			if (Vm.EditableF910201.PROC_STATUS == "0" || string.IsNullOrEmpty(Vm.EditableF910201.PROC_STATUS))
			{
				ShowMessage(new MessagesStruct() { Button = UILib.Services.DialogButton.OK, Image = UILib.Services.DialogImage.Information, Message = Properties.Resources.P9101010000xamlcs_StartPickORD, Title = Properties.Resources.P9101010000xamlcs_Message });

			}
            else if (Vm.CheckProcStatus())
            {
                var win = new P9101010400(Vm.EditableF910201) { Owner = System.Windows.Window.GetWindow(this) };
                win.ShowDialog();
                Vm.SetEditableF910201();
            }
            else
			{
				ShowMessage(new MessagesStruct() { Button = UILib.Services.DialogButton.OK, Image = UILib.Services.DialogImage.Information, Message = Properties.Resources.P9101010000xamlcs_TransferMaintainBeforeProcess, Title = Properties.Resources.P9101010000xamlcs_Message });

			}

		}

		private void BtnReturn_OnClick(object sender, RoutedEventArgs e)
        {
            if (Vm.EditableF910201.PROC_STATUS == "0" || string.IsNullOrEmpty(Vm.EditableF910201.PROC_STATUS))
            {
                ShowMessage(new MessagesStruct() { Button = UILib.Services.DialogButton.OK, Image = UILib.Services.DialogImage.Information, Message = Properties.Resources.P9101010000xamlcs_StartPickORD, Title = Properties.Resources.P9101010000xamlcs_Message });

            }
            else if (Vm.CheckProcStatus())
			{
				var win = new P9101010500(Vm.EditableF910201) { Owner = System.Windows.Window.GetWindow(this) };
				win.ShowDialog();
			}
			
			else
			{
				ShowMessage(new MessagesStruct() { Button = UILib.Services.DialogButton.OK, Image = UILib.Services.DialogImage.Information, Message = Properties.Resources.P9101010000xamlcs_TransferMaintainBeforeBackWarehouse, Title = Properties.Resources.P9101010000xamlcs_Message });

			}
		}

		private void BtnSetLabel_OnClick(object sender, RoutedEventArgs e)
		{
			var function = FormService.GetFunctionFromSession("P9103040000");
			if (function == null)
			{
				DialogService.ShowMessage(Properties.Resources.P9101010000xamlcs_UnAuthentication);
				return;
			}
			var win = new P9103040000()
			{
				Function = function
			};

			win.Show();
		}

		/// <summary>
		/// 列印流通加工單
		/// </summary>
		private void DoPrint(PrintType type = PrintType.Preview)
		{
			var report = new R9101010001();
			report.Load("R9101010001.rpt");
			report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().GupName + "－" + Wms3plSession.Get<GlobalInfo>().CustName + Properties.Resources.P9101010000xamlcs_CirculateProcess;
			report.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;
			report.SetDataSource(Vm.ReportData.ToDataTable());
			report.Subreports[0].SetDataSource(Vm.F910402Data.ToDataTable());
			report.Subreports[1].SetDataSource(Vm.F910403Data.ToDataTable());
			var win = new Wms3plViewer { Owner = Wms3plViewer.GetWindow(this) };
			win.CallReport(report, type);
		}

		/// <summary>
		/// 列印揀料單
		/// </summary>
		private void DoPrintPickTicket(PrintType type = PrintType.Preview)
		{
			if (!Vm.PickReports.Any())
			{
				ShowMessage(Messages.InfoNoData.Message);
				return;
			}

			var report = new RP9101010002();
			report.Load("RP9101010002.rpt");
			report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().GupName + "－" + Wms3plSession.Get<GlobalInfo>().CustName + Properties.Resources.P9101010000xamlcs_Process_PickOrd;
			report.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;
			Vm.PickReports.ForEach(x => x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE));
			report.SetDataSource(Vm.PickReports.ToDataTable());
			var win = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
			win.CallReport(report, type);
		}

		/// <summary>
		/// 開啟新增揀料單的視窗
		/// </summary>
		private void DoOpenCreatePickTicket()
		{
			var win = new P9101010100(Vm.EditableF910201) { Owner = System.Windows.Window.GetWindow(this) };
			win.ShowDialog();
			if (!string.IsNullOrEmpty(win.PickNo))
			{
				Vm.SetEditableF910201();
				var msg = Vm.ShowMessage(Messages.InfoPickNoExists);
				if (msg == DialogResponse.Yes)
				{
					// 列印揀料單
					Vm.DoPrintPickTicket(PrintType.Preview);
				}
			}
		}

		/// <summary>
		/// 當完成日期改變時, 要檢查所選擇的加工項目是否符合日期範圍
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dtFinishDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!Vm.IsValidF910401())
				SetFocusedElement(ddlEditableF910401);
		}

		/// <summary>
		/// 編輯時, 輸入商品編號並按下Enter就帶出符合的商品名稱
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtItemName_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			var val = (sender as TextBox).Text;
			if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(val))
			{
				Vm.EditableItemName = Vm.GetItemName(Vm.EditableF910201.GUP_CODE, val, Vm.EditableF910201.CUST_CODE);
				Vm.ItemImageSource = null;
			}
		}

		/// <summary>
		/// 開啟組合商品清單
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnItemCodeBom_Click(object sender, RoutedEventArgs e)
		{
			var win = new P9101010600(Vm.EditableF910201);
			var result = win.ShowDialog();
			// 當按下Properties.Resources.P9101010000xamlcs_Confirm或是Properties.Resources.NONE_Select時, 回傳值會是true, 此時會把值帶回來
			// 如果回傳False, 代表是直接關閉視窗, 將不做任何處理
			if (result == true)
			{
				if (win.Vm.SelectedData != null) // 按下確認時, 帶回資料
				{
					// 使用者選取後按《確認》，系統將此筆成品編號、成品名稱、組合說明帶到主畫面的成品編號(組合編號)、成品名稱、組合說明欄位。
					Vm.EditableF910201.ITEM_CODE_BOM = win.Vm.SelectedData.BOM_NO;
					Vm.EditableItemBomName = win.Vm.SelectedData.BOM_NAME;
					Vm.EditableF910201.ITEM_CODE = win.Vm.SelectedData.ITEM_CODE;
					//Vm.EditableItemName = win.Vm.SelectedData.ITEM_NAME;
					Vm.EditableItemName = Vm.GetItemName(Vm.EditableF910201.GUP_CODE, Vm.EditableF910201.ITEM_CODE, Vm.EditableF910201.CUST_CODE);
				}
				else // 按下Properties.Resources.NONE_Select時, 將資料清空
				{
					// 使用者可於組合清單視窗點選《不指定》按鈕，即回到主畫面並將成品編號、成品名稱、組合編號、組合名稱清空。
					Vm.EditableF910201.ITEM_CODE_BOM = string.Empty;
					Vm.EditableItemBomName = string.Empty;
					Vm.EditableF910201.ITEM_CODE = string.Empty;
					Vm.EditableItemName = string.Empty;
					Vm.ItemImageSource = null;
				}
			}
		}

		private void ActionAfterUpdate()
		{
			//dgF910201.SelectedIndex = 0;
			//if (dgF910201.Items.Count == 0) return;
			//Vm.SelectedF910201 = (F910201) dgF910201.Items[0];
		}

		private void txtItemName_LostFocus(object sender, RoutedEventArgs e)
		{
			Vm.EditableItemName = Vm.GetItemName(Vm.EditableF910201.GUP_CODE, (sender as TextBox).Text, Vm.EditableF910201.CUST_CODE);
			Vm.ItemImageSource = null;
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var win = new WinSearchProduct();
            win.GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
            win.CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;
            win.Owner = this.Parent as Window;
            win.ShowDialog();
            if (win.DialogResult.HasValue && win.DialogResult.Value)
            {
                SetSearchData(win.SelectData);
            }
        }
        private void SetSearchData(F1903 f1903)
        {
            if (f1903 != null)
            {
                Vm.EditableF910201.ITEM_CODE = f1903.ITEM_CODE;
            }
            else
            {
                Vm.EditableF910201.ITEM_CODE = string.Empty;
            }
        }
    }
}