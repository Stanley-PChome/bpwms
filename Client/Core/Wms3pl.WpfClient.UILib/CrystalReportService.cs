// Wms3pl.WpfClient.UILib
// Author: Charles@BankPro 
// 
// Final Update: 2012/04/26
// Copyright@2012

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F91DataService;
using System;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.Common.Helpers;

namespace Wms3pl.WpfClient.UILib
{
	public partial class CrystalReportService
	{
		private readonly ReportClass _report;
		/// <summary>
		/// 印表機Device。 若要直接印，不在額外選印表機，則要設定印表機Device。
		/// </summary>
		private readonly F910501 _f910501;

		private string _functionCode = "CrystalReportService";
		public CrystalReportService(ReportClass report)
		{
			_report = report;
		}

		/// <summary>
		/// 若要直接由程式設定印表機，請使用這個建構式
		/// </summary>
		/// <param name="report">報表</param>
		/// <param name="f910501">設定印表機</param>
		public CrystalReportService(ReportClass report, F910501 f910501)
			: this(report)
		{
			_f910501 = f910501;
		}

		public ReportClass Report
		{
			get { return _report; }
		}

		public void SetText(string textFieldName, string text)
		{
			_report.SetText(textFieldName, text);
		}

		public void ShowReport(Window owner, PrintType printType)
		{
			if (owner == null) owner = Application.Current.MainWindow;
			var win = new Wms3plViewer { Owner = owner };
			win.CallReport(_report, printType);
		}

		public void ShowReport(FrameworkElement view, PrintType printType)
		{
			var window = GetOwnerWindow(view);
			ShowReport(window, printType);
		}

		private Window GetOwnerWindow(FrameworkElement view)
		{
			if (view == null) return null;
			if (view is Window)
				return view as Window;
			else if (view is Wms3plPage)
				return (view as Wms3plPage).Parent as Window;
			else
			{
				return Window.GetWindow(view);
			}
		}

		/// <summary>
		/// 直接依照設定的印表機列印
		/// </summary>
		/// <param name="formatType">預設為A4，使用F910501的預設印表機，若F910501為NULL，則直接以預設值列印</param>
		/// <param name="nCopies">指出列印份數。</param>
		/// <param name="collated">指出是否自動分頁。</param>
		/// <param name="startPageN">指出要列印的第一頁。</param>
		/// <param name="endPageN">指出要列印的最後一頁。</param>
		public void PrintToPrinter(PrinterType formatType = PrinterType.A4, int nCopies = 1, bool collated = true, int startPageN = 0, int endPageN = 0)
		{
			var printerName = GetPrinterName(formatType);

			if (printerName != null)
			{
				_report.PrintOptions.PrinterName = printerName;

				// 若要直接印，報表必須先設定好預設印表機
				if (!string.IsNullOrWhiteSpace(printerName) && string.IsNullOrWhiteSpace(_report.PrintOptions.PrinterName))
				{
					throw new Exception(string.Format("{0} 報表尚未設定預設印表機\r\n無法由程式直接指定印表機", _report.ResourceName));
				}
			}

			//特殊紙張需指定列印格式
			var printerLayout = GetReportFormat(_report, GetPrintFormat(_functionCode));
			if (!string.IsNullOrEmpty(printerLayout))
			{
				int paperId = GetPaperId(printerLayout, _report.PrintOptions.PrinterName);
				_report.PrintOptions.PaperSize = (PaperSize)paperId;
			}

			try
			{
				_report.PrintToPrinter(nCopies, collated, startPageN, endPageN);
			}
			catch (System.Drawing.Printing.InvalidPrinterException invalidPrinterException)
			{
				var printerList = CommonDataHelper.PrinterList();
				var errorMsg = string.Format("目前存在的印表機:{0}", string.Join("\r\n", printerList.Select(x => x.Value)));
				throw new Exception(errorMsg, invalidPrinterException);
			}
			finally
			{
				_report.Close();
			}
		}

		/// <summary>
		/// 從 F910501 取得要直接列印的印表機，若沒有則使用預設列印
		/// </summary>
		/// <param name="formatType"></param>
		/// <returns></returns>
		private string GetPrinterName(PrinterType formatType)
		{
			if (_f910501 == null)
				return null;

			switch (formatType)
			{
				case PrinterType.Matrix:
					return _f910501.MATRIX_PRINTER;
				case PrinterType.Label:
					return _f910501.LABELING;
				default:
					return _f910501.PRINTER;
			}
		}

		static List<NameValuePair<string>> _printFormatDatas;
		private static List<NameValuePair<string>> GetPrintFormat(string functionCode)
		{
			if (_printFormatDatas == null)
				_printFormatDatas = GetBaseTableService.GetF000904List(functionCode, "PRINT_FORMAT", "");

			return _printFormatDatas;
		}

		private static string GetReportFormat(ReportClass report, List<NameValuePair<string>> printFormatList)
		{
			var format = printFormatList.FirstOrDefault(n => n.Name == report.ResourceName);
			if (format != null)
			{
				return format.Value;
			}
			return string.Empty;
		}

		/// <summary>
		/// 取得列印伺服器內容的列印格式編號
		/// </summary>
		/// <param name="paperName">印表機名稱</param>
		/// <param name="printerName">列印格式</param>
		/// <returns></returns>
		private static int GetPaperId(string paperName, string printerName)
		{
			var doctoprint = new System.Drawing.Printing.PrintDocument { PrinterSettings = { PrinterName = printerName } };

			var paperSize = new System.Drawing.Printing.PaperSize[1000];
			doctoprint.PrinterSettings.PaperSizes.CopyTo(paperSize, 0);

			var selectPrint = paperSize.FirstOrDefault(x => x.PaperName == paperName);
			if (selectPrint != null)
			{
				return
					Convert.ToInt32(
						selectPrint.GetType().GetField("kind",
							System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).
							GetValue(selectPrint));
			}
			return 0;
		}
	}
}