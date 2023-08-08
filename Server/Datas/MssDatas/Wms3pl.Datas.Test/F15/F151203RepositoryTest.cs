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
    public class F151203RepositoryTest: BaseRepositoryTest
    {
        private F151203Repository _f151203Repo;
        public F151203RepositoryTest()
        {
            _f151203Repo = new F151203Repository(Schemas.CoreSchema);
        }
    }
}
