using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05010102RepositoryTest : BaseRepositoryTest
    {
        private F05010102Repository _f05010102Repo;
        public F05010102RepositoryTest()
        {
            _f05010102Repo = new F05010102Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF05010102sByWmsOrdNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "O2017022000001";
            #endregion

            _f05010102Repo.GetF05010102sByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo);
        }

        [TestMethod]
        public void GetF05010102SerialNoData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var ordNo = "17021710240001R";
            #endregion

            _f05010102Repo.GetF05010102SerialNoData(dcCode, gupCode, custCode, ordNo);
        }

        [TestMethod]
        public void BulkDelete()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var custOrdNos = new List<string> { "B123" };
            #endregion

            _f05010102Repo.BulkDelete(dcCode, gupCode, custCode, custOrdNos);
        }
    }
}
