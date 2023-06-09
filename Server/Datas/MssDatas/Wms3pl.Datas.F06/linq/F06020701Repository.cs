using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F06020701Repository : RepositoryBase<F06020701, Wms3plDbContext, F06020701Repository>
	{
		public F06020701Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<ContainerDetail> GetContainerDetails(long id)
		{
			var sql = @"SELECT B.DC_CODE,B.GUP_CODE,B.CUST_CODE,B.WMS_NO ALLOCATION_NO,A.ROWNUM ALLOCATION_SEQ,
                         C.PRE_TAR_WAREHOUSE_ID,A.SKUCODE ITEM_CODE,A.SKUQTY QTY,A.SERIALNUMLIST,
                         A.BINCODE BIN_CODE,A.COMPLETE_TIME,A.ISLASTCONTAINER
										FROM F06020701 A
										JOIN F060201 B
										ON  B.DOC_ID = A.ORDERCODE
										AND B.CMD_TYPE = '1'
										JOIN F151001 C
										ON C.DC_CODE = B.DC_CODE
										AND C.GUP_CODE = B.GUP_CODE
										AND C.CUST_CODE = B.CUST_CODE
										AND C.ALLOCATION_NO = B.WMS_NO
										WHERE A.F060207_ID= @p0 ";
			var parms = new List<SqlParameter>()
			{
				new SqlParameter("@p0",SqlDbType.BigInt){ Value = id }
			};
			return SqlQuery<ContainerDetail>(sql, parms.ToArray());
		}

	

	}
}
