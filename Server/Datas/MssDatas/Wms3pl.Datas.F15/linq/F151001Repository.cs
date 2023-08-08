using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F15
{
	public partial class F151001Repository : RepositoryBase<F151001, Wms3plDbContext, F151001Repository>
	{
		public F151001Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		#region 調撥單維護
		public IQueryable<F151001Data> GetF151001Datas(string dcCode, string gupCode, string custCode, string sourceNo)
		{
			var result = _db.F151001s.AsNoTracking().Where(x => x.DC_CODE == dcCode
																									&& x.GUP_CODE == gupCode
																									&& x.CUST_CODE == custCode
																									&& x.SOURCE_NO == sourceNo)
																									.Select(x => new F151001Data
																									{
																										ALLOCATION_DATE = x.ALLOCATION_DATE,
																										ALLOCATION_NO = x.ALLOCATION_NO,
																										CRT_ALLOCATION_DATE = x.CRT_ALLOCATION_DATE,
																										POSTING_DATE = x.POSTING_DATE,
																										STATUS = x.STATUS,
																										TAR_DC_CODE = x.TAR_DC_CODE,
																										TAR_WAREHOUSE_ID = x.TAR_WAREHOUSE_ID,
																										SRC_WAREHOUSE_ID = x.SRC_WAREHOUSE_ID,
																										SRC_DC_CODE = x.SRC_DC_CODE,
																										SOURCE_TYPE = x.SOURCE_TYPE,
																										SOURCE_NO = x.SOURCE_NO,
																										BOX_NO = x.BOX_NO,
																										MEMO = x.MEMO,
																										DC_CODE = x.DC_CODE,
																										GUP_CODE = x.GUP_CODE,
																										CUST_CODE = x.CUST_CODE,
																										CRT_STAFF = x.CRT_STAFF,
																										CRT_DATE = x.CRT_DATE,
																										UPD_STAFF = x.UPD_STAFF,
																										UPD_DATE = x.UPD_DATE,
																										CRT_NAME = x.CRT_NAME,
																										UPD_NAME = x.UPD_NAME
																									}).ToList();
			for (int i = 0; i < result.Count; i++) { result[i].ROWNUM = i + 1; }
			return result.AsQueryable();
		}
		#endregion

		public IQueryable<F151001> GetDatasBySourceNo(string dcCode, string gupCode, string custCode, string sourceNo)
		{
			var result = _db.F151001s.Where(x => x.DC_CODE == dcCode
																			&& x.GUP_CODE == gupCode
																			&& x.CUST_CODE == custCode
																			&& x.SOURCE_NO == sourceNo);

			return result;
		}

		public IQueryable<F151001> GetDatas(string dcCode, string gupCode, string custCode, List<string> allocationNo)
		{
			var result = _db.F151001s.Where(x => x.DC_CODE == dcCode
																		 && x.GUP_CODE == gupCode
																		 && x.CUST_CODE == custCode
																		 && allocationNo.Contains(x.ALLOCATION_NO));
			return result;
		}

		public IQueryable<F151001> GetDatasNoTracking(string dcCode, string gupCode, string custCode, List<string> allocationNo)
		{
			var result = _db.F151001s.AsNoTracking().Where(x => x.DC_CODE == dcCode
																		 && x.GUP_CODE == gupCode
																		 && x.CUST_CODE == custCode
																		 && allocationNo.Contains(x.ALLOCATION_NO));
			return result;
		}

		/// <summary>
		/// 調撥單據檢核
		/// </summary>
		/// <param name="dcNo">物流中心編號</param>
		/// <param name="custNo">貨主編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="allocNo">調撥單號</param>
		/// <returns></returns>
		public F151001 GetSingleData(string dcNo, string custNo, string gupCode, string allocNo)
		{

			var result = _db.F151001s.Where(x => x.DC_CODE == dcNo
																			&& x.CUST_CODE == custNo
																			&& x.GUP_CODE == gupCode
																			&& x.ALLOCATION_NO == allocNo);

			return result.FirstOrDefault();
		}

		public F151001 GetAllocDatas(string dcCode, string gupCode, string custCode, string allocNo, List<string> statusList)
		{
			return _db.F151001s.Where(x => x.DC_CODE == dcCode
																			&& x.CUST_CODE == custCode
																			&& x.GUP_CODE == gupCode
																			&& x.ALLOCATION_NO == allocNo
																			&& statusList.Contains(x.STATUS)).FirstOrDefault();
		}

		public IQueryable<F151001> GetDatasBySourceNos(string dcCode, string gupCode, string custCode, List<string> sourceNos)
		{
			return _db.F151001s.Where(x => x.DC_CODE == dcCode
																			&& x.CUST_CODE == custCode
																			&& x.GUP_CODE == gupCode
																			&& sourceNos.Contains(x.SOURCE_NO));
		}

    /// <summary>
    /// 取得異常的調撥單(status=8)並回傳調撥單錯誤訊息
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="allocNo"></param>
    /// <returns></returns>
    public List<String> GetUnnormalAllocDatas(string dcCode, string gupCode, string custCode, List<string> allocNos)
    {
      var result = _db.F151001s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                          && x.GUP_CODE == gupCode
                          && x.CUST_CODE == custCode
                          && allocNos.Contains(x.ALLOCATION_NO)
                          && x.STATUS == "8")
                      .Select(x=> $"調撥單號{x.ALLOCATION_NO}{x.MEMO}")
                      .ToList();
      return result;
    }

  }
}