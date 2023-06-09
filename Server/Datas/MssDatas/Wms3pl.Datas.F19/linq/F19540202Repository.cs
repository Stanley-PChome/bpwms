using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F19540202Repository : RepositoryBase<F19540202, Wms3plDbContext, F19540202Repository>
    {
        public F19540202Repository(string connName, WmsTransaction wmsTransaction = null)
         : base(connName, wmsTransaction)
        {
        }
    }
}
