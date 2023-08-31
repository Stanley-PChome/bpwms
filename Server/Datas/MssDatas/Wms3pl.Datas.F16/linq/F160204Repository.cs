using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using static Wms3pl.Datas.Shared.ApiEntities.VendorReturnReq;

namespace Wms3pl.Datas.F16
{
	public partial class F160204Repository : RepositoryBase<F160204, Wms3plDbContext, F160204Repository>
	{
		public F160204Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{

		}
	}
}
