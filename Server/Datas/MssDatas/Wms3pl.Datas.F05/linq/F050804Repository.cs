using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F050804Repository : RepositoryBase<F050804, Wms3plDbContext, F050804Repository>
    {
        public F050804Repository(string connName, WmsTransaction wmsTransaction = null)
             : base(connName, wmsTransaction)
        {
        }
        public F050804 GetF050804(string dcCode, string GupCode, string custCode, string stickerNo)
        {
            var result = _db.F050804s.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == GupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 x.STICKER_NO == stickerNo).FirstOrDefault();

            return result;
        }

        public IQueryable<F050804> GetF050804s(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var result = _db.F050804s.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 x.WMS_ORD_NO == wmsOrdNo);

            return result;
        }
    }
}
