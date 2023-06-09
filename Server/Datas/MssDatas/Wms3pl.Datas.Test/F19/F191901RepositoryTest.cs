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
    public class F191901RepositoryTest: BaseRepositoryTest
    {
        private F191901Repository _f191901Repo;
        public F191901RepositoryTest()
        {
            _f191901Repo = new F191901Repository(Schemas.CoreSchema);
        }
    }
}
