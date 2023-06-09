using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F00
{
    public partial class F0011Repository : RepositoryBase<F0011, Wms3plDbContext, F0011Repository>
    {
        public F0011Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }
        /// <summary>
        /// 取得當日人員單據資料
        /// </summary>
        /// <param name="dcCode">物流中心</param>
        /// <param name="gupCode">業主</param>
        /// <param name="custCode">貨主</param>
        /// <param name="empID">工號</param>
        /// <param name="status">單據狀態</param>
        /// <returns></returns>
        public IEnumerable<F0011BindData> GetF0011List(string dcCode, string gupCode, string custCode, string empID, string status)
        {
					var datas = from a in _db.F0011s
											join b in _db.F1924s
											on a.EMP_ID equals b.EMP_ID into g
											from b in g.DefaultIfEmpty()
											join c in _db.F051201s
											on new {a.DC_CODE,a.GUP_CODE,a.CUST_CODE,a.ORDER_NO} equals new { c.DC_CODE,c.GUP_CODE,c.CUST_CODE, ORDER_NO= c.PICK_ORD_NO}
											where a.DC_CODE == dcCode && a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.EMP_ID  == empID && a.STATUS == status
											&& c.DISP_SYSTEM == "0" && c.PICK_TOOL == "1" 
											select new { A = a, b.EMP_NAME };

            var result = datas.Select((x) => new F0011BindData()
            {
                CLOSE_DATE = x.A.CLOSE_DATE,
                CUST_CODE = x.A.CUST_CODE,
                GUP_CODE = x.A.GUP_CODE,
                DC_CODE = x.A.DC_CODE,
                CRT_DATE = x.A.CRT_DATE,
                UPD_DATE = x.A.UPD_DATE,
                CRT_NAME = x.A.CRT_NAME,
                UPD_NAME = x.A.UPD_NAME,
                EMP_ID = x.A.EMP_ID,
								EMP_NAME = x.EMP_NAME,
                ID = (int)x.A.ID,
                ORDER_NO = x.A.ORDER_NO,
                // ROWNUM = ,
                START_DATE = x.A.START_DATE,
                STATUS = x.A.STATUS,
                //PICK_STATUS = (
                //        (!x.B.PICK_STATUS.HasValue) ? "":
                //        (x.B.PICK_STATUS.Value.ToString() == "0") ? ("待揀貨") :
                //        (x.B.PICK_STATUS.Value.ToString() == "1") ? ("揀貨中") :
                //        (x.B.PICK_STATUS.Value.ToString() == "2") ? ("揀貨完成") :
                //        (x.B.PICK_STATUS.Value.ToString() == "9") ? ("取消") : 
                //        "")
            }).ToList();
            for (int i = 0; i < result.Count(); i++)
                result[i].ROWNUM = i + 1;
            return result.AsEnumerable();
        }

        /// <summary>
        /// 查詢單據綁定資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="empID"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public IEnumerable<F0011BindData> GetF0011ListSearchData(string dcCode, string gupCode, string custCode, string empID, string orderNo, DateTime crtDate)
        {
            var dateS = new DateTime(crtDate.Year, crtDate.Month, crtDate.Day, 0, 0, 0);
            var dateE = dateS.AddDays(1).AddSeconds(-1);

            var query = _db.F0011s
                .Where(x => x.DC_CODE == dcCode
                    && x.GUP_CODE == gupCode
                    && x.CUST_CODE == custCode
                    && dateS <= x.START_DATE
                    && dateE >= x.START_DATE);
            if (!string.IsNullOrEmpty(empID))
                query = query.Where(x => x.EMP_ID == empID);
            if (!string.IsNullOrEmpty(orderNo))
                query = query.Where(x => x.ORDER_NO == orderNo);

			       var result =( from x in query
									join y in _db.F1924s
									on x.EMP_ID equals y.EMP_ID into g
									from y in g.DefaultIfEmpty()
									join o in _db.F051201s
									on new { x.DC_CODE,x.GUP_CODE,x.CUST_CODE,x.ORDER_NO} equals new { o.DC_CODE,o.GUP_CODE,o.CUST_CODE,ORDER_NO = o.PICK_ORD_NO }
									where o.PICK_TOOL == "1"
									select new F0011BindData
									{
										ID = (int)x.ID,
										DC_CODE = x.DC_CODE,
										CUST_CODE = x.CUST_CODE,
										GUP_CODE = x.GUP_CODE,
										EMP_ID = x.EMP_ID,
										EMP_NAME = y.EMP_NAME,
										ORDER_NO = x.ORDER_NO,
										STATUS = x.STATUS,
										START_DATE = x.START_DATE,
										CLOSE_DATE = x.CLOSE_DATE,
										CRT_DATE = x.CRT_DATE,
										UPD_DATE = x.UPD_DATE,
										CRT_NAME = x.CRT_NAME,
										UPD_NAME = x.UPD_NAME,
									}).ToList();



            for (int i = 0; i < result.Count(); i++)
            {
                result[i].ROWNUM = i + 1;
            }
            return result.AsEnumerable();
        }

        public IQueryable<F0011> GetDatas(string dcCode, string gupCode, string custCode, List<string> orderNos)
        {
            return _db.F0011s.Where(x => x.DC_CODE == dcCode &&
                                         x.GUP_CODE == gupCode &&
                                         x.CUST_CODE == custCode &&
                                         orderNos.Contains(x.ORDER_NO));
        }

        public IQueryable<F0011> GetDatas(string dcCode, string gupCode, string custCode, string orderNo)
        {
            return _db.F0011s.Where(x => x.DC_CODE == dcCode &&
                                         x.GUP_CODE == gupCode &&
                                         x.CUST_CODE == custCode &&
                                         x.ORDER_NO == orderNo);
        }

		public F0011 GetDatasForNotClosed(string dcCode, string gupCode, string custCode, string orderNo)
		{
			return _db.F0011s.Where(x => x.DC_CODE == dcCode &&
																	 x.GUP_CODE == gupCode &&
																	 x.CUST_CODE == custCode &&
																	 x.ORDER_NO == orderNo &&
																	 x.CLOSE_DATE == null).FirstOrDefault();
		}
	}
}



