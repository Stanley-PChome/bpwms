using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1915Repository : RepositoryBase<F1915, Wms3plDbContext, F1915Repository>
    {
        public F1915Repository(string connName, WmsTransaction wmsTransaction = null)
                : base(connName, wmsTransaction)
        {
        }

        public List<F1915> GetDatas(string gupCode, string aCode, List<string> custCodes)
        {
            var result = _db.F1915s.Where(x => x.GUP_CODE == gupCode
                                        && x.ACODE == aCode
                                        && custCodes.Contains(x.CUST_CODE));
            return result.ToList();
        }

        public IQueryable<F1915> GetDatasByACode(string gupCode, string custCode, List<string> aCode)
        {
            return _db.F1915s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
                                                        x.CUST_CODE == custCode &&
                                                        aCode.Contains(x.ACODE));
        }
    }
}
