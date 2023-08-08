using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P71.ExDataSources;
using Wms3pl.WebServices.Process.P71.Services;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.F00;

namespace Wms3pl.WebServices.Process.P71
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P71ExDataService : DataService<P71ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		#region P710103 - 儲位屬性維護
		/// <summary>
		/// 取得儲位屬性維護清單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="warehouseId"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseType"></param>
		/// <param name="areaCode"></param>
		/// <param name="locCodeS"></param>
		/// <param name="locCodeE"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F1912WithF1980> GetLocListForWarehouse(string dcCode, string warehouseId, string gupCode
			, string custCode, string warehouseType, string areaCode, string channel, string locCodeS, string locCodeE, string account)
		{
			var srv = new P710103Service();
			// 取得儲位屬性維護
			var result = srv.GetF1912WithF1980(dcCode, warehouseId, gupCode, custCode, warehouseType, areaCode, channel, locCodeS, locCodeE, account);
			return result;
		}

		[WebGet]
		public IQueryable<NameValueList> GetDcWarehouseChannels(string dcCode,string warehouseId,string areaCode)
		{
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			return f1912Repo.GetDcWarehouseChannelList(dcCode, warehouseId, areaCode);
		}
		#endregion

		#region P710104 - 儲位管制
		[WebGet]
		public IQueryable<F1912StatusEx> GetLocListForLocControl(string dcCode, string gupCode, string custCode
			, string warehouseType, string warehouseId, string areaId, string channel, string itemCode, string account)
		{
			var srv = new P710104Service();
			// 取得儲位資訊
			var result = srv.GetLocListForLocControl(dcCode, gupCode, custCode, warehouseType, warehouseId, areaId, channel, itemCode, account);
			return result;
		}
		[WebGet]
		public IQueryable<F1912StatusEx2> GetLocListForLocControlByItemCode(string dcCode, string gupCode, string custCode
			, string itemCode, string account)
		{
			var srv = new P710104Service();
			// 取得儲位資訊
			var result = srv.GetLocListForLocControlByItemCode(dcCode, gupCode, custCode, itemCode, account);
			return result;
		}
		[WebGet]
		public IQueryable<F1912StatisticReport> GetLocStatisticForLocControl(string dcCode, string gupCode, string custCode
		  , string warehouseType, string warehouseId, string account)
		{
			var srv = new P710107Service();
			// 取得儲位資訊
			var result = srv.GetLocStatisticForLocControl(dcCode, gupCode, custCode, warehouseType, warehouseId, account);
			return result;
		}
		#endregion

		#region P7101010000 倉別維護
		[WebGet]
		public IQueryable<F1980Data> GetF1980Datas(string dcCode, string gupCode, string custCode, string typeId, string account)
		{
			var srv = new P710101Service();
			return srv.GetF1980Datas(dcCode, gupCode, custCode, typeId, account);
		}
		#endregion

		#region P710106
		[WebGet]
		public IQueryable<F191202Ex> GetLocTransactionLog(string dcCode, string gupCode, string custCode
		  , string locCode, string startDt, string endDt, string locStatus, string warehouseType, string account)
		{
			var srv = new P710106Service();
			// 取得儲位資訊
			var result = srv.GetLocTransactionLog(dcCode, gupCode, custCode, locCode, startDt, endDt, locStatus, warehouseType, account);
			return result;
		}
		#endregion

		#region P7101050000 料架基本資料維護
		[WebGet]
		public ExecuteResult Delete710105(string locTypeId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710105Service(wmsTransaction);
			var result = srv.DeleteF1942(locTypeId);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		#endregion


		#region P7101020000 儲區維護

		[WebGet]
		public IQueryable<F1919Data> GetF1919Datas(string dcCode, string gupCode, string custCode, string warehourseId, string areaCode)
		{
			var srv = new P710102Service();
			return srv.GetF1919Datas(dcCode, gupCode, custCode, warehourseId, areaCode);
		}

		#endregion

		#region P7102010000 進貨檢驗

		#endregion

		#region P7109010000 配送商主檔維護
		[WebGet]
		public IQueryable<F1947Ex> GetF1947ExQuery(string dcCode, string gupCode, string custCode, string allID, string allComp)
		{
			var srv = new P710901Service();
			return srv.GetF1947ExQuery(dcCode, gupCode, custCode, allID, allComp);
		}

		[WebGet]
		public IQueryable<ExecuteResult> DeleteF1947(string dcCode, string allID)
		{
			var srv = new P710901Service();
			return new List<ExecuteResult>() { srv.DeleteF1947(dcCode, allID) }.AsQueryable();
		}

		[WebGet]
		public IQueryable<F194701WithF1934> GetF194701WithF1934s(string dcCode, string allID)
		{
			var f194701Repo = new F194701Repository(Schemas.CoreSchema);
			return f194701Repo.GetF194701WithF1934s(dcCode, allID);
		}
		#endregion

		#region P7110010000 貨主單據維護
		/// <summary>
		/// 取得貨主單據維護的查詢結果
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F190001Data> GetF190001Data(string dcCode, string gupCode, string custCode, string ticketType)
		{
			var srv = new P711001Service();
			return srv.GetF190001Data(dcCode, gupCode, custCode, ticketType);
		}
		#endregion

		#region P7110030000 出貨單批次參數維護
		/// <summary>
		/// 取得出貨單批次參數維護的查詢結果
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F050004WithF190001> GetF050004WithF190001s(string dcCode, string gupCode, string custCode)
		{
			var srv = new P711003Service();
			return srv.GetF050004WithF190001s(dcCode, gupCode, custCode);
		}

		#endregion

		#region P7110020000 貨主單據倉別維護
		/// <summary>
		/// 取得貨主單據倉別維護的查詢結果
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F190002Data> GetF190002Data(string dcCode, string gupCode, string custCode)
		{
			var srv = new P711002Service();
			return srv.GetF190002Data(dcCode, gupCode, custCode);
		}
		#endregion

		#region P7109030000 貨主主檔維護

		/// <summary>
		/// 檢查統編是否已存在
		/// </summary>
		/// <param name="uniForm"></param>
		/// <returns></returns>
		[WebGet]
		public bool ExistsUniForm(string uniForm)
		{
			var srv = new Shared.Services.SharedService();
			return srv.ExistsUniForm(uniForm);
		}

		/// <summary>
		/// 建立預設的貨主單據(所有不同的物流中心 x (固定業主、貨主) x 單據類型 x 單據類別 的所有里程碑)
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		[WebGet]
		public ExecuteResult CreateDefaultTicketMilestoneNo(string gupCode, string custCode)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710903Service(wmsTransaction);
			var result = srv.CreateDefaultTicketMilestoneNo(gupCode, custCode);
			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		/// <summary>
		/// 配送商服務貨主維護
		/// </summary>
		/// <param name="dcCode"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F194704Data> GetF194704Datas(string dcCode, string gupCode, string custCode)
		{
			var f194704Repo = new F194704Repository(Schemas.CoreSchema);
			return f194704Repo.GetF194704Datas(dcCode, gupCode, custCode);
		}

		[WebGet]
		public ExecuteResult DeleteP710903(string gupCode, string custCode)
		{
			var _wmsTransaction = new WmsTransaction();
			var srv = new P710903Service(_wmsTransaction);
			var result = srv.DeleteP710903(gupCode, custCode);
			if (result.IsSuccessed)
				_wmsTransaction.Complete();
			return result;
		}

		#region 訂單處理進度查詢
		[WebGet]
		public IQueryable<F051201Progress> GetOrderProcessProgress(string dcCode, string gupCode, string custCode, string pickTime, string delvDate)
		{
			var srv = new P710808Service();
			return srv.GetOrderProcessProgress(dcCode, gupCode, custCode, pickTime, delvDate);
		}
		[WebGet]
		public IQueryable<F050301ProgressData> GetProgressData(string dcCode, string gupCode, string custCode, string pickTime, string delvDate, string pickOrdNo)
		{
			var srv = new P710808Service();
			return srv.GetProgressData(dcCode, gupCode, custCode, pickTime, delvDate, pickOrdNo);
		}
		#endregion

		#region P7105020000 作業計價
		[WebGet]
		public IQueryable<F91000301Data> GetAccItemKinds(string itemTypeId)
		{
			var f91000301Repo = new F91000301Repository(Schemas.CoreSchema);
			return f91000301Repo.GetAccItemKinds(itemTypeId);
		}

		[WebGet]
		public IQueryable<F000904DelvAccType> GetDelvAccTypes(string itemTypeId, string accItemKindId)
		{
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			return f000904Repo.GetDelvAccTypes(itemTypeId, accItemKindId);
		}
		[WebGet]
		public ExecuteResult DeleteP7105020000(string dcCode, string accItemKindId, string ordType, string accKind, string accUnit, string delvAccType)
		{
			var _wmsTransaction = new WmsTransaction();
			var srv = new P710502Service(_wmsTransaction);
			var result = srv.DeleteP7105020000(dcCode, accItemKindId, ordType, accKind, accUnit,  delvAccType);
			if (result.IsSuccessed)
				_wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region GetF199001Exs
		[WebGet]
		public IQueryable<F199001Ex> GetF199001Exs(string dcCode, string locTypeID, string tmprType, string status)
		{
			var srv = new P710501Service();
			var result = srv.GetF199001Exs(dcCode, locTypeID, tmprType, status);
			return result;
		}
		#endregion

		#region P7105030000 出貨計價設定
		[WebGet]
		public IQueryable<F199003Data> GetShippingValuation(string dcCode, string accItemKindId, string accKind, string status)
		{
			var repo = new F199003Repository(Schemas.CoreSchema);
			return repo.GetShippingValuation(dcCode, accItemKindId, accKind, status);
		}

		[WebGet]
		public ExecuteResult DeleteP7105030000(string dcCode, string accItemKindId, string accKind, string accUnit, string delvAccType)
		{
			var _wmsTransaction = new WmsTransaction();
			var srv = new P710503Service(_wmsTransaction);
			var result = srv.DeleteP7105030000(dcCode, accItemKindId, accKind, accUnit,  delvAccType);
			if (result.IsSuccessed)
				_wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region "GetF194701WithF1934s"
		[WebGet]
		public IQueryable<NewF194701WithF1934> GetNewF194701WithF1934s(string dcCode, string allID)
		{
			var f194701Repo = new F194701Repository(Schemas.CoreSchema);
			return f194701Repo.GetNewF194701WithF1934s(dcCode, allID);
		}
		#endregion


		[WebGet]
		public IQueryable<F194707Ex> GetP710507SearchData(string dcCode, string allId, string accKind,
		  string inTax, string logiType, string custType, string status)
		{
			var srv = new P710507Service();
			return srv.GetP710507SearchData(dcCode, allId, accKind,
			  inTax, logiType, custType, status);
		}

		[WebGet]
		public IQueryable<InventoryQueryData> GetInventoryQueryDatas(string dcCode, string gupCode, string custCode,
		  string postingDateBegin, string postingDateEnd)
		{
			var srv = new P710810Service();
			return srv.GetInventoryQueryDatas(dcCode, gupCode, custCode, postingDateBegin, postingDateEnd);
		}

		#region F700701 查詢
		[WebGet]
		public IQueryable<F700701QueryData> GetF700701QueryData(string dcCode, string importSDate, string importEDate)
		{
			var srv = new P710706Service();
			var result = srv.GetF700701QueryData(dcCode, importSDate, importEDate);
			return result;
		}
		#endregion

		#region 進貨狀況控管
		[WebGet]
		public IQueryable<DcWmsNoStatusItem> GetReceProcessOver30MinDatasByDc(string dcCode, string receDate)
		{
			var srv = new P710403Service();
			return srv.GetReceProcessOver30MinDatasByDc(dcCode, DateTime.Parse(receDate));
		}
		[WebGet]
		public IQueryable<DcWmsNoStatusItem> GetReceUnUpLocOver30MinDatasByDc(string dcCode, string receDate)
		{
			var srv = new P710403Service();
			return srv.GetReceUnUpLocOver30MinDatasByDc(dcCode, DateTime.Parse(receDate));

		}

		#endregion

		#region 退貨狀況控管
		[WebGet]
		public IQueryable<DcWmsNoStatusItem> GetReturnProcessOver30MinByDc(string dcCode, string returnDate)
		{
			var srv = new P710403Service();
			return srv.GetReturnProcessOver30MinByDc(dcCode, DateTime.Parse(returnDate));
		}
		[WebGet]
		public IQueryable<DcWmsNoStatusItem> GetReturnWaitUpLocOver30MinByDc(string dcCode, string rtnApplyDate)
		{
			var srv = new P710403Service();
			return srv.GetReturnWaitUpLocOver30MinByDc(dcCode, DateTime.Parse(rtnApplyDate));
		}

		[WebGet]
		public IQueryable<DcWmsNoStatusItem> GetReturnNoHelpByDc(string dcCode, string returnDate)
		{
			var srv = new P710403Service();
			return srv.GetReturnNoHelpByDc(dcCode, DateTime.Parse(returnDate));
		}

		#endregion

		#region 流動加工狀況控管
		[WebGet]
		public IQueryable<ProduceLineStatusItem> GetProduceLineStatusItems(string dcCode, string finishDate)
		{
			var srv = new P710403Service();
			return srv.GetProduceLineStatusItems(dcCode, DateTime.Parse(finishDate));
		}
		[WebGet]
		public IQueryable<DcWmsNoStatusItem> GetWorkProcessOverFinishTimeByDc(string dcCode, string finishDate)
		{
			var srv = new P710403Service();
			return srv.GetWorkProcessOverFinishTimeByDc(dcCode, DateTime.Parse(finishDate));
		}

		#endregion

		#region 物流中心看板一
		[WebGet]
		public IQueryable<DcWmsNoOrdPropItem> GeDcWmsNoOrdPropItemsByA(string dcCode, string stockDate)
		{
			var srv = new P710401Service();
			return srv.GeDcWmsNoOrdPropItemsByA(dcCode, DateTime.Parse(stockDate));
		}
		[WebGet]
		public IQueryable<DcWmsNoOrdPropItem> GeDcWmsNoOrdPropItemsByR(string dcCode, string returnDate)
		{
			var srv = new P710401Service();
			return srv.GeDcWmsNoOrdPropItemsByR(dcCode, DateTime.Parse(returnDate));
		}
		[WebGet]
		public IQueryable<DcWmsNoOrdPropItem> GeDcWmsNoOrdPropItemsByT(string dcCode, string allocationDate)
		{
			var srv = new P710401Service();
			return srv.GeDcWmsNoOrdPropItemsByT(dcCode, DateTime.Parse(allocationDate));

		}
		[WebGet]
		public IQueryable<DcWmsNoOrdPropItem> GeDcWmsNoOrdPropItemsByO(string dcCode, string delvDate)
		{
			var srv = new P710401Service();
			return srv.GeDcWmsNoOrdPropItemsByO(dcCode, DateTime.Parse(delvDate));

		}

		#endregion

		#region 物流中心看板二
		[WebGet]
		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItemsByA(string dcCode, string gupCode, string custCode,
			string begStockDate, string endStockDate)
		{
			var srv = new P710402Service();
			return srv.GetDcWmsNoDateItemsByA(dcCode, gupCode, custCode, DateTime.Parse(begStockDate),
				DateTime.Parse(endStockDate));
		}

		[WebGet]
		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItemsByS(string dcCode, string gupCode, string custCode,
			string begOrdDate, string endOrdDate)
		{
			var srv = new P710402Service();
			return srv.GetDcWmsNoDateItemsByS(dcCode, gupCode, custCode, DateTime.Parse(begOrdDate),
				DateTime.Parse(endOrdDate));

		}

		[WebGet]
		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItemsByR(string dcCode, string gupCode, string custCode,
			string begReturnDate, string endReturnDate)
		{
			var srv = new P710402Service();
			return srv.GetDcWmsNoDateItemsByR(dcCode, gupCode, custCode, DateTime.Parse(begReturnDate),
				DateTime.Parse(endReturnDate));
		}

		[WebGet]
		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItemsByW(string dcCode, string gupCode, string custCode,
			string begFinishDate, string endFinishDate)
		{
			var srv = new P710402Service();
			return srv.GetDcWmsNoDateItemsByW(dcCode, gupCode, custCode, DateTime.Parse(begFinishDate),
				DateTime.Parse(endFinishDate));
		}

		[WebGet]
		public IQueryable<DcWmsNoLocTypeItem> GetDcWmsNoLocTypeItems(string dcCode, string gupCode, string custCode)
		{
			var srv = new P710402Service();
			return srv.GetDcWmsNoLocTypeItems(dcCode, gupCode, custCode);
		}

		#endregion

		#region P710705 倉別管理報表
		[WebGet]
		public IQueryable<P710705BackWarehouseInventory> GetP710705BackWarehouseInventory(string dcCode, string gupCode, string custCode, string vnrCode, string account)
		{
			var service = new P710705Service();
			return service.GetP710705BackWarehouseInventory(dcCode, gupCode, custCode, vnrCode, account);
		}

		[WebGet]
		public IQueryable<P710705MergeExecution> GetP710705MergeExecution(string dcCode, int? qty)
		{
			var repo = new F1913Repository(Schemas.CoreSchema);
			return repo.GetP710705LocMergeExecution( dcCode, qty);
		}

		[WebGet]
		public IQueryable<P710705Availability> GetP710705Availability(string dcCode, string gupCode, string custCode, DateTime? inventoryDate, string account)
		{
			var service = new P710705Service();
			return service.GetP710705Availability(dcCode, gupCode, custCode, inventoryDate.HasValue ? inventoryDate.Value.ToString("yyyy/MM/dd") : string.Empty, account);
		}

		[WebGet]
		public IQueryable<P710705ChangeDetail> GetP710705ChangeDetail(string warehouseId, string srcLocCode, string tarLocCode, string itemCodes, DateTime? enterDateBegin, DateTime? enterDateEnd)
		{
			var service = new P710705Service();
			return service.GetP710705ChangeDetail(warehouseId, srcLocCode, tarLocCode, itemCodes, enterDateBegin, enterDateEnd);
		}

		[WebGet]
		public IQueryable<P710705WarehouseDetail> GetP710705WarehouseDetail(string gupCode, string custCode, string warehouseId, string srcLocCode, string tarLocCode, string itemCode, string account)
		{
			var service = new P710705Service();
			return service.GetP710705WarehouseDetail(gupCode, custCode, warehouseId, srcLocCode, tarLocCode, itemCode, account);
		}


		#endregion
		
		#region P710702 對帳報表
		[WebGet]
		public IQueryable<F700101DistrCarData> GetDistrCarDatas(string dcCode, string gupCode, string custCode, 
																DateTime? take_SDate, DateTime? take_EDate, string allId)
		{
			var srv = new P710702Service();
			return srv.GetDistrCarDatas(dcCode, gupCode, custCode, take_SDate, take_EDate, allId);
		}
		[WebGet]
		public IQueryable<F910201ProcessData> GetProcessDatas(string dcCode, string gupCode, string custCode,
			string crtSDate, string crtEDate, string outSourceId)
		{
			var srv = new P710702Service();
			DateTime tmpDate;
			DateTime? crt_SDate = (DateTime.TryParse(crtSDate, out tmpDate)) ? (DateTime?)tmpDate.Date : (DateTime?)null;
			DateTime? crt_EDate = (DateTime.TryParse(crtEDate, out tmpDate)) ? (DateTime?)tmpDate.Date.AddDays(1) : (DateTime?)null;
			return srv.GetProcessDatas(dcCode, gupCode, custCode, crt_SDate, crt_EDate, outSourceId);
		}

		[WebGet]
		public IQueryable<F51ComplexReportData> GetF51ComplexReportData(string dcCode, string calSDate, string calEDate,
			string gupCode, string custCode, string allId)
		{
			var srv = new P710702Service();
			DateTime tmpDate;
			DateTime? crt_SDate = (DateTime.TryParse(calSDate, out tmpDate)) ? (DateTime?)tmpDate.Date : (DateTime?)null;
			DateTime? crt_EDate = (DateTime.TryParse(calEDate, out tmpDate)) ? (DateTime?)tmpDate.Date.AddDays(1) : (DateTime?)null;
			return srv.GetF51ComplexReportData(dcCode, crt_SDate, crt_EDate, gupCode, custCode, allId);
		}

		#endregion

		#region P710601
		[WebGet]
		public IQueryable<P710601LangData> GetP710601LangDataList(string topic, string subtopic, string lang)
		{
			var srv = new P710601Service();
			return srv.GetP710601LangData(topic, subtopic, lang);
		}
        #endregion

        [WebGet]
        public IQueryable<F020201Data> GetDatasByWaitOrUpLoc(string dcCode, string receDate)
        {
            var srv = new P710403Service();
            return srv.GetDatasByWaitOrUpLoc(dcCode, receDate);
            //var f020201Repo = new F020201Repository(Schemas.CoreSchema);
            //return f020201Repo.GetDatasByWaitOrUpLoc(dcCode, DateTime.Parse(receDate));
        }
    }
}
