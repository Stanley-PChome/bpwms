using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
    public partial class F05080501Repository : RepositoryBase<F05080501, Wms3plDbContext, F05080501Repository>
    {
        public F05080501Repository(string connName, WmsTransaction wmsTransaction = null)
             : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F05080501Data> GetF05080501Datas(string dcCode, string gupCode, string custCode, string calNo)
        {
            var f05080501Data = _db.F05080501s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                         x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode &&
                                                                         x.CAL_NO == calNo);

            var vwF000904LangData = _db.VW_F000904_LANGs.AsNoTracking().Where(x => x.TOPIC == "F05080501" &&
                                                                                   x.SUBTOPIC == "RESULT_CODE" &&
                                                                                   x.LANG == Current.Lang &&
                                                                                   f05080501Data.Select(z => z.RESULT_CODE).Distinct().Contains(x.VALUE));

            var result = (from A in f05080501Data
                          join B in vwF000904LangData
                          on A.RESULT_CODE equals B.VALUE
                          select new F05080501Data
                          {
                              DC_CODE = A.DC_CODE,
                              GUP_CODE = A.GUP_CODE,
                              CUST_CODE = A.CUST_CODE,
                              CAL_NO = A.CAL_NO,
                              ORD_NO = A.ORD_NO,
                              RESULT_CODE = A.RESULT_CODE,
                              RESULT_NAME = B.NAME
                          }).OrderBy(x => x.ORD_NO).ToList();

            // RowNum
            for (int i = 0; i < result.Count; i++){ result[i].ROWNUM = i + 1; }

            return result.AsQueryable();
        }
    }
}
