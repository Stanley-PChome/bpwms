using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
    public partial class F161201Repository : RepositoryBase<F161201, Wms3plDbContext, F161201Repository>
    {
        public F161201Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }
        public IQueryable<F161201> GetDatasByCustOrdNoAndStoreNoNotCancel(string dcCode, string gupCode, string custCode, string custOrdNo, string retailCode)
        {
            return _db.F161201s
                    .Where(x => x.DC_CODE == dcCode)
                    .Where(x => x.GUP_CODE == gupCode)
                    .Where(x => x.CUST_CODE == custCode)
                    .Where(x => x.CUST_ORD_NO == custOrdNo)
                    .Where(x => x.RTN_CUST_CODE == retailCode)
                    .Where(x => x.STATUS != "9")
                    .Select(x => x);
        }

        public F161201 GetItem(string dcCode, string gupCode, string custCode, string returnNo)
        {
            return _db.F161201s
                    .Where(x => x.DC_CODE == dcCode)
                    .Where(x => x.GUP_CODE == gupCode)
                    .Where(x => x.CUST_CODE == custCode)
                    .Where(x => x.RETURN_NO == returnNo)
                    .Select(x => x).FirstOrDefault();
        }
        public IQueryable<F161201> GetDatasByCustOrdNo(string dcCode, string gupCode, string custCode, List<string> custOrdNoList)
        {
            return _db.F161201s
                    .Where(x => x.DC_CODE == dcCode)
                    .Where(x => x.GUP_CODE == gupCode)
                    .Where(x => x.CUST_CODE == custCode)
                    .Where(x => x.STATUS != "9")
                    .Where(x => custOrdNoList.Contains(x.CUST_ORD_NO))
                    .Select(x => x);
        }

        /// <summary>
        /// 取得一筆客戶退貨單資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="returnNo"></param>
        /// <returns></returns>
        public F161201 GetData(string dcCode, string gupCode, string custCode, string returnNo)
        {
            return _db.F161201s.Where(x => x.DC_CODE == dcCode &&
                                           x.GUP_CODE == gupCode &&
                                           x.CUST_CODE == custCode &&
                                           x.RETURN_NO == returnNo &&
                                           x.STATUS != "9").SingleOrDefault();
        }
    }
}