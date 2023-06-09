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
    public partial class F1916Repository : RepositoryBase<F1916, Wms3plDbContext, F1916Repository>
    {
        public F1916Repository(string connName, WmsTransaction wmsTransaction = null)
               : base(connName, wmsTransaction)
        {
        }

        public List<F1916> GetDatas(string gupCode, string aCode, string bCode, List<string> custCodes)
        {
            var result = _db.F1916s.Where(x => x.GUP_CODE == gupCode
                                        && x.ACODE == aCode
                                        && x.BCODE == bCode
                                        && custCodes.Contains(x.CUST_CODE));
            return result.ToList();
        }

        public IQueryable<F1916> GetDatasByBCode(string gupCode, string custCode, List<string> bCodes)
        {
            return _db.F1916s.Where(x => x.GUP_CODE == gupCode
                                        && x.CUST_CODE == custCode
                                        && bCodes.Contains(x.BCODE));
        }

        public IQueryable<F1916> GetDatasByMCategory(string gupCode, string custCode, List<PostItemCategoryMCategorysModel> mCategorys)
        {
            return _db.F1916s.Where(x => x.GUP_CODE == gupCode
                                        && x.CUST_CODE == custCode
                                        && mCategorys.Any(z => x.ACODE == z.LCode &&
                                                               x.BCODE == z.MCode));
        }
    }
}
