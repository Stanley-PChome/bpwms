using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710108Service
	{
		private WmsTransaction _wmsTransaction;
		public P710108Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}
		public IQueryable<F1903> SearchItems(List<string> itemCodes, string gupCode, string custCode)
		{
			var repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var results = repo.Filter(x => itemCodes.Contains(x.ITEM_CODE) &&
										   (x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode) || string.IsNullOrEmpty(gupCode) ||
                                           x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode) || string.IsNullOrEmpty(custCode)));
			return results;
		}

		public IQueryable<F1912DataLoc> GetPrintDataLoc(string locStart, string locEnd, string gupCode, string dcCode,
		  string custCode, string warehouseCode, string listItem, bool printEmpty)
		{
			var repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
			var results = repo.GetPrintDataLoc(locStart, locEnd, gupCode, dcCode, custCode, warehouseCode, listItem, printEmpty);
			return results;
		}
		#region 新增R71010803報表

		public IQueryable<F1912DataLocByLocType> GetPrintDataLocByNewReport(string locStart, string locEnd, string gupCode, string dcCode,
	  string custCode, string warehouseCode, string listItem, bool printEmpty)
		{
			var repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
			var results = repo.GetPrintDataLocByNewReport(locStart, locEnd, gupCode, dcCode, custCode, warehouseCode, listItem, printEmpty);
			return results;
		}
		#endregion
	}

}
