using System;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F050803Repository : RepositoryBase<F050803, Wms3plDbContext, F050803Repository>
	{
        
    

        public void InsertByF0515(string dcCode, string gupCode, string custCode, string batchNo)
        {
            var sql = $@"  INSERT INTO F050803(DC_CODE,GUP_CODE,CUST_CODE,WMS_ORD_NO,RETAIL_CODE,DELV_DATE,ARRIVAL_DATE,DELV_NO,
						   CAR_PERIOD,CAR_GUP,DRIVER_ID,DRIVER_NAME,EXTRA_FEE,DELV_WAY,ARRIVAL_TIME_S,ARRIVAL_TIME_E,
                           CRT_STAFF,CRT_NAME,CRT_DATE)								 
                    SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,C.WMS_ORD_NO,C.RETAIL_CODE,C.DELV_DATE,C.ARRIVAL_DATE,D.DELV_NO,
						   E.CAR_PERIOD,E.CAR_GUP,E.DRIVER_ID,E.DRIVER_NAME,E.EXTRA_FEE,D.DELV_WAY,D.ARRIVAL_TIME_S,D.ARRIVAL_TIME_E,
                           '{Current.Staff}'   CRT_STAFF,'{Current.StaffName}' CRT_NAME,@p0 CRT_DATE
						    FROM F0515 A
						    JOIN F051503 B
						    ON B.DC_CODE = A.DC_CODE
						    AND B.GUP_CODE = A.GUP_CODE
						    AND B.CUST_CODE = A.CUST_CODE
						    AND B.BATCH_NO = A.BATCH_NO
						    JOIN F050801 C
						    ON C.DC_CODE = B.DC_CODE
						    AND C.GUP_CODE = B.GUP_CODE
						    AND C.CUST_CODE = B.CUST_CODE
						    AND C.PICK_ORD_NO = B.PICK_ORD_NO
						    JOIN F19471601 D
						    ON D.DC_CODE = C.DC_CODE
						    AND D.GUP_CODE = C.GUP_CODE
						    AND D.CUST_CODE = C.CUST_CODE
						    AND D.RETAIL_CODE = C.RETAIL_CODE
						    JOIN F194716 E
						    ON E.DC_CODE = D.DC_CODE
						    AND E.GUP_CODE = D.GUP_CODE
						    AND E.CUST_CODE = D.CUST_CODE
						    AND E.DELV_NO = D.DELV_NO
						    LEFT JOIN F050803 F
						    ON F.DC_CODE = C.DC_CODE
						    AND F.GUP_CODE = C.GUP_CODE
						    AND F.CUST_CODE = C.CUST_CODE
						    AND F.WMS_ORD_NO = C.WMS_ORD_NO
						    WHERE F.WMS_ORD_NO	IS NULL 
						    AND A.DC_CODE = @p1
						    AND A.GUP_CODE = @p2
						    AND A.CUST_CODE = @p3
						    AND A.BATCH_NO=@p4";

            var parms = new object[] { DateTime.Now, dcCode, gupCode, custCode, batchNo };
            ExecuteSqlCommand(sql, parms);
        }
    }
}
