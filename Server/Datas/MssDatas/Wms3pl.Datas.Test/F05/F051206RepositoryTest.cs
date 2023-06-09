using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F051206RepositoryTest : BaseRepositoryTest
    {
        private F051206Repository _f051206Repo;
        public F051206RepositoryTest()
        {
            _f051206Repo = new F051206Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetGetF051206PicksByQuery()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var delvDateStart = new DateTime(2018, 5, 18);
			var delvDateEnd = new DateTime(2018, 5, 18);
			//var pickTime = "15:26";
            var status = "0";
            var pickOrdNo = "P2018051800003";
            #endregion

            //_f051206Repo.GetGetF051206PicksByQuery(dcCode, gupCode, custCode, delvDateStart, delvDateEnd, status, pickOrdNo);
        }

        [TestMethod]
        public void GetGetF051206PicksByAdd()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var delvDate = new DateTime(2018, 5, 18);
            var pickTime = "15:26";
            var pickOrdNo = "P2018051800003";
            #endregion

            _f051206Repo.GetGetF051206PicksByAdd(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo);
        }

        [TestMethod]
        public void GetF051206AllocationLists()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var editType = "ADD2";
            var status = "0";
            var allocation_no = "T2018081400022";
            #endregion

            _f051206Repo.GetF051206AllocationLists(dcCode, gupCode, custCode, editType, status, allocation_no);
        }

        [TestMethod]
        public void GetF051206LackLists()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var pickOrdNo = "P2018051800003";
            var wmsOrdNo = "O2018051800005";
            var editType = "ADD2";
            #endregion

            _f051206Repo.GetF051206LackLists(dcCode, gupCode, custCode, pickOrdNo, wmsOrdNo, editType);
        }

        [TestMethod]
        public void GetF051206LackLists_Allot()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "T2018081400022";
            var editType = "Test";
            var status = "0";
            #endregion

            _f051206Repo.GetF051206LackLists_Allot(dcCode, gupCode, custCode, allocationNo, editType, status);
        }

        [TestMethod]
        public void GetExistsNoApproveData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var pickOrdNo = "P2018051800003";
            var pickOrdSeq = "0003";
            #endregion

            _f051206Repo.GetExistsNoApproveData(dcCode, gupCode, custCode, pickOrdNo, pickOrdSeq);
        }

        [TestMethod]
        public void GetApproveHasLackDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010002";
            var pickOrdNo = "P2019042500001";
            var pickOrdSeq = "0001";
            #endregion

            _f051206Repo.GetApproveHasLackDatas(dcCode, gupCode, custCode, pickOrdNo, pickOrdSeq);
        }

        [TestMethod]
        public void GetP060202Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var pickSDate = new DateTime(2017, 12, 31);
            var pickEDate = new DateTime(2020, 12, 31);
            var warehouseId = "";
            var itemCode = "JD17110001";
            #endregion

            _f051206Repo.GetP060202Datas(dcCode, gupCode, custCode, pickSDate, pickEDate, warehouseId, itemCode);
        }

        [TestMethod]
        public void GetP060202TransferDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var delvDates = new List<DateTime> { new DateTime(2018, 5, 18), new DateTime(2018, 5, 19) };
            var locCodes = new List<string> { "10H170101", "10H170102" };
            var itemCodes = new List<string> { "JD17110001", "JD17110002" };
            #endregion

            _f051206Repo.GetP060202TransferDatas(dcCode, gupCode, custCode, delvDates, locCodes, itemCodes);
        }

        [TestMethod]
        public void UpdateTransferByLackSeq()
        {
            #region Params
            var allocationNo = "A123";
            var allocationSeq = 1;
            var LackSeqs = new List<decimal> { 13, 15 };
            #endregion

            _f051206Repo.UpdateTransferByLackSeq(allocationNo, allocationSeq, LackSeqs);
        }

    }
}
