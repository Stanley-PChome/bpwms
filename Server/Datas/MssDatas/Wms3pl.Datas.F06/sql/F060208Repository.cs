using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
    public partial class F060208Repository
    {
		public IQueryable<F060208> GetF060208ExistData(string dcCode,string gupCode,string custCode,string containerCode,string oriOrderCode,string positionCode)
		{
			string sql = @"SELECT  * FROM F060208 
							 WHERE DC_CODE =@p0
							 AND GUP_CODE  = @p1
							 AND CUST_CODE =@p2
							 AND CONTAINER_CODE =@p3
							 AND ORI_ORDER_CODE =@p4
							 AND POSITION_CODE =@p5";
			
			var param = new[] {
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", containerCode),
				new SqlParameter("@p4", oriOrderCode),
				new SqlParameter("@p5", positionCode)
			};
			var result = SqlQuery<F060208>(sql, param);
			return result;
		}
		

		public IQueryable<F060208> GetSameContainerStatus(string dcCode,string gupCode,string custCode,string containerCode,string oriOrderCode,int procFlag)
		{
			string sql = @"SELECT  * FROM F060208 
							 WHERE DC_CODE =@p0
							 AND GUP_CODE  = @p1
							 AND CUST_CODE =@p2
							 AND CONTAINER_CODE =@p3
							 AND ORI_ORDER_CODE =@p4
							 AND PROC_FLAG =@p5";

			var param = new[] {
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", containerCode),
				new SqlParameter("@p4", oriOrderCode),
				new SqlParameter("@p5", procFlag)
			};
			var result = SqlQuery<F060208>(sql, param);
			return result;
		}

        public bool CheckIsArrival(string dcCode, string gupCode, string custCode, string containerCode, string oriOrderCode)
        {
            string sql = @"SELECT  * FROM F060208 
							 WHERE DC_CODE =@p0
							 AND GUP_CODE  = @p1
							 AND CUST_CODE =@p2
							 AND CONTAINER_CODE =@p3
							 AND ORI_ORDER_CODE =@p4
							 AND PROC_FLAG IN (1,2) ";

            var param = new[] {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", containerCode),
                new SqlParameter("@p4", oriOrderCode)
            };
            var result = SqlQuery<F060208>(sql, param).Any();
            return result;
        }

        public int GetTop1BoxTotal(string dcCode, string gupCode, string custCode, string oriOrderCode, List<int> excludeProcFlags)
        {
            var insql = string.Empty;
            if (excludeProcFlags != null && excludeProcFlags.Any())
                insql = $" AND PROC_FLAG NOT IN ( {string.Join(",", excludeProcFlags)} ) ";

            string sql = $@"SELECT TOP 1 * FROM F060208 
							 WHERE DC_CODE =@p0
							 AND GUP_CODE  = @p1
							 AND CUST_CODE =@p2
							 AND ORI_ORDER_CODE =@p3
                             {insql}
							 AND POSITION_CODE = TARGET_POS_CODE
                             ORDER BY CRT_DATE DESC ";

            var param = new[] {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", oriOrderCode)
            };
            var result = SqlQuery<F060208>(sql, param).FirstOrDefault();
            return result != null ? result.BOX_TOTAL : 0;
        }

        public void UpdateProcFlag(string dcCode, string gupCode, string custCode, string oriOrderCode, int procFlag)
        {
            var parameters = new List<object>
            {
                procFlag,
                Current.Staff,
                Current.StaffName,
                dcCode,
                gupCode,
                custCode,
                oriOrderCode
            };

            string sql = $@" UPDATE F060208 
                            SET PROC_FLAG = @p0, UPD_DATE = dbo.GetSysDate(), UPD_STAFF=@p1, UPD_NAME=@p2
                            WHERE DC_CODE = @p3
                            AND GUP_CODE = @p4 
                            AND CUST_CODE = @p5
                            AND ORI_ORDER_CODE = @p6  
                            AND PROC_FLAG = 0
                            ";

            ExecuteSqlCommand(sql, parameters.ToArray());
        }

    public void UpdateProcFlag(string dcCode, string gupCode, string custCode, string oriOrderCode, int procFlag, List<int> excludeProcFlags = null)
    {
      var parameters = new List<SqlParameter>
      {
        new SqlParameter("@p0", procFlag) { SqlDbType = SqlDbType.Int },
        new SqlParameter("@p1", Current.Staff) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", Current.StaffName) { SqlDbType = SqlDbType.NVarChar },
        new SqlParameter("@p3", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p5", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p6", oriOrderCode) { SqlDbType = SqlDbType.VarChar },
      };
      
      string sql = $@" UPDATE F060208 
                         SET PROC_FLAG = @p0, UPD_DATE = dbo.GetSysDate(), UPD_STAFF=@p1, UPD_NAME=@p2
                       WHERE DC_CODE = @p3
                         AND GUP_CODE = @p4
                         AND CUST_CODE = @p5
                         AND ORI_ORDER_CODE = @p6  
                            ";
      sql += parameters.CombineSqlNotInParameters(" AND PROC_FLAG", excludeProcFlags, SqlDbType.Int);

      ExecuteSqlCommand(sql, parameters.ToArray());
    }

        public int GetDataCnt(string dcCode, string gupCode, string custCode, string oriOrderCode, List<int> excludeProcFlags)
        {
            var insql = string.Empty;
            if (excludeProcFlags != null && excludeProcFlags.Any())
                insql = $" AND PROC_FLAG NOT IN ( {string.Join(",", excludeProcFlags)} ) ";

            string sql = $@"SELECT 1 FROM F060208 
							 WHERE DC_CODE =@p0
							 AND GUP_CODE  = @p1
							 AND CUST_CODE =@p2
							 AND ORI_ORDER_CODE =@p3
                             {insql}
							 AND POSITION_CODE = TARGET_POS_CODE ";

            var param = new[] {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", oriOrderCode)
            };
            var result = SqlQuery<F060208>(sql, param);
            return result.Count();
        }
		public IQueryable<F060208> GetWorkStataionShipData(string dcCode, string workStationCode)
		{
			string sql = @"SELECT  * 
               FROM F060208 
							 WHERE DC_CODE =@p0
							 AND TARGET_POS_CODE =@p1
               AND PROC_FLAG IN('0','1')
							";

			var param = new[] {
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", workStationCode)
			};
			var result = SqlQuery<F060208>(sql, param);
			return result;
		}

		public IQueryable<F060208> GetDatasByWmsNoAndContainerCode(string dcCode, string gupCode, string custCode, string wmsNo, string containerCode)
		{
			var param = new List<SqlParameter> {
				new SqlParameter("@p0", SqlDbType.VarChar){ Value = dcCode },
				new SqlParameter("@p1", SqlDbType.VarChar){ Value = gupCode },
				new SqlParameter("@p2", SqlDbType.VarChar){ Value = custCode },
				new SqlParameter("@p3", SqlDbType.VarChar){ Value = wmsNo },
			};

			string sql = @"SELECT  * 
                FROM F060208 
							 WHERE DC_CODE =@p0
							 AND GUP_CODE  = @p1
							 AND CUST_CODE =@p2
							 AND ORI_ORDER_CODE =@p3
							 AND PROC_FLAG not in('9') ";
			if (!string.IsNullOrEmpty(containerCode))
			{
				sql += " AND CONTAINER_CODE =@p" + param.Count;
				param.Add(new SqlParameter("@p" + param.Count, SqlDbType.VarChar) { Value = containerCode });
			}
			return SqlQuery<F060208>(sql, param.ToArray());
		}

		public void UpdateProcFlag(string dcCode, string gupCode, string custCode, string wmsNo, string containerCode)
		{
			var param = new List<SqlParameter> {
				new SqlParameter("@p0",SqlDbType.Int){ Value = 9},
				new SqlParameter("@p1",SqlDbType.DateTime2){ Value = DateTime.Now},
				new SqlParameter("@p2",SqlDbType.VarChar){Value = Current.Staff},
				new SqlParameter("@p3",SqlDbType.NVarChar){Value = Current.StaffName},
				new SqlParameter("@p4", SqlDbType.VarChar){ Value = dcCode },
				new SqlParameter("@p5", SqlDbType.VarChar){ Value = gupCode },
				new SqlParameter("@p6", SqlDbType.VarChar){ Value = custCode },
				new SqlParameter("@p7", SqlDbType.VarChar){ Value = wmsNo },

			};

			string sql = @" UPDATE F060208
               SET PROC_FLAG=@p0, UPD_DATE = @p1, UPD_STAFF = @p2,UPD_NAME=@p3
							 WHERE DC_CODE =@p4
							 AND GUP_CODE  = @p5
							 AND CUST_CODE =@p6
							 AND ORI_ORDER_CODE =@p7
							 AND PROC_FLAG NOT IN ('9') ";
			if (!string.IsNullOrEmpty(containerCode))
			{
				sql += " AND CONTAINER_CODE =@p" + param.Count;
				param.Add(new SqlParameter("@p" + param.Count, SqlDbType.VarChar) { Value = containerCode });
			}
			ExecuteSqlCommand(sql, param.ToArray());
		}
	}
}
