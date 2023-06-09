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
	public partial class F500105Repository : RepositoryBase<F500105, Wms3plDbContext, F500105Repository>
	{
		public F500105Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F500105QueryData> GetF500105QueryData(string dcCode, DateTime? enableSDate, DateTime? enableEDate
			, string quoteNo, string status)
		{
            var q = from a in _db.F500105s
                    join b in _db.VW_F000904_LANGs on new { TOPIC = "P500102", SUBTOPIC = "STATUS", STATUS = a.STATUS, LANG = Current.Lang }
                                                   equals new { TOPIC = b.TOPIC, SUBTOPIC = b.SUBTOPIC, STATUS = b.VALUE, LANG = b.LANG }
                    join c in _db.F1901s on a.DC_CODE equals c.DC_CODE into c1
                    from c2 in c1.DefaultIfEmpty()
                    join d in _db.F910404s on new { a.QUOTE_NO, a.DC_CODE, a.GUP_CODE, a.CUST_CODE }
                                           equals new { d.QUOTE_NO, d.DC_CODE, d.GUP_CODE, d.CUST_CODE } into d1
                    from d2 in d1.DefaultIfEmpty()
                    where a.DC_CODE == dcCode
                    select new F500105QueryData
                    {
                        DC_CODE = a.DC_CODE,
                        GUP_CODE = a.GUP_CODE,
                        CUST_CODE = a.CUST_CODE,
                        QUOTE_NO = a.QUOTE_NO,
                        ENABLE_DATE = a.ENABLE_DATE,
                        DISABLE_DATE = a.DISABLE_DATE,
                        ACC_ITEM_NAME = a.ACC_ITEM_NAME,
                        ACC_NUM = a.ACC_NUM,
                        ACC_UNIT = a.ACC_UNIT,
                        IN_TAX = a.IN_TAX,
                        FEE = a.FEE,
                        DELV_ACC_TYPE = a.DELV_ACC_TYPE,
                        APPROV_FEE = a.APPROV_FEE,
                        ITEM_TYPE_ID = a.ITEM_TYPE_ID,
                        MEMO = a.MEMO,
                        STATUS = a.STATUS,
                        CRT_DATE = a.CRT_DATE,
                        CRT_NAME = a.CRT_NAME,
                        UPD_DATE = a.UPD_DATE,
                        UPD_NAME = a.UPD_NAME,
                        STATUS_NAME = b.NAME,
                        DC_NAME=(string.IsNullOrEmpty(c2.DC_NAME)? "不指定": c2.DC_NAME),
                        UPLOAD_FILE = d2.UPLOAD_S_PATH,
                        NET_RATE = Convert.ToDecimal(a.NET_RATE)
                    };

            //單據編號
            if (!string.IsNullOrEmpty(quoteNo))
                q = q.Where(x => x.QUOTE_NO == quoteNo);

            //Status
            if (!string.IsNullOrEmpty(status))
                q = q.Where(x => x.STATUS == status);
            else
                q = q.Where(x => x.STATUS != "9");

            //有效日期-起
            if (enableSDate.HasValue)
                q = q.Where(x => x.ENABLE_DATE >= enableSDate.Value);

            //有效日期-迄
            if (enableEDate.HasValue)
                q = q.Where(x => x.ENABLE_DATE <= enableEDate.Value.AddDays(1));

            return q.OrderBy(x => x.QUOTE_NO);
        }

        public IQueryable<F500105QueryData> GetQuoteDatas(string dcCode, string gupCode, string custCode, List<string> quotes)
        {
            var q = from a in _db.F500105s
                    join b in _db.F91000302s on new { a.ACC_UNIT, ITEM_TYPE_ID = "006" } equals new { b.ACC_UNIT, b.ITEM_TYPE_ID }
                    where (a.DC_CODE == dcCode || dcCode == "000")
                        && a.GUP_CODE == gupCode
                        && a.CUST_CODE == custCode
                        && quotes.Contains(a.QUOTE_NO)
                    select new F500105QueryData
                    {
                        QUOTE_NO = a.QUOTE_NO,
                        ENABLE_DATE = a.ENABLE_DATE,
                        DISABLE_DATE = a.DISABLE_DATE,
                        NET_RATE = decimal.Parse(a.NET_RATE.ToString()),
                        ACC_ITEM_NAME = a.ACC_ITEM_NAME,
                        ACC_UNIT = a.ACC_UNIT,
                        ACC_NUM = a.ACC_NUM,
                        IN_TAX = a.IN_TAX,
                        DELV_ACC_TYPE = a.DELV_ACC_TYPE,
                        FEE = a.FEE,
                        APPROV_FEE = a.APPROV_FEE,
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

                        ACC_UNIT_NAME = b.ACC_UNIT_NAME
                    };
            return q;
        }
    }
}
