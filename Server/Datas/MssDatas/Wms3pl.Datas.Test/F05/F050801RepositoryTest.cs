using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.Datas.Test.F05
{
	[TestClass]
	public class F050801RepositoryTest : BaseRepositoryTest
	{
		private F050801Repository _f050801Repo;
		public F050801RepositoryTest()
		{
			_f050801Repo = new F050801Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void BulkInsert()
		{
			//SharedService ss = new SharedService();
			//var wmsOrdNos = ss.GetNewOrdStackCodes("O", 2);
			//wmsOrdNos.Pop();

			#region Params
			var entities = (new List<F050801>
						{
								new F050801
								{
										WMS_ORD_NO = "O2020092299992",
										DELV_DATE = new DateTime(2020, 09, 22),
										CUST_CODE = "010001",
										ORD_TYPE = "1",
										PICK_TIME = "17:08",
										PRINT_FLAG = "1",
										GUP_CODE = "01",
										DC_CODE= "001",
										CRT_STAFF = "System",
										CRT_DATE= DateTime.Now,
										UPD_STAFF= "System",
										UPD_DATE= DateTime.Now,
										PICK_ORD_NO= "P2020092299997",
										CRT_NAME= "System",
										UPD_NAME = "System",
										STATUS = 5,
										TMPR_TYPE = "01",
										SELF_TAKE = "0",
										FRAGILE_LABEL = "0",
										GUARANTEE = "0",
										HELLO_LETTER = "0",
										SA = "0",
										CUST_NAME = "五支菸",
										INVOICE_PRINT_CNT = 0,
										GENDER = "0",
										ORD_PROP = "O7",
										NO_LOADING = "1",
										SA_QTY = 0,
										NO_AUDIT = "1",
										PRINT_DELV = "0",
										PRINT_BOX = "0",
										VIRTUAL_ITEM = "1",
										ALL_ID = "FAMILY",
										SPECIAL_BUS = "0",
										ZIP_CODE = "241",
										CAN_FAST = "0",
										ALLOWORDITEM = "0",
										PRINT_DETAIL_FLAG = "0",
										SA_CHECK_QTY = 0,
										DELV_PERIOD = "0",
										CVS_TAKE=  "0",
										SELFTAKE_CHECKCODE = "0",
										ROUND_PIECE  ="0"
								},
								new F050801
								{
										WMS_ORD_NO = "O2020092299993",
										DELV_DATE = new DateTime(2020, 09, 22),
										CUST_CODE = "010001",
										ORD_TYPE = "1",
										PICK_TIME = "17:08",
										PRINT_FLAG = "1",
										GUP_CODE = "01",
										DC_CODE= "001",
										CRT_STAFF = "System",
										CRT_DATE= DateTime.Now,
										UPD_STAFF= "System",
										UPD_DATE= DateTime.Now,
										PICK_ORD_NO= "P2020092299996",
										CRT_NAME= "System",
										UPD_NAME = "System",
										STATUS = 5,
										TMPR_TYPE = "01",
										SELF_TAKE = "0",
										FRAGILE_LABEL = "0",
										GUARANTEE = "0",
										HELLO_LETTER = "0",
										SA = "0",
										CUST_NAME = "五支菸",
										INVOICE_PRINT_CNT = 1,
										GENDER = "0",
										ORD_PROP = "O7",
										NO_LOADING = "1",
										SA_QTY = 0,
										NO_AUDIT = "1",
										PRINT_DELV = "0",
										PRINT_BOX = "0",
										VIRTUAL_ITEM = "1",
										ALL_ID = "FAMILY",
										SPECIAL_BUS = "0",
										ZIP_CODE = "241",
										CAN_FAST = "0",
										ALLOWORDITEM = "0",
										PRINT_DETAIL_FLAG = "0",
										SA_CHECK_QTY = 0,
										DELV_PERIOD = "0",
										CVS_TAKE=  "0",
										SELFTAKE_CHECKCODE = "0",
										ROUND_PIECE  ="0"
								}
						}).AsEnumerable();
			var withoutColumns = new string[] { "ZIP_CODE" };
			#endregion

			_f050801Repo.BulkInsert(entities, withoutColumns);
		}

		[TestMethod]
		public void GetF050801WithF055001Datas()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var delvDate = new DateTime(2020, 02, 12);
			var pickTime = "15:48";
			var wmsOrdNo = "O2020021200001";
			var pastNo = "620003369001";
			var itemCode = "BB010101";
			var ordNo = "S2020021200002";
			#endregion

			_f050801Repo.GetF050801WithF055001Datas(dcCode, gupCode, custCode, delvDate, pickTime, wmsOrdNo, pastNo, itemCode, ordNo);
		}

		[TestMethod]
		public void GetIsMerge()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "030001";
			var ordNoList = (new List<string> { "S2019071900010", "S2019071900012" }).AsEnumerable();
			#endregion

			_f050801Repo.GetIsMerge(dcCode, gupCode, custCode, ordNoList);
		}

		[TestMethod]
		public void GetSpiltPostingOrdCount()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var wmsOrdNoList = (new List<string> { "O2017022000001", "O2017022000002" }).AsEnumerable();
			#endregion

			_f050801Repo.GetSpiltPostingOrdCount(dcCode, gupCode, custCode, wmsOrdNoList);
		}

		[TestMethod]
		public void UpdateStatusCancel()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var wmsOrdNoList = (new List<string> { "O2017022000080", "O2017022000081" }).AsEnumerable();
			#endregion

			_f050801Repo.UpdateStatusCancel(gupCode, custCode, dcCode, wmsOrdNoList);
		}



		[TestMethod]
		public void GetData()
		{
			#region Params
			var wmsOrdNo = "O2017022000007";
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			#endregion

			_f050801Repo.GetData(wmsOrdNo, gupCode, custCode, dcCode);
		}

		[TestMethod]
		public void GetF050801NoShipOrders()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var delvDate = new DateTime(2017, 3, 28);
			var pickTime = "08:38";
			var status = "";
			var ordNo = "S2017032800013";
			var custOrdNo = "170327003158REN";
			#endregion

			_f050801Repo.GetF050801NoShipOrders(dcCode, gupCode, custCode, delvDate, pickTime, status, ordNo, custOrdNo);
		}

		[TestMethod]
		public void GetF050801ByOrderNo()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var ordNo = "S2017032800013";
			#endregion

			_f050801Repo.GetF050801ByOrderNo(dcCode, gupCode, custCode, ordNo);
		}

		[TestMethod]
		public void GetF050801ListBySourceNo()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var sourceNo = "D2020010200002";
			#endregion

			_f050801Repo.GetF050801ListBySourceNo(dcCode, gupCode, custCode, sourceNo);
		}

		[TestMethod]
		public void GetP05030201BasicData()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var wmsOrdNo = "O2017022000013";
			var ordNo = "S2017022000006";
			#endregion

			_f050801Repo.GetP05030201BasicData(gupCode, custCode, dcCode, wmsOrdNo, ordNo);
		}

		[TestMethod]
		public void GetSourceNosByWmsOrdNo()
		{
			#region Params
			var gupCode = "01";
			var custCode = "030001";
			var dcCode = "001";
			var wmsOrdNo = "O2019050700001";
			#endregion

			_f050801Repo.GetSourceNosByWmsOrdNo(gupCode, custCode, dcCode, wmsOrdNo);
		}

		[TestMethod]
		public void GetF050801ByDelvPickTime()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "030001";
			var delvDate = new DateTime(2018, 5, 11);
			var pickTime = "12:28";
			var custOrdNo = "141632727";
			#endregion

			_f050801Repo.GetF050801ByDelvPickTime(dcCode, gupCode, custCode, delvDate, pickTime, custOrdNo);
		}

		[TestMethod]
		public void GetF710802Type1()
		{
			#region Params
			var gupCode = "10";
			var custCode = "010001";
			var dcCode = "12";
			var changeDateBegin = new DateTime(2023, 1, 1);
			var changeDateEnd = new DateTime(2023, 01, 31);
			var itemCode = "NN007";
			var itemName = "一般商品";
			var receiptType = "";
			#endregion

			_f050801Repo.GetF710802Type1(gupCode, custCode, dcCode, changeDateBegin, changeDateEnd, itemCode, itemName, receiptType,"");
		}

		[TestMethod]
		public void GetF710802Type2()
		{
      #region Params
      var gupCode = "10";
      var custCode = "010001";
      var dcCode = "12";
      var changeDateBegin = new DateTime(2023, 1, 1);
      var changeDateEnd = new DateTime(2023, 1, 31);
      var itemCode = "";  //"BB020305";
      var itemName = "";  //"Bbia";
      var receiptType = ""; //"04";
      #endregion

      _f050801Repo.GetF710802Type2(gupCode, custCode, dcCode, changeDateBegin, changeDateEnd, itemCode, itemName, receiptType, "");
    }

    [TestMethod]
		public void GetF710802Type3()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var changeDateBegin = new DateTime(2017, 1, 1);
			var changeDateEnd = new DateTime(2020, 12, 31);
			var itemCode = "BB020305";
			var itemName = "Bbia";
			#endregion

			_f050801Repo.GetF710802Type3(gupCode, custCode, dcCode, changeDateBegin, changeDateEnd, itemCode, itemName,"");
		}

		[TestMethod]
		public void GetAllIdByWmsOrdNo()
		{
			#region Params
			var wmsOrdNo = "O2017022000014";
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			#endregion

			_f050801Repo.GetAllIdByWmsOrdNo(wmsOrdNo, gupCode, custCode, dcCode);
		}

		[TestMethod]
		public void GetAllIdByWmsOrdNos()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var wmsOrdNos = new List<string> { "O2017022100180", "O2017022100156" };
			#endregion

			_f050801Repo.GetAllIdByWmsOrdNos(gupCode, custCode, dcCode, wmsOrdNos);
		}

		[TestMethod]
		public void GetF050801ByDelvPickTime2()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var delvDate = new DateTime(2017, 2, 20);
			var pickTime = "16:54";
			var wmsOrdNos = (new List<string> { "O2017022000134", "O2017022000132", "O2017022000133" }).AsEnumerable();
			#endregion

			_f050801Repo.GetF050801ByDelvPickTime(dcCode, gupCode, custCode, delvDate, pickTime, wmsOrdNos);
		}

        [TestMethod]
        public void GetF050801DataForF1101()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var wmsOrdNos = "O2019050700001";
            #endregion

			_f050801Repo.GetF050801DataForF1101(dcCode, gupCode, custCode, wmsOrdNos);
		}

		[TestMethod]
		public void UpdateAllId()
		{
			#region Params
			var gupCode = "01";
			var custCode = "030002";
			var dcCode = "001";
			var wmsOrdNoList = new List<string> { "O2018090700013", "O2018091100023" };
			var allId = "711";
			#endregion

			_f050801Repo.UpdateAllId(gupCode, custCode, dcCode, wmsOrdNoList, allId);
		}

		[TestMethod]
		public void ExistsNonCancelByItemCode()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var itemCode = "BB060101";
			#endregion

			_f050801Repo.ExistsNonCancelByItemCode(gupCode, custCode, itemCode);
		}

		[TestMethod]
		public void GetPickTimeList()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "030001";
			var delvDate = new DateTime(2018, 05, 11);
			#endregion

			_f050801Repo.GetPickTimeList(dcCode, gupCode, custCode, delvDate);
		}

		[TestMethod]
		public void GetDcWmsNoOrdPropItems()
		{
			#region Params
			var dcCode = "001";
			var delvDate = new DateTime(2018, 05, 11);
			#endregion

			_f050801Repo.GetDcWmsNoOrdPropItems(dcCode, delvDate);
		}

		[TestMethod]
		public void GetReplensihStock()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var baseDay = 1500;
			#endregion

			_f050801Repo.GetReplensihStock(dcCode, gupCode, custCode, baseDay);
		}

		[TestMethod]
		public void GetCustomerDatas()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var beginDelvDate = new DateTime(2015, 09, 24);
			var endDelvDate = new DateTime(2020, 09, 24);
			var retailCode = "M06";
			var custName = "碩";
			var wmsOrdNo = "";
			var custOrdNo = "";
			#endregion

			_f050801Repo.GetCustomerDatas(dcCode, gupCode, custCode, beginDelvDate, endDelvDate, retailCode, custName, wmsOrdNo, custOrdNo);
		}

		[TestMethod]
		public void GetF050801sByDistrCarNo()
		{
			#region Params
			var dcCode = "001";
			var distrCarNo = "ZC2017030800049";
			#endregion

			_f050801Repo.GetF050801sByDistrCarNo(dcCode, distrCarNo);
		}

		[TestMethod]
		public void GetDeliveryDatas()
		{
			#region Params
			string dcCode = "001";
			string gupCode = "01";
			string custCode = "010001";
			DateTime settleDateS = new DateTime(2017, 09, 24);
			DateTime settleDateE = new DateTime(2020, 09, 24);
			#endregion

			_f050801Repo.GetDeliveryDatas(dcCode, gupCode, custCode, settleDateS, settleDateE);
		}

		[TestMethod]
		public void GetP050303SearchData()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var delvDateBegin = new DateTime(2020, 1, 1);
			var delvDateEnd = new DateTime(2020, 9, 25);
			var ordNo = "S2020070700006";
			var custOrdNo = "AD20190425002A";
			var wmsOrdNo = "O2020070700005";
			var status = "4";
			var consignNo = "620003369034";
			var itemCode = "BB040101";
			#endregion

			_f050801Repo.GetP050303SearchData(gupCode, custCode, dcCode, delvDateBegin, delvDateEnd,
					ordNo, custOrdNo, wmsOrdNo, status, consignNo, itemCode);
		}

		[TestMethod]
		public void GetF050801sByF050301s()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var ordNos = new string[] { "S2017030900055", "S2017031300120" };
			#endregion

			_f050801Repo.GetF050801sByF050301s(dcCode, gupCode, custCode, ordNos);
		}

		[TestMethod]
		public void GetBetweenStatusF050801s()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var beginStatus = 0;
			var endStatus = 9;
			var wmsOrdNos = (new List<string> { "O2017030800047", "O2017030800094" }).AsEnumerable();
			#endregion

			_f050801Repo.GetBetweenStatusF050801s(dcCode, gupCode, custCode, beginStatus, endStatus, wmsOrdNos);
		}

		[TestMethod]
		public void GetF050801SeparateBillData()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "020003";
			var wmsOrdNo = "O2019012500002";
			#endregion

			_f050801Repo.GetF050801SeparateBillData(dcCode, gupCode, custCode, wmsOrdNo);
		}

		[TestMethod]
		public void GetF700101Data()
		{
			#region Params
			string dcCode = "001";
			string gupCode = "01";
			string custCode = "010001";
			DateTime delvDate = new DateTime(2017, 2, 21);
			string pickTime = "14:49";
			string sourceTye = "";
			string ordType = "0";
			#endregion

			_f050801Repo.GetF700101Data(dcCode, gupCode, custCode, delvDate, pickTime, sourceTye, ordType);
		}

		[TestMethod]
		public void GetDatas()
		{
			#region Params
			string dcCode = "001";
			string gupCode = "01";
			string custCode = "010001";
			DateTime takeDate = new DateTime(2017, 3, 8);
			string status = "5";
			#endregion

			_f050801Repo.GetDatas(dcCode, gupCode, custCode, takeDate, status);
		}

		[TestMethod]
		public void UpdateStatus()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var wmsOrdNo = "O2017030800118";
			var status = 1;
			#endregion

			_f050801Repo.UpdateStatus(dcCode, gupCode, custCode, wmsOrdNo, status);
		}

		[TestMethod]
		public void GetDelvdtlInfo()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var wmsOrdNo = "O2017022000009";
			#endregion

			_f050801Repo.GetDelvdtlInfo(dcCode, gupCode, custCode, wmsOrdNo);
		}

		[TestMethod]
		public void GetDatas2()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var wmsOrdNos = new List<string> { "O2017030900033", "O2017030900073" };
			#endregion

			_f050801Repo.GetDatas(dcCode, gupCode, custCode, wmsOrdNos);
		}

		[TestMethod]
		public void GetDelvNoItems()
		{
			#region Params
			var calDate = new DateTime(2017, 02, 20);
			#endregion

			_f050801Repo.GetDelvNoItems(calDate);
		}


		[TestMethod]
		public void GetDatasByPickOrdNos()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var pickOrdNos = new List<string> { "P2017022000002", "P2017022000001" };
			#endregion

			_f050801Repo.GetDatasByPickOrdNos(dcCode, gupCode, custCode, pickOrdNos);
		}

		[TestMethod]
		public void GetHealthInsuranceSalesData()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var startDate = new DateTime(2017, 2, 20);
			var endDate = new DateTime(2017, 2, 20);
			var itemCode = new string[] { "BB020201", "BB020203" };
			#endregion

			_f050801Repo.GetHealthInsuranceSalesData(dcCode, gupCode, custCode, startDate, endDate, itemCode);
		}

    [TestMethod]
    public void GetReplensihStockData()
    {
      #region Params
      var dcCode = "12";
      var gupCode = "10";
      var custCode = "010001";
      #endregion

      _f050801Repo.GetReplensihStockData(dcCode, gupCode, custCode);
    }
  }
}
