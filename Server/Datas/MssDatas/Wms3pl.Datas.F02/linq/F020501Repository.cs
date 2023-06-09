using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
   public partial class F020501Repository : RepositoryBase<F020501, Wms3plDbContext, F020501Repository>
    {
        public F020501Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F020501> GetDatasByIds(List<long> f020501Ids)
        {
            return _db.F020501s.AsNoTracking().Where(x => f020501Ids.Contains(x.ID));
        }

        public F020501 GetDatasByF0701idAndExcepStatus(long f0701Id, string[] stauts)
        {
          return _db.F020501s.AsNoTracking().Where(o => o.F0701_ID == f0701Id && !stauts.Contains(o.STATUS)).FirstOrDefault();
        }
  }
}
