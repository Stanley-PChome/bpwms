using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910301Repository : RepositoryBase<F910301, Wms3plDbContext, F910301Repository>
    {
        public IQueryable<F910301Report> GetContractReports(string dcCode, string gupCode, string contractNo)
        {
            var paramers = new List<SqlParameter>();

            paramers.Add(new SqlParameter("@p0", dcCode));
            paramers.Add(new SqlParameter("@p1", gupCode));
            paramers.Add(new SqlParameter("@p2", contractNo));


            var strSQL = @"
SELECT AA.CONTRACT_NO,
       CONVERT(VARCHAR(10), AA.ENABLE_DATE, 111)  as MAIN_ENABLE_DATE,
       CONVERT(VARCHAR(10), AA.DISABLE_DATE, 111) as MAIN_DISABLE_DATE,
       AA.OBJECT_NAME,
       AA.CONTACT,
       AA.TEL,
       AA.UNI_FORM,
       AA.MEMO,
       BB.CONTRACT_TYPENAME,
       BB.SUB_CONTRACT_NO,
       BB.ITEM_TYPE_NAME,
       BB.QUOTE_NAME,
       BB.UNIT,
       CONVERT(VARCHAR, BB.WORK_HOUR)             as WORK_HOUR,
       CONVERT(VARCHAR, BB.TASK_PRICE)            as TASK_PRICE,
       CONVERT(VARCHAR(10), BB.ENABLE_DATE, 111)  as ENABLE_DATE,
       CONVERT(VARCHAR(10), BB.DISABLE_DATE, 111) as DISABLE_DATE,
       CONVERT(VARCHAR, BB.PROCESS_ACT)           AS PROCESS_ACT,
       CONVERT(VARCHAR, BB.OUTSOURCE_COST)        AS OUTSOURCE_COST,
       CONVERT(VARCHAR, BB.APPROVE_PRICE)         AS APPROVE_PRICE
FROM   (SELECT a.*,
               CASE
                 WHEN a.OBJECT_TYPE = '0' THEN b.CUST_NAME
                 ELSE c.OUTSOURCE_NAME
               END as OBJECT_NAME,
               CASE
                 WHEN a.OBJECT_TYPE = '0' THEN b.CONTACT
                 ELSE c.CONTACT
               END as CONTACT,
               CASE
                 WHEN a.OBJECT_TYPE = '0' THEN b.TEL
                 ELSE c.TEL
               END as TEL
        FROM   F910301 a
               LEFT JOIN F1909 b
                      on a.GUP_CODE = b.GUP_CODE
                         AND a.UNI_FORM = b.UNI_FORM
                         AND b.STATUS != '9'
               LEFT JOIN F1928 c
                      on a.UNI_FORM = c.UNI_FORM
                         AND c.STATUS != '9') AA
       JOIN  (SELECT a.*,
               CASE
                 WHEN a.CONTRACT_TYPE = '0' THEN N'主約'
                 ELSE N'附約'
               END             as CONTRACT_TYPENAME,
               b.QUOTE_NAME,
               c.ITEM_TYPE     as ITEM_TYPE_NAME,
               d.ACC_UNIT_NAME UNIT,
               e.PROCESS_ACT
        FROM   F910301 aa
               LEFT JOIN F910302 a
                      ON aa.DC_CODE = a.DC_CODE
                         AND aa.GUP_CODE = a.GUP_CODE
                         AND aa.CONTRACT_NO = a.CONTRACT_NO
               LEFT JOIN F1909 cc
                      ON aa.GUP_CODE = cc.GUP_CODE
                         AND aa.UNI_FORM = cc.UNI_FORM
                         AND cc.STATUS != '9'
               LEFT JOIN F910401 b
                      ON a.DC_CODE = b.DC_CODE
                         AND a.GUP_CODE = b.GUP_CODE
                         AND cc.CUST_CODE = b.CUST_CODE
                         AND a.QUOTE_NO = b.QUOTE_NO
               LEFT JOIN F910003 c
                      ON a.ITEM_TYPE = c.ITEM_TYPE_ID
               Left Join F91000302 d
                      ON d.ACC_UNIT = a.UNIT_ID
                         AND d.ITEM_TYPE_ID = '001'
               LEFT JOIN F910001 e
                      ON a.PROCESS_ID = e.PROCESS_ID) BB ON AA.CONTRACT_NO = BB.CONTRACT_NO
   AND AA.DC_CODE = BB.DC_CODE
   AND AA.GUP_CODE = BB.GUP_CODE
WHERE  AA.DC_CODE = @p0
   AND AA.GUP_CODE = @p1
   AND AA.CONTRACT_NO = @p2
";

            return SqlQuery<F910301Report>(strSQL, paramers.ToArray());
        }
    }
}
