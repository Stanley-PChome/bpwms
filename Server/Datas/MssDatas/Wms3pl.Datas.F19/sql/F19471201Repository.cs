using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F19471201Repository : RepositoryBase<F19471201, Wms3plDbContext, F19471201Repository>
    {
        public F19471201 GetData(string dcCode, string gupCode, string custCode, string allId, string channel, string consignType, string isUsed, List<string> exceptConsignNo)
        {
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter("@p0", dcCode));
            param.Add(new SqlParameter("@p1", gupCode));
            param.Add(new SqlParameter("@p2", custCode));
            param.Add(new SqlParameter("@p3", allId));
            param.Add(new SqlParameter("@p4", channel));
            param.Add(new SqlParameter("@p5", consignType));
            param.Add(new SqlParameter("@p6", isUsed));
            var sql = @" SELECT A.*
                     FROM F19471201 A
                     JOIN F194712 B
                       ON B.DC_CODE = A.DC_CODE
                      AND B.GUP_CODE = A.GUP_CODE
                      AND B.CUST_CODE = A.CUST_CODE
                      AND B.ALL_ID = A.ALL_ID
                      AND B.CHANNEL = A.CHANNEL
                      AND B.CUSTOMER_ID = A.CUSTOMER_ID
                      AND B.CONSIGN_TYPE = A.CONSIGN_TYPE
                      AND B.ISTEST = A.ISTEST
                    WHERE A.DC_CODE = @p0
                      AND A.GUP_CODE = @p1
                      AND A.CUST_CODE = @p2
                      AND A.ALL_ID = @p3
                      AND A.CHANNEL = @p4
                      AND A.CONSIGN_TYPE = @p5
                      AND A.ISUSED =@p6 ";
            if (exceptConsignNo.Any())
                sql += " AND A.CONSIGN_NO NOT IN ('" + string.Join("','", exceptConsignNo.ToArray()) + "') ";


#if DEBUG
            sql += " AND B.ISTEST = '1' ";
#else
			sql += " AND B.ISTEST='0' ";
#endif
            var result = SqlQuery<F19471201>(sql, param.ToArray()).FirstOrDefault();
            return result;
        }

        public void UpDataForIsused(string dcCode, string gupCode, string custCode, string channel, string allId = null, string consignType = null, List<string> exceptConsignNo = null)
        {
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter("@p0", Current.Staff));
            param.Add(new SqlParameter("@p1", Current.StaffName));
            param.Add(new SqlParameter("@p2", dcCode));
            param.Add(new SqlParameter("@p3", gupCode));
            param.Add(new SqlParameter("@p4", custCode));
            param.Add(new SqlParameter("@p5", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

            int paramCount = param.Count;

            string sql = @"UPDATE F19471201 
							  SET ISUSED = '0',
								  UPD_DATE = @p5,
								  UPD_STAFF = @p0,
								  UPD_NAME = @P1
							WHERE DC_CODE = @p2
							  AND GUP_CODE = @p3
                AND CUST_CODE = @p4
							  AND ISUSED = '1' ";

            if (!string.IsNullOrEmpty(allId))
            {
                paramCount++;
                param.Add(new SqlParameter(string.Format("@p{0}", paramCount), allId));
                sql += string.Format(" AND ALL_ID = @p{0} ", paramCount);
            }
            if (!string.IsNullOrEmpty(consignType))
            {
                paramCount++;
                param.Add(new SqlParameter(string.Format("@p{0}", paramCount), consignType));
                sql += string.Format(" AND CONSIGN_TYPE = @p{0} ", paramCount);
            }
            if (!string.IsNullOrEmpty(channel))
            {
                paramCount++;
                param.Add(new SqlParameter(string.Format("@p{0}", paramCount), channel));
                sql += string.Format(" AND CHANNEL = @p{0} ", paramCount);
            }

            if (exceptConsignNo.Any())
                sql += " AND CONSIGN_NO IN ('" + string.Join("','", exceptConsignNo.ToArray()) + "') ";
#if DEBUG
            sql += " AND ISTEST = '1' ";
#else
			sql += " AND ISTEST='0' ";
#endif
            ExecuteSqlCommand(sql, param.ToArray());
        }
    }
}
