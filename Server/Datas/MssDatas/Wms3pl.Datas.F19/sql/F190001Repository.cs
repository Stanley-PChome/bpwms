using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F190001Repository : RepositoryBase<F190001, Wms3plDbContext, F190001Repository>
    {
        /// <summary>
        /// 取得貨主單據維護的查詢結果
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public IQueryable<F190001Data> GetF190001Data(string dcCode, string gupCode, string custCode, string ticketType)
        {
            var sqlParams = new SqlParameter[]{
                new SqlParameter("@p0",dcCode),
                new SqlParameter("@p1",gupCode ?? string.Empty ),
                new SqlParameter("@p2",custCode ?? string.Empty ),
                new SqlParameter("@p3",ticketType ),
                new SqlParameter("@p4",Current.Staff)
            };

            var sql = @"
                        SELECT A.*,
                               B.DC_NAME,
                               C.GUP_NAME,
                               D.CUST_NAME,
                               E.ORD_NAME,
                               F.TICKET_CLASS_NAME,
                               ( CASE A.SHIPPING_ASSIGN
                                   WHEN '1' THEN N'是'
                                   ELSE N'否'
                                 END ) AS SHIPPING_ASSIGN_NAME,
                               ( CASE A.FAST_DELIVER
                                   WHEN '1' THEN N'是'
                                   ELSE N'否'
                                 END ) AS FAST_DELIVER_NAME,
                               G.ALL_COMP,
                               H.MILESTONE_ID,
                               H.MILESTONE_NO,
                               H.SORT_NO,
                               I.MILESTONE_NAME
                        FROM   F190001 A
                               LEFT JOIN F1901 B
                                      ON A.DC_CODE = B.DC_CODE
                               LEFT JOIN F1929 C
                                      ON A.GUP_CODE = C.GUP_CODE
                               LEFT JOIN F1909 D
                                      ON A.GUP_CODE = D.GUP_CODE
                                         AND A.CUST_CODE = D.CUST_CODE
                               LEFT JOIN F000901 E
                                      ON A.TICKET_TYPE = E.ORD_TYPE
                               LEFT JOIN F000906 F
                                      ON A.TICKET_CLASS = F.TICKET_CLASS
                               LEFT JOIN F1947 G
                                      ON A.ASSIGN_DELIVER = G.ALL_ID
                                         AND A.DC_CODE = G.DC_CODE
                               LEFT JOIN F19000101 H
                                      ON A.TICKET_ID = H.TICKET_ID
                               LEFT JOIN F19000102 I
                                      ON H.MILESTONE_NO = I.MILESTONE_NO
                        WHERE  A.DC_CODE = @p0
                               AND A.GUP_CODE = (CASE WHEN @p1 = '' THEN A.GUP_CODE ELSE @p1 END)
                               AND A.CUST_CODE = (CASE WHEN @p2 = '' THEN A.CUST_CODE ELSE @p2 END)
                               AND A.TICKET_TYPE = @p3
                               AND EXISTS (SELECT 1
                                           FROM   F192402 J
                                           WHERE  A.DC_CODE = J.DC_CODE
                                                  AND A.GUP_CODE = J.GUP_CODE
                                                  AND A.CUST_CODE = J.CUST_CODE
                                                  AND J.EMP_ID = @p4) 
                        ";

            return SqlQuery<F190001Data>(sql, sqlParams);
        }
    }
}
