using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.F22
{
	public partial class F220102Repository : RepositoryBase<F220102, Wms3plDbContext, F220102Repository>
	{

        public void MergeInsert(string docId, string binCode, string taskCode, string podCode, string wbCode, string locCode)
        {
            var sql = @" MERGE INTO F220102 a
                        USING (
                          SELECT b.TASK_CODE
                            FROM F220101 a
                            LEFT join F220102 b
                            on b.TASK_CODE = a.TASK_CODE 
                            WHERE a.TASK_CODE =@p0
                        ) b ON ( a.TASK_CODE = b.TASK_CODE)
                        WHEN NOT MATCHED THEN
                        INSERT (DOC_NO,BIN_CODE,TASK_CODE,POD_CODE,WB_CODE,LOC_CODE,STATUS,CRT_DATE,CRT_STAFF,CRT_NAME)
                        VALUES(@p1,@p2,@p3,@p4,@p5,@p6,'0',dbo.GetSysDate(),@p7,@p8);
                        ";
            var parms = new object[] { taskCode, docId, binCode, taskCode, podCode, wbCode, locCode, Current.Staff, Current.StaffName };
            ExecuteSqlCommand(sql, parms);
        }
        public void InsertF220102Data(string dcCode, string gupCode, string custCode, string docNo)
        {
            var parms = new List<object> { dcCode, gupCode, custCode, docNo };
            string sql = $@"
                INSERT INTO F220102 (POD_CODE,
										 TASK_CODE,
										 BIN_CODE,
										 DOC_NO,
										 WB_CODE,
										 LOC_CODE,
										 STATUS,
										 CRT_DATE,
										 CRT_STAFF,
										 CRT_NAME)
					   SELECT SUBSTRING (BIN_CODE, 1, 6),
							  A.TASK_CODE,
							  A.BIN_CODE,
							  B.DOC_NO,
							  A.WB_CODE,
							  SUBSTRING (BIN_CODE, 3, 4) + SUBSTRING (BIN_CODE, 9, 5),
							  '0',
							  dbo.GetSysDate(),
							  '{Current.DefaultStaff}',
							  '{Current.DefaultStaffName}'
						 FROM F220101 A
							  INNER JOIN F2201 B
								 ON     A.DC_CODE = B.DC_CODE
									AND A.GUP_CODE = B.GUP_CODE
									AND A.CUST_CODE = B.CUST_CODE
									AND A.REQ_CODE = B.REQ_CODE
						WHERE     A.DC_CODE = @p0
							  AND A.GUP_CODE = @p1
							  AND A.CUST_CODE = @p2
							  AND B.DOC_NO = @p3
							  AND NOT EXISTS
									 (SELECT BIN_CODE
										FROM F220102
									   WHERE BIN_CODE = A.BIN_CODE AND DOC_NO = B.DOC_NO)
					";
            ExecuteSqlCommand(sql, parms.ToArray());
        }
    }
}
