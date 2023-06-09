using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1942Repository : RepositoryBase<F1942, Wms3plDbContext, F1942Repository>
    {
        /// <summary>
        /// 刪除資料，傳入架料代碼
        /// </summary>
        /// <param name="locTypeId"></param>
        public void Delete(string locTypeId)
        {
            string sql = @"
				DELETE F1942 WHERE LOC_TYPE_ID = @p0
			";

            var param = new[] {
                new SqlParameter("@p0", locTypeId)
            };

            ExecuteSqlCommand(sql, param);
        }
    }
}
