using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F91;
using Wms3pl.Common.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P91.ExDataSources;
using Wms3pl.WebServices.Process.P91.Services;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P91
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P91ExDataService : DataService<P91ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		/// <summary>
		/// 以報價單主檔取得除了 F910403 欄位外，也帶出商品名稱跟大分類名稱
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F910403Data> GetF910403DataByQuoteNo(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var srv = new P910302Service();
			return srv.GetF910403DataByQuoteNo(dcCode, gupCode, custCode, quoteNo);
		}

		/// <summary>
		/// 以商品編號取得商品名稱跟大分類名稱
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F1903WithF1915> GetF1903WithF1915(string gupCode, string itemCode,string custCode)
		{
			var srv = new P910302Service();
			return srv.GetF1903WithF1915(gupCode, itemCode, custCode);
		}

		[WebGet]
		public IQueryable<F910301Data> GetContractDatas(string dcCode, string gupCode, string contractNo,
														string objectType, DateTime beginCreateDate, DateTime endCreateDate,
														string uniForm)
		{
			var f910301repo = new F910301Repository(Schemas.CoreSchema);
			var result = f910301repo.GetContractDatas(dcCode, gupCode, contractNo, objectType, beginCreateDate, endCreateDate, uniForm);
			return result;
		}

		[WebGet]
		public IQueryable<F910302Data> GetContractDetails(string dcCode, string gupCode, string contractNo)
		{
			var f910302repo = new F910302Repository(Schemas.CoreSchema);
			var result = f910302repo.GetContractDetails(dcCode, gupCode, contractNo);
			return result;
		}


		[WebGet]
		public IQueryable<F910101Ex> GetF910101Datas(string gupCode, string custCode, string bomNo, string itemCode, string status, string bomType)
		{
			var srv = new P910101Service();
			return srv.GetF910101Datas(gupCode, custCode, bomNo, itemCode, status, bomType);
		}
		[WebGet]
		public IQueryable<F910102Ex> GetF910102Datas(string gupCode, string custCode, string bomNo, string status)
		{
			var srv = new P910101Service();
			return srv.GetF910102Datas(gupCode, custCode, bomNo);
		}
		[WebGet]
		public Wms3pl.Datas.Shared.Entities.ExecuteResult DeleteF910101(string gupCode, string custCode, string bomNo, string userId)
		{
			var srv = new P910101Service();
			return srv.DeleteF910101(gupCode, custCode, bomNo, userId);
		}

		/// <summary>
		/// 取得報價單報表主檔
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F910401Report> GetF910401Report(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var srv = new P910302Service();
			return srv.GetF910401Report(dcCode, gupCode, custCode, quoteNo);
		}

		/// <summary>
		/// 取得報價單動作分析明細報表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F910402Report> GetF910402Reports(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var srv = new P910302Service();
			return srv.GetF910402Reports(dcCode, gupCode, custCode, quoteNo);
		}

		[WebGet]
		public Wms3pl.Datas.Shared.Entities.ExecuteResult DeleteContract(string dcCode, string gupCode, string contractNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910301Service(wmsTransaction);
			var result = srv.DeleteContract(dcCode, gupCode, contractNo);
			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}
			return result;
		}
		/// <summary>
		/// 取得報價單耗材項目報表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F910403Report> GetF910403Reports(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var srv = new P910302Service();
			return srv.GetF910403Reports(dcCode, gupCode, custCode, quoteNo);
		}

		/// <summary>
		/// 取得動作分析
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F910402Detail> GetF910402Detail(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var srv = new P910101Service();
			return srv.GetF910402Detail(dcCode, gupCode, custCode, quoteNo);
		}

		/// <summary>
		/// 取得耗材統計
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F910403Detail> GetF910403Detail(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var srv = new P910101Service();
			return srv.GetF910403Detail(dcCode, gupCode, custCode, quoteNo);
		}

		[WebGet]
		public IQueryable<ExecuteResult> InsertF910201(string dcCode, string gupCode, string custCode, string processSource, string outsourceId, string finishDate, string itemCode, string itemCodeBom, int processQty, int boxQty, int caseQty, string orderNo, string memo, string quoteNo, string finishTime)
		{
			var wmsTransaction = new WmsTransaction();
			//int iBoxQty;
			//int.TryParse(boxQty, out iBoxQty);
			//int iCaseQty;
			//int.TryParse(caseQty, out iCaseQty);
			//int iProcessQty;
			//int.TryParse(processQty, out iProcessQty);

			var srv = new SharedService(wmsTransaction);
			var result = srv.InsertF910201(dcCode, gupCode, custCode, processSource, outsourceId, Convert.ToDateTime(finishDate), itemCode, itemCodeBom, processQty, boxQty, caseQty, orderNo, memo, quoteNo, finishTime);
			if (result.IsSuccessed) wmsTransaction.Complete();
			return new List<ExecuteResult>() { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<ExecuteResult> UpdateF910201(string dcCode, string gupCode, string custCode, string processNo, string processSource, string outsourceId, string finishDate, string itemCode, string itemCodeBom, int processQty, int boxQty, int caseQty, string orderNo, string memo, string quoteNo, int breakQty)
		{
			var wmsTransaction = new WmsTransaction();
			//int iBoxQty;
			//int.TryParse(boxQty, out iBoxQty);
			//int iCaseQty;
			//int.TryParse(caseQty, out iCaseQty);
			//int iProcessQty;
			//int.TryParse(processQty, out iProcessQty);
			//int iBreakQty;
			//int.TryParse(breakQty, out iBreakQty);

			var srv = new P910101Service(wmsTransaction);
			var result = srv.UpdateF910201(dcCode, gupCode, custCode, processNo, processSource, outsourceId, Convert.ToDateTime(finishDate), itemCode, itemCodeBom, processQty, boxQty, caseQty, orderNo, memo, quoteNo, breakQty);
			if (result.IsSuccessed) wmsTransaction.Complete();
			return new List<ExecuteResult>() { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<ExecuteResult> DeleteF910201(string processNo, string gupCode, string custCode, string dcCode)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910101Service(wmsTransaction);
			var result = srv.DeleteF910201(processNo, gupCode, custCode, dcCode);
			if (result.IsSuccessed) wmsTransaction.Complete();
			return new List<ExecuteResult>() { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<F910101Ex2> GetF910101Ex2(string gupCode, string custCode, string status)
		{
			var srv = new P910101Service();
			return srv.GetF910101Ex2(gupCode, custCode, status);
		}

		/// <summary>
		/// 取得組合商品所需總數, 以及在加工倉的庫存數量
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="processNo"></param>
		/// <param name="type">0: 組合商品, 1: 一般商品</param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<BomQtyData> GetBomQtyData(string dcCode, string gupCode, string custCode, string processNo, string type = "0")
		{
			var srv = new P910101Service();
			return srv.GetBomQtyData(dcCode, gupCode, custCode, processNo, type);
		}

		[WebGet]
		public IQueryable<ProcessItem> GetMaterialList(string gupCode, string custCode, string dcCode, string processNo)
		{
			var srv = new P910101Service();
			return srv.GetMaterialList(gupCode, custCode, dcCode, processNo);
		}

		[WebGet]
		public IQueryable<ProcessItem> GetFinishItemList(string gupCode, string custCode, string dcCode, string processNo)
		{
			var srv = new P910101Service();
			return srv.GetFinishItemList(gupCode, custCode, dcCode, processNo);
		}

		[WebGet]
		public IQueryable<StockData> GetStockData(string dcCode, string gupCode, string custCode, string processNo)
		{
			var srv = new P910101Service();
			return srv.GetStockData(dcCode, gupCode, custCode, processNo);
		}

		[WebGet]
		public IQueryable<StockData> GetStockData2(string dcCode, string gupCode, string custCode, string itemCode)
		{
			var srv = new P910101Service();
			return srv.GetStockData2(dcCode, gupCode, custCode, itemCode);
		}

		[WebGet]
		public IQueryable<F910301Report> GetContractReports(string dcCode, string gupCode, string contractNo)
		{
			var f910301repo = new F910301Repository(Schemas.CoreSchema);
			var result = f910301repo.GetContractReports(dcCode, gupCode, contractNo);
			return result;
		}

		/// <summary>
		/// 列出此物流中心設定的生產線清單與其目前未結案工單數、加工數量
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F910004Data> GetF910004Data(string dcCode)
		{
			var repo = new F910004Repository(Schemas.CoreSchema);
			var result = repo.GetF910004Data(dcCode);
			return result;
		}




	

	

	



		



		[WebGet]
		public IQueryable<ExecuteResult> FinishProcess(string processNo, string gupCode, string custCode, string dcCode, int aProcessQty, int breakQty, string memo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910101Service(wmsTransaction);
			var result = srv.FinishProcess(processNo, gupCode, custCode, dcCode, aProcessQty, breakQty, memo);
			if (result != null && result.IsSuccessed) wmsTransaction.Complete();
			return new List<ExecuteResult>() { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<BackData> GetBackListForP9101010500(string dcCode, string gupCode, string custCode, string processNo)
		{
			var srv = new P910101Service();
			var result = srv.GetBackListForP9101010500(dcCode, gupCode, custCode, processNo);
			return result;
		}

        [WebGet]
        public IQueryable<BackData> GetHistoryListForP9101010500(string dcCode, string gupCode, string custCode, string processNo)
        {
            var srv = new P910101Service();
            var result = srv.GetHistoryListForP9101010500(dcCode, gupCode, custCode, processNo);
            return result;
        }

        [WebGet]
		public string GetContractFee(string dcCode, string gupCode, string custCode, string quoteNo, string itemTye)
		{
			var srv = new P910303Service();
			var result = srv.GetContractFee(dcCode, gupCode, custCode, quoteNo, itemTye);
			return result;
		}

		

		

	



	

		[WebGet]
		public IQueryable<PickReport> GetPickTicketReport(string dcCode, string gupCode, string custCode, string processNo)
		{
			var srv = new P910101Service();
			return srv.GetPickTicketReport(dcCode, gupCode, custCode, processNo);
		}

		[WebGet]
		public int GetfinishedItems(string dcCode, string gupCode, string custCode, string processNo)
		{
			var srv = new P910101Service();
			return srv.GetProcessedItemsQty(dcCode, gupCode, custCode, processNo);

		}

		



		/// <summary>
		/// 上架回倉報表: 預覽或列印當前尚未完成回倉的回倉明細調撥單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="processNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<P91010105Report> GetP91010105Reports(string dcCode, string gupCode, string custCode, string processNo)
		{
			var repo = new F91020601Repository(Schemas.CoreSchema);
			return repo.GetP91010105Reports(dcCode, gupCode, custCode, processNo);
		}

		
	}
}
