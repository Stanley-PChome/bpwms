using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
    public partial class F050302Repository : RepositoryBase<F050302, Wms3plDbContext, F050302Repository>
    {
        public F050302Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 取得已使用指定序號且尚未配庫的訂單編號
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="serialNo"></param>
        /// <returns></returns>
        public string GetOrdNoByUsedAssignationSerial(string dcCode, string gupCode, string custCode, string serialNo)
        {
            var f050302Data = _db.F050302s.AsNoTracking().Where(x => x.SERIAL_NO == serialNo &&
                                                                     x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode);

            var f050301Data = _db.F050301s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     x.PROC_FLAG == "0" &&
                                                                     f050302Data.Select(z => z.ORD_NO).Contains(x.ORD_NO));

            var result = from A in f050301Data
                         join B in f050302Data
                         on new { A.ORD_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE } equals new { B.ORD_NO, B.DC_CODE, B.GUP_CODE, B.CUST_CODE }
                         select new
                         {
                             B.ORD_NO
                         };

            return result.FirstOrDefault() != null ? result.FirstOrDefault().ORD_NO : null;
        }
    }
}
