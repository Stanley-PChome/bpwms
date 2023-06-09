using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F0515Repository : RepositoryBase<F0515, Wms3plDbContext, F0515Repository>
	{
        public IQueryable<P050112Batch> GetP050112Batches(string dcCode, string gupCode, string custCode, DateTime batchDateS, DateTime batchDateE, string batchNo, string pickStatus, string putStatus)
        {
            var parms = new List<object> { Current.Lang, Current.Lang, Current.Lang, Current.Lang, Current.Lang, dcCode, gupCode, custCode, batchDateS, batchDateE };
            var sql = @" select ROW_NUMBER()OVER(ORDER BY A.BATCH_NO  ASC) ROWNUM,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.BATCH_DATE,A.BATCH_NO,A.PICK_STATUS,B.NAME PICK_STATUS_NAME,A.PUT_STATUS,C.NAME PUT_STATUS_NAME,A.PICK_TOOL,E.NAME PICK_TOOL_NAME,A.PUT_TOOL,F.NAME PUT_TOOL_NAME,A.ALLOT_CNT,A.ALLOT_TYPE,D.NAME ALLOT_TYPE_NAME,A.RETAIL_CNT,A.ITEM_CNT,A.TOTAL_QTY,A.TRANS_DATE,A.RECV_DATE
										from F0515 A
										JOIN VW_F000904_LANG B
										ON B.TOPIC='F0515'
										AND B.SUBTOPIC='PICK_STATUS'
										AND B.VALUE = A.PICK_STATUS
										AND B.LANG = @p0
										JOIN VW_F000904_LANG C
										ON C.TOPIC='F0515'
										AND C.SUBTOPIC='PUT_STATUS'
										AND C.VALUE = A.PUT_STATUS
										AND C.LANG = @p1
										JOIN VW_F000904_LANG D
										ON D.TOPIC='F0515'
										AND D.SUBTOPIC='ALLOT_TYPE'
										AND D.VALUE = A.ALLOT_TYPE
										AND D.LANG = @p2
										JOIN VW_F000904_LANG E
										ON E.TOPIC='F191902'
										AND E.SUBTOPIC='PICK_TOOL'
										AND E.VALUE = A.PICK_TOOL
										AND E.LANG = @p3
										LEFT JOIN VW_F000904_LANG F
										ON F.TOPIC='F191902'
										AND F.SUBTOPIC='PUT_TOOL'
										AND F.VALUE = A.PUT_TOOL
										AND F.LANG = @p4
										WHERE A.DC_CODE = @p5
										AND A.GUP_CODE = @p6
										AND A.CUST_CODE =@p7
										AND A.BATCH_DATE >= @p8 AND A.BATCH_DATE <= @p9 ";
            if (!string.IsNullOrWhiteSpace(pickStatus))
            {
                sql += " AND A.PICK_STATUS = @p" + parms.Count;
                parms.Add(pickStatus);
            }
            else
            {
                sql += " AND A.PICK_STATUS <>'9' ";
            }
            if (!string.IsNullOrWhiteSpace(putStatus))
            {
                sql += " AND A.PUT_STATUS = @p" + parms.Count;
                parms.Add(putStatus);
            }
            else
            {
                sql += " AND A.PUT_STATUS <>'9' ";
            }
            if (!string.IsNullOrWhiteSpace(batchNo))
            {
                sql += " AND A.BATCH_NO = @p" + parms.Count;
                parms.Add(batchNo);
            }
            sql += " ORDER BY A.BATCH_NO ";

            var result = SqlQuery<P050112Batch>(sql, parms.ToArray());

            return result;
        }
    }
}
