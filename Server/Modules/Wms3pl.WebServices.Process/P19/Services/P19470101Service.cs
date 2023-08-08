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
    public partial class P19470101Service
    {
        private WmsTransaction _wmsTransaction;
        public P19470101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

        public IQueryable<F19470101Datas> GetF19470101Datas(string ALL_ID, string DC_CODE)
        {
            var F19470101 = new F19470101Repository(Schemas.CoreSchema);
            var result = F19470101.GetF19470101Datas(ALL_ID, DC_CODE);
            return result;
        }
    }
}
