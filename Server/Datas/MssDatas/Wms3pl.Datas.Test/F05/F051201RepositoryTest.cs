using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F051201RepositoryTest : BaseRepositoryTest
    {
        private F051201Repository _f051201Repo;
        public F051201RepositoryTest()
        {
            _f051201Repo = new F051201Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF051201Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var delvDate = new DateTime(2017, 2, 20);
            var isPrinted = "1";
            var ordType = "1";
            #endregion

            _f051201Repo.GetF051201Datas(dcCode, gupCode, custCode, delvDate, isPrinted, ordType);
        }

        [TestMethod]
        public void GetF051201SelectedDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010002";
            var delvDate = "2019-04-19";
            var pickTime = "18:16";
            var ordType = "0";
            var isPrinted = "0";
            var isDevicePrint = "0";
            #endregion

            _f051201Repo.GetF051201SelectedDatas(dcCode, gupCode, custCode, delvDate, pickTime, ordType, isPrinted, isDevicePrint);
        }

        [TestMethod]
        public void GetF051201ReportDataAs()
        {
            #region Params
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010002";
            DateTime delvDate = new DateTime(2017, 4, 5);
            string pickTime = "11:39";
            string pickOrdNo = "P2017040500003";
            bool showValidDate = true;
            string ordType = "0";
            #endregion

            _f051201Repo.GetF051201ReportDataAs(dcCode, gupCode, custCode,
            delvDate, pickTime, pickOrdNo, showValidDate, ordType);
        }

        [TestMethod]
        public void GetF051201ReportDataBs()
        {
            #region Params
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010002";
            string delvDate = "2017-04-05";
            string pickTime = "11:39";
            string pickOrdNo = "P2017040500003";
            string ordType = "0";
            #endregion

            _f051201Repo.GetF051201ReportDataBs(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, ordType);
        }

        [TestMethod]
        public void GetOrderProcessProgress()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var pickTime = "11:05";
            var delvDate = new DateTime(2017, 2, 20);
            #endregion

            _f051201Repo.GetOrderProcessProgress(dcCode, gupCode, custCode, pickTime, delvDate);
        }

        [TestMethod]
        public void GetDatasByOrdNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var ordNo = new string[] { "S2017030900053", "S2017030900054" };
            #endregion

            _f051201Repo.GetDatasByOrdNo(dcCode, gupCode, custCode, ordNo);
        }

        [TestMethod]
        public void GetExceedPickFinishTimeDatas()
        {
            #region Params
            var selectDate = new DateTime(2019, 5, 1);
            #endregion

            _f051201Repo.GetExceedPickFinishTimeDatas(selectDate);
        }

        [TestMethod]
        public void GetDatasByNoVirturlItem()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var delvDate = new DateTime(2017, 2, 20);
            var pickTime = "11:05";
            #endregion

            _f051201Repo.GetDatasByNoVirturlItem(dcCode, gupCode, custCode, delvDate, pickTime);
        }

        [TestMethod]
        public void GetPickTimes()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var ordType = "0";
            var delvDate = new DateTime(2017, 2, 21);
            #endregion

            _f051201Repo.GetPickTimes(dcCode, gupCode, custCode, ordType, delvDate);
        }

        [TestMethod]
        public void GetPickOrderNos()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var ordType = "0";
            var delvDate = new DateTime(2017, 2, 21);
            var pickTime = "14:49";
            #endregion

            _f051201Repo.GetPickOrderNos(dcCode, gupCode, custCode, ordType, delvDate, pickTime);
        }

        [TestMethod]
        public void GetWmsOrderNos()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var ordType = "0";
            var delvDate = new DateTime(2017, 2, 21);
            var pickTime = "14:49";
            #endregion

            _f051201Repo.GetWmsOrderNos(dcCode, gupCode, custCode, ordType, delvDate, pickTime);
        }

        [TestMethod]
        public void GetSummaryReport()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var ordType = "0";
            var delvDate = new DateTime(2017, 2, 21);
            var pickOrdNo = "P2017022100014";
            var wmsOrdNo = "O2017022100150";
            #endregion

            _f051201Repo.GetSummaryReport(dcCode, gupCode, custCode, ordType, delvDate, pickOrdNo, wmsOrdNo);
        }

        [TestMethod]
        public void GetF051201_2()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var pickOrdNo = "P2017030900020";
            #endregion

            _f051201Repo.GetF051201(dcCode, gupCode, custCode, pickOrdNo);
        }

        [TestMethod]
        public void UpdateF051201ByResendTask()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wbCode = "Test";
            var pickOrdNo = "P2017051600003";
            #endregion

            _f051201Repo.UpdateF051201ByResendTask(dcCode, gupCode, custCode, wbCode, pickOrdNo);
        }

        

        [TestMethod]
        public void GetP050112PickDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var delvDateS = new DateTime(2017, 1, 1);
            var delvDateE = new DateTime(2020, 12, 31);
            var pickTool = "1";
            var areaCode = "A24";
            #endregion

            _f051201Repo.GetP050112PickDatas(dcCode, gupCode, custCode, delvDateS, delvDateE, pickTool, areaCode);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var pickOrdNos = new List<string> { "P2017032300001", "P2017032400025" };
            #endregion

            _f051201Repo.GetDatas(dcCode, gupCode, custCode, pickOrdNos);
        }

        [TestMethod]
        public void GetP050112PickSummaries()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var pickOrdNos = new List<string> { "P2019042600001" };
            #endregion

            _f051201Repo.GetP050112PickSummaries(dcCode, gupCode, custCode, pickOrdNos);
        }

        [TestMethod]
        public void GetP050112PickSummaryDetails()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var pickOrdNos = new List<string> { "P2017030300003", "P2017030300004" };
            #endregion

            _f051201Repo.GetP050112PickSummaryDetails(dcCode, gupCode, custCode, pickOrdNos);
        }

        [TestMethod]
        public void GetP050112PickSummaryRetails()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var pickOrdNos = new List<string> { "P2017031700001", "P2017033000039" };
            #endregion

            _f051201Repo.GetP050112PickSummaryRetails(dcCode, gupCode, custCode, pickOrdNos);
        }

        [TestMethod]
        public void UpdateF051201PickStatus()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var pickOrdNos = new List<string> { "P2017041200007", "P2017041200008" };
            var pickStatus = "9";
            #endregion

            _f051201Repo.UpdateF051201PickStatus(dcCode, gupCode, custCode, pickOrdNos, pickStatus);
        }

        [TestMethod]
        public void GetPdaBatchPick()
        {
            #region Params
            var mode = "01";
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var wmsNo = "P2019041900001";
            var shipDate = Convert.ToDateTime("2019-04-19");
			      var empId = "wms";
            #endregion

            _f051201Repo.GetPdaBatchPick(mode, dcCode, gupCode, custCode, wmsNo, shipDate, empId);
        }
				[TestMethod]
				public void GetPdaSinglePick()
				{
					#region Params
					var mode = "01";
					var dcCode = "001";
					var gupCode = "01";
					var custCode = "030001";
					var wmsNo = "P2019041900001";
					var shipDate = Convert.ToDateTime("2019-04-19");
					var empId = "wms";
					#endregion

					_f051201Repo.GetPdaSinglePick(mode, dcCode, gupCode, custCode, wmsNo, shipDate, empId);
				}
	}
}
