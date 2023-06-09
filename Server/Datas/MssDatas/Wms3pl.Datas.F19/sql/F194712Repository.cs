using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System.Data.SqlClient;

namespace Wms3pl.Datas.F19
{
    public partial class F194712Repository : RepositoryBase<F194712, Wms3plDbContext, F194712Repository>
    {
        public IQueryable<F194712EX> GetSettings(AutoGenConsignParam param)
        {
            var parms = new List<SqlParameter>();
            var sqlparam = string.Empty;
            //物流中心
            if (!string.IsNullOrWhiteSpace(param.DcCode))
            {
                sqlparam += " AND A.DC_CODE = @p" + parms.Count;
                parms.Add(new SqlParameter("@p" + parms.Count, param.DcCode));
            }
            //業主
            if (!string.IsNullOrWhiteSpace(param.GupCode))
            {
                sqlparam += " AND A.GUP_CODE = @p" + parms.Count;
                parms.Add(new SqlParameter("@p" + parms.Count, param.GupCode));
            }
            //貨主
            if (!string.IsNullOrWhiteSpace(param.CustCode))
            {
                sqlparam += " AND A.CUST_CODE = @p" + parms.Count;
                parms.Add(new SqlParameter("@p" + parms.Count, param.CustCode));
            }
            //通路
            if (!string.IsNullOrWhiteSpace(param.Channel))
            {
                sqlparam += " AND A.CHANNEL = @p" + parms.Count;
                parms.Add(new SqlParameter("@p" + parms.Count, param.Channel));
            }
            //配送商
            if (!string.IsNullOrWhiteSpace(param.AllId))
            {
                sqlparam += " AND A.ALL_ID = @p" + parms.Count;
                parms.Add(new SqlParameter("@p" + parms.Count, param.AllId));
            }
            //託運單類型
            if (!string.IsNullOrWhiteSpace(param.ConsignType))
            {
                sqlparam += " AND A.CONSIGN_TYPE = @p" + parms.Count;
                parms.Add(new SqlParameter("@p" + parms.Count, param.ConsignType));
            }
            //客戶代號
            if (!string.IsNullOrWhiteSpace(param.CustomerId))
            {
                sqlparam += " AND A.CUSTOMER_ID = @p" + parms.Count;
                parms.Add(new SqlParameter("@p" + parms.Count, param.CustomerId));
            }

            if (!string.IsNullOrWhiteSpace(param.IsTest))
            {
                sqlparam += " AND A.ISTEST = @p" + parms.Count;
                parms.Add(new SqlParameter("@p" + parms.Count, param.IsTest));
            }
            else
            {
#if DEBUG
                sqlparam += " AND A.ISTEST = '1' ";
#else
					sqlparam += " AND A.ISTEST = '0' ";
#endif
            }
            if (!string.IsNullOrWhiteSpace(sqlparam))
                sqlparam = " WHERE 1=1 " + sqlparam;



            var sql = $@" SELECT A.DC_CODE,C.DC_NAME,A.GUP_CODE,D.GUP_NAME,A.CUST_CODE,G.CUST_NAME,A.CHANNEL,H.NAME CHANNEL_NAME,A.ALL_ID,E.ALL_COMP,A.CUSTOMER_ID,A.CONSIGN_TYPE,F.NAME CONSIGN_TYPE_NAME,A.SAVE_QTY,A.PATCH_QTY,A.ISTEST,COUNT(DISTINCT CASE WHEN B.ISUSED ='0' THEN  B.CONSIGN_NO ELSE NULL END) UNUSEDQTY
                     FROM F194712 A
                     LEFT JOIN F19471201 B
                       ON A.DC_CODE = B.DC_CODE
                      AND A.GUP_CODE = B.GUP_CODE
                      AND A.CUST_CODE = B.CUST_CODE
                      AND A.CHANNEL = B.CHANNEL
                      AND A.ALL_ID = B.ALL_ID
                      AND A.CUSTOMER_ID = B.CUSTOMER_ID
                      AND A.CONSIGN_TYPE = B.CONSIGN_TYPE
											AND A.ISTEST = B.ISTEST
                     LEFT JOIN F1901 C
                       ON C.DC_CODE=  A.DC_CODE
                     LEFT JOIN F1929 D
                       ON D.GUP_CODE = A.GUP_CODE
                     LEFT JOIN F1947 E
                       ON E.DC_CODE = A.DC_CODE
                      AND E.ALL_ID = A.ALL_ID             
                     LEFT JOIN VW_F000904_LANG F
                       ON F.TOPIC= 'F194712' AND F.SUBTOPIC='CONSIGN_TYPE' AND F.VALUE = A.CONSIGN_TYPE  AND F.LANG = '{Current.Lang}' 
                     LEFT JOIN F1909 G
                       ON G.GUP_CODE = A.GUP_CODE AND G.CUST_CODE = A.CUST_CODE 
                     LEFT JOIN VW_F000904_LANG H
                       ON H.TOPIC='F050101' AND H.SUBTOPIC='CHANNEL' AND H.VALUE = A.CHANNEL AND H.LANG = '{Current.Lang}'
                    {sqlparam}                 
                    GROUP BY A.DC_CODE,C.DC_NAME,A.GUP_CODE,D.GUP_NAME,A.CUST_CODE,G.CUST_NAME,A.CHANNEL,H.NAME,A.ALL_ID,E.ALL_COMP,A.CUSTOMER_ID,A.CONSIGN_TYPE,F.NAME,A.SAVE_QTY,A.PATCH_QTY,A.ISTEST
				";
            var result = SqlQuery<F194712EX>(sql, parms.ToArray()).AsQueryable();
            return result;
        }
    }
}
