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
    public class F194710RepositoryTest: BaseRepositoryTest
    {
        private F194710Repository _f194710Repo;
        public F194710RepositoryTest()
        {
            _f194710Repo = new F194710Repository(Schemas.CoreSchema);
        }
    }
}
