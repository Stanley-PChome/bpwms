using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.ExDataServices.P91ExDataService
{
	public partial class P91ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{

		public IQueryable<F910403Data> GetF910403DataByQuoteNo(String dcCode, String gupCode, String custCode, String quoteNo)
		{
			return CreateQuery<F910403Data>("GetF910403DataByQuoteNo")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("quoteNo", quoteNo);
		}

		public IQueryable<F1903WithF1915> GetF1903WithF1915(String gupCode, String itemCode,string custCode)
		{
			return CreateQuery<F1903WithF1915>("GetF1903WithF1915")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("itemCode", itemCode)
                        .AddQueryExOption("custCode", custCode);
        }

		public IQueryable<F910301Data> GetContractDatas(String dcCode, String gupCode, String contractNo, String objectType, DateTime? beginCreateDate, DateTime? endCreateDate, String uniForm)
		{
			return CreateQuery<F910301Data>("GetContractDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("contractNo", contractNo)
						.AddQueryExOption("objectType", objectType)
						.AddQueryExOption("beginCreateDate", beginCreateDate)
						.AddQueryExOption("endCreateDate", endCreateDate)
						.AddQueryExOption("uniForm", uniForm);
		}

		public IQueryable<F910302Data> GetContractDetails(String dcCode, String gupCode, String contractNo)
		{
			return CreateQuery<F910302Data>("GetContractDetails")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("contractNo", contractNo);
		}

		public IQueryable<F910101Ex> GetF910101Datas(String gupCode, String custCode, String bomNo, String itemCode, String status, String bomType)
		{
			return CreateQuery<F910101Ex>("GetF910101Datas")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("bomNo", bomNo)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("status", status)
						.AddQueryExOption("bomType", bomType);
		}

		public IQueryable<F910102Ex> GetF910102Datas(String gupCode, String custCode, String bomNo, String status)
		{
			return CreateQuery<F910102Ex>("GetF910102Datas")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("bomNo", bomNo)
						.AddQueryExOption("status", status);
		}

		public ExecuteResult DeleteF910101(String gupCode, String custCode, String bomNo, String userId)
		{
			return CreateQuery<ExecuteResult>("DeleteF910101")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("bomNo", bomNo)
						.AddQueryExOption("userId", userId).ToList().FirstOrDefault();
		}

		public IQueryable<F910401Report> GetF910401Report(String dcCode, String gupCode, String custCode, String quoteNo)
		{
			return CreateQuery<F910401Report>("GetF910401Report")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("quoteNo", quoteNo);
		}

		public IQueryable<F910402Report> GetF910402Reports(String dcCode, String gupCode, String custCode, String quoteNo)
		{
			return CreateQuery<F910402Report>("GetF910402Reports")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("quoteNo", quoteNo);
		}

		public ExecuteResult DeleteContract(String dcCode, String gupCode, String contractNo)
		{
			return CreateQuery<ExecuteResult>("DeleteContract")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("contractNo", contractNo).ToList().FirstOrDefault();
		}

		public IQueryable<F910403Report> GetF910403Reports(String dcCode, String gupCode, String custCode, String quoteNo)
		{
			return CreateQuery<F910403Report>("GetF910403Reports")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("quoteNo", quoteNo);
		}

		public IQueryable<F910402Detail> GetF910402Detail(String dcCode, String gupCode, String custCode, String quoteNo)
		{
			return CreateQuery<F910402Detail>("GetF910402Detail")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("quoteNo", quoteNo);
		}

		public IQueryable<F910403Detail> GetF910403Detail(String dcCode, String gupCode, String custCode, String quoteNo)
		{
			return CreateQuery<F910403Detail>("GetF910403Detail")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("quoteNo", quoteNo);
		}

		public IQueryable<ExecuteResult> InsertF910201(String dcCode, String gupCode, String custCode, String processSource, String outsourceId, String finishDate, String itemCode, String itemCodeBom, Int32? processQty, Int32? boxQty, Int32? caseQty, String orderNo, String memo, String quoteNo, String finishTime)
		{
			return CreateQuery<ExecuteResult>("InsertF910201")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("processSource", processSource)
						.AddQueryExOption("outsourceId", outsourceId)
						.AddQueryExOption("finishDate", finishDate)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("itemCodeBom", itemCodeBom)
						.AddQueryExOption("processQty", processQty)
						.AddQueryExOption("boxQty", boxQty)
						.AddQueryExOption("caseQty", caseQty)
						.AddQueryExOption("orderNo", orderNo)
						.AddQueryExOption("memo", memo)
						.AddQueryExOption("quoteNo", quoteNo)
						.AddQueryExOption("finishTime", finishTime);
		}

		public IQueryable<ExecuteResult> UpdateF910201(String dcCode, String gupCode, String custCode, String processNo, String processSource, String outsourceId, String finishDate, String itemCode, String itemCodeBom, String processQty, String boxQty, String caseQty, String orderNo, String memo, String quoteNo, String breakQty)
		{
			return CreateQuery<ExecuteResult>("UpdateF910201")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("processNo", processNo)
						.AddQueryExOption("processSource", processSource)
						.AddQueryExOption("outsourceId", outsourceId)
						.AddQueryExOption("finishDate", finishDate)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("itemCodeBom", itemCodeBom)
						.AddQueryExOption("processQty", processQty)
						.AddQueryExOption("boxQty", boxQty)
						.AddQueryExOption("caseQty", caseQty)
						.AddQueryExOption("orderNo", orderNo)
						.AddQueryExOption("memo", memo)
						.AddQueryExOption("quoteNo", quoteNo)
						.AddQueryExOption("breakQty", breakQty);
		}

		public IQueryable<ExecuteResult> DeleteF910201(String processNo, String gupCode, String custCode, String dcCode)
		{
			return CreateQuery<ExecuteResult>("DeleteF910201")
						.AddQueryExOption("processNo", processNo)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode);
		}

		public IQueryable<F910101Ex2> GetF910101Ex2(String gupCode, String custCode, String status)
		{
			return CreateQuery<F910101Ex2>("GetF910101Ex2")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("status", status);
		}

		public IQueryable<BomQtyData> GetBomQtyData(String dcCode, String gupCode, String custCode, String processNo, String type)
		{
			return CreateQuery<BomQtyData>("GetBomQtyData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("processNo", processNo)
						.AddQueryExOption("type", type);
		}

		public IQueryable<ProcessItem> GetMaterialList(String gupCode, String custCode, String dcCode, String processNo)
		{
			return CreateQuery<ProcessItem>("GetMaterialList")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("processNo", processNo);
		}

		public IQueryable<ProcessItem> GetFinishItemList(String gupCode, String custCode, String dcCode, String processNo)
		{
			return CreateQuery<ProcessItem>("GetFinishItemList")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("processNo", processNo);
		}

		public IQueryable<StockData> GetStockData(String dcCode, String gupCode, String custCode, String processNo)
		{
			return CreateQuery<StockData>("GetStockData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("processNo", processNo);
		}

		public IQueryable<StockData> GetStockData2(String dcCode, String gupCode, String custCode, String itemCode)
		{
			return CreateQuery<StockData>("GetStockData2")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("itemCode", itemCode);
		}

		public IQueryable<F910301Report> GetContractReports(String dcCode, String gupCode, String contractNo)
		{
			return CreateQuery<F910301Report>("GetContractReports")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("contractNo", contractNo);
		}

		public IQueryable<F910004Data> GetF910004Data(String dcCode)
		{
			return CreateQuery<F910004Data>("GetF910004Data")
						.AddQueryExOption("dcCode", dcCode);
		}

	

		
	

		

		



		public IQueryable<ExecuteResult> FinishProcess(String processNo, String gupCode, String custCode, String dcCode, Int32? aProcessQty, Int32? breakQty, String memo)
		{
			return CreateQuery<ExecuteResult>("FinishProcess")
						.AddQueryExOption("processNo", processNo)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("aProcessQty", aProcessQty)
						.AddQueryExOption("breakQty", breakQty)
						.AddQueryExOption("memo", memo);
		}

		public IQueryable<BackData> GetBackListForP9101010500(String dcCode, String gupCode, String custCode, String processNo)
		{
			return CreateQuery<BackData>("GetBackListForP9101010500")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("processNo", processNo);
		}

		public String GetContractFee(String dcCode, String gupCode, String custCode, String quoteNo, String itemTye)
		{
			return CreateQuery<String>("GetContractFee")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("quoteNo", quoteNo)
						.AddQueryExOption("itemTye", itemTye).ToList().FirstOrDefault();
		}

		





		public Int32 GetfinishedItems(String dcCode, String gupCode, String custCode, String processNo)
		{
			return CreateQuery<Int32>("GetfinishedItems")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("processNo", processNo).ToList().FirstOrDefault();
		}



		

		public IQueryable<P91010105Report> GetP91010105Reports(String dcCode, String gupCode, String custCode, String processNo)
		{
			return CreateQuery<P91010105Report>("GetP91010105Reports")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("processNo", processNo);
		}
	}
}

