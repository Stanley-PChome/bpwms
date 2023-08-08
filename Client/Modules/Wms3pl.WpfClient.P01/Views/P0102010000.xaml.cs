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
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.P01.Report;
using Wms3pl.WpfClient.P01.ViewModel;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P01.Views
{
	/// <summary>
	/// P0102010000.xaml 的互動邏輯
	/// </summary>
	public partial class P0102010000 : Wms3plUserControl
	{
		public P0102010000()
		{
			InitializeComponent();
			Vm.SetTxtVnrCodeFocusClick += SetTxtVnrCodeFocusClick;
			Vm.SetTxtCustOrdNoFocusClick += SetTxtCustOrdNoFocusClick;
			Vm.ExcelImport += ExcelImport;
			Vm.DoPrintPalletReport += DoPrintPalletReport;
			//Vm.OnDcCodeChanged += DcCodeChanged;
		}

        private void VnrCode_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.SetVnrInfo();
		}

		private void SetTxtVnrCodeFocusClick()
		{
			SetFocusedElement(TxtVnrCode);
		}
		private void SetTxtCustOrdNoFocusClick()
		{
			SetFocusedElement(TxtCustOrdNo);
		}

		private void VnrCode_LostFocus(object sender, RoutedEventArgs e)
		{
			Vm.CheckVnrCode();
		}

        bool ImportResultData = false;
        private void ExcelImport()
		{
            var win = new WinImportSample(string.Format("{0},{1}", Vm.CustCode, "P0102010000"));
           
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
                    DialogService.ShowMessage("進倉單匯入檔必須為Excel檔案，總共有7欄");
                    dlg = null;
                    return "";
                }
                return dlg.FileName;
			}
			return "";
		}

		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (Vm.AddOrModifyF010202Data != null)
			{
				string msg = Vm.CheckValidQty(Vm.StockQty, Vm.AddOrModifyF010202Data.ITEM_CODE);
				if (!string.IsNullOrEmpty(msg))
				{
					Vm.ShowWarningMessage(msg);
				}
			}
		}

		/// <summary>
		/// 列印棧板標籤
		/// </summary>
		private void DoPrintPalletReport()
		{
			var info = Wms3plSession.Get<GlobalInfo>();
			var lang = string.Empty;
			if (info != null && !string.IsNullOrEmpty(info.Lang) && info.Lang.ToUpper() != "ZH-TW")
				lang = info.Lang;
			var reportNameSpaceFormat = "Wms3pl.WpfClient.P01.Report.{0}";
			var reportName = "RP0102010001.rpt";
			var type = Type.GetType(string.Format(reportNameSpaceFormat, reportName.Replace(".rpt", "")));
			var reportFullTypeName = $"{type.FullName}{lang.Replace("-", "_")},{type.Assembly.FullName}";
			var reportFileName = $"{type.Name}.rpt";
			if (!string.IsNullOrEmpty(lang))
				reportFileName = $"{type.Name}{lang.Replace("-", "_")}.rpt";
			var report = Activator.CreateInstance(Type.GetType(reportFullTypeName)) as ReportClass;
			report.Load(reportFileName);
			report.SetDataSource(Vm.PalletDatas);

			var crystalReportService = new CrystalReportService(report);
			crystalReportService.ShowReport(this,PrintType.Preview);

		}

		private void DcCodeChanged()
		{
			var deviceWindow = new DeviceWindow(Vm.SelectedDcCode);	
			deviceWindow.ShowDialog();
			Vm.SelectedF910501 = deviceWindow.SelectedF910501;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//DcCodeChanged();
		}
	}
}
