using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F91000301Repository : RepositoryBase<F91000301, Wms3plDbContext, F91000301Repository>
    {
        public F91000301Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F91000301Data> GetAccItemKinds(string itemTypeId)
        {
            var q = from a in _db.F91000301s
                     select a;
            if (!string.IsNullOrEmpty(itemTypeId))
            {
                q = q.Where(c => c.ITEM_TYPE_ID == itemTypeId);
            }

            var result = q.Select(c => new F91000301Data
            {
                ACC_ITEM_KIND_ID = c.ACC_ITEM_KIND_ID,
                ACC_ITEM_KIND_NAME = c.ACC_ITEM_KIND_NAME
            }).Distinct();
            return result;
        }
    }
}
