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
    public class F1983RepositoryTest: BaseRepositoryTest
    {
        private F1983Repository _f1983Repo;
        public F1983RepositoryTest()
        {
            _f1983Repo = new F1983Repository(Schemas.CoreSchema);
        }
    }
}
