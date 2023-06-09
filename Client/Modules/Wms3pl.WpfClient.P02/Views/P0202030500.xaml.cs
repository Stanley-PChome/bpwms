using System.Windows;
using Wms3pl.WpfClient.LabelPrinter.Bartender;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.ShareExDataService.ExecuteResult;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.Common;
using System;
using CrystalDecisions.CrystalReports.Engine;

namespace Wms3pl.WpfClient.P02.Views
{
	/// <summary>
	/// P0202030500.xaml 的互動邏輯
	/// </summary>
	public partial class P0202030500 : Wms3plWindow
	{

		public P0202030500(string dcCode, P020203Data data,string selectedPurchaseNo)
		{
			InitializeComponent();
			Vm.SelectedDc = dcCode;
			Vm.BaseData = data;
            //取得進倉單號
            Vm.SelectedPurchaseNo = selectedPurchaseNo;
            Vm.DoPrintReport += PrintReport;
			Vm.InitControls();
		}
		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			if (Vm.ShowMessage(Messages.WarningBeforeExit) == UILib.Services.DialogResponse.OK)
				this.Close();
		}
        /// <summary>
        /// 列印貼紙
        /// </summary>
        /// <param name="printType"></param>
		private void PrintReport(PrintType printType)
        {
            var info = Wms3plSession.Get<GlobalInfo>();
            var lang = string.Empty;
            if (info != null && !string.IsNullOrEmpty(info.Lang) && info.Lang.ToUpper() != "ZH-TW")
                lang = info.Lang;
            var reportNameSpaceFormat = "Wms3pl.WpfClient.P02.Report.{0}";
            var reportName = "RP0202030502.rpt";
            var type = Type.GetType(string.Format(reportNameSpaceFormat, reportName.Replace(".rpt", "")));
            var reportFullTypeName = $"{type.FullName}{lang.Replace("-", "_")},{type.Assembly.FullName}";
            var reportFileName = $"{type.Name}.rpt";
            if (!string.IsNullOrEmpty(lang))
                reportFileName = $"{type.Name}{lang.Replace("-", "_")}.rpt";
            var report = Activator.CreateInstance(Type.GetType(reportFullTypeName)) as ReportClass;
            report.Load(reportFileName);
            report.SetDataSource(Vm.ReportData);

            var crystalReportService = new CrystalReportService(report);
            switch (printType)
            {
                case PrintType.Preview:
                    crystalReportService.ShowReport(this, PrintType.Preview);
                    break;
                case PrintType.ToPrinter:
                    crystalReportService.PrintToPrinter(PrinterType.Label);
                    break;
                default:
                    break;
            }
            
        }
	}
}
