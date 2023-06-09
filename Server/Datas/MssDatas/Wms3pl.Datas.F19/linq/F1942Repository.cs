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
        public F1942Repository(string connName, WmsTransaction wmsTransaction = null)
       : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 傳入LOC_TYPE_ID, 回傳第一筆資料
        /// </summary>
        /// <param name="locTypeId"></param>
        /// <returns></returns>
        public F1942 GetByLocTypeId(string locTypeId)
        {
            var result = _db.F1942s.Where(x => x.LOC_TYPE_ID == locTypeId).FirstOrDefault();

            return result;
        }
    }
}
