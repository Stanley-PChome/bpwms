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
    public class F19700201RepositoryTest: BaseRepositoryTest
    {
        private F19700201Repository _f19700201Repo;
        public F19700201RepositoryTest()
        {
            _f19700201Repo = new F19700201Repository(Schemas.CoreSchema);
        }
    }
}
