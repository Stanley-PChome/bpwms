using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Shared.Services
{
	/// <summary>
	/// 建議儲位
	/// </summary>
	public partial class SharedService
	{
		private string _dcCode;
		private string _gupCode;
		private string _custCode;
		private string _itemCode;
		private string _wareHouseTmpr;
		private List<ItemLocPriorityInfo> _itemPickLocPriorityInfos;
		private List<ItemLocPriorityInfo> _itemResupplyLocPriorityInfos;
		private List<MixItemLoc> _itemMixItemLocList;
		private List<NearestLoc> _itemNearestLocListByCust;
		private List<NearestLoc> _itemNearestLocListByGup;
		private List<string> _excludeItemLocs;
		/// <summary>
		/// 倉別是否要計算材積
		/// </summary>
		private Dictionary<string, bool> _warehouseIsCalcVolumnDict;

		private F1913Repository _f1913Repo;
		private F1912Repository _f1912Repo;
		private F1905Repository _f1905Repo;
		private F1903Repository _f1903Repo;
		private F1909Repository _f1909Repo;
		private F050003Repository _f050003Repo;
		#region 取得建議目的儲位

		/// <summary>
		/// 取得建議目的儲位
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="qty"></param>
		/// <param name="validDate">效期</param>
		/// <param name="enterDate">入庫日</param>
		/// <param name="targetWarehouseType">目的倉別型態(注意:這不是 WAREHOUSE_ID )</param>
		/// <param name="excludeLocs">排除的儲位編號：同一個交易內已取得的儲位(迴圈取得建議儲位時，必須避免之前分配過的儲位再次被使用，故用此ref帶入)</param>
		/// <param name="isIncludeResupply">是否包含補貨區</param>
		/// <param name="targetWarehouseId">若有傳入倉別編號，表示只過濾倉別編號，不會過濾倉別型態</param>
		/// <param name="appointNeverItemMix">指定不允許混品(原本會根據設定去判斷，使用此屬性代表不用判斷設定，直接不準混品，設為false則為依設定判斷)</param>
		/// <param name="notAllowSeparateLoc">是否不允許商品拆儲位放</param>
		/// <returns></returns>
		public List<SuggestLoc> GetSuggestLocs(string gupCode, string custCode, string dcCode, string itemCode, long qty, DateTime validDate, DateTime enterDate, string targetWarehouseType, ref List<F1912> excludeLocs, bool isIncludeResupply = true, string targetWarehouseId = "", bool appointNeverItemMix = false, bool notAllowSeparateLoc = false)
		{
			if (_f1913Repo == null)
				_f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			if (_f1912Repo == null)
				_f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
            
            //降低同一商品取資料速度(改取暫存而非DB,減少DB存取)
            if (dcCode != _dcCode || gupCode != _gupCode || custCode != _custCode || itemCode != _itemCode)
			{
				_dcCode = dcCode;
				_gupCode = gupCode;
				_custCode = custCode;
				_itemCode = itemCode;
				_excludeItemLocs = null;
				_wareHouseTmpr = null;
				_itemPickLocPriorityInfos = null;
				_itemNearestLocListByCust = new List<NearestLoc>();
				_itemNearestLocListByGup = new List<NearestLoc>();
				_itemMixItemLocList = new List<MixItemLoc>();
			}
			if (_wareHouseTmpr == null)
				_wareHouseTmpr = GetWareHouseTmprByItemTmpr(gupCode, itemCode, custCode);
            
            //取出該商品所在儲位已有混其他商品的儲位
            if (_excludeItemLocs == null)
				_excludeItemLocs = _f1913Repo.GetItemMixItemLoc(dcCode, gupCode, custCode, itemCode).ToList();

            var suggestLocs = new List<SuggestLoc>();

			if (string.IsNullOrEmpty(targetWarehouseType) && !string.IsNullOrEmpty(targetWarehouseId))
				targetWarehouseType = targetWarehouseId.Substring(0, 1);

			if (targetWarehouseType != null && targetWarehouseType.ToUpper() == "W") //如果是流通加工倉
			{
                #region 流通加工倉
                var itemLocPriorityInfos = GetGetItemPickLocPriorityInfo(gupCode, custCode, dcCode, itemCode, "A", targetWarehouseType, targetWarehouseId).Where(a => !_excludeItemLocs.Contains(a.LOC_CODE)).ToList();
				if (itemLocPriorityInfos.Any()) //流通加工倉儲位有此商品儲位，不管混批、材積只要有同商品的儲位則全數放入
					GetSuggestLocBySameItem(dcCode, itemLocPriorityInfos, ref qty, ref suggestLocs);
				else
				{
					//找其他流通加工倉一般儲位的空儲位
					GetSuggestLocByEmpty(gupCode, custCode, dcCode, null, targetWarehouseType, "A", ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc, targetWarehouseId);
					if (qty > 0)  //尚有商品沒有建議儲位
					{
						//取得混品儲位
						GetSuggestLocByMixItem(gupCode, custCode, dcCode, itemCode, null, null, targetWarehouseType, "A", ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc, targetWarehouseId);
					}
				}
				if (qty > 0) //尚有商品沒有建議儲位，給來源儲位
				{
					GetSameSourceLoc(dcCode, qty, ref suggestLocs);
				}
                #endregion 流通加工倉

                return suggestLocs;
			}
            #region 非流通加工倉
            // 是否為有指定上架倉別
            if (!string.IsNullOrWhiteSpace(targetWarehouseId))
			{
				GetAutomaticWarehouse(qty, suggestLocs, targetWarehouseId);
			}
			if (_f1903Repo == null)
				_f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1903Item = _f1903Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ITEM_CODE == itemCode);
			//需考慮序號綁儲位商品，若有其他倉別不需考慮時要再加入邏輯條件
			var isBundleSerialLoc = (f1903Item.BUNDLE_SERIALLOC == "1" && targetWarehouseType != null && targetWarehouseType.ToUpper() != "W");
			if (_f1909Repo == null)
				_f1909Repo = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1909Item = _f1909Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode);
			//如果貨主設定檔設定序號綁儲位商品可以混序號 則當作不是序號綁儲位商品取建議儲位
			if (f1909Item.MIX_SERIAL_NO == "1")
				isBundleSerialLoc = false;
			if (_f050003Repo == null)
				_f050003Repo = new F050003Repository(Schemas.CoreSchema, _wmsTransaction);
			//判斷是否是特殊商品(促銷商品)，須放在黃金揀貨區
			var isGoldenItem = _f050003Repo.GetSpecialItemCount(itemCode, gupCode, custCode, dcCode) > 0;
			if (!isGoldenItem)
			{
				//檢查是否為越庫商品，若是則需放在黃金揀貨區
				isGoldenItem = f1903Item.C_D_FLAG == "1";
			}

            if (isGoldenItem)
			{
                #region 取得黃金揀貨區儲位
                var itemLocPriorityInfos = GetGetItemPickLocPriorityInfo(gupCode, custCode, dcCode, itemCode, "B", targetWarehouseType, targetWarehouseId).Where(a => !_excludeItemLocs.Contains(a.LOC_CODE)).ToList();
				if (!isBundleSerialLoc && itemLocPriorityInfos.Any()) //不需考慮序號綁儲位商品，黃金儲位有此商品儲位，不管混批、材積只要有同商品的儲位則全數放入
					GetSuggestLocBySameItem(dcCode, itemLocPriorityInfos, ref qty, ref suggestLocs);
				else
				{
					//找其他黃金儲位的空儲位
					GetSuggestLocByEmpty(gupCode, custCode, dcCode, null, targetWarehouseType, "B", ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc, targetWarehouseId);
					if (qty > 0)  //尚有商品沒有建議儲位
					{
						//取得混品儲位
						GetSuggestLocByMixItem(gupCode, custCode, dcCode, itemCode, null, null, targetWarehouseType, "B", ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc, targetWarehouseId);
					}
				}
				if (qty > 0) //尚有商品沒有建議儲位，給來源儲位
				{
					GetSameSourceLoc(dcCode, qty, ref suggestLocs);
				}
                #endregion 取得黃金揀貨區儲位
                return suggestLocs;
			}
            
            //取得商品材積
            if (_f1905Repo == null)
				_f1905Repo = new F1905Repository(Schemas.CoreSchema);
			var f1905 = _f1905Repo.Find(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.ITEM_CODE == itemCode);

			// 取商品狀態設定  (混批 / 混品 /批號控管)
			var canMixItem = appointNeverItemMix ? false  : (f1903Item.LOC_MIX_ITEM == "1");
			var canMixBatchNo = appointNeverItemMix ? false : (f1903Item.MIX_BATCHNO == "1" && !isBundleSerialLoc) ; //需考慮序號綁儲位商品一定不可混批
			var makeNoRequ = f1903Item.MAKENO_REQU == "1";
            
            //包含補貨區時，先找補貨區儲位
            if (isIncludeResupply)
			{
				#region 取得補貨區儲位
				var itemLocPriorityInfos = GetGetItemResupplyLocPriorityInfo(gupCode, custCode, dcCode, itemCode, targetWarehouseType, targetWarehouseId);
				if (itemLocPriorityInfos.Any()) //補貨區有此商品儲位
				{
					//尋找已有此商品且同效期同入庫日的儲位(商品設為批號控管就不找)，如果沒有才找最接近已放此商品儲位的空儲位
					GetSuggestLocByNearest(gupCode, custCode, dcCode, f1905, validDate, enterDate, itemLocPriorityInfos, _excludeItemLocs, canMixItem, isBundleSerialLoc, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc, makeNoRequ, targetWarehouseId);
				}
				else  //補貨區沒有此商品儲位
				{
					//找其他空儲位
					GetSuggestLocByEmpty(gupCode, custCode, dcCode, f1905, targetWarehouseType, "C", ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc, targetWarehouseId);
				}
				#endregion 取得補貨區儲位
			}

            //不包含補貨區，或找完補貨區尚有商品沒有建議儲位，則找一般的儲位
            if (qty > 0)
			{
				#region 取得一般儲區儲位
				//找最接近儲位
				var itemLocPriorityInfos = GetGetItemPickLocPriorityInfo(gupCode, custCode, dcCode, itemCode, "A", targetWarehouseType, targetWarehouseId);
				if (itemLocPriorityInfos.Any()) //一般儲位有此商品儲位
				{
					//尋找已有此商品且同效期同入庫日的儲位(商品設為批號控管就不找)，如果沒有才找最接近已放此商品儲位的空儲位
					GetSuggestLocByNearest(gupCode, custCode, dcCode, f1905, validDate, enterDate, itemLocPriorityInfos, _excludeItemLocs, canMixItem, isBundleSerialLoc, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc, makeNoRequ, targetWarehouseId);
				}
				else  //一般儲位沒有此商品儲位
				{
					//找其他一般儲位的空儲位
					GetSuggestLocByEmpty(gupCode, custCode, dcCode, f1905, targetWarehouseType, "A", ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc, targetWarehouseId);
				}

				if (qty > 0 && canMixItem) //尚有商品沒有建議儲位，允許混品且非批號管控商品
				{
					//取得混品儲位
					GetSuggestLocByMixItem(gupCode, custCode, dcCode, itemCode, f1905, itemLocPriorityInfos, targetWarehouseType, "A", ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc, targetWarehouseId);
				}

				if (qty > 0 && canMixBatchNo && !makeNoRequ && itemLocPriorityInfos.Any()) //尚有商品沒有建議儲位，允許混效期(混批)且非批號控管
				{
					//取可混批且同入庫日的儲位
					GetSuggestLocByMixBatchNo(dcCode, f1905, enterDate, true, itemLocPriorityInfos, _excludeItemLocs, canMixItem, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc);
					if (qty > 0)
						//取可混批且不同入庫日的儲位
						GetSuggestLocByMixBatchNo(dcCode, f1905, enterDate, false, itemLocPriorityInfos, _excludeItemLocs, canMixItem, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc);
				}

				if (qty > 0) //尚有商品沒有建議儲位，找DC共用空儲位
					GetSuggestLocByDcEmpty(dcCode, f1905, targetWarehouseType, "A", ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc, targetWarehouseId);

                if (qty > 0) //尚有商品沒有建議儲位，給來源儲位
                {
                    //無指定上架倉庫、有上架倉別且找不到建議儲位(qty > 0)且上架倉別為'G'
                    if (string.IsNullOrWhiteSpace(targetWarehouseId) && !string.IsNullOrWhiteSpace(targetWarehouseType) && targetWarehouseType == "G")
                    {
                        GetAutomaticWarehouse(qty, suggestLocs, targetWarehouseId);
                    }
                    else
                    {
                        GetSameSourceLoc(dcCode, qty, ref suggestLocs);
                    }
                    
                }

               
                #endregion 取得一般儲區儲位
            }
            #endregion 非流通加工倉

            return suggestLocs;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="aTypeCode">儲區型態代號，A:一般揀貨區，B:黃金揀貨區</param>
		/// <param name="targetWarehouseType"></param>
		/// <param name="targetWarehouseId"></param>
		/// <returns></returns>
		private List<ItemLocPriorityInfo> GetGetItemPickLocPriorityInfo(string gupCode, string custCode, string dcCode, string itemCode, string aTypeCode, string targetWarehouseType, string targetWarehouseId = "")
		{
			List<ItemLocPriorityInfo> itemLocPriorityInfos = null;
			if (_itemPickLocPriorityInfos == null)
				_itemPickLocPriorityInfos = new List<ItemLocPriorityInfo>();

			if (_itemPickLocPriorityInfos.Any())
			{
				var query = _itemPickLocPriorityInfos.Where(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.ITEM_CODE == itemCode && _wareHouseTmpr.Split(',').Contains(a.TMPR_TYPE) );
				// 若有填倉別 Id，就只針對該倉別 Id 做篩選
				if (string.IsNullOrEmpty(targetWarehouseId))
					query = query.Where(a => a.WAREHOUSE_TYPE == targetWarehouseType);
				else
					query = query.Where(a => a.WAREHOUSE_ID == targetWarehouseId);
				itemLocPriorityInfos = query.ToList();
			}

			// 若從快取找不到優先儲位，則從揀貨儲位尋找
			if (itemLocPriorityInfos == null || !itemLocPriorityInfos.Any())
			{
				itemLocPriorityInfos = _f1913Repo.GetItemPickLocPriorityInfo(dcCode, gupCode, custCode, new List<string> { itemCode }, true, targetWarehouseType, targetWarehouseId, _wareHouseTmpr).ToList();
				_itemPickLocPriorityInfos.AddRange(itemLocPriorityInfos);
			}
			return itemLocPriorityInfos.Where(a => a.ATYPE_CODE == aTypeCode).ToList();
		}

		/// <summary>
		/// 不管混批、材積只要有同商品的儲位則全數放入
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="itemLocPriorityInfos">有此商品的儲位資料</param>
		/// <param name="qty"></param>
		/// <param name="suggestLocs"></param>
		private void GetSuggestLocBySameItem(string dcCode, IEnumerable<ItemLocPriorityInfo> itemLocPriorityInfos, ref long qty, ref List<SuggestLoc> suggestLocs)
		{
			//只要沒有放其他商品，數量0的也可以放
			var itemLocPriorityInfo = itemLocPriorityInfos.OrderByDescending(a => a.ATYPE_CODE).ThenBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
					.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE).First();

			//以此儲位為目的儲位 && 須排除凍結出&凍結進出
			var f1912 = _f1912Repo.Find(a => a.DC_CODE == dcCode && a.LOC_CODE == itemLocPriorityInfo.LOC_CODE);
			suggestLocs.Add(new SuggestLoc { F1912 = f1912, PutQty = qty });

			qty = 0;
		}

		/// <summary>
		/// 依儲區找空儲位
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="f1905"></param>
		/// <param name="targetWarehouseType"></param>
		/// <param name="aTypeCode">儲區型態代號，A:一般揀貨區，B:黃金揀貨區，C:補貨區</param>
		/// <param name="qty"></param>
		/// <param name="excludeLocs"></param>
		/// <param name="suggestLocs"></param>
		/// <param name="notAllowSeparateLoc">是否不允許商品拆儲位放</param>
		/// <param name="targetWarehouseId"></param>
		/// <param name="isVolume">是否計算材積</param>
		private void GetSuggestLocByEmpty(string gupCode, string custCode, string dcCode, F1905 f1905, string targetWarehouseType, string aTypeCode, ref long qty, ref List<F1912> excludeLocs, ref List<SuggestLoc> suggestLocs, bool notAllowSeparateLoc, string targetWarehouseId = "")
		{
            var excludeLocCodes = excludeLocs.Select(a => a.LOC_CODE).ToList();

            //1.先找貨主的空儲位
            var emptyLocPriorityInfos = _f1912Repo.GetEmptyLocPriorityInfo(dcCode, gupCode, custCode, targetWarehouseType, aTypeCode, targetWarehouseId, _wareHouseTmpr).ToList();
            if (excludeLocCodes.Any())
                emptyLocPriorityInfos = emptyLocPriorityInfos.Where(x => !excludeLocCodes.Contains(x.LOC_CODE)).ToList();

            //2.若無，找業主共用的空儲位
            if (!emptyLocPriorityInfos.Any())
            {
                emptyLocPriorityInfos = _f1912Repo.GetEmptyLocPriorityInfo(dcCode, gupCode, null, targetWarehouseType, aTypeCode, targetWarehouseId, _wareHouseTmpr).ToList();
                if (excludeLocCodes.Any())
                    emptyLocPriorityInfos = emptyLocPriorityInfos.Where(x => !excludeLocCodes.Contains(x.LOC_CODE)).ToList();
            }

            if (emptyLocPriorityInfos.Any())
			{
				var sortEmptyLocPriorityInfos = emptyLocPriorityInfos.OrderByDescending(a => a.ATYPE_CODE)
					.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE);
				foreach (var emptyLocPriorityInfo in sortEmptyLocPriorityInfos)
				{
					//所有上架建議儲位皆須排除凍結出&凍結進出
					var f1912 = _f1912Repo.Find(a => a.DC_CODE == dcCode && a.LOC_CODE == emptyLocPriorityInfo.LOC_CODE);

					SetSuggestLocs(f1912, f1905, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc);
					if (qty <= 0)
						break;
				}
			}
        }

		private void GetSuggestLocByDcEmpty(string dcCode, F1905 f1905, string targetWarehouseType, string aTypeCode, ref long qty, ref List<F1912> excludeLocs, ref List<SuggestLoc> suggestLocs, bool notAllowSeparateLoc, string targetWarehouseId = "")
		{
			var excludeLocCodes = excludeLocs.Select(a => a.LOC_CODE).ToList();

            //1.找DC共用的空儲位
            var emptyLocPriorityInfos = _f1912Repo.GetEmptyLocPriorityInfo(dcCode, null, null, targetWarehouseType, aTypeCode, targetWarehouseId, _wareHouseTmpr).ToList();

            if (excludeLocCodes.Any())
                emptyLocPriorityInfos = emptyLocPriorityInfos.Where(x => !excludeLocCodes.Contains(x.LOC_CODE)).ToList();

			if (emptyLocPriorityInfos.Any())
			{
				var sortEmptyLocPriorityInfos = emptyLocPriorityInfos.OrderByDescending(a => a.ATYPE_CODE)
					.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE);
				foreach (var emptyLocPriorityInfo in sortEmptyLocPriorityInfos)
				{
					//所有上架建議儲位皆須排除凍結出&凍結進出
					var f1912 = _f1912Repo.Find(a => a.DC_CODE == dcCode && a.LOC_CODE == emptyLocPriorityInfo.LOC_CODE);

					SetSuggestLocs(f1912, f1905, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc);
					if (qty <= 0)
						break;
				}
			}
		}

		/// <summary>
		/// 取得混品儲位
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="f1905"></param>
		/// <param name="itemLocPriorityInfos">優先順序儲位資訊，不需依參考儲位找最近距離給Null</param>
		/// <param name="targetWarehouseType"></param>
		/// <param name="aTypeCode">儲區型態代號，A:一般揀貨區，B:黃金揀貨區，C:補貨區</param>
		/// <param name="qty"></param>
		/// <param name="excludeLocs"></param>
		/// <param name="suggestLocs"></param>
		/// <param name="notAllowSeparateLoc">是否不允許商品拆儲位放</param>
		/// <param name="targetWarehouseId"></param>
		/// <param name="isVolume">是否計算材積</param>
		private void GetSuggestLocByMixItem(string gupCode, string custCode, string dcCode, string itemCode, F1905 f1905, List<ItemLocPriorityInfo> itemLocPriorityInfos, string targetWarehouseType, string aTypeCode, ref long qty, ref List<F1912> excludeLocs, ref List<SuggestLoc> suggestLocs, bool notAllowSeparateLoc, string targetWarehouseId = "")
		{
			var excludeLocCodes = excludeLocs.Select(a => a.LOC_CODE).ToList();

			var locCode = string.Empty;
			decimal? volume = null;
			if (itemLocPriorityInfos != null && itemLocPriorityInfos.Any())
			{
				//找離第一優先順序儲位最近的空儲位為目的儲位
				var itemLocPriorityInfo = itemLocPriorityInfos.OrderByDescending(a => a.ATYPE_CODE).ThenBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
					.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE).First();
				locCode = itemLocPriorityInfo.LOC_CODE;
				volume = f1905.PACK_HIGHT * f1905.PACK_LENGTH * f1905.PACK_WIDTH;
			}
			List<F1912> f1912List;
			var item =
				_itemMixItemLocList.FirstOrDefault(
					o =>
						o.ATYPE_CODE == aTypeCode && o.WAREHOUSE_ID == targetWarehouseId && o.WAREHOUSE_TYPE == targetWarehouseType &&
						o.Volume == volume);
			if (item == null)
			{
				var mixItemLoc = new MixItemLoc
				{
					ATYPE_CODE = aTypeCode,
					WAREHOUSE_ID = targetWarehouseId,
					WAREHOUSE_TYPE = targetWarehouseType,
					Volume = volume
				};
				//取得可混品儲位
				f1912List =
					_f1912Repo.GetMixItemLoc(dcCode, gupCode, custCode, itemCode, targetWarehouseType, aTypeCode, true,
						targetWarehouseId, volume, _wareHouseTmpr).ToList();

				f1912List = f1912List.Where(x => !excludeLocCodes.Contains(x.LOC_CODE)).ToList();


				mixItemLoc.DataList = f1912List;
				_itemMixItemLocList.Add(mixItemLoc);
			}
			else
				f1912List = item.DataList;
			//排除已使用儲位
			f1912List = f1912List.Where(o => excludeLocCodes.All(c => c != o.LOC_CODE)).ToList();
			//取最近的儲位
			if (!string.IsNullOrEmpty(locCode))
				f1912List = (from o in f1912List
							 select new
							 {
								 F1912 = o,
								 Volume = o.USEFUL_VOLUMN ?? 0 - o.USED_VOLUMN ?? 0
								 ,
								 Nearest = Math.Abs(GetLocArea(locCode) - GetLocArea(o.LOC_CODE))
							 })
					.OrderByDescending(o => o.Volume).ThenBy(o => o.Nearest).ThenBy(o => o.F1912.HOR_DISTANCE).Select(o => o.F1912).ToList();
			else
				f1912List = (from o in f1912List
							 select new { F1912 = o, Volume = o.USEFUL_VOLUMN ?? 0 - o.USED_VOLUMN ?? 0 })
			.OrderByDescending(o => o.Volume).ThenBy(o => o.F1912.HOR_DISTANCE).Select(o => o.F1912).ToList();


			foreach (var f1912Tmp in f1912List)
			{
				var f1912 = f1912Tmp;
				if (f1912 != null)
				{
					SetSuggestLocs(f1912, f1905, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc);
					if (qty <= 0)
						break;
				}
			}
		}

		private List<ItemLocPriorityInfo> GetGetItemResupplyLocPriorityInfo(string gupCode, string custCode, string dcCode, string itemCode, string targetWarehouseType, string targetWarehouseId = "")
		{
			List<ItemLocPriorityInfo> itemLocPriorityInfos = null;
			if (_itemResupplyLocPriorityInfos == null)
				_itemResupplyLocPriorityInfos = new List<ItemLocPriorityInfo>();
			if (_itemResupplyLocPriorityInfos.Any())
			{
				var query = _itemResupplyLocPriorityInfos.Where(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.ITEM_CODE == itemCode && _wareHouseTmpr.Split(',').Contains(a.TMPR_TYPE) );
				// 若有填倉別 Id，就只針對該倉別 Id 做篩選
				if (string.IsNullOrEmpty(targetWarehouseId))
					query = query.Where(a => a.WAREHOUSE_TYPE == targetWarehouseType);
				else
					query = query.Where(a => a.WAREHOUSE_ID == targetWarehouseId);
				itemLocPriorityInfos = query.ToList();
			}

			// 若從快取找不到優先儲位，則從補貨儲位尋找
			if (itemLocPriorityInfos == null || !itemLocPriorityInfos.Any())
			{
				var f1913Rep = new F1913Repository(Schemas.CoreSchema);
				itemLocPriorityInfos = f1913Rep.GetItemResupplyLocPriorityInfo(dcCode, gupCode, custCode, new List<string> { itemCode }, true, targetWarehouseType, targetWarehouseId, _wareHouseTmpr).ToList();
				_itemResupplyLocPriorityInfos.AddRange(itemLocPriorityInfos);

			}
			return itemLocPriorityInfos;
		}

		/// <summary>
		/// 尋找已有此商品且同效期同入庫日的儲位，如果沒有才找最接近已放此商品儲位的空儲位
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="f1905"></param>
		/// <param name="validDate"></param>
		/// <param name="enterDate"></param>
		/// <param name="itemLocPriorityInfos"></param>
		/// <param name="excludeItemLocs"></param>
		/// <param name="canMixItem"></param>
		/// <param name="isBundleSerialLoc"></param>
		/// <param name="qty"></param>
		/// <param name="excludeLocs"></param>
		/// <param name="suggestLocs"></param>
		/// <param name="notAllowSeparateLoc"></param>
		/// <param name="targetWarehouseId"></param>
		private void GetSuggestLocByNearest(string gupCode, string custCode, string dcCode, F1905 f1905, DateTime validDate, DateTime enterDate, List<ItemLocPriorityInfo> itemLocPriorityInfos, List<string> excludeItemLocs, bool canMixItem, bool isBundleSerialLoc, ref long qty, ref List<F1912> excludeLocs, ref List<SuggestLoc> suggestLocs, bool notAllowSeparateLoc,bool makeNoRequ, string targetWarehouseId = "")
		{
			F1912 f1912 = null;

			var excludeLocCodes = excludeLocs.Select(a => a.LOC_CODE).ToList();
			if (!isBundleSerialLoc && !makeNoRequ) //不需考慮序號綁儲位商品 且非批號控管
			{
				var sameValidDateItemLocs = itemLocPriorityInfos.Where(a => a.VALID_DATE == validDate && a.ENTER_DATE == enterDate && !excludeLocCodes.Contains(a.LOC_CODE)).ToList();
				if (sameValidDateItemLocs.Any() && !canMixItem) //若不允許混商品，需再過濾掉同效期同入庫日有其他商品的儲位
				{
					//排除該商品所在儲位已有混其他商品的儲位
					if (excludeItemLocs.Any())
						sameValidDateItemLocs = sameValidDateItemLocs.Where(a => !excludeItemLocs.Contains(a.LOC_CODE)).ToList();
				}

				//若有儲位的入庫日及效期同此商品，則以此儲位為目的儲位 && 須排除凍結出&凍結進出
				foreach (var itemLocPriorityInfo in sameValidDateItemLocs)
				{
					if (qty <= 0)
						break;

					f1912 = _f1912Repo.Find(a => a.DC_CODE == dcCode && a.LOC_CODE == itemLocPriorityInfo.LOC_CODE);
					SetSuggestLocs(f1912, f1905, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc);
				}
			}

			if (qty > 0)
			{
				//1.先找貨主最近的空儲位
				GetNearestLoc(gupCode, custCode, dcCode, f1905, itemLocPriorityInfos, isBundleSerialLoc, EmptyLocSubjectType.Cust, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc, targetWarehouseId);
				if (qty > 0)
					//2.尚有商品，找業主共用最近的空儲位
					GetNearestLoc(gupCode, custCode, dcCode, f1905, itemLocPriorityInfos, isBundleSerialLoc, EmptyLocSubjectType.Group, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc, targetWarehouseId);
			}
		}


		private void GetNearestLoc(string gupCode, string custCode, string dcCode, F1905 f1905, IEnumerable<ItemLocPriorityInfo> itemLocPriorityInfos, bool isBundleSerialLoc, EmptyLocSubjectType subjectType, ref long qty, ref List<F1912> excludeLocs, ref List<SuggestLoc> suggestLocs, bool notAllowSeparateLoc, string targetWarehouseId = "")
		{
			var excludeLocCodes = excludeLocs.Select(a => a.LOC_CODE).ToList();
			//若無儲位的入庫日及效期同此商品，則找離第一優先順序儲位最近的空儲位為目的儲位
			var itemLocPriorityInfo = itemLocPriorityInfos.OrderByDescending(a => a.ATYPE_CODE).ThenBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
				.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE).First();
			var f1912List = new List<F1912>();
			if (subjectType == EmptyLocSubjectType.Cust)
			{
				//考量效能，先從暫存內，依第一優先儲位為基準，尋找同儲區類型、同倉別、同倉別類型的空儲位
				var item = _itemNearestLocListByCust.FirstOrDefault(o => o.ATYPE_CODE == itemLocPriorityInfo.ATYPE_CODE && o.WAREHOUSE_ID == targetWarehouseId && o.WAREHOUSE_TYPE == itemLocPriorityInfo.WAREHOUSE_TYPE);
				if (item == null)
				{
					var nearestLoc = new NearestLoc
					{
						ATYPE_CODE = itemLocPriorityInfo.ATYPE_CODE,
						WAREHOUSE_ID = targetWarehouseId,
						WAREHOUSE_TYPE = itemLocPriorityInfo.WAREHOUSE_TYPE
					};
					//如果查無空儲位，則依水平距離排序，尋找貨主可用同儲區類型、倉別、倉別類型的空儲位
					f1912List =
						_f1912Repo.GetNearestLoc(dcCode, gupCode, custCode, itemLocPriorityInfo.WAREHOUSE_TYPE,
							itemLocPriorityInfo.ATYPE_CODE, excludeLocCodes,_wareHouseTmpr, targetWarehouseId).ToList();
					f1912List = f1912List.Where(x => !excludeLocCodes.Contains(x.LOC_CODE)).ToList();
					nearestLoc.DataList = f1912List;
					_itemNearestLocListByCust.Add(nearestLoc);
				}
				else
					f1912List = item.DataList;
			}
			if (subjectType == EmptyLocSubjectType.Group)
			{
				//找業主共用最近的空儲位
				var item = _itemNearestLocListByGup.FirstOrDefault(o => o.ATYPE_CODE == itemLocPriorityInfo.ATYPE_CODE && o.WAREHOUSE_ID == targetWarehouseId && o.WAREHOUSE_TYPE == itemLocPriorityInfo.WAREHOUSE_TYPE);
				if (item == null)
				{
					var nearestLoc = new NearestLoc
					{
						ATYPE_CODE = itemLocPriorityInfo.ATYPE_CODE,
						WAREHOUSE_ID = targetWarehouseId,
						WAREHOUSE_TYPE = itemLocPriorityInfo.WAREHOUSE_TYPE
					};
					f1912List =
						_f1912Repo.GetNearestLoc(dcCode, gupCode, null, itemLocPriorityInfo.WAREHOUSE_TYPE,
							itemLocPriorityInfo.ATYPE_CODE, excludeLocCodes,_wareHouseTmpr, targetWarehouseId).ToList();
					f1912List = f1912List.Where(x => !excludeLocCodes.Contains(x.LOC_CODE)).ToList();
					nearestLoc.DataList = f1912List;
					_itemNearestLocListByGup.Add(nearestLoc);
				}
				else
					f1912List = item.DataList;

			}
			//排除已使用儲位
			f1912List = f1912List.Where(o => excludeLocCodes.All(c => c != o.LOC_CODE)).ToList();
			//取最近的儲位
			f1912List = (from o in f1912List
						 select new { F1912 = o, Nearest = Math.Abs(GetLocArea(itemLocPriorityInfo.LOC_CODE) - GetLocArea(o.LOC_CODE)) })
					.OrderBy(o => o.Nearest).ThenBy(o => o.F1912.HOR_DISTANCE).Select(o => o.F1912).ToList();

			if (f1912List.Any())
			{
				if (!isBundleSerialLoc)
				{
					foreach (var f1912 in f1912List)
					{
						if (qty <= 0)
							break;

						SetSuggestLocs(f1912, f1905, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc);
					}
				}
				else
				{
					// 序號綁儲位一個儲位只放一個商品
					foreach (var f1912 in f1912List)
					{
						if (qty <= 0)
							break;

						long putQty = 1;
						SetSuggestLocs(f1912, f1905, ref putQty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc);

						if (putQty == 0)
							qty--;
					}
				}
			}
		}

		//將儲位轉換成數字  , 所有以ASCII 顯示 
		public double GetLocArea(string locCode)
		{
			if (string.IsNullOrEmpty(locCode)) return 0;
			//字串ASCII值 * 10 (長度-1)-索引)*2次方組成, *2是因為ASCII轉碼出來都是兩碼避免加總計算錯誤
			return locCode.Select((c, i) => c * (double)Math.Pow(10,((locCode.Length - 1) - i) * 2)).Sum();
		}

		private void GetSuggestLocByMixBatchNo(string dcCode, F1905 f1905, DateTime enterDate, bool isSameEnterDate, List<ItemLocPriorityInfo> itemLocPriorityInfos, List<string> excludeItemLocs, bool canMixItem, ref long qty, ref List<F1912> excludeLocs, ref List<SuggestLoc> suggestLocs, bool notAllowSeparateLoc)
		{
			F1912 f1912 = null;
			var excludeLocCodes = excludeLocs.Select(a => a.LOC_CODE).ToList();
			//混批所以須找數量大於0的儲位，0等同於空儲位
			var itemLocPriorityInfos2 = itemLocPriorityInfos.Where(a => a.QTY > 0 && !excludeLocCodes.Contains(a.LOC_CODE));
			if (isSameEnterDate)
				itemLocPriorityInfos2 = itemLocPriorityInfos2.Where(a => a.ENTER_DATE == enterDate);
			else
				itemLocPriorityInfos2 = itemLocPriorityInfos2.Where(a => a.ENTER_DATE != enterDate);
			//排除該商品所在儲位已有混其他商品的儲位
			if (!canMixItem && excludeItemLocs.Any())
				itemLocPriorityInfos2 = itemLocPriorityInfos2.Where(a => !excludeItemLocs.Contains(a.LOC_CODE)).ToList();
			//找離第一優先順序儲位最近的空儲位為目的儲位
			var itemLocPriorityInfo = itemLocPriorityInfos2.OrderByDescending(a => ((a.USEFUL_VOLUMN ?? 0) - (a.USED_VOLUMN ?? 0))).ThenByDescending(a => a.ATYPE_CODE).ThenBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
					.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE).FirstOrDefault();

			//所有上架建議儲位皆須排除凍結出&凍結進出
			if (itemLocPriorityInfo != null)
				f1912 = _f1912Repo.Find(a => a.DC_CODE == dcCode && a.LOC_CODE == itemLocPriorityInfo.LOC_CODE);

			if (f1912 != null)
			{
				SetSuggestLocs(f1912, f1905, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc);
				if (qty > 0)
					GetSuggestLocByMixBatchNo(dcCode, f1905, enterDate, isSameEnterDate, itemLocPriorityInfos, excludeItemLocs, canMixItem, ref qty, ref excludeLocs, ref suggestLocs, notAllowSeparateLoc);
			}
		}

		private void SetSuggestLocs(F1912 f1912, F1905 f1905, ref long qty, ref List<F1912> excludeLocs, ref List<SuggestLoc> suggestLocs, bool notAllowSeparateLoc)
		{
			var isWarehouseNeedCalVolumn = IsWarehouseNeedCalVolumn(f1912.WAREHOUSE_ID);
			var isAreaNeedCalVolumn = IsAreaNeedCalVolumn(f1912.AREA_CODE);
			// 當儲區非黃金揀貨區且倉別型態設定需計算容積，才真的需考慮容積計算
			var isCalcVolume = isAreaNeedCalVolumn && isWarehouseNeedCalVolumn;

			long putQty;
			if (isCalcVolume) //需考慮材積
			{
				var volume = f1905.PACK_HIGHT * f1905.PACK_LENGTH * f1905.PACK_WIDTH;
				var canUseVolumn = f1912.USEFUL_VOLUMN??0 - f1912.USED_VOLUMN??0;
				if (canUseVolumn <= 0 || volume <= 0) // 可用容積小於等於0 就跳過此儲位
					putQty = 0;
				else
					putQty = (long)(canUseVolumn / volume);

				if (putQty > qty)
				{
					putQty = qty;
					qty = 0;
				}
				else if (!notAllowSeparateLoc && putQty > 0) //要判斷是否不允許商品拆開儲位放
					qty -= putQty;
			}
			else //不考慮材積
			{
				putQty = qty;
				qty = 0;
			}

			if (putQty > 0)
				suggestLocs.Add(new SuggestLoc { F1912 = f1912, PutQty = putQty });

			if (!excludeLocs.Contains(f1912))
				excludeLocs.Add(f1912);
		}

		/// <summary>
		/// 若不等於黃金揀貨區(B)則就要計算材積
		/// </summary>
		/// <param name="areaCode"></param>
		/// <returns></returns>
		private bool IsAreaNeedCalVolumn(string areaCode)
		{
			return !areaCode.StartsWith("B");
		}
		private void GetSameSourceLoc(string dcCode, long qty, ref List<SuggestLoc> suggestLocs)
		{
			var f1912 = new F1912 { DC_CODE = dcCode, LOC_CODE = "000000000" };
			suggestLocs.Add(new SuggestLoc { F1912 = f1912, PutQty = qty });
		}
		#endregion

		/// <summary>
		///  取 依倉位型態 & 依倉別型態 第一個儲位
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="qty"></param>
		/// <param name="targetWarehouseType">F198001 倉別型態</param>
		/// <param name="aTypeCode">F191901 儲位型態主檔</param>
		/// <param name="targetWarehouseId"></param>
		/// <returns></returns>
		public SuggestLoc GetSuggestLocByFirst(string gupCode, string custCode, string dcCode, int qty, string targetWarehouseType, string aTypeCode, string targetWarehouseId = "")
		{
			F1912 f1912;
			var f1912Rep = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);

			//1.先找貨主的儲位
			f1912 = f1912Rep.GetFirstLoc(dcCode, gupCode, custCode, targetWarehouseType, aTypeCode, targetWarehouseId);
			//2.若無，找業主共用的儲位
			if (f1912 == null)
			{
				f1912 = f1912Rep.GetFirstLoc(dcCode, gupCode, custCode, targetWarehouseType, aTypeCode, targetWarehouseId,true,false,true);
			}
			//3.若無，找DC共用的儲位
			if (f1912 == null)
			{
				f1912 = f1912Rep.GetFirstLoc(dcCode, gupCode, custCode, targetWarehouseType, aTypeCode, targetWarehouseId,true,true,true);
			}

			var suggestLocs = new SuggestLoc { F1912 = f1912, PutQty = qty };
			return suggestLocs;
		}

		/// <summary>
		/// 判斷該倉別型態是否需要計算材積
		/// </summary>
		/// <param name="warehouseType"></param>
		/// <param name="warehouseId"></param>
		/// <returns></returns>
		public bool IsWarehouseNeedCalVolumn(string warehouseId)
		{
			// 沒設定倉別則不算材積，理論上不會有這情況
			if (string.IsNullOrEmpty(warehouseId))
				return false;

			var warehouseType = warehouseId.Substring(0, 1);

			if (_warehouseIsCalcVolumnDict == null)
				_warehouseIsCalcVolumnDict = new Dictionary<string, bool>();

			// 尚未從 DB 取得 F198001 的倉別型態是否要計算材積
			if (!_warehouseIsCalcVolumnDict.ContainsKey(warehouseType))
			{
				var f198001Repo = new F198001Repository(Schemas.CoreSchema, _wmsTransaction);
				var calVolumn = f198001Repo.Filter(x => x.TYPE_ID == EntityFunctions.AsNonUnicode(warehouseType))
										   .Select(x => x.CALVOLUMN)
										   .FirstOrDefault();

				_warehouseIsCalcVolumnDict.Add(warehouseType, calVolumn == "1");
			}

			return _warehouseIsCalcVolumnDict[warehouseType];
		}

        /// <summary>
        /// 取得自動倉儲位
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="suggestLocs"></param>
        /// <param name="warehouseId"></param>
        public void GetAutomaticWarehouse( long qty, List<SuggestLoc> suggestLocs, string warehouseId)
        {
            var f1980Repo = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
						//取得自動倉
						var autoWarehouseList = f1980Repo.GetAutoWarehouseList(_dcCode);

						if (!string.IsNullOrEmpty(warehouseId))
				      autoWarehouseList = autoWarehouseList.Where(x=> x.WAREHOUSE_ID == warehouseId);

						if (autoWarehouseList.Any())
            {
				      foreach(var f1980 in autoWarehouseList)
							{
									//取得自動倉的第一筆儲位並檢查儲位凍結狀態((如果儲位凍結狀態為凍結進、凍結進出時就回傳來源儲位))
									var f1912 = f1912Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f1980.DC_CODE &&
																																	 x.WAREHOUSE_ID == f1980.WAREHOUSE_ID).FirstOrDefault();

									if (f1912 == null ||  f1912.NOW_STATUS_ID == "02" || f1912.NOW_STATUS_ID == "04")
									{
										continue;
									}
									//查詢商品資料
									var f1903 = f1903Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == _gupCode &&
																																				x.CUST_CODE == _custCode &&
																																				x.ITEM_CODE == _itemCode).FirstOrDefault();
									//倉庫的溫層跟商品溫層不一致時，也是回傳來源儲位
									if (f1903 != null && !GetWareHouseTmprByItemTmpr(f1903.TMPR_TYPE).Split(',').Contains(f1980.TMPR_TYPE))
									{
										continue;
									}

							  	suggestLocs.Add(new SuggestLoc { F1912 = f1912, PutQty = qty });
					        qty =0;
					        break;
							}
				      
							if(qty>0)
					      GetSameSourceLoc(_dcCode, qty, ref suggestLocs);
               
            }
        }
    }
}
