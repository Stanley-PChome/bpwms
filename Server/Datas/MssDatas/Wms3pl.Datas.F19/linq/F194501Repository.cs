using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F194501Repository : RepositoryBase<F194501, Wms3plDbContext, F194501Repository>
	{
		public F194501Repository(string connName, WmsTransaction wmsTransaction = null)
	  : base(connName, wmsTransaction)
		{
		}

        public IQueryable<F194501> GetF194501ByDcCode(String dcCode)
        {
            return _db.F194501s.AsNoTracking().Where(x => x.DC_CODE == dcCode);
        }
	}
}
