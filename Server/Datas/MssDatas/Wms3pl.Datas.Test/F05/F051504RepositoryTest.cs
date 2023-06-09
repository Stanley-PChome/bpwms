using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F051504RepositoryTest : BaseRepositoryTest
    {
        private F051504Repository _f051504Repo;
        public F051504RepositoryTest()
        {
            _f051504Repo = new F051504Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void UpdateStatusByBatchNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var batchNo = "A123";
            var status = "9";
            #endregion

            _f051504Repo.UpdateStatusByBatchNo(dcCode, gupCode, custCode, batchNo, status);
        }
    }
}
