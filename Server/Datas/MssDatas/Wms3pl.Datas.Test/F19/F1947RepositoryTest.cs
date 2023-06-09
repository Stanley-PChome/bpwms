using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F1947RepositoryTest: BaseRepositoryTest
    {
        private F1947Repository _f1947Repo;
        public F1947RepositoryTest()
        {
            _f1947Repo = new F1947Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1947WithF194701Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var delvTime = "18:00";
            #endregion

            _f1947Repo.GetF1947WithF194701Datas( dcCode,  gupCode, custCode, delvTime);
        }

        [TestMethod]
        public void GetF1947ExQuery()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allID = "711";
            var allComp = "711超商取貨";
            #endregion

            _f1947Repo.GetF1947ExQuery(dcCode, gupCode, custCode, allID, allComp);
        }

        [TestMethod]
        public void GetDistributionData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            #endregion

            _f1947Repo.GetDistributionData(dcCode, gupCode, custCode);
        }

        [TestMethod]
        public void GetAllowedF1947s()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            #endregion

            _f1947Repo.GetAllowedF1947s(dcCode, gupCode, custCode);
        }

        [TestMethod]
        public void GetAllIdByWmsOrdNo()
        {
            #region Params
            var wmsOrdNo = "O2018050200025";
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            #endregion

            _f1947Repo.GetAllIdByWmsOrdNo(wmsOrdNo, gupCode, custCode, dcCode);
        }

        [TestMethod]
        public void GetF1947JoinF194701Datas()
        {
            #region Params
            var ALL_ID = "711";
            var DC_CODE = "001";
            #endregion

            _f1947Repo.GetF1947JoinF194701Datas(ALL_ID, DC_CODE);
        }
    }
}
