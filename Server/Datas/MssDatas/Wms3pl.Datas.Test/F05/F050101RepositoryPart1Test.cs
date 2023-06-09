using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050101RepositoryPart1Test : BaseRepositoryTest
    {
        private F050101Repository _f050101Repo;
        public F050101RepositoryPart1Test()
        {
            _f050101Repo = new F050101Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF050101ByCustOrdNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var custordNos = "17021704390001I";
            #endregion

            _f050101Repo.GetF050101ByCustOrdNo(dcCode, gupCode, custCode, custordNos);
        }
    }
}
