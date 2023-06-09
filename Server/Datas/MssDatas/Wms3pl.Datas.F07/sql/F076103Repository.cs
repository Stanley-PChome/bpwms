using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.F07
{
  public partial class F076103Repository
  {
    public string LockF076103()
    {
      var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F076103';";
      return SqlQuery<string>(sql).FirstOrDefault();
    }

    public void Unlock(string dcCode, string custNo)
    {
      var sql = @" DELETE FROM F076103
                   WHERE DC_CODE = @p0
                     AND CUST_ORD_NO = @p1 ";

      ExecuteSqlCommand(sql, new object[] { dcCode, custNo });
    }
  }
}