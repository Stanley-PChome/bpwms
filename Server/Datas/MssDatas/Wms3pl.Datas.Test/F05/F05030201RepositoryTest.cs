using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05030201RepositoryTest : BaseRepositoryTest
    {
        private F05030201Repository _f05030201Repo;
        public F05030201RepositoryTest()
        {
            _f05030201Repo = new F05030201Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetShippingReportByBomItem()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNos = new List<string> { "O2017022000013", "O2017022000012" };
            #endregion

            _f05030201Repo.GetShippingReportByBomItem(dcCode, gupCode, custCode, wmsOrdNos);
        }

        [TestMethod]
        public void GetDeliveryReportByBomItem()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "O2017022000013";
            short? packageBoxNo = null;
            #endregion

            _f05030201Repo.GetDeliveryReportByBomItem(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo);
        }

        [TestMethod]
        public void GetDatasByWmsOrdNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNos = new List<string> {  };
            #endregion

            _f05030201Repo.GetDatasByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNos);
        }
    }
}
