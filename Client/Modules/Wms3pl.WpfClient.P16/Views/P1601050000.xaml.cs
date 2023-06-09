using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Win32;
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
using Wms3pl.WpfClient.P16.Report;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.Views
{
	/// <summary>
	/// P1601050000.xaml 的互動邏輯
	/// </summary>
	public partial class P1601050000 : Wms3plUserControl
	{
		public P1601050000()
		{
			InitializeComponent();
			Vm.OnPrintCompleted += PrintCompleted;
			Vm.OnExportCompleted += ExportCompleted;
		}

		private void ExportCompleted(bool hasItems)
		{
			if (!hasItems)
			{
				Vm.ShowMessage(Messages.InfoNoData);
				return;
			}

			var saveFileDialog = new SaveFileDialog
			{
				DefaultExt = "txt",
				Filter = "save files (*.txt)|*.txt",
				RestoreDirectory = true,
				OverwritePrompt = true,
				Title = Properties.Resources.P1601050000xamlcs_Destpath
			};

			if (saveFileDialog.ShowDialog() != true)
				return;

			var lines = Vm.GetReportLines();
			if (lines == null)
				return;

			System.IO.File.WriteAllLines(saveFileDialog.FileName, lines);
		}


		private void PrintCompleted(PrintType printType, bool hasItems)
		{
			if (!hasItems)
			{
				Vm.ShowMessage(Messages.InfoNoData);
				return;
			}

			var report = GetReport();
			if (report == null)
				return;

            report.SetDataSource(GetReportData());
			report.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);

			var crystalReportService = new CrystalReportService(report);
			crystalReportService.ShowReport(this, printType);
		}

		ReportClass GetReport()
		{
			switch (Vm.ReportTypeForSearch)
			{
				case "2":	// RTO17840退貨記錄表
                    var report = new RP1602010003();
                    report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().GupName + "－" +
                                 Wms3plSession.Get<GlobalInfo>().CustName +
                                 Properties.Resources.RP1602010003_TITLE;
                    return report;
				case "3":	// B2C退貨記錄表(Friday退貨記錄表)
					return new RP1602010004();
				default:
					return null;
			}
		}
		IEnumerable<object> GetReportData()
		{
			switch (Vm.ReportTypeForSearch)
			{
				case "2":	// RTO17840退貨記錄表
					return Vm.RTO17840ReturnAuditReportList;
				case "3":	// B2C退貨記錄表(Friday退貨記錄表)
					return Vm.B2CReturnAuditReportList;
				default:
					return null;
			}
		}
	}
}
