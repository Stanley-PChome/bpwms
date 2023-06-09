using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;
using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.Shared.Pda.Entitues;

namespace Wms3pl.Datas.F05
{
    public partial class F050801Repository : RepositoryBase<F050801, Wms3plDbContext, F050801Repository>
    {
        public F050801Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        ///  大量新增F050801
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="withoutColumns"></param>
        public override void BulkInsert(IEnumerable<F050801> entities, params string[] withoutColumns)
        {
            var dict = new Dictionary<string, object>
            {
                { "PRINT_DETAIL_FLAG", "0" }
            };

            base.BulkInsert(entities, dict, withoutColumns);
        }

        

        /// <summary>
        /// 取得出貨單
        /// </summary>
        /// <param name="wmsOrdNo"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="dcCode"></param>
        /// <returns></returns>
        public F050801 GetData(string wmsOrdNo, string gupCode, string custCode, string dcCode)
        {
            var result = _db.F050801s.Where(x => x.WMS_ORD_NO == wmsOrdNo &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 x.DC_CODE == dcCode);

            return result.SingleOrDefault();
        }

        /// <summary>
        /// 取出貨單配送商代碼
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsOrdNo"></param>
        /// <returns></returns>
        public string GetAllIdByWmsOrdNo(string wmsOrdNo, string gupCode, string custCode, string dcCode)
        {
            var f700102Data = _db.F700102s.AsNoTracking().Where(x => x.WMS_NO == wmsOrdNo &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     x.DC_CODE == dcCode);

            var f700101Data = _db.F700101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     f700102Data.Select(z => z.DISTR_CAR_NO).Contains(x.DISTR_CAR_NO));

            var result = (from A in f700102Data
                          join C in f700101Data
                          on new { A.DISTR_CAR_NO, A.DC_CODE } equals new { C.DISTR_CAR_NO, C.DC_CODE }
                          select new
                          {
                              C.ALL_ID
                          }).FirstOrDefault();

            return result != null ? result.ALL_ID : null;
        }

        public List<F055001NewPackageBox> GetAllIdByWmsOrdNos(string gupCode, string custCode, string dcCode, List<string> wmsOrdNos)
        {
            var f700102Data = _db.F700102s.AsNoTracking().Where(x => wmsOrdNos.Contains(x.WMS_NO) &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     x.DC_CODE == dcCode);

            var f700101Data = _db.F700101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     f700102Data.Select(z => z.DISTR_CAR_NO).Contains(x.DISTR_CAR_NO));

            var result = (from A in f700102Data
                          join C in f700101Data
                          on new { A.DISTR_CAR_NO, A.DC_CODE } equals new { C.DISTR_CAR_NO, C.DC_CODE }
                          select new F055001NewPackageBox
                          {
                              ALL_ID = C.ALL_ID,
                              WMS_ORD_NO = A.WMS_NO
                          }).ToList();

            return result;
        }

        /// <summary>
        /// 取得批次扣帳的出貨單
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="delvDate"></param>
        /// <param name="pickTime"></param>
        /// <param name="wmsOrdNos"></param>
        /// <returns></returns>
        public IQueryable<F050801> GetF050801ByDelvPickTime(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, IEnumerable<string> wmsOrdNos = null)
        {
            List<decimal> status = new List<decimal> { 5, 6 };

            var f050801Data = _db.F050801s.Where(x => !status.Contains(x.STATUS) &&
                                                      x.DC_CODE == dcCode &&
                                                      x.GUP_CODE == gupCode &&
                                                      x.CUST_CODE == custCode &&
                                                      x.DELV_DATE == delvDate &&
                                                      x.PICK_TIME == pickTime);

            var f05030101Data = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                         x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode);

            if (wmsOrdNos != null && wmsOrdNos.Any())
            {
                f050801Data = f050801Data.Where(x => wmsOrdNos.Contains(x.WMS_ORD_NO));
                f05030101Data = f05030101Data.Where(x => wmsOrdNos.Contains(x.WMS_ORD_NO));
            }

            var f050301Data = _db.F050301s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     x.PROC_FLAG != "9" &&
                                                                     f05030101Data.Select(z => z.ORD_NO).Contains(x.ORD_NO));

            var result = (from A in f050801Data
                          join B in f05030101Data
                          on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.WMS_ORD_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.WMS_ORD_NO }
                          join C in f050301Data
                          on new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.ORD_NO } equals new { C.DC_CODE, C.GUP_CODE, C.CUST_CODE, C.ORD_NO }
                          select A).Distinct();

            return result;
        }


        /// <summary>
        /// 存在尚未取消的出貨單商品
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public bool ExistsNonCancelByItemCode(string gupCode, string custCode, string itemCode)
        {
            var f050801Data = _db.F050801s.AsNoTracking().Where(x => x.STATUS != 9 &&
                                                                     x.DELV_DATE >= DateTime.Today.AddYears(-1) &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode);

            var f050802Data = _db.F050802s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     x.ITEM_CODE == itemCode);

            var result = from A in f050801Data
                         join B in f050802Data
                         on new { A.WMS_ORD_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE } equals new { B.WMS_ORD_NO, B.DC_CODE, B.GUP_CODE, B.CUST_CODE }
                         select A;

            return result.Any();
        }

        /// <summary>
        /// 取得該批次日期的包裝~出貨的批次時段
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="delvDate"></param>
        /// <returns></returns>
        public IQueryable<string> GetPickTimeList(string dcCode, string gupCode, string custCode, DateTime delvDate)
        {
            List<decimal> status = new List<decimal> { 1, 2, 6 };

            var result = _db.F050801s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                x.GUP_CODE == gupCode &&
                                                                x.CUST_CODE == custCode &&
                                                                x.DELV_DATE == delvDate &&
                                                                status.Contains(x.STATUS))
                                                    .GroupBy(x => x.PICK_TIME)
                                                    .Select(x => x.Key)
                                                    .OrderBy(x => x);

            return result;
        }

        public IQueryable<DcWmsNoOrdPropItem> GetDcWmsNoOrdPropItems(string dcCode, DateTime delvDate)
        {
            var result = _db.F050801s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                x.DELV_DATE == delvDate &&
                                                                x.STATUS != 9)
                                                    .GroupBy(x => new { x.CUST_CODE, x.ORD_PROP })
                                                    .Select(x => new DcWmsNoOrdPropItem
                                                    {
                                                        CUST_CODE = x.Key.CUST_CODE,
                                                        ORD_PROP = x.Key.ORD_PROP,
                                                        CUST_FINISHCOUNT = x.Sum(z => z.STATUS == 5 ? 1 : 0),
                                                        CUST_TOTALCOUNT = x.Count()
                                                    }).ToList();

            for (int i = 0; i < result.Count; i++) { result[i].ROWNUM = i + 1; }

            return result.AsQueryable();
        }

        /// <summary>
        /// 從派車單號尋找出貨單
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="distrCarNo"></param>
        /// <returns></returns>
        public IQueryable<F050801> GetF050801sByDistrCarNo(string dcCode, string distrCarNo)
        {
            var f700101Data = _db.F700101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.DISTR_CAR_NO == distrCarNo);

            var f700102Data = _db.F700102s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.DISTR_CAR_NO == distrCarNo);

            var f050801Data = _db.F050801s.Where(x => f700102Data.Any(z => z.DC_CODE == x.DC_CODE) &&
                                                      f700102Data.Any(z => z.GUP_CODE == x.GUP_CODE) &&
                                                      f700102Data.Any(z => z.CUST_CODE == x.CUST_CODE) &&
                                                      f700102Data.Any(z => z.WMS_NO == x.WMS_ORD_NO));

            var result = from A in f700101Data
                         join B in f700102Data
                         on new { A.DISTR_CAR_NO, A.DC_CODE } equals new { B.DISTR_CAR_NO, B.DC_CODE }
                         join C in f050801Data
                         on new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.WMS_NO } equals new { C.DC_CODE, C.GUP_CODE, C.CUST_CODE, WMS_NO = C.WMS_ORD_NO }
                         select C;

            return result;
        }

        /// <summary>
        /// 單純從F050301的訂單編號找F050801出貨單
        /// </summary>
        /// <returns></returns>
        public IQueryable<F050801> GetF050801sByF050301s(string dcCode, string gupCode, string custCode, params string[] ordNos)
        {
            var f050301Data = _db.F050301s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     ordNos.Contains(x.ORD_NO));

            var f05030101Data = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                         x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode &&
                                                                         ordNos.Contains(x.ORD_NO));

            var f050801Data = _db.F050801s.Where(x => x.DC_CODE == dcCode &&
                                                      x.GUP_CODE == gupCode &&
                                                      x.CUST_CODE == custCode &&
                                                      f05030101Data.Select(z => z.WMS_ORD_NO).Contains(x.WMS_ORD_NO));

            var result = (from A in f050801Data
                          join B in f05030101Data
                          on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.WMS_ORD_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.WMS_ORD_NO }
                          join C in f050301Data
                          on new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.ORD_NO } equals new { C.DC_CODE, C.GUP_CODE, C.CUST_CODE, C.ORD_NO }
                          select A).Distinct();

            return result;
        }

        /// <summary>
        /// 若其中一張出貨單存在狀態的範圍則回傳 true
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="beginStatus"></param>
        /// <param name="endStatus"></param>
        /// <param name="wmsOrdNos"></param>
        /// <returns></returns>
        public IQueryable<F050801> GetBetweenStatusF050801s(string dcCode, string gupCode, string custCode, int beginStatus, int endStatus, IEnumerable<string> wmsOrdNos)
        {
            var result = _db.F050801s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                x.GUP_CODE == gupCode &&
                                                                x.CUST_CODE == custCode &&
                                                                x.STATUS >= beginStatus &&
                                                                x.STATUS <= endStatus &&
                                                                (x.NO_DELV == null || x.NO_DELV == "0") &&
                                                                wmsOrdNos.Contains(x.WMS_ORD_NO));

            return result;
        }

        public IQueryable<F050801> GetDatas(string dcCode, string gupCode, string custCode, DateTime takeDate, string status)
        {
            var f050801Data = _db.F050801s.Where(x => x.DC_CODE == dcCode &&
                                                      x.GUP_CODE == gupCode &&
                                                      x.CUST_CODE == custCode &&
                                                      x.STATUS == Convert.ToDecimal(status));

            var f700102Data = _db.F700102s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     f050801Data.Select(z => z.WMS_ORD_NO).Contains(x.WMS_NO));

            var f700101Data = _db.F700101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.TAKE_DATE == takeDate &&
                                                                     f700102Data.Select(z => z.DISTR_CAR_NO).Contains(x.DISTR_CAR_NO));

            var result = from A in f050801Data
                         join B in f700102Data
                         on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.WMS_ORD_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, WMS_ORD_NO = B.WMS_NO }
                         join C in f700101Data
                         on new { B.DC_CODE, B.DISTR_CAR_NO } equals new { C.DC_CODE, C.DISTR_CAR_NO }
                         select A;

            return result;
        }

        

        public IQueryable<F050801> GetDatasNoTracking(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
        {
            var result = _db.F050801s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 wmsOrdNos.Contains(x.WMS_ORD_NO));

            return result;
        }

        public IQueryable<F050801> GetDatas(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
        {
            var result = _db.F050801s.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 wmsOrdNos.Contains(x.WMS_ORD_NO));

            return result;
        }

        public IQueryable<F050801> GetDatasByPickOrdNos(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
        {
            var result = _db.F050801s.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode);

            if (pickOrdNos.Any())
            {
                result = result.Where(x => pickOrdNos.Contains(x.PICK_ORD_NO));
            }

            return result;
        }

        public IQueryable<F050801> GetDatasForWmsOrdNos(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
        {
            return _db.F050801s.Where(x => x.DC_CODE == dcCode &&
                                           x.GUP_CODE == gupCode &&
                                           x.CUST_CODE == custCode &&
                                           wmsOrdNos.Contains(x.WMS_ORD_NO));
        }

		// 從廠退出貨單號找到出貨單
		public F050801 GetWmsOrdNoForRtnWmsNo(string dcCode, string gupCode, string custCode, string rtnWmsNo)
		{
			var f160204s = _db.F160204s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
			var  result = from A in _db.F050801s
						 join B in _db.F05030101s
						 on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.WMS_ORD_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.WMS_ORD_NO }
						 join C in _db.F050301s
						 on new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.ORD_NO } equals new { C.DC_CODE, C.GUP_CODE, C.CUST_CODE, C.ORD_NO }
						 join D in f160204s
						 on new { C.DC_CODE, C.GUP_CODE, C.CUST_CODE, RTN_WMS_NO = C.SOURCE_NO } equals new { D.DC_CODE, D.GUP_CODE, D.CUST_CODE, D.RTN_WMS_NO }
						 where D.RTN_WMS_NO == rtnWmsNo
						 select A;
			return result.FirstOrDefault();
		}

		/// <summary>
		/// 取得該O單的P單，該P單找出有哪些O單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="ordNo"></param>
		/// <returns></returns>
		public IQueryable<F050801> GetWmsOrdNosByOrdNosInsidePickNo(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var f050801 = _db.F050801s.AsNoTracking().Where(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.WMS_ORD_NO == ordNo).FirstOrDefault();
			return _db.F050801s.AsNoTracking().Where(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.PICK_ORD_NO == f050801.PICK_ORD_NO);
		}


		public IQueryable<WmsOrdStatus> GetWmsOrdStatuses(List<string> wmsOrdNos)
		{
			var q = from a in _db.F050801s
							where wmsOrdNos.Contains(a.WMS_ORD_NO)
							select new WmsOrdStatus
							{
								DcCode = a.DC_CODE,
								WmsOrdNo = a.WMS_ORD_NO,
								Status = a.STATUS
							};

			return q;
		}
	}
}
