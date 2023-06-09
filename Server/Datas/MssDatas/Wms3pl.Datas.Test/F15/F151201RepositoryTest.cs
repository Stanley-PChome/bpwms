using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F15;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F15
{
    [TestClass]
    public class F151201RepositoryTest: BaseRepositoryTest
    {
        private F151201Repository _f151201Repo;
        public F151201RepositoryTest()
        {
            _f151201Repo = new F151201Repository(Schemas.CoreSchema);
        }
    }
}
