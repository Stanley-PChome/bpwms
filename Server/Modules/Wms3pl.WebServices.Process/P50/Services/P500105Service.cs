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
	public partial class P500105Service
	{
		private WmsTransaction _wmsTransaction;
		public P500105Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult InsertF500102(F500102 f500102)
		{
			var sharedService = new SharedService();
			var quoteNo = sharedService.GetNewOrdCode("Q"); //報價單號

			var repF500102 = new F500102Repository(Schemas.CoreSchema);
			f500102.CUST_TYPE = f500102.DELV_ACC_TYPE == "04" ? "0" : "1";
			f500102.QUOTE_NO = quoteNo;
			f500102.CRT_DATE = DateTime.Now;
			repF500102.Add(f500102);

			return new ExecuteResult(true, quoteNo);
		}

		public ExecuteResult UpdateF500102Status(F500102 f500102, string statusType)
		{

			var repF500102 = new F500102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500102Item = repF500102.Find(o => o.QUOTE_NO == f500102.QUOTE_NO && o.DC_CODE == f500102.DC_CODE
							&& o.GUP_CODE == f500102.GUP_CODE && o.CUST_CODE == f500102.CUST_CODE);
			if (f500102Item != null)
			{
				f500102Item.STATUS = statusType;
				repF500102.Update(f500102Item);
			}
			return new ExecuteResult(true, f500102Item.QUOTE_NO);
		}

		public ExecuteResult UpdateF500102(F500102 f500102)
		{
			var repF500102 = new F500102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500102Item = repF500102.Find(o => o.QUOTE_NO == f500102.QUOTE_NO && o.DC_CODE == f500102.DC_CODE
							&& o.GUP_CODE == f500102.GUP_CODE && o.CUST_CODE == f500102.CUST_CODE);
			if (f500102Item != null)
			{
				f500102Item.ENABLE_DATE = f500102.ENABLE_DATE;
				f500102Item.DISABLE_DATE = f500102.DISABLE_DATE;
				f500102Item.NET_RATE = f500102.NET_RATE;
				f500102Item.APPROV_FEE = f500102.APPROV_FEE;

				if (f500102Item.ACC_KIND != "A")
				{
					f500102Item.APPROV_OVER_UNIT_FEE = f500102.APPROV_OVER_UNIT_FEE;
				}

				f500102Item.MEMO = f500102.MEMO;
				repF500102.Update(f500102Item);
			}
			return new ExecuteResult(true, f500102Item.QUOTE_NO);
		}

		public ExecuteResult DeleteF500102(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var repF500102 = new F500102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f500102Item = repF500102.Find(o => o.QUOTE_NO == quoteNo && o.DC_CODE == dcCode
							&& o.GUP_CODE == gupCode && o.CUST_CODE == custCode);
			if (f500102Item != null)
			{
				f500102Item.STATUS = "9";
				repF500102.Update(f500102Item);
			}
			return new ExecuteResult(true, f500102Item.QUOTE_NO);
		}

		public IQueryable<F500102QueryData> GetF500102QueryData(string dcCode, string enableSDate, string enableEDate, string quoteNo, string status)
		{
			//有效日期
			var coverCrtSDate = (string.IsNullOrEmpty(enableSDate)) ? ((DateTime?)null) : Convert.ToDateTime(enableSDate);
			var coverCrtEDate = (string.IsNullOrEmpty(enableEDate)) ? ((DateTime?)null) : Convert.ToDateTime(enableEDate);


			var repF500102 = new F500102Repository(Schemas.CoreSchema);
			return repF500102.GetF500102QueryData(dcCode, coverCrtSDate, coverCrtEDate, quoteNo, status);
		}
	}
}
