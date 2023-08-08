using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.Datas.Shared.Entities;
using System;

namespace Wms3pl.Datas.F05
{
    public partial class F051202Repository : RepositoryBase<F051202, Wms3plDbContext, F051202Repository>
    {
        public F051202Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }
		


		public int GetNotFinishCnt(string dcNo, string gupCode, string custNo, string wmsNo)
		{
			var result = _db.F051202s.AsNoTracking().Where(x => x.DC_CODE == dcNo
																										 && x.GUP_CODE == gupCode
																										 && x.CUST_CODE == custNo
																										 && x.WMS_ORD_NO == wmsNo
																										 && !new List<string> { "1", "9" }.Contains(x.PICK_STATUS))
																							.Count();

			return result;
		}

		/// <summary>
		/// 取得完成最新的一筆資料
		/// </summary>
		/// <param name="dcNo"></param>
		/// <param name="gupCode"></param>
		/// <param name="custNo"></param>
		/// <param name="wmsNo"></param>
		/// <returns></returns>
		public F051202 GetTop1FinishData(string dcNo, string gupCode, string custNo, string wmsNo)
		{

			var result = _db.F051202s.Where(x => x.DC_CODE == dcNo
																			&& x.GUP_CODE == gupCode
																			&& x.CUST_CODE == custNo
																			&& x.WMS_ORD_NO == wmsNo
																			&& new List<string> { "1", "9" }.Contains(x.PICK_STATUS))
															 .OrderByDescending(x => x.UPD_DATE);
			return result.FirstOrDefault();
		}


		public IQueryable<F051202> GetDatasByWmsOrdNosAndPickStatus10(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			return _db.F051202s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			wmsOrdNos.Contains(x.WMS_ORD_NO) &&
			x.PICK_STATUS == "1");
		}

  }
}
