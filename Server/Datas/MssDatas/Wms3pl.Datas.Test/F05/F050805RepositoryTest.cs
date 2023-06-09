using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050805RepositoryTest : BaseRepositoryTest
    {
        private F050805Repository _f050805Repo;
        public F050805RepositoryTest()
        {
            _f050805Repo = new F050805Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF050805Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030002";
            var calNo = "20190731112030";
            #endregion

            _f050805Repo.GetF050805Datas(dcCode, gupCode, custCode, calNo);
        }
    }
}
