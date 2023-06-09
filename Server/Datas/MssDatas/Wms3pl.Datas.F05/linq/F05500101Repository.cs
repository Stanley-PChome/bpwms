using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System.Data.Objects;

namespace Wms3pl.Datas.F05
{
    public partial class F05500101Repository : RepositoryBase<F05500101, Wms3plDbContext, F05500101Repository>
    {
        public F05500101Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="wmsOrdNo"></param>
    /// <returns></returns>
    public IQueryable<F05500101> GetAllDataByShipPackageService(string dcCode, string gupCode, string custCode, string wmsOrdNo)
    {
      var result = _db.F05500101s
            .Where(x => x.WMS_ORD_NO == wmsOrdNo &&
                x.DC_CODE == dcCode &&
                x.GUP_CODE == gupCode &&
                x.CUST_CODE == custCode)
            .Where(o => !string.IsNullOrWhiteSpace(o.SERIAL_NO) && o.ISPASS == "1")
            .OrderByDescending(x => x.LOG_SEQ)
            .GroupBy(x => x.SERIAL_NO)
            .Select(x => x.OrderByDescending(a => a.CRT_DATE).First());
      return result;
    }

    public int GetNextLogSeq(string dcCode, string gupCode, string custCode, string wmsOrdNo, short packageBoxNo)
        {
            var maxSeq = Filter(x => x.WMS_ORD_NO == EntityFunctions.AsNonUnicode(wmsOrdNo)
                                    && x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
                                    && x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
                                    && x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
                                    && x.PACKAGE_BOX_NO == packageBoxNo)
                        .Max(x => (int?)x.LOG_SEQ);
            return (maxSeq ?? 0) + 1;
        }
    }
}