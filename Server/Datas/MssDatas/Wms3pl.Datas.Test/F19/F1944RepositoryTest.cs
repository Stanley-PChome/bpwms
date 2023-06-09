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
    public class F1944RepositoryTest: BaseRepositoryTest
    {
        private F1944Repository _f1944Repo;
        public F1944RepositoryTest()
        {
            _f1944Repo = new F1944Repository(Schemas.CoreSchema);
        }
    }
}
