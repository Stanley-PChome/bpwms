using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1954Repository : RepositoryBase<F1954, Wms3plDbContext, F1954Repository>
    {
        public IQueryable<F1954> GetDatasForPda(string account, string custCode, string dcCode)
        {
            var sql =
@"Select A.FUN_CODE,B.FUN_NAME,A.FUN_TYPE,B.FUN_DESC,A.CRT_STAFF,
        	       A.CRT_DATE,A.UPD_STAFF,A.UPD_DATE,A.STATUS,A.UPLOAD_DATE,
        	       A.DISABLE,A.CRT_NAME,A.UPD_NAME,A.MAIN_SHOW,A.SIDE_SHOW  
            From F1954 A
        	  Join F1954_I18N B
        	    On A.FUN_CODE=B.FUN_CODE
        	 Where A.FUN_CODE In (Select FUN_CODE From F192401 C,F195301 D,F192402 E
                               Where C.GRP_ID  = D.GRP_ID 
                                AND C.EMP_ID  = E.EMP_ID 
                                AND C.EMP_ID=@p0
                                 And E.CUST_CODE=@p1
                                 And E.DC_CODE=@p2)
             And A.FUN_CODE Like '08%'
             And A.FUN_CODE Like '%0000'
             And A.FUN_CODE Not Like '%000000'
             And A.FUN_CODE Not Like '%00000000'
        	   And B.LANG=@p3";

            var paramers = new[] {
                        new SqlParameter("@p0", account),
                        new SqlParameter("@p1", custCode),
                        new SqlParameter("@p2", dcCode),
                        new SqlParameter("@p3", Current.Lang)
                    };

            return SqlQuery<F1954>(sql, paramers);
        }


        /// <summary>
		/// 更新程式清單. 包含狀態, 檔案日期, 名稱.
		/// </summary>
		/// <param name="funCode"></param>
		/// <param name="funName"></param>
		/// <param name="uploadDate"></param>
		/// <param name="status"></param>
		/// <param name="userId"></param>
		public void Update(string funCode, string funName, DateTime uploadDate, string status, string userId)
        {
            var sql = @"UPDATE F1954 SET STATUS = @p1
										, FUN_NAME = @p2
										, FUN_DESC = @p2
										, UPLOAD_DATE = Convert(datetime,@p3)
										, UPD_STAFF = @p4
										, UPD_NAME = @p5
										, UPD_DATE = dbo.GetSysDate()
						WHERE FUN_CODE = @p0";
            var param = new[]{
                new SqlParameter("@p0", funCode),
                new SqlParameter("@p1", status),
                new SqlParameter("@p2", funName),
                new SqlParameter("@p3", uploadDate.ToString("yyyy/MM/dd HH:mm:ss")),
                new SqlParameter("@p4", userId),
                new SqlParameter("@p5", Current.StaffName)
            };

            ExecuteSqlCommand(sql, param);
        }

        /// <summary>
		/// 更新DISABLE. 不更新其它欄位.
		/// </summary>
		/// <param name="funCode"></param>
		/// <param name="status"></param>
		/// <param name="userId"></param>
		public void UpdateDisabled(string funCode, string status, string userId)
        {
            var sql = @"UPDATE F1954 
				SET DISABLE = @p1
					, UPD_STAFF = @p2
					, UPD_NAME = @p3
					, UPD_DATE = dbo.GetSysDate() 
				WHERE FUN_CODE = @p0";
            var param = new[]{
                new SqlParameter("@p0", funCode),
                new SqlParameter("@p1", status),
                new SqlParameter("@p2", userId),
                new SqlParameter("@p3", Current.StaffName)
            };

            ExecuteSqlCommand(sql, param);
        }
    }
}
