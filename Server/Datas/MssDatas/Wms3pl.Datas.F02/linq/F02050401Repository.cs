using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
  public partial class F02050401Repository : RepositoryBase<F02050401, Wms3plDbContext, F02050401Repository>
  {
    public F02050401Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
    {

    }


  }
}
