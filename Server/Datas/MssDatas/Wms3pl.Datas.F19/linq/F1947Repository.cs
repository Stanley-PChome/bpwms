using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1947Repository : RepositoryBase<F1947, Wms3plDbContext, F1947Repository>
    {
        public F1947Repository(string connName, WmsTransaction wmsTransaction = null)
      : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 取得有權限的配送商主檔
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public IQueryable<F1947> GetAllowedF1947s(string dcCode, string gupCode, string custCode)
        {
            var f1947s = _db.F1947s.Where(x => x.DC_CODE == dcCode);
            var f194704s = _db.F194704s.Where(x =>x.DC_CODE == dcCode 
                                               && x.GUP_CODE == gupCode 
                                               && x.CUST_CODE == custCode);

            var result = from A in f1947s
                           join B in  f194704s on new {A.DC_CODE,A.ALL_ID} equals new {B.DC_CODE,B.ALL_ID}
                           select A;

            return result;
        }

        /// <summary>
        /// 取得配送商資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <returns></returns>
        public IQueryable<F1947> GetDatas(string dcCode)
        {
            return _db.F1947s.Where(x => x.DC_CODE == dcCode);
        }
    }
}
