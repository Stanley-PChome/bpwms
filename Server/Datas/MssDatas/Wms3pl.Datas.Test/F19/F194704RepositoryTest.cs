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
    public class F194704RepositoryTest: BaseRepositoryTest
    {
        private F194704Repository _f194704Repo;
        public F194704RepositoryTest()
        {
            _f194704Repo = new F194704Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF194701WithF1934s()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            #endregion

            _f194704Repo.GetF194704Datas(dcCode, gupCode, custCode);

        }
    }
}
