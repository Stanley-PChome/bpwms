using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060601Repository
	{
		public long GetF060601NextId()
		{
			var sql = @"SELECT NEXT VALUE FOR SEQ_F060601_ID";

			return SqlQuery<long>(sql).Single();
		}

        public void CancelRepeatData(string dcCode, string calDate)
        {
            var parameters = new List<object> {
                Current.Staff,
                Current.StaffName,
                dcCode,
                calDate,
                dcCode,
                calDate
            };

            var sql = @"  UPDATE F060601 
                          SET PROC_FLAG = '9', 
                          UPD_DATE =dbo.GetSysDate(), 
                          UPD_STAFF = @p0, 
                          UPD_NAME = @p1
                          WHERE ID IN (
                          SELECT ID FROM F060601 Z
                          WHERE Z.DC_CODE = @p2
                          AND Z.CAL_DATE = @p3
                          AND Z.PROC_FLAG = '0'
                          AND EXISTS (
                          SELECT * FROM (
                          SELECT CURRENT_PAGE,MAX(ID) MAX_ID 
                          FROM F060601 
                          WHERE DC_CODE = @p4
                          AND CAL_DATE = @p5
                          AND PROC_FLAG = '0'
                          GROUP BY CURRENT_PAGE) X
                          WHERE X.CURRENT_PAGE = Z.CURRENT_PAGE
                          AND X.MAX_ID <> Z.ID)
                          )
                          ";

            ExecuteSqlCommand(sql, parameters.ToArray());
        }
    }
}
