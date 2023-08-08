using CrystalDecisions.CrystalReports.Engine;
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
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices.P71WcfService;
using Wms3pl.WpfClient.P71.Report;
using Wms3pl.WpfClient.P71.ViewModel;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7101080000.xaml 的互動邏輯
	/// </summary>
	public partial class P7101080000 : Wms3plUserControl
	{
		public P7101080000()
		{
			InitializeComponent();
			Vm.DoPrint += GetReport;
			Vm.OnScrollIntoViewItems += ScrollIntoViewItems;
			Vm.OnDcCodeChanged += DcCodeChanged;
		}

		/// <summary>
		/// 列印儲位卡
		/// </summary>
		/// <param name="printType"></param>
		private void GetReport(PrintType printType, PrintBy selectPrintBy, ReportClass rc)
		{
			//取得 [儲位卡] 之查詢條件
			if (selectPrintBy == PrintBy.a4Printer && Vm.selectSizeSource.Value == "1")
			{
				var list = Vm.LocListR71010803;
				if (list == null || list.Count == 0)
				{
					ShowMessage(Properties.Resources.P7101080000xamlcs_listNoData);
					return;
				}

				var temp = (from a in list
							select new F1912DataLocByLocType
							{ AREA_NAME = a.AREA_NAME,
								BARCODE = BarcodeConverter128.StringToBarcode(a.BARCODE),
								LOC = a.LOC,
								LOC_CODE_TYPE = a.LOC_CODE_TYPE,
								WAREHOUSE_NAME = a.WAREHOUSE_NAME
							}).ToDataTable();
				rc.SetDataSource(temp);
			}
			else
			{
				var list = Vm.LocList;
				if (list == null || list.Count == 0)
				{
					ShowMessage(Properties.Resources.P7101080000xamlcs_listNoData);
					return;
				}

                bool locCodeIsFormat = selectPrintBy == PrintBy.labelPrinter && Vm.selectSizeSource.Value == "2";
                
                var temp = (from a in list
							select new F1912DataLoc { AREA = a.AREA, BARCODE = BarcodeConverter128.StringToBarcode(a.BARCODE), LOC = locCodeIsFormat ? LocCodeFormat(a.LOC) : a.LOC })
					.ToDataTable();
				rc.SetDataSource(temp);
			}

            if (printType == PrintType.ToPrinter)
            {
                PrinterType printerType = PrinterType.A4;
                switch (selectPrintBy)
                {
                    case PrintBy.a4Printer:
                        printerType = PrinterType.A4;
                        break;
                    case PrintBy.labelPrinter:
                        printerType = PrinterType.Label;
                        break;
                    case PrintBy.bartender:
                        break;
                }
                Vm.PrintReport(rc, Vm.SelectedF910501, printerType);
            }
            else
            {
                var win = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
                win.CallReport(rc, printType);
            }
        }

        private string LocCodeFormat(string locCode)
        {
            List<string> res = new List<string>();
            string currStr = string.Empty;
            for (int i = locCode.Length; i > 0; i -= 2)
            {
                currStr = string.Empty;
                if (i - 2 < 0)
                    currStr = locCode.Substring(0, 1);
                else
                    currStr = locCode.Substring(i - 2, 2);

                res.Insert(0, currStr);
            }
                
            return string.Join("-", res);
        }

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			TextBox obj = ((TextBox)sender);
			if (obj.Name == "txtLocCodeStart" || obj.Name == "txtLocCodeEnd")
				Vm.ChoseOption = ChoseOptionType.LocCode;
			else
				Vm.ChoseOption = ChoseOptionType.ItemCode;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Vm.ChoseOption = ChoseOptionType.ItemCode;
		}


		public void ScrollIntoViewItems()
		{
			ScrollIntoView(dgItems, Vm.SelectedItem);
		}

		private void Window_OnLoaded(object sender, RoutedEventArgs e)
		{
			if (Vm.SelectedF910501 != null)
				return;

			DcCodeChanged();
		}

		private void DcCodeChanged()
		{
            var openDeviceWindow = OpenDeviceWindow(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedDc);
            if (openDeviceWindow.Any())
            {
                Vm.SelectedF910501 = openDeviceWindow.FirstOrDefault();
            }
            else
            {
                var deviceWindow = new DeviceWindow(Vm.SelectedDc);
                deviceWindow.Owner = this.Parent as Window;
                deviceWindow.ShowDialog();
                Vm.SelectedF910501 = deviceWindow.SelectedF910501;
            }
            
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
				Vm.SearchItemCode = winSearchProduct.SelectData.ITEM_CODE;
				SetFocusedElement(txtItemCode);
			}
		}
	}
}
