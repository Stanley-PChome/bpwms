using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050804RepositoryTest : BaseRepositoryTest
    {
        private F050804Repository _f05080401Repo;
        public F050804RepositoryTest()
        {
            _f05080401Repo = new F050804Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF050804()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var stickerNo = "A123";
            #endregion

            _f05080401Repo.GetF050804(dcCode, gupCode, custCode, stickerNo);
        }

      

        [TestMethod]
        public void GetF050804s()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "O2020092800001";
            #endregion

            _f05080401Repo.GetF050804s(dcCode, gupCode, custCode, wmsOrdNo);
        }
    }
}
