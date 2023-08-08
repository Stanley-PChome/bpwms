using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
    public partial class P190181Service
    {
        private WmsTransaction _wmsTransaction;
        public P190181Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }


    }
}
