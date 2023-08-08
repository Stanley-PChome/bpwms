using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
	public partial class F910501Repository : RepositoryBase<F910501, Wms3plDbContext, F910501Repository>
	{
		public void Update(string dcCode, string deviceIp, string noSpecReports, string closeByBoxNo)
		{
			var param = new object[]
			{
				noSpecReports,
				closeByBoxNo,
				DateTime.Now,
				Current.DefaultStaff,
				Current.DefaultStaffName,
				dcCode,
				deviceIp
			};
			var sql = @"UPDATE F910501 
						SET NO_SPEC_REPROTS = @p0,
						CLOSE_BY_BOXNO = @p1,
						UPD_DATE = @p2,
						UPD_STAFF = @p3,
						UPD_NAME = @p4
						WHERE DC_CODE = @p5
						AND DEVICE_IP = @p6";
			ExecuteSqlCommand(sql, param.ToArray());
		}

		public void Update910501(DeviceData data)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",System.Data.SqlDbType.NVarChar){ Value = (object)data.LABELING??DBNull.Value },
				new SqlParameter("@p1",System.Data.SqlDbType.NVarChar){ Value = (object)data.PRINTER??DBNull.Value},
				new SqlParameter("@p2",System.Data.SqlDbType.NVarChar){ Value = (object)data.MATRIX_PRINTER??DBNull.Value},
				new SqlParameter("@p3",System.Data.SqlDbType.VarChar){ Value = (object)data.WORKSTATION_CODE??DBNull.Value},
				new SqlParameter("@p4",System.Data.SqlDbType.VarChar){ Value = (object)data.WORKSTATION_TYPE??DBNull.Value},
				new SqlParameter("@p5",System.Data.SqlDbType.Char){ Value =(object)data.WORKSTATION_GROUP??DBNull.Value},
				new SqlParameter("@p6",System.Data.SqlDbType.VarChar){ Value = data.DC_CODE},
				new SqlParameter("@p7",System.Data.SqlDbType.VarChar){ Value = data.DEVICE_IP},
			};

			var sql = @"UPDATE F910501 SET 
							LABELING = @p0,
							PRINTER = @p1,
							MATRIX_PRINTER = @p2,
							VIDEO_ERROR = '1',
							WEIGHING_ERROR = '1',
							NOTRUNAGV = '1',
							NOTCAPS = '1',
							WORKSTATION_CODE = @p3,
							WORKSTATION_TYPE = @p4,
							WORKSTATION_GROUP = @p5,
							NO_SPEC_REPROTS = '0',
							CLOSE_BY_BOXNO = '0'
							WHERE DC_CODE = @p6
							AND DEVICE_IP = @p7";

			ExecuteSqlCommandWithSqlParameterSetDbType(sql, param.ToArray());
		}
	}
}
