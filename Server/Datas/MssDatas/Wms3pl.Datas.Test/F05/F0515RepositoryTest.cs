using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F0515RepositoryTest: BaseRepositoryTest
    {
        private F0515Repository _f0515Repo;
        public F0515RepositoryTest()
        {
            _f0515Repo = new F0515Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetP050112Batches()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var batchDateS = new DateTime(2017, 12, 31);
            var batchDateE = new DateTime(2020, 12, 31);
            var batchNo = "1";
            var pickStatus = "0";
            var putStatus = "0";
            #endregion

            _f0515Repo.GetP050112Batches(dcCode, gupCode, custCode, batchDateS, batchDateE, batchNo, pickStatus, putStatus);
        }

        [TestMethod]
        public void GetData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var batchNo = "2";
            #endregion

            _f0515Repo.GetData(dcCode, gupCode, custCode, batchNo);
        }


        [TestMethod]
        public void GetAGVHasWorkBatchByNotInBatchNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var batchNo = "2";
            #endregion

            _f0515Repo.GetAGVHasWorkBatchByNotInBatchNo(dcCode, gupCode, custCode, batchNo);
        }
    }
}