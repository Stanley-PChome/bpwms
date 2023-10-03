using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
    public partial class F060302Repository
    {
      public void AddReleaseRecord(string dcCode, string gupCode, string custCode, string wmsNo, long? f0701Id)
      {
        var param = new List<SqlParameter>
        {
          new SqlParameter("@p0", SqlDbType.DateTime2) { Value = DateTime.Now },
          new SqlParameter("@p1", SqlDbType.VarChar) { Value = Current.Staff },
          new SqlParameter("@p2", SqlDbType.NVarChar) { Value = Current.StaffName },
          new SqlParameter("@p3", SqlDbType.VarChar) { Value = dcCode },
          new SqlParameter("@p4", SqlDbType.VarChar) { Value = gupCode },
          new SqlParameter("@p5", SqlDbType.VarChar) { Value = custCode },
          new SqlParameter("@p6", SqlDbType.VarChar) { Value = wmsNo }
        };

        var sql = @"
                  INSERT INTO F060302(
                    DC_CODE,
                    CUST_CODE,
                    WAREHOUSE_ID,
                    CONTAINER_CODE,
                    STATUS,
                    CRT_DATE,
                    CRT_STAFF,
                    CRT_NAME)
                  SELECT 
                    B.DC_CODE,
                    B.CUST_CODE,
                    B.WAREHOUSE_ID,
                    B.CONTAINER_CODE,
                    '0' STATUS,
                    @p0 CRT_DATE, 
                    @p1 CRT_STAFF,
                    @p2 CRT_NAME
                  FROM F070101 A
                  JOIN F0701 B
                    ON B.ID = A.F0701_ID
                  LEFT JOIN F060302 C
                    ON C.DC_CODE = B.DC_CODE
                    AND C.CUST_CODE = B.CUST_CODE
                    AND C.WAREHOUSE_ID = B.WAREHOUSE_ID
                    AND C.CONTAINER_CODE = B.CONTAINER_CODE
                    AND C.STATUS IN('0','T')
                  JOIN F1980 D
                    ON D.DC_CODE = B.DC_CODE
                    AND D.WAREHOUSE_ID = B.WAREHOUSE_ID
                  WHERE 
                    D.DEVICE_TYPE !='0' 
                    AND C.STATUS IS NULL 
                    AND A.DC_CODE = @p3
                    AND A.GUP_CODE = @p4
                    AND A.CUST_CODE = @p5
                    AND A.WMS_NO = @p6
                  ";

      if (!string.IsNullOrWhiteSpace(f0701Id.ToString()))
      {
        sql += $" AND B.ID = @p{param.Count}";
        param.Add(new SqlParameter($"@p{param.Count}", SqlDbType.BigInt) { Value = f0701Id });
      }

        ExecuteSqlCommand(sql, param.ToArray());
      }

    public void AddReleaseRecord2(long f0531_ID)
    {
      var param = new List<SqlParameter>
        {
          new SqlParameter("@p0", SqlDbType.DateTime2) { Value = DateTime.Now },
          new SqlParameter("@p1", SqlDbType.VarChar) { Value = Current.Staff },
          new SqlParameter("@p2", SqlDbType.NVarChar) { Value = Current.StaffName }
        };

      var sql = @"
                  INSERT INTO F060302(
                    DC_CODE,
                    CUST_CODE,
                    WAREHOUSE_ID,
                    CONTAINER_CODE,
                    STATUS,
                    CRT_DATE,
                    CRT_STAFF,
                    CRT_NAME)
                  SELECT 
                    A.DC_CODE,
                    A.CUST_CODE,
                    A.WAREHOUSE_ID,
                    A.CONTAINER_CODE,
                    '0' STATUS,
                    @p0 CRT_DATE, 
                    @p1 CRT_STAFF,
                    @p2 CRT_NAME
                  FROM F0701 A
                  JOIN F070101 B
                    ON A.ID = B.F0701_ID
                  LEFT JOIN F060302 C
                    ON C.DC_CODE = A.DC_CODE
                    AND C.CUST_CODE = A.CUST_CODE
                    AND C.WAREHOUSE_ID = A.WAREHOUSE_ID
                    AND C.CONTAINER_CODE = A.CONTAINER_CODE
                    AND C.STATUS IN('0','T')
                  WHERE 
                    A.ID IN(SELECT F0701_ID FROM F053201 WHERE F0531_ID = @p0)
                  ";

      ExecuteSqlCommand(sql, param.ToArray());
    }
  }
}
