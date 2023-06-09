using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F055007Repository : RepositoryBase<F055007, Wms3plDbContext, F055007Repository>
	{
		public F055007Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public void DeleteByPackageBoxNo(string dcCode, string gupCode, string custCode, string wmsOrdNo, Int16 packageBoxNo)
		{
			var param = new List<SqlParameter>
												{
																new SqlParameter("@p0", dcCode),
																new SqlParameter("@p1", gupCode),
																new SqlParameter("@p2", custCode),
																new SqlParameter("@p3", wmsOrdNo),
																new SqlParameter("@p4", packageBoxNo)
												};

			string sql = @"
			            	DELETE
                            FROM F055007 
                            WHERE DC_CODE = @p0
                            AND GUP_CODE = @p1
                            AND CUST_CODE = @p2
                            AND WMS_ORD_NO = @p3
                            AND PACKAGE_BOX_NO = @p4
			            ";

			ExecuteSqlCommand(sql, param.ToArray());
		}
		public void UpdateShipReportList(string WmsOrdNo, int ReportSeq, int PackageBoxNo, string IsPrinted, DateTime? PrintTime)
		{
			var para = new List<SqlParameter>
			{
				new SqlParameter("@p0", WmsOrdNo) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p1", ReportSeq) { SqlDbType = System.Data.SqlDbType.Int },
				new SqlParameter("@p2", PackageBoxNo) { SqlDbType = System.Data.SqlDbType.SmallInt },
				new SqlParameter("@p3", IsPrinted) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p4", PrintTime) { SqlDbType = System.Data.SqlDbType.DateTime2 },
				new SqlParameter("@p5", Current.Staff) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p6", Current.StaffName) { SqlDbType = System.Data.SqlDbType.NVarChar },
			};
			var sql = @"UPDATE F055007 with(rowlock) SET ISPRINTED=@p3, PRINT_TIME=@p4,UPD_DATE=dbo.GetSysDate(),UPD_STAFF=@p5,UPD_NAME=@p6 WHERE WMS_ORD_NO=@p0 AND REPORT_SEQ=@p1 AND PACKAGE_BOX_NO=@p2 AND (ISPRINTED='0' OR ISPRINTED IS NULL)";
			ExecuteSqlCommand(sql, para.ToArray());
		}

    public IQueryable<F055007> GetDataByWmsOrCustOrdNo(string dcCode, string gupCode, string custCode, string orderNo)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode){ SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode){ SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2", custCode){ SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p3", orderNo){ SqlDbType = System.Data.SqlDbType.VarChar }
      };

      var sql = @"SELECT * FROM F055007 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND (WMS_ORD_NO=@p3 OR CUST_ORD_NO=@p3);";

      return SqlQuery<F055007>(sql,param.ToArray());
    }
	}
}
