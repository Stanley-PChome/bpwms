using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P05.ExDataSources;
using Wms3pl.WebServices.Process.P05.Services;

namespace Wms3pl.WebServices.Process.P05
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P05ExDataService : DataService<P05ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		#region P0501010000 B2B揀貨單列印

		[WebGet]
		public IQueryable<F051201Data> GetF051201DatasForB2B(string dcCode, string gupCode, string custCode, DateTime delvDate,
			string isPrinted)
		{
			var p050101Service = new P050101Service();
			return p050101Service.GetF051201Datas(dcCode, gupCode, custCode, delvDate, isPrinted, "0");
		}

		[WebGet]
		public IQueryable<F051202Data> GetF051202DatasForB2B(string dcCode, string gupCode, string custCode, string delvDate,
			string pickTime)
		{
			var p050101Service = new P050101Service();
			return p050101Service.GetF051202Datas(dcCode, gupCode, custCode, delvDate, pickTime, "0");
		}

		[WebGet]
		public IQueryable<F051201ReportDataA> GetF051201ReportDataAsForB2B(string dcCode, string gupCode, string custCode,
			DateTime delvDate, string pickTime, string pickOrdNo)
		{
			var p050101Service = new P050101Service();
			return p050101Service.GetF051201ReportDataAs(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo);
		}
		[WebGet]
		public IQueryable<F051201ReportDataB> GetF051201ReportDataBsForB2B(string dcCode, string gupCode, string custCode,
			string delvDate, string pickTime, string pickOrdNo)
		{
			var p050101Service = new P050101Service();
			return p050101Service.GetF051201ReportDataBs(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo);
		}

		[WebGet]
		public IQueryable<F051201SelectedData> GetF051201SelectedDatasForB2B(string dcCode, string gupCode, string custCode,
			string delvDate, string pickTime)
		{
			var p050101Service = new P050101Service();
			return p050101Service.GetF051201SelectedDatas(dcCode, gupCode, custCode, delvDate, pickTime, "0");
		}
		[WebGet]
		public IQueryable<ExecuteResult> UpdateF051201ForB2B(string dcCode, string gupCode, string custCode,
			string delvDate, string pickTime, string userId, string userName)
		{
			var wmsTransaction = new WmsTransaction();
			var p050101Service = new P050101Service(wmsTransaction);
			List<string> pickOrdNoList;
			var result = p050101Service.UpdateF051201IsPrinted(dcCode, gupCode, custCode, delvDate, pickTime, userId, userName, out pickOrdNoList);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return new List<ExecuteResult> { result }.AsQueryable();

		}

		#endregion

		#region P0501020000 B2C揀貨單列印
		[WebGet]
		public IQueryable<F051201Data> GetF051201DatasForB2C(string dcCode, string gupCode, string custCode, DateTime delvDate,
			string isPrinted)
		{
			var p050101Service = new P050101Service();
			return p050101Service.GetF051201Datas(dcCode, gupCode, custCode, delvDate, isPrinted, "1");
		}
		[WebGet]
		public IQueryable<F051202Data> GetF051202DatasForB2C(string dcCode, string gupCode, string custCode, string delvDate,
			string pickTime)
		{
			var p050101Service = new P050101Service();
			return p050101Service.GetF051202Datas(dcCode, gupCode, custCode, delvDate, pickTime, "1");
		}

		[WebGet]
		public IQueryable<F051201ReportDataA> GetF051201ReportDataAsForB2C(string dcCode, string gupCode, string custCode,
			DateTime delvDate, string pickTime, string pickOrdNo)
		{
			var p050102Service = new P050102Service();
			return p050102Service.GetF051201ReportDataAs(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo);
		}

		[WebGet]
		public IQueryable<F051201SelectedData> GetF051201SelectedDatasForB2C(string dcCode, string gupCode, string custCode,
			string delvDate, string pickTime)
		{
			var p050101Service = new P050101Service();
			return p050101Service.GetF051201SelectedDatas(dcCode, gupCode, custCode, delvDate, pickTime, "1");
		}
		[WebGet]
		public IQueryable<ExecuteResult> UpdateF051201ForB2C(string dcCode, string gupCode, string custCode,
			string delvDate, string pickTime, string userId, string userName, string deviceCount)
		{
			var wmsTransaction = new WmsTransaction();
			var p050102Service = new P050102Service(wmsTransaction);
			var p050101Service = new P050101Service(wmsTransaction);
			var devicePickOrdNoList = p050102Service.UpdateF050102IsDevice(dcCode, gupCode, custCode, delvDate, pickTime, int.Parse(deviceCount));
			List<string> pickOrdNoList;
			var result = p050101Service.UpdateF051201IsPrinted(dcCode, gupCode, custCode, delvDate, pickTime, userId, userName, 
                out pickOrdNoList, isB2B: false, exceptDevicePickOrdNoList: devicePickOrdNoList);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return new List<ExecuteResult> { result }.AsQueryable();

		}

		#endregion

		#region F050801 - 出貨抽稽維護
		[WebGet]
		public IQueryable<F050801WithF055001> GetF050801WithF055001Datas(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string wmsOrdNo, string pastNo, string itemCode, string ordNo)
		{
			var p050801Service = new P050801Service();
			return p050801Service.GetF050801WithF055001Datas(dcCode, gupCode, custCode, delvDate, pickTime, wmsOrdNo, pastNo, itemCode, ordNo);
		}

		#endregion

		#region F0513 - 出貨碼頭分配
		[WebGet]
		public IQueryable<F0513WithF1909> GetF0513WithF1909Datas(string dcCode, string gupCode, string custCode, string delvDate, string delvTime, string status)
		{
			DateTime delvDateTime;
			DateTime.TryParse(delvDate, out delvDateTime);

			var p050201Service = new P050201Service();
			return p050201Service.GetF0513WithF1909Datas(dcCode, gupCode, custCode, delvDateTime, delvTime, status);
		}
		[WebGet]
		public IQueryable<ExecuteResult> UpdatePierCode(string dcCode, string gupCode, string custCode, string delvDate,
			string pickTime, string allId, string takeTime, string pierCode)
		{
			var wmsTransation = new WmsTransaction();
			var p050201Service = new P050201Service(wmsTransation);
			var result = p050201Service.UpdatePierCode(dcCode, gupCode, custCode, delvDate, pickTime, allId, takeTime, pierCode);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return new List<ExecuteResult> { result }.AsQueryable();
		}

		#endregion

		#region F0513 - 扣帳作業-批次
		[WebGet]
		public IQueryable<F0513WithF050801Batch> GetBatchDebitDatas(string dcCode, string gupCode, string custCode, bool notOrder, bool isB2c)
		{
			var p0513Service = new P0513Service();
			return p0513Service.GetBatchDebitDatas(dcCode, gupCode, custCode, notOrder, isB2c).AsQueryable();
		}
		#endregion

		

		#region 未出貨訂單查詢
		[WebGet]
		public IQueryable<F050801NoShipOrders> GetF050801NoShipOrders(string dcCode, string gupCode, string custCode, string delvDate, string pickTime, string status, string ordNo, string custOrdNo)
		{
			var srv = new P050301Service();
			return srv.GetF050801NoShipOrders(dcCode, gupCode, custCode, delvDate, pickTime, status, ordNo, custOrdNo);
		}
		#endregion

		#region 訂單主檔
		[WebGet]
		public IQueryable<F050101Ex> GetF050101ExDatas(string gupCode, string custCode, string dcCode, string ordDateFrom, string ordDateTo, string ordNo, string arriveDateFrom,
			string arriveDateTo, string custOrdNo, string status, string retailCode, string custName, string wmsOrdNo, string pastNo, string address, string channel, string delvType, string allId, string moveOutTarget, string subChannel)
		{
			var srv = new P050302Service();
			return srv.GetF050101ExDatas(gupCode, custCode, dcCode, ordDateFrom, ordDateTo, ordNo, arriveDateFrom,
			arriveDateTo, custOrdNo, status, retailCode, custName, wmsOrdNo, pastNo, address, channel, delvType, allId,moveOutTarget, subChannel);
		}
		#endregion

		#region 訂單明細
		[WebGet]
		public IQueryable<F050102Ex> GetF050102ExDatas(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var srv = new P050302Service();
			return srv.GetF050102ExDatas(dcCode, gupCode, custCode, ordNo);
		}
		#endregion

		#region 訂單維護，點選出貨明細會用到(P0503020100)
		[WebGet]
		public IQueryable<F050102WithF050801> GetF050102WithF050801s(string gupCode, string custCode, string dcCode, string wmsordno)
		{
			var srv = new P050302Service();
			return srv.GetF050102WithF050801s(gupCode, custCode, dcCode, wmsordno);
		}
		[WebGet]
		public IQueryable<P05030201BasicData> GetP05030201BasicData(string gupCode, string custCode, string dcCode, string wmsOrdNo, string ordNo)
		{
			var srv = new P050302Service();
			return srv.GetP05030201BasicData(gupCode, custCode, dcCode, wmsOrdNo, ordNo);
		}

		[WebGet]
		public string GetSourceNosByWmsOrdNo(string gupCode, string custCode, string dcCode, string wmsOrdNo)
		{
			var repo = new F050801Repository(Schemas.CoreSchema);
			return string.Join("、", repo.GetSourceNosByWmsOrdNo(gupCode, custCode, dcCode, wmsOrdNo).Where(sourceNo => !string.IsNullOrWhiteSpace(sourceNo)));
		}
    #endregion

    #region 貨主訂單手動挑單
    [WebGet]
    public IQueryable<F050001Data> GetF050001Datas(string dcCode, string gupCode, string custCode, string ordType,
      string ordSDate, string ordEDate, string arrivalSDate, string arrivalEDate, string ordNo, string custOrdNo,
      string consignee, string itemCode, string itemName, string sourceType, string retailCode, string carPeriod,
      string delvNo, string custCost, string fastDealType, string crossCode, string channel, string subChannel)
    {
      var srv = new P050304Service();
      return srv.GetF050001Datas(dcCode, gupCode, custCode, ordType, ordSDate, ordEDate, arrivalSDate,
        arrivalEDate, ordNo, custOrdNo, consignee, itemCode, itemName, sourceType, retailCode, carPeriod,
        delvNo, custCost, fastDealType, crossCode, channel, subChannel);
    }

    #region 貨主訂單手動挑單-總庫試算
    [WebGet]
		public List<F050805Data> GetF050805Datas(string dcCode, string gupCode, string custCode, string calNo)
		{
			var srv = new P050304Service();
			return srv.GetF050805Datas(dcCode, gupCode, custCode, calNo);
		}

		[WebGet]
		public List<F05080501Data> GetF05080501Datas(string dcCode, string gupCode, string custCode, string calNo)
		{
			var srv = new P050304Service();
			return srv.GetF05080501Datas(dcCode, gupCode, custCode, calNo);
		}
		[WebGet]
		public List<F05080502Data> GetF05080502Datas(string dcCode, string gupCode, string custCode, string calNo)
		{
			var srv = new P050304Service();
			return srv.GetF05080502Datas(dcCode, gupCode, custCode, calNo);
		}
		[WebGet]
		public List<F05080504Data> GetF05080504Datas(string dcCode, string gupCode, string custCode, string calNo)
		{
			var srv = new P050304Service();
			return srv.GetF05080504Datas(dcCode, gupCode, custCode, calNo);
		}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="calNo"></param>
    /// <param name="flag">查詢模式 0:手動挑單試算 1:配庫試算結果查詢</param>
    /// <returns></returns>
    [WebGet]
    public List<F05080505Data> GetF05080505Datas(string dcCode, string gupCode, string custCode, string calNo, string flag = "1")
    {
      var srv = new P050304Service();
      return srv.GetF05080505Datas(dcCode, gupCode, custCode, calNo, flag);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="calNo"></param>
    /// <param name="flag">查詢模式 0:手動挑單試算 1:配庫試算結果查詢</param>
    /// <returns></returns>
    [WebGet]
    public List<F05080506Data> GetF05080506Datas(string dcCode, string gupCode, string custCode, string calNo, string flag = "1")
    {
      var srv = new P050304Service();
      return srv.GetF05080506Datas(dcCode, gupCode, custCode, calNo, flag);
    }

    #endregion
    #endregion

    #region F710802 作業異動查詢
    [WebGet]
    public IQueryable<P710802SearchResult> GetF710802Type1(string gupCode, string custCode, string dcCode,
      string changeDateBegin, string changeDateEnd, string itemCode, string itemName, string receiptType, string makeNo)
    {
      var repo = new F050801Repository(Schemas.CoreSchema);
      itemName = HttpUtility.HtmlDecode(itemName);
      return repo.GetF710802Type1(gupCode, custCode, dcCode,
          Convert.ToDateTime(changeDateBegin), Convert.ToDateTime(changeDateEnd), itemCode, itemName, receiptType, makeNo);
    }

    [WebGet]
    public IQueryable<P710802SearchResult> GetF710802Type2(string gupCode, string custCode, string dcCode,
      string changeDateBegin, string changeDateEnd, string itemCode, string itemName, string receiptType, string makeNo)
    {
      var repo = new F050801Repository(Schemas.CoreSchema);
      itemName = HttpUtility.HtmlDecode(itemName);
      return repo.GetF710802Type2(gupCode, custCode, dcCode,
          Convert.ToDateTime(changeDateBegin), Convert.ToDateTime(changeDateEnd), itemCode, itemName, receiptType, makeNo);
    }

    [WebGet]
    public IQueryable<P710802SearchResult> GetF710802Type3(string gupCode, string custCode, string dcCode,
      string changeDateBegin, string changeDateEnd, string itemCode, string itemName, string makeNo)
    {
      var repo = new F050801Repository(Schemas.CoreSchema);
      itemName = HttpUtility.HtmlDecode(itemName);
      return repo.GetF710802Type3(gupCode, custCode, dcCode,
          Convert.ToDateTime(changeDateBegin), Convert.ToDateTime(changeDateEnd), itemCode, itemName, makeNo);
    }
    #endregion

    #region P0503030000出貨單查詢
    // 揀貨單資料
    [WebGet]
		public IQueryable<F051201WithF051202> GetF051201WithF051202s(string dcCode,string gupCode,string custCode,string wmsOrdNo)
		{
			var repo = new F051201Repository(Schemas.CoreSchema);
			return repo.GetF051201WithF051202s(dcCode, gupCode, custCode, wmsOrdNo);
		}

		//揀貨單明細
		[WebGet]
		public IQueryable<PickDetail> GetPickDetails(string dcCode,string gupCode,string custCode,string wmsOrdNo)
		{
			var repo = new F051202Repository(Schemas.CoreSchema);
			return repo.GetPickDetails(dcCode, gupCode, custCode, wmsOrdNo);
		}

		// 託運單
		[WebGet]
		public IQueryable<ConsignmentNote> GetConsignmentNote(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var repo = new F055001Repository(Schemas.CoreSchema);
			return repo.GetConsignmentNote(dcCode, gupCode, custCode, wmsOrdNo);
		}
		#endregion

		[WebGet]
		public bool CanViewPersonalData(string empID)
		{
			var srv = new P050302Service();
			var result = srv.GetEmpHasAuth(empID);
			if (result.Count() > 0)
				return true;
			else
				return false;
		}

		[WebGet]
		public IQueryable<P050303QueryItem> GetP050303SearchData(string gupCode, string custCode, string dcCode,
			DateTime? delvDateBegin, DateTime? delvDateEnd, string ordNo, string custOrdNo,
			string wmsOrdNo, string status, string consignNo,string itemCode)
		{
			var srv = new P050303Service();
            return srv.GetP050303SearchData(gupCode, custCode, dcCode, delvDateBegin, delvDateEnd, ordNo, custOrdNo,
                wmsOrdNo, status, consignNo, itemCode);
		}


		[WebGet]
		public IQueryable<F055002WithGridLog> GetF055002WithGridLog(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var srv = new P050303Service();
			return srv.GetF055002WithGridLog(dcCode, gupCode, custCode, wmsOrdNo);
		}

		#region 超取的明細資料
		[WebGet]
		public IQueryable<F050304AddEService> GetF050304ExDatas(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var srv = new P050302Service();
			return srv.GetF050304ExDatas(dcCode, gupCode, custCode, ordNo);
		}
		#endregion

		#region F050801 拆單資料
		[WebGet]
		public IQueryable<F050801WithBill> GetF050801SeparateBillData(string dcCode, string gupCode, string custCode
																	, string wmsOrdNo)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return f050801Repo.GetF050801SeparateBillData(dcCode, gupCode, custCode, wmsOrdNo);
		}
		#endregion

		#region 批次出車時段
		[WebGet]
		public IQueryable<F700101CarData> GetF700101Data(string dcCode, string gupCode, string custCode, string delvDate, string pickTime
														, string sourceTye, string ordType)
		{
			var p050102Service = new P050102Service();
			return p050102Service.GetF700101Data(dcCode, gupCode, custCode, Convert.ToDateTime(delvDate), pickTime, sourceTye, ordType);
		}
		#endregion

		[WebGet]
		public IQueryable<F05010103> GetOrderDelvRetailLogs(string dcCode, string gupCode, string custCode, string ordNo, string type)
		{
			var repo = new F05010103Repository(Schemas.CoreSchema);
			return repo.GetDatasByOrdNo(dcCode, gupCode, custCode, ordNo, type);
		}
		#region 匯總報表
		[WebGet]
		public IQueryable<P050103ReportData> GetSummaryReport(string dcCode, string gupCode, string custCode, string ordType, DateTime delvDate, string pickOrdNo, string wmsOrdNo)
		{
			var repo = new P050103Service();
			return repo.GetSummaryReport(dcCode, gupCode, custCode, ordType, delvDate, pickOrdNo, wmsOrdNo);
		}

		[WebGet]
		public IQueryable<P050103PickTime> GetPickTimes(string dcCode, string gupCode, string custCode, string ordType, DateTime delvDate)
		{
			var repo = new P050103Service();
			return repo.GetPickTimes(dcCode, gupCode, custCode, ordType, delvDate);
		}
		[WebGet]
		public IQueryable<P050103PickOrdNo> GetPickOrderNos(string dcCode, string gupCode, string custCode, string ordType, DateTime delvDate, string pickTime)
		{
			var repo = new P050103Service();
			return repo.GetPickOrdNos(dcCode, gupCode, custCode, ordType, delvDate, pickTime);
		}
		[WebGet]
		public IQueryable<P050103WmsOrdNo> GetWmsOrderNos(string dcCode, string gupCode, string custCode, string ordType, DateTime delvDate, string pickTime)
		{
			var repo = new P050103Service();
			return repo.GetWmsOrderNos(dcCode, gupCode, custCode, ordType, delvDate, pickTime);
		}
		#endregion

		

		#region P050112 揀貨彙總作業
		[WebGet]
		public IQueryable<P050112Pick> GetP050112PickDatas(string dcCode, string gupCode, string custCode, string delvDateS, string delvDateE, string pickTool, string areaCode)
		{
			var repo = new F051201Repository(Schemas.CoreSchema);
			return repo.GetP050112PickDatas(dcCode, gupCode, custCode, DateTime.Parse(delvDateS), DateTime.Parse(delvDateE), pickTool, areaCode);
		}
		[WebGet]
		public IQueryable<P050112Batch> GetP050112Batches(string dcCode, string gupCode, string custCode, string batchDateS, string batchDateE, string batchNo, string pickStatus, string putStatus)
		{
			var repo = new F0515Repository(Schemas.CoreSchema);
			return repo.GetP050112Batches(dcCode, gupCode, custCode, DateTime.Parse(batchDateS), DateTime.Parse(batchDateE), batchNo, pickStatus, putStatus);
		}
		[WebGet]
		public IQueryable<PutReportData> GetPutReportDatas(string dcCode, string gupCode, string custCode, string batchNo)
		{
			var repo = new F051602Repository(Schemas.CoreSchema);
			return repo.GetPutReportDatas(dcCode, gupCode, custCode, batchNo);
		}
		[WebGet]
		public IQueryable<PickReportData> GetPickReportDatas(string dcCode, string gupCode, string custCode, string batchNo)
		{
			var repo = new F051502Repository(Schemas.CoreSchema);
			return repo.GetPickReportDatas(dcCode, gupCode, custCode, batchNo);
		}
		[WebGet]
		public IQueryable<BatchPickStation> GetBatchPickStations(string dcCode, string gupCode, string custCode, string batchNo)
		{
			var repo = new F051501Repository(Schemas.CoreSchema);
			return repo.GetBatchPickStations(dcCode, gupCode, custCode, batchNo);
		}
        #endregion

        #region 配庫試算結果查詢

        [WebGet]
        public IQueryable<P0503050000CalHead> GetCalHeadList(string dcCode,string gupCode,string custCode, 
            DateTime? calDateBegin, DateTime? calDateEnd, string calNo)
        {
            var repo = new F05080503Repository(Schemas.CoreSchema);
            return repo.GetCalHeadList(dcCode, gupCode, custCode, calDateBegin, calDateEnd, calNo);
        }

        #endregion

      

        [WebGet]
        public IQueryable<RP0501010004Model> GetF051201SingleStickersReportDataAsForB2C(string dcCode, string gupCode, string custCode,
            DateTime delvDate, string pickTime, string pickOrdNo)
        {
            var p050102Service = new P050102Service();
            return p050102Service.GetF051201SingleStickersReportDataAsForB2C(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo);
        }

        [WebGet]
        public IQueryable<P050103ReportData> GetF051201BatchReportDataAsForB2C(string dcCode, string gupCode, string custCode,
           DateTime delvDate, string pickTime, string pickOrdNo)
        {
            var p050102Service = new P050102Service();
            return p050102Service.GetF051201BatchReportDataAsForB2C(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo);
        }

        [WebGet]
        public IQueryable<RP0501010005Model> GetF051201BatchStickersReportDataAsForB2C(string dcCode, string gupCode, string custCode,
          DateTime delvDate, string pickTime, string pickOrdNo)
        {
            var p050102Service = new P050102Service();
            return p050102Service.GetF051201BatchStickersReportDataAsForB2C(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo);
        }

        [WebGet]
        public IQueryable<F051202WithF055002> GetF051202WithF055002s(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var srv = new F051202Repository(Schemas.CoreSchema);
            return srv.GetF051202WithF055002s(dcCode, gupCode, custCode, wmsOrdNo);
        }

		[WebGet]
		public IQueryable<BatchPickNoList> GetBatchPickNoList(string dcCode, string gupCode, string custCode, string sourceType,string custCost)
		{
			var p050104Service = new P050104Service();
			return p050104Service.GetBatchPickNoList(dcCode, gupCode, custCode, sourceType, custCost);
		}

		[WebGet]
		public IQueryable<RePickNoList> GetRePickNoList(string dcCode, string gupCode, string custCode, string sourceType, string custCost)
		{
			var p050104Service = new P050104Service();
			return p050104Service.GetRePickNoList(dcCode, gupCode, custCode, sourceType, custCost);
		}

		[WebGet]
		public IQueryable<RePrintPickNoList> GetReprintPickNoList(string dcCode,string gupCode,string custCode,string pickOrdNo,string wmsOrdNo)
		{
			var p050104Service = new P050104Service();
			return p050104Service.GetRePrintPickNoList(dcCode, gupCode, custCode, pickOrdNo, wmsOrdNo);
		}

    /// <summary>
    /// 補印批量揀貨單查詢
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="sourceType"></param>
    /// <param name="custCost"></param>
    /// <returns></returns>
    [WebGet]
    public IQueryable<BatchPickNoList> GetReBatchPrintPickNoList(string dcCode, string gupCode, string custCode, DateTime DelvDate)
    {
      var p050104Service = new P050104Service();
      return p050104Service.GetReBatchPrintPickNoList(dcCode, gupCode, custCode, DelvDate);
    }

    //單一揀貨紙本張數
    [WebGet]
    public IQueryable<SinglePickingReportData> GetSinglePickingReportDatas(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string pickOrdNo)
    {
      var p050104Service = new P050104Service();
      return p050104Service.GetSinglePickingReportDatas(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, false);
    }

    [WebGet]
    public IQueryable<SinglePickingReportData> GetSinglePickingReportDatasCheckNotRePick(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string pickOrdNo, Boolean IsCheckNotRePick)
    {
      var p050104Service = new P050104Service();
      return p050104Service.GetSinglePickingReportDatas(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, IsCheckNotRePick);
    }

    //批量揀紙本張數
    [WebGet]
    public IQueryable<BatchPickingReportData> GetBatchPickingReportDatas(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string pickOrdNo)
    {
      var p050104Service = new P050104Service();
      return p050104Service.GetBatchPickingReportDatas(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, false);
    }

    [WebGet]
    public IQueryable<BatchPickingReportData> GetBatchPickingReportDatasCheckRePrint(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string pickOrdNo, Boolean IsCheckNotRePick)
    {
      var p050104Service = new P050104Service();
      return p050104Service.GetBatchPickingReportDatas(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, IsCheckNotRePick);
    }

    [WebGet]
    public IQueryable<SinglePickingTickerData> GetSinglePickingTickerDatas(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string pickOrdNo)
    {
      var p050104Service = new P050104Service();
      return p050104Service.GetSinglePickingTickerDatas(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, false);
    }

    [WebGet]
    public IQueryable<SinglePickingTickerData> GetSinglePickingTickerDatasCheckRePrint(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string pickOrdNo, Boolean IsCheckNotRePick)
    {
      var p050104Service = new P050104Service();
      return p050104Service.GetSinglePickingTickerDatas(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, IsCheckNotRePick);
    }

    [WebGet]
    public IQueryable<BatchPickingTickerData> GetBatchPickingTickerDatas(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string pickOrdNo)
    {
      var p050104Service = new P050104Service();
      return p050104Service.GetBatchPickingTickerDatas(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, false);
    }

    [WebGet]
    public IQueryable<BatchPickingTickerData> GetBatchPickingTickerDatasCheckRePrint(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string pickOrdNo, Boolean IsCheckNotRePick)
    {
      var p050104Service = new P050104Service();
      return p050104Service.GetBatchPickingTickerDatas(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, IsCheckNotRePick);
    }

    #region 揀貨完成容器資料
    [WebGet]
    public IQueryable<PickContainer> GetPickContainer(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var p050303Service = new P050303Service();
      return p050303Service.GetPickContainerData(dcCode, gupCode, custCode, wmsNo);
    }
    #endregion 揀貨完成容器資料

    #region 訂單取消資訊
    [WebGet]
    public IQueryable<OrderCancelInfo> GetOrderCancelInfoType1(string dcCode, string gupCode, string custCode, string pickOrdNo)
    {
      var p050303Service = new P050303Service();
      return p050303Service.GetOrderCancelInfoDataType1(dcCode, gupCode, custCode, pickOrdNo);
    }

    [WebGet]
    public IQueryable<OrderCancelInfo> GetOrderCancelInfoType2(string dcCode, string gupCode, string custCode, string pickOrdNo)
    {
      var p050303Service = new P050303Service();
      return p050303Service.GetOrderCancelInfoDataType2(dcCode, gupCode, custCode, pickOrdNo);
    }
    #endregion 訂單取消資訊

    #region 分貨資訊
    [WebGet]
    public IQueryable<DivideInfo> GetDivideInfo(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var p050303Service = new P050303Service();
      return p050303Service.GetDivideInfo(dcCode, gupCode, custCode, wmsNo);
    }

    [WebGet]
    public IQueryable<DivideDetail> GetDivideDetail(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var p050303Service = new P050303Service();
      return p050303Service.GetDivideDetail(dcCode, gupCode, custCode, wmsNo);
    }
    #endregion 分貨資訊

    #region 集貨場進出紀錄
    [WebGet]
    public IQueryable<CollectionRecord> GetCollectionRecord(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var p050303Service = new P050303Service();
      return p050303Service.GetCollectionRecord(dcCode, gupCode, custCode, wmsNo);
    }
    #endregion 集貨場進出紀錄

    #region 託運單箱內明細資料
    [WebGet]
    public IQueryable<ConsignmentDetail> GetConsignmentDetail(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var p050303Service = new P050303Service();
      return p050303Service.GetConsignmentDetail(dcCode, gupCode, custCode, wmsNo);
    }
    #endregion 託運單箱內明細資料
  }
}
