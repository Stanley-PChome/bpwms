using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F192403Repository : RepositoryBase<F192403, Wms3plDbContext, F192403Repository>
    {
        public F192403Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 取得人員倉別資料
        /// </summary>
        /// <param name="accNo">帳號</param>
        /// <returns></returns>
        public IQueryable<WarehouseInfo> GetUserWarehouse(string accNo)
        {
            var f192403s = _db.F192403s.AsNoTracking().Where(x => x.EMP_ID == accNo);
            var f196301s = _db.F196301s.AsNoTracking();
            var f1912s = _db.F1912s.AsNoTracking();
            var f1980s = _db.F1980s.AsNoTracking();
            var result = (from A in f192403s
                         join B in f196301s on A.WORK_ID equals B.WORK_ID
                         join C in f1912s on B.LOC_CODE equals C.LOC_CODE
                         join D in f1980s on new { C.DC_CODE, C.WAREHOUSE_ID } equals new { D.DC_CODE, D.WAREHOUSE_ID }
                         select new WarehouseInfo
                         {
                             WhNo = D.WAREHOUSE_ID,
                             WhName = D.WAREHOUSE_NAME
                         }).Distinct();
            return result;
        }

        /// <summary>
        /// 檢核是否登入者有權限
        /// </summary>
        /// <param name="empId">作業人員帳號</param>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="locCode">實際儲位</param>
        /// <returns></returns>
        public bool CheckActLoc(string empId, string dcCode, string locCode)
        {

            var worlIdList = _db.F192403s.AsNoTracking().Where(x => x.EMP_ID == empId).Select(x => x.WORK_ID);
            var result = _db.F196301s.AsNoTracking().Where(x => worlIdList.Contains(x.WORK_ID)
                                                        && x.DC_CODE == dcCode
                                                        && x.LOC_CODE == locCode);

            return result.Count() > 0;

        }
    }
}
