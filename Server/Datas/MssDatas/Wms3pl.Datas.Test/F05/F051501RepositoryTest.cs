using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F051501RepositoryTest : BaseRepositoryTest
    {
        private F051501Repository _f051501Repo;
        public F051501RepositoryTest()
        {
            _f051501Repo = new F051501Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void UpdateStatusByBatcnNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var batchNo = "1";
            var status = "9";
            #endregion

            _f051501Repo.UpdateStatusByBatcnNo(dcCode, gupCode, custCode, batchNo, status);
        }

        

        [TestMethod]
        public void GetBatchPickStations()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var batchNo = "2";
            #endregion

            _f051501Repo.GetBatchPickStations(dcCode, gupCode, custCode, batchNo);
        }
    }
}
