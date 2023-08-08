using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using PaperSize = CrystalDecisions.Shared.PaperSize;
using PrintDialog = System.Windows.Forms.PrintDialog;

namespace Wms3pl.WpfClient.UILib
{
  /// <summary>
  /// Interaction logic for Wms3plViewer.xaml
  /// </summary>
  public partial class Wms3plViewer : Window
  {
    public Wms3plViewer()
    {
      InitializeComponent();
    }

    public void CallReport(ReportClass report, PrintType printType)
    {
			using (report)
			{
				using (ReportsViewer)
				{
					//增加先跳出設定視窗
					//var dialog = new PrintDialog();
					//var printSetting = new PrinterSettings();
					//var pageSetting = new PageSettings();
					//if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					//{
					//  printSetting = dialog.PrinterSettings;
					//  pageSetting = new PageSettings(dialog.PrinterSettings);
					//  report.PrintOptions.CopyFrom(printSetting, pageSetting);
					//}

					if (printType == PrintType.Preview)
					{
						ReportsViewer.ReportSource = report;
						ShowDialog();
					}
					else if (printType == PrintType.ToPrinter)
					{
						//report.PrintToPrinter(printSetting, pageSetting, true);
						ReportsViewer.ReportSource = report;
						ReportsViewer.PrintReport();
					}
				}
			}
    }
  }
}
