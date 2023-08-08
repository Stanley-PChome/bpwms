using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
	/// <summary>
	/// 儲位檢核
	/// </summary>
	public partial class SharedService
	{
    /// <summary>
    /// 檢核儲位庫存與傳入調撥明細是否有混品
    /// Step 1 檢核傳入調撥明細是否有混品
    /// Step 2 檢核儲位庫存是否有混品，若不能有混品 傳入參數不能不同品號
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public ExecuteResult CheckItemMixLoc(List<CheckItemTarLocAndParamsMixLoc> items)
    {
      ExecuteResult res = new ExecuteResult { IsSuccessed = true };
      if (!items.Any())
        return res;

      var firstItem = items.First();
      var dcCode = firstItem.DcCode;
      var gupCode = firstItem.GupCode;
      var custCode = firstItem.CustCode;

      CommonService commonService = new CommonService();

      // 取得傳入調撥明細的商品資訊
      var paramF1903Datas = commonService.GetProductList(gupCode, custCode, items.Select(x => x.ItemCode).ToList());

      // 整合傳入調撥明細商品 混品資料
      var data = (from A in items
                  join B in paramF1903Datas
                  on new { GUP_CODE = A.GupCode, CUST_CODE = A.CustCode, ITEM_CODE = A.ItemCode } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE }
                  select new
                  {
                    A.GupCode,
                    A.CustCode,
                    A.ItemCode,
                    A.TarLocCode,
                    IsMixItem = B.LOC_MIX_ITEM       // 是否可儲位混商品
                  }).ToList();

      // 先找出同傳入參數同儲位資料
      var paramsDatas = data.GroupBy(o => o.TarLocCode)
          .Where(x => x.Any(z => z.IsMixItem == "0"))
          .Select(x => new
          {
            TarLocCode = x.Key,
            MixItems = x.GroupBy(z => z.ItemCode).Select(z => z.Key).ToList()
          }).ToList();

            // 驗證傳入參數有無混品
            foreach (var item in paramsDatas)
            {
                // 代表有不允許混品，卻超過1種品號在同儲位
                if (item.MixItems.Count > 1)
                {
                    return new ExecuteResult
                    {
                        IsSuccessed = false,
                        Message = $"品號 { string.Join("、", item.MixItems.Distinct()) } 同一儲位 {item.TarLocCode} 無法混品"
                    };
                }
            }

      // 驗證該儲位庫存是否有不能混品
      if (_f1913Repo == null)
        _f1913Repo = new F1913Repository(Schemas.CoreSchema);

            //撈出該儲位自己外的商品
            var f1913Datas = _f1913Repo.GetDatasByLocs(dcCode, gupCode, custCode, items.Select(x => x.TarLocCode).Distinct().ToList())
                .Where(x => items.All(z => z.ItemCode != x.ITEM_CODE)).ToList();

            var f1903Datas = commonService.GetProductList(gupCode, custCode, f1913Datas.Select(x => x.ITEM_CODE).Distinct().ToList());

      var dataTemp = from A in f1913Datas
                     join B in f1903Datas
                     on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE }
                     select new
                     {
                       A.LOC_CODE,
                       A.ITEM_CODE,
                       B.LOC_MIX_ITEM
                     };

      if (dataTemp.Any())
      {
        foreach (var item in data)
        {
          // 該儲位庫存不允許混品的資料
          var currLocItems = dataTemp.Where(x => x.LOC_CODE == item.TarLocCode &&
          x.LOC_MIX_ITEM == "0" &&
          x.ITEM_CODE != item.ItemCode);
          //var currLocItems = dataTemp.Where(x => x.LOC_CODE == item.TarLocCode &&
          //          x.LOC_MIX_ITEM == "0");

                    if (currLocItems.Any())
                    {
                        return new ExecuteResult
                        {
                            IsSuccessed = false,
                            Message = $"品號 { item.ItemCode } 同一儲位 {item.TarLocCode} 無法混品"
                        };
                    }
                }
            }

      return res;
    }

    public ExecuteResult CheckLocCode(string dcCode, string locCode, string userId)
    {
      if (string.IsNullOrWhiteSpace(locCode))
        return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.LocNotEmpty };
      var item = GetF1912(dcCode, locCode);
      if (item == null)
        return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.LocNotExist };
      var items = _f1912Repo.GetUserCanUseLocCode(dcCode, locCode, userId);
      if (!items.Any())
        return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.LocNotUserPremission };
      var f1980Item = GetF1980(item.DC_CODE, item.WAREHOUSE_ID);
      if (f1980Item == null)
        return new ExecuteResult(false, string.Format(Properties.Resources.LocNotFindWarehouse, locCode));
      return new ExecuteResult { IsSuccessed = true, Message = "" };
    }

    /// <summary>
    /// 儲位檢核
    /// </summary>
    /// <param name="locCode">儲位</param>
    /// <param name="tarDcCode">目的物流中心</param>
    /// <param name="tarWareHouseId">目的倉編號</param>
    /// <param name="userId">使用者</param>
    /// <returns></returns>
    public ExecuteResult CheckLocCode(string locCode, string tarDcCode, string tarWareHouseId, string userId,string itemCode, bool checkFreeze = true)
		{
			var result = CheckLocCode(tarDcCode, locCode, userId);
			if (!result.IsSuccessed)
				return result;
			var item = GetF1912(tarDcCode,locCode);
			var f1980Item = GetF1980(item.DC_CODE, tarWareHouseId);
			if (item.WAREHOUSE_ID != tarWareHouseId)
				return new ExecuteResult { IsSuccessed = false, Message = string.Format("儲位必須為{0}儲位", f1980Item.WAREHOUSE_NAME) };
			if (CheckItemMixLoc(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, itemCode, locCode) == false)
				return new ExecuteResult { IsSuccessed = false, Message = "該商品無法同一儲位混品" };

            if (checkFreeze)
                return CheckLocFreeze(item.DC_CODE, item.LOC_CODE, "2");
            else
                return result;
		}

    /// <summary>
    /// 儲位檢核(檢查儲位是否可用、是否可混商品、是否可混批(效期)、是否儲位被凍結)
    /// </summary>
    /// <param name="locCode">儲位</param>
    /// <param name="tarDcCode">目的物流中心</param>
    /// <param name="tarWareHouseId">目的倉編號</param>
    /// <param name="userId">使用者</param>
    /// <returns></returns>
    public ExecuteResult CheckLocCodeByMixLocAndMixItem(string locCode, string tarDcCode, string tarWareHouseId, string userId, string itemCode, string validDate, bool checkFreeze = true)
    {
      var result = CheckLocCode(tarDcCode, locCode, userId);
      if (!result.IsSuccessed)
        return result;
      var item = GetF1912(tarDcCode, locCode);
      var f1980Item = GetF1980(item.DC_CODE, tarWareHouseId);
      if (item.WAREHOUSE_ID != tarWareHouseId)
        return new ExecuteResult { IsSuccessed = false, Message = string.Format("儲位必須為{0}儲位", f1980Item.WAREHOUSE_NAME) };
      if (CheckItemMixLoc(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, itemCode, locCode) == false)
        return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.ItemCannotMixLoc, itemCode, locCode) };
      if (CheckItemMixBatch(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, itemCode, locCode, validDate.ToString()) == false)
        return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.ItemCannotMixBatch, itemCode) };

      if (checkFreeze)
        return CheckLocFreeze(item.DC_CODE, item.LOC_CODE, "2");
      else
        return result;
    }

    public ExecuteResult CheckItemLocStatus(string dcCode, string tarDcCode, string gupCode, string custCode, string itemCode, string srcLocCode, string tarLocCode, string validDate, bool isAutoWarehourse = false, string tarWarehouseId = null, bool isCheck = true)
    {
      ExecuteResult result = new ExecuteResult { IsSuccessed = true };
      var messages = new List<string>();
      if (isCheck && !isAutoWarehourse && (tarWarehouseId == null || !tarWarehouseId.StartsWith("M")))
      {
        if (CheckItemMixLoc(tarDcCode, gupCode, custCode, itemCode, tarLocCode) == false)
        {
          messages.Add(string.Format(Properties.Resources.ItemCannotMixLoc, itemCode, tarLocCode));
          result.IsSuccessed = false;
        }
        if (CheckItemMixBatch(tarDcCode, gupCode, custCode, itemCode, tarLocCode, validDate) == false)
        {
          messages.Add(string.Format(Properties.Resources.ItemCannotMixBatch, itemCode));
          result.IsSuccessed = false;
        }
      }
      var checkResult = new ExecuteResult();
      if (!string.IsNullOrWhiteSpace(srcLocCode))
      {
        checkResult = CheckLocFreeze(dcCode, srcLocCode, "1");
        if (!checkResult.IsSuccessed)
        {
          messages.Add(checkResult.Message);
          result.IsSuccessed = false;
        }
      }
      if (!string.IsNullOrWhiteSpace(tarLocCode))
      {
        checkResult = CheckLocFreeze(dcCode, tarLocCode, "2");
        if (!checkResult.IsSuccessed)
        {
          messages.Add(checkResult.Message);
          result.IsSuccessed = false;
        }
      }
      result.Message = string.Join("\r\n", messages);
      return result;

    }

    // 自動倉的檢核商品儲位
    public ExecuteResult CheckItemLocStatus(string dcCode, string srcLocCode, string tarLocCode)
        {
            ExecuteResult result = new ExecuteResult { IsSuccessed = true };
            
            var checkResult = CheckLocFreeze(dcCode, srcLocCode, "1");
            if (!checkResult.IsSuccessed)
            {
                result.Message += checkResult.Message + Environment.NewLine;
                result.IsSuccessed = false;
            }
            checkResult = CheckLocFreeze(dcCode, tarLocCode, "2");
            if (!checkResult.IsSuccessed)
            {
                result.Message += checkResult.Message + Environment.NewLine;
                result.IsSuccessed = false;
            }
            return result;

        }


    /// <summary>
    /// 檢查 該商品是否符合儲位混商品條件
    /// 回傳 False 代表檢核不通過
    /// </summary>
    public bool CheckItemMixLoc(string dcCode, string gupCode, string custCode, string itemCode, string locCode)
    {
      var f1909Item = GetF1909(gupCode, custCode);
      //貨主允許同儲位混品
      if (f1909Item != null && f1909Item.MIX_LOC_ITEM == "1")
        return true;
      var f1903Item = GetF1903s(gupCode, custCode, new List<string> { itemCode }).FirstOrDefault();
      if (f1903Item != null)
      {
        if (f1903Item.LOC_MIX_ITEM == "0")  //儲位不能混商品 , 進行以下檢核
        {
          if (_f1903Repo == null)
            _f1903Repo = new F1903Repository(Schemas.CoreSchema);
          var locData = _f1903Repo.GetItemMixItemLoc(dcCode, gupCode, custCode, itemCode, locCode);
          if (locData != null && locData.Count() > 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    public ExecuteResult CheckItemMixBatch(List<CheckItemTarLocMixLoc> items)
		{
			if (!items.Any())
				return new ExecuteResult(true);
			var gupCode = items.First().GupCode;
			var custCode = items.First().CustCode;
			var itemCode = items.First().ItemCode;

			var f1903Item = GetF1903s(gupCode, custCode, new List<string> { itemCode }).FirstOrDefault();
			foreach (var checkItemTarLocMixLoc in items)
			{
				if (f1903Item != null && f1903Item.MIX_BATCHNO == "0" && checkItemTarLocMixLoc.CountValidDate > 1)
					return new ExecuteResult(false, "該商品無法混批(效期)");

			}
			return new ExecuteResult(true);
		}

		/// <summary>
		/// 檢查 該商品是否符合儲位混批(效期)條件
		/// 回傳 False 代表檢核不通過
		/// </summary>
		public bool CheckItemMixBatch(string dcCode, string gupCode, string custCode, string itemCode, string locCode, string validDate)
		{
			var validDateResult = (string.IsNullOrEmpty(validDate)) ? ((DateTime?)null) : Convert.ToDateTime(validDate);
			var f1903Item = GetF1903s(gupCode, custCode, new List<string> { itemCode }).FirstOrDefault();
			if (f1903Item != null)
			{
				if (f1903Item.MIX_BATCHNO == "0")  //若不能儲位混批(效期) , 進行以下檢核
				{
					if (_f1903Repo == null)
						_f1903Repo = new F1903Repository(Schemas.CoreSchema);

					var locData = _f1903Repo.GetItemMixBatchLoc(dcCode, gupCode, custCode, itemCode,locCode, validDateResult);
					if (locData != null && locData.Any())
					{
						return false;
					}
				}
			}
			return true;
		}

    /// <summary>
    /// 檢查 該商品是否符合儲位混批(效期)條件
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public ExecuteResult CheckStockItemMixBatch(List<CheckStockItemTarLocMixLoc> items)
    {
      var messages = new List<string>();
      if (_f1913Repo == null)
        _f1913Repo = new F1913Repository(Schemas.CoreSchema);

      var gupItems = items.GroupBy(x => new { x.DcCode, x.GupCode, x.CustCode });
      if (gupItems.Count() > 1)
        return new ExecuteResult(false, "混品檢查不可傳入多個物流中心、業主、貨主");
      if (items == null || !items.Any())
        return new ExecuteResult(true);

      var firstItem = items.First();
      var f1903s = GetF1903s(firstItem.GupCode, firstItem.CustCode, items.Select(x => x.ItemCode).ToList());
      var f1913s = _f1913Repo.GetDatasByItems(firstItem.DcCode, firstItem.GupCode, firstItem.CustCode, items.Select(x => x.ItemCode).Distinct().ToList()).ToList();

      var combindMixBatch = from a in items
                            join b in f1903s on new { a.GupCode, a.CustCode, a.ItemCode } equals new { GupCode = b.GUP_CODE, CustCode = b.CUST_CODE, ItemCode = b.ITEM_CODE }
                            where b.MIX_BATCHNO == "0"
                            select a;

      var NgItems = new List<CheckStockItemTarLocMixLoc>();
      foreach (var f1903Item in combindMixBatch)
      {
        var chkMixBatch = f1913s.Where(
            o => o.ITEM_CODE == f1903Item.ItemCode && o.LOC_CODE == f1903Item.TarLocCode && o.VALID_DATE != f1903Item.ValidDate);

        if (chkMixBatch.Any())
          NgItems.Add(f1903Item);
      }

      if (NgItems.Any())
      {
        return new ExecuteResult()
        {
          IsSuccessed = false,
          Message =
            string.Join("\r\n",
            NgItems.GroupBy(x => new { x.ItemCode, x.TarLocCode, x.ValidDate }).Distinct().Select(x => $"商品:{x.Key.ItemCode} 上架儲位:{x.Key.TarLocCode} 效期:{x.Key.ValidDate.Value.ToString("yyyy/MM/dd")} 無法混批(效期)"))
        };
      }
      //message += "該商品無法混批(效期)";
      return new ExecuteResult(true);
    }

    #region 儲位狀態檢核
    /// <summary>
    /// 檢查 該儲位狀態是否凍結
    /// 回傳 False 代表檢核不通過
    /// </summary>
    /// <param name="locType"> 1: 來源倉  2 :目的倉 </param>
    public ExecuteResult CheckLocFreeze(string dcCode, string locCode, string locType)
		{
			//01:使用中    02:凍結進    03:凍結出    04:凍結進出 (F1943)
			// 來源倉 : 只檢查 03:凍結出    04:凍結進出
			// 目的倉 : 只檢查 02:凍結進    04:凍結進出
			var f1912Data = GetF1912(dcCode, locCode);
			if (f1912Data != null)
			{
				if (CheckLocFreeze(f1912Data, locType))
					return new ExecuteResult(true);
				else
				{
					if (locType == "1")
						return new ExecuteResult(false, string.Format(Properties.Resources.SrcLocHasFreeze, locCode));
					else
						return new ExecuteResult(false, string.Format(Properties.Resources.TarLocHasFreeze, locCode));
				}
			}
			return new ExecuteResult(true);
		}

		/// <summary>
		///  檢查 該儲位狀態是否凍結
		///  回傳 False 代表檢核不通過
		/// </summary>
		/// <param name="f1912"></param>
		/// <param name="locType"></param>
		/// <returns></returns>
		private bool CheckLocFreeze(F1912 f1912,string locType)
		{
			//01:使用中    02:凍結進    03:凍結出    04:凍結進出 (F1943)
			// 來源倉 : 只檢查 03:凍結出    04:凍結進出
			// 目的倉 : 只檢查 02:凍結進    04:凍結進出
			if (locType == "1" && f1912.NOW_STATUS_ID == "03")
				return false;
			else if (locType == "2" && f1912.NOW_STATUS_ID == "02")
				return false;
			else if (f1912.NOW_STATUS_ID == "04")
				return false;
			return true;
		}

		#endregion

		#region 儲位溫層與商品溫層檢查
		public string CheckItemLocTmprByWareHouse(string dcCode,string gupCode,string custCode,List<string> itemCodes,string warehouseId)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1980Repo = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);
			var items = f1903Repo.GetDatasByItems(gupCode, custCode, itemCodes);
			var locTmpr = f1980Repo.GetWareHouseTmprTypeByWareHouse(dcCode, warehouseId);
			foreach (var itemCode in itemCodes)
				if (!items.Any(o => o.ITEM_CODE == itemCode))
					return string.Format("商品{0}不存在", itemCode);

			foreach (var item in items)
			{
				var result = CheckItemLocTmpr(locTmpr, item);
				if (!result.IsSuccessed)
					return result.Message;
			}
			return string.Empty;
		}

		/// <summary>
		/// 檢查商品與儲位溫層是否相同
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="locCode"></param>
		/// <returns></returns>
		public string CheckItemLocTmpr(string dcCode, string gupCode, string itemCode, string custCode, string locCode)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema,_wmsTransaction);
			var item = f1903Repo.GetDatasByItems(gupCode, custCode, new List<string> { itemCode }).FirstOrDefault();
			var f1980Repo = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);
			var locTmpr = f1980Repo.GetWareHouseTmprTypeByLocCodes(new List<string> { dcCode }, new List<string> { locCode }).FirstOrDefault();
			if (item == null)
				return string.Format("商品{0}不存在",itemCode);
			var result = CheckItemLocTmpr(locTmpr, item);
			return result.Message;
		}

		private ExecuteResult CheckItemLocTmpr(WareHouseTmprTypeByLocCode wareHouseTmprTypeByLocCode,F1903 f1903)
		{
			//  商品溫度              倉別溫層
			//  02(恆溫),03(冷藏) =>  02(低溫)
			//  01(常溫)          =>  01(常溫)
			//  04(冷凍)          =>  03(冷凍)
			if (GetWareHouseTmprByItemTmpr(f1903.TMPR_TYPE).Split(',').Contains(wareHouseTmprTypeByLocCode.TMPR_TYPE))
				return new ExecuteResult(true);
			return new ExecuteResult(false, string.Format(Properties.Resources.LocTmprNotInItemTmpr, wareHouseTmprTypeByLocCode.LOC_CODE, GetWareHouseTmprName(wareHouseTmprTypeByLocCode.TMPR_TYPE), f1903.ITEM_NAME, GetItemTmprName(f1903.TMPR_TYPE)));
		}

		public string GetItemTmprName(string itemTmprCode)
		{
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var item = f000904Repo.GetDatas("F1903", "TMPR_TYPE").FirstOrDefault(o=>o.VALUE == itemTmprCode);
			return item?.NAME;
		}
		public string GetWareHouseTmprName(string wareHouseTmprCode)
		{
			var f000904Repo = new F000904Repository(Schemas.CoreSchema, _wmsTransaction);
			var item = f000904Repo.GetDatas("F1980", "TMPR_TYPE").FirstOrDefault(o=> o.VALUE == wareHouseTmprCode);
			return item.NAME;
		}

		public string GetWareHouseTmprByItemTmpr(string gupCode, string itemCode, string custCode)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema,_wmsTransaction);
			var item = f1903Repo.Find(o => o.GUP_CODE == gupCode && o.ITEM_CODE == itemCode && o.CUST_CODE == custCode);
			return GetWareHouseTmprByItemTmpr(item.TMPR_TYPE);
		}
		/// <summary>
		/// 商品溫層與倉別溫層對照表
		///  商品溫度              倉別溫層
		///  02(恆溫),03(冷藏) =>  02(低溫)
		///  01(常溫)          =>  01(常溫)
		///  04(冷凍)          =>  03(冷凍) 
		/// </summary>
		/// <param name="itemTmpr"></param>
		/// <returns></returns>
		public string GetWareHouseTmprByItemTmpr(string itemTmpr)
		{
			var wareHouseTmpr = "";
			switch (itemTmpr)
			{
				case "01":
					wareHouseTmpr = "01";
					break;
				case "02":
					wareHouseTmpr = "02";
					break;
				case "03":
					wareHouseTmpr = "02";
					break;
				case "04":
					wareHouseTmpr = "03";
					break;
			}
			return wareHouseTmpr;
		}

		#endregion

		/// <summary>
		/// 檢查儲位是否為其他貨主使用
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="locCode">儲位編號</param>
		/// <param name="nowCustCode">貨主編號</param>
		/// <returns></returns>
		public ExecuteResult CheckNowCustCodeLoc(string dcCode,string locCode,string nowCustCode)
		{
			ExecuteResult result = new ExecuteResult { IsSuccessed = true };
			var f1912Repo = new F1912Repository(Schemas.CoreSchema,_wmsTransaction);
			var data = f1912Repo.GetDifferentNowCustCodeLoc(dcCode, locCode, nowCustCode);
			if (data.Any())
				return new ExecuteResult(false, string.Format(Properties.Resources.LocHasOtherCustUsed, locCode));
			return result;
		}

		/// <summary>
		/// 儲位檢核(多筆)
		/// </summary>
		/// <param name="checkLocs">檢核儲位清單</param>
		/// <returns></returns>
		public List<CheckLoc> CheckMultiLocCode(List<CheckLoc> checkLocs)
		{
			var messages = new List<string>();
			var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
			var dcs = checkLocs.Select(x => x.DcCode).Distinct().ToList();
			var locs = checkLocs.Select(x => x.LocCode).Distinct().ToList();
			var f1912s = f1912Repo.GetDatas(dcs, locs);
			foreach(var checkLoc in checkLocs)
			{
				var loc = f1912s.FirstOrDefault(x => x.DC_CODE == checkLoc.DcCode && x.LOC_CODE == checkLoc.LocCode);
				if (loc == null)
				 checkLoc.Message = string.Format("{0}儲位{1}不存在 , 或儲區相關資料錯誤", checkLoc.LocType == "1" ? "來源" : "目的", checkLoc.LocCode);
				else if (!CheckLocFreeze(loc, checkLoc.LocType))
					checkLoc.Message = string.Format("{0}儲位{1}狀態為凍結{2}或凍結進出", checkLoc.LocType == "1" ? "來源" : "目的", checkLoc.LocCode, checkLoc.LocType == "1" ? "出" : "進");
				else if(!string.IsNullOrWhiteSpace(checkLoc.WarehouseId) && loc.WAREHOUSE_ID != checkLoc.WarehouseId)
					checkLoc.Message = string.Format("{0}倉{1}無此{0}儲位{2}", checkLoc.LocType == "1" ? "來源" : "目的",checkLoc.WarehouseId, checkLoc.LocCode);
				checkLoc.IsSuccessed = string.IsNullOrWhiteSpace(checkLoc.Message);
			}
			return checkLocs.Where(x=> !x.IsSuccessed).ToList();
		}

		/// <summary>
		/// 儲位商品檢核(多筆)
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="checkLocItems"></param>
		/// <returns></returns>
		public List<CheckLocItem> CheckMultiLocItem(string gupCode,string custCode,List<CheckLocItem> checkLocItems)
		{
			var messages = new List<string>();
			var f1980Repo = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var dcs = checkLocItems.Select(x => x.DcCode).Distinct().ToList();
			var locs = checkLocItems.Select(x => x.LocCode).Distinct().ToList();
			var locTmprs = f1980Repo.GetWareHouseTmprTypeByLocCodes(dcs, locs);
			var items = f1903Repo.GetDatasByItems(gupCode, custCode, checkLocItems.Select(x => x.ItemCode).ToList());
			foreach (var checkLocItem in checkLocItems)
			{
				var checkLocResult = CheckLocCode(checkLocItem.DcCode, checkLocItem.LocCode,Current.Staff);
				if (!checkLocResult.IsSuccessed)
				{
					checkLocItem.Message = checkLocResult.Message;
					continue;
				}
				var item = items.FirstOrDefault(x => x.GUP_CODE == gupCode && x.ITEM_CODE == checkLocItem.ItemCode);
				if(item == null)
				{
					checkLocItem.Message = string.Format("商品{0}不存在", checkLocItem.ItemCode);
					continue;
				}
				var locTmpr = locTmprs.First(x => x.DC_CODE == checkLocItem.DcCode && x.LOC_CODE == checkLocItem.LocCode);
				//檢查儲位與商品溫層是否一致
				var checkItemTmprResult = CheckItemLocTmpr(locTmpr, item);
				if(!checkItemTmprResult.IsSuccessed)
				{
					checkLocItem.Message = checkItemTmprResult.Message;
					continue;
				}

				checkLocItem.IsSuccessed = true;
			}
			return checkLocItems.Where(x => !x.IsSuccessed).ToList();
		}

        /// <summary>
        /// 計算可用容積
        /// </summary>
        /// <param name="Length">儲位長度</param>
        /// <param name="Depth">儲位深度</param>
        /// <param name="Height">儲位高度</param>
        /// <param name="VolumeRate">容積率(%)</param>
        /// <returns></returns>
        public static decimal GetUsefulColumn(int Length, int Depth, int Height, decimal VolumeRate)
        {
            return (decimal)Length * Depth * Height * VolumeRate / 100m; //VOLUME_RATE 是0~100，所以要除100
        }

    }
}
