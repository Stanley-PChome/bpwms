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
    public partial class F1929Repository : RepositoryBase<F1929, Wms3plDbContext, F1929Repository>
    {
        public void UpdateName(string gupCode, string subName)
        {
      //      //var paramers = new List<SqlParameter>{
      //      //		new SqlParameter(":p0", subName),
      //      //		new SqlParameter(":p1", gupCode)
      //      //	};
      //      //var sb = new StringBuilder();
      //      //var sbp = new StringBuilder();
      //      //for (var i = 2; i < 10002; i++)
      //      //{
      //      //	sbp.Clear();
      //      //	sb.Append(":p").Append(i.ToString()).Append(",");
      //      //	paramers.Add(new SqlParameter(sbp.Append(":p").Append(i.ToString()).ToString(), gupCode));
      //      //}
      //      var paramers = new List<object>{
      //              subName,
      //              gupCode,
      //              Current.Staff,
      //              Current.StaffName
      //          };
      //      var sb = new StringBuilder();
      //      var sbp = new StringBuilder();
      //      for (var i = 2; i < 10002; i++)
      //      {
      //          sbp.Clear();
      //          sb.Append(":p").Append(i.ToString()).Append(",");
      //          paramers.Add(gupCode);
      //      }

      //      var ps = sb.ToString().TrimEnd(',');
      //      var sql = @"
						//Update F1929
						//Set GUP_NAME=Concat(GUP_NAME,:p0)
						//	, GUP_CODE=:p1
						//	, UPD_DATE =sysdate  
						//	, UPD_STAFF = :p2  
						//	, UPD_NAME = :p3  						
						//Where GUP_CODE In (" + ps + ")";

      //      ExecuteSqlCommand(sql, paramers.ToArray());
        }

        public void UpdateName2(string gupCode, string subName)
        {
       //     var paramers = new List<SqlParameter>{
       //             new SqlParameter(":p0", subName),
       //             new SqlParameter(":p1", gupCode),
       //             new SqlParameter(":p2", Current.Staff),
       //             new SqlParameter(":p3", Current.StaffName)
       //         };
       //     var sb = new StringBuilder();
       //     var sbp = new StringBuilder();
       //     for (var i = 2; i < 10002; i++)
       //     {
       //         sbp.Clear();
       //         sb.Append(":p").Append(i.ToString()).Append(",");
       //         paramers.Add(new SqlParameter(sbp.Append(":p").Append(i.ToString()).ToString(), gupCode));
       //     }

       //     var ps = sb.ToString().TrimEnd(',');
       //     var sql =
       //             @"Update F1929
							// Set GUP_NAME=Concat(GUP_NAME,:p0)
							//	, GUP_CODE=:p1
							//	, UPD_DATE =sysdate  
							//	, UPD_STAFF = :p2  
							//	, UPD_NAME = :p3    
						 //Where GUP_CODE In (" + ps + ")";

       //     ExecuteSqlCommand(sql, paramers.ToArray());
        }

        public void UpdateName3(string gupCode, string subName)
        {
            var paramers = new List<SqlParameter>{
                    new SqlParameter("@p0", subName),
                    new SqlParameter("@p1", gupCode),
                    new SqlParameter("@p2", Current.Staff),
                    new SqlParameter("@p3", Current.StaffName)
                };
            var sql =
                @"Update F1929
						 Set GUP_NAME=Concat(GUP_NAME,@p0)
								, UPD_DATE =dbo.GetSysDate()  
								, UPD_STAFF = @p2  
								, UPD_NAME = @p3 
					Where GUP_CODE=@p1";

            ExeSqlCmdCountMustGreaterZero(sql, "測試必須有執行數量!", paramers.ToArray());
        }
    }

    
}
