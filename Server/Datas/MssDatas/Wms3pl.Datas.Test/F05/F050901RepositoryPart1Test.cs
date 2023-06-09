using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050901RepositoryPart1Test : BaseRepositoryTest
    {
        private F050901Repository _f050901Repo;
        public F050901RepositoryPart1Test()
        {
            _f050901Repo = new F050901Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF050901CSV()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "020003";
            var begCrtDate = new DateTime(2020, 2, 12);
            var endCrtDate = new DateTime(2020, 2, 12);
            #endregion
            _f050901Repo.GetF050901CSV(dcCode, gupCode, custCode, begCrtDate, endCrtDate);
        }
    }
}
