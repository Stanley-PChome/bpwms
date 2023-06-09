using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F051502RepositoryTest : BaseRepositoryTest
    {
        private F051502Repository _f051502Repo;
        public F051502RepositoryTest()
        {
            _f051502Repo = new F051502Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void UpdateStatusByBatchNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var batchNo = "1";
            var status = "9";
            #endregion

            _f051502Repo.UpdateStatusByBatchNo(dcCode, gupCode, custCode, batchNo, status);
        }

       

        [TestMethod]
        public void GetPickReportDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var batchNo = "2";
            #endregion

            _f051502Repo.GetPickReportDatas(dcCode, gupCode, custCode, batchNo);
        }

       

        [TestMethod]
        public void GetPickDetail()
        {
            #region Params
            var dcNo = "001";
            var gupCode = "01";
            var custNo = "030001";
            var statusList = new List<string> { "0", "2" };
            var batchPickNo = "U123";
            #endregion

            _f051502Repo.GetPickDetail(dcNo, gupCode, custNo, statusList, batchPickNo);
        }
    }
}
