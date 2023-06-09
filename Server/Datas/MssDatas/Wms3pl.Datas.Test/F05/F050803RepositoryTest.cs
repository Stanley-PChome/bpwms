using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050803RepositoryTest : BaseRepositoryTest
    {
        private F050803Repository _f050803Repo;
        public F050803RepositoryTest()
        {
            _f050803Repo = new F050803Repository(Schemas.CoreSchema);
        }

       

       

        [TestMethod]
        public void InsertByF0515()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var batchNo = "Z123";
            #endregion

            _f050803Repo.InsertByF0515(dcCode, gupCode, custCode, batchNo);
        }
    }
}
