using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F91DataService;

namespace Wms3pl.WpfClient.Services
{
	public class PrintPdfService
	{
		public void Print(byte[] fileByteArray,string functionCode,string clientIp, string dcCode,string printName,Int16 copies)
		{
			try
			{
				PdfDocument doc = new PdfDocument();
				doc.LoadFromBytes(fileByteArray);

				doc.PrintSettings.PrinterName = printName;

				//設定列印份數為1份
				doc.PrintSettings.Copies = copies;

				//靜默列印PDF檔案
				doc.PrintSettings.PrintController = new StandardPrintController();

				//doc.PrintSettings.EndPrint += PrintSettings_EndPrint;


				//使用預設印表機列印檔案所有頁面
				doc.Print();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		//private void PrintSettings_EndPrint(object sender, PrintEventArgs e)
		//{
		//	MessageBox.Show("列印成功!");
		//}
	}
}
