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
    public partial class F195402Repository : RepositoryBase<F195402, Wms3plDbContext, F195402Repository>
    {
        public F195402Repository(string connName, WmsTransaction wmsTransaction = null)
         : base(connName, wmsTransaction)
        {
        }
        public string GetNewMenuCode()
        {
            var maxMenuCode = _db.F195402s.AsNoTracking().Max(x=>x.MENU_CODE);
            var result = (Convert.ToInt32(maxMenuCode == null ? "000" : maxMenuCode) + 1).ToString().PadLeft(3, '0'); ;
            return result;
        }
    }
}
