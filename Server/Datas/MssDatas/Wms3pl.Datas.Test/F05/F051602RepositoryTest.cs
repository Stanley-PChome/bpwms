using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F051602RepositoryTest : BaseRepositoryTest
    {
        private F051602Repository _f051602Repo;
        public F051602RepositoryTest()
        {
            _f051602Repo = new F051602Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void InsertF051602ByBatchNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var batchNo = "2";
            #endregion

            _f051602Repo.InsertF051602ByBatchNo(dcCode, gupCode, custCode, batchNo);
        }

        [TestMethod]
        public void GetPutReportDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var batchNo = "1";
            #endregion

            _f051602Repo.GetPutReportDatas(dcCode, gupCode, custCode, batchNo);
        }
    }
}
