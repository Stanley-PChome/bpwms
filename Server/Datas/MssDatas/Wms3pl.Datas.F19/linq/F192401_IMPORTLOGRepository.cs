﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F192401_IMPORTLOGRepository : RepositoryBase<F192401_IMPORTLOG, Wms3plDbContext, F192401_IMPORTLOGRepository>
    {
        public F192401_IMPORTLOGRepository(string connName, WmsTransaction wmsTransaction = null)
         : base(connName, wmsTransaction)
        {
        }
    }
}
