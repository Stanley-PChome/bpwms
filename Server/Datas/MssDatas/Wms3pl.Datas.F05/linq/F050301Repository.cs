using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
    public partial class F050301Repository : RepositoryBase<F050301, Wms3plDbContext, F050301Repository>
    {
        public F050301Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        ///  大量新增F050301
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="withoutColumns"></param>
        public override void BulkInsert(IEnumerable<F050301> entities, params string[] withoutColumns)
        {
            var dict = new Dictionary<string, object>
            {
                { "HELLO_LETTER", "0" },
                { "HELLO_LETTER_PRINTED", "0" }
            };

            base.BulkInsert(entities, dict, withoutColumns);
        }

        public IQueryable<F050301> GetNonCancelDatasByStatus(string procFlag)
        {
            var result = _db.F050301s.Where(x => x.PROC_FLAG == procFlag &&
                                                 x.PROC_FLAG != "9");

            return result;
        }


        public IQueryable<F050301ProgressData> GetProgressData(string dcCode, string gupCode, string custCode, string pickTime, DateTime? delvDate, string pickOrdNo)
        {
            var f050801Data = _db.F050801s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     x.PICK_TIME == pickTime &&
                                                                     x.DELV_DATE == delvDate &&
                                                                     x.PICK_ORD_NO == pickOrdNo);
            var wmsOrdNos = f050801Data.Select(x => x.WMS_ORD_NO);

            var f055001Data = _db.F055001s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                         x.GUP_CODE == gupCode &&
                                                         x.CUST_CODE == custCode &&
                                                         wmsOrdNos.Contains(x.WMS_ORD_NO));

            var f05030101Data = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                         x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode &&
                                                                         wmsOrdNos.Contains(x.WMS_ORD_NO));

            var f050301Data = _db.F050301s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                         x.GUP_CODE == gupCode &&
                                                         x.CUST_CODE == custCode &&
                                                         f05030101Data.Select(z => z.ORD_NO).Contains(x.ORD_NO));

            var data = from A in f050301Data
                       join B in f05030101Data
                       on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ORD_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.ORD_NO } into subB
                       from B in subB.DefaultIfEmpty()
                       join C in f050801Data
                       on new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.WMS_ORD_NO } equals new { C.DC_CODE, C.GUP_CODE, C.CUST_CODE, C.WMS_ORD_NO } into subC
                       from C in subC.DefaultIfEmpty()
                       join D in f055001Data
                       on new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.WMS_ORD_NO } equals new { D.DC_CODE, D.GUP_CODE, D.CUST_CODE, D.WMS_ORD_NO } into subD
                       from D in subD.DefaultIfEmpty()
                       select new F050301ProgressData
                       {
                           DC_CODE = A.DC_CODE,
                           GUP_CODE = A.GUP_CODE,
                           CUST_CODE = A.CUST_CODE,
                           CUST_ORD_NO = A.CUST_ORD_NO,
                           ORD_NO = A.ORD_NO,
                           WMS_ORD_NO = C.WMS_ORD_NO ?? null,
                           PICK_ORD_NO = C.PICK_ORD_NO ?? null,
                           APPROVE_DATE = C.APPROVE_DATE ?? null,
                           INCAR_DATE = C.INCAR_DATE ?? null,
                           PAST_NO = D.PAST_NO ?? null
                       };

            // RowNum
            var result = data.OrderBy(x => x.ORD_NO).ToList();
            for (int i = 0; i < result.Count; i++) { result[i].ROWNUM = i + 1; }

            return result.AsQueryable();
        }


        /// <summary>
        ///  檢查出貨單(wmsOrdNo) 對應 050301 相關出貨單是否結案 
        /// </summary>
        /// <returns></returns>
        public IQueryable<F050301WmsOrdNoData> GetWmsOrdNoWithF050301Data(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var ordNos = _db.F05030101s.AsNoTracking().Where(x => x.WMS_ORD_NO == wmsOrdNo).Select(x => x.ORD_NO);


            var f050301Data = _db.F050301s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     ordNos.Contains(x.ORD_NO));

            var f05030101Data = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                         x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode &&
                                                                         ordNos.Contains(x.ORD_NO));

            var f050801Data = _db.F050801s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     f05030101Data.Select(z => z.WMS_ORD_NO).Contains(x.WMS_ORD_NO));

            var result = (from A in f050301Data
                          join B in f05030101Data
                          on new { A.ORD_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE } equals new { B.ORD_NO, B.DC_CODE, B.GUP_CODE, B.CUST_CODE }
                          join C in f050801Data
                          on new { B.WMS_ORD_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE } equals new { C.WMS_ORD_NO, C.DC_CODE, C.GUP_CODE, C.CUST_CODE }
                          select new F050301WmsOrdNoData
                          {
                              ORD_NO = A.ORD_NO,
                              WMS_ORD_NO = B.WMS_ORD_NO,
                              STATUS = Convert.ToInt32(C.STATUS),
                              SOURCE_NO = A.SOURCE_NO,
                              SOURCE_TYPE = A.SOURCE_TYPE
                          }).OrderBy(x => x.ORD_NO);

            return result;
        }

        public IQueryable<F050301Data> GetF050301WmsNoData(string dcCode, string gupCode, string custCode, string ordNo)
        {
            var f05030101Data = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                         x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode &&
                                                                         x.ORD_NO == ordNo);

            var wmsOrdNos = f05030101Data.Select(x => x.WMS_ORD_NO);


            var f050801Data = _db.F050801s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     wmsOrdNos.Contains(x.WMS_ORD_NO));

            var f05030101Data2 = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                          x.GUP_CODE == gupCode &&
                                                                          x.CUST_CODE == custCode &&
                                                                          wmsOrdNos.Contains(x.WMS_ORD_NO));

            var f050301Data = _db.F050301s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     f05030101Data2.Select(z => z.ORD_NO).Contains(x.ORD_NO));

            var data = (from A in f05030101Data
                        join B in f050801Data
                        on A.WMS_ORD_NO equals B.WMS_ORD_NO
                        join C in f05030101Data2
                        on B.WMS_ORD_NO equals C.WMS_ORD_NO
                        join D in f050301Data
                        on C.ORD_NO equals D.ORD_NO
                        select new F050301Data
                        {
                            DC_CODE = D.DC_CODE,
                            GUP_CODE = D.GUP_CODE,
                            CUST_CODE = D.CUST_CODE,
                            CUST_ORD_NO = D.CUST_ORD_NO,
                            ORD_NO = D.ORD_NO,
                            ORD_TYPE = D.ORD_TYPE,
                            DELV_DATE = B.DELV_DATE,
                            PICK_TIME = B.PICK_TIME
                        }).Distinct();

            //RowNum
            var result = data.ToList();
            for (int i = 0; i < result.Count; i++) { result[i].ROWNUM = i + 1; }

            return result.AsQueryable();
        }

        public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItems(string dcCode, string gupCode, string custCode,
            DateTime begOrdDate, DateTime endOrdDate)
        {
            var data = _db.F050301s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                              x.GUP_CODE == gupCode &&
                                                              x.CUST_CODE == custCode &&
                                                              x.ORD_DATE >= begOrdDate &&
                                                              x.ORD_DATE <= endOrdDate &&
                                                              x.PROC_FLAG != "9")
                                                   .GroupBy(x => x.ORD_DATE)
                                                   .Select(x => new DcWmsNoDateItem
                                                   {
                                                       WmsDate = x.Key,
                                                       WmsCount = x.Count()
                                                   });

            // RowNum
            var result = data.OrderBy(x => x.WmsDate).ToList();
            for (int i = 0; i < result.Count; i++) { result[i].ROWNUM = i + 1; }

            return result.AsQueryable();
        }

        public IQueryable<F050301> GetF050301DataByOrdNos(string gupCode, string custCode, List<string> ordNos)
        {
            var result = _db.F050301s.Where(x => x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 ordNos.Contains(x.ORD_NO));

            return result;
        }

       

        public IQueryable<F050301> GetF050301DataByF05030101(string dcCode, string gupCode, string custCode, List<string> ordNos)
        {
            var result = _db.F050301s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && ordNos.Contains(x.ORD_NO));

            return result;
        }

        public IQueryable<F050301> GetF050301ForCancelAllocStockOrder(string dcCode, string gupCode, string custCode, string ordNo)
        {
            var f05030101Data = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                         x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode &&
                                                                         x.ORD_NO == ordNo);

            var wmsOrdNos = f05030101Data.Select(x => x.WMS_ORD_NO);


            var f050801Data = _db.F050801s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     wmsOrdNos.Contains(x.WMS_ORD_NO));

            var f05030101Data2 = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                          x.GUP_CODE == gupCode &&
                                                                          x.CUST_CODE == custCode &&
                                                                          wmsOrdNos.Contains(x.WMS_ORD_NO));

            var f050301Data = _db.F050301s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     f05030101Data2.Select(z => z.ORD_NO).Contains(x.ORD_NO));

            var data = (from A in f05030101Data
                        join B in f050801Data
                        on A.WMS_ORD_NO equals B.WMS_ORD_NO
                        join C in f05030101Data2
                        on B.WMS_ORD_NO equals C.WMS_ORD_NO
                        join D in f050301Data
                        on C.ORD_NO equals D.ORD_NO
                        select D).Distinct();

            return data;
        }

		public IQueryable<F050301> GetF050301ForWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var f050801s = _db.F050801s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == wmsOrdNo);

			var result = from A in _db.F05030101s
							 // on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.WMS_ORD_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.WMS_ORD_NO }
						 join B in _db.F050301s
						 on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ORD_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.ORD_NO }
						 where A.DC_CODE == dcCode && A.GUP_CODE == gupCode && A.CUST_CODE == custCode && A.WMS_ORD_NO == wmsOrdNo
						 select B;
			return result;

		}
	}
}
