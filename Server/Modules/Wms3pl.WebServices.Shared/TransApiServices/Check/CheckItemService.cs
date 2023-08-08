using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
	public class CheckItemService
	{
		private TransApiBaseService _tacService;
		private CommonService _commonService;

		public CheckItemService()
		{
			_tacService = new TransApiBaseService();
			_commonService = new CommonService();
		}

		public void CheckValueNotZero(List<ApiResponse> res, PostItemDataItemsModel item)
		{
			List<string> failCols = new List<string>();
			List<string> chkCols = new List<string> { "UnitQty", "PackLength", "PackWidth", "PackHeight", "PackWeight", "PickSafetyQty" };

			chkCols.ForEach(colName =>
			{
        if (colName == "PackWeight")
        {
          if (!DataCheckHelper.CheckDataEqualOrGreaterThanZero(item, colName))
          {
            failCols.Add(colName);
          }
        }
        else
        {
          if (!DataCheckHelper.CheckDataNotZero(item, colName))
          {
            failCols.Add(colName);
          }
        }
			});

			if (failCols.Count > 0)
			{
				res.Add(new ApiResponse
				{
					No = item.ItemCode,
					MsgCode = "20019",
					MsgContent = string.Format(_tacService.GetMsg("20019"),
						item.ItemCode,
						string.Join("、", failCols))
				});
			}
		}

		/// <summary>
		/// 檢核商品大分類是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="item"></param>
		/// <param name="aCodes"></param>
		public void CheckLType(List<ApiResponse> res, PostItemDataItemsModel item, List<string> aCodes)
		{
			if (!aCodes.Contains(item.Ltype))
				res.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20776", MsgContent = string.Format(_tacService.GetMsg("20776"), item.ItemCode, item.Ltype) });
		}

		/// <summary>
		/// 檢核商品中分類是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="item"></param>
		/// <param name="bCodes"></param>
		public void CheckMType(List<ApiResponse> res, PostItemDataItemsModel item, List<string> bCodes)
		{
			if (!bCodes.Contains(item.Mtype))
				res.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20777", MsgContent = string.Format(_tacService.GetMsg("20777"), item.ItemCode, item.Mtype) });
		}

		/// <summary>
		/// 檢核商品小分類是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="item"></param>
		/// <param name="cCodes"></param>
		public void CheckSType(List<ApiResponse> res, PostItemDataItemsModel item, List<string> cCodes)
		{
			if (!cCodes.Contains(item.Stype))
				res.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20778", MsgContent = string.Format(_tacService.GetMsg("20778"), item.ItemCode, item.Stype) });
		}

		/// <summary>
		/// 檢核商品單位編號
		/// </summary>
		/// <param name="res"></param>
		/// <param name="itemBom"></param>
		/// <param name="unitIdList"></param>
		public void CheckUnitId(List<ApiResponse> res, PostItemDataItemsModel itemBom, List<string> unitIdList)
		{
			if (!unitIdList.Contains(itemBom.UnitId))
				res.Add(new ApiResponse { No = itemBom.ItemCode, MsgCode = "21078", MsgContent = string.Format(_tacService.GetMsg("21078"), itemBom.ItemCode, itemBom.UnitId) });
		}

		/// <summary>
		/// 檢核上架倉別代碼是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="item"></param>
		/// <param name="typeIds"></param>
		public void CheckWarehouse(List<ApiResponse> res, PostItemDataItemsModel item, List<string> typeIds)
		{
			if (!typeIds.Contains(item.PickWarehouse))
				res.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20779", MsgContent = string.Format(_tacService.GetMsg("20779"), item.ItemCode, item.PickWarehouse) });
		}

		/// <summary>
		/// 檢核揀貨倉庫編號是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="item"></param>
		/// <param name="warehouseIds"></param>
		public void CheckWarehouseId(List<ApiResponse> res, PostItemDataItemsModel item, List<string> warehouseIds)
		{
			if (!string.IsNullOrWhiteSpace(item.PickWarehouseId) && !warehouseIds.Contains(item.PickWarehouseId))
				res.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20780", MsgContent = string.Format(_tacService.GetMsg("20780"), item.ItemCode, item.PickWarehouseId) });
		}

		/// <summary>
		/// 檢核揀貨倉庫編號是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="item"></param>
		public void CheckSerialRule(List<ApiResponse> res, PostItemDataItemsModel item)
		{
			List<string> values = new List<string> { "0", "1" };

			if (!values.Contains(item.SerialRule))
				res.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20782", MsgContent = string.Format(_tacService.GetMsg("20782"), item.ItemCode) });
		}

		/// <summary>
		/// 檢核欄位是否為(0:否, 1:是)
		/// </summary>
		/// <param name="res"></param>
		/// <param name="item"></param>
		public void CheckBoolean(List<ApiResponse> res, PostItemDataItemsModel item)
		{
			List<string> failCols = new List<string>();
			List<string> chkCols = new List<string> { "IsFragile", "IsSpill", "MultiFlag", "IsCarton", "MixBatchno", "LocMixItem", "BundleSerialloc", "BundleSerialno", "CDFlag", "AllowOrdItem", "MakenoRequ", "ItemReturn" };
			List<string> values = new List<string> { "0", "1" };

			chkCols.ForEach(colName =>
			{
				string value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(item, colName));
				if (!values.Contains(value))
					failCols.Add(colName);
			});

			if (failCols.Count > 0)
			{
				res.Add(new ApiResponse
				{
					No = item.ItemCode,
					MsgCode = "20781",
					MsgContent = string.Format(_tacService.GetMsg("20781"),
						item.ItemCode,
						string.Join("、", failCols))
				});
			}
		}

		/// <summary>
		/// 檢核首次進貨日
		/// </summary>
		/// <param name="res"></param>
		/// <param name="item"></param>
		public void CheckFirstInDate(List<ApiResponse> res, PostItemDataItemsModel item)
		{
			string colName = "NeedExpired";
			List<string> values = new List<string> { "0", "1" };
			if (item.FirstInDate != null)
			{
				// 是否為效期商品為空，判斷必須填入"0"或"1"
				string value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(item, colName));
				if (!string.IsNullOrWhiteSpace(value) && !values.Contains(value))
				{
					res.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20784", MsgContent = string.Format(_tacService.GetMsg("20784"), item.ItemCode) });
				}
				// 是否為效期商品為否，總保存天數、允收天數及警示天數必須為空
				else if (value == "0" && (item.SaveDay != null || item.AllDln != null || item.AllShp != null))
				{
					res.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20785", MsgContent = string.Format(_tacService.GetMsg("20785"), item.ItemCode) });
				}
				// 是否為效期商品為是，總保存天數、允收天數及警示天數不能為空且大於30
				else if (value == "1" && (item.SaveDay <= 30 || item.SaveDay == null || item.AllDln == null || item.AllDln <= 0 || item.AllShp == null || item.AllShp <= 0))
				{
					res.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20786", MsgContent = string.Format(_tacService.GetMsg("20786"), item.ItemCode) });
				}
			}
		}

		/// <summary>
		/// 序號綁儲位則:當序號綁儲位商品，序號商品必須為1
		/// </summary>
		/// <param name="res"></param>
		/// <param name="item"></param>
		public void CheckBoundleSerialLoc(List<ApiResponse> res, PostItemDataItemsModel item)
		{
			if (item.BundleSerialloc == "1" && item.BundleSerialno != "1")
			{
				res.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20787", MsgContent = string.Format(_tacService.GetMsg("20787"), item.ItemCode) });
			}

		}

    public void CheckOriVnrCode(List<ApiResponse> res, PostItemDataItemsModel item, List<string> vnrCodeList)
    {
      if (!string.IsNullOrWhiteSpace(item.OriVnrCode) && !vnrCodeList.Contains(item.OriVnrCode))
        // [編號{0}]原廠商編號{1}不存在
        res.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20074", MsgContent = string.Format(_tacService.GetMsg("20074"), item.ItemCode, item.OriVnrCode) });
    }


  }
}
