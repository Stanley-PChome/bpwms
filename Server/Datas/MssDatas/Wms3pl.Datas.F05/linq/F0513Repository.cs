using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;
using Wms3pl.Datas.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Wms3pl.Datas.F05
{
    public partial class F0513Repository : RepositoryBase<F0513, Wms3plDbContext, F0513Repository>
    {
        public F0513Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F0513> GetData(string dcCode, string gupCode, string custCode, DateTime delvDate)
        {
            var result = _db.F0513s.Where(x => x.DC_CODE == dcCode &&
                                               x.GUP_CODE == gupCode &&
                                               x.CUST_CODE == custCode &&
                                               x.DELV_DATE == delvDate);

            return result;
        }

        /// <summary>
        /// 取得最接近的出車時間
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public IQueryable<F0513PickTime> GetNearestPickTime(string dcCode, string gupCode, string custCode)
        {
            DateTime now = DateTime.Now;

            var data = _db.F0513s.AsNoTracking().Where(x => x.DELV_DATE.ToString("yyyy-MM-dd").CompareTo(now.ToString("yyyy-MM-dd")) >= 0 &&
                                                            x.PICK_TIME.CompareTo(now.ToString("HH:mm")) > 0 &&
                                                            x.DC_CODE == dcCode &&
                                                            x.GUP_CODE == gupCode &&
                                                            x.CUST_CODE == custCode)
                                                .Select(x => new F0513PickTime
                                                {
                                                    DELV_DATE = x.DELV_DATE,
                                                    CUST_CODE = x.CUST_CODE,
                                                    ORD_TYPE = x.ORD_TYPE,
                                                    PICK_TIME = x.PICK_TIME,
                                                    RETAIL_QTY = x.RETAIL_QTY,
                                                    PICK_CNT = x.PICK_CNT,
                                                    PROC_FLAG = x.PROC_FLAG,
                                                    GUP_CODE = x.GUP_CODE,
                                                    DC_CODE = x.DC_CODE,
                                                    CRT_STAFF = x.CRT_STAFF,
                                                    CRT_DATE = x.CRT_DATE,
                                                    UPD_STAFF = x.UPD_STAFF,
                                                    UPD_DATE = x.UPD_DATE,
                                                    CRT_NAME = x.CRT_NAME,
                                                    UPD_NAME = x.UPD_NAME,
                                                    PIER_CODE = x.PIER_CODE,
                                                    CHECKOUT_TIME = x.CHECKOUT_TIME,
                                                    ALL_ID = x.ALL_ID,
                                                    CAR_NO_A = x.CAR_NO_A,
                                                    CAR_NO_B = x.CAR_NO_B,
                                                    CAR_NO_C = x.CAR_NO_C,
                                                    ISSEAL = x.ISSEAL
                                                })
                                                .OrderBy(x => x.DELV_DATE)
                                                .ThenBy(x => x.PICK_TIME)
                                                .ToList();

            // RowNum
            for (int i = 0; i < data.Count; i++) { data[i].ROW_NUM = i + 1; }

            var result = data.AsQueryable().Where(x => x.ROW_NUM == 1);

            return result;
        }

		public IQueryable<F0513> GetDatasByCalcatePercent(string dcCode, string gupCode, string custCode, List<CalcatePickPercent> calcatePickPercentList)
		{
			return _db.F0513s.Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			calcatePickPercentList.Any(z =>
			z.DELV_DATE == x.DELV_DATE &&
			z.PICK_TIME == x.PICK_TIME));
		}
    }
}




