using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
  /// <summary>
  /// 進貨驗收
  /// </summary>
  public class P810119Controller : ApiController
  {
    /// <summary>
    /// 進貨檢驗/驗收資料查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/GetStockReceivedData")]
    public ApiResult GetStockReceivedData(GetStockReceivedDataReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetStockReceivedData", req, () =>
      {
        var service = new P810119Service(new WmsTransaction());
        return service.GetStockReceivedData(req, gupCode);
      });
    }

    /// <summary>
    /// 進貨檢驗/驗收明細資料查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/GetStockReceivedDetData")]
    public ApiResult GetStockReceivedDetData(GetStockReceivedDetDataReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetStockReceivedDetData", req, () =>
      {
        var service = new P810119Service(new WmsTransaction());
        return service.GetStockReceivedDetData(req, gupCode);
      });
    }

    /// <summary>
    /// 進貨檢驗-驗收商品查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/GetRecvItemData")]
    public ApiResult GetRecvItemData(RecvItemDataReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetRecvItemData", req, () =>
      {
        var service = new P810119Service(new WmsTransaction());
        return service.GetRecvItemData(req, gupCode);
      });
    }

		/// <summary>
		/// 進貨檢驗-序號刷讀查詢
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/pda/GetSerialItemData")]
		public ApiResult GetSerialItemData(GetSerialItemDataReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetSerialItemData", req, () =>
			{
				var service = new P810119Service(new WmsTransaction());
				return service.GetSerialItemData(req, gupCode);
			});
		}

		/// <summary>
		/// 進貨檢驗-序號登錄/序號刪除
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/pda/AddOrDelItemSerialNo")]
		public ApiResult AddOrDelItemSerialNo(AddOrDelItemSerialNoReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);
			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "AddOrDelItemSerialNo", req, () =>
			{
				var service = new P810119Service(new WmsTransaction());
				return service.AddOrDelItemSerialNo(req, gupCode);
			});
		}
		/// <summary>
		/// 進貨檢驗-取得不良品登錄查詢
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/pda/GetDefectItemData")]
		public ApiResult GetDefectItemData(GetDefectItemDataReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);
			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetDefectItemData", req, () =>
			{
				var service = new P810119Service(new WmsTransaction());
				return service.GetDefectItemData(req, gupCode);
			});
		}


		/// <summary>
		/// 進貨檢驗-不良品登錄/刪除
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
    [HttpPost]
    [Route("api/pda/AddOrDelDefectItem")]

    public ApiResult AddOrDelDefectItem(AddOrDelDefectItemReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "AddOrDelDefectItem", req, () =>
      {
        var service = new P810119Service(new WmsTransaction());
        return service.AddOrDelDefectItem(req, gupCode);
      });
    }

    /// <summary>
    /// 進貨檢驗/儲存商品驗收註記
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/SaveRecvItemMemo")]

    public ApiResult SaveRecvItemMemo(SaveRecvItemMemoReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "SaveRecvItemMemo", req, () =>
      {
        var service = new P810119Service(new WmsTransaction());
        return service.SaveRecvItemMemo(req, gupCode);
      });

    }

    /// <summary>
    /// 進貨檢驗/取得物流中心進貨可上架倉別清單
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/GetWarhouseInDcTarWarehouseList")]

    public ApiResult GetWarhouseInDcTarWarehouseList(GetWarhouseInDcTarWarehouseListReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, null, null, "GetWarhouseInDcTarWarehouseList", req, () =>
      {
        var service = new P810119Service(new WmsTransaction());
        return service.GetWarhouseInDcTarWarehouseList(req);
      });

    }

    /// <summary>
    /// 進貨檢驗/儲存商品驗收結果
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/CheckandSaveRecvItemData")]

    public ApiResult CheckandSaveRecvItemData(CheckandSaveRecvItemDataReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "CheckandSaveRecvItemData", req, () =>
      {
        var service = new P810119Service(new WmsTransaction());
        return service.CheckandSaveRecvItemData(req, gupCode);
      });

    }

    /// <summary>
    /// 進貨檢驗/驗收完成
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/RecvItemCompleted")]

    public ApiResult RecvItemCompleted(RecvItemCompletedReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "RecvItemCompleted", req, () =>
      {
        var service = new P810119Service(new WmsTransaction());
        return service.RecvItemCompleted(req, gupCode);
      });

    }

    /// <summary>
    /// 進貨檢驗/刪除驗收紀錄
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/DeleteRecvItemData")]

    public ApiResult DelectRecvItemData(DelectRecvItemDataReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "DeleteRecvItemData", req, () =>
      {
        var service = new P810119Service(new WmsTransaction());
        return service.DeleteRecvItemData(req, gupCode);
      });

    }

  }
}
