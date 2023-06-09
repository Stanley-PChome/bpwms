using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910403Repository : RepositoryBase<F910403, Wms3plDbContext, F910403Repository>
    {
        /// <summary>
        /// 刪除不存在於指定的商品編號裡的耗材項目
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="quoteNo"></param>
        /// <param name="itemCodeList"></param>
        public void DeleteNotItemCodeList(string dcCode, string gupCode, string custCode, string quoteNo, IEnumerable<string> itemCodeList)
        {
            var paramList = new List<object>
            {
                dcCode, gupCode, custCode, quoteNo
            };

            int paramStartIndex = paramList.Count;
            var sqlNotIn = paramList.CombineSqlNotInParameters("ITEM_CODE", itemCodeList, ref paramStartIndex);

            var sql = @"DELETE FROM F910403
						WHERE DC_CODE = @p0
						AND GUP_CODE = @p1
						AND CUST_CODE = @p2
						AND QUOTE_NO = @p3
						AND " + sqlNotIn;

            ExecuteSqlCommand(sql, paramList.ToArray());
        }
    }
}
