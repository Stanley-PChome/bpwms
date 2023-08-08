using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F194701Repository : RepositoryBase<F194701, Wms3plDbContext, F194701Repository>
    {
        public F194701Repository(string connName, WmsTransaction wmsTransaction = null)
       : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F194701WithF1934> GetF194701WithF1934s(string dcCode, string allID)
        {
            var f19470101s = _db.F19470101s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                                            && x.ALL_ID == allID);
            var f1934s = _db.F1934s.AsNoTracking();

            var result = from A in f19470101s
                         join B in f1934s on A.ZIP_CODE equals B.ZIP_CODE
                         group new { A, B } by new {A.DELV_TIME,B.COUDIV_ID} into g
                         orderby g.Key.DELV_TIME,g.Key.COUDIV_ID
                         select new F194701WithF1934
                         {
                             DELV_TIME = g.Key.DELV_TIME,
                             COUDIV_ID = g.Key.COUDIV_ID
                         };
            return result;
        }

        
    }
}
