using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F51
{
	public partial class F510105Repository : RepositoryBase<F510105, Wms3plDbContext, F510105Repository>
	{
		public F510105Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public void BulkDelete(List<F510105> data)
		{
			SqlBulkDeleteForAnyCondition(data, "F510105", new List<string> { "ID" });
		}

		public IQueryable<F510105> GetDataByPending(string dcCode, string calDate)
		{
			return _db.F510105s.Where(x => 
			x.DC_CODE == dcCode &&
			x.CAL_DATE == calDate &&
			x.PROC_FLAG == "0");
		}
	}
}
