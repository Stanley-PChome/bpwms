using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;

namespace Wms3pl.WpfClient.P08.Report
{
	public class DeliveryNoteDataForReport : DeliveryNoteData
	{
		public string DELV_DATE_TEXT { get; set; }//出貨日期文字(F050801)

		public string RECEIPT_NO_TEXT { get; set; }//發票號碼文字(F050301)，RECEIPT_NO 或 RECEIPT_NO_HELP
		public string StockNoBarcode { get; set; }
	}

	public class DeliveryNoteSubDataForReport : DeliveryNoteSubData
	{

	}

	public class DeliveryNoteSubDataForReportA : DeliveryNoteSubDataA
	{

	}

	public class DeliveryNoteSubDataForReportB : DeliveryNoteSubDataB
	{
		
	}
}
