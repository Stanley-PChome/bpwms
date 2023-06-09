
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
	public partial class P500103Service
	{
		private WmsTransaction _wmsTransaction;
		public P500103Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult InsertF500104(F500104 f500104)
		{
			var sharedService = new SharedService();
			var quoteNo = sharedService.GetNewOrdCode("Q"); //領用單號

			var repF500104 = new F500104Repository(Schemas.CoreSchema, _wmsTransaction);
			f500104.QUOTE_NO = quoteNo;
			f500104.CRT_DATE = DateTime.Now;
			repF500104.Add(f500104);

			return new ExecuteResult(true, quoteNo);
		}

		public ExecuteResult UpdateF500104Status(F500104 f500104, string statusType)
		{			
			var repF500104 = new F500104Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500104Item = repF500104.Find(o => o.QUOTE_NO == f500104.QUOTE_NO && o.DC_CODE == f500104.DC_CODE
							&& o.GUP_CODE == f500104.GUP_CODE && o.CUST_CODE == f500104.CUST_CODE);
			if (f500104Item != null)
			{
				f500104Item.STATUS = statusType;
				repF500104.Update(f500104Item);
			}
			return new ExecuteResult(true, f500104Item.QUOTE_NO);
		}

		public ExecuteResult UpdateF500104(F500104 f500104)
		{
			var repF500104 = new F500104Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500104Item = repF500104.Find(o => o.QUOTE_NO == f500104.QUOTE_NO && o.DC_CODE == f500104.DC_CODE
							&& o.GUP_CODE == f500104.GUP_CODE && o.CUST_CODE == f500104.CUST_CODE);
			if (f500104Item != null)
			{
				f500104Item.ENABLE_DATE = f500104.ENABLE_DATE;
				f500104Item.DISABLE_DATE = f500104.DISABLE_DATE;
				f500104Item.NET_RATE = f500104.NET_RATE;

				if (f500104Item.ACC_KIND == "A")
				{
					f500104Item.APPROV_FEE = f500104.APPROV_FEE;
				}
				else
				{
					f500104Item.APPROV_BASIC_FEE = f500104.APPROV_BASIC_FEE;
					f500104Item.APPROV_OVER_FEE = f500104.APPROV_OVER_FEE;
				}

				f500104Item.MEMO = f500104.MEMO;
				repF500104.Update(f500104Item);
			}
			return new ExecuteResult(true, f500104Item.QUOTE_NO);
		}

		public ExecuteResult DeleteF500104(string dcCode, string gupCode, string custCode, string quoteNo)
		{		
			var repF500104 = new F500104Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500104Item = repF500104.Find(o => o.QUOTE_NO == quoteNo && o.DC_CODE == dcCode
							&& o.GUP_CODE == gupCode && o.CUST_CODE == custCode);
			if (f500104Item != null)
			{
				f500104Item.STATUS = "9";
				repF500104.Update(f500104Item);
			}
			return new ExecuteResult(true, f500104Item.QUOTE_NO);
		}

		public IQueryable<F500104QueryData> GetF500104QueryData(string dcCode, string enableSDate, string enableEDate, string quoteNo, string status)
		{
			//有效日期
			var coverCrtSDate = (string.IsNullOrEmpty(enableSDate)) ? ((DateTime?)null) : Convert.ToDateTime(enableSDate);
			var coverCrtEDate = (string.IsNullOrEmpty(enableEDate)) ? ((DateTime?)null) : Convert.ToDateTime(enableEDate);


			var repF500104 = new F500104Repository(Schemas.CoreSchema);
			return repF500104.GetF500104QueryData(dcCode, coverCrtSDate, coverCrtEDate, quoteNo, status);
		}

	}
}
