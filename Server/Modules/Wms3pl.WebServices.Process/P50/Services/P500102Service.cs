
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F50;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
namespace Wms3pl.WebServices.Process.P50.Services
{
	public partial class P500102Service
	{
		private WmsTransaction _wmsTransaction;
		public P500102Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult InsertF500101(F500101 f500101)
		{
			var sharedService = new SharedService();
			var quoteNo = sharedService.GetNewOrdCode("Q"); //領用單號

			var repF500101 = new F500101Repository(Schemas.CoreSchema, _wmsTransaction);
			f500101.QUOTE_NO = quoteNo;
			f500101.CRT_DATE = DateTime.Now;
			repF500101.Add(f500101);

			return new ExecuteResult(true, quoteNo);
		}

		public ExecuteResult UpdateF500101Status(F500101 f500101, string statusType)
		{
			var sharedService = new SharedService();
			var repF500101 = new F500101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500101Item = repF500101.Find(o => o.QUOTE_NO == f500101.QUOTE_NO && o.DC_CODE == f500101.DC_CODE
							&& o.GUP_CODE == f500101.GUP_CODE && o.CUST_CODE == f500101.CUST_CODE);
			if (f500101Item != null)
			{
				f500101Item.STATUS = statusType;
				repF500101.Update(f500101Item);
			}
			return new ExecuteResult(true, f500101Item.QUOTE_NO);
		}

		public ExecuteResult UpdateF500101(F500101 f500101)
		{			
			var repF500101 = new F500101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500101Item = repF500101.Find(o => o.QUOTE_NO == f500101.QUOTE_NO && o.DC_CODE == f500101.DC_CODE
							&& o.GUP_CODE == f500101.GUP_CODE && o.CUST_CODE == f500101.CUST_CODE);
			if (f500101Item != null)
			{
				f500101Item.ENABLE_DATE = f500101.ENABLE_DATE;
				f500101Item.DISABLE_DATE = f500101.DISABLE_DATE;
				f500101Item.NET_RATE = f500101.NET_RATE;
				f500101Item.APPROV_UNIT_FEE = f500101.APPROV_UNIT_FEE;
				f500101Item.MEMO = f500101.MEMO;
				repF500101.Update(f500101Item);
			}
			return new ExecuteResult(true, f500101Item.QUOTE_NO);
		}

		public ExecuteResult DeleteF500101(string dcCode, string gupCode, string custCode, string quoteNo)
		{			
			var repF500101 = new F500101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500101Item = repF500101.Find(o => o.QUOTE_NO == quoteNo && o.DC_CODE == dcCode
							&& o.GUP_CODE == gupCode && o.CUST_CODE == custCode);
			if (f500101Item != null)
			{
				f500101Item.STATUS = "9";
				repF500101.Update(f500101Item);
			}
			return new ExecuteResult(true, f500101Item.QUOTE_NO);
		}

		public IQueryable<F500101QueryData> GetF500101QueryData(string dcCode,string gupCode,string custCode, string enableSDate, string enableEDate, string quoteNo, string status)
		{
			//有效日期
			var coverCrtSDate = (string.IsNullOrEmpty(enableSDate)) ? ((DateTime?)null) : Convert.ToDateTime(enableSDate);
			var coverCrtEDate = (string.IsNullOrEmpty(enableEDate)) ? ((DateTime?)null) : Convert.ToDateTime(enableEDate);


			var repF500101 = new F500101Repository(Schemas.CoreSchema);
			return repF500101.GetF500101QueryData(dcCode,gupCode,custCode, coverCrtSDate, coverCrtEDate, quoteNo, status);
		}

	}
}
