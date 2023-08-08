using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F194708Repository : RepositoryBase<F194708, Wms3plDbContext, F194708Repository>
    {
        public F194708Repository(string connName, WmsTransaction wmsTransaction = null)
       : base(connName, wmsTransaction)
        {
        }

        public IQueryable<string> GetZipCode(string dcCode, string allId, decimal accAreaId)
        {
            var f194708s = _db.F194708s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                                            && x.ALL_ID == allId
                                                            && x.ACC_AREA_ID == accAreaId);
            var f19470801s = _db.F19470801s.AsNoTracking();

            var result = from A in f194708s
                         join B in f19470801s on A.ACC_AREA_ID equals B.ACC_AREA_ID
                         select B.ZIP_CODE;

            return result;
        }
    }
}
