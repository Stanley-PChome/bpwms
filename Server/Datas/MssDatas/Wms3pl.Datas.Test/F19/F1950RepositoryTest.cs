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
    public class F1950RepositoryTest: BaseRepositoryTest
    {
        private F1950Repository _f1950Repo;
        public F1950RepositoryTest()
        {
            _f1950Repo = new F1950Repository(Schemas.CoreSchema);
        }
    }
}
