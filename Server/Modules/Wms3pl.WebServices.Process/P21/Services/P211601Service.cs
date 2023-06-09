using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P21.Services
{
    public class P211601Service
    {
        private WmsTransaction _wmsTransaction;
        public P211601Service(WmsTransaction wmsTransaction)
        {
            _wmsTransaction = wmsTransaction;
        }

        //public IQueryable<F0090Base> GetF0090x(String DcCode, String QueryCount, Boolean IsSortDesc, String ExternalSystem, F0006 FunctionName, String SearchOrdNo, String ReturnMessage)
        //{

        //}
    }
}
