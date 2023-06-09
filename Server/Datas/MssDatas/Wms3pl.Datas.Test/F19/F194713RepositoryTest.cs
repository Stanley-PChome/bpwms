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
    public class F194713RepositoryTest: BaseRepositoryTest
    {
        private F194713Repository _f194713Repo;
        public F194713RepositoryTest()
        {
            _f194713Repo = new F194713Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void Get()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allId = "711";
            var eservice = "Shop";
            #endregion

            _f194713Repo.Get(dcCode, gupCode, custCode, allId, eservice);
        }

        [TestMethod]
        public void GetAllEServiceItem()
        {
            #region Params
            var dcCode = "001";
            #endregion

            _f194713Repo.GetAllEServiceItem();
        }
    }
}
