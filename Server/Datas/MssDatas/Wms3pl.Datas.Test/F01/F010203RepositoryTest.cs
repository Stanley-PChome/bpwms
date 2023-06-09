using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F01;

namespace Wms3pl.Datas.Test.F01
{
    [TestClass]
    public class F010203RepositoryTest : BaseRepositoryTest
    {
        private F010203Repository _f010203Repo;
        public F010203RepositoryTest()
        {
            _f010203Repo = new F010203Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void DeleteByStockNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var stockNo = "A2018122500001";
            var stockType = "2";
            #endregion

            _f010203Repo.DeleteByStockNo(dcCode, gupCode, custCode, stockNo, stockType);
        }

        [TestMethod]
        public void GetPalletDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var rtNO = "201904180001";
            #endregion

            _f010203Repo.GetPalletDatas(dcCode, gupCode, custCode, rtNO);
        }
    }
}
