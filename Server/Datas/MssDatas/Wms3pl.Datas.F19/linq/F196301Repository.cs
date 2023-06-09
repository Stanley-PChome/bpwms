using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F196301Repository : RepositoryBase<F196301, Wms3plDbContext, F196301Repository>
    {
        public F196301Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        /// <summary>
		/// 取得已設定的儲位
		/// </summary>
		/// <param name="workId"></param>
		/// <returns></returns>
		public IQueryable<F1912LocData> GetAllowedF1912LocDatas(string workId)
        {
            var f1912s = _db.F1912s.AsNoTracking();
           
            var f196301s = _db.F196301s.AsNoTracking().Where(x => x.WORK_ID == Convert.ToDecimal(workId));

            var result = from A in f1912s
                         join B in f196301s on new { A.DC_CODE, A.LOC_CODE } equals new { B.DC_CODE, B.LOC_CODE }
                         select new F1912LocData
                         {
                             DC_CODE = A.DC_CODE,
                             AREA_CODE = A.AREA_CODE,
                             LOC_CODE = A.LOC_CODE
                         };
            return result;
        }
    }
}
