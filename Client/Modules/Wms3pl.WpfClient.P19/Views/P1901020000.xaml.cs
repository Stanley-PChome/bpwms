using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;
using wcfShare = Wms3pl.WpfClient.ExDataServices.SharedWcfService;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1901020000.xaml 的互動邏輯
	/// </summary>
	public partial class P1901020000 : Wms3plUserControl
	{
		private P1901030000 _frmP1901030000;

		public P1901020000()
		{
			InitializeComponent();
			Vm.BackToFirstTab += GoToFirstTab;
			Vm.HasSelectedRecord = false;
			Vm.ExcelImport += ExcelImport;
		}

		private void GoToFirstTab()
		{
			LayoutRoot.SelectedIndex = 0;
		}

		private void Upload_Click(object sender, RoutedEventArgs e)
		{
			var gupCode = Vm.GupCode;
			var itemCode = (Vm.SelectedData != null) ? Vm.SelectedData.ITEM_CODE : string.Empty;
			var crtYear = (Vm.SelectedData != null) ? Vm.SelectedData.CRT_DATE.Year.ToString() : string.Empty;
			if (string.IsNullOrEmpty(itemCode))
			{
				ShowMessage(Properties.Resources.P1901020000xamlcs_SetItemNo);
				return;
			}
			var win = new P1901020100(itemCode, gupCode, crtYear);
			win.Owner = this.Parent as Window;
			win.Topmost = true;
			win.ShowDialog();
			win.Topmost = false;
			if (win.DialogResult.HasValue)
			{
				Vm.LoadImage();
			}
		}

		private void btnSize_Click(object sender, RoutedEventArgs e)
		{
			if (_frmP1901030000 == null)
			{
				var formService = new FormService();
				_frmP1901030000 = formService.AddFunctionForm("P1901030000", this.Parent as Window) as P1901030000;
			}
			if (_frmP1901030000 != null)
			{
				if (Vm.UserOperateMode == OperateMode.Query)
				{
					if (_frmP1901030000.ShowItem(Vm.CurrentRecordByView.GUP_CODE, Vm.CurrentRecordByView.CUST_CODE,
					Vm.CurrentRecordByView.ITEM_CODE))
						_frmP1901030000.Show();
					else
						_frmP1901030000.Close();
				}
				else
				{
					if (_frmP1901030000.ShowItem(Vm.CurrentRecord.GUP_CODE, Vm.CurrentRecord.CUST_CODE,
						Vm.CurrentRecord.ITEM_CODE))
						_frmP1901030000.Show();
					else
						_frmP1901030000.Close();
				}
			}
			e.Handled = true;

		}
		private void btnCheckItem_Click(object sender, RoutedEventArgs e)
		{
			var function = FormService.GetFunctionFromSession("P1902060000");
			if (function == null)
			{
				DialogService.ShowMessage(Properties.Resources.P1901020000xamlcs_NoAuthorize);
				return;
			}
			if (Vm.UserOperateMode == OperateMode.Query)
			{
				var win = new P1902060000(Vm.CurrentRecordByView.ITEM_CODE, Vm.CurrentRecordByView.CUST_CODE, Vm.CurrentRecordByView.GUP_CODE)
				{
					Function = function
				};
				win.Show();
			}
			else
			{
				var win = new P1902060000(Vm.CurrentRecord.ITEM_CODE, Vm.CurrentRecord.CUST_CODE, Vm.CurrentRecord.GUP_CODE)
				{
					Function = function
				};
				win.Show();
			}
			e.Handled = true;

		}

		private void ExportExcel(object sender, RoutedEventArgs e)
		{
			var source = ((DataGrid)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget).Items;
		}

		//private void BundleSerialLoc_Checked(object sender, RoutedEventArgs e)
		//{
		//	CheckSerialLoc(sender);
		//}

		//private void ComboPickWare_Changed(object sender, SelectionChangedEventArgs e)
		//{
		//	CheckSerialLoc(sender);
		//}

		//private void CheckSerialLoc(object obj)
		//{
		//	if (Vm.UserOperateMode == OperateMode.Query)
		//		return;

		//	if (BUNDLE_SERIALLOCCheckBox.IsChecked == true)
		//	{
		//		if (Vm.CurrentRecordDetail == null)
		//		{
		//			ShowMessage(Properties.Resources.P1901020000xamlcs_NoDataToInsertUpdate);
		//			this.ControlView(() =>
		//			{
		//				if (obj is CheckBox)
		//					// 序號商品=>加上選取;序號綁儲位=>取消選取
		//					((CheckBox)obj).IsChecked = (((CheckBox)obj).Name == BUNDLE_SERIALNOCheckBox.Name);

		//			});
		//			return;
		//		}

		//		//檢核流通加工倉
		//		var pickWare = Vm.WarehouseTypeList.Where(x => x.Name == Properties.Resources.P1901020000xamlcs_CirculateProcessWarehouse).Select(x => x.Value).SingleOrDefault();
		//		if (pickWare == null || Vm.CurrentRecordDetail.PICK_WARE != pickWare)
		//		{
		//			ShowMessage(Properties.Resources.P1901020000xamlcs_ItemSerailNoBundleCirculateProcessWarehouse);
		//			this.ControlView(() =>
		//			{
		//				if (obj is CheckBox)
		//					// 序號商品=>加上選取;序號綁儲位=>取消選取
		//					((CheckBox)obj).IsChecked = (((CheckBox)obj).Name == BUNDLE_SERIALNOCheckBox.Name);
		//				else if (obj is ComboBox)
		//					((ComboBox)obj).SelectedValue = pickWare;
		//			});
		//			return;
		//		}

		//		//檢核序號商品必勾
		//		if (string.IsNullOrEmpty(Vm.CurrentRecordDetail.BUNDLE_SERIALNO) || Vm.CurrentRecordDetail.BUNDLE_SERIALNO.ToString() == "0")
		//		{
		//			ShowMessage(Properties.Resources.P1901020000xamlcs_ItemSerailNoNeedCheck);
		//			this.ControlView(() =>
		//			{
		//				if (obj is CheckBox)
		//					// 序號商品=>加上選取;序號綁儲位=>取消選取
		//					((CheckBox)obj).IsChecked = (((CheckBox)obj).Name == BUNDLE_SERIALNOCheckBox.Name);
		//			});
		//			return;
		//		}
		//	}
		//}

		//private void ALLOWORDITEM_Checked(object sender, RoutedEventArgs e)
		//{
		//	if (Vm.CurrentRecordDetail == null)
		//	{
		//		ShowMessage(Properties.Resources.P1901020000xamlcs_NoDataToInsertUpdate);
		//		this.ControlView(() =>
		//		{
		//			((CheckBox)sender).IsChecked = false;
		//		});
		//		return;
		//	}
		//	if (string.IsNullOrEmpty(Vm.CurrentRecord.VIRTUAL_TYPE))
		//	{
		//		this.ControlView(() =>
		//		{
		//			txtVIRTUAL_TYPE.IsEnabled = false;
		//		});
		//		return;
		//	}
		//	else
		//	{
		//		ShowMessage(Properties.Resources.P1901020000xamlcs_ItemsCannotBeVirtual);
		//		this.ControlView(() =>
		//		{
		//			((CheckBox)sender).IsChecked = false;
		//		});
		//	}
		//}

		//private void ALLOWORDITEM_UnChecked(object sender, RoutedEventArgs e)
		//{
		//	this.ControlView(() =>
		//	{
		//		txtVIRTUAL_TYPE.IsEnabled = true;
		//	});
		//}
		private void ItemCode_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter) return;
			Vm.GetFindExistItem();
		}

		private void WarehouseType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.UserOperateMode != OperateMode.Query && Vm.CurrentRecord != null && !string.IsNullOrEmpty(Vm.CurrentRecord.PICK_WARE))
			{
				if (Vm.WarehouseIdListGroupByWareHouseType.Any(x => x.Key == Vm.CurrentRecord.PICK_WARE))
				{
					Vm.WareHouseIdList = Vm.WarehouseIdListGroupByWareHouseType[Vm.CurrentRecord.PICK_WARE];
					if (Vm.UserOperateMode == OperateMode.Add)
						Vm.CurrentRecord.PICK_WARE_ID = Vm.WareHouseIdList.FirstOrDefault()?.Value;
				}
				else
					Vm.WareHouseIdList = new List<Common.NameValuePair<string>>();
			}
		}

		bool ImportResultData = false;
		private void ExcelImport()
		{
			var win = new WinImportSample(string.Format("{0},{1}", Vm.CustCode, "P1901020000"));

			win.ImportResult = (t) => { ImportResultData = t; };
			win.ShowDialog();
			Vm.ImportFilePath = null;
			if (ImportResultData)
				Vm.ImportFilePath = OpenFileDialogFun();
		}
		private string OpenFileDialogFun()
		{
			var dlg = new Microsoft.Win32.OpenFileDialog
			{
				DefaultExt = ".xls",
				Filter = "excel files (*.xls,*.xlsx)|*.xls*|csv files (*.csv)|*.csv"
			};

			if (dlg.ShowDialog() == true)
			{
				String[] ex = dlg.SafeFileName.Split('.');
				//防止*.*的判斷式
				if (ex[ex.Length - 1] != "xls" && ex[ex.Length - 1] != "xlsx")
				{
					DialogService.ShowMessage("商品主檔匯入檔必須為Excel檔案，總共有58欄");
					dlg = null;
					return "";
				}
				return dlg.FileName;
			}
			return "";
		}

		private void TxtVnrCode_LostFocus(object sender, RoutedEventArgs e)
		{
			// 廠商編號不為空
			if (!string.IsNullOrWhiteSpace(Vm.CurrentRecord.VNR_CODE))
			{
				// 查詢廠商名稱
				var vnrName = Vm.GetVnrName(Vm.CurrentRecord.VNR_CODE);
				// 查無廠商名稱顯示錯誤訊息:查無此廠商編號，並將廠商編號清空
				if (string.IsNullOrWhiteSpace(vnrName))
				{
					DialogService.ShowMessage(Properties.Resources.P1901020000_VnrCodeNotFind);
					Vm.VNR_NAME = null;
				}
				else
				{
					Vm.VNR_NAME = vnrName;
				}
			}
			else
			{
				Vm.VNR_NAME = null;
			}
		}

		/// <summary>
		/// 新增、修改畫面計算允收天數和警示天數
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private void TxtSaveDay_LostFocus(object sender, RoutedEventArgs e)
		{
			Boolean NeedUpdateAllValue = Vm.OrgRecord.ALL_DLN == null && Vm.OrgRecord.ALL_SHP == null;
			if (Vm.CurrentRecord.NEED_EXPIRED == "1")
			{
				if (Vm.CurrentRecord.SAVE_DAY <= 30)
				{
					if (NeedUpdateAllValue)
					{
						Vm.CurrentRecord.ALL_DLN = null;
						Vm.CurrentRecord.ALL_SHP = null;
					}
					DialogService.ShowMessage(Properties.Resources.P1901020000_SaveDayShort);
				}
				if (NeedUpdateAllValue)
				{

					var proxy = new wcfShare.SharedWcfServiceClient();
					var result = Vm.RunWcfMethod<wcfShare.GetItemAllDlnAndAllShpRes>(
					proxy.InnerChannel,
					() => proxy.GetItemAllDlnAndAllShp(Vm.CurrentRecord.SAVE_DAY));
					var AllDlnAndAllShp = result;
					Vm.CurrentRecord.ALL_DLN = (short)AllDlnAndAllShp.ALL_DLN;
					Vm.CurrentRecord.ALL_SHP = AllDlnAndAllShp.ALL_SHP;
				}
			}
			else
			{
				if (NeedUpdateAllValue)
				{
					Vm.CurrentRecord.ALL_DLN = null;
					Vm.CurrentRecord.ALL_SHP = null;
				}
				Vm.CurrentRecord.SAVE_DAY = null;
			}

		}

		private void CheckBoxNeedExpired_UnCecked(object sender, RoutedEventArgs e)
		{
			if (Vm.CurrentRecord != null)
			{
				if (Vm.CurrentRecord.NEED_EXPIRED == "0")
				{
					Vm.CurrentRecord.ALL_DLN = null;
					Vm.CurrentRecord.ALL_SHP = null;
					Vm.CurrentRecord.SAVE_DAY = null;
				}
			}
		}

		private void TxtVnrCode_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter) return;
			TxtVnrCode_LostFocus(sender, e);
		}

    private void BUNDLE_SERIALNOCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
      if (Vm.CurrentRecord?.BUNDLE_SERIALNO == "0")
        Vm.CurrentRecord.BUNDLE_SERIALLOC = "0";
    }
  }
}
