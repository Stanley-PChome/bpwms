using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F191301Repository : RepositoryBase<F191301, Wms3plDbContext, F191301Repository>
    {

        public ExecuteResult UpdateItemStock()
        {
            return new ExecuteResult(true);
        }
    }
}
