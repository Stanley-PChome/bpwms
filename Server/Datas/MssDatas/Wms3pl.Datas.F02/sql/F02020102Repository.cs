
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.F02
{
	public partial class F02020102Repository : RepositoryBase<F02020102, Wms3plDbContext, F02020102Repository>
	{
		public void Delete(string dcCode,string gupCode,string custCode,string purchaseNo,string rtNo)
		{
			var sql = @" DELETE FROM F02020102
                   WHERE DC_CODE = @p0
                     AND GUP_CODE = @p1
                     AND CUST_CODE = @p2
                     AND PURCHASE_NO = @p3
                     AND RT_NO = @p4 ";
			ExecuteSqlCommand(sql, new object[] { dcCode, gupCode, custCode, purchaseNo, rtNo });
		}
	}
}
