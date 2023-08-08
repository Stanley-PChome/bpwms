using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F25
{
    public partial class F250105Repository : RepositoryBase<F250105, Wms3plDbContext, F250105Repository>
    {
        public void UpdateF250105ExtendData(List<string> listSerialNo, string gupCode, string custCode,
           string userId, string userName)
        {
            var parameters = new List<SqlParameter>
      {
        new SqlParameter("@p0", userId),
        new SqlParameter("@p1", userName),
        new SqlParameter("@p2", gupCode),
        new SqlParameter("@p3", custCode),
        new SqlParameter("@p4", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };
            var sql = @"
                        UPDATE F250105
                           SET STATUS = '1',
                               UPD_STAFF = @p0,
                               UPD_NAME = @p1,
                               UPD_DATE = @p4
                         WHERE     GUP_CODE = @p2
                               AND CUST_CODE = @p3
                               AND ISPASS = '1'
                               AND STATUS = '0'
                               AND SERIAL_NO IN ({0})
                        ";
            var serialNos = string.Format("'{0}'", string.Join("','", listSerialNo));
            ExecuteSqlCommand(string.Format(sql, serialNos), parameters.ToArray());
        }

        public void DeleteF250105(string gupCode, string custCode, string clientIp, string userId, string userName)
        {
            var parameters = new List<SqlParameter>
      {
        new SqlParameter("@p0", userId),
        new SqlParameter("@p1", userName),
        new SqlParameter("@p2", gupCode),
        new SqlParameter("@p3", custCode),
        new SqlParameter("@p4", clientIp),
        new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };

            var sql = @"
                        UPDATE F250105
                           SET STATUS = '9',
                               UPD_STAFF = @p0,
                               UPD_NAME = @p1,
                               UPD_DATE = @p5
                         WHERE     GUP_CODE = @p2
                               AND CUST_CODE = @p3
                               AND STATUS = '0'
                               AND CLIENT_IP = @p4
                        ";
            ExecuteSqlCommand(sql, parameters.ToArray());
        }
    }
}
