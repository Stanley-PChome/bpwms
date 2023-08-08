
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
    public partial class F1909Service
    {
        private WmsTransaction _wmsTransaction;
        public F1909Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        public IQueryable<F1909EX> GetF1909EXDatas()
        {
            var F1909Rep = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
            return F1909Rep.GetF1909Datas();           
        }

    }
}
