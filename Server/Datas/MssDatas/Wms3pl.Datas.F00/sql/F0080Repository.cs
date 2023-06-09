using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F0080Repository : RepositoryBase<F0080, Wms3plDbContext, F0080Repository>
    {
        public override void Add(F0080 entity, params string[] withoutColumns)
        {
            base.AddToBulkInsert(entity, null, withoutColumns);
        }
    }
}

