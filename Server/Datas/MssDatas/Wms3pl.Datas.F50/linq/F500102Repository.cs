using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.F50
{
	public partial class F500102Repository : RepositoryBase<F500102, Wms3plDbContext, F500102Repository>
	{
		public F500102Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
        public IQueryable<F500102QueryData> GetQuoteDatas(string dcCode, string gupCode, string custCode, List<string> quotes)
        {
            var q = from a in _db.F500102s
                    join b in _db.F91000302s on new { a.ACC_UNIT, ITEM_TYPE_ID = "005" } equals new { b.ACC_UNIT, b.ITEM_TYPE_ID } into b1
                    from b2 in b1.DefaultIfEmpty()
                    where (a.DC_CODE == dcCode || dcCode == "000")
                        && a.GUP_CODE == gupCode
                        && a.CUST_CODE == custCode
                        && quotes.Contains(a.QUOTE_NO)
                    select new F500102QueryData
                    {
                        QUOTE_NO = a.QUOTE_NO,
                        ENABLE_DATE = a.ENABLE_DATE,
                        DISABLE_DATE = a.DISABLE_DATE,
                        NET_RATE = decimal.Parse(a.NET_RATE.ToString()),
                        ACC_ITEM_KIND_ID = a.ACC_ITEM_KIND_ID,
                        ACC_ITEM_NAME = a.ACC_ITEM_NAME,
                        //CUST_TYPE = a.CUST_TYPE,
                        LOGI_TYPE = a.LOGI_TYPE,
                        ACC_KIND = a.ACC_KIND,
                        IS_SPECIAL_CAR = a.IS_SPECIAL_CAR,
                        CAR_KIND_ID = int.Parse(string.IsNullOrWhiteSpace(a.CAR_KIND_ID.ToString()) ? "0" : a.CAR_KIND_ID.ToString()),
                        ACC_AREA_ID = int.Parse(a.ACC_AREA_ID.ToString()),
                        DELV_TMPR = a.DELV_TMPR,
                        DELV_EFFIC = a.DELV_EFFIC,
                        ACC_UNIT = a.ACC_UNIT,
                        IN_TAX = a.IN_TAX,
                        ACC_NUM = a.ACC_NUM,
                        MAX_WEIGHT = a.MAX_WEIGHT,
                        FEE = a.FEE,
                        APPROV_FEE = a.APPROV_FEE,
                        OVER_VALUE = a.OVER_VALUE,
                        OVER_UNIT_FEE = a.OVER_UNIT_FEE,
                        APPROV_OVER_UNIT_FEE = a.APPROV_OVER_UNIT_FEE,
                        DELV_ACC_TYPE = a.DELV_ACC_TYPE,
                        ITEM_TYPE_ID = a.ITEM_TYPE_ID,
                        STATUS = a.STATUS,
                        MEMO = a.MEMO,
                        DC_CODE = a.DC_CODE,
                        GUP_CODE = a.GUP_CODE,
                        CUST_CODE = a.CUST_CODE,
                        //CRT_STAFF = a.CRT_STAFF,
                        CRT_NAME = a.CRT_NAME,
                        CRT_DATE = a.CRT_DATE,
                        //UPD_STAFF = a.UPD_STAFF,
                        UPD_NAME = a.UPD_NAME,
                        UPD_DATE = a.UPD_DATE,
                        //ACC_UNIT_NAME = b.ACC_UNIT_NAME,
                        ACC_UNIT_NAME = b2.ACC_UNIT_NAME,
                    };

            //以上註解為缺少欄位
            //var result = q.ToList();
            //for (int i = 0; i < result.Count(); i++) result[i].ROWNUM
            //沒有序號的欄位

            return q;
        }
    }
}
