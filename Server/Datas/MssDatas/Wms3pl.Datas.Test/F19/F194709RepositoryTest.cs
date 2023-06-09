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
    class F194709RepositoryTest: BaseRepositoryTest
    {
        private F194709Repository _f194709Repo;
        public F194709RepositoryTest()
        {
            _f194709Repo = new F194709Repository(Schemas.CoreSchema);
        }
    }
}
