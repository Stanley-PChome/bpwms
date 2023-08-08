using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.FromWcsWebApi.Business.mssql.TransApiServices;

namespace Wms3pl.WebServices.FromWcsWebApi.WebApiLib.mssql.Controllers
{
	public class WmsTaskController : ApiController
	{
		/// <summary>
		/// 出庫完成結果回傳
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsTask/OutWarehouseReceipt")]
		public WcsApiResult<WcsApiOutWarehouseReceiptResultData> OutWarehouseReceipt(OutWarehouseReceiptReq req)
		{
			var comService = new TransCommonService();
			return comService.OutWarehouseReceipt(req);
		}

		/// <summary>
		/// 入庫完成結果
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsTask/InWarehouseReceipt")]
		public WcsApiResult<WcsInWarehouseReceiptApiDataResult> InWarehouseReceipt(InWarehouseReceiptReq req)
		{
			var comService = new TransCommonService();
			return comService.InWarehouseReceipt(req);
		}

		/// <summary>
		/// 盤點完成結果回傳
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsTask/InventoryReceipt")]
		public WcsApiResult<WcsApiInventoryReceiptResultData> InventoryReceipt(InventoryReceiptReq req)
		{
			var comService = new TransCommonService();
			return comService.InventoryReceipt(req);
		}

		/// <summary>
		/// 盤點調整完成結果回傳
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsTask/InventoryAdjustReceipt")]
		public WcsApiResult<WcsApiInventoryAdjustReceiptResultData> InventoryAdjustReceipt(InventoryAdjustReceiptReq req)
		{
			var comService = new TransCommonService();
			return comService.InventoryAdjustReceipt(req);
		}

		/// <summary>
		/// 每日庫存快照回傳
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsTask/SnapshotStocks")]
		public WcsApiResult<WcsApiSnapshotStocksReceiptResultData> SnapshotStocks(SnapshotStocksReceiptReq req)
		{
			var comService = new TransCommonService();
			return comService.SnapshotStocksReceipt(req);
		}

		/// <summary>
		/// 分揀出貨資訊回報
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsTask/ShipToDebit")]
		public WcsApiResult<WcsApiShipToDebitReceiptResultData> ShipToDebit(ShipToDebitReceiptReq req)
		{
			var comService = new TransCommonService();
			return comService.ShipToDebitReceipt(req);
		}

		/// <summary>
		/// 容器釋放狀態查詢
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsTask/SearchUsableContainer")]
		public WcsApiResult<WcsApiSearchUsableContainerReceiptResultData> SearchUsableContainer(SearchUsableContainerReceiptReq req)
		{
			var comService = new TransCommonService();
			return comService.SearchUsableContainerReceipt(req);
		}

		/// <summary>
		/// 容器位置回報
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsTask/ContainerPosition")]
		public WcsApiResult<WcsApiContainerPositionReceiptResultData> ContainerPosition(ContainerPositionReceiptReq req)
		{
			var comService = new TransCommonService();
			return comService.ContainerPositionReceipt(req);
		}
	

        /// <summary>
        /// 補貨超揀申請
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/WmsTask/OverPickApply")]
        public WcsApiResult<WcsApiOverPickApplyReceiptResultData> OverPickApply(OverPickApplyReq req)
        {
            var comService = new TransCommonService();
            return comService.OverPickApply(req);
        }

        /// <summary>
        /// 出庫結果回報(按箱)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
		[HttpPost]
		[Route("api/WmsTask/OutWarehouseContainer")]
		public WcsApiResult<WcsApiOutWarehouseContainerReceiptResultData> OutWarehouseContainerReceipt(OutWarehouseContainerReceiptReq req)
		{
			var comService = new TransCommonService();
			return comService.OutWarehouseContainerReceipt(req);
		}
	

        /// <summary>
        /// 儲位異常回報(A7)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/WmsTask/AutoLocAbnormalNotify")]
        public WcsApiResult<WcsApiAutoLocAbnormalNotifyResultData> AutoLocAbnormalNotify(AutoLocAbnormalNotifyReq req)
        {
            var comService = new TransCommonService();
            return comService.AutoLocAbnormalNotify(req);
        }

		/// <summary>
		/// 分揀機異常回報(A7)
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsTask/SorterAbnormalNotify")]
		public WcsApiResult<WcsApiSorterAbnormalNotifyResultData> SorterAbnormalNotify(SorterAbnormalNotifyReq req)
		{
			var comService = new TransCommonService();
			return comService.SorterAbnormalNotify(req);
		}

    /// <summary>
    /// 包裝完成回報(12)
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/WmsTask/PackageFinish")]
    public WcsApiResult<WcsApiPackageFinishResultData> PackageFinish(PackageFinishReq req)
    {
      var comService = new TransCommonService();
      return comService.PackageFinish(req);
    }

  }
}
