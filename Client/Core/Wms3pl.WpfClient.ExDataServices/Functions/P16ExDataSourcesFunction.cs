using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.ExDataServices.P16ExDataService
{
	public partial class P16ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{

		public IQueryable<F161201DetailDatas> GetF161201DetailDatas(String dcCode, String gupCode, String custCode, String returnNo)
		{
			return CreateQuery<F161201DetailDatas>("GetF161201DetailDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnNo", returnNo);
		}

		public IQueryable<F161201DetailDatas> GetItemsByWmsOrdNo(String dcCode, String gupCode, String custCode, String wmsOrdNo)
		{
			return CreateQuery<F161201DetailDatas>("GetItemsByWmsOrdNo")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo);
		}

		public IQueryable<ExecuteResult> DeleteP160101(String returnNo, String gupCode, String custCode, String dcCode)
		{
			return CreateQuery<ExecuteResult>("DeleteP160101")
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode);
		}

		public IQueryable<F161601DetailDatas> GetF161601DetailDatas(String dcCode, String gupCode, String custCode, String rtnApplyNo)
		{
			return CreateQuery<F161601DetailDatas>("GetF161601DetailDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("rtnApplyNo", rtnApplyNo);
		}

		public IQueryable<F161401ReturnWarehouse> GetF161401ReturnWarehouse(String dcCode, String gupCode, String custCode, String returnNo, String locCode, String itemCode, String itemName)
		{
			return CreateQuery<F161401ReturnWarehouse>("GetF161401ReturnWarehouse")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("locCode", locCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("itemName", itemName);
		}

		public IQueryable<PrintF161601Data> GetPrintF161601Data(String dcCode, String gupCode, String custCode, String rtnApplyNo)
		{
			return CreateQuery<PrintF161601Data>("GetPrintF161601Data")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("rtnApplyNo", rtnApplyNo);
		}

		public ExecuteResult DeleteP160102(String dcCode, String gupCode, String custCode, String rtnApplyNo)
		{
			return CreateQuery<ExecuteResult>("DeleteP160102")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("rtnApplyNo", rtnApplyNo).ToList().FirstOrDefault();
		}

		public IQueryable<F160201Data> GetF160201Datas(String dcCode, String gupCode, String custCode, String status, DateTime? createBeginDateTime, DateTime? createEndDateTime, String postingBeginDateTime, String postingEndDateTime, String returnNo, String custOrdNo, String vendorCode, String vendorName)
		{
			return CreateQuery<F160201Data>("GetF160201Datas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("status", status)
						.AddQueryExOption("createBeginDateTime", createBeginDateTime)
						.AddQueryExOption("createEndDateTime", createEndDateTime)
						.AddQueryExOption("postingBeginDateTime", postingBeginDateTime)
						.AddQueryExOption("postingEndDateTime", postingEndDateTime)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("custOrdNo", custOrdNo)
						.AddQueryExOption("vendorCode", vendorCode)
						.AddQueryExOption("vendorName", vendorName);
		}

		public IQueryable<F160201DataDetail> GetF160201DataDetails(String dcCode, String gupCode, String custCode, String returnNo)
		{
			return CreateQuery<F160201DataDetail>("GetF160201DataDetails")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnNo", returnNo);
		}

		public IQueryable<F160201ReturnDetail> GetF160201ReturnDetails(String dcCode, String gupCode, String custCode, String warehouseId, DateTime? enterDateBegin, DateTime? enterDateEnd, DateTime? validDateBegin, DateTime? validDateEnd, String locBegin, String locEnd, String itemCode, String itemName)
		{
			return CreateQuery<F160201ReturnDetail>("GetF160201ReturnDetails")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("warehouseId", warehouseId)
						.AddQueryExOption("enterDateBegin", enterDateBegin)
						.AddQueryExOption("enterDateEnd", enterDateEnd)
						.AddQueryExOption("validDateBegin", validDateBegin)
						.AddQueryExOption("validDateEnd", validDateEnd)
						.AddQueryExOption("locBegin", locBegin)
						.AddQueryExOption("locEnd", locEnd)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("itemName", itemName);
		}

		public IQueryable<F160201ReturnDetail> GetF160201ReturnDetailsForEdit(String dcCode, String gupCode, String custCode, String vendorCode, String returnNo)
		{
			return CreateQuery<F160201ReturnDetail>("GetF160201ReturnDetailsForEdit")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("vendorCode", vendorCode)
						.AddQueryExOption("returnNo", returnNo);
		}

		public IQueryable<F160201Data> GetF160201DatasNotFinish(String dcCode, String gupCode, String custCode, DateTime? createBeginDateTime, DateTime? createEndDateTime, DateTime? returnBeginDateTime, DateTime? returnEndDateTime, String selfTake, String returnNo, String returnType, String vendorCode, String vendorName)
		{
			return CreateQuery<F160201Data>("GetF160201DatasNotFinish")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("createBeginDateTime", createBeginDateTime)
						.AddQueryExOption("createEndDateTime", createEndDateTime)
						.AddQueryExOption("returnBeginDateTime", returnBeginDateTime)
						.AddQueryExOption("returnEndDateTime", returnEndDateTime)
						.AddQueryExOption("selfTake", selfTake)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("returnType", returnType)
						.AddQueryExOption("vendorCode", vendorCode)
						.AddQueryExOption("vendorName", vendorName);
		}

		public IQueryable<F160204Detail> ConvertToF160204Detail(String dcCode, String gupCode, String custCode, String returnNo)
		{
			return CreateQuery<F160204Detail>("ConvertToF160204Detail")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnNo", returnNo);
		}

		public IQueryable<F160204SearchResult> GetF160204SearchResult(String dcCode, String gupCode, String custCode, String createBeginDateTime, String createEndDateTime, String returnWmsNo, String returnVnrNo, String orderNo, String empId, String empName )
		{
			return CreateQuery<F160204SearchResult>("GetF160204SearchResult")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("createBeginDateTime", createBeginDateTime)
						.AddQueryExOption("createEndDateTime", createEndDateTime)
						.AddQueryExOption("returnWmsNo", returnWmsNo)
						.AddQueryExOption("returnVnrNo", returnVnrNo)
						.AddQueryExOption("orderNo", orderNo)
                        .AddQueryExOption("empId",empId)
                        .AddQueryExOption("empName",empName);
		}

		public IQueryable<F160204SearchResult> GetF160204SearchResultDetail(String dcCode, String gupCode, String custCode, String returnWmsNo)
		{
			return CreateQuery<F160204SearchResult>("GetF160204SearchResultDetail")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnWmsNo", returnWmsNo);
		}

		public ExecuteResult PrintP160102(String dcCode, String gupCode, String custCode, String rtnApplyNo)
		{
			return CreateQuery<ExecuteResult>("PrintP160102")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("rtnApplyNo", rtnApplyNo).ToList().FirstOrDefault();
		}

		public IQueryable<P160102Report> GetP160102Reports(String dcCode, String gupCode, String custCode, String rtnApplyNo)
		{
			return CreateQuery<P160102Report>("GetP160102Reports")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("rtnApplyNo", rtnApplyNo);
		}

		public IQueryable<F160402Data> GetF160402ScrapDetails(String dcCode, String gupCode, String custCode, String scrapNo)
		{
			return CreateQuery<F160402Data>("GetF160402ScrapDetails")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("scrapNo", scrapNo);
		}

		public IQueryable<F160402AddData> GetF160402AddScrapDetails(String dcCode, String gupCode, String custCode, String wareHouseId, String itemCode, String locCode, String itemName, String validDateStart, String validDateEnd)
		{
			return CreateQuery<F160402AddData>("GetF160402AddScrapDetails")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wareHouseId", wareHouseId)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("locCode", locCode)
						.AddQueryExOption("itemName", itemName)
						.AddQueryExOption("validDateStart", validDateStart)
						.AddQueryExOption("validDateEnd", validDateEnd);
		}

		public ExecuteResult DeleteP160401(String dcCode, String gupCode, String custCode, String scrapNo)
		{
			return CreateQuery<ExecuteResult>("DeleteP160401")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("scrapNo", scrapNo).ToList().FirstOrDefault();
		}

		public IQueryable<F160501Data> Get160501QueryData(String dcItem, String gupCode, String custCode, String destoryNo, String postingSDate, String postingEDate, String custOrdNo, String status, String ordNo, String crtSDate, String crtEDate)
		{
			return CreateQuery<F160501Data>("Get160501QueryData")
						.AddQueryExOption("dcItem", dcItem)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("destoryNo", destoryNo)
						.AddQueryExOption("postingSDate", postingSDate)
						.AddQueryExOption("postingEDate", postingEDate)
						.AddQueryExOption("custOrdNo", custOrdNo)
						.AddQueryExOption("status", status)
						.AddQueryExOption("ordNo", ordNo)
						.AddQueryExOption("crtSDate", crtSDate)
						.AddQueryExOption("crtEDate", crtEDate);
		}

		public IQueryable<F160502Data> Get160502DetailData(String destoryNo)
		{
			return CreateQuery<F160502Data>("Get160502DetailData")
						.AddQueryExOption("destoryNo", destoryNo);
		}

		public IQueryable<F160502Data> Get160504SerialData(String destoryNo)
		{
			return CreateQuery<F160502Data>("Get160504SerialData")
						.AddQueryExOption("destoryNo", destoryNo);
		}

		public IQueryable<F160501Status> GetF160501Status(String destoryNo)
		{
			return CreateQuery<F160501Status>("GetF160501Status")
						.AddQueryExOption("destoryNo", destoryNo);
		}

		public IQueryable<F160501FileData> GetDestoryNoFile(String destoryNo)
		{
			return CreateQuery<F160501FileData>("GetDestoryNoFile")
						.AddQueryExOption("destoryNo", destoryNo);
		}

		public IQueryable<F160501FileData> GetDestoryNoRelation(String destoryNo)
		{
			return CreateQuery<F160501FileData>("GetDestoryNoRelation")
						.AddQueryExOption("destoryNo", destoryNo);
		}

		public IQueryable<F161301Data> Get161301Data(String dcCode, String reciptSDate, String reciptEDate, String pastNo, String returnNo, String eanCode)
		{
			return CreateQuery<F161301Data>("Get161301Data")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("reciptSDate", reciptSDate)
						.AddQueryExOption("reciptEDate", reciptEDate)
						.AddQueryExOption("pastNo", pastNo)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("eanCode", eanCode);
		}

		public IQueryable<F161301Report> Get161301Report(String dcCode, String rtnCheckNo, String printer)
		{
			return CreateQuery<F161301Report>("Get161301Report")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("rtnCheckNo", rtnCheckNo)
						.AddQueryExOption("printer", printer);
		}

		public IQueryable<CustomerData> GetCustomerDatas(String dcCode, String gupCode, String custCode, DateTime? beginDelvDate, DateTime? endDelvDate, String retailCode, String custName)
		{
			return CreateQuery<CustomerData>("GetCustomerDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("beginDelvDate", beginDelvDate)
						.AddQueryExOption("endDelvDate", endDelvDate)
						.AddQueryExOption("retailCode", retailCode)
						.AddQueryExOption("custName", custName);
		}

		public IQueryable<P160201Report> GetP160201Reports(String dcCode, String gupCode, String custCode, DateTime? beginRtnVnrDate, DateTime? endRtnVnrDate, String rtnVnrNo, String status)
		{
			return CreateQuery<P160201Report>("GetP160201Reports")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("beginRtnVnrDate", beginRtnVnrDate)
						.AddQueryExOption("endRtnVnrDate", endRtnVnrDate)
						.AddQueryExOption("rtnVnrNo", rtnVnrNo)
						.AddQueryExOption("status", status);
		}

		public IQueryable<P17ReturnAuditReport> GetP17ReturnAuditReports(String dcCode, String gupCode, String custCode, DateTime? beginReturnDate, DateTime? endReturnDate, String returnNo, String status)
		{
			return CreateQuery<P17ReturnAuditReport>("GetP17ReturnAuditReports")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("beginReturnDate", beginReturnDate)
						.AddQueryExOption("endReturnDate", endReturnDate)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("status", status);
		}

		public IQueryable<RTO17840ReturnAuditReport> GetRTO17840ReturnAuditReports(String dcCode, String gupCode, String custCode, DateTime? beginReturnDate, DateTime? endReturnDate, String returnNo, String status)
		{
			return CreateQuery<RTO17840ReturnAuditReport>("GetRTO17840ReturnAuditReports")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("beginReturnDate", beginReturnDate)
						.AddQueryExOption("endReturnDate", endReturnDate)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("status", status);
		}

		public IQueryable<B2CReturnAuditReport> GetB2CReturnAuditReports(String dcCode, String gupCode, String custCode, DateTime? beginReturnDate, DateTime? endReturnDate, String returnNo, String status)
		{
			return CreateQuery<B2CReturnAuditReport>("GetB2CReturnAuditReports")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("beginReturnDate", beginReturnDate)
						.AddQueryExOption("endReturnDate", endReturnDate)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("status", status);
		}

		public IQueryable<P106ReturnNotMoveDetail> GetP106ReturnNotMoveDetails(String dcCode, String gupCode, String custCode, DateTime? beginReturnDate, DateTime? endReturnDate, String returnNo, String status)
		{
			return CreateQuery<P106ReturnNotMoveDetail>("GetP106ReturnNotMoveDetails")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("beginReturnDate", beginReturnDate)
						.AddQueryExOption("endReturnDate", endReturnDate)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("status", status);
		}

		public IQueryable<TxtFormatReturnDetail> GetTxtFormatReturnDetails(String dcCode, String gupCode, String custCode, DateTime? beginReturnDate, DateTime? endReturnDate, String returnNo, String status)
		{
			return CreateQuery<TxtFormatReturnDetail>("GetTxtFormatReturnDetails")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("beginReturnDate", beginReturnDate)
						.AddQueryExOption("endReturnDate", endReturnDate)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("status", status);
		}

		public IQueryable<ReturnSerailNoByType> GetReturnSerailNosByType(String dcCode, String gupCode, String custCode, DateTime? beginReturnDate, DateTime? endReturnDate, String returnNo, String status, String commaSeparatorTypes)
		{
			return CreateQuery<ReturnSerailNoByType>("GetReturnSerailNosByType")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("beginReturnDate", beginReturnDate)
						.AddQueryExOption("endReturnDate", endReturnDate)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("status", status)
						.AddQueryExOption("commaSeparatorTypes", commaSeparatorTypes);
		}

		public IQueryable<P015ForecastReturnDetail> GetP015ForecastReturnDetails(String dcCode, String gupCode, String custCode, DateTime? beginReturnDate, DateTime? endReturnDate, String returnNo, String status)
		{
			return CreateQuery<P015ForecastReturnDetail>("GetP015ForecastReturnDetails")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("beginReturnDate", beginReturnDate)
						.AddQueryExOption("endReturnDate", endReturnDate)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("status", status);
		}

		public IQueryable<F160502Data> GetF1913ScrapData(String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<F160502Data>("GetF1913ScrapData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}
	}
}

