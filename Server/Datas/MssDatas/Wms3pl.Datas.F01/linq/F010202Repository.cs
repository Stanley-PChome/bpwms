using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
    public partial class F010202Repository : RepositoryBase<F010202, Wms3plDbContext, F010202Repository>
    {
        public F010202Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F010202Data> GetF010202Datas(string dcCode, string gupCode, string custCode, string stockNo)
        {
            var f010202Data = _db.F010202s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     x.STOCK_NO == stockNo);

            var f1903Data = _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
                                                                 x.CUST_CODE == custCode &&
                                                                 f010202Data.Select(z => z.ITEM_CODE).Distinct().Contains(x.ITEM_CODE));

            var data = from A in f010202Data
                       join B in f1903Data
                       on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE }
                       select new F010202Data
                       {
                           ChangeFlag = "N",
                           DC_CODE = A.DC_CODE,
                           GUP_CODE = A.GUP_CODE,
                           CUST_CODE = A.CUST_CODE,
                           STOCK_NO = A.STOCK_NO,
                           STOCK_SEQ = A.STOCK_SEQ,
                           ITEM_CODE = A.ITEM_CODE,
                           ITEM_NAME = B.ITEM_NAME ?? null,
                           ITEM_SIZE = B.ITEM_SIZE ?? null,
                           ITEM_SPEC = B.ITEM_SPEC ?? null,
                           ITEM_COLOR = B.ITEM_COLOR ?? null,
                           STOCK_QTY = A.STOCK_QTY,
                           VALI_DATE = A.VALI_DATE,
                           MAKE_NO = A.MAKE_NO,
                           EAN_CODE1 = B.EAN_CODE1,
                           EAN_CODE2 = B.EAN_CODE2
                       };

            // RowNum
            var result = data.OrderBy(x => x.STOCK_SEQ).ToList();
            for (int i = 0; i < result.Count; i++) { result[i].ROWNUM = i + 1; }

            return result.AsQueryable();
        }

        public IQueryable<F010202> GetDatas(string dcCode, string gupCode, string custCode, string stockNo)
        {
            var result = _db.F010202s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 x.STOCK_NO == stockNo);

            return result;
        }

        public IQueryable<F010202> GetDatasByDc(string dcCode, DateTime stockDate)
        {
            var f010202Data = _db.F010202s.AsNoTracking().Where(x => x.DC_CODE == dcCode);

            var f010201Data = _db.F010201s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.STOCK_DATE == stockDate &&
                                                                     x.STATUS != "9");

            var result = from A in f010202Data
                         join B in f010201Data
                         on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.STOCK_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.STOCK_NO }
                         select A;

            return result;
        }


        #region 廠商報到Grid
        public IQueryable<F010202Data> GetF010202DatasMargeValidate(string dcCode, string gupCode, string custCode, string stockNo)
        {
			var f010201s = _db.F010201s.Where(x => x.DC_CODE == dcCode &&
						 x.GUP_CODE == gupCode &&
						 x.CUST_CODE == custCode &&
						 x.STOCK_NO == stockNo);

			var f010202s = _db.F010202s.Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.STOCK_NO == stockNo);

			var data = from A in f010201s
								 join B in f010202s
								 on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.STOCK_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.STOCK_NO }
								 join C in _db.F1903s
								 on new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE } equals new { C.GUP_CODE, C.CUST_CODE, C.ITEM_CODE }
								 join D in _db.F1908s
								 on new { A.GUP_CODE, A.CUST_CODE, A.VNR_CODE } equals new { D.GUP_CODE, D.CUST_CODE, D.VNR_CODE }
								 group new { A, B, C, D } by new
								 {
									 A.STOCK_NO,
									 A.CUST_ORD_NO,
									 B.STOCK_SEQ,
									 B.ITEM_CODE,
									 C.ITEM_NAME,
									 C.EAN_CODE1,
									 C.EAN_CODE2,
									 D.VNR_NAME

								 } into g
								 select new F010202Data
								 {
									 STOCK_NO = g.Key.STOCK_NO,
									 STOCK_SEQ = g.Key.STOCK_SEQ,
									 CUST_ORD_NO = g.Key.CUST_ORD_NO,
									 ITEM_CODE = g.Key.ITEM_CODE,
									 ITEM_NAME = g.Key.ITEM_NAME,
									 EAN_CODE1 = g.Key.EAN_CODE1,
									 EAN_CODE2 = g.Key.EAN_CODE2,
									 STOCK_QTY = g.Sum(x => x.B.STOCK_QTY),
									 VNR_NAME = g.Key.VNR_NAME
								 };

			var result = data.OrderBy(x => x.STOCK_SEQ).ToList();
			for (int i = 0; i < result.Count; i++) { result[i].ROWNUM = i + 1; }
			return result.AsQueryable();
			//var f010202Data = _db.F010202s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			//                                                         x.GUP_CODE == gupCode &&
			//                                                         x.CUST_CODE == custCode &&
			//                                                         x.STOCK_NO == stockNo)
			//                                                         .GroupBy(x=>new { x.DC_CODE,x.GUP_CODE,x.CUST_CODE,x.ITEM_CODE})
			//                                                         .Select(y=>new F010202 {
			//                                                             GUP_CODE = y.Key.GUP_CODE,
			//                                                             CUST_CODE = y.Key.CUST_CODE,
			//                                                             ITEM_CODE = y.Key.ITEM_CODE,
			//                                                             STOCK_QTY = y.Sum(z=>z.STOCK_QTY)
			//                                                         })
			//                                                         ;

			//var f1903Data = _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
			//                                                     x.CUST_CODE == custCode &&
			//                                                     f010202Data.Select(z => z.ITEM_CODE).Distinct().Contains(x.ITEM_CODE));

			//var data = from A in f010202Data
			//           join B in f1903Data
			//           on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE }
			//           select new F010202Data
			//           {
			//               ITEM_CODE = A.ITEM_CODE,
			//               ITEM_NAME = B.ITEM_NAME,
			//               EAN_CODE1 = B.EAN_CODE1,
			//               EAN_CODE2 = B.EAN_CODE2,
			//               STOCK_QTY = A.STOCK_QTY
			//           };



			//// RowNum
			//var result = data.OrderBy(x => x.STOCK_SEQ).ToList();
			//for (int i = 0; i < result.Count; i++) { result[i].ROWNUM = i + 1; }

			//return result.AsQueryable();
		}

		public IQueryable<F010201MainData> GetF010202DatasMargeValidateChange(string dcCode, string gupCode, string custCode, string stockNo)
		{
			var f010201s = _db.F010201s.Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.STOCK_NO == stockNo);

			var f010202s = _db.F010202s.Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.STOCK_NO == stockNo);
			var f000904s = _db.VW_F000904_LANGs.Where(x => x.TOPIC == "F010201" &&
			x.SUBTOPIC == "BOOKING_IN_PERIOD" && x.LANG == Current.Lang);

			var data = from A in f010201s
								 join B in f010202s
								 on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.STOCK_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.STOCK_NO }

								 group new { A, B } by new
								 {
									 A.STOCK_NO, //進倉單號
									 A.CUST_ORD_NO, //貨主單號                          
									 A.BOOKING_IN_PERIOD, //預定進貨時段
									 A.DELIVER_DATE, //預定進倉日
									 A.DC_CODE,
									 A.GUP_CODE,
									 A.CUST_CODE
								 } into g
								 select new F010201MainData
								 {
									 STOCK_NO = g.Key.STOCK_NO,
									 ITEM_COUNT = g.Count(),
									 CUST_ORD_NO = g.Key.CUST_ORD_NO,
									 BOOKING_IN_PERIOD = f000904s.Where(o => o.VALUE == g.Key.BOOKING_IN_PERIOD).Any() ? (f000904s.Where(o => o.VALUE == g.Key.BOOKING_IN_PERIOD).FirstOrDefault().NAME) : (string.Empty),
									 DELIVER_DATE = g.Key.DELIVER_DATE,
									 DC_CODE = g.Key.DC_CODE,
									 GUP_CODE = g.Key.GUP_CODE,
									 CUST_CODE = g.Key.CUST_CODE
								 };

			var result = data.OrderBy(x => x.STOCK_NO).Distinct().ToList();
			for (int i = 0; i < result.Count; i++) { result[i].ROWNUM = i + 1; }
			return result.AsQueryable();

		}
		#endregion

		public IQueryable<F010202> GetDatasByStockNos(string dcCode, string gupCode, string custCode, List<string> stockNos)
		{
			return _db.F010202s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			stockNos.Contains(x.STOCK_NO));
		}
	}
}
