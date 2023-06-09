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
    public class F1981RepositoryTest: BaseRepositoryTest
    {
        private F1981Repository _f1981Repo;
        public F1981RepositoryTest()
        {
            _f1981Repo = new F1981Repository(Schemas.CoreSchema);
        }
    }
}
