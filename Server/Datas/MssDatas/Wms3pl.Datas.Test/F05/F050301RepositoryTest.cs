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
    public class F050301RepositoryTest : BaseRepositoryTest
    {
        private F050301Repository _f050301Repo;
        public F050301RepositoryTest()
        {
            _f050301Repo = new F050301Repository(Schemas.CoreSchema);
        }


        [TestMethod]
        public void BulkInsert()
        {
            #region Params
            var entities = (new List<F050301>
            {
                new F050301
                {
                    ORD_NO = "S2020092199990",
                    CUST_ORD_NO = "Test",
                    CUST_CODE = "010001",
                    ORD_TYPE = "1",
                    ORD_PROP = "O7",
                    ORD_DATE = new DateTime(2020, 9, 21),
                    PROC_FLAG = "1",
                    CUST_NAME = "林*樺",
                    SELF_TAKE = "0",
                    GUP_CODE = "01",
                    DC_CODE = "001",
                    CRT_STAFF = "00003",
                    CRT_DATE = DateTime.Now,
                    CRT_NAME = "蔡佳華",
                    FRAGILE_LABEL = "0",
                    GUARANTEE = "0",
                    HELLO_LETTER = "1",
                    SA = "0",
                    GENDER = "0",
                    ADDRESS = "Test",
                    ORDER_BY = 0,
                    CONSIGNEE = "0",
                    TRAN_CODE = "O7",
                    SP_DELV = "00",
                    BATCH_NO = "COSM_20170221_1001",
                    CHANNEL = "00",
                    POSM = "0",
                    CONTACT = "Test",
                    CONTACT_TEL = "Test",
                    SPECIAL_BUS = "0",
                    ALL_ID = "711",
                    COLLECT = "1",
                    COLLECT_AMT = 111,
                    ARRIVAL_DATE = new DateTime(2020, 9, 21),
                    TYPE_ID = "G",
                    CAN_FAST = "0",
                    TEL_1 = "Test",
                    PRINT_RECEIPT = "0",
                    TICKET_ID = 2,
                    PICK_TEMP_NO = "170221132431_01",
                    ZIP_CODE = "338",
                    INVO_TAX_TYPE = "0",
                    INVO_PRINTED = "0",
                    HELLO_LETTER_PRINTED = "1",
                    VOLUMN = 111,
                    HAVE_ITEM_INVO = "0",
                    WEIGHT = 2,
                    NP_FLAG = "0",
                    SA_CHECK_QTY = 0,
                    DELV_PERIOD = "4",
                    CVS_TAKE = "1",
                    SUBCHANNEL = "COSM",
                    ROUND_PIECE = "0"
                },
                new F050301
                {
                    ORD_NO = "S2020092199991",
                    CUST_ORD_NO = "Test",
                    CUST_CODE = "010001",
                    ORD_TYPE = "1",
                    ORD_PROP = "O7",
                    ORD_DATE = new DateTime(2020, 9, 21),
                    PROC_FLAG = "1",
                    CUST_NAME = "林*樺",
                    SELF_TAKE = "0",
                    GUP_CODE = "01",
                    DC_CODE = "001",
                    CRT_STAFF = "00003",
                    CRT_DATE = DateTime.Now,
                    CRT_NAME = "蔡佳華",
                    FRAGILE_LABEL = "0",
                    GUARANTEE = "0",
                    HELLO_LETTER = "2",
                    SA = "0",
                    GENDER = "0",
                    ADDRESS = "Test",
                    ORDER_BY = 0,
                    CONSIGNEE = "0",
                    TRAN_CODE = "O7",
                    SP_DELV = "00",
                    BATCH_NO = "COSM_20170221_1001",
                    CHANNEL = "00",
                    POSM = "0",
                    CONTACT = "Test",
                    CONTACT_TEL = "Test",
                    SPECIAL_BUS = "0",
                    ALL_ID = "711",
                    COLLECT = "1",
                    COLLECT_AMT = 111,
                    ARRIVAL_DATE = new DateTime(2020, 9, 21),
                    TYPE_ID = "G",
                    CAN_FAST = "0",
                    TEL_1 = "Test",
                    PRINT_RECEIPT = "0",
                    TICKET_ID = 2,
                    PICK_TEMP_NO = "170221132431_01",
                    ZIP_CODE = "338",
                    INVO_TAX_TYPE = "0",
                    INVO_PRINTED = "0",
                    HELLO_LETTER_PRINTED = "4",
                    VOLUMN = 111,
                    HAVE_ITEM_INVO = "0",
                    WEIGHT = 2,
                    NP_FLAG = "0",
                    SA_CHECK_QTY = 0,
                    DELV_PERIOD = "4",
                    CVS_TAKE = "1",
                    SUBCHANNEL = "COSM",
                    ROUND_PIECE = "0"
                }
            }).AsEnumerable();
            string[] withoutColumns = new string[] { "HELLO_LETTER_PRINTED" };
            #endregion

            _f050301Repo.BulkInsert(entities, withoutColumns);
        }

        [TestMethod]
        public void UpdateStatus()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var dcCode = "001";
            var ordNoList = (new List<string> { "S2017030100004", "S2017030100005" }).AsEnumerable();
            var status = "0";
            #endregion

            _f050301Repo.UpdateStatus(gupCode, custCode, dcCode, ordNoList, status);
        }

        [TestMethod]
        public void GetNonCancelDatasByStatus()
        {
            #region Params
            var procFlag = "0";
            #endregion

            _f050301Repo.GetNonCancelDatasByStatus(procFlag);
        }

        [TestMethod]
        public void GetF050301Datas()
        {
            #region Params
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string delvDate = "2020-07-07";
            string pickTime = "17:37";
            string custOrdNo = "AD20190425004A";
            string itemCode = "I";
            string consignee = "";
            string ordNo = "S2020070700004";
            string workType = "5";
            #endregion

            _f050301Repo.GetF050301Datas(dcCode, gupCode, custCode, delvDate,
            pickTime, custOrdNo, itemCode, consignee, ordNo, workType);
        }

        [TestMethod]
        public void IsOrderNotPackage()
        {
            #region Params
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string ordNo = "S2017022000093";
            #endregion

            _f050301Repo.IsOrderNotPackage(dcCode, gupCode, custCode, ordNo);
        }

       

        [TestMethod]
        public void GetDataByWmsOrdNo()
        {
            #region Params
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string wmsOrdNo = "O2017022000009";
            #endregion

            _f050301Repo.GetDataByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo);
        }

        [TestMethod]
        public void GetProgressData()
        {
            #region Params
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string pickTime = "10:57";
            DateTime? delvDate = new DateTime(2017, 2, 20);
            string pickOrdNo = "P2017022000001";
            #endregion

            _f050301Repo.GetProgressData(dcCode, gupCode, custCode, pickTime, delvDate, pickOrdNo);
        }

        [TestMethod]
        public void GetWmsOrdNoWithF050301Data()
        {
            #region Params
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string wmsOrdNo = "O2017022000009";
            #endregion

            _f050301Repo.GetWmsOrdNoWithF050301Data(dcCode, gupCode, custCode, wmsOrdNo);
        }

        [TestMethod]
        public void GetF050301WmsNoData()
        {
            #region Params
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string ordNo = "S2017030900042";
            #endregion

            _f050301Repo.GetF050301WmsNoData(dcCode, gupCode, custCode, ordNo);
        }

        [TestMethod]
        public void GetDcWmsNoDateItems()
        {
            #region Params
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            DateTime begOrdDate = new DateTime(2017, 4, 7);
            DateTime endOrdDate = new DateTime(2020, 9, 21);
            #endregion

            _f050301Repo.GetDcWmsNoDateItems(dcCode, gupCode, custCode, begOrdDate, endOrdDate);
        }

        [TestMethod]
        public void GetEgsReturnConsigns()
        {
            #region Params
            var param = new EgsReturnConsignParam
            {
                CustomerId = "1265635401",
                DcCode = "001",
                GupCode = "01",
                CustCode = "010001",
                Channel = "00",
                AllId = "TCAT",
                DelvTimes = "11",
                OrdSDate = new DateTime(2020, 2, 12),
                OrdEDate = new DateTime(2020, 7, 7),
                DelvSDate = new DateTime(2020, 2, 12),
                DelvEDate = new DateTime(2020, 7, 7)
            };
            #endregion

            _f050301Repo.GetEgsReturnConsigns(param);
        }

        
      

        [TestMethod]
        public void GetF050301DataByOrdNos()
        {
            #region Params
            string gupCode = "01";
            string custCode = "010001";
            List<string> ordNos = new List<string> { "S2017030100002", "S2017030100003" };
            #endregion

            _f050301Repo.GetF050301DataByOrdNos(gupCode, custCode, ordNos);
        }

       
      
        [TestMethod]
        public void UpdateLackToCancelOrder()
        {
            #region Params
            string gupCode = "01";
            string custCode = "010001";
            #endregion

            _f050301Repo.UpdateLackToCancelOrder(gupCode, custCode);
        }

        [TestMethod]
        public void DeleteLackOrder()
        {
            #region Params
            string gupCode = "01";
            string custCode = "010002";
            #endregion

            _f050301Repo.DeleteLackOrder(gupCode, custCode);
        }
    }
}
