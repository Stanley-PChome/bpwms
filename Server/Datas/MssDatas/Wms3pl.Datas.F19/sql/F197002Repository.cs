using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F197002Repository : RepositoryBase<F197002, Wms3plDbContext, F197002Repository>
    {
        public ExecuteResult UpdateF197002(string year, string labelType, string upNo)
        {
            string type = labelType == "1" ? " BOX_NO = @p0 " : " PALLET_NO =@p0 ";
            string sql = $@" UPDATE F197002 SET {type} , UPD_DATE = dbo.GetSysDate(),UPD_STAFF = @p1 ,UPD_NAME = @p2 WHERE YEAR = @p3 ";
            var sqlParamers = new List<SqlParameter>();
            sqlParamers.Add(new SqlParameter("@p0", upNo));
            sqlParamers.Add(new SqlParameter("@p1", upNo));
            sqlParamers.Add(new SqlParameter("@p2", upNo));
            sqlParamers.Add(new SqlParameter("@p3", year));
            ExecuteSqlCommand(sql, sqlParamers.ToArray());
            return new ExecuteResult { IsSuccessed = true };
        }
    }
}
