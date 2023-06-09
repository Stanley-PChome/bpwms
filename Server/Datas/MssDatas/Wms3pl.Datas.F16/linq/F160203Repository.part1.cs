using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
	public partial class F160203Repository : RepositoryBase<F160203, Wms3plDbContext, F160203Repository>
	{
		public F160203Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

        public List<F160203> GetALLF160203()
        {
            return _db.F160203s.AsNoTracking().ToList();
        }
    }
}
