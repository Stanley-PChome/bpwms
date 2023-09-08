using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Shared.Services
{
	public partial class NewSuggestLocService
	{
		private Dictionary<string, List<string>> _dcExcludeLocCodeList;
		private Dictionary<ItemKey, bool> _itemIsSpecialItemList;
		private CommonProduct _currentF1903;
		private F1905 _currentF1905;
		private F1909 _currentF1909;
		private List<string> _currentItemWareHouseTmprList;
		private string _currentTarWarehouseType;
		private NewSuggestLocParam _req;
		private List<NewSuggestLoc> _suggestLocs;
		private List<NewItemLocPriorityInfo> _itemPickLocPriorityInfos;
		private List<NewItemLocPriorityInfo> _itemResupplyLocPriorityInfos;
		private F1913Repository _f1913Repo;
		private F1912Repository _f1912Repo;
		private Dictionary<ItemKey, List<string>> _excludeItemLocs;
		private Dictionary<string, bool> _warehouseIsCalcVolumnDict;
		private List<NewNearestLoc> _itemNearestLocListByCust;
		private List<NewNearestLoc> _itemNearestLocListByGup;
		private List<EmptyLoc> _emptyLocListByCust;
		private List<EmptyLoc> _emptyLocListByGup;
		private List<EmptyLoc> _emptyLocListByDc;
		private List<NewMixItemLoc> _mixItemLocList;
		private List<string> _bookingLocCodes;
		//SQL排除儲位最大筆數
		private int _limit = 500;

		public CommonService CommonService
		{
			get;set;
		}
		public SharedService SharedService
		{
			get;set;
		}

		public NewSuggestLocService()
		{
			_dcExcludeLocCodeList = new Dictionary<string, List<string>>();
			_f1913Repo = new F1913Repository(Schemas.CoreSchema);
			_f1912Repo = new F1912Repository(Schemas.CoreSchema);
		}

		/// <summary>
		/// 取得商品建議上架儲位
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public List<NewSuggestLoc> GetSuggestLocs(NewSuggestLocParam req,ref List<string> excludeLocs)
		{
      _suggestLocs = new List<NewSuggestLoc>();
			if (excludeLocs == null)
				excludeLocs = new List<string>();

			if (CommonService == null)
				CommonService = new CommonService();

			if (SharedService == null)
				SharedService = new SharedService();

			_req = req;

			#region 取得資料
      // 取得商品資料
      _currentF1903 = CommonService.GetProduct(_req.GupCode, _req.CustCode, _req.ItemCode);
			if (_currentF1903 == null)
			{
				SetSameSourceLoc(_req.Qty);
				return _suggestLocs;
			}

      // 取得貨主資料
      _currentF1909 = CommonService.GetCust(_req.GupCode, _req.CustCode);
			if (_currentF1909 == null)
			{
				SetSameSourceLoc(_req.Qty);
				return _suggestLocs;
			}

      if (!string.IsNullOrWhiteSpace(_req.TarWarehouseType))
				_currentTarWarehouseType = _req.TarWarehouseType.ToUpper();
			else if (!string.IsNullOrWhiteSpace(_req.TarWarehouseId))
				_currentTarWarehouseType = _req.TarWarehouseId.Substring(0, 1).ToUpper();
			else
			{
				SetSameSourceLoc(_req.Qty);
				return _suggestLocs;
			}

			// 取得商品儲位溫噌
			_currentItemWareHouseTmprList = SharedService.GetWareHouseTmprByItemTmpr(_currentF1903.TMPR_TYPE).Split(',').ToList();

			//需考慮序號綁儲位商品，若有其他倉別不需考慮時要再加入邏輯條件
			var isBundleSerialLoc = (_currentF1903.BUNDLE_SERIALLOC == "1" &&  _currentTarWarehouseType != "W");
			if (_currentF1909.MIX_SERIAL_NO == "1")
				isBundleSerialLoc = false;

			// 取商品狀態設定  (混批 / 混品 /批號控管)
			var canMixItem = _req.AppointNeverItemMix ? false : (_currentF1903.LOC_MIX_ITEM == "1");
			var canMixBatchNo = _req.AppointNeverItemMix ? false : (_currentF1903.MIX_BATCHNO == "1" && !isBundleSerialLoc); //需考慮序號綁儲位商品一定不可混批
			var makeNoRequ = _currentF1903.MAKENO_REQU == "1";


			#endregion

			

			// 如果未取得已預約的儲位且未指定倉庫編號或指定倉庫編號非自動倉，則取得已經預約的儲位加入排除儲位清單
			if (_bookingLocCodes== null && (string.IsNullOrWhiteSpace(_req.TarWarehouseId) || (!string.IsNullOrWhiteSpace(_req.TarWarehouseId) && !CommonService.IsAutoWarehouse(_req.DcCode, _req.TarWarehouseId))))
			{
					var f191204Repos = new F191204Repository(Schemas.CoreSchema);
					_bookingLocCodes = f191204Repos.GetLockLocData(_req.DcCode, _req.GupCode, _req.CustCode).Select(x => x.LOC_CODE).Distinct().ToList();
					excludeLocs.AddRange(_bookingLocCodes);
			}

			// 如果上架倉別為加工倉
			if (_currentTarWarehouseType == "W")
			{
				#region 流通加工倉處理

				var itemLocPriorityInfos = GetItemPickLocPriorityInfo( "A", canMixItem,ref excludeLocs).ToList();
				// 流通加工倉儲位有此商品儲位，不管混批、材積只要有同商品的儲位則全數放入(會考慮商品是否混品)
				if (itemLocPriorityInfos.Any())
					GetSuggestLocBySameItem(itemLocPriorityInfos);
				else
				{
					//找其他流通加工倉一般儲位的空儲位
					GetSuggestLocByEmpty("A",isBundleSerialLoc,ref excludeLocs);
					//還有剩餘數量，取得混品儲位(不管商品是否設定不允許混品)
					if (_req.Qty > 0 )
					{
						GetCanMixLoc("A", isBundleSerialLoc, ref excludeLocs, null);
					}
					// 還有剩餘數量，回傳預設儲位
					if (_req.Qty > 0)
					{
						SetSameSourceLoc(_req.Qty);
					}
				}

				#endregion

			}
			// 如果上架倉別為非加工倉
			else
			{
				#region 非流通加工倉處理

				#region 指定上架倉別為自動倉，取得自動倉第一個儲位為建議儲位
				// 如果有指定上架倉庫編號且該倉庫為自動倉，取得自動倉第一個儲位為建議儲位
				if (!string.IsNullOrWhiteSpace(_req.TarWarehouseId) && CommonService.IsAutoWarehouse(_req.DcCode, _req.TarWarehouseId))
				{
					SetAutoWarehouseFirstLoc(_req.DcCode, _req.TarWarehouseId);
					return _suggestLocs;
				}
				#endregion

			

        // 是否為特殊商品(促銷商品)，若是則須放在黃金揀貨區
        var isGoldenItem = GetHasSpecialItem();
				if (!isGoldenItem)
				{
					//檢查是否為越庫商品，若是則需放在黃金揀貨區
					isGoldenItem = _currentF1903.C_D_FLAG == "1";
				}

        #region 取得黃金揀貨區建議儲位
        if (isGoldenItem)
				{
					var itemLocPriorityInfos = GetItemPickLocPriorityInfo("B", canMixItem,ref excludeLocs).ToList();
					// 黃金揀貨區儲位有此商品儲位，不管混批、材積只要有同商品的儲位則全數放入(會考慮商品是否混品)
					if (itemLocPriorityInfos.Any())
						GetSuggestLocBySameItem(itemLocPriorityInfos);
					else
					{
						//找黃金揀貨區的空儲位
						GetSuggestLocByEmpty("B", isBundleSerialLoc, ref excludeLocs);
						//還有剩餘數量，取得混品儲位(不管商品是否設定不允許混品)
						if (_req.Qty > 0)
						{
							GetCanMixLoc("B", isBundleSerialLoc, ref excludeLocs, null);
						}
						// 還有剩餘數量，回傳預設儲位
						if (_req.Qty > 0)
						{
							SetSameSourceLoc(_req.Qty);
						}
					}
					return _suggestLocs;
				}
        #endregion

        // 取得商品材積
        _currentF1905 = CommonService.GetProductSize(_req.GupCode, _req.CustCode, _req.ItemCode);
				if (_currentF1905 == null)
				{
					SetSameSourceLoc(_req.Qty);
					return _suggestLocs;
				}

        #region 取得補貨區建議儲位
        if (_req.IsIncludeResupply)
				{
					var itemResupplyLocPriorityInfos = GetItemResupplyLocPriorityInfo(canMixItem, ref excludeLocs).ToList();
					// 補貨區儲位有此商品儲位，先找有此商品同效期同入庫日的儲位，如果沒有才找最接近已放此商品儲位的空儲位
					if (itemResupplyLocPriorityInfos.Any())
						GetSuggestLocByNearest(itemResupplyLocPriorityInfos, isBundleSerialLoc, canMixBatchNo,makeNoRequ,ref excludeLocs);

					// 還有剩餘數量，找補貨區的空儲位
					if (_req.Qty > 0)
						GetSuggestLocByEmpty("C", isBundleSerialLoc, ref excludeLocs);

				}
        #endregion

        #region 取得揀貨區建議儲位
        if (_req.Qty > 0)
				{
					var itemLocPriorityInfos = GetItemPickLocPriorityInfo("A", canMixItem, ref excludeLocs).ToList();
					// 揀貨區儲位有此商品儲位，先找有此商品同效期同入庫日的儲位，如果沒有才找最接近已放此商品儲位的空儲位
					if (itemLocPriorityInfos.Any())
						GetSuggestLocByNearest(itemLocPriorityInfos, isBundleSerialLoc, canMixBatchNo, makeNoRequ, ref excludeLocs);
					
					// 還有剩餘數量，找其他一般儲位的空儲位
					if (_req.Qty > 0)
						GetSuggestLocByEmpty("A", isBundleSerialLoc, ref excludeLocs);

					//還有剩餘數量，取得混品儲位(需考慮商品是否設定不允許混品)
					if (_req.Qty > 0 && canMixItem)
					{
						GetCanMixLoc("A", isBundleSerialLoc, ref excludeLocs, itemLocPriorityInfos);
					}

					//還有剩餘數量，取得混批儲位
					if (_req.Qty > 0 && canMixBatchNo && itemLocPriorityInfos.Any())
					{
						//取可混批且同入庫日的儲位
						GetSuggestLocByMixBatchNo(true, itemLocPriorityInfos, ref excludeLocs);
						//取可混批且不同入庫日的儲位
						if(_req.Qty >0)
							GetSuggestLocByMixBatchNo(false, itemLocPriorityInfos, ref excludeLocs);
					}

					//還有剩餘數量，取得DC共用空儲位
					if (_req.Qty > 0)
						GetEmptyLoc(EmptyLocSubjectType.Dc, "A", isBundleSerialLoc, ref excludeLocs);

				}
        #endregion

        #region 未指定倉庫編號，指定倉庫型態為良品倉(G)，取得自動倉揀貨區建議儲位

        // 還有剩餘數量，且未指定目的倉庫，且指定倉庫型態=G(良品倉)，再找找看自動倉是否有符合的儲位可以用
        if (_req.Qty > 0 && string.IsNullOrWhiteSpace(_req.TarWarehouseId) && !string.IsNullOrWhiteSpace(_currentTarWarehouseType) && _currentTarWarehouseType == "G")
					SetAutoWarehouseSuggestLoc(_req.DcCode);
				// 還有剩餘數量，找不到建議儲位，給預設值
				else if (_req.Qty > 0)
					SetSameSourceLoc(_req.Qty);

        #endregion

        #endregion
      }

      return _suggestLocs;
		}

	

		#region 自動倉建議儲位共用方法
		

		/// <summary>
		/// 設定自動倉第一個儲位
		/// </summary>
		/// <param name="qty"></param>
		/// <returns></returns>
		private void SetAutoWarehouseFirstLoc(string dcCode,string warehouseId)
		{
			var f1912 = CommonService.GetWarehouseFirstLoc(dcCode, warehouseId);
			if (f1912 == null)
				SetSameSourceLoc(_req.Qty);
			else
			{
				_suggestLocs.Add(new NewSuggestLoc { LocCode = f1912.LOC_CODE, WarehouseId = f1912.WAREHOUSE_ID, PutQty = _req.Qty });
				_req.Qty = 0;
			}
		}

		/// <summary>
		/// 取得自動倉建議儲位
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="qty"></param>
		private void SetAutoWarehouseSuggestLoc(string dcCode)
		{
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			var autoWarehouseList = f1980Repo.GetAutoWarehouseList(dcCode).ToList();
			if (!autoWarehouseList.Any())
			{
				SetSameSourceLoc(_req.Qty);
				return;
			}
			foreach (var warehouse in autoWarehouseList)
			{
				var f1912 = CommonService.GetWarehouseFirstLoc(dcCode, warehouse.WAREHOUSE_ID);
				// 找不到儲位 或是該儲位已凍結進(02)、凍結進出(04)就跳過找下一個自動倉
				if (f1912 == null || f1912.NOW_STATUS_ID == "02" ||f1912.NOW_STATUS_ID =="04")
					continue;
				// 如果商品溫層不符合該倉庫溫層 跳過找下一個自動倉
				else if(!_currentItemWareHouseTmprList.Contains(warehouse.TMPR_TYPE))
					continue;
				else
				{
					_suggestLocs.Add(new NewSuggestLoc { LocCode = f1912.LOC_CODE, WarehouseId = f1912.WAREHOUSE_ID, PutQty = _req.Qty });
					_req.Qty = 0;
					break;
				}
			}
			if(_req.Qty >0)
				SetSameSourceLoc(_req.Qty);
		}
		#endregion

		#region 建議儲位共用

		//將儲位轉換成數字  , 所有以ASCII 顯示 
		public double GetLocArea(string locCode)
		{
			if (string.IsNullOrEmpty(locCode)) return 0;
			//字串ASCII值 * 10 (長度-1)-索引)*2次方組成, *2是因為ASCII轉碼出來都是兩碼避免加總計算錯誤
			return locCode.Select((c, i) => c * (double)Math.Pow(10, ((locCode.Length - 1) - i) * 2)).Sum();
		}

		/// <summary>
		/// 找不到建議儲位回傳
		/// </summary>
		/// <param name="qty"></param>
		/// <returns></returns>
		private void SetSameSourceLoc(long qty)
		{
			_suggestLocs.Add(new NewSuggestLoc { LocCode = "000000000", WarehouseId = "", PutQty = qty });
			_req.Qty = 0;
		}

		/// <summary>
		/// 設定建議儲位
		/// </summary>
		/// <param name="locCode">建議儲位</param>
		/// <param name="warehouseId">倉別編號</param>
		/// <param name="areaCode">儲區編號</param>
		/// <param name="usefulVolumn">儲位可用容積</param>
		/// <param name="usedVolumn">儲位已用容積</param>
		/// <param name="excludeLocs">排除儲位清單</param>
		private void SetSuggestLocs(string locCode,string warehouseId,string areaCode, decimal? usefulVolumn,decimal? usedVolumn,long qty ,ref List<string> excludeLocs)
		{
			var isWarehouseNeedCalVolumn = IsWarehouseNeedCalVolumn(warehouseId);
			var isAreaNeedCalVolumn = IsAreaNeedCalVolumn(areaCode);
			// 當儲區非黃金揀貨區且倉別型態設定需計算容積，才真的需考慮容積計算
			var isCalcVolume = isAreaNeedCalVolumn && isWarehouseNeedCalVolumn;

			long putQty;
			if (isCalcVolume) //需考慮材積
			{
				var volume = _currentF1905.PACK_HIGHT * _currentF1905.PACK_LENGTH * _currentF1905.PACK_WIDTH;
				var canUseVolumn = usefulVolumn ?? 0 - usedVolumn ?? 0;
				if (canUseVolumn <= 0 || volume <= 0) // 可用容積小於等於0 就跳過此儲位
					putQty = 0;
				else
					putQty = (long)(canUseVolumn / volume);

				if (putQty > qty)
				{
					putQty = qty;
					qty = 0;
				}
				else if (!_req.NotAllowSeparateLoc && putQty > 0) //要判斷是否不允許商品拆開儲位放
					qty -= putQty;
			}
			else //不考慮材積
			{
				putQty = qty;
				qty = 0;
			}

			if (putQty > 0)
				_suggestLocs.Add(new NewSuggestLoc { LocCode = locCode, WarehouseId = warehouseId, PutQty = putQty });

			if (!excludeLocs.Contains(locCode))
				excludeLocs.Add(locCode);

			_req.Qty -= putQty;
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
				var f198001Repo = new F198001Repository(Schemas.CoreSchema);
				var calVolumn = f198001Repo.GetDatasByTrueAndCondition(x => x.TYPE_ID ==warehouseType)
											 .Select(x => x.CALVOLUMN)
											 .FirstOrDefault();

				_warehouseIsCalcVolumnDict.Add(warehouseType, calVolumn == "1");
			}

			return _warehouseIsCalcVolumnDict[warehouseType];
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

		/// <summary>
		/// 是否是特殊商品(促銷商品)
		/// </summary>
		/// <returns></returns>
		private bool GetHasSpecialItem()
		{
			if (_itemIsSpecialItemList == null)
				_itemIsSpecialItemList = new Dictionary<ItemKey, bool>();

			var item = _itemIsSpecialItemList.FirstOrDefault(x => x.Key.DcCode == _req.DcCode && x.Key.GupCode == _req.GupCode && x.Key.CustCode == _req.CustCode && x.Key.ItemCode == _req.ItemCode);
			if (!item.Equals(default(KeyValuePair<ItemKey, bool>)))
				return item.Value;

			var _f050003Repo = new F050003Repository(Schemas.CoreSchema);
			//判斷是否是特殊商品(促銷商品)，須放在黃金揀貨區
			var hasSpecialItem = _f050003Repo.GetSpecialItemCount(_req.ItemCode, _req.GupCode, _req.CustCode, _req.DcCode) > 0;
			_itemIsSpecialItemList.Add(new ItemKey { GupCode = _req.GupCode, CustCode = _req.CustCode, ItemCode = _req.ItemCode }, hasSpecialItem);
			return hasSpecialItem;
		}

	

		/// <summary>
		/// 取得有此商品揀區儲位(會考慮商品是否混品)
		/// </summary>
		/// <param name="aTypeCode">儲區型態代號，A:一般揀貨區，B:黃金揀貨區</param>
		/// <param name="canMixItem">商品是否允許混品</param>
		/// <returns></returns>
		private List<NewItemLocPriorityInfo> GetItemPickLocPriorityInfo(string aTypeCode,bool canMixItem,ref List<string> excludeLocs)
		{
			if (_itemPickLocPriorityInfos == null)
				_itemPickLocPriorityInfos = new List<NewItemLocPriorityInfo>();

			var datas = _itemPickLocPriorityInfos.Where(x => x.DC_CODE == _req.DcCode && x.GUP_CODE == _req.GupCode && x.CUST_CODE == _req.CustCode &&
			x.ITEM_CODE == _req.ItemCode && _currentItemWareHouseTmprList.Contains(x.TMPR_TYPE)).ToList();
			if(!datas.Any())
			{
				datas = _f1913Repo.GetNewItemPickLocPriorityInfo(_req.DcCode,_req.GupCode,_req.CustCode, new List<string> { _req.ItemCode }, true, _currentTarWarehouseType, _req.TarWarehouseId, string.Join(",",_currentItemWareHouseTmprList)).ToList();
				_itemPickLocPriorityInfos.AddRange(datas);
			}

			datas = datas.Where(x => x.ATYPE_CODE == aTypeCode && x.WAREHOUSE_TYPE == _currentTarWarehouseType).ToList();

			if (!string.IsNullOrWhiteSpace(_req.TarWarehouseId))
				datas = datas.Where(x => x.WAREHOUSE_ID == _req.TarWarehouseId).ToList();

			// 不允許混品，排除有此商品但已混其他商品的儲位
			if (!canMixItem)
				datas = datas.Where(x => GetItemMixOtherItemLocs().All(y=> y != x.LOC_CODE)).ToList();
			
			// 有指定排除儲位，要排除該儲位庫存
			if(excludeLocs.Any())
			{
				var locs = excludeLocs;
				datas = datas.Where(x => locs.All(y => y != x.LOC_CODE)).ToList();
			}

			return datas;
		}

		/// <summary>
		/// 取得有此商品補區儲位(會考慮商品是否混品)
		/// </summary>
		/// <param name="canMixItem">商品是否允許混品</param>
		/// <returns></returns>
		private List<NewItemLocPriorityInfo> GetItemResupplyLocPriorityInfo(bool canMixItem,ref List<string> excludeLocs)
		{
			if (_itemResupplyLocPriorityInfos == null)
				_itemResupplyLocPriorityInfos = new List<NewItemLocPriorityInfo>();

			var datas = _itemResupplyLocPriorityInfos.Where(x => x.DC_CODE == _req.DcCode && x.GUP_CODE == _req.GupCode && x.CUST_CODE == _req.CustCode &&
			x.ITEM_CODE == _req.ItemCode && _currentItemWareHouseTmprList.Contains(x.TMPR_TYPE)).ToList();
			if (!datas.Any())
			{
				datas = _f1913Repo.GetNewItemResupplyLocPriorityInfo(_req.DcCode, _req.GupCode, _req.CustCode, new List<string> { _req.ItemCode }, true, _currentTarWarehouseType, _req.TarWarehouseId, string.Join(",", _currentItemWareHouseTmprList)).ToList();
				_itemResupplyLocPriorityInfos.AddRange(datas);
			}

				datas = datas.Where(x => x.WAREHOUSE_TYPE == _currentTarWarehouseType).ToList();

			if (!string.IsNullOrWhiteSpace(_req.TarWarehouseId))
				datas = datas.Where(x => x.WAREHOUSE_ID == _req.TarWarehouseId).ToList();

			// 不允許混品，排除有此商品但已混其他商品的儲位
			if (!canMixItem)
				datas = datas.Where(x => GetItemMixOtherItemLocs().All(y => y != x.LOC_CODE)).ToList();

			// 有指定排除儲位，要排除該儲位庫存
			if (excludeLocs.Any())
			{
				var locs = excludeLocs;
				datas = datas.Where(x => locs.All(y => y != x.LOC_CODE)).ToList();
			}

			return datas;
		}

		/// <summary>
		/// 取出該商品所在儲位已有混其他商品的儲位
		/// </summary>
		/// <returns></returns>
		private List<string> GetItemMixOtherItemLocs()
		{
			if (_excludeItemLocs == null)
			{
				_excludeItemLocs = new Dictionary<ItemKey, List<string>>();
			}
			var item = _excludeItemLocs.Where(x => x.Key.DcCode == _req.DcCode && x.Key.GupCode == _req.GupCode && x.Key.CustCode == _req.CustCode && x.Key.ItemCode == _req.ItemCode).FirstOrDefault();
			if (!item.Equals(default(KeyValuePair<ItemKey, List<string>>)))
				return item.Value;

			var datas = _f1913Repo.GetItemMixOtherItemLocs(_req.DcCode, _req.GupCode, _req.CustCode, _req.ItemCode).ToList();
			_excludeItemLocs.Add(new ItemKey { DcCode = _req.DcCode, GupCode = _req.GupCode, CustCode = _req.CustCode, ItemCode = _req.ItemCode }, datas);
			return datas;
		}

		/// <summary>
		/// 不管混批、材積只要有同商品的儲位則全數放入
		/// </summary>
		/// <param name="itemLocPriorityInfos">有此商品的儲位資料</param>
		private void GetSuggestLocBySameItem(IEnumerable<NewItemLocPriorityInfo> itemLocPriorityInfos)
		{
			//只要沒有放其他商品，數量0的也可以放
			var itemLocPriorityInfo = itemLocPriorityInfos.OrderByDescending(a => a.ATYPE_CODE).ThenBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
					.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE).First();

			_suggestLocs.Add(new NewSuggestLoc { LocCode = itemLocPriorityInfo.LOC_CODE, WarehouseId = itemLocPriorityInfo.WAREHOUSE_ID, PutQty = _req.Qty });
			_req.Qty = 0;
		}

		/// <summary>
		/// 尋找已有此商品且同效期同入庫日的儲位，如果沒有才找最接近已放此商品儲位的空儲位
		/// </summary>
		/// <param name="itemLocPriorityInfos">有此商品庫存資料</param>
		/// <param name="isBundleSerialLoc">是否為序號綁儲位</param>
		/// <param name="canMixBatchNo">是否允許混批(效期)</param>
		/// <param name="makeNoRequ">是否批號控管</param>
		private void GetSuggestLocByNearest(List<NewItemLocPriorityInfo> itemLocPriorityInfos, bool isBundleSerialLoc,bool canMixBatchNo, bool makeNoRequ,ref List<string> excludeLocs)
		{
			// 非序號綁儲位商品，非批號管控商品
			if(!isBundleSerialLoc && !makeNoRequ)
			{
				// 商品不允許混批(效期+入庫日)
				if(!canMixBatchNo)
				{
					//篩選該商品庫存，儲位只能有該商品一個效期一個入庫日
					itemLocPriorityInfos = itemLocPriorityInfos.GroupBy(x => new { x.LOC_CODE })
						.Select(x => new { x.Key.LOC_CODE, Cnt = x.GroupBy(y=> new { y.VALID_DATE,y.ENTER_DATE }).Count(), data = x })
						.Where(x => x.Cnt == 1).SelectMany(x => x.data).ToList();
				}
				// 找同效期同入庫日商品庫存
				var sameValidDateItemLocs = itemLocPriorityInfos.Where(x => x.VALID_DATE == _req.ValidDate && x.ENTER_DATE == _req.EnterDate).ToList();

				// 設定建議儲位
				foreach(var itemLocPriorityInfo in sameValidDateItemLocs)
				{
					if (_req.Qty <= 0)
						break;

					SetSuggestLocs(itemLocPriorityInfo.LOC_CODE, itemLocPriorityInfo.WAREHOUSE_ID, itemLocPriorityInfo.AREA_CODE, itemLocPriorityInfo.USEFUL_VOLUMN, itemLocPriorityInfo.USED_VOLUMN, _req.Qty, ref excludeLocs);
				}
			}

			if(_req.Qty > 0)
			{
				//1.先找貨主最近的空儲位
				GetNearestLoc(itemLocPriorityInfos, EmptyLocSubjectType.Cust, isBundleSerialLoc, ref excludeLocs);
				if(_req.Qty > 0)
					//再找業主共用最近的空儲位
					GetNearestLoc(itemLocPriorityInfos, EmptyLocSubjectType.Group, isBundleSerialLoc, ref excludeLocs);
			}
		}

		/// <summary>
		/// 若無同入庫日及效期此商品儲位，則找離第一優先順序儲位最近的空儲位為目的儲位
		/// </summary>
		/// <param name="itemLocPriorityInfos"></param>
		/// <param name="subjectType"></param>
		/// <param name="isBundleSerialLoc"></param>
		/// <param name="excludeLocs"></param>
		private void GetNearestLoc(List<NewItemLocPriorityInfo> itemLocPriorityInfos, EmptyLocSubjectType subjectType,bool isBundleSerialLoc, ref List<string> excludeLocs)
		{
			// 有指定倉庫編號，就只找指定倉庫最近的儲位
			if(!string.IsNullOrWhiteSpace(_req.TarWarehouseId))
				itemLocPriorityInfos = itemLocPriorityInfos.Where(x => x.WAREHOUSE_ID == _req.TarWarehouseId).ToList();

			//若無儲位的入庫日及效期同此商品，則找離第一優先順序儲位最近的空儲位為目的儲位
			var itemLocPriorityInfo = itemLocPriorityInfos.OrderByDescending(a => a.ATYPE_CODE).ThenBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
				.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE).FirstOrDefault();

			if (itemLocPriorityInfo == null)
				return;

			if (subjectType == EmptyLocSubjectType.Group && _itemNearestLocListByGup == null)
				_itemNearestLocListByGup = new List<NewNearestLoc>();
			if (subjectType == EmptyLocSubjectType.Cust && _itemNearestLocListByCust == null)
				_itemNearestLocListByCust = new List<NewNearestLoc>();


			var list = subjectType == EmptyLocSubjectType.Cust ? _itemNearestLocListByCust : _itemNearestLocListByGup;
			// 若有指定倉庫編號，先從暫存篩選找同倉庫編號
			if (!string.IsNullOrWhiteSpace(_req.TarWarehouseId))
				list = list.Where(x => x.WAREHOUSE_ID == _req.TarWarehouseId).ToList();
			else //否則撈無指定倉庫編號的資料
				list = list.Where(x => string.IsNullOrEmpty(x.WAREHOUSE_ID)).ToList();


			//再以依第一優先儲位為基準，尋找同儲區類型、同倉別類型的空儲位
			var nearestLoc = list.FirstOrDefault(x => x.DcCode == _req.DcCode && x.ATYPE_CODE == itemLocPriorityInfo.ATYPE_CODE && x.WAREHOUSE_TYPE == itemLocPriorityInfo.WAREHOUSE_TYPE);

			var locPriorityInfoes = new List<NearestLocPriorityInfo>();
			// 限制可以進行SQL排除儲位筆數
			var locs = excludeLocs;

			if (nearestLoc == null)
			{
				nearestLoc = new NewNearestLoc
				{
					DcCode = _req.DcCode,
					ATYPE_CODE = itemLocPriorityInfo.ATYPE_CODE,
					WAREHOUSE_ID = _req.TarWarehouseId,
					WAREHOUSE_TYPE = itemLocPriorityInfo.WAREHOUSE_TYPE,
				};

				// 則依水平距離排序，尋找貨主可用同儲區類型、倉別、倉別類型的空儲位 
				// 若排除儲位數量小於限制可以進行SQL排除儲位筆數，就取需求量筆數(降低取過多儲位導致效能慢)
				if (excludeLocs.Count < _limit)
					locPriorityInfoes =
						_f1912Repo.GetNewNearestLoc(_req.DcCode, _req.GupCode, _req.CustCode, itemLocPriorityInfo.WAREHOUSE_TYPE,
							itemLocPriorityInfo.ATYPE_CODE, excludeLocs, string.Join(",", _currentItemWareHouseTmprList), _req.TarWarehouseId, _req.Qty).ToList();
				else
				{
					// 取得所有空儲位
					locPriorityInfoes =
						_f1912Repo.GetNewNearestLoc(_req.DcCode, _req.GupCode, _req.CustCode, itemLocPriorityInfo.WAREHOUSE_TYPE,
							itemLocPriorityInfo.ATYPE_CODE, null, string.Join(",", _currentItemWareHouseTmprList), _req.TarWarehouseId).ToList();
					// 在排除不可使用的儲位
					locPriorityInfoes = locPriorityInfoes.Where(x => locs.All(y => y != x.LOC_CODE)).ToList();
					nearestLoc.IsAll = true;
				}
				nearestLoc.DataList = locPriorityInfoes;
				list.Add(nearestLoc);
			}
			else
			{
				
				locPriorityInfoes = nearestLoc.DataList.Where(x => locs.All(y => y != x.LOC_CODE)).ToList();
				// 若非取得全部空儲位且暫存可用儲位小於需求量
				if (!nearestLoc.IsAll && locPriorityInfoes.Count < _req.Qty)
				{
					// 若排除儲位數量小於限制可以進行SQL排除儲位筆數，就取需求量筆數(降低取過多儲位導致效能慢)
					if (excludeLocs.Count < _limit)
						locPriorityInfoes =
							_f1912Repo.GetNewNearestLoc(_req.DcCode, _req.GupCode, _req.CustCode, itemLocPriorityInfo.WAREHOUSE_TYPE,
								itemLocPriorityInfo.ATYPE_CODE, excludeLocs, string.Join(",", _currentItemWareHouseTmprList), _req.TarWarehouseId,_req.Qty).ToList();
					else
					{
						// 取得所有空儲位
						locPriorityInfoes =
						_f1912Repo.GetNewNearestLoc(_req.DcCode, _req.GupCode, _req.CustCode, itemLocPriorityInfo.WAREHOUSE_TYPE,
							itemLocPriorityInfo.ATYPE_CODE, null, string.Join(",", _currentItemWareHouseTmprList), _req.TarWarehouseId).ToList();
						// 在排除不可使用的儲位
						locPriorityInfoes = locPriorityInfoes.Where(x => locs.All(y => y != x.LOC_CODE)).ToList();
						nearestLoc.IsAll = true;
					}
					nearestLoc.DataList = locPriorityInfoes;
				}
			}

			//取最近的儲位
			locPriorityInfoes = (from o in locPriorityInfoes
									 select new { F1912 = o, Nearest = Math.Abs(GetLocArea(itemLocPriorityInfo.LOC_CODE) - GetLocArea(o.LOC_CODE)) })
					.OrderBy(o => o.Nearest).ThenBy(o => o.F1912.HOR_DISTANCE).Select(o => o.F1912).ToList();

			if (locPriorityInfoes.Any())
			{
				foreach (var f1912 in locPriorityInfoes)
				{
					if (_req.Qty <= 0)
						break;
					// 如果是序號綁儲位商品，一個pcs一個儲位
					long putQty = (isBundleSerialLoc) ? 1 :_req.Qty;
					SetSuggestLocs(f1912.LOC_CODE, f1912.WAREHOUSE_ID, f1912.AREA_CODE, f1912.USEFUL_VOLUMN, f1912.USED_VOLUMN, putQty, ref excludeLocs);
				}
			}
		}

		/// <summary>
		/// 依儲區類型找空儲位
		/// </summary>
		/// <param name="aTypeCode"></param>
		/// <param name="isBundleSerialLoc"></param>
		/// <param name="excludeLocs"></param>
		private void GetSuggestLocByEmpty(string aTypeCode,bool isBundleSerialLoc, ref List<string> excludeLocs)
		{

			//1.先找貨主的空儲位
			GetEmptyLoc(EmptyLocSubjectType.Cust, aTypeCode, isBundleSerialLoc, ref excludeLocs);

			//2.若無，找業主共用的空儲位
			if(_req.Qty > 0)
				GetEmptyLoc(EmptyLocSubjectType.Group, aTypeCode, isBundleSerialLoc, ref excludeLocs);
		}

		/// <summary>
		/// 依共用類型取得儲區類型空儲位
		/// </summary>
		/// <param name="subjectType"></param>
		/// <param name="aTypeCode"></param>
		/// <param name="isBundleSerialLoc"></param>
		/// <param name="excludeLocs"></param>
		private void GetEmptyLoc(EmptyLocSubjectType subjectType,string aTypeCode, bool isBundleSerialLoc, ref List<string> excludeLocs)
		{
			if (subjectType == EmptyLocSubjectType.Cust && _emptyLocListByCust == null)
				_emptyLocListByCust = new List<EmptyLoc>();
			if (subjectType == EmptyLocSubjectType.Group && _emptyLocListByGup == null)
				_emptyLocListByGup = new List<EmptyLoc>();
			if (subjectType == EmptyLocSubjectType.Dc && _emptyLocListByDc == null)
				_emptyLocListByDc = new List<EmptyLoc>();
			var list = subjectType == EmptyLocSubjectType.Cust ? _emptyLocListByCust : subjectType == EmptyLocSubjectType.Group ? _emptyLocListByGup : _emptyLocListByDc;
			// 若有指定倉庫編號，先從暫存篩選找同倉庫編號
			if (!string.IsNullOrWhiteSpace(_req.TarWarehouseId))
				list = list.Where(x => x.WAREHOUSE_ID == _req.TarWarehouseId).ToList();
			else //否則撈無指定倉庫編號的資料
				list = list.Where(x => string.IsNullOrEmpty(x.WAREHOUSE_ID)).ToList();

			//再以依第一優先儲位為基準，尋找同儲區類型、同倉別類型的空儲位
			var emptyLoc = list.FirstOrDefault(x => x.DcCode == _req.DcCode && x.ATYPE_CODE == aTypeCode  && x.WAREHOUSE_TYPE == _currentTarWarehouseType);


			var emptyLocPriorityInfos = new List<LocPriorityInfo>();
			// 限制可以進行SQL排除儲位筆數
			var locs = excludeLocs;

			if (emptyLoc == null)
			{
				emptyLoc = new EmptyLoc
				{
					DcCode = _req.DcCode,
					ATYPE_CODE = aTypeCode,
					WAREHOUSE_TYPE = _currentTarWarehouseType,
					WAREHOUSE_ID = _req.TarWarehouseId,
				};
				// 取得空儲位 
				// 若排除儲位數量小於限制可以進行SQL排除儲位筆數，就取需求量筆數(降低取過多儲位導致效能慢)
				if (excludeLocs.Count < _limit)
					emptyLocPriorityInfos =
						_f1912Repo.GetEmptyLocPriorityInfo(_req.DcCode, subjectType != EmptyLocSubjectType.Dc ? _req.GupCode : null, subjectType == EmptyLocSubjectType.Cust ? _req.CustCode : null, _currentTarWarehouseType, aTypeCode,_req.TarWarehouseId, string.Join(",", _currentItemWareHouseTmprList), excludeLocs, _req.Qty).ToList();
				else
				{
					// 取得所有空儲位
					emptyLocPriorityInfos =
						_f1912Repo.GetEmptyLocPriorityInfo(_req.DcCode, subjectType != EmptyLocSubjectType.Dc ? _req.GupCode : null, subjectType == EmptyLocSubjectType.Cust ? _req.CustCode : null, _currentTarWarehouseType, aTypeCode, _req.TarWarehouseId, string.Join(",", _currentItemWareHouseTmprList)).ToList();
					// 在排除不可使用的儲位
					emptyLocPriorityInfos = emptyLocPriorityInfos.Where(x => locs.All(y => y != x.LOC_CODE)).ToList();
					emptyLoc.IsAll = true;
				}
				emptyLoc.DataList = emptyLocPriorityInfos;
				list.Add(emptyLoc);
			}
			else
			{
				emptyLocPriorityInfos = emptyLoc.DataList.Where(x => locs.All(y => y != x.LOC_CODE)).ToList();
				// 若非取得全部空儲位且暫存可用儲位小於需求量
				if (!emptyLoc.IsAll && emptyLocPriorityInfos.Count < _req.Qty)
				{
					// 取得空儲位  
					// 若排除儲位數量小於限制可以進行SQL排除儲位筆數，就取需求量筆數(降低取過多儲位導致效能慢)
					if (excludeLocs.Count < _limit)
					emptyLocPriorityInfos =
						_f1912Repo.GetEmptyLocPriorityInfo(_req.DcCode, subjectType != EmptyLocSubjectType.Dc ? _req.GupCode : null, subjectType == EmptyLocSubjectType.Cust ? _req.CustCode : null, _currentTarWarehouseType, aTypeCode, _req.TarWarehouseId, string.Join(",", _currentItemWareHouseTmprList), excludeLocs, _req.Qty).ToList();
					else
					{
						// 取得所有空儲位
						emptyLocPriorityInfos =
							_f1912Repo.GetEmptyLocPriorityInfo(_req.DcCode, subjectType != EmptyLocSubjectType.Dc ? _req.GupCode : null, subjectType == EmptyLocSubjectType.Cust ? _req.CustCode : null, _currentTarWarehouseType, aTypeCode, _req.TarWarehouseId, string.Join(",", _currentItemWareHouseTmprList)).ToList();

						// 在排除不可使用的儲位
						emptyLocPriorityInfos = emptyLocPriorityInfos.Where(x => locs.All(y => y != x.LOC_CODE)).ToList();
						emptyLoc.IsAll = true;
					}
					emptyLoc.DataList = emptyLocPriorityInfos;
				}
			}

			var sortEmptyLocPriorityInfos = emptyLocPriorityInfos.OrderByDescending(a => a.ATYPE_CODE)
					.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE);

			foreach (var locPriorityInfo in sortEmptyLocPriorityInfos)
			{
				if (_req.Qty <= 0)
					break;
				// 如果是序號綁儲位商品，一個pcs一個儲位
				long putQty = (isBundleSerialLoc) ? 1 : _req.Qty;
				SetSuggestLocs(locPriorityInfo.LOC_CODE, locPriorityInfo.WAREHOUSE_ID, locPriorityInfo.AREA_CODE, locPriorityInfo.USEFUL_VOLUMN, locPriorityInfo.USED_VOLUMN, putQty, ref excludeLocs);
			}
		}

		/// <summary>
		/// 依共用類型取得儲區類型空儲位
		/// </summary>
		/// <param name="subjectType"></param>
		/// <param name="aTypeCode"></param>
		/// <param name="isBundleSerialLoc"></param>
		/// <param name="excludeLocs"></param>
		private void GetCanMixLoc(string aTypeCode, bool isBundleSerialLoc, ref List<string> excludeLocs, List<NewItemLocPriorityInfo> itemLocPriorityInfos=null)
		{
      string nearestLoc = string.Empty;
			if(itemLocPriorityInfos!=null && itemLocPriorityInfos.Any())
			{
				var itemLocPriorityInfo = itemLocPriorityInfos.OrderByDescending(a => a.ATYPE_CODE).ThenBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
				.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE).First();
				nearestLoc = itemLocPriorityInfo.LOC_CODE;
			}

			if (_mixItemLocList == null)
				_mixItemLocList = new List<NewMixItemLoc>();

			var list = _mixItemLocList;
			// 若有指定倉庫編號，先從暫存篩選找同倉庫編號
			if (!string.IsNullOrWhiteSpace(_req.TarWarehouseId))
				list = list.Where(x => x.WAREHOUSE_ID == _req.TarWarehouseId).ToList();
			else //否則撈無指定倉庫編號的資料
				list = list.Where(x => string.IsNullOrEmpty(x.WAREHOUSE_ID)).ToList();

			// 取得商品容積
			var volumn = _currentF1905.PACK_HIGHT * _currentF1905.PACK_LENGTH * _currentF1905.PACK_WIDTH;

			//尋找同儲區類型、同倉別類型、同商品容積的可混品儲位
			var mixItemLoc = list.FirstOrDefault(x => x.DcCode == _req.DcCode && x.ATYPE_CODE == aTypeCode && x.WAREHOUSE_TYPE == _currentTarWarehouseType && x.Volume == volumn);

			var mixItemLocs = new List<MixLocPriorityInfo>();
			// 限制可以進行SQL排除儲位筆數
			var locs = excludeLocs;

			if (mixItemLoc == null)
			{
				mixItemLoc = new NewMixItemLoc
				{
					DcCode = _req.DcCode,
					ATYPE_CODE = aTypeCode,
					WAREHOUSE_TYPE = _currentTarWarehouseType,
					WAREHOUSE_ID = _req.TarWarehouseId,
					Volume = volumn
				};
        // 取得混品儲位 
        // 若排除儲位數量小於限制可以進行SQL排除儲位筆數，就取需求量筆數(降低取過多儲位導致效能慢)
        if (excludeLocs.Count < _limit)
        {
          mixItemLocs =
            _f1912Repo.GetNewMixItemLoc(_req.DcCode, _req.GupCode, _req.CustCode, _req.ItemCode, _currentTarWarehouseType, aTypeCode, true, _req.TarWarehouseId, volumn, string.Join(",", _currentItemWareHouseTmprList), locs, true, _req.Qty).ToList();
        }
        else
        {
          // 取得所有混品儲位 
          mixItemLocs =
            _f1912Repo.GetNewMixItemLoc(_req.DcCode, _req.GupCode, _req.CustCode, _req.ItemCode, _currentTarWarehouseType, aTypeCode, true, _req.TarWarehouseId, volumn, string.Join(",", _currentItemWareHouseTmprList)).ToList();
          // 在排除不可使用的儲位
          mixItemLocs = mixItemLocs.Where(x => locs.All(y => y != x.LOC_CODE)).ToList();
          mixItemLoc.IsAll = true;
        }
				mixItemLoc.DataList = mixItemLocs;
				list.Add(mixItemLoc);
			}
			else
			{
				mixItemLocs = mixItemLoc.DataList.Where(x => locs.All(y => y != x.LOC_CODE)).ToList();
				// 若非取得全部空儲位且暫存可用儲位小於需求量
				if (!mixItemLoc.IsAll && mixItemLocs.Count < _req.Qty)
				{
					// 取得混品儲位   
					// 若排除儲位數量小於限制可以進行SQL排除儲位筆數，就取需求量筆數(降低取過多儲位導致效能慢)
					if (excludeLocs.Count < _limit)
						mixItemLocs =
							_f1912Repo.GetNewMixItemLoc(_req.DcCode, _req.GupCode, _req.CustCode, _req.ItemCode, _currentTarWarehouseType, aTypeCode, true, _req.TarWarehouseId, volumn, string.Join(",", _currentItemWareHouseTmprList), locs, true, _req.Qty).ToList();
					else
					{
						// 取得所有混品儲位 
						mixItemLocs =
							_f1912Repo.GetNewMixItemLoc(_req.DcCode, _req.GupCode, _req.CustCode, _req.ItemCode, _currentTarWarehouseType, aTypeCode, true, _req.TarWarehouseId, volumn, string.Join(",", _currentItemWareHouseTmprList)).ToList();
						// 在排除不可使用的儲位
						mixItemLocs = mixItemLocs.Where(x => locs.All(y => y != x.LOC_CODE)).ToList();
						mixItemLoc.IsAll = true;
					}
					mixItemLoc.DataList = mixItemLocs;
				}
			}

			if(!string.IsNullOrEmpty(nearestLoc))
			{
				mixItemLocs = (from o in mixItemLocs
										 select new
										 {
											 Data = o,
											 Volume = o.USEFUL_VOLUMN ?? 0 - o.USED_VOLUMN ?? 0,
											 Nearest = Math.Abs(GetLocArea(nearestLoc) - GetLocArea(o.LOC_CODE))
										 })
					.OrderByDescending(o => o.Volume).ThenBy(o => o.Nearest).ThenBy(o => o.Data.HOR_DISTANCE).Select(o => o.Data).ToList();
			}
			else
				mixItemLocs = (from o in mixItemLocs
										 select new { Data = o, Volume = o.USEFUL_VOLUMN ?? 0 - o.USED_VOLUMN ?? 0 })
			.OrderByDescending(o => o.Volume).ThenBy(o => o.Data.HOR_DISTANCE).Select(o => o.Data).ToList();

			foreach (var locPriorityInfo in mixItemLocs)
			{
				if (_req.Qty <= 0)
					break;
				// 如果是序號綁儲位商品，一個pcs一個儲位
				long putQty = (isBundleSerialLoc) ? 1 : _req.Qty;
				SetSuggestLocs(locPriorityInfo.LOC_CODE, locPriorityInfo.WAREHOUSE_ID, locPriorityInfo.AREA_CODE, locPriorityInfo.USEFUL_VOLUMN, locPriorityInfo.USED_VOLUMN, putQty, ref excludeLocs);
			}
		}



		private void GetSuggestLocByMixBatchNo( bool isSameEnterDate,  List<NewItemLocPriorityInfo> itemLocPriorityInfos, ref List<string> excludeLocs)
		{
			var locs = excludeLocs;
			//混批所以須找數量大於0的儲位，0等同於空儲位
			var itemLocPriorityInfos2 = itemLocPriorityInfos.Where(a => a.QTY > 0 && !locs.All(b=> b != a.LOC_CODE));
			if (isSameEnterDate)
				itemLocPriorityInfos2 = itemLocPriorityInfos2.Where(a => a.ENTER_DATE == _req.EnterDate);
			else
				itemLocPriorityInfos2 = itemLocPriorityInfos2.Where(a => a.ENTER_DATE != _req.EnterDate);
			
			//找離第一優先順序儲位最近的空儲位為目的儲位
			itemLocPriorityInfos2 = itemLocPriorityInfos2.OrderByDescending(a => ((a.USEFUL_VOLUMN ?? 0) - (a.USED_VOLUMN ?? 0))).ThenByDescending(a => a.ATYPE_CODE).ThenBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
					.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE).ToList();

			foreach (var locPriorityInfo in itemLocPriorityInfos2)
			{
				if (_req.Qty <= 0)
					break;
				SetSuggestLocs(locPriorityInfo.LOC_CODE, locPriorityInfo.WAREHOUSE_ID, locPriorityInfo.AREA_CODE, locPriorityInfo.USEFUL_VOLUMN, locPriorityInfo.USED_VOLUMN, _req.Qty, ref excludeLocs);
			}
		}
		#endregion
	}
}
