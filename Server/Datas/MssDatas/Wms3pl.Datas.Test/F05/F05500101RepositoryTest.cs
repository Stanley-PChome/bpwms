using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05500101RepositoryTest : BaseRepositoryTest
    {
        private F05500101Repository _f05500101Repo;
        public F05500101RepositoryTest()
        {
            _f05500101Repo = new F05500101Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetAllData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "A123,A456";
            #endregion

            _f05500101Repo.GetAllDataByShipPackageService(dcCode, gupCode, custCode, wmsOrdNo);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var stockNo = "test";
            #endregion

            _f05500101Repo.GetDatas(dcCode, gupCode, custCode, stockNo);
        }

        [TestMethod]
        public void GetF05500101Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var stockNo = "test";
            var itemCode = "test";
            #endregion

            _f05500101Repo.GetF05500101Datas(dcCode, gupCode, custCode, stockNo, itemCode);
        }

        [TestMethod]
        public void GetNextLogSeq()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030002";
            var wmsOrdNo = "O2019010700001";
            short packageBoxNo = 1;
            #endregion

            _f05500101Repo.GetNextLogSeq(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo);
        }
    }
}
