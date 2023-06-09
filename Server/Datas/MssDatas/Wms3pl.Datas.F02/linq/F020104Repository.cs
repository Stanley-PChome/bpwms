using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
    public partial class F020104Repository : RepositoryBase<F020104, Wms3plDbContext, F020104Repository>
    {
        public F020104Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 取得進倉預排清單
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="dcCode"></param>
        /// <returns></returns>
		public IQueryable<F020104Detail> GetF020104Detail(string dcCode, DateTime beginDate, DateTime endDate
            , string pierCode, string area, string allowIn, string allowOut)
		{
            return  _db.F020104s
                .Where(P=>P.DC_CODE == dcCode || dcCode==null)
                .Where(P=>P.BEGIN_DATE.Date >= (beginDate).Date)
                .Where(P=>P.END_DATE.Date <= (endDate).Date)
                .Where(P=>P.PIER_CODE == pierCode || pierCode==null)
                .Where(P=>P.TEMP_AREA.ToString() == area || area==null)
                .Where(P=>P.ALLOW_IN == allowIn || allowIn==null)
                .Where(P=>P.ALLOW_OUT == allowOut || allowOut==null)
                .Select(x=>new F020104Detail{
                    BEGIN_DATE=x.BEGIN_DATE,
                    END_DATE=x.END_DATE,
                    DC_CODE=x.DC_CODE,
                    PIER_CODE=x.PIER_CODE,
                    TEMP_AREA=x.TEMP_AREA,
                    ALLOW_IN=x.ALLOW_IN,
                    ALLOW_OUT=x.ALLOW_OUT
                })
                .OrderBy(x => x.TEMP_AREA).ThenBy(x => x.PIER_CODE);
		}
    }
}
