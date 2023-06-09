using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
    public partial class F160201Repository : RepositoryBase<F160201, Wms3plDbContext, F160201Repository>
    {
        public F160201Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F160201> GetDatasByCustVnrRetrunNos(string dcCode, string gupCode, string custCode, List<string> custVnrRetrunNos)
        {
            return _db.F160201s.Where(x => x.DC_CODE == dcCode &&
                                           x.GUP_CODE == gupCode &&
                                           x.CUST_CODE == custCode &&
                                           x.STATUS != "9" &&
                                           custVnrRetrunNos.Contains(x.CUST_ORD_NO));
        }

        /// <summary>
        /// 取得一筆廠商退貨單資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="returnNo"></param>
        /// <returns></returns>
        public F160201 GetData(string dcCode, string gupCode, string custCode, string rtnVnrNo)
        {
            return _db.F160201s.Where(x => x.DC_CODE == dcCode &&
                                           x.GUP_CODE == gupCode &&
                                           x.CUST_CODE == custCode &&
                                           x.RTN_VNR_NO == rtnVnrNo &&
                                           x.STATUS != "9").SingleOrDefault();
        }

        public IQueryable<F160201> GetDatasByCustOrdNo(string dcCode, string gupCode, string custCode, string custOrdNo)
        {
            return _db.F160201s.Where(x => x.DC_CODE == dcCode &&
                                           x.GUP_CODE == gupCode &&
                                           x.CUST_CODE == custCode &&
                                           x.STATUS != "9" &&
                                           (x.CUST_ORD_NO == custOrdNo || x.RTN_VNR_NO == custOrdNo));
        }
    }
}
