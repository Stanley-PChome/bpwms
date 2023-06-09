using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910003Repository : RepositoryBase<F910003, Wms3plDbContext, F910003Repository>
    {
        public F910003Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F910003> GetF910003Datas(string ITEM_TYPE_ID, string ITEM_TYPE)
        {
            var q = from a in _db.F910003s
                    select a;

            if (!string.IsNullOrWhiteSpace(ITEM_TYPE_ID))
            {
                q = q.Where(c => c.ITEM_TYPE_ID == ITEM_TYPE_ID);
            }

            if (!string.IsNullOrWhiteSpace(ITEM_TYPE))
            {
                q = q.Where(c => c.ITEM_TYPE.Contains(ITEM_TYPE));
            }

            q = q.OrderBy(c => c.ITEM_TYPE_ID);
            return q;
        }

        public IQueryable<F910003> GetDatas(string itemType)
        {
            return _db.F910003s.AsNoTracking().Where(x => x.ITEM_TYPE == itemType);
        }
    }
}
