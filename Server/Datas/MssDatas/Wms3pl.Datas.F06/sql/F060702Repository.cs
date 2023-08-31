﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Wms3pl.Datas.F06
{
  public partial class F060702Repository
  {

    /// <summary>
    /// 取得集貨等待通知排程用資料
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <returns></returns>
    public IQueryable<F060702> getWCSCollectionData(string dcCode, string gupCode, string custCode, int midApiRelmt)
    {
      var sql = @"SELECT * FROM F060702 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND PROC_FLAG IN('0','T') AND RESENT_CNT<@p3;";
      var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=gupCode},
                new SqlParameter("@p2",SqlDbType.VarChar){Value=custCode},
                new SqlParameter("@p3",SqlDbType.Int){Value=midApiRelmt}
            };

      return SqlQuery<F060702>(sql, para.ToArray());
    }

    /// <summary>
    /// 檢查是否存在等待補檢(status=1)的集貨等待通知
    /// </summary>
    /// <returns></returns>
    public Boolean IsExistStatus1Data(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode },
        new SqlParameter("@p3", SqlDbType.VarChar) { Value = wmsNo },
      };

      var sql = @"SELECT TOP 1 1 FROM F060702 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND ORI_ORDER_CODE=@p3 AND STATUS='1'";
      return SqlQuery<int>(sql, para.ToArray()).Any();
    }

  }
}
