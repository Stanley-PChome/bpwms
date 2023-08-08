using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.P05.Report;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P05.Views
{
	public partial class P0501040000 : Wms3plUserControl
	{
		DispatcherTimer dispatcherTimer = new DispatcherTimer
		{
			Interval = new TimeSpan(0, 2, 0)
		};
		public P0501040000()
		{
			InitializeComponent();
			Vm.SelectedTabIndex = 0;
			Vm.StartAutoUpdate += StartAutoUpdate;
			Vm.StopAutoUpdate += StopAutoUpdate;
			Vm.DoPrintReport += DoPrintReport;
			dispatcherTimer.Tick -= TimeAction;
			dispatcherTimer.Tick += TimeAction;
			this.OnClosing = () =>
			{
				dispatcherTimer.Tick -= TimeAction;
				dispatcherTimer.Stop();
				return true;
			};
			//StartAutoUpdate();
		}


		private void TimeAction(object sender, EventArgs e)
		{
			if (!Vm.IsBusy)
			{
				Vm.AutoUpdateInfo = $"此頁面2分鐘會自動更新，最後更新日{DateTime.Now}";
				Vm.SearchCommand.Execute(null);
			}
		}

    private void StartAutoUpdate()
    {
      //dispatcherTimer = new DispatcherTimer();
      //dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
      dispatcherTimer.Start();
      Vm.SearchCommand.Execute(null);
      this.Dispatcher.Invoke((Action)(() =>
      {
        tab2.IsEnabled = true;
        tab3.IsEnabled = true;
      }));
    }

    private void StopAutoUpdate(Boolean IsLockTab = true)
		{
			dispatcherTimer.Stop();
			this.Dispatcher.Invoke((Action)(() =>
			{
        if (IsLockTab)
        {
          tab2.IsEnabled = false;
          tab3.IsEnabled = false;
        }
			}));
		}

		private void TabItem_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl)
			{
				if (Vm.SelectedTabIndex == 0)
				{
					StartAutoUpdate();
				}
				else
				{
					StopAutoUpdate(false);
				}
			}
		}

		private void DoPrintReport()
		{
			// 重新抓取印表機設定
			var openDeviceWindow = OpenDeviceWindow(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedDcCode.Value);
			Vm.SelectedF910501 = openDeviceWindow.First();
			// 單一揀貨紙本
			if (Vm.SinglePickingReportDatas.Any())
			{
				Vm.SinglePickingReportDatas.ForEach(x =>
				{
					x.SerialNoBarcode = BarcodeConverter128.StringToBarcode(x.SERIAL_NO == null ? string.Empty : x.SERIAL_NO);
					x.PickOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.PICK_ORD_NO);
					x.WmsOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.WMS_ORD_NO);
					x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE);
					x.PickLocBarcode = BarcodeConverter128.StringToBarcode(x.PICK_LOC.Replace("-", ""));
				});
	
				var report1 = ReportHelper.CreateAndLoadReport<RP0501040001>();
				report1.SetDataSource(Vm.SinglePickingReportDatas.ToDataTable());
				report1.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().GupName;
				report1.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);
				report1.SetParameterValue("vnrCodeTitle", string.IsNullOrWhiteSpace(Vm.SinglePickingReportDatas[0].RTN_VNR_CODE)?string.Empty: "廠商編號");
				report1.SetParameterValue("vnrNameTitle", string.IsNullOrWhiteSpace(Vm.SinglePickingReportDatas[0].RTN_VNR_CODE) ? string.Empty : "廠商名稱");
				PrintReport(report1, Vm.SelectedF910501,PrintType.ToPrinter);
			}

			// 批量揀貨紙本
			if(Vm.BatchPickingReportDatas.Any())
			{
				Vm.BatchPickingReportDatas.ForEach(x =>
				{
					x.SerialNoBarcode = BarcodeConverter128.StringToBarcode(x.SERIAL_NO == null ? string.Empty : x.SERIAL_NO);
					x.PickOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.PICK_ORD_NO);
					x.WmsOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.WMS_ORD_NO);
					x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE);
					x.PickLocBarcode = BarcodeConverter128.StringToBarcode(x.PICK_LOC.Replace("-", ""));
				});

				var report1 = ReportHelper.CreateAndLoadReport<RP0501040001>();
				report1.SetDataSource(Vm.BatchPickingReportDatas.ToDataTable());
				report1.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().GupName;
				report1.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);
				report1.SetParameterValue("vnrCodeTitle", string.IsNullOrWhiteSpace(Vm.BatchPickingReportDatas[0].RTN_VNR_CODE) ? string.Empty : "廠商編號");
				report1.SetParameterValue("vnrNameTitle", string.IsNullOrWhiteSpace(Vm.BatchPickingReportDatas[0].RTN_VNR_CODE) ? string.Empty : "廠商名稱");
				PrintReport(report1, Vm.SelectedF910501, PrintType.ToPrinter);
			}

			// 單一揀貨貼紙
			if (Vm.SinglePickingTickerDatas.Any())
			{
				var list2 = Vm.SinglePickingTickerDatas;
				list2.ForEach(x =>
				{
					x.PickOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.PICK_ORD_NO);
				});
				var report2 = ReportHelper.CreateAndLoadReport<RP0501040002>();
				report2.SetDataSource(list2.ToDataTable());
				report2.SetParameterValue("vnrCodeTitle", string.IsNullOrWhiteSpace(Vm.SinglePickingTickerDatas[0].RTN_VNR_CODE) ? string.Empty : "廠商編號");
				PrintReport(report2, Vm.SelectedF910501, PrintType.ToPrinter, PrinterType.Label);
			}

			// 批量揀貨貼紙
			if (Vm.BatchPickingTickerDatas.Any())
			{
				var list3 = Vm.BatchPickingTickerDatas;
				list3.ForEach(x =>
				{
					x.PickOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.PICK_ORD_NO);
				});
				var report3 = ReportHelper.CreateAndLoadReport<RP0501040003>();
				report3.SetDataSource(list3.ToDataTable());
				PrintReport(report3, Vm.SelectedF910501, PrintType.ToPrinter, PrinterType.Label);
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// 在切換 Tab 時，UserControl 會重新觸發 Loaded，故需判斷是否已經設定過
			if (Vm.SelectedF910501 == null)
				ShowDeviceWindowAndSetF910501();
		}

		void ShowDeviceWindowAndSetF910501()
		{
			var openDeviceWindow = OpenDeviceWindow(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedDcCode.Value);
			if (openDeviceWindow.Any())
			{
				Vm.SelectedF910501 = openDeviceWindow.FirstOrDefault();
			}
			else
			{
				var deviceWindow = new DeviceWindow(Vm.SelectedDcCode.Value);
				deviceWindow.Owner = this.Parent as Window;
				deviceWindow.ShowDialog();
				Vm.SelectedF910501 = deviceWindow.SelectedF910501;
			}

		}
		
	}
}