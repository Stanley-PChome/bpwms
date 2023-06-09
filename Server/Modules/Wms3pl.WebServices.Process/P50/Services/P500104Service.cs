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
	public partial class P500104Service
	{
		private WmsTransaction _wmsTransaction;
		public P500104Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult InsertF500103(F500103 f500103)
		{
			var sharedService = new SharedService();
			var quoteNo = sharedService.GetNewOrdCode("Q"); //領用單號

			var repF500103 = new F500103Repository(Schemas.CoreSchema, _wmsTransaction);
			f500103.QUOTE_NO = quoteNo;
			f500103.CRT_DATE = DateTime.Now;
			repF500103.Add(f500103);

			return new ExecuteResult(true, quoteNo);
		}

		public ExecuteResult UpdateF500103Status(F500103 f500103, string statusType)
		{

			var repF500103 = new F500103Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500103Item = repF500103.Find(o => o.QUOTE_NO == f500103.QUOTE_NO && o.DC_CODE == f500103.DC_CODE
							&& o.GUP_CODE == f500103.GUP_CODE && o.CUST_CODE == f500103.CUST_CODE);
			if (f500103Item != null)
			{
				f500103Item.STATUS = statusType;
				repF500103.Update(f500103Item);
			}
			return new ExecuteResult(true, f500103Item.QUOTE_NO);
		}

		public ExecuteResult UpdateF500103(F500103 f500103)
		{
			var repF500103 = new F500103Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500103Item = repF500103.Find(o => o.QUOTE_NO == f500103.QUOTE_NO && o.DC_CODE == f500103.DC_CODE
							&& o.GUP_CODE == f500103.GUP_CODE && o.CUST_CODE == f500103.CUST_CODE);
			if (f500103Item != null)
			{
				f500103Item.ENABLE_DATE = f500103.ENABLE_DATE;
				f500103Item.DISABLE_DATE = f500103.DISABLE_DATE;
				f500103Item.NET_RATE = f500103.NET_RATE;

				if (f500103Item.ACC_KIND == "A")
				{
					f500103Item.APPROV_FEE = f500103.APPROV_FEE;
				}
				else
				{
					f500103Item.APPROV_BASIC_FEE = f500103.APPROV_BASIC_FEE;
					f500103Item.APPROV_OVER_FEE = f500103.APPROV_OVER_FEE;
				}

				f500103Item.MEMO = f500103.MEMO;
				repF500103.Update(f500103Item);
			}
			return new ExecuteResult(true, f500103Item.QUOTE_NO);
		}

		public ExecuteResult DeleteF500103(string dcCode, string gupCode, string custCode, string quoteNo)
		{		
			var repF500103 = new F500103Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500103Item = repF500103.Find(o => o.QUOTE_NO == quoteNo && o.DC_CODE == dcCode
							&& o.GUP_CODE == gupCode && o.CUST_CODE == custCode);
			if (f500103Item != null)
			{
				f500103Item.STATUS = "9";
				repF500103.Update(f500103Item);
			}
			return new ExecuteResult(true, f500103Item.QUOTE_NO);
		}

		public IQueryable<F500103QueryData> GetF500103QueryData(string dcCode, string enableSDate, string enableEDate, string quoteNo, string status)
		{
			//有效日期
			var coverCrtSDate = (string.IsNullOrEmpty(enableSDate)) ? ((DateTime?)null) : Convert.ToDateTime(enableSDate);
			var coverCrtEDate = (string.IsNullOrEmpty(enableEDate)) ? ((DateTime?)null) : Convert.ToDateTime(enableEDate);


			var repF500103 = new F500103Repository(Schemas.CoreSchema);
			return repF500103.GetF500103QueryData(dcCode, coverCrtSDate, coverCrtEDate, quoteNo, status);
		}
	}
}
