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
    public class F194715RepositoryTest: BaseRepositoryTest
    {
        private F194715Repository _f194715Repo;
        public F194715RepositoryTest()
        {
            _f194715Repo = new F194715Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatasByAllId()
        {
            #region Params
            var allId = "TCAT";
            var customerId = "1265635401";
            #endregion

            _f194715Repo.GetSettings(allId, customerId);
        }

    }
}
