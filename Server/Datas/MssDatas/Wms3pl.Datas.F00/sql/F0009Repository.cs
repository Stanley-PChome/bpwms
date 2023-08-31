using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
  public partial class F0009Repository : RepositoryBase<F0009, Wms3plDbContext, F0009Repository>
  {
    public string LockF0009()
    {
      var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F0009';";
      return SqlQuery<string>(sql).FirstOrDefault();
    }

    public void GetSequence(string ordType, int count, out int seq)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", ordType) { SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
        new SqlParameter("@p1", Current.Staff) { SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
        new SqlParameter("@p2", Current.StaffName) { SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
        new SqlParameter("@p3", count) { SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input },
        new SqlParameter("@p4", SqlDbType.Int) {  Direction = ParameterDirection.Output },
      };

      var sql = @" 
                Exec SP_GetNewOrderSeq 
                  @OrdType = @p0,
                  @Staff = @p1,
                  @StaffName = @p2, 
                  @NeedQty = @p3,
                  @Seq = @p4 OUTPUT
                ";

      ExecuteSqlCommand(sql, out seq, param.ToArray());
    }
  }
}
