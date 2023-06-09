using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1917Repository : RepositoryBase<F1917, Wms3plDbContext, F1917Repository>
    {
        public F1917Repository(string connName, WmsTransaction wmsTransaction = null)
              : base(connName, wmsTransaction)
        {
        }

        public List<F1917> GetDatas(string gupCode, string aCode, string bCode, string cCode, List<string> custCodes)
        {
            var result = _db.F1917s.Where(x => x.GUP_CODE == gupCode
                                        && x.ACODE == aCode
                                        && x.BCODE == bCode
                                        && x.CCODE == cCode);
            if (custCodes.Count() > 0)
            {
                result = result.Where(x => custCodes.Contains(x.CUST_CODE));
            }
            return result.ToList();

        }

        public IQueryable<F1917> GetDatasByCCode(string gupCode, string custCode, List<string> cCodes)
        {
            return _db.F1917s.Where(x => x.GUP_CODE == gupCode
                                        && x.CUST_CODE == custCode
                                        && cCodes.Contains(x.CCODE));
        }

        public IQueryable<F1917> GetDatasBySCategory(string gupCode, string custCode, List<PostItemCategorySCategorysModel> sCategorys)
        {
            return _db.F1917s.Where(x => x.GUP_CODE == gupCode
                                        && x.CUST_CODE == custCode
                                        && sCategorys.Any(z => x.ACODE == z.LCode &&
                                                               x.BCODE == z.MCode &&
                                                               x.CCODE == z.SCode));
        }
    }
}
