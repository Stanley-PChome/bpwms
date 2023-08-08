﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F0093Repository : RepositoryBase<F0093, Wms3plDbContext, F0093Repository>
    {
        public F0093Repository(string connName, WmsTransaction wmsTransaction = null) 
            : base(connName, wmsTransaction)
        {

        }
    }
}
