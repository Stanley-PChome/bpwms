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
    public class F1951RepositoryTest : BaseRepositoryTest
    {
        private F1951Repository _f1951Repo;
        public F1951RepositoryTest()
        {
            _f1951Repo = new F1951Repository(Schemas.CoreSchema);
        }
    }
}
