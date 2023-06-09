using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.ExDataServices.P71ExDataService
{
	public partial class P71ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{

		public IQueryable<F1912WithF1980> GetLocListForWarehouse(String dcCode, String warehouseId, String gupCode, String custCode, String warehouseType, String areaCode, String locCodeS, String locCodeE, String account)
		{
			return CreateQuery<F1912WithF1980>("GetLocListForWarehouse")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("warehouseId", warehouseId)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("warehouseType", warehouseType)
						.AddQueryExOption("areaCode", areaCode)
						.AddQueryExOption("locCodeS", locCodeS)
						.AddQueryExOption("locCodeE", locCodeE)
						.AddQueryExOption("account", account);
		}

		public IQueryable<F1912StatusEx> GetLocListForLocControl(String dcCode, String gupCode, String custCode, String warehouseType, String warehouseId, String areaId, String itemCode, String account)
		{
			return CreateQuery<F1912StatusEx>("GetLocListForLocControl")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("warehouseType", warehouseType)
						.AddQueryExOption("warehouseId", warehouseId)
						.AddQueryExOption("areaId", areaId)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("account", account);
		}

		public IQueryable<F1912StatusEx2> GetLocListForLocControlByItemCode(String dcCode, String gupCode, String custCode, String itemCode, String account)
		{
			return CreateQuery<F1912StatusEx2>("GetLocListForLocControlByItemCode")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("account", account);
		}

		public IQueryable<F1912StatisticReport> GetLocStatisticForLocControl(String dcCode, String gupCode, String custCode, String warehouseType, String warehouseId, String account)
		{
			return CreateQuery<F1912StatisticReport>("GetLocStatisticForLocControl")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("warehouseType", warehouseType)
						.AddQueryExOption("warehouseId", warehouseId)
						.AddQueryExOption("account", account);
		}

		public IQueryable<F1980Data> GetF1980Datas(String dcCode, String gupCode, String custCode, String warehourseId, String account)
		{
			return CreateQuery<F1980Data>("GetF1980Datas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("warehourseId", warehourseId)
						.AddQueryExOption("account", account);
		}

		public IQueryable<F191202Ex> GetLocTransactionLog(String dcCode, String gupCode, String custCode, String locCode, String startDt, String endDt, String locStatus, String warehouseType, String account)
		{
			return CreateQuery<F191202Ex>("GetLocTransactionLog")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("locCode", locCode)
						.AddQueryExOption("startDt", startDt)
						.AddQueryExOption("endDt", endDt)
						.AddQueryExOption("locStatus", locStatus)
						.AddQueryExOption("warehouseType", warehouseType)
						.AddQueryExOption("account", account);
		}

		public ExecuteResult Delete710105(String locTypeId)
		{
			return CreateQuery<ExecuteResult>("Delete710105")
						.AddQueryExOption("locTypeId", locTypeId).ToList().FirstOrDefault();
		}

		public IQueryable<F1919Data> GetF1919Datas(String dcCode, String gupCode, String custCode, String warehourseId, String areaCode)
		{
			return CreateQuery<F1919Data>("GetF1919Datas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("warehourseId", warehourseId)
						.AddQueryExOption("areaCode", areaCode);
		}

		public IQueryable<F1947Ex> GetF1947ExQuery(String dcCode, String gupCode, String custCode, String allID, String allComp)
		{
			return CreateQuery<F1947Ex>("GetF1947ExQuery")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allID", allID)
						.AddQueryExOption("allComp", allComp);
		}

		public IQueryable<ExecuteResult> DeleteF1947(String dcCode, String allID)
		{
			return CreateQuery<ExecuteResult>("DeleteF1947")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("allID", allID);
		}

		public IQueryable<F194701WithF1934> GetF194701WithF1934s(String dcCode, String allID)
		{
			return CreateQuery<F194701WithF1934>("GetF194701WithF1934s")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("allID", allID);
		}

		public IQueryable<F190001Data> GetF190001Data(String dcCode, String gupCode, String custCode, String ticketType)
		{
			return CreateQuery<F190001Data>("GetF190001Data")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("ticketType", ticketType);
		}

		public IQueryable<F050003Ex> GetF050003Exs(String dcCode, String gupCode, String custCode, String ticketId, String itemCode, String beginDelvDate, String endDelvDate)
		{
			return CreateQuery<F050003Ex>("GetF050003Exs")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("ticketId", ticketId)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("beginDelvDate", beginDelvDate)
						.AddQueryExOption("endDelvDate", endDelvDate);
		}

		public IQueryable<F05000301Ex> GetF05000301Exs(Int32? seqNo)
		{
			return CreateQuery<F05000301Ex>("GetF05000301Exs")
						.AddQueryExOption("seqNo", seqNo);
		}

		public IQueryable<F050004WithF190001> GetF050004WithF190001s(String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<F050004WithF190001>("GetF050004WithF190001s")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}

		public IQueryable<F190002Data> GetF190002Data(String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<F190002Data>("GetF190002Data")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}

		public Boolean ExistsUniForm(String uniForm)
		{
			return CreateQuery<Boolean>("ExistsUniForm")
						.AddQueryExOption("uniForm", uniForm).ToList().FirstOrDefault();
		}

		public ExecuteResult CreateDefaultTicketMilestoneNo(String gupCode, String custCode)
		{
			return CreateQuery<ExecuteResult>("CreateDefaultTicketMilestoneNo")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode).ToList().FirstOrDefault();
		}

		public IQueryable<F194704Data> GetF194704Datas(String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<F194704Data>("GetF194704Datas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}

		public ExecuteResult DeleteP710903(String gupCode, String custCode)
		{
			return CreateQuery<ExecuteResult>("DeleteP710903")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode).ToList().FirstOrDefault();
		}

		public IQueryable<F051201Progress> GetOrderProcessProgress(String dcCode, String gupCode, String custCode, String pickTime, String delvDate)
		{
			return CreateQuery<F051201Progress>("GetOrderProcessProgress")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("delvDate", delvDate);
		}

		public IQueryable<F050301ProgressData> GetProgressData(String dcCode, String gupCode, String custCode, String pickTime, String delvDate, String pickOrdNo)
		{
			return CreateQuery<F050301ProgressData>("GetProgressData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickOrdNo", pickOrdNo);
		}

		public IQueryable<F91000301Data> GetAccItemKinds(String itemTypeId)
		{
			return CreateQuery<F91000301Data>("GetAccItemKinds")
						.AddQueryExOption("itemTypeId", itemTypeId);
		}

		public IQueryable<F000904DelvAccType> GetDelvAccTypes(String itemTypeId, String accItemKindId)
		{
			return CreateQuery<F000904DelvAccType>("GetDelvAccTypes")
						.AddQueryExOption("itemTypeId", itemTypeId)
						.AddQueryExOption("accItemKindId", accItemKindId);
		}

		public ExecuteResult DeleteP7105020000(String dcCode, String accItemKindId, String ordType, String accKind, String accUnit, String delvAccType)
		{
			return CreateQuery<ExecuteResult>("DeleteP7105020000")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("accItemKindId", accItemKindId)
						.AddQueryExOption("ordType", ordType)
						.AddQueryExOption("accKind", accKind)
						.AddQueryExOption("accUnit", accUnit)
						.AddQueryExOption("delvAccType", delvAccType).ToList().FirstOrDefault();
		}

		public IQueryable<F199001Ex> GetF199001Exs(String dcCode, String locTypeID, String tmprType, String status)
		{
			return CreateQuery<F199001Ex>("GetF199001Exs")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("locTypeID", locTypeID)
						.AddQueryExOption("tmprType", tmprType)
						.AddQueryExOption("status", status);
		}

		public IQueryable<F199003Data> GetShippingValuation(String dcCode, String accItemKindId, String accKind, String status)
		{
			return CreateQuery<F199003Data>("GetShippingValuation")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("accItemKindId", accItemKindId)
						.AddQueryExOption("accKind", accKind)
						.AddQueryExOption("status", status);
		}

		public ExecuteResult DeleteP7105030000(String dcCode, String accItemKindId, String accKind, String accUnit, String delvAccType)
		{
			return CreateQuery<ExecuteResult>("DeleteP7105030000")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("accItemKindId", accItemKindId)
						.AddQueryExOption("accKind", accKind)
						.AddQueryExOption("accUnit", accUnit)
						.AddQueryExOption("delvAccType", delvAccType).ToList().FirstOrDefault();
		}

		public IQueryable<NewF194701WithF1934> GetNewF194701WithF1934s(String dcCode, String allID)
		{
			return CreateQuery<NewF194701WithF1934>("GetNewF194701WithF1934s")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("allID", allID);
		}

		public IQueryable<F194707Ex> GetP710507SearchData(String dcCode, String allId, String accKind, String inTax, String logiType, String custType, String status)
		{
			return CreateQuery<F194707Ex>("GetP710507SearchData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("allId", allId)
						.AddQueryExOption("accKind", accKind)
						.AddQueryExOption("inTax", inTax)
						.AddQueryExOption("logiType", logiType)
						.AddQueryExOption("custType", custType)
						.AddQueryExOption("status", status);
		}

		public IQueryable<InventoryQueryData> GetInventoryQueryDatas(String dcCode, String gupCode, String custCode, String postingDateBegin, String postingDateEnd)
		{
			return CreateQuery<InventoryQueryData>("GetInventoryQueryDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("postingDateBegin", postingDateBegin)
						.AddQueryExOption("postingDateEnd", postingDateEnd);
		}

		public IQueryable<F700701QueryData> GetF700701QueryData(String dcCode, String importSDate, String importEDate)
		{
			return CreateQuery<F700701QueryData>("GetF700701QueryData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("importSDate", importSDate)
						.AddQueryExOption("importEDate", importEDate);
		}

		public IQueryable<F0010AbnormalData> GetAbnormalDatas(String dcCode, String gupCode, String custCode, String crtSDate, String crtEDate)
		{
			return CreateQuery<F0010AbnormalData>("GetAbnormalDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("crtSDate", crtSDate)
						.AddQueryExOption("crtEDate", crtEDate);
		}

		public IQueryable<F700101DeliveryFailureData> GetDeliveryFailureDatas(String dcCode, String gupCode, String custCode, String takeSDate, String takeEDate, String allId)
		{
			return CreateQuery<F700101DeliveryFailureData>("GetDeliveryFailureDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("takeSDate", takeSDate)
						.AddQueryExOption("takeEDate", takeEDate)
						.AddQueryExOption("allId", allId);
		}

		public IQueryable<F700101DistributionRate> GetDistributionRateDatas(String dcCode, String gupCode, String custCode, String takeSDate, String takeEDate, String allId)
		{
			return CreateQuery<F700101DistributionRate>("GetDistributionRateDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("takeSDate", takeSDate)
						.AddQueryExOption("takeEDate", takeEDate)
						.AddQueryExOption("allId", allId);
		}

		public IQueryable<DcWmsNoStatusItem> GetReceProcessOver30MinDatasByDc(String dcCode, String receDate)
		{
			return CreateQuery<DcWmsNoStatusItem>("GetReceProcessOver30MinDatasByDc")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("receDate", receDate);
		}

		public IQueryable<DcWmsNoStatusItem> GetReceUnUpLocOver30MinDatasByDc(String dcCode, String receDate)
		{
			return CreateQuery<DcWmsNoStatusItem>("GetReceUnUpLocOver30MinDatasByDc")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("receDate", receDate);
		}

		public IQueryable<DcWmsNoStatusItem> GetReturnProcessOver30MinByDc(String dcCode, String returnDate)
		{
			return CreateQuery<DcWmsNoStatusItem>("GetReturnProcessOver30MinByDc")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("returnDate", returnDate);
		}

		public IQueryable<DcWmsNoStatusItem> GetReturnWaitUpLocOver30MinByDc(String dcCode, String rtnApplyDate)
		{
			return CreateQuery<DcWmsNoStatusItem>("GetReturnWaitUpLocOver30MinByDc")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("rtnApplyDate", rtnApplyDate);
		}

		public IQueryable<DcWmsNoStatusItem> GetReturnNoHelpByDc(String dcCode, String returnDate)
		{
			return CreateQuery<DcWmsNoStatusItem>("GetReturnNoHelpByDc")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("returnDate", returnDate);
		}

		public IQueryable<ProduceLineStatusItem> GetProduceLineStatusItems(String dcCode, String finishDate)
		{
			return CreateQuery<ProduceLineStatusItem>("GetProduceLineStatusItems")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("finishDate", finishDate);
		}

		public IQueryable<DcWmsNoStatusItem> GetWorkProcessOverFinishTimeByDc(String dcCode, String finishDate)
		{
			return CreateQuery<DcWmsNoStatusItem>("GetWorkProcessOverFinishTimeByDc")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("finishDate", finishDate);
		}

		public IQueryable<DcWmsNoOrdPropItem> GeDcWmsNoOrdPropItemsByA(String dcCode, String stockDate)
		{
			return CreateQuery<DcWmsNoOrdPropItem>("GeDcWmsNoOrdPropItemsByA")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("stockDate", stockDate);
		}

		public IQueryable<DcWmsNoOrdPropItem> GeDcWmsNoOrdPropItemsByR(String dcCode, String returnDate)
		{
			return CreateQuery<DcWmsNoOrdPropItem>("GeDcWmsNoOrdPropItemsByR")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("returnDate", returnDate);
		}

		public IQueryable<DcWmsNoOrdPropItem> GeDcWmsNoOrdPropItemsByT(String dcCode, String allocationDate)
		{
			return CreateQuery<DcWmsNoOrdPropItem>("GeDcWmsNoOrdPropItemsByT")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("allocationDate", allocationDate);
		}

		public IQueryable<DcWmsNoOrdPropItem> GeDcWmsNoOrdPropItemsByO(String dcCode, String delvDate)
		{
			return CreateQuery<DcWmsNoOrdPropItem>("GeDcWmsNoOrdPropItemsByO")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("delvDate", delvDate);
		}

		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItemsByA(String dcCode, String gupCode, String custCode, String begStockDate, String endStockDate)
		{
			return CreateQuery<DcWmsNoDateItem>("GetDcWmsNoDateItemsByA")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("begStockDate", begStockDate)
						.AddQueryExOption("endStockDate", endStockDate);
		}

		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItemsByS(String dcCode, String gupCode, String custCode, String begOrdDate, String endOrdDate)
		{
			return CreateQuery<DcWmsNoDateItem>("GetDcWmsNoDateItemsByS")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("begOrdDate", begOrdDate)
						.AddQueryExOption("endOrdDate", endOrdDate);
		}

		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItemsByR(String dcCode, String gupCode, String custCode, String begReturnDate, String endReturnDate)
		{
			return CreateQuery<DcWmsNoDateItem>("GetDcWmsNoDateItemsByR")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("begReturnDate", begReturnDate)
						.AddQueryExOption("endReturnDate", endReturnDate);
		}

		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItemsByW(String dcCode, String gupCode, String custCode, String begFinishDate, String endFinishDate)
		{
			return CreateQuery<DcWmsNoDateItem>("GetDcWmsNoDateItemsByW")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("begFinishDate", begFinishDate)
						.AddQueryExOption("endFinishDate", endFinishDate);
		}

		public IQueryable<DcWmsNoLocTypeItem> GetDcWmsNoLocTypeItems(String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<DcWmsNoLocTypeItem>("GetDcWmsNoLocTypeItems")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}

		public IQueryable<P710705BackWarehouseInventory> GetP710705BackWarehouseInventory(String dcCode, String gupCode, String custCode, String vnrCode, String account)
		{
			return CreateQuery<P710705BackWarehouseInventory>("GetP710705BackWarehouseInventory")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("vnrCode", vnrCode)
						.AddQueryExOption("account", account);
		}

		public IQueryable<P710705MergeExecution> GetP710705MergeExecution(String dcCode, Int32? qty)
		{
			return CreateQuery<P710705MergeExecution>("GetP710705MergeExecution")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("qty", qty);
		}

		public IQueryable<P710705Availability> GetP710705Availability(String dcCode, String gupCode, String custCode, DateTime? inventoryDate, String account)
		{
			return CreateQuery<P710705Availability>("GetP710705Availability")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("inventoryDate", inventoryDate)
						.AddQueryExOption("account", account);
		}

		public IQueryable<P710705ChangeDetail> GetP710705ChangeDetail(String warehouseId, String srcLocCode, String tarLocCode, String itemCodes, DateTime? enterDateBegin, DateTime? enterDateEnd)
		{
			return CreateQuery<P710705ChangeDetail>("GetP710705ChangeDetail")
						.AddQueryExOption("warehouseId", warehouseId)
						.AddQueryExOption("srcLocCode", srcLocCode)
						.AddQueryExOption("tarLocCode", tarLocCode)
						.AddQueryExOption("itemCodes", itemCodes)
						.AddQueryExOption("enterDateBegin", enterDateBegin)
						.AddQueryExOption("enterDateEnd", enterDateEnd);
		}

		public IQueryable<P710705WarehouseDetail> GetP710705WarehouseDetail(String gupCode, String custCode, String warehouseId, String srcLocCode, String tarLocCode, String itemCode, String account)
		{
			return CreateQuery<P710705WarehouseDetail>("GetP710705WarehouseDetail")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("warehouseId", warehouseId)
						.AddQueryExOption("srcLocCode", srcLocCode)
						.AddQueryExOption("tarLocCode", tarLocCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("account", account);
		}

		public IQueryable<F700101DistrCarData> GetDistrCarDatas(String dcCode, String gupCode, String custCode, DateTime? take_SDate, DateTime? take_EDate, String allId)
		{
			return CreateQuery<F700101DistrCarData>("GetDistrCarDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("take_SDate", take_SDate)
						.AddQueryExOption("take_EDate", take_EDate)
						.AddQueryExOption("allId", allId);
		}

		public IQueryable<F910201ProcessData> GetProcessDatas(String dcCode, String gupCode, String custCode, String crtSDate, String crtEDate, String outSourceId)
		{
			return CreateQuery<F910201ProcessData>("GetProcessDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("crtSDate", crtSDate)
						.AddQueryExOption("crtEDate", crtEDate)
						.AddQueryExOption("outSourceId", outSourceId);
		}

		public IQueryable<F51ComplexReportData> GetF51ComplexReportData(String dcCode, String calSDate, String calEDate, String gupCode, String custCode, String allId)
		{
			return CreateQuery<F51ComplexReportData>("GetF51ComplexReportData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("calSDate", calSDate)
						.AddQueryExOption("calEDate", calEDate)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allId", allId);
		}
	}
}

