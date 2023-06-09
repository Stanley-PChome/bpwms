using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
    public partial class F1934Service
    {
        private WmsTransaction _wmsTransaction;
        public F1934Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        public IQueryable<F1934EX> GetF1934EXDatas()
        {
            var Repository = new F1934Repository(Schemas.CoreSchema, _wmsTransaction);
            return Repository.GetF1934EXDatas();        
        }

    }
}
