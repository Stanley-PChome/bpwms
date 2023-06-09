using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F000301Repository : RepositoryBase<F000301, Wms3plDbContext, F000301Repository>
    {
        public F000301Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F000301> GetDefaultSetting(string sysType)
        {
          var param = new List<SqlParameter>
          {
            new SqlParameter("@p0", sysType) {SqlDbType = SqlDbType.VarChar}
          };

          var sql = @"SELECT AP_NAME, DESCRIPT, SYS_PATH
                    FROM F000301
                    WHERE SYS_TYPE = @p0";

          var result = SqlQuery<F000301>(sql, param.ToArray());
          return result;
        }
    }
}
