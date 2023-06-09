using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F194710Repository : RepositoryBase<F194710, Wms3plDbContext, F194710Repository>
    {
        public F194710Repository(string connName, WmsTransaction wmsTransaction = null)
      : base(connName, wmsTransaction)
        {
        }
    }
}
