using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910302Repository : RepositoryBase<F910302, Wms3plDbContext, F910302Repository>
    {
        public F910302Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 取得包含所有動作的附約資料，主要要取得委外商與動作金額。
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="processIds"></param>
        /// <returns></returns>
        public IQueryable<F910302WithF1928> GetF910302ByProcessIds(string dcCode, string gupCode, DateTime enableDate, DateTime disableDate, IEnumerable<string> processIds)
        {
            var q = from a in _db.F910301s
                    join b in _db.F910302s on new { a.CONTRACT_NO, a.DC_CODE, a.GUP_CODE }
                    equals new { b.CONTRACT_NO, b.DC_CODE, b.GUP_CODE }
                    join c in _db.F1928s on a.UNI_FORM equals c.UNI_FORM
                    where c.STATUS != "9"
                    && a.OBJECT_TYPE == "1"
                    && (a.DC_CODE == dcCode || a.DC_CODE == "000")
                    && a.GUP_CODE == gupCode
                    && b.ENABLE_DATE <= enableDate
                    && b.DISABLE_DATE >= disableDate
                    && processIds.Contains(b.PROCESS_ID)
                    select new F910302WithF1928
                    {
                        CONTRACT_TYPE = b.CONTRACT_TYPE,
                        PROCESS_ID = b.PROCESS_ID,
                        OUTSOURCE_ID = c.OUTSOURCE_ID,
                        APPROVE_PRICE = b.APPROVE_PRICE,
                        TASK_PRICE = b.TASK_PRICE,
                        OUTSOURCE_NAME = c.OUTSOURCE_NAME
                    };

            return q;
        }
    }
}
