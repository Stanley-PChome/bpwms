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
		

		/// <summary>
		/// 取得揀貨單身檔資料
		/// </summary>
		/// <param name="dcNo"></param>
		/// <param name="gupCode"></param>
		/// <param name="custNo"></param>
		/// <param name="pickOrdNo"></param>
		/// <returns></returns>
		public IQueryable<F051202> GetDatas(string dcNo, string gupCode, string custNo, string pickOrdNo)
		{
			var result = _db.F051202s.Where(x => x.DC_CODE == dcNo
																			&& x.GUP_CODE == gupCode
																			&& x.CUST_CODE == custNo 
																			&& x.PICK_ORD_NO == pickOrdNo);
			return result;
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

		public IQueryable<F051202> GetDatasForOrdNos(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
		{
			return _db.F051202s.Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			pickOrdNos.Contains(x.PICK_ORD_NO));
		}

		public IQueryable<F051202> GetDatasByF0011BindDatas(string dcCode, string gupCode, string custCode, List<string> pickNos)
		{
			return _db.F051202s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && pickNos.Contains(x.PICK_ORD_NO));
		}

		public IQueryable<F051202> GetDatasByPickNos(string dcCode, string gupCode, string custCode, List<string> pickNos)
		{
			return _db.F051202s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && pickNos.Contains(x.PICK_ORD_NO));
		}


		public IQueryable<F051202> GetDatasByPickNosNotStatus(string dcCode, string gupCode, string custCode, string pickStatus, List<string> pickNos)
		{
			return GetDatasByPickNos(dcCode, gupCode, custCode, pickNos).Where(x => x.PICK_STATUS != pickStatus);
		}

		public IQueryable<F051202> GetDatasByPickNos(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			return _db.F051202s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PICK_ORD_NO == pickOrdNo);
		}

		public IQueryable<F051202> GetDatasByPickNosNotStatus(string dcCode, string gupCode, string custCode, string pickStatus, string pickOrdNo)
		{
			return GetDatasByPickNos(dcCode, gupCode, custCode, pickOrdNo).Where(x => x.PICK_STATUS != pickStatus);
		}

		public IQueryable<F051202> GetDatasByOrdNos(string dcCode, string gupCode, string custCode, List<string> ordNos)
		{
			return _db.F051202s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && ordNos.Contains(x.WMS_ORD_NO));
		}

		public IQueryable<F051202> GetDataForPickNos(string dcCode, string gupCode, string custCode, string pickOrdNo, List<int> seqs)
		{
			return _db.F051202s.Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			pickOrdNo == x.PICK_ORD_NO &&
			seqs.Contains(Convert.ToInt32(x.PICK_ORD_SEQ)));
		}

		public IQueryable<F051202> GetDatasByOrdNo(string dcCode, string gupCode, string custCode, string ordNo)
		{
			return _db.F051202s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == ordNo);
		}
		public IQueryable<string> GetManualWarehouseIdByOrdNo(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var locCodes = _db.F051202s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == ordNo).Select(x => x.PICK_LOC);
			var warehouseIds = _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode && locCodes.Contains(x.LOC_CODE)).Select(x => x.WAREHOUSE_ID);
			var result = _db.F1980s.AsNoTracking().Where(x => x.DC_CODE == dcCode && warehouseIds.Contains(x.WAREHOUSE_ID) && x.DEVICE_TYPE == "0").Select(x => x.WAREHOUSE_ID);
			return result;
		}

		public IQueryable<F051202> GetDatasByWmsOrdNosAndPickStatus1(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			return _db.F051202s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			wmsOrdNos.Contains(x.WMS_ORD_NO) &&
			x.PICK_STATUS == "1");
		}

        public IQueryable<F051202> GetDatasByF051201s(List<F051201> f051201s)
        {
            return _db.F051202s.Where(x =>
            f051201s.Any(z =>
            x.DC_CODE == z.DC_CODE &&
            x.GUP_CODE == z.GUP_CODE &&
            x.CUST_CODE == z.CUST_CODE &&
            x.PICK_ORD_NO == z.PICK_ORD_NO));
        }
    }
}
