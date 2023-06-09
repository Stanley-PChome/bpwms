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
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P15ExDataService;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using Wms3pl.WpfClient.P15.Report;
using Wms3pl.WpfClient.P15.ViewModel;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P15.Views
{
	/// <summary>
	/// P1502010000.xaml 的互動邏輯
	/// </summary>
	public partial class P1502010000 : Wms3plUserControl
	{
		public P1502010000()
		{
			InitializeComponent();
			Vm.DoPrintReport += GetReport;
			Vm.DoPrintStickerReport += GetStickerReport;
			Vm.DoSendCar += DoSendCar;
			Vm.ExcelImport += ExcelImport;
			Vm.IsExpend += IsExpend;
            Vm.FinishedOffShelfLackProcess = () => { LackProgess(false); };
        }



    private void Posting_Click(object sender, RoutedEventArgs e)
    {
      //呼叫缺貨視窗
      LackProgess(true);
      if (Vm.IsPosted)
      {
        Vm.ShowInfoMessage(Properties.Resources.P1502010000_NO_LACK_TO_COMPLETE);
        Vm.DoSearch();
        if (Vm.SelectedData != null && Vm.DgList.Any(x => x.ALLOCATION_NO == Vm.SelectedData.ALLOCATION_NO))
          Vm.SelectedData = Vm.DgList.FirstOrDefault(x => x.ALLOCATION_NO == Vm.SelectedData.ALLOCATION_NO);
        return;
      }

      //沒有缺貨的話也要呼叫調撥確認下架
      if (!Vm.IsProcessedLack(Vm.SelectedData.SRC_DC_CODE, Vm.SelectedData.GUP_CODE, Vm.SelectedData.CUST_CODE, Vm.SelectedData.ALLOCATION_NO) && Vm.SelectedData.STATUS == "1")
        Vm.DoFinishedOffShelf();

      if (Vm.SelectedData != null)
      {
        Vm.IsPosting = true;
        var win = new P1502010100(ExDataMapper.Map<P1502010000Data, F151001>(Vm.SelectedData));
        win.ShowDialog();
        var item = Vm.SelectedData;
        var tmpAllocationNo = item.ALLOCATION_NO;
        Vm.IsPosting = false;
        Vm.CRTDateS = item.CRT_ALLOCATION_DATE.Date;
        Vm.CRTDateE = item.CRT_ALLOCATION_DATE.Date;
        Vm.PostingDateS = null;
        Vm.PostingDateE = null;
        Vm.SourceDcCode = item.SRC_DC_CODE;
        Vm.TargetDcCode = item.TAR_DC_CODE;
        Vm.TxtSearchAllocationNo = "";
        Vm.SelectSourceWarehouse = "";
        Vm.SelectTargetWarehouse = "";
        Vm.UserOperateMode = OperateMode.Query;
        Vm.DoSearch();
        if (Vm.DgList.Any(x => x.ALLOCATION_NO == tmpAllocationNo))
          Vm.SelectedData = Vm.DgList.FirstOrDefault(x => x.ALLOCATION_NO == tmpAllocationNo);

      }
    }

    private void AddDetail_Click(object sender, RoutedEventArgs e)
		{
			var win = new P1502010200(Vm.AddOrUpdateF151001,Vm.DgItemList.Select(x=> x.Item).ToList());
			win.ShowDialog();
			Vm.AddItemList = win.Vm.ReturnData;
			Vm.ReturnDetailData();
		}

		/// <summary>
		/// 列印標籤貼紙
		/// </summary>
		/// <param name="printType"></param>
		private void GetStickerReport(PrintType printType)
		{
				var list3 = Vm.F151001ReportDataDTByTicket;
			  if(list3.Rows.Count > 0)
				{
					var report3 = new Report.RP1502010006();
					report3.Load(@"RP1502010006.rpt");
					report3.SetDataSource(list3);

				var deviceSerivce = new DeviceWindowService();
				var settings = deviceSerivce.GetDeviceSettings(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedData.DC_CODE);
				
				if (printType == PrintType.ToPrinter && (!settings.Any() || string.IsNullOrWhiteSpace(settings.First().LABELING)))
					ShowMessage(Properties.Resources.NotSetPrint3);
				else
					PrintReport(report3, settings.First(), printType, PrinterType.Label);
				}
				else
					DialogService.ShowMessage(Properties.Resources.P1502010000xamlcs_EmptyData);

		}

		private void GetReport(PrintType printType)
		{
			var deviceSerivce = new DeviceWindowService();
			var settings = deviceSerivce.GetDeviceSettings(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedData.DC_CODE);
			if (printType == PrintType.ToPrinter && (!settings.Any() || string.IsNullOrWhiteSpace(settings.First().PRINTER)))
			{
				ShowMessage(Properties.Resources.NotSetPrint1);
				return;
			}

		  var isSrcAutoWarehouse = Vm.CheckAutomaticWarehouse(Vm.SelectedData.SRC_DC_CODE, Vm.SelectedData.SRC_WAREHOUSE_ID);
			var isTarAutoWarehouse = Vm.CheckAutomaticWarehouse(Vm.SelectedData.TAR_DC_CODE, Vm.SelectedData.TAR_WAREHOUSE_ID);
			switch (Vm.SelectedData.STATUS)
			{
				case "5": //結案-印調撥結果報表
					var list = Vm.F151001ReportDataByExpendDate;
					if (list == null || list.Count == 0)
					{
						DialogService.ShowMessage(Properties.Resources.P1502010000xamlcs_EmptyData);
						return;
					}
					var report = new RP1502010005();
					string reportTitle = Wms3plSession.Get<GlobalInfo>().GupName + "－" + Wms3plSession.Get<GlobalInfo>().CustName + Properties.Resources.P1502010000xamlcs_OntheMarket;
					report.Load(@"RP1502010005.rpt");
					report.SetText("TitleText", reportTitle);
					report.SetText("txtUserName", Wms3plSession.Get<UserInfo>().AccountName);
					report.SetText("txtDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
					report.SetDataSource(list);
			    PrintReport(report, settings.First(), printType, PrinterType.A4);
					break;
				//列印紙本調撥單
				case "0": 
				case "1": 
				case "3":
					if (Vm.SelectedData.ISEXPENDDATE == "1" )
					{
						var list1 = Vm.F151001ReportDataByExpendDate;
						if (list1 == null || list1.Count == 0)
						{
							DialogService.ShowMessage(Properties.Resources.P1502010000xamlcs_EmptyData);
							return;
						}
						var list2 = Vm.F151001ReportDataDTByExpendDate;
						var itemTotal = list1.GroupBy(x => x.ITEM_CODE).Count();
						var srcQtyTotal = list1.Sum(x => x.SRC_QTY);
						var tarQtyTotal = list1.Sum(x => x.TAR_QTY);

						if (!isSrcAutoWarehouse && Vm.SelectedData.SOURCE_TYPE!="04" && !string.IsNullOrWhiteSpace(Vm.SelectedData.SRC_WAREHOUSE_ID))
						{
							string reportTitle4 = Wms3pl.WpfClient.Common.Wms3plSession.Get<Wms3pl.WpfClient.Common.GlobalInfo>().GupName + "－"
															+ Wms3pl.WpfClient.Common.Wms3plSession.Get<Wms3pl.WpfClient.Common.GlobalInfo>().CustName + Properties.Resources.P1502010000xamlcs_TransferDiscontinued;

							var report4 = new Report.RP1502010004();
							report4.Load(@"RP1502010004.rpt");
							report4.SetText("TitleText", reportTitle4);
							report4.SetText("txtUserName", Wms3plSession.Get<UserInfo>().AccountName);
							report4.SetText("txtDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
							report4.SetDataSource(list2);
							report4.SetParameterValue("itemTotal", itemTotal);
							report4.SetParameterValue("qtyTotal", srcQtyTotal);
							PrintReport(report4, settings.First(), printType, PrinterType.A4);

						}
						if (!isTarAutoWarehouse && !string.IsNullOrWhiteSpace(Vm.SelectedData.TAR_WAREHOUSE_ID))
						{
							string reportTitle3 = Wms3pl.WpfClient.Common.Wms3plSession.Get<Wms3pl.WpfClient.Common.GlobalInfo>().GupName + "－"
																						+ Wms3pl.WpfClient.Common.Wms3plSession.Get<Wms3pl.WpfClient.Common.GlobalInfo>().CustName + Properties.Resources.P1502010000xamlcs_TransferOntheMarket;

							var report3 = new Report.RP1502010003();
							report3.Load(@"RP1502010003.rpt");
							report3.SetText("TitleText", reportTitle3);
							report3.SetText("txtUserName", Wms3plSession.Get<UserInfo>().AccountName);
							report3.SetText("txtDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
							report3.SetDataSource(list2);
							report3.SetParameterValue("itemTotal", itemTotal);
							report3.SetParameterValue("qtyTotal", srcQtyTotal);
							PrintReport(report3, settings.First(), printType, PrinterType.A4);
						}
					}
					else
					{
						var list1 = Vm.F151001ReportData;
						if (list1 == null || list1.Count == 0)
						{
							DialogService.ShowMessage(Properties.Resources.P1502010000xamlcs_EmptyData);
							return;
						}
						var list2 = Vm.F151001ReportDataDT;
						var itemTotal = list1.GroupBy(x => x.ITEM_CODE).Count();
						var srcQtyTotal = list1.Sum(x => x.SRC_QTY);
						var tarQtyTotal = list1.Sum(x => x.TAR_QTY);

						if (!isSrcAutoWarehouse && Vm.SelectedData.SOURCE_TYPE != "04" && !string.IsNullOrWhiteSpace(Vm.SelectedData.SRC_WAREHOUSE_ID))
						{
							string reportTitle2 = Wms3pl.WpfClient.Common.Wms3plSession.Get<Wms3pl.WpfClient.Common.GlobalInfo>().GupName + "－"
																						+ Wms3pl.WpfClient.Common.Wms3plSession.Get<Wms3pl.WpfClient.Common.GlobalInfo>().CustName + Properties.Resources.P1502010000xamlcs_TransferDiscontinued;

							var report4 = new Report.RP1502010002();
							report4.Load(@"RP1502010002.rpt");
							report4.SetText("TitleText", reportTitle2);
							report4.SetText("txtUserName", Wms3plSession.Get<UserInfo>().AccountName);
							report4.SetText("txtDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
							report4.SetDataSource(list2);
							report4.SetParameterValue("itemTotal", itemTotal);
							report4.SetParameterValue("qtyTotal", srcQtyTotal);
							PrintReport(report4, settings.First(), printType, PrinterType.A4);

						}
						if (!isTarAutoWarehouse && !string.IsNullOrWhiteSpace(Vm.SelectedData.TAR_WAREHOUSE_ID))
						{
							string reportTitle1 = Wms3pl.WpfClient.Common.Wms3plSession.Get<Wms3pl.WpfClient.Common.GlobalInfo>().GupName + "－"
																						+ Wms3pl.WpfClient.Common.Wms3plSession.Get<Wms3pl.WpfClient.Common.GlobalInfo>().CustName + Properties.Resources.P1502010000xamlcs_TransferOntheMarket;

							var report1 = new Report.RP1502010001();
							report1.Load(@"RP1502010001.rpt");
							report1.SetText("TitleText", reportTitle1);
							report1.SetText("txtUserName", Wms3plSession.Get<UserInfo>().AccountName);
							report1.SetText("txtDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
							report1.SetDataSource(list2);
							report1.SetParameterValue("itemTotal", itemTotal);
							report1.SetParameterValue("qtyTotal", srcQtyTotal);
							PrintReport(report1, settings.First(), printType, PrinterType.A4);
						}
					}
					break;
			}

		}

		private void DoSendCar()
		{
			var win = new P1502010400()
			{
				Owner = System.Windows.Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				FontSize = Application.Current.MainWindow.FontSize,
			};
			win.ShowDialog();
		}
		private bool _isChangeDc;

		private void SourceDc_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.UserOperateMode != OperateMode.Query)
			{
				if (!Vm.IsFirstAddorUpdate)
				{
					if (!_isChangeDc && Vm.DgItemList != null && Vm.DgItemList.Any())
					{
						_isChangeDc = true;
						var message = new MessagesStruct
						{
							Button = DialogButton.YesNo,
							Image = DialogImage.Warning,
							Message = Properties.Resources.P1502010000_ConfirmToDelAllDetailDataByModifyDC,
							Title = Properties.Resources.P1502010000xamlcs_Information};
						if (Vm.ShowMessage(message) == DialogResponse.Yes)
						{
							Vm.DgItemList = new List<SelectionItem<F151001DetailDatas>>();
							_isChangeDc = false;
						}
						else
						{
							var combobox = sender as ComboBox;
							combobox.SelectedValue = (e.RemovedItems[0] as NameValuePair<string>).Value;
						}
					}
					else
						_isChangeDc = false;

				}

				Vm.AddOrUpdateSourceWarehouseList = Vm.GetUserWarehouse(Vm.AddOrUpdateF151001.SRC_DC_CODE, false);
				if (Vm.UserOperateMode == OperateMode.Add && Vm.IsFirstAddorUpdate)
					Vm.AddOrUpdateF151001.SRC_WAREHOUSE_ID = Vm.AddOrUpdateSourceWarehouseList.Any()
						? Vm.AddOrUpdateSourceWarehouseList.First().Value
						: "";
			}
		}

		private void TargetDc_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.UserOperateMode != OperateMode.Query)
			{
				if (!Vm.IsFirstAddorUpdate)
				{
					if (!_isChangeDc && Vm.DgItemList != null && Vm.DgItemList.Any())
					{
						_isChangeDc = true;
						var message = new MessagesStruct
						{
							Button = DialogButton.YesNo,
							Image = DialogImage.Warning,
							Message = Properties.Resources.P1502010000_ConfirmToReturnTAR_QTYByModifyDC,
							Title = Properties.Resources.P1502010000xamlcs_Information};
						if (Vm.ShowMessage(message) == DialogResponse.Yes)
						{
							var tmpDg = Vm.DgItemList.ToList();
							foreach (var item in tmpDg)
							{
								item.Item.TAR_QTY = item.Item.SRC_QTY;
							}
							Vm.DgItemList = tmpDg.ToList();
							_isChangeDc = false;
						}
						else
						{
							var combobox = sender as ComboBox;
							combobox.SelectedValue = (e.RemovedItems[0] as NameValuePair<string>).Value;
						}
					}
					else
						_isChangeDc = false;
				}
				Vm.AddOrUpdateTargetWarehouseList = Vm.GetUserWarehouse(Vm.AddOrUpdateF151001.TAR_DC_CODE, false);
				if (Vm.UserOperateMode == OperateMode.Add && Vm.IsFirstAddorUpdate)
					Vm.AddOrUpdateF151001.TAR_WAREHOUSE_ID = Vm.AddOrUpdateTargetWarehouseList.Any()
						? Vm.AddOrUpdateTargetWarehouseList.First().Value
						: "";
				else
					Vm.IsFirstAddorUpdate = false;
			}
		}

		private void OnCellInEditGotFocus(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(Vm.AddOrUpdateF151001.TAR_WAREHOUSE_ID))
			{
				var tx = sender as TextBox;
				int qty = 0;
				if (int.TryParse(tx.Text, out qty))
				{
					Vm.SelectedDgItem.Item.TAR_QTY = qty;
				}
			}
		}

        bool ImportResultData = false;
        private void ExcelImport()
		{
            var win = new WinImportSample(string.Format("{0},{1}", Vm.CustCode, "P1502010000"));

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
                Filter = "excel files (*.xls,*.xlsx)|*.xls*"
            };

			if (dlg.ShowDialog() == true)
			{
                String[] ex = dlg.SafeFileName.Split('.');

                //防止*.*的判斷式
                if (ex[ex.Length - 1] != "xls" && ex[ex.Length - 1] != "xlsx")
                {
                    DialogService.ShowMessage("匯入檔必須為Excel檔案，總共有12欄");
                    dlg = null;
                    return "";
                }
                return dlg.FileName;
			}
			return "";
		}

		private bool _isChanged = false;
		private void IsExpend_Checked(object sender, RoutedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				var ck = sender as CheckBox;
				if (!_isChanged && Vm.DgItemList != null && Vm.DgItemList.Any())
				{
					var message = string.Format(Properties.Resources.P1502010000xamlcs_CheckConfirmToModify, (ck.IsChecked ?? false) ? Properties.Resources.EXPEND : Properties.Resources.P1502010000xamlcs_Fold, Environment.NewLine);

                    var yesOrNo = Vm.ShowConfirmMessage(message);
                    if (yesOrNo == DialogResponse.No)
                    {
                        _isChanged = true;
                        ck.IsChecked = !ck.IsChecked;
                        return;
                    }
                    else if (yesOrNo == DialogResponse.Yes)
                    {
                        Vm.FormatData(false);
                    }
				}
				//if (!_isChanged)
				//{
				//	Vm.DgItemList = new List<SelectionItem<F151001DetailDatas>>();
				//	var validCol = DgItemList.Columns.Where(x => x.Header != null && x.Header.ToString() == Properties.Resources.VALID_DATE).FirstOrDefault();
				//	if (validCol != null)
				//		validCol.Visibility = (ck.IsChecked ?? false) ? Visibility.Visible : Visibility.Collapsed;
				//	var enterCol = DgItemList.Columns.Where(x => x.Header != null && x.Header.ToString() == Properties.Resources.ENTER_DATE).FirstOrDefault();
				//	if (enterCol != null)
				//		enterCol.Visibility = (ck.IsChecked ?? false) ? Visibility.Visible : Visibility.Collapsed;
				//}
				_isChanged = false;
			}

		}

		private void IsExpend()
		{
			//if (Vm.SelectedData != null)
			//{
			//	var validCol = DgDetailList.Columns.Where(x => x.Header != null && x.Header.ToString() == Properties.Resources.VALID_DATE).FirstOrDefault();
			//	if (validCol != null)
			//		validCol.Visibility = (Vm.SelectedData.ISEXPENDDATE == "1") ? Visibility.Visible : Visibility.Collapsed;
			//	var enterCol = DgDetailList.Columns.Where(x => x.Header != null && x.Header.ToString() == Properties.Resources.ENTER_DATE).FirstOrDefault();
			//	if (enterCol != null)
			//		enterCol.Visibility = (Vm.SelectedData.ISEXPENDDATE == "1") ? Visibility.Visible : Visibility.Collapsed;
			//}
		}

    private void LackProgess(Boolean IsCheckAutoWarehouse)
    {
      if (IsCheckAutoWarehouse && Vm.SelectedData.SRC_WH_DEVICE_TYPE != "0")
        return;

      if (!Vm.IsProcessedLack(Vm.SelectedData.SRC_DC_CODE, Vm.SelectedData.GUP_CODE, Vm.SelectedData.CUST_CODE, Vm.SelectedData.ALLOCATION_NO) && Vm.SelectedData.STATUS == "1")
      {
        var drAlloLack = Vm.ShowConfirmMessage("是否有缺貨");
        if (drAlloLack == DialogResponse.Yes)
        {
          var AlloLackWin = new P1502010500(Vm.SelectedData.SRC_DC_CODE, Vm.SelectedData.GUP_CODE, Vm.SelectedData.CUST_CODE, Vm.SelectedData.ALLOCATION_NO);
          Vm.IsInputLack = AlloLackWin.ShowDialog().Value;
          Vm.IsPosted = AlloLackWin.Vm.IsPosted;
        }
        else
        {
          Vm.IsInputLack = false;
          Vm.IsPosted = false;
        }
      }

    }
  }
}
