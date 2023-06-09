using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F0070Repository : RepositoryBase<F0070, Wms3plDbContext, F0070Repository>
    {
        public void RemoveAllByGroupNameAndUserName(string groupName, string userName)
        {
            var parms = new List<SqlParameter>();
            parms.Add(new SqlParameter("@p0", groupName));
            parms.Add(new SqlParameter("@p1", userName));
            var sql = @" DELETE 
                     FROM F0070
                    WHERE GROUPNAME = @p0
                      AND USERNAME = @p1 ";
            ExecuteSqlCommand(sql, parms.ToArray());
        }


        public void InsertLoginLog(string mcCode, string accNo, string devCode)
        {
            //刪除登入紀錄
            DeleteLoginLog(devCode, mcCode);

            // 寫入登入紀錄
            var sql = @"INSERT INTO F0070 (CONNECTID, USERNAME, HOSTNAME, GROUPNAME, UNLOCKTIME, CRT_DATE, CRT_STAFF, CRT_NAME)
                        VALUES(@p0, @p1, @p2, 'Pda', CONVERT(DATETIME,@p3), CONVERT(DATETIME,@p4), @p5, @p6)";

            var paramers = new[] {
                new SqlParameter("@p0", mcCode),
                new SqlParameter("@p1", accNo),
                new SqlParameter("@p2", devCode),
                new SqlParameter("@p3", DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:dd")),
                new SqlParameter("@p4", DateTime.Now.ToString("yyyy-MM-dd HH:mm:dd")),
                new SqlParameter("@p5", Current.Staff),
                new SqlParameter("@p6", Current.StaffName)
            };

            ExecuteSqlCommand(sql, paramers);
        }

        #region 刪除登入紀錄
        public void DeleteLoginLog(string devCode, string mcCode, string accNo = null)
        {
            var param = new List<SqlParameter> {
                new SqlParameter("@p0", devCode)
            };
            var sql = @"DELETE F0070
                        WHERE HOSTNAME=@p0 AND GROUPNAME='Pda'";

           
            if (!string.IsNullOrWhiteSpace(mcCode))
            {
                sql += " AND CONNECTID = @p" + param.Count();
                param.Add(new SqlParameter("@p" + param.Count(), mcCode));
            };
            if (!string.IsNullOrWhiteSpace(accNo))
            {
                sql += " AND USERNAME = @p" + param.Count();
                param.Add(new SqlParameter("@p" + param.Count(), accNo));
            }
            ExecuteSqlCommand(sql, param.ToArray());
        }
        #endregion
    }
}

