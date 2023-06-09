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
    public class F194714RepositoryTest: BaseRepositoryTest
    {
        private F194714Repository _f194714Repo;
        public F194714RepositoryTest()
        {
            _f194714Repo = new F194714Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetAll()
        {
            #region Params
            #endregion

            _f194714Repo.GetAll();
        }

        [TestMethod]
        public void GetDatasByAllId()
        {
            #region Params
            var allId = "TCAT";
            #endregion

            _f194714Repo.GetDatasByAllId(allId);
        }
    }
}
