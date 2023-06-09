using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
	public partial class F020302Repository : RepositoryBase<F020302, Wms3plDbContext, F020302Repository>
	{
		public F020302Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public List<string> GetDatasBySns(string dcCode, string gupCode, string custCode, string status, List<string> serialNos)
		{
			return _db.F020302s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.STATUS == status &&
			serialNos.Contains(x.SERIAL_NO)).Select(x => x.SERIAL_NO).ToList();
		}

		public IQueryable<F020302> GetDatasByFileNames(string dcCode, string gupCode, string custCode, List<string> fileNames)
		{
			return _db.F020302s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			fileNames.Contains(x.FILE_NAME));
		}

        public IQueryable<F020302> GetDatasBySns(string dcCode, string gupCode, string custCode, List<string> serialNos)
        {
            return _db.F020302s.AsNoTracking().Where(x =>
            x.DC_CODE == dcCode &&
            x.GUP_CODE == gupCode &&
            x.CUST_CODE == custCode &&
            serialNos.Contains(x.SERIAL_NO));
        }
    }
}
