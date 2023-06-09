using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F191201Repository : RepositoryBase<F191201, Wms3plDbContext, F191201Repository>
	{
        public void DeletedF191201Datas(List<string> datas)
        {
            var param = new List<object>();
            int paramStartIndex = 0;
            var inSql = param.CombineSqlInParameters("(DC_CODE+TYPE+VALUE)", datas, ref paramStartIndex);
            string sql = string.Format("DELETE F191201 WHERE {0}", inSql);
            ExecuteSqlCommand(sql, param.ToArray());
        }
    }
}
