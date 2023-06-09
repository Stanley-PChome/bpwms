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
	public partial class F500103Repository : RepositoryBase<F500103, Wms3plDbContext, F500103Repository>
	{
		public F500103Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F500103QueryData> GetQuoteDatas(string dcCode, string gupCode, string custCode, List<string> quotes)
		{
            return  from a in _db.F500103s
                        join b in _db.F91000302s on new { a.ACC_UNIT, ITEM_TYPE_ID = "004" } equals new { b.ACC_UNIT, ITEM_TYPE_ID = b.ITEM_TYPE_ID } into b1
                        from b2 in b1.DefaultIfEmpty()
                        where (a.DC_CODE == dcCode || dcCode == "000")
                            && a.GUP_CODE == gupCode
                            && a.CUST_CODE == custCode
                            && quotes.Contains(a.QUOTE_NO)
                        select new F500103QueryData
                        {
                            QUOTE_NO = a.QUOTE_NO,
                            ENABLE_DATE = a.ENABLE_DATE,
                            DISABLE_DATE = a.DISABLE_DATE,
                            NET_RATE = decimal.Parse(a.NET_RATE.ToString()),
                            ACC_ITEM_NAME = a.ACC_ITEM_NAME,
                            ACC_ITEM_KIND_ID = a.ACC_ITEM_KIND_ID,
                            ACC_KIND = a.ACC_KIND,
                            ACC_UNIT = a.ACC_UNIT,
                            ACC_NUM = a.ACC_NUM,
                            IN_TAX = a.IN_TAX,
                            FEE = a.FEE,
                            BASIC_FEE = a.BASIC_FEE,
                            OVER_FEE = a.OVER_FEE,
                            DELV_ACC_TYPE = a.DELV_ACC_TYPE,
                            APPROV_FEE = a.APPROV_FEE,
                            APPROV_BASIC_FEE = a.APPROV_BASIC_FEE,
                            APPROV_OVER_FEE = a.APPROV_OVER_FEE,
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

                            ACC_UNIT_NAME = b2.ACC_UNIT_NAME,

                            //ROWNUM = ""
                        };
			            //var sql = @"SELECT ROWNUM,A.*,B.ACC_UNIT_NAME FROM F500103 A 
			            //						  LEFT JOIN F91000302 B ON A.ACC_UNIT = B.ACC_UNIT AND B.ITEM_TYPE_ID = '004'
			            //						 WHERE (A.DC_CODE = :p0 OR :p1 = '000') AND A.GUP_CODE = :p2 AND A.CUST_CODE = :p3 " + inSql;
		}
	}
}
