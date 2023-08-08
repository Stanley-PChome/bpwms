using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F051802Repository : RepositoryBase<F051802, Wms3plDbContext, F051802Repository>
	{
		public void DeleteByRtnWmsNo(string dcCode, string convenientCode, string cellCode, string rtnWmsNo)
		{
			var parm = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", convenientCode),
								new SqlParameter("@p2", cellCode),
								new SqlParameter("@p3", rtnWmsNo)
						};

			var sql = @" DELETE F051802
										WHERE DC_CODE = @p0
										AND CONVENIENT_CODE =@p1
										AND CELL_CODE = @p2
										AND WMS_NO = @p3
                        ";

			ExecuteSqlCommand(sql, parm.ToArray());
		}

		public IQueryable<F051802> GetDataByNotRtnWmsNo(string dcCode, string gupCode, string custCode, string convenientCode, string cellCode, string excludeRtnWmsNo)
		{
			var sql = @" SELECT * FROM 
									 F051802
									WHERE DC_CODE = @p0  
									 AND GUP_CODE = @p1
									 AND CUST_CODE= @p2
									 AND CONVENIENT_CODE = @p3
									 AND CELL_CODE = @p4
									 AND WMS_NO <> @p5 ";
			var param = new object[] { dcCode, gupCode, custCode, convenientCode, cellCode, excludeRtnWmsNo };

			return SqlQuery<F051802>(sql, param);
		}

		public IQueryable<F051802> GetDetailData(string dcCode, string convenientCode, string vnrCode, string gupCode, string custCode)
		{
			var sql = @" SELECT ISNULL(B.CELL_CODE, A.CELL_CODE) CELL_CODE, B.WMS_NO
										FROM F051801 A
										LEFT JOIN F051802 B
										ON A.DC_CODE = B.DC_CODE
										AND A.CONVENIENT_CODE = B.CONVENIENT_CODE
										AND A.CELL_CODE = B.CELL_CODE
										WHERE A.DC_CODE = @p0
										AND A.CONVENIENT_CODE = @p1
										AND A.VNR_CODE = @p2
										AND A.GUP_CODE = @p3
										AND A.CUST_CODE = @p4 ";
			var param = new object[] { dcCode, convenientCode, vnrCode, gupCode, custCode };

			return SqlQuery<F051802>(sql, param);
		}
	}
}
