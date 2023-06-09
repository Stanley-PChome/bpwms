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
    public class F194706RepositoryTest: BaseRepositoryTest
    {
        private F194706Repository _f194706Repo;
        public F194706RepositoryTest()
        {
            _f194706Repo = new F194706Repository(Schemas.CoreSchema);
        }
    }
}
