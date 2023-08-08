using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System.Data.SqlClient;
using Wms3pl.Datas.Shared.Entities;
using System.Data;

namespace Wms3pl.Datas.F19
{
    public partial class F1924Repository : RepositoryBase<F1924, Wms3plDbContext, F1924Repository>
    {
        #region P1905 權限功能
        /// <summary>
		/// 刪除F1924, 只標記ISDELETED='1'
		/// </summary>
		/// <param name="empId"></param>
		/// <param name="userId"></param>
		public void Delete(string empId, string userId)
        {
            var sql = @"
				UPDATE F1924 SET ISDELETED = '1'
				, UPD_STAFF = @p1
				, UPD_NAME = @p2
				, UPD_DATE = @p3
				WHERE EMP_ID = @p0
			";
            var param = new[] {
                new SqlParameter("@p0", empId),
                new SqlParameter("@p1", userId),
                new SqlParameter("@p2", Current.StaffName),
                new SqlParameter("@p3", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };
            ExecuteSqlCommand(sql, param);
        }
        #endregion

        #region P1906 使用者安全控管設定
        /// <summary>
        /// 傳回F1924 + F1954 + F195301 List
        /// </summary>
        /// <returns></returns>
        public IQueryable<EmpWithFuncionName> EmpWithFuncionName(string EmpId)
        {
            string sql = @"
      SELECT  DISTINCT ROW_NUMBER()OVER(ORDER BY TMP.FUN_CODE,TMP.EMP_ID) AS ROW_NUM,TMP.* 
        FROM (SELECT
        E.EMP_ID,
        D.FUN_CODE as FUN_CODE,
        D.FUN_NAME as FUN_NAME,
                     D.FUN_NAME AS BTNAME,CASE WHEN ISNULL( E.BT_CODE,'*') <> '*'  THEN 1  ELSE 0  END BTOPT 
                FROM (
 SELECT A.FUN_CODE,A.FUN_NAME,BT.BT_CODE,BT.BT_NAME
 FROM 
 (
   SELECT * FROM F1954
   WHERE SUBSTRING(FUN_CODE,1,1) <>'B' 
 ) A
 LEFT JOIN 
 (
   SELECT FUN_CODE BT_CODE,FUN_NAME BT_NAME
   FROM F1954
   WHERE SUBSTRING(FUN_CODE,1,1) = 'B'
 ) BT ON
 A.FUN_CODE = CONCAT(SUBSTRING(BT_CODE,2,10) , '0')
 ) D
                LEFT JOIN
                      (
                       SELECT DISTINCT FUN.EMP_ID,FUN.FUN_CODE,BT.BT_CODE 
                         FROM (SELECT A.EMP_ID,C.FUN_CODE
                                 FROM F1924 A  LEFT JOIN  F192401 B 
                                   ON A.EMP_ID = B.EMP_ID
                                 LEFT JOIN (SELECT * FROM F195301 WHERE SUBSTRING(FUN_CODE,1,1) <> 'B') C
                                   ON B.GRP_ID=C.GRP_ID 
                                WHERE A.EMP_ID=@p0) FUN 
                         LEFT JOIN
                              (SELECT A.EMP_ID,C.FUN_CODE as BT_CODE
                                 FROM F1924 A  LEFT JOIN  F192401 B 
                                   ON A.EMP_ID = B.EMP_ID
                                 LEFT JOIN (SELECT * FROM F195301 WHERE SUBSTRING(FUN_CODE,1,1) = 'B') C
                                   ON B.GRP_ID=C.GRP_ID 
                                WHERE A.EMP_ID=@p0) BT 
                           ON FUN.FUN_CODE=CONCAT(SUBSTRING(BT.BT_CODE,2,10)  , '0')) E 
                  ON (D.FUN_CODE= E.FUN_CODE AND D.FUN_CODE= E.BT_CODE))  TMP  
       INNER JOIN
             (SELECT x.* 
								FROM F195301 x 
								LEFT JOIN F192401 y 
									ON x.GRP_ID = y.GRP_ID 
							 WHERE y.EMP_ID = @p0) F 
          ON TMP.FUN_CODE = F.FUN_CODE
			 WHERE SUBSTRING(TMP.FUN_CODE,-6,6) <> '000000'
    ORDER BY TMP.FUN_CODE,TMP.EMP_ID";

            var param = new[] {
                new SqlParameter("@p0", EmpId)
            };
            var result = SqlQuery<EmpWithFuncionName>(sql, param).AsQueryable();

            return result;
        }
		#endregion

		#region 人員資訊同步
		public IQueryable<F1924> GetF1924ByEmpId(string empId)
		{
			string sql = @"SELECT * FROM F1924 WHERE EMP_ID = @p0";
			var param = new List<object> { empId };
			var result = SqlQuery<F1924>(sql, param.ToArray());
			return result;
		}

		public void updateF1924(string empId,string empName,string isDelected,string depId)
		{
			var sql = @"UPDATE F1924 SET EMP_NAME = @p1,
						ISDELETED = @p2,
						DEP_ID = @p3,
						UPD_DATE = @p4,
						UPD_STAFF = @p5,
						UPD_NAME = @p6
						WHERE EMP_ID =@p0";

			var paramers = new[]
			{
				new SqlParameter("@p0", empId),
				new SqlParameter("@p1", empName),
				new SqlParameter("@p2", isDelected),
				new SqlParameter("@p3", depId),
				new SqlParameter("@p4", DateTime.Now),
				new SqlParameter("@p5", Current.Staff),
				new SqlParameter("@p6", Current.StaffName),
			};

			ExecuteSqlCommand(sql, paramers);
		}
		#endregion

		public IQueryable<F1924Data> GetF1924DataByAccount(string account)
		{
			var param = new List<SqlParameter> {
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = account}

		};
			var sql = @"SELECT * FROM F1924 WHERE EMP_ID = @p0";
			return SqlQuery<F1924Data>(sql, param.ToArray());
		}

		public string GetEmpName(string empId)
		{
			var parm = new List<SqlParameter>
			{
				new SqlParameter("@p0",empId){SqlDbType = SqlDbType.VarChar}
			};
			var sql = @" SELECT TOP (1) EMP_NAME
                     FROM F1924 
                    WHERE EMP_ID = @p0 ";
			var empName = SqlQuery<string>(sql, empId).FirstOrDefault();
			if (string.IsNullOrEmpty(empName))
				empName = "支援人員";

			return empName;
		}

    public IQueryable<F1924> GetDatasForEmpIds(List<string> empIds)
    {
      var para = new List<SqlParameter>();

      var sql = @"SELECT * FROM F1924 WHERE ";
      sql += para.CombineSqlInParameters(" EMP_ID", empIds, SqlDbType.VarChar);
      return SqlQuery<F1924>(sql, para.ToArray());

      #region 原LINQ語法
      /*
      return _db.F1924s.AsNoTracking().Where(x => empIds.Contains(x.EMP_ID) && x.ISDELETED == "0");
      */
      #endregion
    }

  }
}
