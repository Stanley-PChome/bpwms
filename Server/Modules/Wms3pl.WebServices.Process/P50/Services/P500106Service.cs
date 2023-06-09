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
	public partial class P500106Service
	{
		private WmsTransaction _wmsTransaction;
		public P500106Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult InsertF500105(F500105 f500105)
		{
			var sharedService = new SharedService();
			var quoteNo = sharedService.GetNewOrdCode("Q"); //領用單號

			var repF500105 = new F500105Repository(Schemas.CoreSchema, _wmsTransaction);
			f500105.QUOTE_NO = quoteNo;
			f500105.CRT_DATE = DateTime.Now;
			repF500105.Add(f500105);

			return new ExecuteResult(true, quoteNo);
		}

		public ExecuteResult UpdateF500105Status(F500105 f500105, string statusType)
		{
			var repF500105 = new F500105Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500105Item = repF500105.Find(o => o.QUOTE_NO == f500105.QUOTE_NO && o.DC_CODE == f500105.DC_CODE
							&& o.GUP_CODE == f500105.GUP_CODE && o.CUST_CODE == f500105.CUST_CODE);
			if (f500105Item != null)
			{
				f500105Item.STATUS = statusType;
				repF500105.Update(f500105Item);
			}
			return new ExecuteResult(true, f500105Item.QUOTE_NO);
		}

		public ExecuteResult UpdateF500105(F500105 f500105)
		{
			var repF500105 = new F500105Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500105Item = repF500105.Find(o => o.QUOTE_NO == f500105.QUOTE_NO && o.DC_CODE == f500105.DC_CODE
							&& o.GUP_CODE == f500105.GUP_CODE && o.CUST_CODE == f500105.CUST_CODE);
			if (f500105Item != null)
			{
				f500105Item.ENABLE_DATE = f500105.ENABLE_DATE;
				f500105Item.DISABLE_DATE = f500105.DISABLE_DATE;
				f500105Item.NET_RATE = f500105.NET_RATE;
				f500105Item.APPROV_FEE = f500105.APPROV_FEE;			
				f500105Item.MEMO = f500105.MEMO;
				repF500105.Update(f500105Item);
			}
			return new ExecuteResult(true, f500105Item.QUOTE_NO);
		}

		public ExecuteResult DeleteF500105(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var repF500105 = new F500105Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500105Item = repF500105.Find(o => o.QUOTE_NO == quoteNo && o.DC_CODE == dcCode
							&& o.GUP_CODE == gupCode && o.CUST_CODE == custCode);
			if (f500105Item != null)
			{
				f500105Item.STATUS = "9";
				repF500105.Update(f500105Item);
			}
			return new ExecuteResult(true, f500105Item.QUOTE_NO);
		}

		public IQueryable<F500105QueryData> GetF500105QueryData(string dcCode, string enableSDate, string enableEDate, string quoteNo, string status)
		{
			//有效日期
			var coverCrtSDate = (string.IsNullOrEmpty(enableSDate)) ? ((DateTime?)null) : Convert.ToDateTime(enableSDate);
			var coverCrtEDate = (string.IsNullOrEmpty(enableEDate)) ? ((DateTime?)null) : Convert.ToDateTime(enableEDate);


			var repF500105 = new F500105Repository(Schemas.CoreSchema);
			return repF500105.GetF500105QueryData(dcCode, coverCrtSDate, coverCrtEDate, quoteNo, status);
		}
	}
}
