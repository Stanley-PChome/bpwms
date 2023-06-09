using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.ExDataServices.P05ExDataService
{
	public partial class P05ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{

		public IQueryable<GetF050901CSV> GetF050901CSVData(String dcCode, String gupCode, String custCode, DateTime? begCrtDate, DateTime? endCrtDate)
		{
			return CreateQuery<GetF050901CSV>("GetF050901CSVData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("begCrtDate", begCrtDate)
						.AddQueryExOption("endCrtDate", endCrtDate);
		}

		public IQueryable<F051201Data> GetF051201DatasForB2B(String dcCode, String gupCode, String custCode, DateTime? delvDate, String isPrinted)
		{
			return CreateQuery<F051201Data>("GetF051201DatasForB2B")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("isPrinted", isPrinted);
		}

		public IQueryable<F051202Data> GetF051202DatasForB2B(String dcCode, String gupCode, String custCode, String delvDate, String pickTime)
		{
			return CreateQuery<F051202Data>("GetF051202DatasForB2B")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime);
		}

		public IQueryable<F051201ReportDataA> GetF051201ReportDataAsForB2B(String dcCode, String gupCode, String custCode, DateTime? delvDate, String pickTime, String pickOrdNo)
		{
			return CreateQuery<F051201ReportDataA>("GetF051201ReportDataAsForB2B")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("pickOrdNo", pickOrdNo);
		}

		public IQueryable<F051201ReportDataB> GetF051201ReportDataBsForB2B(String dcCode, String gupCode, String custCode, String delvDate, String pickTime, String pickOrdNo)
		{
			return CreateQuery<F051201ReportDataB>("GetF051201ReportDataBsForB2B")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("pickOrdNo", pickOrdNo);
		}

		public IQueryable<F051201SelectedData> GetF051201SelectedDatasForB2B(String dcCode, String gupCode, String custCode, String delvDate, String pickTime)
		{
			return CreateQuery<F051201SelectedData>("GetF051201SelectedDatasForB2B")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime);
		}

		public IQueryable<ExecuteResult> UpdateF051201ForB2B(String dcCode, String gupCode, String custCode, String delvDate, String pickTime, String userId, String userName)
		{
			return CreateQuery<ExecuteResult>("UpdateF051201ForB2B")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("userId", userId)
						.AddQueryExOption("userName", userName);
		}

		public IQueryable<F051201Data> GetF051201DatasForB2C(String dcCode, String gupCode, String custCode, DateTime? delvDate, String isPrinted)
		{
			return CreateQuery<F051201Data>("GetF051201DatasForB2C")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("isPrinted", isPrinted);
		}

		public IQueryable<F051202Data> GetF051202DatasForB2C(String dcCode, String gupCode, String custCode, String delvDate, String pickTime)
		{
			return CreateQuery<F051202Data>("GetF051202DatasForB2C")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime);
		}

		public IQueryable<F051201ReportDataA> GetF051201ReportDataAsForB2C(String dcCode, String gupCode, String custCode, DateTime? delvDate, String pickTime, String pickOrdNo)
		{
			return CreateQuery<F051201ReportDataA>("GetF051201ReportDataAsForB2C")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("pickOrdNo", pickOrdNo);
		}

		public IQueryable<F051201SelectedData> GetF051201SelectedDatasForB2C(String dcCode, String gupCode, String custCode, String delvDate, String pickTime)
		{
			return CreateQuery<F051201SelectedData>("GetF051201SelectedDatasForB2C")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime);
		}

		public IQueryable<ExecuteResult> UpdateF051201ForB2C(String dcCode, String gupCode, String custCode, String delvDate, String pickTime, String userId, String userName, String deviceCount)
		{
			return CreateQuery<ExecuteResult>("UpdateF051201ForB2C")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("userId", userId)
						.AddQueryExOption("userName", userName)
						.AddQueryExOption("deviceCount", deviceCount);
		}

		public IQueryable<F050801WithF055001> GetF050801WithF055001Datas(String dcCode, String gupCode, String custCode, DateTime? delvDate, String pickTime, String wmsOrdNo, String pastNo, String itemCode, String ordNo)
		{
			return CreateQuery<F050801WithF055001>("GetF050801WithF055001Datas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo)
						.AddQueryExOption("pastNo", pastNo)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("ordNo", ordNo);
		}

		public IQueryable<F0513WithF1909> GetF0513WithF1909Datas(String dcCode, String gupCode, String custCode, String delvDate, String delvTime, String status)
		{
			return CreateQuery<F0513WithF1909>("GetF0513WithF1909Datas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("delvTime", delvTime)
						.AddQueryExOption("status", status);
		}

		public IQueryable<ExecuteResult> UpdatePierCode(String dcCode, String gupCode, String custCode, String delvDate, String pickTime, String allId, String takeTime, String pierCode)
		{
			return CreateQuery<ExecuteResult>("UpdatePierCode")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("allId", allId)
						.AddQueryExOption("takeTime", takeTime)
						.AddQueryExOption("pierCode", pierCode);
		}

		public IQueryable<F0513WithF050801Batch> GetBatchDebitDatas(String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<F0513WithF050801Batch>("GetBatchDebitDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}


		public IQueryable<F050801StatisticsData> GetF050801StatisticsData(String dcCode, String gupCode, String custCode, String delvDate)
		{
			return CreateQuery<F050801StatisticsData>("GetF050801StatisticsData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate);
		}

		public IQueryable<PickingStatistics> GetPickingStatistics(String dcCode, String gupCode, String custCode, String delvDate, String startPickTime, String endPickTime)
		{
			return CreateQuery<PickingStatistics>("GetPickingStatistics")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("startPickTime", startPickTime)
						.AddQueryExOption("endPickTime", endPickTime);
		}

		public IQueryable<F050801NoShipOrders> GetF050801NoShipOrders(String dcCode, String gupCode, String custCode, String delvDate, String pickTime, String status, String ordNo, String custOrdNo)
		{
			return CreateQuery<F050801NoShipOrders>("GetF050801NoShipOrders")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("status", status)
						.AddQueryExOption("ordNo", ordNo)
						.AddQueryExOption("custOrdNo", custOrdNo);
		}

		public IQueryable<F050101Ex> GetF050101ExDatas(String gupCode, String custCode, String dcCode, String ordDateFrom, String ordDateTo, String ordNo, String arriveDateFrom, String arriveDateTo, String custOrdNo, String status, String retailCode, String custName, String wmsOrdNo, String pastNo, String address)
		{
			return CreateQuery<F050101Ex>("GetF050101ExDatas")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("ordDateFrom", ordDateFrom)
						.AddQueryExOption("ordDateTo", ordDateTo)
						.AddQueryExOption("ordNo", ordNo)
						.AddQueryExOption("arriveDateFrom", arriveDateFrom)
						.AddQueryExOption("arriveDateTo", arriveDateTo)
						.AddQueryExOption("custOrdNo", custOrdNo)
						.AddQueryExOption("status", status)
						.AddQueryExOption("retailCode", retailCode)
						.AddQueryExOption("custName", custName)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo)
						.AddQueryExOption("pastNo", pastNo)
						.AddQueryExOption("address", address);
		}

		public IQueryable<F050102Ex> GetF050102ExDatas(String dcCode, String gupCode, String custCode, String ordNo)
		{
			return CreateQuery<F050102Ex>("GetF050102ExDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("ordNo", ordNo);
		}

		public IQueryable<F050102WithF050801> GetF050102WithF050801s(String gupCode, String custCode, String dcCode, String wmsordno)
		{
			return CreateQuery<F050102WithF050801>("GetF050102WithF050801s")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("wmsordno", wmsordno);
		}

		public IQueryable<P05030201BasicData> GetP05030201BasicData(String gupCode, String custCode, String dcCode, String wmsOrdNo, String ordNo)
		{
			return CreateQuery<P05030201BasicData>("GetP05030201BasicData")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo)
						.AddQueryExOption("ordNo", ordNo);
		}

		public String GetSourceNosByWmsOrdNo(String gupCode, String custCode, String dcCode, String wmsOrdNo)
		{
			return CreateQuery<String>("GetSourceNosByWmsOrdNo")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo).ToList().FirstOrDefault();
		}

		public IQueryable<F050001Data> GetF050001Datas(String dcCode, String gupCode, String custCode, String ordType, String ordSDate, String ordEDate, String arrivalSDate, String arrivalEDate, String ordNo, String custOrdNo, String consignee, String itemCode, String itemName, String sourceType)
		{
			return CreateQuery<F050001Data>("GetF050001Datas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("ordType", ordType)
						.AddQueryExOption("ordSDate", ordSDate)
						.AddQueryExOption("ordEDate", ordEDate)
						.AddQueryExOption("arrivalSDate", arrivalSDate)
						.AddQueryExOption("arrivalEDate", arrivalEDate)
						.AddQueryExOption("ordNo", ordNo)
						.AddQueryExOption("custOrdNo", custOrdNo)
						.AddQueryExOption("consignee", consignee)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("itemName", itemName)
						.AddQueryExOption("sourceType", sourceType);
		}

		public IQueryable<F051205Data> GetF051205Record(Int32? pickNo)
		{
			return CreateQuery<F051205Data>("GetF051205Record")
						.AddQueryExOption("pickNo", pickNo);
		}

		public IQueryable<F051205Data> GetF051205Datas(String pickNo, String dcCode, String warehouseId, String floor, String startChannel, String endChannel)
		{
			return CreateQuery<F051205Data>("GetF051205Datas")
						.AddQueryExOption("pickNo", pickNo)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("warehouseId", warehouseId)
						.AddQueryExOption("floor", floor)
						.AddQueryExOption("startChannel", startChannel)
						.AddQueryExOption("endChannel", endChannel);
		}

		public IQueryable<P710802SearchResult> GetF710802Type1(String gupCode, String custCode, String dcCode, String changeDateBegin, String changeDateEnd, String itemCode, String itemName, String receiptType)
		{
			return CreateQuery<P710802SearchResult>("GetF710802Type1")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("changeDateBegin", changeDateBegin)
						.AddQueryExOption("changeDateEnd", changeDateEnd)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("itemName", itemName)
						.AddQueryExOption("receiptType", receiptType);
		}

		public IQueryable<P710802SearchResult> GetF710802Type2(String gupCode, String custCode, String dcCode, String changeDateBegin, String changeDateEnd, String itemCode, String itemName, String receiptType)
		{
			return CreateQuery<P710802SearchResult>("GetF710802Type2")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("changeDateBegin", changeDateBegin)
						.AddQueryExOption("changeDateEnd", changeDateEnd)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("itemName", itemName)
						.AddQueryExOption("receiptType", receiptType);
		}

		public IQueryable<P710802SearchResult> GetF710802Type3(String gupCode, String custCode, String dcCode, String changeDateBegin, String changeDateEnd, String itemCode, String itemName)
		{
			return CreateQuery<P710802SearchResult>("GetF710802Type3")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("changeDateBegin", changeDateBegin)
						.AddQueryExOption("changeDateEnd", changeDateEnd)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("itemName", itemName);
		}

	





		public Boolean CanViewPersonalData(String empID)
		{
			return CreateQuery<Boolean>("CanViewPersonalData")
						.AddQueryExOption("empID", empID).ToList().FirstOrDefault();
		}

		public IQueryable<P050303QueryItem> GetP050303SearchData(String gupCode, String custCode, String dcCode, DateTime? delvDateBegin, DateTime? delvDateEnd, String ordNo, String custOrdNo, String wmsOrdNo, String status, String consignNo)
		{
			return CreateQuery<P050303QueryItem>("GetP050303SearchData")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("delvDateBegin", delvDateBegin)
						.AddQueryExOption("delvDateEnd", delvDateEnd)
						.AddQueryExOption("ordNo", ordNo)
						.AddQueryExOption("custOrdNo", custOrdNo)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo)
						.AddQueryExOption("status", status)
						.AddQueryExOption("consignNo", consignNo);
		}

		public IQueryable<F055002WithGridLog> GetF055002WithGridLog(String dcCode, String gupCode, String custCode, String wmsOrdNo)
		{
			return CreateQuery<F055002WithGridLog>("GetF055002WithGridLog")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo);
		}

		public IQueryable<F050801WithBill> GetF050801SeparateBillData(String dcCode, String gupCode, String custCode, String wmsOrdNo)
		{
			return CreateQuery<F050801WithBill>("GetF050801SeparateBillData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo);
		}

		public IQueryable<F700101CarData> GetF700101Data(String dcCode, String gupCode, String custCode, String delvDate, String pickTime, String sourceTye, String ordType)
		{
			return CreateQuery<F700101CarData>("GetF700101Data")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("sourceTye", sourceTye)
						.AddQueryExOption("ordType", ordType);
        }
        
        public IQueryable<RP0501010004Model> GetF051201SingleStickersReportDataAsForB2C(String dcCode, String gupCode, String custCode, DateTime? delvDate, String pickTime, String pickOrdNo)
        {
            return CreateQuery<RP0501010004Model>("GetF051201SingleStickersReportDataAsForB2C")
                        .AddQueryExOption("dcCode", dcCode)
                        .AddQueryExOption("gupCode", gupCode)
                        .AddQueryExOption("custCode", custCode)
                        .AddQueryExOption("delvDate", delvDate)
                        .AddQueryExOption("pickTime", pickTime)
                        .AddQueryExOption("pickOrdNo", pickOrdNo);
        }

        public IQueryable<P050103ReportData> GetF051201BatchReportDataAsForB2C(String dcCode, String gupCode, String custCode, DateTime? delvDate, String pickTime, String pickOrdNo)
        {
            return CreateQuery<P050103ReportData>("GetF051201BatchReportDataAsForB2C")
                        .AddQueryExOption("dcCode", dcCode)
                        .AddQueryExOption("gupCode", gupCode)
                        .AddQueryExOption("custCode", custCode)
                        .AddQueryExOption("delvDate", delvDate)
                        .AddQueryExOption("pickTime", pickTime)
                        .AddQueryExOption("pickOrdNo", pickOrdNo);
        }
        public IQueryable<RP0501010005Model> GetF051201BatchStickersReportDataAsForB2C(String dcCode, String gupCode, String custCode, DateTime? delvDate, String pickTime, String pickOrdNo)
        {
            return CreateQuery<RP0501010005Model>("GetF051201BatchStickersReportDataAsForB2C")
                        .AddQueryExOption("dcCode", dcCode)
                        .AddQueryExOption("gupCode", gupCode)
                        .AddQueryExOption("custCode", custCode)
                        .AddQueryExOption("delvDate", delvDate)
                        .AddQueryExOption("pickTime", pickTime)
                        .AddQueryExOption("pickOrdNo", pickOrdNo);
        }
    }
}

