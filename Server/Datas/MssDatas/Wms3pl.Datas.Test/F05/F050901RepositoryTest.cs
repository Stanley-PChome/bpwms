using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050901RepositoryTest : BaseRepositoryTest
    {
        private F050901Repository _f050901Repo;
        public F050901RepositoryTest()
        {
            _f050901Repo = new F050901Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void BulkInsert()
        {
            #region Params
            DateTime now = DateTime.Now;

            var entities = (new List<F050901>
            {
                new F050901
                {
                    CONSIGN_ID = 26938,
                    CONSIGN_NO = "905073307999",
                    WMS_NO = "O2020092900002",
                    ROUTE_CODE = "A105",
                    GUP_CODE = "01",
                    CUST_CODE = "010001",
                    DC_CODE = "001",
                    CRT_STAFF = Current.Staff,
                    CRT_NAME = Current.StaffName,
                    CRT_DATE = now,
                    ROUTE = "當日配北區",
                    DELV_TIMES = "11",
                    STATUS = "0",
                    CUST_EDI_STATUS = "0",
                    DISTR_EDI_STATUS = "0",
                    CUST_EDI_QTY = 0,
                    SEND_DATE = new DateTime(2020, 9, 29),
                    DISTR_USE = "01",
                    DISTR_SOURCE = "0",
                    DELIVID_SEQ_NAME = "TCATConsign",
                    BOXQTY = 99
                },
                new F050901
                {
                    CONSIGN_ID = 26939,
                    CONSIGN_NO = "905073307999",
                    WMS_NO = "O2020092900003",
                    ROUTE_CODE = "A105",
                    GUP_CODE = "01",
                    CUST_CODE = "010001",
                    DC_CODE = "001",
                    CRT_STAFF = Current.Staff,
                    CRT_NAME = Current.StaffName,
                    CRT_DATE = now,
                    ROUTE = "當日配北區",
                    DELV_TIMES = "11",
                    STATUS = "0",
                    CUST_EDI_STATUS = "0",
                    DISTR_EDI_STATUS = "0",
                    CUST_EDI_QTY = 0,
                    SEND_DATE = new DateTime(2020, 9, 29),
                    DISTR_USE = "01",
                    DISTR_SOURCE = "0",
                    DELIVID_SEQ_NAME = "TCATConsign",
                    BOXQTY = 99
                }
            }).AsEnumerable();
            string[] withoutColumns = new string[] { "SEND_DATE" };
            #endregion

            _f050901Repo.BulkInsert(entities, withoutColumns);
        }

        [TestMethod]
        public void BulkUpdateDistrEdiStatus()
        {
            #region Params
            
            List<EgsReturnConsign> datas = new List<EgsReturnConsign>
            {
                new EgsReturnConsign{ DC_CODE = "001", GUP_CODE = "01", CUST_CODE = "010001", CONSIGN_NO = "76519191374" },
                new EgsReturnConsign{ DC_CODE = "001", GUP_CODE = "01", CUST_CODE = "010001", CONSIGN_NO = "76519191516" }
            };

            List<HctShipReturn> datas2 = new List<HctShipReturn>
            {
                new HctShipReturn{ DC_CODE = "001", GUP_CODE = "01", CUST_CODE = "010001", CONSIGN_NO = "76519191523" },
                new HctShipReturn{ DC_CODE = "001", GUP_CODE = "01", CUST_CODE = "010001", CONSIGN_NO = "76519191521" }
            };

            List<KTJShipReturn> datas3 = new List<KTJShipReturn>
            {
                new KTJShipReturn{ DC_CODE = "001", GUP_CODE = "01", CUST_CODE = "010001", CONSIGN_NO = "76519191517" },
                new KTJShipReturn{ DC_CODE = "001", GUP_CODE = "01", CUST_CODE = "010001", CONSIGN_NO = "76519191377" }
            };
            #endregion

            _f050901Repo.BulkUpdateDistrEdiStatus(datas);
        }

        [TestMethod]
        public void BulkUpdateDistrEdiStatusSod()
        {
            #region Params
            var datas = new List<F050901>
            {
                new F050901
                {
                    STATUS = "7",
                    PAST_DATE = new DateTime(2020, 9, 29),
                    SEND_DATE = null,
                    RESULT = "已送達2",
                    DC_CODE = "001",
                    GUP_CODE = "01",
                    CUST_CODE = "010001",
                    CONSIGN_NO = "905154698356"
                },
                new F050901
                {
                    STATUS = "7",
                    PAST_DATE = new DateTime(2020, 9, 29),
                    SEND_DATE = new DateTime(2020, 9, 30),
                    RESULT = "已送達2。",
                    DC_CODE = "001",
                    GUP_CODE = "01",
                    CUST_CODE = "010001",
                    CONSIGN_NO = "625049876551"
                }
            };
            var status = "7";
            #endregion

            _f050901Repo.BulkUpdateDistrEdiStatusSod(datas, status);
        }

        [TestMethod]
        public void GetUpDataForSOD()
        {
            #region Params
            var customerId = "8686509404";
            var consignNos = new List<string> { "905175584390", "905175584406" };
            #endregion

            _f050901Repo.GetUpDataForSOD(customerId, consignNos);
        }

        [TestMethod]
        public void GetUpDataForLogId()
        {
            #region Params
            var customerId = "04644480168";
            var logId = "TOMORROWDELIVID";
            var consignNos = new List<string> { "1944320206", "1944320280", "1944320092" };
            #endregion

            _f050901Repo.GetUpDataForLogId(customerId, logId, consignNos);
        }

        [TestMethod]
        public void UpdateData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var consinNo = "99999999999";
            var wmsNo = "O2017030100200";
            #endregion

            _f050901Repo.UpdateData(dcCode, gupCode, custCode, consinNo, wmsNo);
        }

      
        [TestMethod]
        public void GetDatasByConsignNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var consignNos = new List<string> { "1559569", "1559598" };
            #endregion

            _f050901Repo.GetDatasByConsignNo(dcCode, gupCode, custCode, consignNos);
        }

        [TestMethod]
        public void GetHctShipReturns()
        {
            #region Params
            var hctShipReturnParam = new HctShipReturnParam
            {
                CustomerId = "04644480176",
                DcCode = "001",
                GupCode = "01",
                CustCode = "020003",
                Channel = "00",
                AllId = "HCT",
                DelvTimes = "11",
                OrdSDate = new DateTime(2019, 7, 23),
                OrdEDate = new DateTime(2019, 7, 23),
                DelvSDate = new DateTime(2019, 7, 23),
                DelvEDate = new DateTime(2019, 7, 23)
            };
            #endregion

            _f050901Repo.GetHctShipReturns(hctShipReturnParam);
        }

        [TestMethod]
        public void GetKTJShipReturns()
        {
            #region Params
            var hctShipReturnParam = new HctShipReturnParam
            {
                CustomerId = "04644480176",
                DcCode = "001",
                GupCode = "01",
                CustCode = "020003",
                Channel = "00",
                AllId = "HCT",
                DelvTimes = "11",
                OrdSDate = new DateTime(2019, 7, 23),
                OrdEDate = new DateTime(2019, 7, 23),
                DelvSDate = new DateTime(2019, 7, 23),
                DelvEDate = new DateTime(2019, 7, 23)
            };
            #endregion

            _f050901Repo.GetKTJShipReturns(hctShipReturnParam);
		}

		[TestMethod]
		public void GetDatasByWmsOrdNos()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var wmsOrdNos = new List<string> { "O2017022000001", "O2017022000002" };
			#endregion

			_f050901Repo.GetDatasByWmsOrdNos(dcCode, gupCode, custCode, wmsOrdNos);
		}
	}
}
