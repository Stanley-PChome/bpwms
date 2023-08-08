using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P16.ExDataSources;
using Wms3pl.WebServices.Process.P16.ServiceEntites;
using Wms3pl.WebServices.Process.P16.Services;

namespace Wms3pl.WebServices.Process.P16
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P16ExDataService : DataService<P16ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		#region P161201 退貨單維護相關
		[WebGet]
		public IQueryable<F161201DetailDatas> GetF161201DetailDatas(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var p161201Service = new P161201Service();
			return p161201Service.GetF161201DetailDatas(dcCode, gupCode, custCode, returnNo);
		}

		/// <summary>
		/// 新增退貨單時，可用出貨單號來匯入出貨單的 Item
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo">F050802的出貨單號</param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F161201DetailDatas> GetItemsByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var srv = new P160101Service();
			return srv.GetItemsByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo);
		}

		[WebGet]
		public IQueryable<ExecuteResult> DeleteP160101(string returnNo, string gupCode, string custCode, string dcCode)
		{
			var wmsTransaction = new WmsTransaction();
			var p160101Srv = new P160101Service(wmsTransaction);
			var result = p160101Srv.DeleteP160101(returnNo, gupCode, custCode, dcCode);
			if (result.IsSuccessed) wmsTransaction.Complete();

			return (new List<ExecuteResult> { result }).AsQueryable();
		}
		#endregion

		#region F161601 退貨上架申請相關
		[WebGet]
		public IQueryable<F161601DetailDatas> GetF161601DetailDatas(string dcCode, string gupCode, string custCode, string rtnApplyNo)
		{
			var p161601Service = new P160102Service();
			return p161601Service.GetF161601DetailDatas(dcCode, gupCode, custCode, rtnApplyNo);
		}

		[WebGet]
		public IQueryable<F161401ReturnWarehouse> GetF161401ReturnWarehouse(string dcCode, string gupCode, string custCode, string returnNo, string locCode, string itemCode, string itemName)
		{
			var p161601Service = new P160102Service();
			return p161601Service.GetF161401ReturnWarehouse(dcCode, gupCode, custCode, returnNo, locCode, itemCode, itemName);
		}

		[WebGet]
		public IQueryable<PrintF161601Data> GetPrintF161601Data(string dcCode, string gupCode, string custCode, string rtnApplyNo)
		{
			var p161601Service = new P160102Service();
			return p161601Service.GetPrintF161601Data(dcCode, gupCode, custCode, rtnApplyNo);
		}

		[WebGet]
		public ExecuteResult DeleteP160102(string dcCode, string gupCode, string custCode, string rtnApplyNo)
		{
			var _wmsTransaction = new WmsTransaction();
			var srv = new P160102Service(_wmsTransaction);
			var result = srv.DeleteP160102(dcCode, gupCode, custCode, rtnApplyNo);
			if (result.IsSuccessed)
				_wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region 廠退與廠退出貨

		[WebGet]
		public IQueryable<F160201Data> GetF160201Datas(string dcCode, string gupCode, string custCode, string status,
			DateTime createBeginDateTime, DateTime createEndDateTime, string postingBeginDateTime, string postingEndDateTime,
			string returnNo, string custOrdNo, string vendorCode, string vendorName)
		{
			var repo = new F160201Repository(Schemas.CoreSchema);

			var result = repo.GetF160201Datas(dcCode, gupCode, custCode, status,
				createBeginDateTime, createEndDateTime,
				postingBeginDateTime, postingEndDateTime, returnNo, custOrdNo, vendorCode, vendorName);

			return result;
		}

		[WebGet]
		public IQueryable<F160201DataDetail> GetF160201DataDetails(string dcCode, string gupCode, string custCode,
			string returnNo)
		{
			var repo = new F160201Repository(Schemas.CoreSchema);
			var result = repo.GetF160201DataDetails(dcCode, gupCode, custCode, returnNo);
			return result;
		}

		[WebGet]
		public IQueryable<F160201ReturnDetail> GetF160201ReturnDetails(string dcCode, string gupCode, string custCode,
			string warehouseId, DateTime? enterDateBegin, DateTime? enterDateEnd, DateTime? validDateBegin, DateTime? validDateEnd,
			string locBegin, string locEnd, string itemCode, string itemName)
		{
			var repo = new F160201Repository(Schemas.CoreSchema);
			var result = repo.GetF160201ReturnDetails(dcCode, gupCode, custCode, warehouseId,
				enterDateBegin,
				enterDateEnd,
				validDateBegin,
				validDateEnd,
				locBegin, locEnd, itemCode, itemName);
			return result;
		}

		[WebGet]
		public IQueryable<F160201ReturnDetail> GetF160201ReturnDetailsForEdit(string dcCode, string gupCode, string custCode,
			string vendorCode, string returnNo)
		{
			var repo = new F160201Repository(Schemas.CoreSchema);
			var result = repo.GetF160201ReturnDetailsForEdit(dcCode, gupCode, custCode, vendorCode, returnNo);
			return result;
		}

		[WebGet]
		public IQueryable<F160201Data> GetF160201DatasNotFinish(string dcCode, string gupCode, string custCode,
			DateTime createBeginDateTime, DateTime createEndDateTime, DateTime? returnBeginDateTime, DateTime? returnEndDateTime,
			string deliveryWay, string returnNo, string returnType, string vendorCode, string vendorName,string typeId,string custOrdNo)
		{
			var repo = new F160201Repository(Schemas.CoreSchema);

			var result = repo.GetF160201DatasNotFinish(dcCode, gupCode, custCode,
				createBeginDateTime, createEndDateTime,
				returnBeginDateTime, returnEndDateTime,
				deliveryWay, returnNo, returnType, vendorCode, vendorName,typeId,custOrdNo);

			return result;
		}

		[WebGet]
		public IQueryable<F160204Detail> ConvertToF160204Detail(string dcCode, string gupCode, string custCode,
			string returnNo)
		{
			var repo = new F160204Repository(Schemas.CoreSchema);
			var result = repo.ConvertToF160204Detail(dcCode, gupCode, custCode, returnNo);
			return result;
		}

		[WebGet]
		public IQueryable<F160204SearchResult> GetF160204SearchResult(string dcCode, string gupCode, string custCode,
			string createBeginDateTime, string createEndDateTime, string returnWmsNo, string returnVnrNo,
			string orderNo,string empId,string empName,string custOrdNo)
		{
			var repo = new F160204Repository(Schemas.CoreSchema);

			var result = repo.GetF160204SearchResult(dcCode, gupCode, custCode,
				Convert.ToDateTime(createBeginDateTime),
				(Convert.ToDateTime(createEndDateTime)).AddDays(1).AddMinutes(-1),
				returnWmsNo, returnVnrNo, orderNo,empId,empName, custOrdNo);

			return result;
		}

		[WebGet]
		public IQueryable<F160204SearchResult> GetF160204SearchResultDetail(string dcCode, string gupCode, string custCode,
			string returnWmsNo)
		{
			var repo = new F160204Repository(Schemas.CoreSchema);
			var result = repo.GetF160204SearchResultDetail(dcCode, gupCode, custCode, returnWmsNo);
			return result;
		}
		#endregion

		/// <summary>
		/// 退貨上架申請，依照倉別產生調撥單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="rtnApplyNo"></param>
		/// <returns></returns>
		[WebGet]
		public ExecuteResult PrintP160102(string dcCode, string gupCode, string custCode, string rtnApplyNo)
		{
			WmsTransaction _wmsTransaction = new WmsTransaction();
			var srv = new P160102Service(_wmsTransaction);
			var result = srv.PrintP160102(dcCode, gupCode, custCode, rtnApplyNo);
			if (!result.IsSuccessed)
				return result;

			_wmsTransaction.Complete();
			return new ExecuteResult(true, Properties.Resources.P16ExDataService_CRT_Transfer_Complete);
		}

		[WebGet]
		public IQueryable<P160102Report> GetP160102Reports(string dcCode, string gupCode, string custCode, string rtnApplyNo)
		{
			var repo = new F161601Repository(Schemas.CoreSchema);
			return repo.GetP160102Reports(dcCode, gupCode, custCode, rtnApplyNo);
		}

		#region 報廢單維護
		[WebGet]
		public IQueryable<F160402Data> GetF160402ScrapDetails(string dcCode, string gupCode, string custCode, string scrapNo)
		{
			var f160402repo = new F160402Repository(Schemas.CoreSchema);
			var result = f160402repo.GetF160402ScrapDetails(dcCode, gupCode, custCode, scrapNo);
			return result;
		}

		[WebGet]
		public IQueryable<F160402AddData> GetF160402AddScrapDetails(string dcCode, string gupCode, string custCode, string wareHouseId,
			string itemCode = null, string locCode = null, string itemName = null, string validDateStart = null, string validDateEnd = null)
		{
			var srv = new P160402Service();
			var result = srv.GetF160402AddScrapDetails(dcCode, gupCode, custCode, wareHouseId, itemCode, locCode, itemName, validDateStart, validDateEnd);
			return result;
		}
		[WebGet]
		public ExecuteResult DeleteP160401(string dcCode, string gupCode, string custCode, string scrapNo)
		{
			var _wmsTransaction = new WmsTransaction();
			var srv = new P160401Service(_wmsTransaction);
			var result = srv.DeleteP160401(dcCode, gupCode, custCode, scrapNo);
			if (result.IsSuccessed)
				_wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region 銷毀主檔查詢
		[WebGet]
		public IQueryable<F160501Data> Get160501QueryData(
			string dcItem , string gupCode , string custCode, string destoryNo, string postingSDate
			, string postingEDate, string custOrdNo, string status, string ordNo, string crtSDate, string crtEDate)
		{
			var srv = new P160501Service();
			var result = srv.Get160501QueryData(dcItem, gupCode, custCode, destoryNo, postingSDate, postingEDate, custOrdNo
			, status, ordNo, crtSDate, crtEDate);

			return result;
		}
		#endregion

		#region 銷毀明細資料
		[WebGet]
		public IQueryable<F160502Data> Get160502DetailData(string dcCode,string gupCode,string custCode,string destoryNo)
		{
			var srv = new P160502Service();
			var result = srv.Get160502DetailData(dcCode,gupCode,custCode,destoryNo);

			return result;
		}
		#endregion

		#region 取 銷毀虛擬商品序號明細資料
		[WebGet]
		public IQueryable<F160502Data> Get160504SerialData(string dcCode, string gupCode, string custCode, string destoryNo)
		{
			var srv = new P160504Service();
			var result = srv.Get160504SerialData(dcCode,gupCode,custCode,destoryNo);

			return result;
		}
		#endregion

		#region 取得銷毀單據狀態
		[WebGet]
		public IQueryable<F160501Status> GetF160501Status(string dcCode, string gupCode, string custCode, string destoryNo)
		{
			//讀取圖檔
			var srv = new P160501Service();
			return srv.GetF160501Status(dcCode,gupCode,custCode,destoryNo);

		}
		#endregion

		#region 檢查單據-檔案上傳資訊
		[WebGet]
		public IQueryable<F160501FileData> GetDestoryNoFile(string destoryNo)
		{
			var srv = new P160503Service();
			return srv.GetDestoryNoFile(destoryNo);

		}
		#endregion

		#region 取得銷毀單相關連單據-檔案上傳資訊
		[WebGet]
		public IQueryable<F160501FileData> GetDestoryNoRelation(string destoryNo)
		{
			var srv = new P160503Service();
			return srv.GetDestoryNoRelation(destoryNo);

		}
		#endregion


		/// <summary>
		/// 取得退貨單維護新增時，左側查詢歷史出貨單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginDelvDate"></param>
		/// <param name="endDelvDate"></param>
		/// <param name="retailCode"></param>
		/// <param name="custName"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<CustomerData> GetCustomerDatas(string dcCode, string gupCode, string custCode, DateTime beginDelvDate, DateTime endDelvDate, string retailCode, string custName, string wmsOrdNo, string custOrdNo)
		{
			var repo = new F050801Repository(Schemas.CoreSchema);
			return repo.GetCustomerDatas(dcCode, gupCode, custCode, beginDelvDate, endDelvDate, retailCode, custName, wmsOrdNo, custOrdNo);
		}

		/// <summary>
		/// 取得P017_退貨報表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginRtnVnrDate"></param>
		/// <param name="endRtnVnrDate"></param>
		/// <param name="rtnVnrNo"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<P160201Report> GetP160201Reports(string dcCode, string gupCode, string custCode, DateTime? beginRtnVnrDate, DateTime? endRtnVnrDate, string rtnVnrNo, string status)
		{
			var repo = new F160201Repository(Schemas.CoreSchema);
			return repo.GetP160201Reports(dcCode, gupCode, custCode, beginRtnVnrDate, endRtnVnrDate, rtnVnrNo, status);
		}

		/// <summary>
		/// 取得P107_退貨記錄總表報表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<P17ReturnAuditReport> GetP17ReturnAuditReports(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status)
		{
			var repo = new F161401Repository(Schemas.CoreSchema);
			return repo.GetP17ReturnAuditReports(dcCode, gupCode, custCode, beginReturnDate, endReturnDate, returnNo, status);
		}

		/// <summary>
		/// 取得RTO17840退貨記錄表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<RTO17840ReturnAuditReport> GetRTO17840ReturnAuditReports(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status)
		{
			var repo = new F161401Repository(Schemas.CoreSchema);
			return repo.GetRTO17840ReturnAuditReports(dcCode, gupCode, custCode, beginReturnDate, endReturnDate, returnNo, status);
		}

		/// <summary>
		/// B2C退貨記錄表(Friday退貨記錄表)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<B2CReturnAuditReport> GetB2CReturnAuditReports(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status)
		{
			var repo = new F161401Repository(Schemas.CoreSchema);
			return repo.GetB2CReturnAuditReports(dcCode, gupCode, custCode, beginReturnDate, endReturnDate, returnNo, status);
		}

		/// <summary>
		/// 取得P106_退貨未上架明細表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<P106ReturnNotMoveDetail> GetP106ReturnNotMoveDetails(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status)
		{
			var repo = new F161401Repository(Schemas.CoreSchema);
			return repo.GetP106ReturnNotMoveDetails(dcCode, gupCode, custCode, beginReturnDate, endReturnDate, returnNo, status);
		}

		/// <summary>
		/// 61-6 Txt 格式的退貨詳細資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<TxtFormatReturnDetail> GetTxtFormatReturnDetails(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status)
		{
			var repo = new F161401Repository(Schemas.CoreSchema);
			return repo.GetTxtFormatReturnDetails(dcCode, gupCode, custCode, beginReturnDate, endReturnDate, returnNo, status);
		}

		/// <summary>
		/// 61-7	 Txt 格式的退貨資料以 F1903.TYPE in (types) 的資料為主
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <param name="commaSeparatorTypes">逗號分隔的F1903.TYPE</param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<ReturnSerailNoByType> GetReturnSerailNosByType(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status, string commaSeparatorTypes)
		{
			var repo = new F161401Repository(Schemas.CoreSchema);
			return repo.GetReturnSerailNosByType(dcCode, gupCode, custCode, beginReturnDate, endReturnDate, returnNo, status, commaSeparatorTypes.ToSplit(','));
		}

		/// <summary>
		/// 取得P015_預計退貨明細表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<P015ForecastReturnDetail> GetP015ForecastReturnDetails(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status)
		{
			var repo = new F161401Repository(Schemas.CoreSchema);
			return repo.GetP015ForecastReturnDetails(dcCode, gupCode, custCode, beginReturnDate, endReturnDate, returnNo, status);
		}


		#region 取得F1913 報廢倉商品 
		[WebGet]
		public IQueryable<F160502Data> GetF1913ScrapData(string dcCode, string gupCode, string custCode)
		{			
			var srv = new P160501Service();
			return srv.GetF1913ScrapData(dcCode, gupCode, custCode);
		}
		#endregion

		#region 取得廠退出貨資料
		[WebGet]
		public IQueryable<F160204Data> GetF160204Data(string dcCode,string gupCode, string custCode, string wmsOrdNo)
		{
			var srv = new P160203Service();
			return srv.GetF160204Data(dcCode, gupCode, custCode, wmsOrdNo);
		}
		#endregion

		#region 更新廠退出貨資料
		[WebGet]
    public ExecuteResult UpdateF160204Data(string dcCode, string gupCode, string custCode, string wmsOrdNo, string deliveryWay,
      string allId, string procFlag, string sheetNum, string memo)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P160203Service(wmsTransaction);
      var result = srv.UpdateF160204Data(dcCode, gupCode, custCode, wmsOrdNo, deliveryWay, allId, procFlag, sheetNum, memo);
      if (result.IsSuccessed == true) wmsTransaction.Complete();
      return result;
    }

    #endregion


  }
}