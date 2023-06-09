using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F70
{
	public partial class F700707Repository : RepositoryBase<F700707, Wms3plDbContext, F700707Repository>
	{
		public F700707Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

        /// <summary>
        /// 從驗收單取得物流中心進貨狀況
        /// </summary>
        /// <param name="receDate"></param>
        /// <returns></returns>
        public IQueryable<F700707ForSchedule> GetDcPurchaseQty(DateTime receDate)
        {
            return from a in _db.F020201s
                    where a.RECE_DATE == receDate
                    group a by new { a.DC_CODE, a.RECE_DATE, CNT_DAY = a.RECE_DATE.Value.Day - 1 } into a1
                    select new F700707ForSchedule
                    {
                        DC_CODE = a1.Key.DC_CODE,
                        CNT_DATE = a1.Key.RECE_DATE.Value,
                        CNT_DAY = (a1.Key.RECE_DATE.Value.Day - 1).ToString(),
                        QTY = a1.Sum(x => x.RECV_QTY.HasValue ? x.RECV_QTY.Value : 0)
                    };
        }
    }
}
