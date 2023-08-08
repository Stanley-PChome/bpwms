using System.Collections.Generic;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F050006Repository : RepositoryBase<F050006, Wms3plDbContext, F050006Repository>
    {
        public void DeletedF050006Datas(List<string> datas)
        {
            var param = new List<object>();
            int paramStartIndex = 0;
            var inSql = param.CombineSqlInParameters("(DC_CODE+GUP_CODE+CUST_CODE+ZIP_CODE)", datas, ref paramStartIndex);
            string sql = string.Format("DELETE F050006 WHERE {0}", inSql);
            ExecuteSqlCommand(sql, param.ToArray());
        }
    }
}
