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
    public class F191902RepositoryTest: BaseRepositoryTest
    {
        private F191902Repository _f191902Repo;
        public F191902RepositoryTest()
        {
            _f191902Repo = new F191902Repository(Schemas.CoreSchema);
        }
    }
}
