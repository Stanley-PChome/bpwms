﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F00.Interfaces;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F009002Repository : RepositoryBase<F009002, Wms3plDbContext, F009002Repository>, IApiLogRepository<F009002>
	{
        public F009002Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }
    }
}

