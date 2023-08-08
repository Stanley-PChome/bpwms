using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
  public partial class F191204Repository : RepositoryBase<F191204, Wms3plDbContext, F191204Repository>
  {
    public F191204Repository(string connName, WmsTransaction wmsTransaction = null)
      : base(connName, wmsTransaction)
    {
    }

    public IQueryable<F1912> GetLockLocData(string dcCode, string gupCode, string custCode)
    {
      return _db.F191204s.Join(_db.F1912s, a => new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.LOC_CODE },
           b => new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.LOC_CODE }, (a, b) => new { a, b })
           .Where(x => x.a.DC_CODE == dcCode)
           .Where(x => x.a.GUP_CODE == gupCode)
           .Where(x => x.a.CUST_CODE == custCode)
           .Where(x => x.a.STATUS == "0")
           .Select(x => x.b);
    }

    public void DeleteByIDs(long[] Ids)
    {
      var StrIdPara = new StringBuilder();
      var para = new List<SqlParameter>();
      foreach (var item in Ids)
      {
        StrIdPara.Append($"@p{para.Count},");
        para.Add(new SqlParameter($"@p{para.Count}", item) { SqlDbType = SqlDbType.BigInt });
      }

      var sql = $"DELETE FROM F191204 WHERE ID IN({StrIdPara.ToString().TrimEnd(',')})";
      ExecuteSqlCommand(sql, para.ToArray());
    }
  }
}
