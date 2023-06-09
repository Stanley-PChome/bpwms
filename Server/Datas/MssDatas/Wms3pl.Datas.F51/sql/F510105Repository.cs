using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F51
{
	public partial class F510105Repository : RepositoryBase<F510105, Wms3plDbContext, F510105Repository>
	{
        public void UpdateProcFlag(string dcCode, string calDate, string procFlag)
        {
            var parameters = new List<object>
            {
                procFlag,
                Current.Staff,
                Current.StaffName,
                dcCode,
                calDate
            };

            var sql = @"
				UPDATE  F510105  SET PROC_FLAG= @p0,
                                     DIFF_QTY=(WMS_QTY+BOOKING_QTY-WCS_QTY),
                                     UPD_STAFF = @p1,
						             UPD_DATE = dbo.GetSysDate(),
						             UPD_NAME = @p2
				 Where DC_CODE = @p3
                     And CAL_DATE = @p4
                     And PROC_FLAG = '0' ";

            ExecuteSqlCommand(sql, parameters.ToArray());
        }
    }
}
