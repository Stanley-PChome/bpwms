using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F0513RepositoryTest : BaseRepositoryTest
    {
        private F0513Repository _f0513Repo;
        public F0513RepositoryTest()
        {
            _f0513Repo = new F0513Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var delvDate = new DateTime(2017, 2, 20);
            #endregion

            _f0513Repo.GetData(dcCode, gupCode, custCode, delvDate);
        }

        [TestMethod]
        public void GetF0513WithF1909Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var delvDate = new DateTime(2017, 2, 20);
            string delvTime = "16:54";
            string status = "0";
            #endregion

            _f0513Repo.GetF0513WithF1909Datas(dcCode, gupCode, custCode, delvDate, delvTime, status);
        }

        [TestMethod]
        public void GetNearestPickTime()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            #endregion

            _f0513Repo.GetNearestPickTime(dcCode, gupCode, custCode);
        }

        [TestMethod]
        public void GetBatchDebitDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var notOrder = false;
            var isB2c = true;
            #endregion

            _f0513Repo.GetBatchDebitDatas(dcCode, gupCode, custCode, notOrder, isB2c);
        }

        [TestMethod]
        public void GetF0513Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var delvDate = "2017-02-23";
            #endregion

            _f0513Repo.GetF0513Datas(dcCode, gupCode, custCode, delvDate);
        }
    }
}
