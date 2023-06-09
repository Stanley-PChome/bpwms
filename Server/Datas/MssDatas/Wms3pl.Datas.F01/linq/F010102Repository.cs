using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
    public partial class F010102Repository : RepositoryBase<F010102, Wms3plDbContext, F010102Repository>
    {
        public F010102Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 查詢採購單內的明細
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="shopNo"></param>
        /// <returns></returns>
        public IQueryable<F010102Data> GetF010102Datas(string dcCode, string gupCode, string custCode, string shopNo)
        {
            #region 採購單明細資料
            var f010102Data = _db.F010102s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     x.SHOP_NO == shopNo);
            #endregion

            #region 組資料
            var result = (from A in f010102Data
                          join B in _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode &&
                                                                         f010102Data.Select(z => z.ITEM_CODE).Distinct().Contains(x.ITEM_CODE))
                          on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE } into subB
                          from B in subB.DefaultIfEmpty()
                          select new F010102Data
                          {
                              SHOP_NO = A.SHOP_NO,
                              SHOP_SEQ = A.SHOP_SEQ,
                              ITEM_CODE = A.ITEM_CODE,
                              ITEM_NAME = B.ITEM_NAME ?? null,
                              ITEM_SIZE = B.ITEM_SIZE ?? null,
                              ITEM_SPEC = B.ITEM_SPEC ?? null,
                              ITEM_COLOR = B.ITEM_COLOR ?? null,
                              SHOP_QTY = Convert.ToInt32(A.SHOP_QTY)
                          }).OrderBy(x => x.SHOP_SEQ);
            #endregion

            return result;
        }
    }
}
