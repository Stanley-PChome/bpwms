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
    public class F194703RepositoryTest: BaseRepositoryTest
    {
        private F194703Repository _f194703Repo;
        public F194703RepositoryTest()
        {
            _f194703Repo = new F194703Repository(Schemas.CoreSchema);
        }
    }
}
