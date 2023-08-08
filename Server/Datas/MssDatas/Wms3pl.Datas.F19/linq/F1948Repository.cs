using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1948Repository : RepositoryBase<F1948, Wms3plDbContext, F1948Repository>
    {
        public F1948Repository(string connName, WmsTransaction wmsTransaction = null)
      : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F1948Data> GetF1948(string dcCode)
        {
            var result = _db.F1948s.AsNoTracking().Where(x => x.STATUS == "0"
                                                       && x.DC_CODE == dcCode).OrderBy(x => x.ACC_AREA).
                                                       Select(x => new F1948Data
                                                       {
                                                           ACC_AREA_ID = x.ACC_AREA_ID,
                                                           ACC_AREA = x.ACC_AREA
                                                       });
            return result;
        }

        public IQueryable<string> GetZipCodes(decimal accAreaId)
        {
            var f1948s = _db.F1948s.AsNoTracking().Where(x => x.ACC_AREA_ID == accAreaId
                                                        && x.STATUS == "0");
            var f194801s = _db.F194801s.AsNoTracking();

            var result = from A in f1948s
                         join B in f194801s on A.ACC_AREA_ID equals B.ACC_AREA_ID
                         select B.ZIP_CODE;
            return result;
        }
    }
}
