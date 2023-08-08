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
using Microsoft.Win32;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.ExDataServices.P14ExDataService;
using Wms3pl.WpfClient.P14.Report;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P14.Views
{
	/// <summary>
	/// P1401010000.xaml 的互動邏輯
	/// </summary>
	public partial class P1401010000 : Wms3plUserControl
	{
		public P1401010000()
		{
			InitializeComponent();
			Vm.DgScorllInView += dgScorllInView;
			Vm.DoReportShow += DoReport;
            Vm.ExcelImportItem += ExcelImportItem;
            Vm.ExcelImportInventoryDetailItem += ExcelImportInventoryDetailItem;
        }
		private void AddOrEditDc_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				Vm.DoSearchWareHouse();
				Vm.SetIsCanCharge();
				Vm.SetAddOrEditWareHouseList();
			}
		}
		private void InventoryDate_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				Vm.SetInventoryName();
			}
		}
		private void InventoryType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				if (Vm.AddOrEditF140101.INVENTORY_TYPE != "1")
				{
					Vm.AddOrEditF140101.INVENTORY_CYCLE = null;
					Vm.AddOrEditF140101.INVENTORY_YEAR = null;
					Vm.AddOrEditF140101.INVENTORY_MON = null;
					Vm.AddOrEditF140101.CYCLE_TIMES = null;
				}
				else
				{
					Vm.AddOrEditF140101.INVENTORY_CYCLE = (short?)DateTime.Now.DayOfWeek;
					Vm.AddOrEditF140101.INVENTORY_YEAR = (short?)DateTime.Now.Year;
					Vm.AddOrEditF140101.INVENTORY_MON = (short?)DateTime.Now.Month;
					Vm.SetIsCanSetInventoryCycle();
				}
                Vm.InventoryItemList = new List<InventoryItem>();
                Vm.SetInventoryName();
				Vm.SetIsCanCharge();
            }
		}

		private void InventoryYear_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				Vm.SetInventoryMonthList();
				Vm.SetInventoryName();
			}
		}
		private void InventoryMonth_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				Vm.SetInventoryName();
				Vm.SetIsCanSetInventoryCycle();
			}

		}


		private void IsCharge_OnUnChecked(object sender, RoutedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				if (Vm.AddOrEditF140101.ISCHARGE == "0")
					Vm.AddOrEditF140101.FEE = null;
			}
		}

		private void WareHouseType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				Vm.DoSearchWareHouse();
			}
		}


		private void CheckAllWareHouse_OnChecked(object sender, RoutedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
				Vm.CheckAllWareHouse();
		}

		private void WareHouseItemCheck_OnHandler(object sender, RoutedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				var checkBox = sender as CheckBox;

				if (!checkBox.IsChecked ?? false)
				{
					var dep = (DependencyObject)e.OriginalSource;
					while ((dep != null) &&
							!(dep is DataGridRow))
					{
						dep = VisualTreeHelper.GetParent(dep);
					}
					if (dep == null) return;
					DataGridRow row = dep as DataGridRow;
					var item = row.Item as InventoryWareHouse;
                    item.FLOOR_BEGIN = null;
                    item.FLOOR_END = null;
					item.CHANNEL_BEGIN = null;
					item.CHANNEL_END = null;
                    item.PLAIN_BEGIN = null;
                    item.PLAIN_END = null;
				}
			}
		}

		private void CheckAllItem_OnChecked(object sender, RoutedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
				Vm.CheckAllItem();
		}

		
		private void CheckAllEditInventoryDetailItem_OnChecked(object sender, RoutedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Edit)
				Vm.CheckAllEditInventoryDetailItem();
		}

		private void InventoryCycle_OnKeyUp(object sender, KeyEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
				Vm.SetInventoryName();
		}

		private void CycleTimes_OnKeyUp(object sender, KeyEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
				Vm.SetInventoryName();
		}

		private void dgScorllInView()
		{
			dgGroupList.ScrollIntoView(dgGroupList.SelectedItem);
		}

		private void DoReport(PrintType printType)
		{
			var win = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };

			switch (Vm.SelectedReportType)
			{
				case "01"://盤點單
					var list = Vm.ReportDataList;
					if (list == null || list.Count == 0)
					{
						ShowMessage(Properties.Resources.P1401010000xamlcs_QueryDataEmpty);
						return;
					}
					var report = new R140102();
					report.Load(@"R140102.rpt");
					report.SetDataSource(list);
					report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().GupName + "－" +
																					 Wms3plSession.Get<GlobalInfo>().CustName +
					Properties.Resources.P1401010000xamlcs_Inventory;
					report.SetParameterValue("PRINT_STAFF_NAME", Wms3plSession.Get<UserInfo>().AccountName);
					report.SetParameterValue("INVENTORY_DATE", Vm.SelectedF140101.INVENTORY_DATE.ToString("yyyy/MM/dd"));
					win.CallReport(report, printType);
					break;
				case "02"://盤點清冊
					var list2 = Vm.ReportDataList2;
					if (list2 == null || list2.Count == 0)
					{
						ShowMessage(Properties.Resources.P1401010000xamlcs_QueryDataEmpty);
						return;
					}
					var report2 = new R14010201();
					report2.Load(@"R14010201.rpt");
					report2.SetDataSource(list2);
					report2.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().GupName + "－" +
																					 Wms3plSession.Get<GlobalInfo>().CustName +
					Properties.Resources.P1401010000xamlcs_IventoryList;
					report2.SetParameterValue("PRINT_STAFF_NAME", Wms3plSession.Get<UserInfo>().AccountName);
					win.CallReport(report2, printType);
					break;
			}
		}

        private void ExcelImportInventoryDetailItem()
        {
            bool ImportResultData = false;
            var win = new WinImportSample(string.Format("{0},{1}", Vm.CustCode, "P1401010000"), "BP1401010016.xlsx");

            win.ImportResult = (t) => { ImportResultData = t; };
            win.ShowDialog();
            Vm.ImportInventoryDetailItemFilePath = null;
            if (ImportResultData)
                Vm.ImportInventoryDetailItemFilePath = OpenInventoryDetailItemFileDialogFun();
        }

        private string OpenInventoryDetailItemFileDialogFun()
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".xls",
                Filter = "excel files (*.xls,*.xlsx)|*.xls*"
            };

            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }
            return "";
        }

    private void ExcelImportItem(string buttonID)
    {
      bool ImportResultData = false;

      WinImportSample win;
      if (buttonID == "BP1401010013")
        win = new WinImportSample(string.Format("{0},{1}", Vm.CustCode, "P1401010000"), "BP1401010013.xlsx");
      else if (buttonID == "BP1401010021")
        win = new WinImportSample(string.Format("{0},{1}", Vm.CustCode, "P1401010000"), "BP1401010021.xlsx");
      else
        return;

      win.ImportResult = (t) => { ImportResultData = t; };
      win.ShowDialog();
      Vm.ImportItemCodeFilePath = null;
      if (ImportResultData)
        Vm.ImportItemCodeFilePath = OpenItemFileDialogFun();
      //bool ImportResultData = false;
      //var win = new WinImportSample(string.Format("{0},{1}", Vm.CustCode, "P0102010000", "BP1401010013"));

      //win.ImportResult = (t) => { ImportResultData = t; };
      //win.ShowDialog();
      //Vm.ImportFilePath = null;
      //if (ImportResultData)
      //{
      //    var dlg = new OpenFileDialog { DefaultExt = ".xls", Filter = "excel files (*.xls,*.xlsx)|*.xls*" };
      //    if (dlg.ShowDialog() ?? false)
      //    {
      //        try
      //        {
      //            Vm.ImportItemCodeFilePath = dlg.FileName;
      //            Vm.ExcelImportItemCommand.Execute(null);
      //        }
      //        catch (Exception ex)
      //        {
      //            var errorMsg = ErrorHandleHelper.GetCustomErrorCodeDescription(ex, Properties.Resources.P1401010000xamlcs_ImportFail, true);
      //            Vm.ShowWarningMessage(errorMsg);
      //        }
      //    }

      //    //Vm.ImportFilePath = OpenFileDialogFun();
      //}
    }

    private string OpenItemFileDialogFun()
    {
      var dlg = new OpenFileDialog
      {
        DefaultExt = ".xls",
        Filter = "excel files (*.xls,*.xlsx)|*.xls*"
      };

      if (dlg.ShowDialog() == true)
      {
        return dlg.FileName;
      }
      return "";
    }
  }


}
