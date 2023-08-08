using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F25
{
    [TestClass]
    public class F2501RepositoryTest : BaseRepositoryTest
    {
        private readonly F2501Repository _f2501Repository;
        public F2501RepositoryTest()
        {
            _f2501Repository = new F2501Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void Get2501QueryData()
        {
            string gupCode = "01";
            string custCode = "030001";
            string[] itemCode = new string[] { };
            string boxSerial = "";
            string batchNo = "";
            string[] serialNo = new string[] { };
            string cellNum = "";
            string poNo = "";
            string[] wmsNo = new string[] { };
            string status = "";
            string OrdProp = "";
            string retailCode = "";
            Int16? combinNo = null;
            string crtName = "";
            DateTime? crtSDate = null;
            DateTime? crtEDate = null;
            DateTime? updSDate = null;
            DateTime? updEDate = null;

            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                gupCode,
                custCode,
                itemCode,
                boxSerial,
                batchNo,
                serialNo,
                cellNum,
                poNo,
                wmsNo,
                status,
                OrdProp,
                retailCode,
                combinNo,
                crtName,
                crtSDate,
                crtEDate,
                updSDate,
                updEDate
            })}");
            var result = _f2501Repository.Get2501QueryData(gupCode,
                custCode,
                itemCode,
                boxSerial,
                batchNo,
                serialNo,
                cellNum,
                poNo,
                wmsNo,
                status,
                OrdProp,
                retailCode,
                combinNo,
                crtName,
                crtSDate,
                crtEDate,
                updSDate,
                updEDate);
            Console.WriteLine($@"{JsonSerializer.Serialize(result)}");
        }

        [TestMethod]
        public void GetcClearSerialBoxOrCaseNoesByAllocation()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "030001";
            string allocationNo = "T2018081400023";
            List<string> itemCodes = new List<string>();
            string countColumnName = "CASE_NO";
            bool isUp = false;
            Output(new
            {
                dcCode,
                gupCode,
                custCode,
                allocationNo,
                itemCodes,
                countColumnName,
                isUp
            });
            var result = _f2501Repository.GetcClearSerialBoxOrCaseNoesByAllocation(dcCode,
                gupCode,
                custCode,
                allocationNo,
                itemCodes,
                countColumnName,
                isUp);
            Output(result);
        }

        [TestMethod]
        public void GetDatasByBoxSerial()
        {
            string gupCode = "01";
            string custCode = "030001";
            string boxSerial = "TRE220";
            Output(new { gupCode, custCode, boxSerial });
            var result = _f2501Repository.GetDatasByBoxSerial(gupCode, custCode, boxSerial);
            Output(result);
        }

        [TestMethod]
        public void GetDatasByBatchNoAndBoxSerial()
        {
            string gupCode = "01";
            string custCode = "030001";
            string batchNo = "66525";
            string boxSerial = "TRE220";
            Output(new { gupCode, custCode, batchNo, boxSerial });
            var result = _f2501Repository.GetDatasByBatchNoAndBoxSerial(gupCode, custCode, batchNo, boxSerial);
            Output(result);
        }

        [TestMethod]
        public void GetDatasByCaseNo()
        {
            string gupCode = "01";
            string custCode = "030001";
            string caseNo = "91";
            Output(new { gupCode, custCode, caseNo });
            var result = _f2501Repository.GetDatasByCaseNo(gupCode, custCode, caseNo);
            Output(result);
        }

        [TestMethod]
        public void GetFirstDataByBatchNo()
        {
            string gupCode = "01";
            string custCode = "030001";
            string batchNo = "66525";
            Output(new { gupCode, custCode, batchNo });
            var result = _f2501Repository.GetFirstDataByBatchNo(gupCode, custCode, batchNo);
            Output(result);
        }

        [TestMethod]
        public void GetFirstDataByBoxSerial()
        {
            string gupCode = "01";
            string custCode = "030001";
            string boxSerial = "TRE220";
            Output(new { gupCode, custCode, boxSerial });
            var result = _f2501Repository.GetFirstDataByBoxSerial(gupCode, custCode, boxSerial);
            Output(result);
        }

        [TestMethod]
        public void GetFirstDataByCaseNo()
        {
            string gupCode = "01";
            string custCode = "030001";
            string caseNo = "91";
            Output(new { gupCode, custCode, caseNo });
            var result = _f2501Repository.GetFirstDataByCaseNo(gupCode, custCode, caseNo);
            Output(result);
        }

        [TestMethod]
        public void GetF2501DataBySerialNoAndValidDate()
        {
            string custCode = "030001";
            string gupCode = "01";
            string serialNoBegin = "1";
            string serialNoEnd = "6";
            DateTime? validDateBegin = null;
            DateTime? validDateEnd = null;
            Output(new { custCode, gupCode, serialNoBegin, serialNoEnd, validDateBegin, validDateEnd });
            var result = _f2501Repository.GetF2501DataBySerialNoAndValidDate(custCode, gupCode, serialNoBegin, serialNoEnd, validDateBegin, validDateEnd);
            Output(result);
        }

        [TestMethod]
        public void GetDatasByCombinNo()
        {
            string gupCode = "01"; string custCode = "030001"; long combinNo = 32;
            Output(new { gupCode, custCode, combinNo });
            var result = _f2501Repository.GetDatasByCombinNo(gupCode, custCode, combinNo);
            Output(result);
        }

        [TestMethod]
        public void GetF2501sByBoundleItemCode()
        {
            string gupCode = "01"; string custCode = "030001";
            IEnumerable<string> serialNos = new List<string>() { "6" };

            Output(new { gupCode, custCode, serialNos });
            var result = _f2501Repository.GetF2501sByBoundleItemCode(gupCode, custCode, serialNos);
            Output(result);
        }

        [TestMethod]
        public void GetF2501sByF16140101()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "030001";
            string returnNo = "";
            string postingStatus = "";

            Output(new { dcCode, gupCode, custCode, returnNo, postingStatus });
            var result = _f2501Repository.GetF2501sByF16140101(dcCode, gupCode, custCode, returnNo, postingStatus);
            Output(result);
        }

        [TestMethod]
        public void GetDatas()
        {
            
            string gupCode = "01";
            string custCode = "030001";
            List<string> snList = new List<string>() { "6"};
            Output(new {  gupCode, custCode, snList });
            var result = _f2501Repository.GetDatas(gupCode, custCode, snList);
            Output(result);
        }

        [TestMethod]
        public void GetSerialIsFreeze()
        {
            string gupCode = "";
            string custCode = "";
            string controlType = "";
            IEnumerable<string> serialNos = new List<string>();
            Output(new { gupCode, custCode, controlType, serialNos});
            var result = _f2501Repository.GetSerialIsFreeze(gupCode, custCode, controlType, serialNos);
            Output(result);
        }

   

       

      
      

        [TestMethod]
        public void GetF2501WcfData()
        {
            string gupCode = "01";
            string custCode = "030001";
            string serialNo = "5";
            Output(new { gupCode, custCode, serialNo });
            var result = _f2501Repository.GetF2501WcfData(gupCode, custCode, serialNo);
            Output(result);
        }

      

        [TestMethod]
        public void ProcessClearCombinNo()
        {
            string gupCode = "01";
            string custCode = "010001";
            List<string> serialNo = new List<string>() { "4" };
            Output(new { gupCode, custCode, serialNo });
            _f2501Repository.ProcessClearCombinNo(gupCode, custCode, serialNo);
        }

        [TestMethod]
        public void UpdateF2501ValidDate()
        {
            List<string> listSerialNo = new List<string>() { "4" };
            DateTime validDate = DateTime.Today;
            string gupCode = "01";
            string custCode = "010001";
            string userId = "T0001";
            string userName = "TestA";
            Output(new { listSerialNo, validDate, gupCode, custCode, userId, userName });
            _f2501Repository.UpdateF2501ValidDate(listSerialNo, validDate, gupCode, custCode, userId, userName);
        }

        [TestMethod]
        public void GetDataPprocessing1()
        {
            #region Params
            var cust_code = "030001";
            var gup_code = "01";
            var upd_date = Convert.ToDateTime("2019-08-23 00:00:00");
            var rownum = 3;
            #endregion

            _f2501Repository.GetDataPprocessing1(cust_code, gup_code, upd_date, rownum);
        }

        [TestMethod]
        public void GetDataPprocessing2()
        {
            #region Params
            var custCode = "030001";
            var gupCode = "01";
            var snList = new string[] { "1", "2" };
            #endregion

            _f2501Repository.GetDataPprocessing2(custCode, gupCode, snList);
        }

        [TestMethod]
        public void GetSnList1()
        {
            #region Params
            var dcCode = "001";
            var custCode = "030001";
            var gupCode = "01";
            var snList = new List<string> { "5542" };
            #endregion

            _f2501Repository.GetSnList(dcCode,custCode, gupCode, snList);
        }

        [TestMethod]
        public void GetSnList2()
        {
            #region Params
			      var gupCode = "01";
            var custCode = "010001";
            var itemCode = new List<string> { "BPX003" };
            var snList = new List<string> {};
            #endregion

            _f2501Repository.GetSnList(gupCode,custCode, itemCode, snList);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
