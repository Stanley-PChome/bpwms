﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910207Repository : RepositoryBase<F910207, Wms3plDbContext, F910207Repository>
    {
        public F910207Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }
    }
}
