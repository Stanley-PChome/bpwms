using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Entities
{
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F51ComplexReportData
	{
		public decimal ROWNUM { get; set; }
		public DateTime CAL_DATE { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string CHECK_ACCOUNT_TYPE { get; set; }
		public string ACC_ITEM_KIND_ID { get; set; }
		public string DELV_ACC_TYPE { get; set; }
		public string ITEM_TYPE_ID { get; set; }
		public string ACC_ITEM_NAME { get; set; }
		public string WMS_NO { get; set; }
		public string ITEM_CODE { get; set; }
		public string QTY { get; set; }
		public string AMT { get; set; }
		public string LOC_TYPE_ID { get; set; }
		public string TMPR_TYPE { get; set; }
		public string DELV_DATE { get; set; }
		public string PAST_NO { get; set; }
		public string INVOICE_CNT { get; set; }
		public string SA_QTY { get; set; }
		public string PACKAGE_BOX_NO { get; set; }
		public string ITEM_CODE_BOM { get; set; }
		public string PROCESS_ID { get; set; }
		public string TAKE_TIME { get; set; }
		public string DISTR_CAR_NO { get; set; }
		public string VOLUMN { get; set; }
		public string WEIGHT { get; set; }
		public string ZIP_CODE { get; set; }
		public string DELV_TMPR { get; set; }
		public string CAN_FAST { get; set; }
		public string DISTR_USE { get; set; }
		public string SP_CAR { get; set; }
		public string ALL_ID { get; set; }
		public string CHECK_ACCOUNT_TYPE_NAME { get; set; }
		public string DC_NAME { get; set; }
		public string GUP_NAME { get; set; }
		public string CUST_NAME { get; set; }
		public string LOC_TYPE_NAME { get; set; }
		public string TMPR_TYPE_NAME { get; set; }
		public string DELV_ACC_TYPE_NAME { get; set; }
		public string ITEM_NAME { get; set; }
		public string ITEM_NAME_BOM { get; set; }
		public string ALL_COMP { get; set; }

		public string DELV_TMPR_NAME { get; set; }
		public string DISTR_USE_NAME { get; set; }
	}

	
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class SettleMonFeeData
	{
		public decimal ROWNUM { get; set; }
		public string ACC_ITEM_NAME { get; set; }
		public int ACC_NUM { get; set; }
		public int? OVER_VALUE { get; set; }
		public decimal UNIT_FEE { get; set; }
		public decimal BASIC_FEE { get; set; }
		public decimal OVER_FEE { get; set; }	
		public int PRICE_CNT { get; set; }		
		public Decimal COST { get; set; }		
		public int AMOUNT { get; set; }		
		public string IN_TAX { get; set; }
		public string ACC_KIND { get; set; }
		public string OUTSOURCE_ID { get; set; }
		public string ACC_UNIT_NAME { get; set; }
	}

	[Serializable]
	[DataServiceKey("CAL_DATE", "DC_CODE", "GUP_CODE", "CUST_CODE", "WMS_NO", "ITEM_CODE")]
	public class TempStockSettleData
	{
		public DateTime CAL_DATE { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }		
		public string WMS_NO { get; set; }
		public string ORD_TYPE { get; set; }
		public string ITEM_CODE { get; set; }
		public Decimal QTY { get; set; }		
	}
}
