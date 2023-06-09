using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1934Repository : RepositoryBase<F1934, Wms3plDbContext, F1934Repository>
    {
        public F1934Repository(string connName, WmsTransaction wmsTransaction = null)
      : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F1934EX> GetF1934EXDatas()
        {
            var result = _db.F1934s.AsNoTracking().Select(x=>new F1934EX {
                ZIP_CODE = x.ZIP_CODE,
                ZIP_NAME = x.ZIP_NAME,
                COUDIV_ID = x.COUDIV_ID,
                ISCHECKED = 0
            });
            return result;
        }

        public IQueryable<F1934> GetF1934Datas(string COUDIV_ID)
        {
            var result = _db.F1934s.AsQueryable();
            if (!string.IsNullOrWhiteSpace(COUDIV_ID))
            {
                result = result.Where(x => x.COUDIV_ID == COUDIV_ID);
            }
            result = result.OrderBy(x => x.COUDIV_ID);
            return result;
        }
    }
}
