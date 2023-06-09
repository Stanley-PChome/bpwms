using System.Collections.Generic;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051601Repository : RepositoryBase<F051601, Wms3plDbContext, F051601Repository>
	{
        public void InsertF051601ByBatchNo(string dcCode, string gupCode, string custCode, string batchNo)
        {
            var parms = new List<object> { Current.Staff, Current.StaffName, dcCode, gupCode, custCode, batchNo };
            var sql = @" INSERT INTO F051601(DC_CODE,GUP_CODE,CUST_CODE,BATCH_DATE,BATCH_NO,ITEM_CODE,ITEM_NAME,ITEM_UNIT,PACK_LABEL,PACK_QTY,STATUS,CRT_DATE,CRT_STAFF,CRT_NAME)
									SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,D.BATCH_DATE,D.BATCH_NO ,A.ITEM_CODE,E.ITEM_NAME,F.ACC_UNIT_NAME ITEM_UNIT,COUNT(DISTINCT B.RETAIL_CODE) PACK_LABEL,SUM(B_PICK_QTY) PACK_QTY,'0' STATUS,dbo.GetSysDate() CRT_DATE,@p0 CRT_STAFF,@p1 CRT_NAME
										FROM F051202 A
										JOIN F050801 B
										ON B.DC_CODE = A.DC_CODE
										AND B.GUP_CODE = A.GUP_CODE
										AND B.CUST_CODE = A.CUST_CODE
										AND B.WMS_ORD_NO = A.WMS_ORD_NO
										JOIN F051503 C
										ON C.DC_CODE = A.DC_CODE
										AND C.GUP_CODE = A.GUP_CODE
										AND C.CUST_CODE = A.CUST_CODE
										AND C.PICK_ORD_NO = A.PICK_ORD_NO
										JOIN F0515 D
										ON D.DC_CODE = C.DC_CODE
										AND D.GUP_CODE = C.GUP_CODE
										AND D.CUST_CODE = C.CUST_CODE
										AND D.BATCH_NO = C.BATCH_NO
										JOIN F1903 E
										ON E.GUP_CODE = A.GUP_CODE
										AND E.ITEM_CODE = A.ITEM_CODE
                                        AND E.CUST_CODE = A.CUST_CODE
										JOIN F91000302 F 
										ON F.ITEM_TYPE_ID='001'
										AND F.ACC_UNIT = E.ITEM_UNIT
										WHERE D.DC_CODE= @p2
											AND D.GUP_CODE = @p3
											AND D.CUST_CODE =@p4
											AND D.BATCH_NO =@p5
										GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,D.BATCH_DATE,D.BATCH_NO,A.ITEM_CODE,E.ITEM_NAME,F.ACC_UNIT_NAME";
            ExecuteSqlCommand(sql, parms.ToArray());
        }
    }
}
