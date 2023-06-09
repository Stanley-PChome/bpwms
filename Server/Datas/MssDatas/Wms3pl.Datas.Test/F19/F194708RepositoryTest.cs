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
    public class F194708RepositoryTest: BaseRepositoryTest
    {
        private F194708Repository _f194708Repo;
        public F194708RepositoryTest()
        {
            _f194708Repo = new F194708Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetZipCode()
        {
            #region Params
            var dcCode = "001";
            var allId = "TCAT";
            var accAreaId = 1;
            #endregion

            _f194708Repo.GetZipCode(dcCode, allId, accAreaId);

        }
    }
}
