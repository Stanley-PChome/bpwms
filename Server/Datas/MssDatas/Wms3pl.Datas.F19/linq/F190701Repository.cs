using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
  public partial class F190701Repository : RepositoryBase<F190701, Wms3plDbContext, F190701Repository>
  {
    public F190701Repository(string connName, WmsTransaction wmsTransaction = null)
      : base(connName, wmsTransaction)
    {
    }

    public IQueryable<F190701> GetQueryListByGroupId(string gid)
    {
        return _db.F190701s.Join(_db.F190704s, A => A.QID, B => B.QID, (A, B) => new { A, B })
                .Where(x => x.B.GRP_ID.ToString() == gid)
                .OrderBy(x => x.A.QID)
                                .Select(x => x.A);
    }

        public IQueryable<F190701> GetQueryListByEmpId(string empId, string qGroup)
        {
            var query = _db.F190701s
                            .Join(_db.F190704s, f190701 => f190701.QID, f190704 => f190704.QID, (f190701, f190704) => new { f190701, f190704 })
                            .Join(_db.F192401s, AB => AB.f190704.GRP_ID, f192401 => f192401.GRP_ID, (AB, f192401) => new { AB, f192401 })
                            .Join(_db.F1924s, ABC => ABC.f192401.EMP_ID, f1924 => f1924.EMP_ID, (ABCD, f1924) => new { ABCD, f1924 });
            query = query.Where(x => x.f1924.EMP_ID == empId);
            if (!string.IsNullOrEmpty(qGroup))
                query = query.Where(x => x.ABCD.AB.f190701.QGROUP == qGroup);
            return query.OrderBy(x => x.ABCD.AB.f190701.NAME)
                        .Select(x => x.ABCD.AB.f190701);
        }
    }
}
