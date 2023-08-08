using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F051401Repository : RepositoryBase<F051401, Wms3plDbContext, F051401Repository>
	{
		public string LockF051401()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F051401';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public F051401 GetData(string wmsNo, string cellCode)
		{
			var sql = @"
									SELECT * FROM F051401 
									WHERE WMS_ORD_NO = @p0 
									AND CELL_CODE = @p1 
									AND (STATUS = 1 OR STATUS = 2) --已安排 或 已放入
                 ";
			return SqlQuery<F051401>(sql, new object[] { wmsNo, cellCode }).FirstOrDefault();
		}

		public void UpdateStatus(string status, string dcCode, string wmsNo)
		{
			var parameters = new List<object>
						{
								status,
								dcCode,
								wmsNo
						};

			var sql = @"
				UPDATE  F051401 SET STATUS= @p0 
				 Where DC_CODE = @p1 
					 And WMS_ORD_NO = @p2 ";
			ExecuteSqlCommand(sql, parameters.ToArray());
		}

        /// <summary>
        /// 縮小集貨格
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="collectionCode"></param>
        /// <param name="cellCode"></param>
        public void AdjustCellCount(string dcCode, string gupCode, string custCode, string collectionCode, string cellCode, string CellType)
        {
            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=gupCode},
                new SqlParameter("@p2",SqlDbType.VarChar){Value=custCode},
                new SqlParameter("@p3",SqlDbType.VarChar){Value=collectionCode},
                new SqlParameter("@p4",SqlDbType.VarChar){Value=cellCode},
                new SqlParameter("@p5",SqlDbType.VarChar){Value=CellType}
            };
            var sql = @"DELETE FROM F051401 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND COLLECTION_CODE=@p3 AND CELL_CODE>@p4 AND CELL_TYPE=@p5";
            ExecuteSqlCommand(sql, para.ToArray());
        }

    }
}
