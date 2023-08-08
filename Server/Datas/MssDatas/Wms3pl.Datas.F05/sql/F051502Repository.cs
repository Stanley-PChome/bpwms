using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051502Repository : RepositoryBase<F051502, Wms3plDbContext, F051502Repository>
	{
        public void UpdateStatusByBatchNo(string dcCode, string gupCode, string custCode, string batchNo, string status)
        {
            var sql = @" UPDATE A SET A.STATUS=@p0,UPD_DATE =@p1,UPD_STAFF=@p2,UPD_NAME =@p3 from F051502 A
									WHERE EXISTS(SELECT DC_CODE,GUP_CODE,CUST_CODE,BATCH_PICK_NO
											FROM F051501 B
										WHERE B.DC_CODE =@p4
											AND B.GUP_CODE =@p5
											AND B.CUST_CODE =@p6
											AND B.BATCH_NO = @p7
											AND B.DC_CODE = A.DC_CODE
											AND B.GUP_CODE = A.GUP_CODE
											AND B.CUST_CODE = A.CUST_CODE
											AND B.BATCH_PICK_NO = A.BATCH_PICK_NO
											)";
            var parms = new List<object> { status, DateTime.Now, Current.Staff, Current.StaffName, dcCode, gupCode, custCode, batchNo };
            ExecuteSqlCommand(sql, parms.ToArray());
        }

       

        public IQueryable<PickReportData> GetPickReportDatas(string dcCode, string gupCode, string custCode, string batchNo)
        {
            var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.AREA_CODE,A.SHELF_NO,A.LOC_CODE,A.ITEM_CODE ASC) ROWNUM,A.*
										FROM (
										SELECT TOP 100 PERCENT E.AREA_CODE,E.AREA_NAME,CASE WHEN F.PICK_TOOL = '4' THEN  A.SHELF_NO ELSE '' END SHELF_NO,A.LOC_CODE,A.ITEM_CODE,C.ITEM_NAME,A.B_PICK_QTY,CASE WHEN A.STATUS IN('0','9') THEN NULL ELSE A.A_PICK_QTY END A_PICK_QTY
											FROM F051502 A
											JOIN F051501 B
											ON B.DC_CODE = A.DC_CODE
											AND B.GUP_CODE = A.GUP_CODE
											AND B.CUST_CODE = A.CUST_CODE
											AND B.BATCH_PICK_NO = A.BATCH_PICK_NO
											JOIN F1903 C
											ON C.GUP_CODE = A.GUP_CODE
											AND C.CUST_CODE = A.CUST_CODE
											AND C.ITEM_CODE = A.ITEM_CODE
											JOIN F1912 D
											ON D.DC_CODE = A.DC_CODE
											AND D.LOC_CODE = A.LOC_CODE
											JOIN F1919 E
											ON E.DC_CODE = D.DC_CODE
											AND E.AREA_CODE = D.AREA_CODE
											JOIN F0515 F
											ON F.DC_CODE = B.DC_CODE
											AND F.GUP_CODE = B.GUP_CODE
											AND F.CUST_CODE = B.CUST_CODE
											AND F.BATCH_NO = B.BATCH_NO
											WHERE B.DC_CODE = @p0
											AND B.GUP_CODE = @p1
											AND B.CUST_CODE = @p2
											AND B.BATCH_NO =@p3
											ORDER BY E.AREA_CODE,SHELF_NO,LOC_CODE,ITEM_CODE) A ";

            var parms = new List<object> { dcCode, gupCode, custCode, batchNo };

            var result = SqlQuery<PickReportData>(sql, parms.ToArray());

            return result;
        }

       
    }
}
