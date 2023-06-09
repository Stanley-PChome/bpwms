using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    public class F192401_IMPORTLOGRepositoryTest : BaseRepositoryTest
    {
        private F192401_IMPORTLOGRepository _f192401ImportLogRepo;
        public F192401_IMPORTLOGRepositoryTest()
        {
            _f192401ImportLogRepo = new F192401_IMPORTLOGRepository(Schemas.CoreSchema);
        }

    }
}
