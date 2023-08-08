using Microsoft.EntityFrameworkCore;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F191201Repository : RepositoryBase<F191201, Wms3plDbContext, F191201Repository>
    {
        public F191201Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 查詢PK區樓層
        /// </summary>
        /// <param name="dcCode"></param>
        /// <returns></returns>
        public IQueryable<string> GetF191201Floors(string dcCode)
        {
            return _db.F191201s
                    .AsNoTracking()
                    .Where(x => x.TYPE == "6")
                    .Where(x => x.DC_CODE == dcCode)
                    .Select(x => x.VALUE);

        }

    }
}
