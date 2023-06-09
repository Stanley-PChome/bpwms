using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P08.Services;
using Wms3pl.WebServices.Process.P14.ExDataSources;
using Wms3pl.WebServices.Process.P14.Services;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P14
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P14ExDataService : DataService<P14ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		[WebGet]
		public IQueryable<InventoryDetailItem> GetInventoryDetailItems(string dcCode, string gupCode, string custCode,
	string inventoryNo, string wareHouseId, string begLocCode, string endLocCode, string itemCode)
		{
			var service = new P140101Service();
			return service.GetInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode);
		}

		[WebGet]
		public IQueryable<InventoryDetailItem> FindInventoryDetailItems(string dcCode, string gupCode, string custCode,
			string inventoryNo, string locCode, string itemCode, string enterDate, string validDate, string makeNo)
		{
			var service = new P140101Service();
			return service.FindInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, DateTime.Parse(enterDate), DateTime.Parse(validDate), makeNo);
		}

		[WebGet]
		public IQueryable<InventoryDetailItem> GetInventoryDetailItemsExport(string dcCode, string gupCode, string custCode,
		string inventoryNo, string wareHouseId, string begLocCode, string endLocCode, string itemCode)
		{
			var service = new P140101Service();
			return service.GetInventoryDetailItemsExport(dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode);
		}

        [WebGet]
		public IQueryable<ExecuteResult> DeleteF140101(string dcCode, string gupCode, string custCode, string inventoryNo, string checkTool)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P140101Service(wmsTransation);
			var result = service.DeleteF140101(dcCode, gupCode, custCode, inventoryNo, checkTool);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return new List<ExecuteResult> { result }.AsQueryable();
		}
		[WebGet]
		public IQueryable<InventoryItem> GetInventoryItems(string gupCode, string custCode, string type, string lType,
			string mType, string sType,string vnrCode,string vnrName, string itemCode)
		{
			var service = new P140101Service();
			return service.GetInventoryItems(gupCode, custCode, type, lType, mType, sType, vnrCode, vnrName, itemCode);

		}

		[WebGet]
		public IQueryable<InventoryWareHouse> GetInventoryWareHouses(string dcCode, string wareHouseType, string tool)
		{
			var service = new P140101Service();
			return service.GetInventoryWareHouses(dcCode, wareHouseType, tool);
		}

		[WebGet]
		public IQueryable<F140101Expansion> GetDatasExpansion(string dcCode, string gupCode, string custCode,
			string inventoryNo, string inventoryType,
			string inventorySDate, string inventoryEDate,
			string inventoryCycle, string inventoryYear, string inventoryMonth, string status)
		{
			DateTime? sdate = null;
			DateTime? edate = null;
			if (!string.IsNullOrEmpty(inventorySDate))
			{
				sdate = DateTime.Parse(inventorySDate);
			}
			if (!string.IsNullOrEmpty(inventoryEDate))
			{
				edate = DateTime.Parse(inventoryEDate);
				edate = edate.Value.AddDays(1).AddSeconds(-1);
			}
			var service = new P140101Service();

			return service.GetDatasExpansion(dcCode, gupCode, custCode, inventoryNo, inventoryType, sdate, edate, inventoryCycle, inventoryYear, inventoryMonth, status);
		}

		[WebGet]
		public IQueryable<InventoryDetailItemsByIsSecond> GetInventoryDetailItemsByIsSecond(string dcCode, string gupCode, string custCode,
			string inventoryNo, string isSecond, string wareHouseId, string itemCodes, string differencesRangeStart,
			string differencesRangeEnd, string isRepotTag, string showCnt)
		{
			var service = new P140104Service();
			return service.GetInventoryDetailItemsByIsSecond(dcCode, gupCode, custCode,
				inventoryNo, isSecond, wareHouseId, itemCodes, differencesRangeStart, differencesRangeEnd, isRepotTag, showCnt);
		}

		[WebGet]
		public IQueryable<P140102ReportData> GetP140102ReportData(string dcCode, string gupCode, string custCode, string inventoryNo, string isSecond)
		{
			var service = new P140104Service();
			return service.GetP140102ReportData(dcCode, gupCode, custCode, inventoryNo, isSecond);
		}



		[WebGet]
		public IQueryable<ExecuteResult> ReInsertF140105(string dcCode, string gupCode, string custCode, string inventoryNo, string clientName)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P140104Service(wmsTransation);
			var result = service.ReInsertF140105(dcCode, gupCode, custCode, inventoryNo, clientName);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return new List<ExecuteResult> { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<ExecuteResult> CheckF140105Exist(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P140104Service(wmsTransation);
			var result = service.CheckF140105Exist(dcCode, gupCode, custCode, inventoryNo);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return new List<ExecuteResult> { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<F140106QueryData> GetF140106QueryData(string dcCode, string gupCode, string custCode, string inventoryDateS, string inventoryDateE, string inventoryNo, string procWmsNo, string itemCode, string checkTool)
		{
			DateTime? sdate = null;
			DateTime? edate = null;
			if (!string.IsNullOrEmpty(inventoryDateS))
				sdate = DateTime.Parse(inventoryDateS);
			if (!string.IsNullOrEmpty(inventoryDateE))
				edate = DateTime.Parse(inventoryDateE);

			var service = new P140103Service();
			return service.GetF140106QueryData(dcCode, gupCode, custCode, sdate, edate, inventoryNo, procWmsNo, itemCode, checkTool);
		}



		[WebGet]
		public IQueryable<ExecuteResult> CheckLocCode(string locCode, string dcCode, string itemCode)
		{
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			var item = f1912Repo.Find(o => o.DC_CODE == dcCode && o.LOC_CODE == locCode);
			if (item == null)
				return new List<ExecuteResult> { new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P14ExDataService_LocCodeNotFound } }.AsQueryable();

			var sharedService = new SharedService();
			var result = sharedService.CheckLocCode(locCode, item.DC_CODE, item.WAREHOUSE_ID, Current.Staff, itemCode);
			if (result.IsSuccessed)
				result.Message = item.WAREHOUSE_ID;
			return new List<ExecuteResult> { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<WareHouseFloor> GetWareHouseFloor()
		{
			var service = new P140101Service();
			return service.GetWareHouseFloor();
		}

		[WebGet]
		public IQueryable<WareHouseChannel> GetWareHouseChannel()
		{
			var service = new P140101Service();
			return service.GetWareHouseChannel();
		}

		[WebGet]
		public IQueryable<WareHousePlain> GetWareHousePlain()
		{
			var service = new P140101Service();
			return service.GetWareHousePlain();
		}

		[WebGet]
		public IQueryable<InventoryQueryDataForDc> GetInventoryQueryDatasForDc(string dcCode, string gupCode, string custCode,
			string inventoryNo, string sortByCount, string warehouseId, string itemCodes)
		{
			var srv = new P140104Service();
			return srv.GetInventoryQueryDatasForDc(dcCode, gupCode, custCode, inventoryNo, sortByCount, warehouseId, itemCodes);
		}

		[WebGet]
		public IQueryable<InventoryDetailItemsByIsSecond> GetReportData(string dcCode, string gupCode, string custCode,
			string inventoryNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P140101Service(wmsTransaction);
			var result = srv.UpdateIsPrinted(dcCode, gupCode, custCode, inventoryNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return srv.GetReportData(dcCode, gupCode, custCode, inventoryNo);
		}

		[WebGet]
		public IQueryable<ExecuteResult> CheckInventoryDetailHasEnterQty(string dcCode, string gupCode, string custCode,
			string inventoryNo)
		{
			var srv = new P140104Service();
			var result = srv.CheckInventoryDetailHasEnterQty(dcCode, gupCode, custCode, inventoryNo);
			return new List<ExecuteResult> { result }.AsQueryable();
		}

		/// <summary>
		/// 檢核是否完成初、複盤
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<ExecuteResult> CheckInventoryIsFinish(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var srv = new P140104Service();
			var result = srv.CheckInventoryIsFinish(dcCode, gupCode, custCode, inventoryNo);
			return new List<ExecuteResult> { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<F1913Data> GetF140106QueryDetailData(string dcCode, string gupCode, string custCode,
		string inventoryNo)
		{
			var srv = new P140103Service();
			return srv.GetF140106QueryDetailData(dcCode, gupCode, custCode, inventoryNo);

		}

		[WebGet]
		public IQueryable<InventoryByLocDetail> GetReportData2(string dcCode, string gupCode, string custCode,
			string inventoryNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P140101Service(wmsTransaction);
			var result = srv.UpdateIsPrinted(dcCode, gupCode, custCode, inventoryNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return srv.GetReportData2(dcCode, gupCode, custCode, inventoryNo);
		}

		[WebGet]
		public IQueryable<InventoryLocItem> GetInventoryLocItems(string dcCode, string gupCode, string custCode,
			string inventoryNo)
		{
			var p081001Service = new P081001Service();
			return p081001Service.GetInventoryLocItems(dcCode, gupCode, custCode, inventoryNo);
		}

		[WebGet]
		public IQueryable<F1913Data> GetInventoryDetailData(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var srv = new P140103Service();
			return srv.GetInventoryDetailData(dcCode, gupCode, custCode, inventoryNo);
		}

		[WebGet]
		public IQueryable<InventoryDoc> GetInventoryDoc(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var srv = new P140103Service();
			return srv.GetInventoryDoc(dcCode, gupCode, custCode, inventoryNo);
		}
		#region  盤點單維護
		[WebGet]
		public IQueryable<F140101Expansion> GetDatas(string dcCode, string gupCode, string custCode,
			string inventoryNo, string inventoryType,
			string inventorySDate, string inventoryEDate,
			string inventoryCycle, string inventoryYear, string inventoryMonth, string status)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			DateTime? sdate = null;
			DateTime? edate = null;
			if (!string.IsNullOrEmpty(inventorySDate))
			{
				sdate = DateTime.Parse(inventorySDate);
			}
			if (!string.IsNullOrEmpty(inventoryEDate))
			{
				edate = DateTime.Parse(inventoryEDate);
				edate = edate.Value.AddDays(1).AddSeconds(-1);
			}

			return f140101Repo.GetDatas(dcCode, gupCode, custCode, inventoryNo, inventoryType, sdate, edate, inventoryCycle, inventoryYear, inventoryMonth, status);
		}
		#endregion
	}

}
