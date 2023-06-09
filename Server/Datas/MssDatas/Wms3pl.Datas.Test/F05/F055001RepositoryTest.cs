using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F055001RepositoryTest : BaseRepositoryTest
    {
        private F055001Repository _f055001Repo;
        public F055001RepositoryTest()
        {
            _f055001Repo = new F055001Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            var wmsOrdNo = "O2017022000003";
            var gupCode = "01";
            var custCode = "010001";
            var dcCode = "001";
            #endregion

            _f055001Repo.GetDatas(wmsOrdNo, gupCode, custCode, dcCode);
        }

        [TestMethod]
        public void GetF055001DatasByWmsOrdNos()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNos = new List<string> { "O2017032000088" };
            #endregion

            _f055001Repo.GetF055001DatasByWmsOrdNos(dcCode, gupCode, custCode, wmsOrdNos);
        }

        [TestMethod]
        public void GetNewPackageBoxNo()
        {
            #region Params
            var wmsOrdNo = "O2017032000088";
            var gupCode = "01";
            var custCode = "010001";
            var dcCode = "001";
            #endregion

            _f055001Repo.GetNewPackageBoxNo(wmsOrdNo, gupCode, custCode, dcCode);
        }

        [TestMethod]
        public void GetNewPackageBoxNos()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var dcCode = "001";
            var wmsOrdNos = new List<string> { "O2017022000013", "O2017022000015" };
            #endregion

            _f055001Repo.GetNewPackageBoxNos(gupCode, custCode, dcCode, wmsOrdNos);
        }

        [TestMethod]
        public void FindSelfF055001ByNoPrint()
        {
            #region Params
            var wmsOrdNo = "O2017032000088";
            var gupCode = "01";
            var custCode = "010001";
            var dcCode = "001";
            #endregion

            _f055001Repo.FindSelfF055001ByNoPrint(wmsOrdNo, gupCode, custCode, dcCode);
        }

        [TestMethod]
        public void GetCountByNotStatus()
        {
            #region Params
            var wmsOrdNo = "O2017032900097";
            var gupCode = "01";
            var custCode = "010001";
            var dcCode = "001";
            var status = "0";
            #endregion

            _f055001Repo.GetCountByNotStatus(wmsOrdNo, gupCode, custCode, dcCode, status);
        }

        [TestMethod]
        public void GetConsignData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "O2017030100035";
            var packageBoxNo = "1";
            #endregion

            _f055001Repo.GetConsignData(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo);
        }

        [TestMethod]
        public void GetConsignItemData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "O2017022100155";
            var pastNo = "905157958946";
            #endregion

            _f055001Repo.GetConsignItemData(dcCode, gupCode, custCode, wmsOrdNo, pastNo);
        }

        [TestMethod]
        public void UpdateData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var consignNo = "10179199";
            var orgOrdCode = "O2017032000060";
            #endregion

            _f055001Repo.UpdateData(dcCode, gupCode, custCode, consignNo, orgOrdCode);
        }

        [TestMethod]
        public void GetDatas2()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNos = new List<string> { "O2017022000001", "O2017022000002" };
            #endregion

            _f055001Repo.GetDatas(dcCode, gupCode, custCode, wmsOrdNos);
        }

    }
}
