using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05080502RepositoryTest : BaseRepositoryTest
    {
        private F05080502Repository _f05080502Repo;
        public F05080502RepositoryTest()
        {
            _f05080502Repo = new F05080502Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF05080502Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030002";
            var calNo = "20190731111615";
            #endregion

            _f05080502Repo.GetF05080502Datas(dcCode, gupCode, custCode, calNo);
        }
    }
}
