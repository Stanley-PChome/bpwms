using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F195301_IMPORTLOGRepositoryTest: BaseRepositoryTest
    {
        private F195301_IMPORTLOGRepository _f1952ImportlogRepo;
        public F195301_IMPORTLOGRepositoryTest()
        {
            _f1952ImportlogRepo = new F195301_IMPORTLOGRepository(Schemas.CoreSchema);
        }
    }
}
