using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1928Repository : RepositoryBase<F1928, Wms3plDbContext, F1928Repository>
    {
        public F1928Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        public F1928 GetFirstData()
        {
            var result = _db.F1928s.Where(x => x.STATUS == "0").OrderBy(x => x.CRT_DATE).FirstOrDefault();
            return result;
            
        }
    }
}
