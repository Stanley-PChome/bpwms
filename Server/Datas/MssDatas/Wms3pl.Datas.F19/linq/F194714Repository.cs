using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F194714Repository : RepositoryBase<F194714, Wms3plDbContext, F194714Repository>
    {
        public F194714Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        public List<F194714> GetAll()
        {
            var result = _db.F194714s;
            return result.ToList();
        }

        public List<F194714> GetDatasByAllId(string allId)
        {
            var result = _db.F194714s.Where(x => x.ALL_ID == allId);
            return result.ToList();
        }
    }
}
