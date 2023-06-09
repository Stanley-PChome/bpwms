using System.Collections.Generic;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
    public partial class F010102Repository : RepositoryBase<F010102, Wms3plDbContext, F010102Repository>
    {
        /// <summary>
        /// 刪除採購單內的明細
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="shopNo"></param>		
        public void DeleteShopDetail(string dcCode, string gupCode, string custCode, string shopNo)
        {
            var paramList = new List<object>
            {
                dcCode,
                gupCode,
                custCode,
                shopNo
            };

            var sql = @"DELETE FROM F010102
						WHERE DC_CODE = @p0
						AND GUP_CODE = @p1
						AND CUST_CODE = @p2
						AND SHOP_NO = @p3";

            ExecuteSqlCommand(sql, paramList.ToArray());
        }
    }
}
