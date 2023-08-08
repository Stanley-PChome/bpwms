
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Wms3pl.Common;
using Wms3pl.Common.Collections;
using Wms3pl.Common.Extensions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F51;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Helper;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Transaction.T05.ServiceEntites;

namespace Wms3pl.WebServices.Transaction.T05.Services
{
	/// <summary>
	/// 配庫
	/// </summary>
	public partial class T050101Service
	{
		private WmsTransaction _wmsTransaction;
		public T050101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
			if (_wmsTransaction != null)
				_wmsTransaction.UseBulkInsertFirst = true;
			Initialize();
		}

		private DateTime? _pickDateTimeMax = null; //目前最大的批次時段
		private int _pickIndex = 0; //批次序號
		private string _notifyOrdNo = null;
		private List<F1912> _excludeLocs = new List<F1912>();
		private F05030101Repository _f05030101Rep;
		private F050801Repository _f050801Rep;
		private SharedService _sharedService;
		private ConsignService _consignService;
		private F050802Repository _f050802Rep;
		private F050901Repository _f050901Rep;
		private F051201Repository _f051201Rep;
		private F051202Repository _f051202Rep;
		private F0513Repository _f0513Rep;
        private F1913Repository _f1913Rep;
        private F1511Repository _f1511Rep;
		private F1903Repository _f1903Rep;
		private F194704Repository _f194704Repo;
		private F05120101Repository _f05120101Repo;
		private F05030202Repository _f05030202Repo;
		private List<ExecuteResult> _exeResults = new List<ExecuteResult>();
		private List<F700102> _specialCars = new List<F700102>();
		private List<F1913> _returnStocks;
		private List<ReturnNewAllocation> _returnNewAllocations;
        private WmsLogHelper _wmsLogHelper;
		//訂單批次取得最大筆數
		private int _batchMaxCount = 1000;
    private CommonService _commonService;

		/// <summary>
		/// 預計配庫訂單數
		/// </summary>
		private int _preAllocOrderCnt = 0;
		/// <summary>
		/// 完成配庫訂單數
		/// </summary>
		private int _finishAllocOrderCnt = 0;

		private void Initialize()
		{
			_f05030101Rep = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			_f050801Rep = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			_sharedService = new SharedService(_wmsTransaction);
			_consignService = new ConsignService(_wmsTransaction);
			_f050802Rep = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
			_f050901Rep = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			//_f051201Rep = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			//_f051202Rep = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			_f0513Rep = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			_f1913Rep = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			//_f1511Rep = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			//_f1903Rep = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			_f194704Repo = new F194704Repository(Schemas.CoreSchema, _wmsTransaction);
			//_f05120101Repo = new F05120101Repository(Schemas.CoreSchema, _wmsTransaction);
			_f05030202Repo = new F05030202Repository(Schemas.CoreSchema, _wmsTransaction);
            _wmsLogHelper = new WmsLogHelper();

        }


		#region 配庫之Common
		private List<F1909> _f1909s = new List<F1909>();
		private List<F1929> _f1929s = new List<F1929>();
		private List<F1901> _f1901s = new List<F1901>();
		private List<F1980> _f1980s = new List<F1980>();
		private List<F1947> _f1947s = new List<F1947>();
		private List<F0003> _f0003s = new List<F0003>();
		private List<F000904> _f000904s = new List<F000904>();
		private List<F19000101> _f19000101s = new List<F19000101>();
		private List<F1905> _boxF1905s = new List<F1905>();
		private List<F1905> _f1905s = new List<F1905>();
		private List<F0020> _f0020s = new List<F0020>();
		private List<F194704> _f194704s = new List<F194704>();
		private List<F19470101> _f19470101s = new List<F19470101>();
		private List<DelvTimeArea> _delvTimeAreas = new List<DelvTimeArea>();
		private List<F194707WithF19470801> _f194707WithF19470801s = null;
		// 紀錄是否已經讀取過該物流中心、配送商、郵遞區號
		private HashSet<Keys<string, string, string>> _dcCodeWithAllIdWithZipCodeDict = null;
		private Dictionary<F050801, F050801ItemsInfo> _f050801ItemsInfoDict = null;
		private Dictionary<Keys<string, string, string>, F1903> _f1903sDict = new Dictionary<Keys<string, string, string>, F1903>();
		// 紀錄查詢過的行事曆是否為假日資訊
		private Dictionary<Keys<string, DateTime, string>, bool> _isHolidayDict = null;
		private List<F191902> _f191902s = new List<F191902>();

		private F191902 GetF191902(string dcCode, string gupCode, string custCode, string warehouseId, string areaCode)
		{
			var f191902 = _f191902s.Where(a => a.DC_CODE == dcCode && a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.WAREHOUSE_ID == warehouseId && a.AREA_CODE == areaCode).FirstOrDefault();
			if (f191902 == null)
			{
				var f191902Rep = new F191902Repository(Schemas.CoreSchema, _wmsTransaction);
				f191902 = f191902Rep.Find(a => a.DC_CODE == dcCode && a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.WAREHOUSE_ID == warehouseId && a.AREA_CODE == areaCode, false);
				if (f191902 != null)
					_f191902s.Add(f191902);
			}
			return f191902;
		}

		private F1909 GetF1909(string gupCode, string custCode)
		{
			var f1909 = _f1909s.Where(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode).SingleOrDefault();
			if (f1909 == null)
			{
				var f1909Rep = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
				f1909 = f1909Rep.Find(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode, false);
				if (f1909 != null)
					_f1909s.Add(f1909);
			}
			return f1909;
		}

		private F1929 GetF1929(string gupCode)
		{
			var f1929 = _f1929s.Where(a => a.GUP_CODE == gupCode).SingleOrDefault();
			if (f1929 == null)
			{
				var f1929Rep = new F1929Repository(Schemas.CoreSchema, _wmsTransaction);
				f1929 = f1929Rep.Find(a => a.GUP_CODE == gupCode, false);
				if (f1929 != null)
					_f1929s.Add(f1929);
			}
			return f1929;
		}

		private F1901 GetF1901(string dcCode)
		{
			var f1901 = _f1901s.Where(a => a.DC_CODE == dcCode).SingleOrDefault();
			if (f1901 == null)
			{
				var f1901Rep = new F1901Repository(Schemas.CoreSchema, _wmsTransaction);
				f1901 = f1901Rep.Find(a => a.DC_CODE == dcCode, false);
				if (f1901 != null)
					_f1901s.Add(f1901);
			}
			return f1901;
		}

		private F1980 GetF1980(string dcCode, string warehouseId)
		{
			var f1980 = _f1980s.Where(a => a.DC_CODE == dcCode && a.WAREHOUSE_ID == warehouseId).SingleOrDefault();
			if (f1980 == null)
			{
				var f1980Rep = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);
				f1980 = f1980Rep.Find(a => a.DC_CODE == dcCode && a.WAREHOUSE_ID == warehouseId, false);
				if (f1980 != null)
					_f1980s.Add(f1980);
			}
			return f1980;
		}

		private F1947 GetF1947(string dcCode, string allId)
		{
			var f1947 = _f1947s.Where(a => a.DC_CODE == dcCode && a.ALL_ID == allId).SingleOrDefault();
			if (f1947 == null)
			{
				var f1947Rep = new F1947Repository(Schemas.CoreSchema, _wmsTransaction);
				f1947 = f1947Rep.Find(a => a.DC_CODE == dcCode && a.ALL_ID == allId, false);
				if (f1947 != null)
					_f1947s.Add(f1947);
			}
			return f1947;
		}

		private F0003 GetF0003(string dcCode, string apName)
		{
			var f0003 = _f0003s.Find(a => a.DC_CODE == dcCode && a.AP_NAME == apName);
			if (f0003 == null)
			{
				var f0003Rep = new F0003Repository(Schemas.CoreSchema, _wmsTransaction);
				f0003 = f0003Rep.Find(a => a.DC_CODE == dcCode && a.AP_NAME == apName, false);
				if (f0003 != null)
					_f0003s.Add(f0003);
			}
			return f0003;
		}

		private F000904 GetF000904(string topic, string subTopic, string value)
		{
			var f000904 = _f000904s.Find(a => a.TOPIC == topic && a.SUBTOPIC == subTopic && a.VALUE == value);
			if (f000904 == null)
			{
				var f000904Rep = new F000904Repository(Schemas.CoreSchema, _wmsTransaction);
				f000904 = f000904Rep.GetDatas(topic, subTopic).FirstOrDefault(o => o.VALUE == value);
				if (f000904 != null)
					_f000904s.Add(f000904);
			}
			return f000904;
		}

		private F194704 GetF194704(string dcCode, string gupCode, string custCode, string allId)
		{
			var f194704 = _f194704s.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ALL_ID == allId);
			if (f194704 == null)
			{
				f194704 = _f194704Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ALL_ID == allId);
				if (f194704 != null)
					_f194704s.Add(f194704);
			}
			return f194704;
		}

		private List<F19000101> GetF19000101s(decimal ticketId)
		{
			var f19000101s = _f19000101s.Where(a => a.TICKET_ID == ticketId).ToList();
			if (!f19000101s.Any())
			{
				var f19000101Rep = new F19000101Repository(Schemas.CoreSchema, _wmsTransaction);
				f19000101s = f19000101Rep.GetMilestones(ticketId).ToList();
				if (f19000101s.Any())
					_f19000101s.AddRange(f19000101s);
			}
			return f19000101s;
		}

		private List<F1905> GetBoxF1905s(string gupCode, string custCode)
		{
			var f1905s = _boxF1905s.Where(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode).ToList();
			if (!f1905s.Any())
			{
				var repo = new F1905Repository(Schemas.CoreSchema, _wmsTransaction);
				f1905s = repo.GetCartonSize(gupCode, custCode, null).ToList();
				if (f1905s.Any())
					_boxF1905s.AddRange(f1905s);
			}
			return f1905s;
		}

		private List<F1905> GetF1905s(string gupCode,string custCode, IEnumerable<string> itemCodes)
		{
			// 找出還沒有快取的F1905
			var newItemCodes = itemCodes.Where(itemCode => _f1905s.All(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE != itemCode)).ToList(); ;
			if (newItemCodes.Any())
			{
				var f1905Rep = new F1905Repository(Schemas.CoreSchema, _wmsTransaction);
				var f1905s = f1905Rep.InWithTrueAndCondition("ITEM_CODE", newItemCodes, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode).ToList();
				_f1905s.AddRange(f1905s);
			}
			return _f1905s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && itemCodes.Contains(x.ITEM_CODE)).ToList();
		}


		private bool IsFindDelvSetting(string dcCode, string allId, string zipCode)
		{
			var isFind = _f19470101s.Any(x => x.DC_CODE == dcCode && x.ALL_ID == allId && x.ZIP_CODE == zipCode);
			if (!isFind)
			{
				var f19470101Repo = new F19470101Repository(Schemas.CoreSchema);
				isFind = f19470101Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.ALL_ID == allId && x.ZIP_CODE == zipCode).Any();
			}
			return isFind;
		}


		/// <summary>
		/// 取得可配送時段的區域資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="allID">配送商編號</param>
		/// <param name="canFast">是否可快速到貨</param>
		/// <param name="delvTmpr">溫層</param>
		/// <param name="zipCode">郵遞區號</param>
		/// <param name="takeDate">出車日期</param>
		/// <param name="minDelvTime">限制最早出車時段</param>
		/// <param name="maxDelvTime">限制最晚出車時段</param>
		/// <returns></returns>
		private List<DelvTimeArea> GetDelvTimeAreas(string dcCode, string allID, bool canFast, string delvTmpr, string zipCode, DateTime takeDate, string minDelvTime, string maxDelvTime = null)
		{
			var dayOfWeek = Convert.ToString((int)takeDate.DayOfWeek);

			// 建立依以下條件過濾可配送時段的查詢運算式
			var query = from x in _delvTimeAreas
									where x.DC_CODE == dcCode                                       // 物流中心
									where x.ALL_ID == allID                                         // 配送商
									where x.DELV_TMPR == delvTmpr                                   // 配送溫層(A:常溫、B：低溫)
									where string.Compare(x.DELV_TIME, minDelvTime) >= 0             // 最早出車時間
									where x.DELV_FREQ != null && x.DELV_FREQ.Contains(dayOfWeek)    // 指定的出車日期是否包含在星期幾內
									where x.ZIP_CODE == zipCode                                     // 郵遞區號
									select x;

			// 若有設定最晚出車時段，則也加入到過濾條件
			if (!string.IsNullOrEmpty(maxDelvTime))
				query = query.Where(a => string.Compare(a.DELV_TIME, maxDelvTime) <= 0);


			// 若可快速到貨，優先使用快速到貨的配送時段
			if (canFast && query.Any(x => x.DELV_EFFIC != "01"))
			{
				return query.Where(x => x.DELV_EFFIC != "01").ToList();
			}

			if (!query.Any())
			{
				// 取不到，由資料庫依配送商、溫層、最小時間、是否快速到貨及最大時間(若有)，過濾可配送時段
				var f19470101Rep = new F194701Repository(Schemas.CoreSchema, _wmsTransaction);
				var delvTimeAreas = f19470101Rep.GetDelvTimeAreas(dcCode, allID, canFast, delvTmpr, minDelvTime, maxDelvTime).ToList();

				// 更新快取的配送時段區域資料，後續直接使用查詢運算式取得即可
				if (delvTimeAreas.Any())
					_delvTimeAreas.AddRange(delvTimeAreas);
				_delvTimeAreas = _delvTimeAreas.Distinct().ToList();
			}

			return query.ToList();
		}

		/// <summary>
		/// 載入配送商計價設定與郵遞區號資訊
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="allIds"></param>
		/// <param name="zipCodes"></param>
		private void LoadF194707WithF19470801s(string dcCode, IEnumerable<string> allIds, IEnumerable<string> zipCodes)
		{
			if (_f194707WithF19470801s == null)
				_f194707WithF19470801s = new List<F194707WithF19470801>();

			if (_dcCodeWithAllIdWithZipCodeDict == null)
				_dcCodeWithAllIdWithZipCodeDict = new HashSet<Keys<string, string, string>>();

			var query = from allId in allIds
									from zipCode in zipCodes
									select new Keys<string, string, string>(dcCode, allId, zipCode);

			var list = query.Where(k => !_dcCodeWithAllIdWithZipCodeDict.Contains(k)).ToList();
			if (!list.Any())
				return;

			var f194707Repo = new F194707Repository(Schemas.CoreSchema, _wmsTransaction);
			// 取得正物流、且過濾物流中心、配送商、郵遞區號的配送商計價設定與郵遞區號資訊
			_f194707WithF19470801s.AddRange(f194707Repo.GetF194707WithF19470801s(dcCode, list.Select(x => x.Key2), list.Select(x => x.Key3)));
			_f194707WithF19470801s = _f194707WithF19470801s.Distinct().ToList();

			_dcCodeWithAllIdWithZipCodeDict.UnionWith(list);
		}

		/// <summary>
		/// 載入F050801品項相關的尺寸、材積、重量資訊
		/// </summary>
		/// <param name="f050801Dict"></param>
		private void LoadF050801ItemsInfoDict(Dictionary<F050801, List<F050802>> f050801Dict)
		{
			if (_f050801ItemsInfoDict == null)
				_f050801ItemsInfoDict = new Dictionary<F050801, F050801ItemsInfo>();

			foreach (var kvp in f050801Dict)
			{
				if (_f050801ItemsInfoDict.ContainsKey(kvp.Key))
					continue;

				var cubicCentimetre = GetF050802CubicCentimetre(kvp.Key, kvp.Value);
				var weight = GetF050802Weight(kvp.Key, kvp.Value);
				// 計算出貨單明細材積(size / 28317)
				var cuft = Math.Round((cubicCentimetre / 28317), 2);

				_f050801ItemsInfoDict.Add(kvp.Key, new F050801ItemsInfo(cubicCentimetre, weight, cuft));
			}
		}

		/// <summary>
		/// 計算出貨單明細尺寸(長x高x寬)
		/// </summary>
		/// <param name="f050801"></param>
		/// <param name="f050802s"></param>
		/// <returns></returns>
		private decimal GetF050802CubicCentimetre(F050801 f050801, List<F050802> f050802s)
		{
			//原箱出貨，以商品尺寸計算
			//if (f050801.ALLOWORDITEM == "1")
			//	return GetTotalItemsSize(f050801.DC_CODE, f050801.GUP_CODE, f050802s);

			decimal boxTotalSize;
			if (TryGetBoxTotalSize(f050801, f050802s, out boxTotalSize))
			{
				//非原箱出貨，以箱子的尺寸計算
				return boxTotalSize;
			}
			else
			{
				//若找不到箱子，以商品尺寸計算
				return GetTotalItemsSize(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050802s);
			}
		}

		/// <summary>
		/// 從商品總材積取得需要裝的箱子總材積
		/// </summary>
		/// <param name="f050801"></param>
		/// <param name="f050802s"></param>
		/// <param name="boxTotalSize"></param>
		/// <returns>若有箱子，才會回傳 true</returns>
		private bool TryGetBoxTotalSize(F050801 f050801, List<F050802> f050802s, out decimal boxTotalSize)
		{
			boxTotalSize = 0;

			// 取得符合的紙箱大小
			var f1905s = GetBoxF1905s(f050801.GUP_CODE, f050801.CUST_CODE);

			// 取得預設容積率
			var boxRate = GetBoxRate(f050801);

			// 先規劃出所需要的箱子材積與容積資訊
			var boxs = (from f1905 in f1905s
									let volume = Math.Abs(f1905.PACK_HIGHT * f1905.PACK_LENGTH * f1905.PACK_WIDTH)  // 絕對值防負值呆
									orderby volume descending   // 最大的箱子擺第一個，方便等等取得使用
									select new
									{
										// 箱子材積x容積率
										VolumeForRate = volume * boxRate,
										// 箱子材積，最後計算所有箱子材積用
										Volume = volume
									}).ToList();

			// 若沒有箱子或正確的箱子材積，則表示不能用箱子的材積
			if (boxs.All(x => x.VolumeForRate == 0))
				return false;

			// 商品總體積
			var itemsTotalVolume = GetTotalItemsVolume(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050802s);

			// 先由最大的箱子容積來分配大部分的商品材積
			var maxF1905 = boxs.First();
			var maxCount = (itemsTotalVolume / maxF1905.VolumeForRate);

			// 剩餘的商品材積，在挑選一個裝得下且最小的箱子容積
			var remainder = (itemsTotalVolume % maxF1905.VolumeForRate);
			var lastF1905 = boxs.Where(x => remainder > 0 && x.VolumeForRate >= remainder)
													.OrderBy(x => x.VolumeForRate)
													.First();

			// 好幾個大箱子 + 一個最適合的小箱子材積，就是所有用到的箱子總材積
			boxTotalSize = (maxF1905.Volume * maxCount) + lastF1905.Volume;
			return true;
		}

		/// <summary>
		/// 取得預設容積率
		/// </summary>
		/// <param name="f050801"></param>
		private decimal GetBoxRate(F050801 f050801)
		{
      if (_commonService == null)
      {
        _commonService = new CommonService();
      }

      var f0003Setting = _commonService.GetSysGlobalValue(f050801.DC_CODE, "BoxRate");
			var sysPath = f0003Setting == null ? "" : f0003Setting;

			decimal boxRate;
			if (!decimal.TryParse(sysPath, out boxRate))
				boxRate = 1;

			return boxRate;
		}

		/// <summary>
		/// 計算出貨單明細重量
		/// </summary>
		/// <param name="f050801"></param>
		/// <param name="f050802s"></param>
		/// <returns></returns>
		private decimal GetF050802Weight(F050801 f050801, List<F050802> f050802s)
		{
			return GetTotalItemsWeight(f050801.GUP_CODE, f050801.CUST_CODE, f050802s);
		}

		private F0020 GetF0020(string msgNo)
		{
			var f0020 = _f0020s.Where(a => a.MSG_NO == msgNo).SingleOrDefault();
			if (f0020 == null)
			{
				var f0020Rep = new F0020Repository(Schemas.CoreSchema, _wmsTransaction);
				f0020 = f0020Rep.Find(a => a.MSG_NO == msgNo, false);
				if (f0020 != null)
					_f0020s.Add(f0020);
			}
			return f0020;
		}

		private List<F1903> GetF1903s(string gupCode, string custCode, IEnumerable<string> itemCodes)
		{
			_f1903Rep = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var newItemCodes = itemCodes.Where(itemCode => !_f1903sDict.ContainsKey(new Keys<string, string, string>(gupCode, custCode, itemCode))).ToList();
			if (newItemCodes.Any())
			{
				//最大批次取資料筆數
				var maxBatchQty = 500;
				var batchCnt = newItemCodes.Count / maxBatchQty;
				if (newItemCodes.Count % maxBatchQty > 0)
					batchCnt++;

				for (var i = 0; i < batchCnt; i++)
				{
					var findItems = newItemCodes.Skip(i * maxBatchQty).Take(maxBatchQty).ToList();
					var f1903s = _f1903Rep.InWithTrueAndCondition("ITEM_CODE", findItems, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode);
					foreach (var f1903 in f1903s)
						_f1903sDict.Add(new Keys<string, string, string>(gupCode, custCode, f1903.ITEM_CODE), f1903);
				}
			}

			return itemCodes.Where(itemCode => _f1903sDict.ContainsKey(new Keys<string, string, string>(gupCode, custCode, itemCode)))
											.Select(itemCode => _f1903sDict[new Keys<string, string, string>(gupCode, custCode, itemCode)])
											.ToList();
		}
		/// <summary>
		/// 是否為假日，會自動從已讀取過的行事曆中做快取
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="takeDate"></param>
		/// <param name="scheduleType"></param>
		/// <returns></returns>
		private bool IsHoliday(string dcCode, DateTime takeDate, string scheduleType = "H")
		{
			if (_isHolidayDict == null)
				_isHolidayDict = new Dictionary<Keys<string, DateTime, string>, bool>();

			var key = new Keys<string, DateTime, string>(dcCode, takeDate, scheduleType);
			if (!_isHolidayDict.ContainsKey(key))
			{
				var f700501Rep = new F700501Repository(Schemas.CoreSchema, _wmsTransaction);
				var isHoliday = f700501Rep.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode) && x.SCHEDULE_DATE == takeDate && x.SCHEDULE_TYPE == EntityFunctions.AsNonUnicode(scheduleType)).Any();
				_isHolidayDict.Add(key, isHoliday);
			}
			return _isHolidayDict[key];
		}

		/// <summary>
		/// 將非加工使用中的組合商品拆成細項
		/// </summary>
		/// <param name="detail"></param>
		/// <returns></returns>
		private List<F05030201> MergeBomItemList(ref List<F050002> detail)
		{
			var f05030201List = new List<F05030201>();
			var f910102Repo = new F910102Repository(Schemas.CoreSchema);
			var group = detail.GroupBy(o => new { o.GUP_CODE, o.CUST_CODE });
			foreach (var item in group)
			{
				//取得BOM表組合商品明細
				var bomItemList = f910102Repo.GetBomItemDetailList(item.Key.GUP_CODE, item.Key.CUST_CODE, item.Select(o => o.ITEM_CODE).Distinct().ToList());
				//從訂單找出品號為組合商品品號
				var findDataInBoms = item.Where(o => bomItemList.Select(x => x.ITEM_CODE).Any(c => c == o.ITEM_CODE));
				//將組合商品拆成細項 (組合C 拆成 A,B)
				foreach (var findItem in findDataInBoms)
				{
					//找出此組合商品BOM表細項
					var bomItems = bomItemList.Where(o => o.ITEM_CODE == findItem.ITEM_CODE);
					//從BOM表細項產生或合併訂單明細及產生不加工的組合商品明細
					foreach (var bomItem in bomItems)
					{
						var newItem = AutoMapper.Mapper.DynamicMap<F05030201>(findItem);
						newItem.BOM_ITEM_CODE = findItem.ITEM_CODE;
						newItem.ITEM_CODE = bomItem.MATERIAL_CODE;
						newItem.BOM_QTY = bomItem.BOM_QTY;
						//細項商品訂貨數 = 細項商品所需數量 * 組合C訂貨數
						newItem.ORD_QTY = bomItem.BOM_QTY * findItem.ORD_QTY;
						//找出此訂單是否有此細項商品
						var ordItem = detail.FirstOrDefault(o => o.DC_CODE == findItem.DC_CODE && o.GUP_CODE == findItem.GUP_CODE && o.CUST_CODE == findItem.CUST_CODE && o.ORD_NO == findItem.ORD_NO && o.ITEM_CODE == bomItem.MATERIAL_CODE);
						//存在就合併該商品數量 
						if (ordItem != null)
						{
							ordItem.ORD_QTY += newItem.ORD_QTY;
							newItem.ORD_SEQ = ordItem.ORD_SEQ;
						}
						//不存在就建立一筆新的訂單明細
						else
						{
							ordItem = AutoMapper.Mapper.DynamicMap<F050002>(newItem);
							ordItem.ORD_SEQ = (detail.Where(o => o.DC_CODE == findItem.DC_CODE && o.GUP_CODE == findItem.GUP_CODE && o.CUST_CODE == findItem.CUST_CODE && o.ORD_NO == findItem.ORD_NO).Max(o => int.Parse(o.ORD_SEQ)) + 1).ToString();
							newItem.ORD_SEQ = ordItem.ORD_SEQ;
							detail.Add(ordItem);
						}
						//寫入貨主單據身檔(不加工的組合商品明細)
						f05030201List.Add(newItem);
					}
					detail.Remove(findItem);
				}
			}
			return f05030201List;
		}



		private List<ItemLimitValidDay> _itemLimitValidDays = new List<ItemLimitValidDay>();
		/// <summary>
		/// 依門市取得品項效期天數
		/// </summary>
		/// <param name="f050301"></param>
		/// <param name="itemCodes"></param>
		/// <returns></returns>
		private List<ItemLimitValidDay> GetItemLimitValidDays(string gupCode, string custCode, string reltailCode, List<string> itemCodes)
		{
			var itemLimitValidDays = _itemLimitValidDays.Where(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.RETAIL_CODE == reltailCode && itemCodes.Contains(a.ITEM_CODE)).ToList();
			if (itemLimitValidDays == null || !itemLimitValidDays.Any())
			{
				var f191001Repo = new F191001Repository(Schemas.CoreSchema);
				itemLimitValidDays = f191001Repo.GeItemLimitValidDays(gupCode, custCode, reltailCode, itemCodes).ToList();
				_itemLimitValidDays.AddRange(itemLimitValidDays);
			}

			return itemLimitValidDays;
		}

		#region 取得未配庫訂單資料
		/// <summary>
		/// 指定配庫訂單
		/// </summary>
		/// <param name="ordNos"></param>
		/// <param name="isTrialCalculation">是否配庫試算</param>
		/// <param name="f050001s"></param>
		/// <param name="f050002s"></param>
		private void GetDirectOrders(List<string> ordNos, bool isTrialCalculation, ref List<F050001> f050001s, ref List<F050002> f050002s)
		{
			_wmsLogHelper.AddRecord("取得訂單池訂單開始");
			if (!ordNos.Any())
				return;

			var f050001Repo = new F050001Repository(Schemas.CoreSchema);
			var f050002Repo = new F050002Repository(Schemas.CoreSchema);
			var batchOrders = new List<List<string>>();
			if (ordNos.Count > _batchMaxCount)
			{
				var pages = ordNos.Count / _batchMaxCount + (ordNos.Count % _batchMaxCount > 0 ? 1 : 0);
				for (var page = 0; page < pages; page++)
					batchOrders.Add(ordNos.Skip(page * _batchMaxCount).Take(_batchMaxCount).ToList());
			}
			else
				batchOrders.Add(ordNos);

			foreach (var batchOrder in batchOrders)
			{
				if(!isTrialCalculation)
					f050001Repo.SetStatus("1", batchOrder);

				f050001s.AddRange(f050001Repo.GetNotAllotedDatas(batchOrder).ToList());
				f050002s.AddRange(f050002Repo.GetNotAllotedDatas(batchOrder).ToList());
			}
			_wmsLogHelper.AddRecord("取得訂單池訂單結束");
		}
		/// <summary>
		/// 所有未配庫訂單
		/// </summary>
		/// <param name="limitDcList">限制取得未配庫訂單的物流中心清單</param>
		/// <param name="f050001s"></param>
		/// <param name="f050002s"></param>
		private void GetNonAllocOrders(ref List<F050001> f050001s, ref List<F050002> f050002s)
		{
			var f050001Repo = new F050001Repository(Schemas.CoreSchema);
			var f050002Repo = new F050002Repository(Schemas.CoreSchema);
			_wmsLogHelper.AddRecord("取得訂單池訂單開始");
			f050001Repo.SetStatus("1", "0");
			f050001s = f050001Repo.GetNotAllotedDatas().ToList();
			f050002s = f050002Repo.GetNotAllotedDatas().ToList();
			_wmsLogHelper.AddRecord("取得訂單池訂單結束");
		}

		#endregion

		#region 更新訂單池狀態為未配庫

		/// <summary>
		/// 將指定單號更新未配庫成功訂單狀態改為未配庫
		/// </summary>
		/// <param name="ordNos"></param>
		private void UpdateNoAllotDirectOrders(List<string> ordNos)
		{
			if (!ordNos.Any())
				return;
			var f050001Repo = new F050001Repository(Schemas.CoreSchema);
			var batchOrders = new List<List<string>>();
			if (ordNos.Count > _batchMaxCount)
			{
				var pages = ordNos.Count / _batchMaxCount + (ordNos.Count % _batchMaxCount > 0 ? 1 : 0);
				for (var page = 0; page < pages; page++)
					batchOrders.Add(ordNos.Skip(page * _batchMaxCount).Take(_batchMaxCount).ToList());
			}
			else
				batchOrders.Add(ordNos);

			foreach (var batchOrder in batchOrders)
			{
				f050001Repo.SetStatus("0", batchOrder);
			}
		}
		/// <summary>
		/// 更新未配庫成功訂單狀態改為未配庫
		/// </summary>
		/// <param name="limitDcList"></param>
		private void UpdateNoAllotOrders()
		{
			var f050001Repo = new F050001Repository(Schemas.CoreSchema);
			f050001Repo.SetStatus("0", "1");
		}

		#endregion

		#endregion 配庫之Common

	

		public IQueryable<ExecuteResult> AllotStocks()
		{
			//訂單池訂單配庫
			return AllotStocks(new List<string>(), true);
		}

		public IQueryable<ExecuteResult> AllotStocks(List<string> ordNos)
		{
			return AllotStocks(ordNos, false);
		}

		private IQueryable<ExecuteResult> AllotStocks(List<string> ordNos, bool isAutoPick)
		{
            var ordNosByCreateF060201 = new List<string>();// 用以新增出庫任務觸發的OrdNos
			_preAllocOrderCnt = 0;
			_finishAllocOrderCnt = 0;
			//判斷是否已有程序在配庫中
			var f0500Rep = new F0500Repository(Schemas.CoreSchema);
			var f050001Rep = new F050001Repository(Schemas.CoreSchema);
			F0500 f0500 = null;
			var canDo = true;
			var chkTimes = 0;

            _wmsLogHelper.StartRecord(WmsLogProcType.AllotStock);
            _wmsLogHelper.AddRecord("檢查配庫狀態");
            do
			{
				chkTimes++;
				f0500 = f0500Rep.Find(a => a.WORK_CODE == "0", isForUpdate: true, isByCache: false);
				if (f0500 != null && f0500.STATUS != "0") //已有程序在配庫中且為排程執行，每隔30秒再檢查一次，最多檢查6次
				{
					canDo = false;
					if (!isAutoPick)
						break;
					System.Threading.Thread.Sleep(30000);
				}
				else
					canDo = true;

			} while (!canDo && chkTimes <= 6);

			if (!canDo) //最後仍有程序在配庫中則跳出停止配庫
			{
				_exeResults.Add(new ExecuteResult { IsSuccessed = false, Message = "仍有程序在配庫中，請稍待再配庫" });
				return _exeResults.AsQueryable();
			}
			List<F050001> f050001s = new List<F050001>();
			List<F050002> f050002s = new List<F050002>();

			try
			{
                _wmsLogHelper.AddRecord("更新配庫狀態為配庫中");
                if (f0500 != null)
				{
					//設定狀態為配庫中
					f0500.STATUS = "1";
					f0500Rep.Update(f0500);
				}

				// 取得訂單池訂單
				if (isAutoPick)
					GetNonAllocOrders(ref f050001s, ref f050002s);
				else
					GetDirectOrders(ordNos,false, ref f050001s, ref f050002s);

				ordNosByCreateF060201 = f050001s.Select(x => x.ORD_NO).ToList();

				_preAllocOrderCnt = f050001s.Count;

								_wmsLogHelper.AddRecord("取得訂單池訂單結束");

                //檢查訂單是否有單據類別Id未設定
                _wmsLogHelper.AddRecord("檢查訂單單據類別是否有設定");
                if (f050001s.Any(x => x.TICKET_ID == 0))
				{
					var f0020 = GetF0020("AAM00019");
					AddMessagePoolForInside("9", f050001s.First().DC_CODE, f050001s.First().GUP_CODE, f050001s.First().CUST_CODE, f0020.MSG_NO, string.Format(f0020.MSG_CONTENT, string.Join("、", f050001s.Where(x => x.TICKET_ID == 0).Select(x => x.ORD_NO))), true);
					return _exeResults.AsQueryable();
				}

                //檢查此次配庫是否有非今日配達之訂單，如果有要顯示訊息
                _wmsLogHelper.AddRecord("檢查是否有非今日配達訂單");
                var f050004Rep = new F050004Repository(Schemas.CoreSchema, _wmsTransaction);
				var f050001GP = f050001s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.TICKET_ID });
				var OverTodayOrderList = new List<string>();
				foreach (var item in f050001GP)
				{
					var f1909 = GetF1909(item.Key.GUP_CODE, item.Key.CUST_CODE);
					//是否允許預先配庫(0:不允許 1:允許)
					if (f1909.ALLOW_ADVANCEDSTOCK == "0")
					{
						var f050004 = f050004Rep.GetData(item.Key.DC_CODE, item.Key.GUP_CODE, item.Key.CUST_CODE, item.Key.TICKET_ID);
						if (f050004 == null)
						{
							var ordNo = item.Select(o => o.ORD_NO);
							_exeResults.Add(new ExecuteResult { IsSuccessed = false, Message = string.Format("訂單編號:{0}{1}{0}出貨批次產生參數未設定", Environment.NewLine, string.Join("、" + Environment.NewLine, ordNo)) });
							return _exeResults.AsQueryable();
						}
						var overTodayOrderNos = item.Where(x => (x.ARRIVAL_DATE.HasValue && x.ARRIVAL_DATE.Value.AddDays(-1) > DateTime.Today) || (!x.ARRIVAL_DATE.HasValue && x.ORD_DATE.AddDays(f050004.DELV_DAY).AddDays(-1) > DateTime.Today)).Select(x => x.ORD_NO);
						OverTodayOrderList.AddRange(overTodayOrderNos);
					}
				}
				if (OverTodayOrderList.Any())
				{
					_exeResults.Add(new ExecuteResult { IsSuccessed = true, Message = string.Format("部分訂單配庫未完成，訂單指定到貨日為非今天或明天{0}訂單編號:{0}{1}", Environment.NewLine, string.Join("、" + Environment.NewLine, OverTodayOrderList)) });
				}
                _wmsLogHelper.AddRecord("將非加工使用中的組合商品拆成細項");
                var f05030201List = MergeBomItemList(ref f050002s);
				var f050006Rep = new F050006Repository(Schemas.CoreSchema, _wmsTransaction);
				var f050006s = f050006Rep.GetAllDatas().ToList();

                //設定訂單的郵遞區號及分DC
                _wmsLogHelper.AddRecord("開始地址解析");
                if (SetZipCodeDc(f050001s, f050006s))
				{
					var f050001HasDcs = f050001s.Where(a => !string.IsNullOrWhiteSpace(a.DC_CODE));

                    //挑單及檢查庫存
                    _wmsLogHelper.AddRecord("挑單及檢查庫存");
                    var f050001Gs = f050001HasDcs.GroupBy(a => new { a.GUP_CODE, a.CUST_CODE, a.DC_CODE });
					foreach (var f050001G in f050001Gs)
					{
						try
						{
							var custf050001s = f050001G.ToList();
							// 依據特殊商品或非特殊商品等條件來拆開批次時段
							var f050001sCustList = PickOrders(custf050001s, f050002s, f050001G.Key.GUP_CODE, f050001G.Key.CUST_CODE, f050001G.Key.DC_CODE);

							_pickIndex = 0;
							//檢查貨主庫存
							foreach (var f050001sCust in f050001sCustList)
							{
								_pickIndex++; //累加批次序號							
								CheckStocks(f050001sCust, f050002s, f05030201List);
								//if (!result)
								//	_wmsTransaction.SqlCommands.Clear();
								_wmsTransaction.Complete();
							}
						}
						catch (Exception ex)
						{
							//丟到訊息池
							var msgNo = "AAM00001"; //配庫發生不可預期的錯誤!
							var f0020 = GetF0020(msgNo);
							AddMessagePoolForInside("9", f050001G.Key.DC_CODE, f050001G.Key.GUP_CODE, f050001G.Key.CUST_CODE, msgNo, f0020.MSG_CONTENT, true);
							throw new Exception("配庫發生例外", ex);
						}
					}
				}
                _wmsLogHelper.AddRecord("結束地址解析");
            }
			finally
			{
				var f050001Repo = new F050001Repository(Schemas.CoreSchema);
				var f050002Repo = new F050002Repository(Schemas.CoreSchema);
				var f050101Repo = new F050101Repository(Schemas.CoreSchema);
				var f050301Repo = new F050301Repository(Schemas.CoreSchema);
				var f050302Repo = new F050302Repository(Schemas.CoreSchema);
				f050002Repo.DeleteHasAllot();
				f050001Repo.DeleteHasAllot();

                _wmsLogHelper.AddRecord("檢查是否取消缺貨訂單");
                var g = f050001s.GroupBy(x => new { x.GUP_CODE, x.CUST_CODE });
				foreach (var item in g)
				{
					var f1909 = GetF1909(item.Key.GUP_CODE, item.Key.CUST_CODE);
					if (f1909 != null && f1909.ALLOW_CANCEL_LACKORD == "1") //允許取消缺貨訂單
					{
						f050001Repo.DeleteLackOrder(item.Key.GUP_CODE, item.Key.CUST_CODE);
						f050002Repo.DeleteLackOrder(item.Key.GUP_CODE, item.Key.CUST_CODE);
						f050101Repo.UpdateLackToCancelOrder(item.Key.GUP_CODE, item.Key.CUST_CODE);
						f050301Repo.UpdateLackToCancelOrder(item.Key.GUP_CODE, item.Key.CUST_CODE);
					}
					else
					{
						f050302Repo.DeleteLackOrder(item.Key.GUP_CODE, item.Key.CUST_CODE);
						f050301Repo.DeleteLackOrder(item.Key.GUP_CODE, item.Key.CUST_CODE);
					}
				}

                _wmsLogHelper.AddRecord("修改訂單池資料未配庫");
				if (ordNos != null && ordNos.Any())
				{
                    //修改訂單池資料未配庫                    
                   UpdateNoAllotDirectOrders(ordNos);
				}
				else
				{
                    //修改訂單池資料為未配庫
                    UpdateNoAllotOrders();
				}

				_wmsLogHelper.AddRecord("將狀態改回待配庫");
                f0500 = f0500Rep.Find(a => a.WORK_CODE == "0", isForUpdate: true, isByCache: false);
				if (f0500 != null)
				{
                    //將狀態改回待配庫                    
                    f0500.STATUS = "0";
					f0500Rep.Update(f0500);
				}
			}
			if (!_exeResults.Any())
				_exeResults.Add(new ExecuteResult { IsSuccessed = true,Message = string.Format(Properties.Resources.AllocOrderMessage,_preAllocOrderCnt,_finishAllocOrderCnt) });

            _wmsLogHelper.StopRecord();

            #region 出庫任務觸發
            CreateF060201(ordNosByCreateF060201);
            #endregion

            return _exeResults.AsQueryable();
		}

        /// <summary>
        /// 出庫任務觸發
        /// </summary>
        /// <param name="ordNos"></param>
        private void CreateF060201(List<string> ordNos)
        {
            if (_f05030101Rep == null)
                _f05030101Rep = new F05030101Repository(Schemas.CoreSchema);
            if (_f051202Rep == null)
                _f051202Rep = new F051202Repository(Schemas.CoreSchema);
            var sharedService = new SharedService();

            // 找出倉別是否為自動倉的揀貨單號
            var pickNoDatas = _f05030101Rep.GetPickNos(ordNos).ToList();

            pickNoDatas.ForEach(pickNoData =>
            {
                var currWmsOrdDatas = _f051202Rep.GetWmsOrdNoCnt(pickNoData.PickOrdNo);

                //if (currWmsOrdDatas.Where(x => x.Cnt == 1).Any())
                //{
                //    // 每個WMS_ORD_NO在F050802只有一筆資料，走特殊結構訂單處理
                //}
                //else

                // 一般訂單處理
                sharedService.CreateF060201(pickNoData.DcCode, pickNoData.GupCode, pickNoData.CustCode, pickNoData.WmsOrdNo, pickNoData.PickOrdNo, pickNoData.WarehouseId);
            });

        }

        #region 讀取貨主訂單—自動篩選
        /// <summary>
        /// 先取得庫存不足被釋放的訂單先配庫
        /// </summary>
        private void AllotPreNotEnough(bool isAutoPick = true)
		{
			var f050301Rep = new F050301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050302Rep = new F050302Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050301s = f050301Rep.GetNonCancelDatasByStatus("0").ToList();
			if (f050301s.Any())
			{
				var gF050301s = f050301s.GroupBy(a => new { a.GUP_CODE, a.CUST_CODE, a.DC_CODE, a.TICKET_ID, a.PICK_TEMP_NO });

				//拆併單及配庫
				foreach (var gF050301 in gF050301s)
				{
					//因為每次配庫的品項不一定相同，配庫前先初始化_itemLimitValidDays
					_itemLimitValidDays = new List<ItemLimitValidDay>();

					_pickDateTimeMax = GetMaxPickTime(gF050301.Key.DC_CODE, gF050301.Key.GUP_CODE,
							gF050301.Key.CUST_CODE, DateTime.Today);

					var ordNos = gF050301.Select(a => a.ORD_NO).Distinct().ToList();
					var f050302s = f050302Rep.InWithTrueAndCondition("ORD_NO", ordNos, a => a.GUP_CODE == gF050301.Key.GUP_CODE && a.CUST_CODE == gF050301.Key.CUST_CODE && a.DC_CODE == gF050301.Key.DC_CODE).ToList();
					var f050301First = gF050301.ToList().First();
					var isB2b = (f050301First.ORD_TYPE == "0");
					var sourceType = f050301First.SOURCE_TYPE;
					TakeApartAndMergeOrder(gF050301.Key.GUP_CODE, gF050301.Key.CUST_CODE, gF050301.Key.DC_CODE, gF050301.ToList(), f050302s, isB2b, sourceType, true);
					_wmsTransaction.Complete();
				}
			}

			//if (!isAutoPick) //手挑單此部分產生的訊息不顯示
			//	_exeResults.Clear();
		}

		/// <summary>
		/// 取得業主貨主ZipCode的解析資料，這個只會包含一開始沒有 Zip，並且包含是否有例外錯誤
		/// </summary>
		/// <param name="f1909s"></param>
		/// <param name="f050001NonZipCodeDcs"></param>
		/// <returns></returns>
		private List<AddressParsedResult> GetParsedAddressData(IEnumerable<F050001> f050001NonZipCodeDcs)
		{
			// 取得貨主的 ORDER_ADDRESS 設定 
			f050001NonZipCodeDcs.ToList().ForEach(o => o.ADDRESS = (string.IsNullOrWhiteSpace(o.ADDRESS) ? o.ADDRESS : o.ADDRESS.Trim()));
			var f194704s = f050001NonZipCodeDcs.Where(x => !string.IsNullOrWhiteSpace(x.DC_CODE) && !string.IsNullOrWhiteSpace(x.ALL_ID)).GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ALL_ID }).Select(g => GetF194704(g.Key.DC_CODE, g.Key.GUP_CODE, g.Key.CUST_CODE, g.Key.ALL_ID)).Where(x => x != null).ToList();
			var f1909s = f050001NonZipCodeDcs.GroupBy(x => new { x.GUP_CODE, x.CUST_CODE }).Select(g => GetF1909(g.Key.GUP_CODE, g.Key.CUST_CODE)).Where(x => x != null).ToList();
			var parsedAddressQuery = from g in f050001NonZipCodeDcs.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ZIP_CODE, x.ADDRESS, x.CVS_TAKE, x.SELF_TAKE, x.ALL_ID })
															 where string.IsNullOrEmpty(g.Key.ZIP_CODE)
															 where g.Key.ADDRESS != null && g.Key.ADDRESS.Length >= 3
															 let zipCode = g.Key.ADDRESS.Substring(0, 3)
															 let parsedZipCode = zipCode.All(c => Char.IsDigit(c)) ? zipCode : string.Empty
															 let getConsignNo = !f194704s.Any(x => x.DC_CODE == g.Key.DC_CODE && x.GUP_CODE == g.Key.GUP_CODE && x.CUST_CODE == g.Key.CUST_CODE && x.ALL_ID == g.Key.ALL_ID) ? "1" : f194704s.First(x => x.DC_CODE == g.Key.DC_CODE && x.GUP_CODE == g.Key.GUP_CODE && x.CUST_CODE == g.Key.CUST_CODE && x.ALL_ID == g.Key.ALL_ID).GET_CONSIGN_NO
															 let orderAddress = !f1909s.Any(x => x.GUP_CODE == g.Key.GUP_CODE && x.CUST_CODE == g.Key.CUST_CODE) ? "1" : f1909s.First(x => x.GUP_CODE == g.Key.GUP_CODE && x.CUST_CODE == g.Key.CUST_CODE).ORDER_ADDRESS
															 select new AddressParsedResult
															 {
																 GUP_CODE = g.Key.GUP_CODE,
																 CUST_CODE = g.Key.CUST_CODE,
																 ZIP_CODE = parsedZipCode,
																 ADDRESS = g.Key.ADDRESS,
																 // 是否需要解析地址: 預設沒郵遞區號、有地址可以解析、宅配單且貨主設定要求解析地址 
																 IsNeedParseAddress = string.IsNullOrEmpty(parsedZipCode) && !string.IsNullOrWhiteSpace(g.Key.ADDRESS)
																													&& (orderAddress == "1" || (g.Key.SELF_TAKE == "0" && g.Key.CVS_TAKE == "0" && getConsignNo == "1"))
															 };
			// 先準備所有地址的資料
			var parsedAddressData = parsedAddressQuery.ToList();

			// 建立查詢式給迴圈重複判斷用，只做沒有郵遞區號，且需要解析地址的資料，且尚未出現例外錯誤過
			var query = parsedAddressData.Where(x => string.IsNullOrEmpty(x.ZIP_CODE) && x.IsNeedParseAddress && !x.HasParsedSucceed);
			if (!query.Any())
				return parsedAddressData;

			// 最多嘗試三次
			for (int i = 1; i <= 3; i++)
			{
				// 透過郵局或Google來解析地址，並設定到每個傳入的 AddressParsedResult 
				Parallel.ForEach(query, _sharedService.ParseAddress);

				if (!query.Any())
					break;

				// 如果說解析過，還沒全部解析完成，則先暫停0.5秒。若每次都有失敗，則最後一次不停
				if (i < 3)
					Thread.Sleep(500);
			}

			return parsedAddressData;
		}

		private bool SetZipCodeDc(List<F050001> f050001s, List<F050006> f050006s)
		{
			var results = new List<ExecuteResult>();

			var f050001Rep = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f190101Rep = new F190101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f190002Rep = new F190002Repository(Schemas.CoreSchema, _wmsTransaction);

			//取得沒有指定倉別的訂單的貨主單據倉別設定檔
			var nonTypeIdTicketIds = f050001s.Where(a => string.IsNullOrEmpty(a.TYPE_ID)).Select(a => a.TICKET_ID).Distinct().ToList();
			var f190002s = f190002Rep.GetDatas(nonTypeIdTicketIds).ToList();

			var f050001NonZipCodeDcs = f050001s.Where(a => string.IsNullOrEmpty(a.ZIP_CODE) || string.IsNullOrEmpty(a.DC_CODE) || string.IsNullOrEmpty(a.TYPE_ID));

			var parsedAddressData = GetParsedAddressData(f050001NonZipCodeDcs);

			// 若有沒成功解析郵遞區號，且是需要解析地址的，還包含例外錯誤的話，就寫入訊息池，並不做這次的配庫
			var addressParsedResult = parsedAddressData.FirstOrDefault(x => !x.IsSucceedParsedZipCode && x.IsNeedParseAddress && x.Exception != null);
			if (addressParsedResult != null)
			{
				var f050001 = f050001s.Where(x => x.ADDRESS == addressParsedResult.ADDRESS).FirstOrDefault();
				if (string.IsNullOrEmpty(f050001.DC_CODE))
				{
					var f1901Rep = new F1901Repository(Schemas.CoreSchema, _wmsTransaction);
					f050001.DC_CODE = f1901Rep.GetFirstData().DC_CODE;
				}
				var f1909 = GetF1909(f050001.GUP_CODE, f050001.CUST_CODE);

				var msgNo = "AAM00017"; //貨主:「{0}」解析地址服務中斷。地址:「{1}」 
				var f0020 = GetF0020(msgNo);
				var custName = (f1909 == null) ? "" : f1909.SHORT_NAME;
				var msg = string.Format(f0020.MSG_CONTENT, custName, addressParsedResult.ADDRESS);
				AddMessagePoolForInside("9", f050001.DC_CODE, f050001.GUP_CODE, f050001.CUST_CODE, msgNo, msg, false, true);

				results.Add(new ExecuteResult(false, msg));
				return false;
			}


			foreach (var f050001 in f050001NonZipCodeDcs)
			{
				//設定 ZipCode
				var isSetZipCode = false;
				if (string.IsNullOrEmpty(f050001.ZIP_CODE))
				{
					var parsedF050001 = parsedAddressData.Find(x => x.GUP_CODE == f050001.GUP_CODE && x.CUST_CODE == f050001.CUST_CODE && x.ADDRESS == f050001.ADDRESS);
					if (parsedF050001 != null && !string.IsNullOrEmpty(parsedF050001.ZIP_CODE))
					{
						f050001.ZIP_CODE = parsedF050001.ZIP_CODE;
						if (!string.IsNullOrEmpty(parsedF050001.ADDRESS_PARSE))
						{
							f050001.ADDRESS_PARSE = parsedF050001.ADDRESS_PARSE;
						}
						isSetZipCode = true;
					}
				}

				//依郵遞區號分配DC
				var isSetDc = false;
				if (string.IsNullOrEmpty(f050001.DC_CODE))
				{
					if (!AllotDc(f050001, f050006s)) //假如無法分配DC，則設定為服務此貨主的主DC
					{
						f050001.DC_CODE = f190101Rep.Filter(a => a.GUP_CODE == EntityFunctions.AsNonUnicode(f050001.GUP_CODE) && a.CUST_CODE == EntityFunctions.AsNonUnicode(f050001.CUST_CODE)).OrderBy(a => a.CRT_DATE).Select(a => a.DC_CODE).FirstOrDefault();
						if (string.IsNullOrEmpty(f050001.DC_CODE))
						{
							var f1901Rep = new F1901Repository(Schemas.CoreSchema, _wmsTransaction);
							var f1901 = f1901Rep.GetFirstData();
							var f1909 = GetF1909(f050001.GUP_CODE, f050001.CUST_CODE);
							//丟到訊息池
							var msgNo = "AAM00002"; //貨主:「{0}」未設定物流中心
							var f0020 = GetF0020(msgNo);
							AddMessagePoolForInside("9", f1901.DC_CODE, f050001.GUP_CODE, f050001.CUST_CODE, msgNo, string.Format(f0020.MSG_CONTENT, (f1909 == null) ? "" : f1909.SHORT_NAME), false, true);
						}
					}
					else
						isSetDc = true;
				}

				//檢查如果沒有指定倉別，須由貨主單據倉別設定檔取得，若沒有則預設給良品倉
				var isSetTypeId = false;
				if (string.IsNullOrEmpty(f050001.TYPE_ID))
				{
					isSetTypeId = true;
					var f190002 = f190002s.FirstOrDefault(a => a.TICKET_ID == f050001.TICKET_ID);
					if (f190002 != null)
						f050001.TYPE_ID = f190002.WAREHOUSE_TYPE;
					else
						f050001.TYPE_ID = "G";
				}

				if (isSetZipCode || isSetDc || isSetTypeId)
				{
					f050001Rep.UpdateZipCodeDc(f050001.ORD_NO, f050001.GUP_CODE, f050001.CUST_CODE, f050001.ZIP_CODE, f050001.DC_CODE, f050001.TYPE_ID, f050001.ADDRESS_PARSE);
				}
			}
			_wmsTransaction.Complete();
			return true;
		}

		private bool AllotDc(F050001 f050001, List<F050006> f050006s)
		{
			var zipCode3 = (f050001.ZIP_CODE != null && f050001.ZIP_CODE.Length == 5) ? f050001.ZIP_CODE.Substring(0, 3) : f050001.ZIP_CODE;
			var dcCode = f050006s.Where(a => a.ZIP_CODE == zipCode3 && a.GUP_CODE == f050001.GUP_CODE && a.CUST_CODE == f050001.CUST_CODE).Select(a => a.DC_CODE).SingleOrDefault();
			if (string.IsNullOrEmpty(dcCode))
				return false;

			f050001.DC_CODE = dcCode;
			return true;
		}

		private List<List<F050001>> PickOrders(List<F050001> f050001s, List<F050002> f050002s, string gupCode, string custCode, string dcCode)
		{
			var results = new List<List<F050001>>();

			//先排除專車且未派車的單據
			PickOutSpecialCarNoCar(gupCode, custCode, dcCode, ref f050001s);

			var ticketIds = f050001s.Select(a => a.TICKET_ID).Distinct().ToList();
			var f050003Rep = new F050003Repository(Schemas.CoreSchema, _wmsTransaction);
			//取得貨主的特殊商品
			var specialItems = f050003Rep.GetSpecialItems(ticketIds, gupCode, custCode, dcCode);

			//1.依檔案類別區分及排序
			var f190001Rep = new F190001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f190001s = f190001Rep.GetDatas(dcCode, gupCode, custCode).ToList();
			var f050001Gs = (from a in f050001s.GroupBy(a => new { a.TICKET_ID })
											 join b in f190001s on a.Key.TICKET_ID equals b.TICKET_ID
											 orderby b.PRIORITY
											 select a);
			var f1909 = GetF1909(gupCode, custCode);
			foreach (var f050001G in f050001Gs)
			{
				//依到貨日期過濾可出貨訂單
				var f050004Rep = new F050004Repository(Schemas.CoreSchema, _wmsTransaction);
				var f050004 = f050004Rep.GetData(dcCode, gupCode, custCode, f050001G.Key.TICKET_ID);
				var f050001Cs = f050001G.ToList();

				//貨主設定是否允許預先配庫 (0:不允許 1:允許)
				if (f1909.ALLOW_ADVANCEDSTOCK == "0")
					f050001Cs = f050001Cs.Where(a => (a.ARRIVAL_DATE.HasValue && a.ARRIVAL_DATE.Value.AddDays(-1) <= DateTime.Today)
							|| (!a.ARRIVAL_DATE.HasValue && a.ORD_DATE.AddDays(f050004.DELV_DAY).AddDays(-1) <= DateTime.Today)).ToList();

				f050001Cs = f050001Cs.Take(f050004.ORDER_LIMIT).ToList();


				#region 2.過濾特殊商品
				//特殊商品訂單 (使用 Linq 的 Inner Join 過濾)
				var specialList = (from a in f050001Cs
													 join b in f050002s on new { a.ORD_NO, a.GUP_CODE, a.CUST_CODE, a.DC_CODE } equals new { b.ORD_NO, b.GUP_CODE, b.CUST_CODE, b.DC_CODE }
													 join c in specialItems on new { a.TICKET_ID, b.ITEM_CODE } equals new { c.TICKET_ID, c.ITEM_CODE }
													 select a).ToList();
				//非特殊商品訂單
				f050001Cs.RemoveAll(a => specialList.Contains(a));
				var notSpecalList = f050001Cs;
				#endregion 2.過濾特殊商品

				#region 3.訂單量超過數量則依東部、中南部、北部訂單順序處理

				List<F050007> f050007s = null;
				List<string> zipCodesE = null;
				List<string> zipCodesS = null;
				List<string> zipCodesN = null;
				if (specialList.Count > f050004.SOUTH_PRIORITY_QTY || notSpecalList.Count > f050004.SOUTH_PRIORITY_QTY)
				{
					var f050007Rep = new F050007Repository(Schemas.CoreSchema, _wmsTransaction);
					f050007s = f050007Rep.GetDatas(gupCode, custCode).ToList();
					zipCodesE = f050007s.Where(a => a.REGION_CODE == "E").Select(a => a.ZIP_CODE).ToList();
					zipCodesS = f050007s.Where(a => a.REGION_CODE == "S").Select(a => a.ZIP_CODE).ToList();
					zipCodesN = f050007s.Where(a => a.REGION_CODE == "N").Select(a => a.ZIP_CODE).ToList();
				}

				if (specialList.Count > f050004.SOUTH_PRIORITY_QTY)
				{
					//特殊商品訂單數超過指定數量，依東部、中南部、北部訂單順序處理
					var specalListE = specialList.Where(a => zipCodesE.Contains(a.ZIP_CODE)).ToList();
					var specalListS = specialList.Where(a => zipCodesS.Contains(a.ZIP_CODE)).ToList();
					var specalListN = specialList.Where(a => zipCodesN.Contains(a.ZIP_CODE)).ToList();
					if (specalListE.Any()) results.Add(specalListE);
					if (specalListS.Any()) results.Add(specalListS);
					if (specalListN.Any()) results.Add(specalListN);
				}
				else if (specialList.Any()) //沒超量
					results.Add(specialList);

				if (notSpecalList.Count > f050004.SOUTH_PRIORITY_QTY)
				{
					//非特殊商品訂單數超過指定數量，依東部、中南部、北部訂單順序處理
					var notSpecalListE = notSpecalList.Where(a => zipCodesE.Contains(a.ZIP_CODE)).ToList();
					var notSpecalListS = notSpecalList.Where(a => zipCodesS.Contains(a.ZIP_CODE)).ToList();
					var notSpecalListN = notSpecalList.Where(a => zipCodesN.Contains(a.ZIP_CODE)).ToList();
					if (notSpecalListE.Any()) results.Add(notSpecalListE);
					if (notSpecalListS.Any()) results.Add(notSpecalListS);
					if (notSpecalListN.Any()) results.Add(notSpecalListN);
				}
				else if (notSpecalList.Any()) //沒超量
					results.Add(notSpecalList);
				#endregion 3.訂單量超過數量則依東部、中南部、北部訂單順序處理
			}

			return results;
		}

		private void PickOutSpecialCarNoCar(string gupCode, string custCode, string dcCode, ref List<F050001> f050001s)
		{
			//先排除專車且未派車的單據
			var f700102Rep = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
			//若為銷毀單，派車單關聯的是銷毀單號，所以要取SOURCE_NO
			var specialCarOrdNos = f050001s.Where(a => a.SPECIAL_BUS == "1").Select(a => (a.SOURCE_TYPE == "08") ? a.SOURCE_NO : a.ORD_NO).Distinct().ToList();
			if (specialCarOrdNos.Any())
			{
				// 取得待處理的派車單明細
				_specialCars = f700102Rep.GetF700102ByWmsNos(dcCode, gupCode, custCode, status: "0", wmsNos: specialCarOrdNos).ToList();
				var noSpecialCarOrdNos = (from a in specialCarOrdNos
																	join b in _specialCars on a equals b.WMS_NO into j
																	from c in j.DefaultIfEmpty()
																	where c == null
																	select a).ToList();

				f050001s = f050001s.Where(a => !noSpecialCarOrdNos.Contains((a.SOURCE_TYPE == "08") ? a.SOURCE_NO : a.ORD_NO)).ToList();

				if (noSpecialCarOrdNos.Any())
				{
					var msgNo = "AAM00003"; //訂單:「{0}」為專車，尚未配車，需先派車才可配庫
					var f0020 = GetF0020(msgNo);
					var messageContent = string.Format(f0020.MSG_CONTENT, string.Join("、", noSpecialCarOrdNos));
					AddMessagePoolForInside("9", dcCode, gupCode, custCode, msgNo, messageContent);
				}
			}
		}
		#endregion 讀取貨主訂單—自動篩選

		#region 檢查庫存
		/// <summary>
		/// 檢查庫存
		/// </summary>
		/// <param name="f050001s">須為同貨主同一訂單類型(B2B或B2C)的訂單</param>
		/// <returns></returns>
		private bool CheckStocks(List<F050001> f050001s, IEnumerable<F050002> f050002s, List<F05030201> f05030201s)
		{
			//因為每次配庫的品項不一定相同，配庫前先初始化_itemLimitValidDays
			_itemLimitValidDays = new List<ItemLimitValidDay>();

			var f050302Repo = new F050302Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050001First = f050001s.First();
			var isB2b = f050001First.ORD_TYPE == "0";
			var gupCode = f050001First.GUP_CODE;
			var custCode = f050001First.CUST_CODE;
			var dcCode = f050001First.DC_CODE;
			var sourceType = f050001First.SOURCE_TYPE;
			var notEnoughWareHouses = new Dictionary<string, Dictionary<string, int>>();
			// 已使用的指定序號<序號, 序號出現次數>
			var usedAssignationSerials = new Dictionary<string, int>();
			var f1909 = GetF1909(gupCode, custCode);
			//依倉別群組化訂單
			var f050001GTs = f050001s.GroupBy(a => new { a.TYPE_ID });
			//取得目前批次最新時段
			_pickDateTimeMax = GetMaxPickTime(dcCode, gupCode, custCode, DateTime.Today);

			foreach (var f050001GT in f050001GTs)
			{
				var notEnoughItems = new Dictionary<string, int>();

				//檢查總庫存時要排除不出庫
				var f050002ListByNotNoDelv = f050002s.Where(o => o.NO_DELV == "0").ToList();

				//用Linq的Inner Join過濾同一倉別訂單明細，並用ITEM_CODE群組化訂單明細
				var f050002GIs = (from a in f050002ListByNotNoDelv
													join b in f050001GT.AsEnumerable() on new { a.ORD_NO, a.GUP_CODE, a.CUST_CODE, a.DC_CODE } equals new { b.ORD_NO, b.GUP_CODE, b.CUST_CODE, b.DC_CODE }
													select a).GroupBy(a => new { a.ITEM_CODE });
				foreach (var f050002GI in f050002GIs)
				{
					//取得商品庫存數
					var stocks = GetStock(dcCode, gupCode, custCode, f050002GI.Key.ITEM_CODE, f050001GT.Key.TYPE_ID);
					var sumItemQty = f050002GI.Sum(a => a.ORD_QTY);
					if (stocks < sumItemQty)
					{
						if (f1909.SAM_ITEM == "1" && CheckNeedCheckSameItem(f050001First.TICKET_ID)) //同質性商品轉換
						{
							var checkSameQty = sumItemQty - stocks;
							if (f050002GI.Any(a => a.CHECKED_SAMEITEM == "1"))
							{
								//檢查上次配庫是否有庫存不足檢查過同質性商品，
								//若有則以本次的總數量扣掉上次已檢查過的數量為這次要檢查的數量，
								//(不考慮期間是否有進倉，此部分由人員自行判斷，在修改產生的加工單數量及調撥單數量)
								var checkedItemQty = f050002GI.Where(a => a.CHECKED_SAMEITEM == "1").Sum(a => a.ORD_QTY);
								checkSameQty = sumItemQty - checkedItemQty;
							}
							if (checkSameQty > 0)
							{
								//檢查是否有同質性商品並產生流通加工單及調撥單
								var isCheckOk = CheckSameItem(dcCode, gupCode, custCode, f050002GI.Key.ITEM_CODE, checkSameQty, f050001GT.Key.TYPE_ID);
								if (isCheckOk)
								{
									var ordNos = f050002GI.Select(a => a.ORD_NO).Distinct().ToList();
									f050002Repo.UpdateCheckedSameItem(gupCode, custCode, dcCode, f050002GI.Key.ITEM_CODE, ordNos, "1");
								}
							}
						}

						notEnoughItems.Add(f050002GI.Key.ITEM_CODE, sumItemQty - stocks);
					}
					else
					{
						// 這裡是紀錄指定序號重複使用次數，並且將不足的品項+1
						foreach (var f050002 in f050002GI.Where(x => !string.IsNullOrWhiteSpace(x.SERIAL_NO)))
						{
							// 使用該序號次數 +1
							if (!usedAssignationSerials.ContainsKey(f050002.SERIAL_NO)) usedAssignationSerials.Add(f050002.SERIAL_NO, 0);
							usedAssignationSerials[f050002.SERIAL_NO]++;

							// 取得已使用指定序號且尚未配庫的訂單編號，若有的話，表示重複使用序號 +1
							var ordNo = f050302Repo.GetOrdNoByUsedAssignationSerial(f050002.DC_CODE, f050002.GUP_CODE, f050002.CUST_CODE, f050002.SERIAL_NO);
							if (!string.IsNullOrEmpty(ordNo))
								usedAssignationSerials[f050002.SERIAL_NO]++;

							// 檢查指定序號有無重複配庫
							if (usedAssignationSerials[f050002.SERIAL_NO] <= 1)
								continue;

							// 將該序號商品不足數 +1
							if (!notEnoughItems.ContainsKey(f050002GI.Key.ITEM_CODE)) notEnoughItems.Add(f050002GI.Key.ITEM_CODE, 0);
							notEnoughItems[f050002GI.Key.ITEM_CODE]++;
						}
					}
				}


				if (f1909.SPILT_OUTCHECK == "0") //不允許部分出貨才需踢單
				{
					//有品項不足的訂單
					if (notEnoughItems.Any())
					{
						notEnoughWareHouses.Add(f050001GT.Key.TYPE_ID, notEnoughItems);
						if (!isB2b || f1909.ISB2B_ALONE_OUT == "1") //B2C或貨主設定允許B2B單張訂單出貨 移除庫存不足的訂單
						{
							//用 Linq 的 Inner Join 取得含有缺貨的訂單
							var f050001NEs = (from a in f050001GT.AsEnumerable()
																join b in f050002ListByNotNoDelv on new { a.ORD_NO, a.GUP_CODE, a.CUST_CODE, a.DC_CODE } equals new { b.ORD_NO, b.GUP_CODE, b.CUST_CODE, b.DC_CODE }
																join c in notEnoughItems.Keys on b.ITEM_CODE equals c
																select a).Distinct();

							//取出含有缺貨的訂單所含商品及數量
							var f050001Items = (from a in f050001NEs
																	join b in f050002ListByNotNoDelv on new { a.ORD_NO, a.GUP_CODE, a.CUST_CODE, a.DC_CODE } equals new { b.ORD_NO, b.GUP_CODE, b.CUST_CODE, b.DC_CODE }
																	select new { a.ORD_NO, b.ITEM_CODE, b.ORD_QTY }).GroupBy(d => d.ORD_NO)
																											.Select(g => new { ORD_NO = g.Key, Items = g.ToList(), ItemCount = g.Count() }).ToList();
							//取得一單一品訂單，再依商品數及訂單編號排序
							var f050001SingleItems = f050001Items.Where(a => a.ItemCount == 1).OrderBy(t => t.Items.Single().ORD_QTY).ThenByDescending(t => t.ORD_NO).ToList();

							f050001Items.RemoveAll(a => f050001SingleItems.Contains(a));
							//一單多品依品項數及訂單編號排序
							f050001Items = f050001Items.OrderBy(t => t.ItemCount).ThenByDescending(t => t.ORD_NO).ToList();

							var notEnoughItemsTmp = notEnoughItems.Select(a => new KeyValuePair<string, int>(a.Key, a.Value)).ToDictionary(a => a.Key, a => a.Value);
							//釋放一單一品訂單
							foreach (var f050001Item in f050001SingleItems)
							{
								var isRelease = false;
								var item = f050001Item.Items.Single();
								var notEnoughItemQty = notEnoughItemsTmp[item.ITEM_CODE];
								if (notEnoughItemQty > 0)
								{
									notEnoughItemQty = notEnoughItemQty - item.ORD_QTY;
									notEnoughItemsTmp[item.ITEM_CODE] = notEnoughItemQty;
									isRelease = true;
								}
								if (isRelease)
									f050001s.Remove(f050001s.Single(a => a.ORD_NO == f050001Item.ORD_NO));
							}

							//釋放一單多品訂單
							foreach (var f050001Item in f050001Items)
							{
								var isRelease = false;
								foreach (var item in f050001Item.Items)
								{
									if (notEnoughItemsTmp.Keys.Contains(item.ITEM_CODE))
									{
										var notEnoughItemQty = notEnoughItemsTmp[item.ITEM_CODE];
										if (notEnoughItemQty > 0)
										{
											notEnoughItemQty = notEnoughItemQty - item.ORD_QTY;
											notEnoughItemsTmp[item.ITEM_CODE] = notEnoughItemQty;
											isRelease = true;
										}
									}
								}
								if (isRelease)
									f050001s.Remove(f050001s.Single(a => a.ORD_NO == f050001Item.ORD_NO));
							}
						}
					}
				}
			}

			//有品項不足的訂單
			if (notEnoughWareHouses.Any())
			{
				//丟到訊息池
				var notEnoughMsgs = new List<string>();
				var notEnoughMsgsCust = new List<string>();
				var f1901 = GetF1901(dcCode);
				var f1929 = GetF1929(gupCode);
				var f198001Rep = new F198001Repository(Schemas.CoreSchema, _wmsTransaction);
				var f198001s = f198001Rep.InWithTrueAndCondition("TYPE_ID", notEnoughWareHouses.Keys.ToList()).ToList();

				var msgNo = "AAM00004"; //物流中心:「{0}」業主:「{1}」貨主:「{2}」倉別:「{3}」商品:「{4}:{5}」總庫存不足數量:{6}
				var f0020 = GetF0020(msgNo);
				var msgNoCust = "AAM00016"; //物流中心:「{0}」商品:「{1}:{2}」總庫存不足數量:{3}
				var f0020Cust = GetF0020(msgNoCust);
				foreach (var notEnoughWareHouse in notEnoughWareHouses)
				{
					var f1903s = GetF1903s(gupCode, custCode, notEnoughWareHouse.Value.Keys.ToList());
					var f198001 = f198001s.Where(a => a.TYPE_ID == notEnoughWareHouse.Key).SingleOrDefault();

					foreach (var notEnoughItem in notEnoughWareHouse.Value)
					{
						var f1903 = f1903s.Where(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.ITEM_CODE == notEnoughItem.Key).SingleOrDefault();
						notEnoughMsgs.Add(string.Format(f0020.MSG_CONTENT,
								(f1901 == null) ? "" : f1901.DC_NAME,
								(f1929 == null) ? "" : f1929.GUP_NAME,
								(f1909 == null) ? "" : f1909.SHORT_NAME,
								(f198001 == null) ? "" : f198001.TYPE_NAME,
								notEnoughItem.Key,
								(f1903 == null) ? "" : f1903.ITEM_NAME,
								notEnoughItem.Value));

						notEnoughMsgsCust.Add(string.Format(f0020Cust.MSG_CONTENT,
								(f1901 == null) ? "" : f1901.DC_NAME,
								(f1903 == null) ? "" : f1903.CUST_ITEM_CODE,
								(f1903 == null) ? "" : f1903.ITEM_NAME,
								notEnoughItem.Value));
					}
				}
				if (notEnoughMsgs.Any())
					AddMessagePoolForInside("9", dcCode, gupCode, custCode, msgNo, string.Join("\n", notEnoughMsgs));

				var notEnoughMsgs2 = new List<string>();
				msgNo = "AAM00005"; //物流中心:「{0}」業主:「{1}」貨主:「{2}」指定序號:「{3}」重複配庫
				f0020 = GetF0020(msgNo);
				// 指定序號重複配庫，或者在這批次裡面重複配庫
				foreach (var usedAssignationSerial in usedAssignationSerials.Where(x => x.Value > 1))
				{
					notEnoughMsgs2.Add(string.Format(f0020.MSG_CONTENT,
																					(f1901 == null) ? "" : f1901.DC_NAME,
																					(f1929 == null) ? "" : f1929.GUP_NAME,
																					(f1909 == null) ? "" : f1909.SHORT_NAME,
																					usedAssignationSerial.Key));
				}

				if (notEnoughMsgs2.Any())
					AddMessagePoolForInside("9", dcCode, gupCode, custCode, msgNo, string.Join("\n", notEnoughMsgs2));
				//通知貨主，丟到訊息池
				if (notEnoughMsgsCust.Any())
					AddMessagePoolForCust("9", dcCode, gupCode, custCode, msgNoCust, string.Join("\n", notEnoughMsgsCust), false);

				if (isB2b && f1909.ISB2B_ALONE_OUT == "0") //B2B且貨主設定不允許B2B單張訂單出貨  全部回訂單池
					return false;
			}

			//貨主主檔設定判斷效期允出天數
			if (isB2b && f1909.ISALLOW_DELV_DAY == "1")
			{
				//預先由資料庫取出各門市的商品的效期天數供後續由記憶體取用
				var retailCodes = f050001s.Select(a => a.RETAIL_CODE).Distinct().ToList();
				foreach (var retailCode in retailCodes)
				{
					var ordNos = f050001s.Where(a => a.RETAIL_CODE == retailCode).Select(a => a.ORD_NO).ToList();
					var ordItemCodes = f050002s.Where(a => ordNos.Contains(a.ORD_NO)).Select(a => a.ITEM_CODE).Distinct().ToList();
					GetItemLimitValidDays(gupCode, custCode, retailCode, ordItemCodes);
				}

				//不允許部分出貨才需踢單
				if (f1909.SPILT_OUTCHECK == "0")
				{
					var noEnoughOrder = new List<string>();
					var f1913Repo = new F1913Repository(Schemas.CoreSchema);
					var gwF050001s = f050001s.GroupBy(a => new { a.TYPE_ID });
					foreach (var gF050001 in gwF050001s)
					{
						var wF050002s = (from a in f050002s
														 join b in gF050001.AsEnumerable() on a.ORD_NO equals b.ORD_NO
														 select a).ToList();
						var itemCodes = (from a in wF050002s
														 select a.ITEM_CODE).Distinct().ToList();
						var pickLocPriorityInfos = f1913Repo.GetItemPickLocPriorityInfo(dcCode, gupCode, custCode, itemCodes, false, gF050001.Key.TYPE_ID).ToList();
						foreach (var f050001 in gF050001)
						{
							var ordF050002s = f050002s.Where(a => a.ORD_NO == f050001.ORD_NO);
							var tempPickLocPriorityInfos = new List<ItemLocPriorityInfo>();
							pickLocPriorityInfos.ForEach((x) =>
							{
								ItemLocPriorityInfo y = new ItemLocPriorityInfo();
								x.CloneProperties(y);
								tempPickLocPriorityInfos.Add(y);
							});
							if (!CheckValidDate(f050001, ordF050002s, tempPickLocPriorityInfos))
							{
								//庫存不足移除不配庫
								f050001s.Remove(f050001);
								f050002s = f050002s.Where(x => x.ORD_NO != f050001.ORD_NO);
								noEnoughOrder.Add(f050001.ORD_NO);
							}
						}
					}
				}
			}

			//允許部分出貨且依出車時段平均分配，若沒設出車時段則踢單
			if (f1909.SPILT_OUTCHECK == "1" && f1909.SPILT_OUTCHECKWAY == "1")
			{
				var noCarPeriodsOrds = GetNoCarPeriodOrds(dcCode, gupCode, custCode, f050002s.ToList(), f050001s);
			}

			List<F050301> f050301s;
			List<F050302> f050302s;
			//複製訂單至F050301、F050302
			var result = CopyDataToF050301(f050001s, f050002s, f05030201s, out f050301s, out f050302s);
			if (!result)
				return result;


			//配庫拆併單
			TakeApartAndMergeOrder(gupCode, custCode, dcCode, f050301s, f050302s, isB2b, sourceType);

			//如果已產生調撥單資料 整批寫入調撥單
			if (_returnNewAllocations != null && _returnNewAllocations.Any())
			{
				_sharedService.BulkInsertAllocation(_returnNewAllocations, _returnStocks);
				_returnStocks = null;
				_returnNewAllocations = null;
			}

			return true;
		}

		private bool CheckNeedCheckSameItem(decimal ticketId)
		{
			var f190001Rep = new F190001Repository(Schemas.CoreSchema, _wmsTransaction);
			var ticketClass = f190001Rep.Find(a => a.TICKET_ID == ticketId).TICKET_CLASS;
			if (ticketClass == "O1" //門市出貨(B2B)
					|| ticketClass == "O2" //消費者出貨(B2C)
					|| ticketClass == "O5" //內部交易/互賣出貨
					|| ticketClass == "O6" //換貨單出貨(B2B)
					|| ticketClass == "O7" //換貨單出貨(B2C)
					)
				return true;

			return false;
		}

		private int GetStock(string dcCode, string gupCode, string custCode, string itemCode, string warehouseType, bool isGWithVirtual = true, bool isIncludeResupply = false)
		{
			var f1913Rep = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var stock = 0;
			if (warehouseType == "G" && isGWithVirtual) //良品倉
			{
				//庫存需再加上補貨區及虛擬倉的庫存
				stock = f1913Rep.GetItemStockWithVirtual(dcCode, gupCode, custCode, itemCode, warehouseType, false);
			}
			else if (warehouseType == "G" && !isGWithVirtual && isIncludeResupply)
			{
				//庫存需再加上補貨區的庫存
				stock = f1913Rep.GetItemStock(dcCode, gupCode, custCode, itemCode, warehouseType, true);
			}
			else if (warehouseType == "D")
			{
				// 報廢倉，單純取得該商品存在於倉別的總庫存量，不會過濾效期
				stock = f1913Rep.GetItemStock(dcCode, gupCode, custCode, itemCode, warehouseType);
			}
			else
			{
				//庫存不包含補貨區及虛擬倉的庫存
				stock = f1913Rep.GetItemStockWithoutResupply(dcCode, gupCode, custCode, itemCode, warehouseType);
			}

			return stock;
		}

		private bool CheckSameItem(string dcCode, string gupCode, string custCode, string itemCode, int notEnoughItemQty, string warehouseType)
		{
			var results = new List<ExecuteResult>();
			//查詢Bom表同質性商品
			var f1903Rep = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var bomSameItems = f1903Rep.GetSameItemWithBom(gupCode, custCode, itemCode).ToList();
			var sameItemCode = string.Empty;
			BomSameItem bomSameItem = null;
			foreach (var tmpBomSameItem in bomSameItems)
			{
				var stock = GetStock(dcCode, gupCode, custCode, tmpBomSameItem.ITEM_CODE, warehouseType, false, true);
				var sameToItemQty = (tmpBomSameItem.BOM_TYPE == "0") ? (int)(stock / tmpBomSameItem.BOM_QTY.Value) : (int)(stock * tmpBomSameItem.BOM_QTY.Value);
				if (sameToItemQty >= notEnoughItemQty)
				{
					sameItemCode = tmpBomSameItem.ITEM_CODE;
					bomSameItem = tmpBomSameItem;
					break;
				}
			}

			//若不夠查詢數量夠的同質性商品
			if (bomSameItem == null)
			{
				var sameItemCodes = bomSameItems.Select(a => a.ITEM_CODE).Distinct().ToList();
				var sameItemCodes2 = f1903Rep.GetSameItems(gupCode, itemCode, custCode, sameItemCodes).ToList();
				foreach (var sameItemCodeTmp in sameItemCodes2)
				{
					var stock = GetStock(dcCode, gupCode, custCode, sameItemCodeTmp, warehouseType, false);
					if (stock >= notEnoughItemQty)
					{
						sameItemCode = sameItemCodeTmp;
						break;
					}
				}
			}
			else //如果Bom表中有找到足夠數量的同質性商品則產生工單
			{
				var processNo = CreateProcess(dcCode, gupCode, custCode, (bomSameItem.BOM_TYPE == "0") ? itemCode : sameItemCode,
						(bomSameItem.BOM_TYPE == "0")
								? notEnoughItemQty
								: (int)Math.Ceiling((Decimal)notEnoughItemQty / bomSameItem.BOM_QTY.Value), bomSameItem.BOM_NO);

				if (!string.IsNullOrEmpty(processNo))
				{
					//丟到訊息池
					var msgNo = "AAM00006"; //有同質性商品需加工，請至加工單維護處理，流通加工編號:{0}
					var f0020 = GetF0020(msgNo);
					AddMessagePoolForInside("9", dcCode, gupCode, custCode, msgNo, string.Format(f0020.MSG_CONTENT, processNo));
				}
				else
					return false;
			}

			//如果有找到足夠數量的同質性商品則產生調撥單
			if (!string.IsNullOrEmpty(sameItemCode))
			{
				var f1980Rep = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);
				var f1912Rep = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
				//var targetF1980 = f1980Rep.GetFirstData(dcCode, "W");
				//if (targetF1980 == null)
				//{
				//	//丟到訊息池
				//	AddLoMessagePoolForInside("9", dcCode, gupCode, custCode, "尚未建立流通加工倉庫");
				//	return;
				//}
				//var targetLoc = f1912Rep.GetFirstData(dcCode, targetF1980.WAREHOUSE_ID);
				var notEnoughItems = new List<ItemQty> {
										new ItemQty {
												ItemCode = sameItemCode,
												Qty = (bomSameItem == null) ? notEnoughItemQty :
																				((bomSameItem.BOM_TYPE == "0")? notEnoughItemQty * bomSameItem.BOM_QTY.Value : (int)((notEnoughItemQty + bomSameItem.BOM_QTY.Value - 1) / bomSameItem.BOM_QTY.Value))
										}
								};

				//建立調撥單
				var allocationNos = CreateAllocation(dcCode, gupCode, custCode, notEnoughItems, warehouseType, "W");
				//丟到訊息池
				var msgNo = "AAM00007"; //有同質性商品需調撥，請至調撥單維護處理，調撥單號:{0}
				var f0020 = GetF0020(msgNo);
				AddMessagePoolForInside("9", dcCode, gupCode, custCode, msgNo, string.Format(f0020.MSG_CONTENT, string.Join("、", allocationNos)));
			}

			return true;
		}

		/// <summary>
		/// 建立流通加工單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="processQty"></param>
		/// <param name="bomNo"></param>
		private string CreateProcess(string dcCode, string gupCode, string custCode, string itemCode, int processQty, string bomNo)
		{
			var f910301Rep = new F910301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1909 = GetF1909(gupCode, custCode);
			var f910401 = f910301Rep.GetF910301WithF910401(gupCode, dcCode, f1909.UNI_FORM, DateTime.Today.ToString("yyyy/MM/dd")).FirstOrDefault();
			if (f910401 == null)
			{
				var msgNo = "AAM00008"; //物流中心「{0}」貨主「{1}」找不到合約有效期間內的加工報價單
				var f0020 = GetF0020(msgNo);
				var f1901 = GetF1901(dcCode);
				AddMessagePoolForInside("9", dcCode, gupCode, custCode, msgNo, string.Format(f0020.MSG_CONTENT, f1901.DC_NAME, f1909.SHORT_NAME));
				return string.Empty;
			}
			var f190301Rep = new F190301Repository(Schemas.CoreSchema, _wmsTransaction);
			var boxQty = 1;
			var caseQty = 1;
			var f190301 = f190301Rep.Find(a => a.GUP_CODE == gupCode && a.ITEM_CODE == itemCode && a.UNIT_ID == "002", false);
			if (f190301 != null)
				boxQty = f190301.UNIT_QTY;
			f190301 = f190301Rep.Find(a => a.GUP_CODE == gupCode && a.ITEM_CODE == itemCode && a.UNIT_ID == "003", false);
			if (f190301 != null)
				caseQty = f190301.UNIT_QTY;
			var finishDate = DateTime.Today.AddHours(18);
			var finishTime = DateTime.Now.ToString("HH:mm");
			var result = _sharedService.InsertF910201(dcCode, gupCode, custCode, "1", f910401.OUTSOURCE_ID, finishDate, itemCode, bomNo, processQty, boxQty, caseQty, "", "", f910401.QUOTE_NO, finishTime, "1");
			if (result.IsSuccessed)
				return result.Message;
			return string.Empty;
		}

        /// <summary>
        /// 建立調撥單
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="notEnoughItemQty"></param>
        /// <param name="srcWarehouseType"></param>
        /// <returns></returns>
        private List<string> CreateAllocation(string dcCode, string gupCode, string custCode, List<ItemQty> notEnoughItems, string srcWarehouseType, string targetWarehouseType)
        {
            if (_returnStocks == null)
				_returnStocks = new List<F1913>();
			var allocationParam = new NewAllocationItemParam
			{
				AllocationType = AllocationType.Both,
				GupCode = gupCode,
				CustCode = custCode,
				SrcDcCode = dcCode,
				ReturnStocks = _returnStocks,
				SrcStockFilterDetails = notEnoughItems.GroupBy(x => x.ItemCode).Select(x => new StockFilter
				{
					ItemCode = x.Key,
					Qty = x.Sum(y => y.Qty),
				}).ToList(),
				SrcWarehouseType = srcWarehouseType,
                TarDcCode = dcCode,
                TarWarehouseType = targetWarehouseType,
                SourceType = "01",
                //AllocationTypeCode 
                //ContainerCode = ContainerCode,
                //F0701_ID = F0701_ID,
                //PRE_TAR_WAREHOUSE_ID = PRE_TAR_WAREHOUSE_ID
            };

			var result = _sharedService.CreateOrUpdateAllocation(allocationParam);
			if (_returnNewAllocations == null)
				_returnNewAllocations = new List<ReturnNewAllocation>();
			_returnNewAllocations.AddRange(result.AllocationList);
			_returnStocks = result.StockList;
			return result.AllocationList.Select(x => x.Master.ALLOCATION_NO).ToList();
		}

		private bool CopyDataToF050301(List<F050001> f050001s, IEnumerable<F050002> f050002s, List<F05030201> f05030201s, out List<F050301> f050301s, out List<F050302> f050302s)
		{
			var f050301Rep = new F050301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050302Rep = new F050302Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030201Rep = new F05030201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050001Rep = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050002Rep = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);

            List<F1903> f1903s = null;
			var custOrdSerialNos = (from a in new List<object>()
															select new { CUST_ORD_NO = "", SERIAL_NO = "", ITEM_CODE = "" }).ToList();
			List<F05000201> f05000201s = null;
			if (f050001s.Any())
			{
				var f050001First = f050001s.First();
				var itemCodes = f050002s.Select(a => a.ITEM_CODE).Distinct().ToList();
				f1903s = GetF1903s(f050001First.GUP_CODE, f050001First.CUST_CODE, itemCodes);

				var custOrdNos = f050001s.Select(a => a.CUST_ORD_NO).Distinct().ToList();
				var f05000201Rep = new F05000201Repository(Schemas.CoreSchema, _wmsTransaction);
				//取Welcome Letter的序號
				f05000201s = f05000201Rep.InWithTrueAndCondition("CUST_ORD_NO", custOrdNos, a => a.GUP_CODE == f050001First.GUP_CODE && a.CUST_CODE == f050001First.CUST_CODE).ToList();
				if (f05000201s.Any())
				{
					var serialNos = f05000201s.Select(a => a.SERIAL_NO).Distinct().ToList();
					var f2501Rep = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
					var f2501s = f2501Rep.InWithTrueAndCondition("SERIAL_NO", serialNos, a => a.GUP_CODE == f050001First.GUP_CODE && a.CUST_CODE == f050001First.CUST_CODE).ToList();
					//取Welcome Letter的序號關聯序號檔的資料
					custOrdSerialNos = (from a in f05000201s
															join b in f2501s on a.SERIAL_NO equals b.SERIAL_NO
															select new { a.CUST_ORD_NO, a.SERIAL_NO, b.ITEM_CODE }).ToList();
				}

			}
			f050301s = new List<F050301>();
			f050302s = new List<F050302>();
			foreach (var f050001 in f050001s)
			{
				var ordF050002s = f050002s.Where(a => a.ORD_NO == f050001.ORD_NO).ToList();
				foreach (var f050002 in ordF050002s)
				{
					if (f1903s.Any(a => a.ITEM_CODE == f050002.ITEM_CODE && a.BUNDLE_SERIALLOC == "1" && string.IsNullOrEmpty(f050002.SERIAL_NO)))
					{
						List<string> itemSerialNos = null;
						//若沒取到，取Welcome Letter的序號關聯序號檔的資料
						if (custOrdSerialNos != null && custOrdSerialNos.Any())
						{
							itemSerialNos = custOrdSerialNos.Where(a => a.CUST_ORD_NO == f050001.CUST_ORD_NO && a.ITEM_CODE == f050002.ITEM_CODE).Select(a => a.SERIAL_NO).ToList();
						}
						if (itemSerialNos == null || !itemSerialNos.Any())
						{
							var f050302SnLoc = AutoMapper.Mapper.DynamicMap<F050302>(f050002);
							f050302s.Add(f050302SnLoc);
						}
						else if (itemSerialNos.Count < f050002.ORD_QTY)
						{
							//丟到訊息池
							var msgNo = "AAM00009"; //訂單「{0}」的商品編號:「{1}」序號數量不足訂貨數
							var f0020 = GetF0020(msgNo);
							AddMessagePoolForInside("9", f050001.DC_CODE, f050001.GUP_CODE, f050001.CUST_CODE, msgNo, string.Format(f0020.MSG_CONTENT, f050001.ORD_NO, f050002.ITEM_CODE));
							return false;
						}
						else if (itemSerialNos.Count > f050002.ORD_QTY)
						{
							//丟到訊息池
							var msgNo = "AAM00010"; //訂單「{0}」的商品編號:「{1}」指定的序號數量大於訂貨數
							var f0020 = GetF0020(msgNo);
							AddMessagePoolForInside("9", f050001.DC_CODE, f050001.GUP_CODE, f050001.CUST_CODE, msgNo, string.Format(f0020.MSG_CONTENT, f050001.ORD_NO, f050002.ITEM_CODE));
							return false;
						}
						else
						{
							foreach (var itemSerialNo in itemSerialNos)
							{
								var f050302SnLoc = AutoMapper.Mapper.DynamicMap<F050302>(f050002);
								f050302SnLoc.SERIAL_NO = itemSerialNo;
								f050302SnLoc.ORD_QTY = 1;
								f050302s.Add(f050302SnLoc);
							}
						}
					}
					else
					{
						var f050302SnLoc = AutoMapper.Mapper.DynamicMap<F050302>(f050002);
						f050302s.Add(f050302SnLoc);
					}
				}
			}

			// 暫存要新增的531跟532訂單
			var f050301BulkInsertList = new List<F050301>();
			var f050302BulkInsertList = new List<F050302>();
			var f05030201BulkInsertList = new List<F05030201>();
			var pickTempNo = DateTime.Now.ToString("yyMMddHHmmss") + "_" + _pickIndex.ToString("00");
			f050301s = f050001s.Select(a => AutoMapper.Mapper.DynamicMap<F050301>(a)).ToList();
			foreach (var f050301 in f050301s)
			{
				if (f05000201s.Where(a => a.CUST_ORD_NO == f050301.CUST_ORD_NO && a.GUP_CODE == f050301.GUP_CODE && a.CUST_CODE == f050301.CUST_CODE).Any())
					f050301.HELLO_LETTER = "1";
				else
					f050301.HELLO_LETTER = "0";

				f050301.PROC_FLAG = "0";
				f050301.PICK_TEMP_NO = pickTempNo;
				f050301.ORD_PROP = f050001s.First().TRAN_CODE;
				if (string.IsNullOrEmpty(f050301.SELF_TAKE))
					f050301.SELF_TAKE = "0";


				var tmpF050302s = f050302s.Where(a => a.GUP_CODE == f050301.GUP_CODE
						&& a.CUST_CODE == f050301.CUST_CODE
						&& a.DC_CODE == f050301.DC_CODE
						&& a.ORD_NO == f050301.ORD_NO).ToList();

				var tmpF05030201s = f05030201s.Where(a => a.DC_CODE == f050301.DC_CODE
				&& a.GUP_CODE == f050301.GUP_CODE
				&& a.CUST_CODE == f050301.CUST_CODE
				&& a.ORD_NO == f050301.ORD_NO).ToList();



				foreach (var f050302 in tmpF050302s)
				{
					var f05030201 = tmpF05030201s.FirstOrDefault(o => o.ORD_SEQ == f050302.ORD_SEQ);
					if (f05030201 != null)
					{
						f05030201.ORD_SEQ = f050302.ORD_SEQ;
						f05030201BulkInsertList.Add(f05030201);
					}
					f050302BulkInsertList.Add(f050302);
				}
				//總材積
				f050301.VOLUMN = GetTotalItemsVolumn(f050301.GUP_CODE, f050301.CUST_CODE, tmpF050302s);
				//總重量
				f050301.WEIGHT = GetTotalItemsWeight(f050301.GUP_CODE, f050301.CUST_CODE, tmpF050302s);

				//宅配
				if (f050301.CVS_TAKE == "0" && f050301.SELF_TAKE == "0")
				{
					//如果有郵遞區號 且長度超過3碼 只抓取前三碼 以免對應不到出車時段
					if (!string.IsNullOrWhiteSpace(f050301.ZIP_CODE) && f050301.ZIP_CODE.Length > 3)
						f050301.ZIP_CODE = f050301.ZIP_CODE.Substring(0, 3);
					var f194704 = GetF194704(f050301.DC_CODE, f050301.GUP_CODE, f050301.CUST_CODE, f050301.ALL_ID);
					//如果取號方式為3外部取號 or 2虛擬取號
					if (f194704 != null && (f194704.GET_CONSIGN_NO == "3" || f194704.GET_CONSIGN_NO == "2"))
					{
						//有郵遞區號但不存在F19470101 視為找不到郵遞區號並設為空白 由貨主指定郵遞區號取代(如果有設定的話)
						if (!string.IsNullOrWhiteSpace(f050301.ZIP_CODE) && !IsFindDelvSetting(f050301.DC_CODE, f050301.ALL_ID, f050301.ZIP_CODE))
							f050301.ZIP_CODE = null;

						var f1909 = GetF1909(f050301.GUP_CODE, f050301.CUST_CODE);
						//無郵遞區號且指定貨主預設郵遞區號有設定(00000找不到,99999海外) 此訂單郵遞區號為貨主預設郵遞區號
						if (string.IsNullOrWhiteSpace(f050301.ZIP_CODE) && !string.IsNullOrWhiteSpace(f1909.ZIP_CODE))
							f050301.ZIP_CODE = f1909.ZIP_CODE;
					}
				}

				//批量裝車以指定到貨日資料篩選,如果指定到貨日未設定 預設D+1
				if (f050301.ARRIVAL_DATE == null)
					f050301.ARRIVAL_DATE = DateTime.Today.AddDays(1);

				f050301BulkInsertList.Add(f050301);
			}

			// 大量新增531跟532訂單
			f050301Rep.BulkInsert(f050301BulkInsertList);
			f050302Rep.BulkInsert(f050302BulkInsertList);
			f05030201Rep.BulkInsert(f05030201BulkInsertList);

			//將剩餘的訂單池資料狀態由1:配庫中改為0:未配庫
			//f050001Rep.SetStatus("0", "1");

			//此階段為一個段落，後續若配庫不成功，仍應停留在此階段，所以在這邊先完成一次Transaction
			_wmsTransaction.Complete();
			return true;
		}

		private Dictionary<int, List<string>> MergeOrder(List<F050301> f050301s, List<F050302> f050302s, bool isB2b, List<string> apartOrdNos)
		{
			var mergeOrders = new Dictionary<int, List<string>>();
			var mergeIdx = 0;
			//排除需拆單的訂單，再找出需併單的訂單 //20150709改成拆單仍要併單
			var gF050301s = (from a in f050301s
											 where //!apartOrdNos.Contains(a.ORD_NO) &&  //20150709改成拆單仍要併單
											 a.SPECIAL_BUS != "1" //專車不可併單 
											 && a.CVS_TAKE == "0" //超取不可併單
											 select a)
																			.GroupBy(a =>
																					new
																					{
																						a.ARRIVAL_DATE,  //指定到貨日
																									a.SP_DELV,       //特殊出貨
																									a.CHANNEL,       //通路類型
																									a.POSM,          //是否為POSM量更新
																									a.SELF_TAKE,     //自取
																									a.ALL_ID,        //指定配送商
																									a.PRINT_RECEIPT, //是否列印發票
																									a.RECEIPT_NO,    //指定或代產生發票號碼
																									a.SA,            //是否列印SA申裝書
																									a.HELLO_LETTER,  //是否列印welcome letter
																									a.RETAIL_CODE,   //門市編號
																									a.COLLECT,       //是否代收
																									a.CAN_FAST,      //快速到貨
																									a.ADDRESS,       //收件人地址
																									a.CONSIGNEE,     //收件人
																									a.TEL,           //電話
																									a.TEL_1,                 //收件人連絡電話1
																									a.TEL_2,                 //收件人連絡電話2
																									a.TEL_AREA,          //市話區碼
																									a.AGE,           //年紀
																									a.GENDER,        //性別(0:不明1:男2:女)
																									a.TYPE_ID,        //出貨倉別(F198001)
																									a.SPECIAL_BUS,    //專車
																									a.CUST_COST,      //成本中心
																									a.DELV_PERIOD,     //到貨時段
																									a.ROUND_PIECE      //來回件
																								});
			foreach (var gF050301 in gF050301s)
			{
				var hasMerge = false;
				foreach (var f050301 in gF050301.ToList())
				{
					if (!hasMerge)
					{
						mergeOrders.Add(mergeIdx, new List<string> { f050301.ORD_NO });
						hasMerge = true;
					}
					else
						mergeOrders[mergeIdx].Add(f050301.ORD_NO);
				}
				mergeIdx++;
			}
			return mergeOrders;
		}

		private List<string> GetNoCarPeriodOrds(string dcCode, string gupCode, string custCode, List<F050002> f050002s, List<F050001> gF050001)
		{
			var repF194716 = new F194716Repository(Schemas.CoreSchema, _wmsTransaction);
			var retailCodes = gF050001.AsEnumerable().Select(a => a.RETAIL_CODE).ToList();
			var retailCarPeriods = repF194716.GetRetailCarPeriods(dcCode, gupCode, custCode, retailCodes);
			//允許部分出貨，依出貨時段
			var oOrdNos = from a in gF050001.AsEnumerable()
										where !retailCarPeriods.Select(r => r.RETAIL_CODE).Contains(a.RETAIL_CODE)
										select a.ORD_NO;

			return oOrdNos.ToList();
		}
		#endregion 檢查庫存

		#region 拆併單

		/// <summary>
		/// 產生出貨單LO
		/// </summary>
		/// <param name="f050801BulkInsertList"></param>
		/// <param name="f050802BulkInsertList"></param>
		/// <param name="f05030101s"></param>
		/// <param name="f050301s"></param>
		private void CreateWmsOrdLo(List<F050801> f050801BulkInsertList, List<F050802> f050802BulkInsertList, List<F05030101> f05030101s, List<F050301> f050301s)
		{
			var query = f050801BulkInsertList.Select(f050801 =>
			{
				var f05030101 = f05030101s.Find(x => x.WMS_ORD_NO == f050801.WMS_ORD_NO);
				var f050301 = f050301s.Find(x => x.ORD_NO == f05030101.ORD_NO);

				var ticketItems = (from a in f050802BulkInsertList.Where(x => x.WMS_ORD_NO == f050801.WMS_ORD_NO)
													 select new TicketItem
													 {
														 CUST_CODE = a.CUST_CODE,
														 GUP_CODE = a.GUP_CODE,
														 ITEM_CODE = a.ITEM_CODE,
														 QTY = a.B_DELV_QTY.Value,
														 SERIAL_NO = a.SERIAL_NO
													 }).ToList();
				return new
				{
					F050801 = f050801,
					F050301 = f050301,
					TicketItems = ticketItems
				};
			});

			foreach (var g in query.GroupBy(x => new { x.F050301.TICKET_ID, x.F050301.DC_CODE }))
			{
				var ticketMasters = g.Select(x => new TicketMaster
				{
					TicketNo = x.F050801.WMS_ORD_NO,
					TicketItems = x.TicketItems
				}).ToList();

			}
		}

		private void TakeApartAndMergeOrder(string gupCode, string custCode, string dcCode, List<F050301> f050301s, List<F050302> f050302s, bool isB2b, string sourceType, bool isPreNotEnough = false)
		{
			if (f050301s.Count == 0)
				return;
			//篩選掉F050302.NO_DELV = 0 //設定不為不出貨
			f050302s = f050302s.Where(o => o.NO_DELV == "0").ToList();
			//檢查是否併單
			var f050004Rep = new F050004Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050004 = f050004Rep.GetData(dcCode, gupCode, custCode, f050301s.First().TICKET_ID);
			var itemCodes = f050302s.Select(a => a.ITEM_CODE).Distinct().ToList();

			var f1903s = GetF1903s(gupCode, custCode, itemCodes);

			//配庫
			var allotedStockOrders = AllotStockForOrder(gupCode, custCode, dcCode, f050301s, f050302s);
			//配庫數量不足不需再拆單做後面的流程
			if (allotedStockOrders == null || !allotedStockOrders.Any())
				return;

			//為原箱出貨拆單訂單，原箱出貨品項，一品項一數量一個出貨單
			//var apartOriItemOrderDetails = TakeApartByOriginalItem(allotedStockOrders, f1903s);

			//虛擬商品拆單，虛擬商品部分拆同一張單
			var apartVirItemOrderDetails = TakeApartByVirtualItem(f050301s, allotedStockOrders, f1903s);

			//有拆單的訂單編號
			List<string> apartOrdNos = null;
			var isMerge = false;
			if (f050004.MERGE_ORDER == "1")
			{
				isMerge = true;
				//20150709改成拆單仍要併單
				////有拆單的訂單編號，有原箱出貨訂單，若品項超過兩個一定有拆單
				//apartOrdNos = (from a in f050302s
				//							 join b in apartOriItemOrderDetails on a.ORD_NO equals b.F050302.ORD_NO
				//							 group a by a.ORD_NO into g
				//							 select new { OrdNo = g.Key, Cnt = g.Count() })
				//							.Where(a => a.Cnt >= 2).Select(a => a.OrdNo).ToList();

				////有虛擬商品訂單就不可併單，所以將這些訂單算在有拆單的訂單中
				//apartOrdNos.AddRange(apartVirItemOrderDetails.Keys.ToList());
				//apartOrdNos = apartOrdNos.Distinct().ToList();
			}

			//為非原箱出貨及非虛擬商品拆單的拆單訂單
			var apartOrders = new Dictionary<int, List<string>>();
			var apartOrderDetails = new Dictionary<int, List<AllotedStockOrder>>();

			//拆併單
			var splitPickType = f050004.SPLIT_PICK_TYPE;
			SplitPickType enumSplitPickType;
			if (!Enum.TryParse(splitPickType, out enumSplitPickType))
				enumSplitPickType = SplitPickType.Tmpr;

			switch (enumSplitPickType)
			{
				case SplitPickType.Tmpr: //依溫層拆併單
				case SplitPickType.Area: //依儲區拆併單(儲區還是用溫層進行訂單拆併單產生出貨單，再由出貨單依儲區拆揀貨單)
					TakeApartByTemperature(f050301s, f050302s, isB2b, allotedStockOrders, f1903s, isMerge, apartOrdNos, ref apartOrders, ref apartOrderDetails);
					break;
				case SplitPickType.TmprAndFloor: //依溫層+樓層拆併單
					TakeApartByTemperatureFloor(f050301s, f050302s, isB2b, allotedStockOrders, f1903s, isMerge, apartOrdNos, ref apartOrders, ref apartOrderDetails);
					break;
					//case SplitPickType.Area: //依儲區拆併單
					//    TakeApartByArea(f050301s, f050302s, isB2b, allotedStockOrders, f1903s, isMerge, apartOrdNos, ref apartOrders, ref apartOrderDetails);
					//    break;
			}


			//每張揀貨單分配出貨單上限數
			var f0003Rep = new F0003Repository(Schemas.CoreSchema, _wmsTransaction);
			var pickWmsLimit = 0;
			int.TryParse(f0003Rep.Find(a => a.AP_NAME == "PickWmsLimit" && a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode, false).SYS_PATH, out pickWmsLimit);
			if (pickWmsLimit == 0)
				pickWmsLimit = 12;

			#region 產生出貨單並設定揀貨單與出貨單關係
			//揀貨單號對應的出貨單，Key為揀貨單號，Value為對應的出貨單號
			var pickOrdRelatedWmsOrds = new List<PickOrdRelatedWmsOrd>();

			var pickTime = GetPickTime(sourceType);
			var f050801Dic = new Dictionary<F050801, List<F050802>>();


			//紀錄已產生過出貨單之訂單清單
			var newWmsOrdNosStack = _sharedService.GetNewOrdStackCodes("O",  apartVirItemOrderDetails.Count + apartOrders.Count);
			var hasGenerateWmsOrderNoList = new List<string>();
			var f05030101s = new List<F05030101>();
			var f050801BulkInsertList = new List<F050801>();
			var f050802BulkInsertList = new List<F050802>();
			var f05030202BulkInsertList = new List<F05030202>();
			////原箱出貨部分
			//foreach (var apartOriItemOrderDetail in apartOriItemOrderDetails)
			//{
			//	List<F050802> f050802s;
			//	var f050801 = CreateWmsOrdsAndSetPickOrds(gupCode, custCode, dcCode, newWmsOrdNosStack,
			//			new List<string> { apartOriItemOrderDetail.F050302.ORD_NO }, new List<AllotedStockOrder> { apartOriItemOrderDetail },
			//			f050301s, f1903s, pickTime, pickWmsLimit, ref pickOrdRelatedWmsOrds,
			//			ref f05030101s, out f050802s, enumSplitPickType, ref hasGenerateWmsOrderNoList, ref f05030202BulkInsertList, false, true);
			//	f050801Dic.Add(f050801, f050802s);
			//	f050801BulkInsertList.Add(f050801);
			//	f050802BulkInsertList.AddRange(f050802s);
			//}

			apartOrders = apartOrders.OrderByDescending(n => n.Value.Count).ToDictionary(a => a.Key, a => a.Value);
			//非虛擬商品拆單的拆單部分
			foreach (var apartOrder in apartOrders)
			{
				List<F050802> f050802s;
				var f050801 = CreateWmsOrdsAndSetPickOrds(gupCode, custCode, dcCode, newWmsOrdNosStack, apartOrder.Value,
						apartOrderDetails[apartOrder.Key], f050301s, f1903s, pickTime, pickWmsLimit, ref pickOrdRelatedWmsOrds, ref f05030101s, out f050802s, enumSplitPickType, ref hasGenerateWmsOrderNoList, ref f05030202BulkInsertList);
				f050801Dic.Add(f050801, f050802s);
				f050801BulkInsertList.Add(f050801);
				f050802BulkInsertList.AddRange(f050802s);
			}

			//虛擬商品拆單部分
			foreach (var apartVirItemOrderDetail in apartVirItemOrderDetails)
			{
				List<F050802> f050802s;
				var f050801 = CreateWmsOrdsAndSetPickOrds(gupCode, custCode, dcCode, newWmsOrdNosStack,
						new List<string> { apartVirItemOrderDetail.Key }, apartVirItemOrderDetail.Value, f050301s, f1903s, pickTime,
						pickWmsLimit, ref pickOrdRelatedWmsOrds, ref f05030101s, out f050802s, enumSplitPickType,
						ref hasGenerateWmsOrderNoList, ref f05030202BulkInsertList, true);
				f050801Dic.Add(f050801, f050802s);
				f050801BulkInsertList.Add(f050801);
				f050802BulkInsertList.AddRange(f050802s);
			}

			// 大量處理 581 HelloLetter 拆單時，只會
			SetF050801FirstHelloLetter(f050801BulkInsertList, f05030101s);

			// 大量新增 581, 582, 531
			_f050801Rep.BulkInsert(f050801BulkInsertList);
			_f050802Rep.BulkInsert(f050802BulkInsertList);
			_f05030101Rep.BulkInsert(f05030101s);
			_f05030202Repo.BulkInsert(f05030202BulkInsertList, "ID");

			// 若為專車的訂單，則會將手動建立的派車明細的物流單號，改為出貨單號
			UpdateF700102WmsNoBySpecialBus(f050301s, f05030101s);

			// 大量建立 LO
			CreateWmsOrdLo(f050801BulkInsertList, f050802BulkInsertList, f05030101s, f050301s);

			#endregion 產生出貨單並設定揀貨單與出貨單關係

			List<F051201> f051201BulkInsertList = new List<F051201>();
			List<F051202> f051202BulkInsertList = new List<F051202>();
			List<F1511> f1511BulkInsertList = new List<F1511>();
			var f05120101BulkInsertList = new List<F05120101>();
			//產生揀貨單
			foreach (var pickOrdRelatedWmsOrd in pickOrdRelatedWmsOrds)
			{
				var pickWmsOrderAllotedStockOrders = pickOrdRelatedWmsOrd.WmsAllotedStockOrders;

				List<F051202> f051202s;
				List<F1511> f1511s;
				F05120101 f05120101;
				var f051201 = CreatePickOrder(gupCode, custCode, dcCode, pickOrdRelatedWmsOrd.PickOrdNo, pickWmsOrderAllotedStockOrders, isB2b, pickTime, enumSplitPickType, pickOrdRelatedWmsOrd.WarehouseId, pickOrdRelatedWmsOrd.Area, out f051202s, out f1511s, out f05120101);
				f051201BulkInsertList.Add(f051201);
				f051202BulkInsertList.AddRange(f051202s);
				f1511BulkInsertList.AddRange(f1511s);
				if (f05120101 != null)
					f05120101BulkInsertList.Add(f05120101);
			}
			// 大量新增 51201, 51202, 1511 揀貨資料
			_f051201Rep = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			_f05120101Repo = new F05120101Repository(Schemas.CoreSchema, _wmsTransaction);
			_f1511Rep = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			_f051202Rep = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);

			_f051201Rep.BulkInsert(f051201BulkInsertList);
			_f051202Rep.BulkInsert(f051202BulkInsertList);
			_f1511Rep.BulkInsert(f1511BulkInsertList);
			_f05120101Repo.BulkInsert(f05120101BulkInsertList);

			var ordNos = allotedStockOrders.Select(a => a.F050302.ORD_NO).Distinct().ToList();
			//更改F050301的狀態為已配庫
			var f050301Rep = new F050301Repository(Schemas.CoreSchema, _wmsTransaction);
			f050301Rep.UpdateStatus(gupCode, custCode, dcCode, ordNos, "1");
			_finishAllocOrderCnt += ordNos.Count;
			//Robin 20180507 修改可部分出貨，同時修改非B2B時才須派車
			//if (!isB2b) //*****暫時註解，待討論******
			//{
			//派車
			DistributeCar(gupCode, custCode, dcCode, f050801Dic, f050301s, f05030101s, isB2b);
			//}

			//產生批次資料表
			CreatePickTimeTable(gupCode, custCode, dcCode, pickTime, f050801Dic);
		}

		private void SetF050801FirstHelloLetter(List<F050801> f050801BulkInsertList, List<F05030101> f05030101s)
		{
			// 紀錄已經使用過的拆單訂單編號
			var takeApartOrdNoHashSet = new HashSet<string>();

			foreach (var f581 in f050801BulkInsertList.Where(x => x.HELLO_LETTER == "1").OrderBy(x => x.WMS_ORD_NO))
			{
				var associateOrdNos = f05030101s.Where(f5311 => f5311.WMS_ORD_NO == f581.WMS_ORD_NO)
																				.Select(f5311 => f5311.ORD_NO)
																				.ToList();

				// 已使用過訂單編號的話，表示已經是拆單的第二張出貨單，則不能列印HELLO_LETTER。
				if (associateOrdNos.Any(ordNo => takeApartOrdNoHashSet.Contains(ordNo)))
				{
					f581.HELLO_LETTER = "0";
				}

				takeApartOrdNoHashSet.UnionWith(associateOrdNos);
			}
		}

		/// <summary>
		/// 若為專車的訂單，則會將手動建立的派車明細的物流單號，改為出貨單號
		/// </summary>
		/// <param name="f050301s"></param>
		/// <param name="f05030101s"></param>
		private void UpdateF700102WmsNoBySpecialBus(List<F050301> f050301s, List<F05030101> f05030101s)
		{
			var query = from f050301 in f050301s
									join f05030101 in f05030101s on f050301.ORD_NO equals f05030101.ORD_NO
									where f050301.SPECIAL_BUS == "1"
									select new
									{
										WmsNo = (f050301.SOURCE_TYPE == "08") ? f050301.SOURCE_NO : f050301.ORD_NO,
										F05030101 = f05030101
									};

			if (!query.Any())
				return;

			// 專車時須將配車單的物流單號，改為多張出貨單的派車明細
			var f700102Rep = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
			var addF700102List = new List<F700102>();

			foreach (var g in query.GroupBy(x => x.WmsNo))
			{
				var f700102 = _specialCars.SingleOrDefault(x => x.WMS_NO == g.Key);
				if (f700102 == null)
					continue;

				// 刪除原本 WmsNo 是來源單據或訂單單號的派車明細
				f700102Rep.Delete(x => x.DC_CODE == f700102.DC_CODE
														&& x.GUP_CODE == f700102.GUP_CODE
														&& x.CUST_CODE == f700102.CUST_CODE
														&& x.DISTR_CAR_NO == f700102.DISTR_CAR_NO);

				// 在重新將多張出貨單號設定回 WmsNo 欄位
				var addF700102sQuery = g.OrderBy(x => x.F05030101.WMS_ORD_NO)
																.Select((x, index) =>
																{
																	var addF700102 = AutoMapper.Mapper.DynamicMap<F700102>(f700102);
																	addF700102.DISTR_CAR_SEQ = (short)(index + 1);
																	addF700102.ORD_TYPE = "O";
																	addF700102.WMS_NO = x.F05030101.WMS_ORD_NO;
																	return addF700102;
																});

				addF700102List.AddRange(addF700102sQuery);
			}

			f700102Rep.BulkInsert(addF700102List);
		}

		//private List<AllotedStockOrder> TakeApartByOriginalItem(List<AllotedStockOrder> allotedStockOrders, List<F1903> f1903s)
		//{
		//	var apartOriItemOrderDetails = new List<AllotedStockOrder>();
		//	var tmpDetails = (from a in allotedStockOrders
		//										join b in f1903s on a.F050302.ITEM_CODE equals b.ITEM_CODE
		//										where b.ALLOWORDITEM == "1"
		//										select a).ToList();

		//	foreach (var tmpDetail in tmpDetails)
		//	{
		//		for (var i = 0; i < tmpDetail.F050302.ORD_QTY; i++)
		//		{
		//			AutoMapper.Mapper.CreateMap<AllotedStockOrder, AllotedStockOrder>();
		//			AutoMapper.Mapper.CreateMap<F050302, F050302>();
		//			var allotedStockOrder = AutoMapper.Mapper.Map<AllotedStockOrder>(tmpDetail);
		//			allotedStockOrder.F050302.ORD_QTY = 1;
		//			apartOriItemOrderDetails.Add(allotedStockOrder);
		//		}
		//	}
		//	return apartOriItemOrderDetails;
		//}

		private Dictionary<string, List<AllotedStockOrder>> TakeApartByVirtualItem(List<F050301> f050301s, List<AllotedStockOrder> allotedStockOrders, List<F1903> f1903s)
		{
			var apartVirItemOrderDetails = new Dictionary<string, List<AllotedStockOrder>>();
			var gApartVirItemOrderDetails = (from a in allotedStockOrders
																			 join c in f1903s on a.F050302.ITEM_CODE equals c.ITEM_CODE
																			 where c.ALLOWORDITEM != "1" && !string.IsNullOrEmpty(c.VIRTUAL_TYPE)
																			 select a).GroupBy(a => a.F050302.ORD_NO);
			foreach (var gApartVirItemOrderDetail in gApartVirItemOrderDetails)
			{
				apartVirItemOrderDetails.Add(gApartVirItemOrderDetail.Key, gApartVirItemOrderDetail.ToList());
			}
			return apartVirItemOrderDetails;
		}

		private void TakeApartByTemperature(List<F050301> f050301s, List<F050302> f050302s, bool isB2b,
				List<AllotedStockOrder> allotedStockOrders, List<F1903> f1903s,
				bool isMerge, List<string> apartOrdNos,
				ref Dictionary<int, List<string>> apartOrders, ref Dictionary<int, List<AllotedStockOrder>> apartOrderDetails)
		{
			var aprtIdx = 0;
			var TmprMergeOrderIndexs = new Dictionary<KeyValuePair<string, List<string>>, int>();
			var gF050302s = (from a in allotedStockOrders
											 join c in f1903s on a.F050302.ITEM_CODE equals c.ITEM_CODE
											 where  string.IsNullOrEmpty(c.VIRTUAL_TYPE)
											 select new { a, c.TMPR_TYPE }).GroupBy(n => new { n.a.F050302.ORD_NO, n.TMPR_TYPE });
			Dictionary<int, List<string>> mergeOrders = null;
			if (isMerge)
			{
				//20150709改成拆單仍要併單
				////非原箱出貨有拆單的訂單編號
				////gF050302s的每個Key代表拆成一張單，若Key中有多筆相同的訂單編號，代表這張訂單有拆單
				//var apartOrdNos2 = (from a in gF050302s
				//										group a by a.Key.ORD_NO into g
				//										select new { OrdNo = g.Key, Cnt = g.Count() })
				//										.Where(a => a.Cnt >= 2).Select(a => a.OrdNo).ToList();
				////合併原箱出貨及非原箱出貨有拆單的訂單編號
				//apartOrdNos.AddRange(apartOrdNos2);
				//apartOrdNos = apartOrdNos.Distinct().ToList();
				//找出無拆單且需併單的訂單號
				mergeOrders = MergeOrder(f050301s, f050302s, isB2b, apartOrdNos);
			}

			foreach (var gF050302 in gF050302s)
			{
				var hasMerge = false;
				var apartF050302s = gF050302.ToList().Select(a => a.a).ToList();
				if (isMerge)
				{
					var ms = (from a in mergeOrders.Values
										from b in a
										where b == gF050302.Key.ORD_NO
										select a).SingleOrDefault();
					if (ms != null)
					{
						var idxKey = new KeyValuePair<string, List<string>>(gF050302.Key.TMPR_TYPE, ms);
						var idx = aprtIdx;
						if (TmprMergeOrderIndexs.Keys.Contains(idxKey))
						{
							//前面已有同併單的資料，加入至同併單的資料
							idx = TmprMergeOrderIndexs[idxKey];
							if (!apartOrders[idx].Contains(gF050302.Key.ORD_NO))
								apartOrders[idx].Add(gF050302.Key.ORD_NO);
							apartOrderDetails[idx].AddRange(apartF050302s);
						}
						else
						{
							//前面尚未有同併單的資料，紀錄新的併單的資料
							TmprMergeOrderIndexs.Add(idxKey, aprtIdx);
							apartOrders.Add(aprtIdx, new List<string> { gF050302.Key.ORD_NO });
							apartOrderDetails.Add(aprtIdx, apartF050302s);
							aprtIdx++;
						}
						hasMerge = true;
					}
				}

				if (!hasMerge)
				{
					apartOrders.Add(aprtIdx, new List<string> { gF050302.Key.ORD_NO });
					apartOrderDetails.Add(aprtIdx, apartF050302s);
					aprtIdx++;
				}
			}
		}

		private void TakeApartByTemperatureFloor(List<F050301> f050301s, List<F050302> f050302s, bool isB2b,
				List<AllotedStockOrder> allotedStockOrders, List<F1903> f1903s,
				bool isMerge, List<string> apartOrdNos,
				ref Dictionary<int, List<string>> apartOrders, ref Dictionary<int, List<AllotedStockOrder>> apartOrderDetails)
		{
			var aprtIdx = 0;
			var TmprMergeOrderIndexs = new Dictionary<KeyValuePair<string, List<string>>, int>();
			var gF050302s = (from a in allotedStockOrders
											 join c in f1903s on a.F050302.ITEM_CODE equals c.ITEM_CODE
											 where  string.IsNullOrEmpty(c.VIRTUAL_TYPE)
											 select new { a, c.TMPR_TYPE }).GroupBy(n => new { n.a.F050302.ORD_NO, n.TMPR_TYPE, n.a.Floor });

			Dictionary<int, List<string>> mergeOrders = null;
			if (isMerge)
			{
				//20150709改成拆單仍要併單
				////非原箱出貨有拆單的訂單編號
				////gF050302s的每個Key代表拆成一張單，若Key中有多筆相同的訂單編號，代表這張訂單有拆單
				//var apartOrdNos2 = (from a in gF050302s
				//										group a by a.Key.ORD_NO into g
				//										select new { OrdNo = g.Key, Cnt = g.Count() })
				//										.Where(a => a.Cnt >= 2).Select(a => a.OrdNo).ToList();
				////合併原箱出貨及非原箱出貨有拆單的訂單編號
				//apartOrdNos.AddRange(apartOrdNos2);
				//apartOrdNos = apartOrdNos.Distinct().ToList();
				//找出無拆單且需併單的訂單號
				mergeOrders = MergeOrder(f050301s, f050302s, isB2b, apartOrdNos);
			}

			foreach (var gF050302 in gF050302s)
			{
				var hasMerge = false;
				var apartF050302s = gF050302.ToList().Select(a => a.a).ToList();
				if (isMerge)
				{
					var ms = (from a in mergeOrders.Values
										from b in a
										where b == gF050302.Key.ORD_NO
										select a).SingleOrDefault();
					if (ms != null)
					{
						var idxKey = new KeyValuePair<string, List<string>>(gF050302.Key.TMPR_TYPE + "|" + gF050302.Key.Floor, ms);
						var idx = aprtIdx;
						if (TmprMergeOrderIndexs.Keys.Contains(idxKey))
						{
							//前面已有同併單的資料，加入至同併單的資料
							idx = TmprMergeOrderIndexs[idxKey];
							if (!apartOrders[idx].Contains(gF050302.Key.ORD_NO))
								apartOrders[idx].Add(gF050302.Key.ORD_NO);
							apartOrderDetails[idx].AddRange(apartF050302s);
						}
						else
						{
							//前面尚未有同併單的資料，紀錄新的併單的資料
							TmprMergeOrderIndexs.Add(idxKey, aprtIdx);
							apartOrders.Add(aprtIdx, new List<string> { gF050302.Key.ORD_NO });
							apartOrderDetails.Add(aprtIdx, apartF050302s);
							aprtIdx++;
						}
						hasMerge = true;
					}
				}

				if (!hasMerge)
				{
					apartOrders.Add(aprtIdx, new List<string> { gF050302.Key.ORD_NO });
					apartOrderDetails.Add(aprtIdx, apartF050302s);
					aprtIdx++;
				}
			}
		}

		private void TakeApartByArea(List<F050301> f050301s, List<F050302> f050302s, bool isB2b,
				List<AllotedStockOrder> allotedStockOrders, List<F1903> f1903s,
				bool isMerge, List<string> apartOrdNos,
				ref Dictionary<int, List<string>> apartOrders, ref Dictionary<int, List<AllotedStockOrder>> apartOrderDetails)
		{
			var aprtIdx = 0;
			var TmprMergeOrderIndexs = new Dictionary<KeyValuePair<string, List<string>>, int>();
			var gF050302s = (from a in allotedStockOrders
											 join c in f1903s on a.F050302.ITEM_CODE equals c.ITEM_CODE
											 where  string.IsNullOrEmpty(c.VIRTUAL_TYPE)
											 select new { a }).GroupBy(n => new { n.a.F050302.ORD_NO, n.a.WarehouseId, n.a.AreaCode });

			Dictionary<int, List<string>> mergeOrders = null;
			if (isMerge)
			{
				//20150709改成拆單仍要併單
				////非原箱出貨有拆單的訂單編號
				////gF050302s的每個Key代表拆成一張單，若Key中有多筆相同的訂單編號，代表這張訂單有拆單
				//var apartOrdNos2 = (from a in gF050302s
				//										group a by a.Key.ORD_NO into g
				//										select new { OrdNo = g.Key, Cnt = g.Count() })
				//										.Where(a => a.Cnt >= 2).Select(a => a.OrdNo).ToList();
				////合併原箱出貨及非原箱出貨有拆單的訂單編號
				//apartOrdNos.AddRange(apartOrdNos2);
				//apartOrdNos = apartOrdNos.Distinct().ToList();
				//找出無拆單且需併單的訂單號
				mergeOrders = MergeOrder(f050301s, f050302s, isB2b, apartOrdNos);
			}

			foreach (var gF050302 in gF050302s)
			{
				var hasMerge = false;
				var apartF050302s = gF050302.ToList().Select(a => a.a).ToList();
				if (isMerge)
				{
					var ms = (from a in mergeOrders.Values
										from b in a
										where b == gF050302.Key.ORD_NO
										select a).SingleOrDefault();
					if (ms != null)
					{
						var idxKey = new KeyValuePair<string, List<string>>(gF050302.Key.WarehouseId + "|" + gF050302.Key.AreaCode, ms);
						var idx = aprtIdx;
						if (TmprMergeOrderIndexs.Keys.Contains(idxKey))
						{
							//前面已有同併單的資料，加入至同併單的資料
							idx = TmprMergeOrderIndexs[idxKey];
							if (!apartOrders[idx].Contains(gF050302.Key.ORD_NO))
								apartOrders[idx].Add(gF050302.Key.ORD_NO);
							apartOrderDetails[idx].AddRange(apartF050302s);
						}
						else
						{
							//前面尚未有同併單的資料，紀錄新的併單的資料
							TmprMergeOrderIndexs.Add(idxKey, aprtIdx);
							apartOrders.Add(aprtIdx, new List<string> { gF050302.Key.ORD_NO });
							apartOrderDetails.Add(aprtIdx, apartF050302s);
							aprtIdx++;
						}
						hasMerge = true;
					}
				}

				if (!hasMerge)
				{
					apartOrders.Add(aprtIdx, new List<string> { gF050302.Key.ORD_NO });
					apartOrderDetails.Add(aprtIdx, apartF050302s);
					aprtIdx++;
				}
			}
		}

		/// <summary>
		/// 產生出貨單並設定揀貨單與出貨單關係
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="ordNos"></param>
		/// <param name="allotedStockOrders"></param>
		/// <param name="f050301s"></param>
		/// <param name="f1903s"></param>
		/// <param name="pickTime"></param>
		/// <param name="pickWmsLimit"></param>
		/// <param name="wmsOrdCount"></param>
		/// <param name="pickOrdNo"></param>
		/// <param name="pickOrdRelatedWmsOrds"></param>
		private F050801 CreateWmsOrdsAndSetPickOrds(string gupCode, string custCode, string dcCode, Stack<string> newWmsOrdNosStack, List<string> ordNos, List<AllotedStockOrder> allotedStockOrders,
				List<F050301> f050301s, List<F1903> f1903s, string pickTime, int pickWmsLimit,
				ref List<PickOrdRelatedWmsOrd> pickOrdRelatedWmsOrds, ref List<F05030101> f05030101s,
				out List<F050802> f050802s, SplitPickType enumSplitPickType, ref List<string> hasGenerateWmsOrderNoList, ref List<F05030202> f05030202s, bool isVirtual = false)
		{
			//產生出貨單
			var f050801 = CreateWmsOrder(gupCode, custCode, dcCode, newWmsOrdNosStack, ordNos, allotedStockOrders, f050301s, f1903s, pickTime, out f050802s, ref f05030101s, isVirtual, ref hasGenerateWmsOrderNoList, ref f05030202s);
			var groupAllotedStockOrders = allotedStockOrders.GroupBy(x => new { x.Floor, x.TmprType, x.WarehouseId, x.AreaCode });


			// 再依出貨明細
			foreach (var groupAllotedStockOrder in groupAllotedStockOrders)
			{
				var floor = string.Empty;
				var pickOrdNo = string.Empty;
				var area = string.Empty;
				var warehouseId = string.Empty;
				switch (enumSplitPickType)
				{
					case SplitPickType.Tmpr://溫層
						var pickOrdNos1 = pickOrdRelatedWmsOrds.Select(g => new { PickOrdNo = g.PickOrdNo,TmprType = g.TmprType, Count = g.WmsAllotedStockOrders.Keys.Count(), IsVirtualItem = g.IsVirtualItem }).ToList();
						pickOrdNo = pickOrdNos1.Where(a => a.TmprType == groupAllotedStockOrder.Key.TmprType && a.Count < pickWmsLimit && a.IsVirtualItem == isVirtual).Select(a => a.PickOrdNo).FirstOrDefault();
						break;
					case SplitPickType.TmprAndFloor: //溫層+樓層
						var pickOrdNos2 = pickOrdRelatedWmsOrds.Select(g => new { PickOrdNo = g.PickOrdNo, Floor = g.Floor, TmprType = g.TmprType, Count = g.WmsAllotedStockOrders.Keys.Count(), IsVirtualItem = g.IsVirtualItem }).ToList();
						pickOrdNo = pickOrdNos2.Where(a => a.Floor == groupAllotedStockOrder.Key.Floor && a.TmprType == groupAllotedStockOrder.Key.TmprType && a.Count < pickWmsLimit && a.IsVirtualItem == isVirtual).Select(a => a.PickOrdNo).FirstOrDefault();
						break;
					case SplitPickType.Area: //儲區
						var pickOrdNos3 = pickOrdRelatedWmsOrds.Select(g => new { PickOrdNo = g.PickOrdNo, Floor = g.Floor, TmprType = g.TmprType, WarehouseId = g.WarehouseId, Area = g.Area, Count = g.WmsAllotedStockOrders.Keys.Count(), IsVirtualItem = g.IsVirtualItem }).ToList();
						pickOrdNo = pickOrdNos3.Where(a => a.WarehouseId == groupAllotedStockOrder.Key.WarehouseId && a.Area == groupAllotedStockOrder.Key.AreaCode && a.Count < pickWmsLimit && a.IsVirtualItem == isVirtual).Select(a => a.PickOrdNo).FirstOrDefault();
						break;
				}
				if (string.IsNullOrEmpty(pickOrdNo))
				{
					pickOrdNo = _sharedService.GetNewOrdCode("P");
				}
				var pickOrdRelatedWmsOrd = pickOrdRelatedWmsOrds.Where(a => a.PickOrdNo == pickOrdNo).FirstOrDefault();
				if (pickOrdRelatedWmsOrd != null)
				{
					var wmsAllotedStockOrders = pickOrdRelatedWmsOrd.WmsAllotedStockOrders.FirstOrDefault(x => x.Key == f050801.WMS_ORD_NO);
					if(wmsAllotedStockOrders.Equals(default(KeyValuePair<string,List< AllotedStockOrder>>)))
					{
						pickOrdRelatedWmsOrd.WmsAllotedStockOrders.Add(f050801.WMS_ORD_NO, groupAllotedStockOrder.ToList());
					}
					else
					{
						wmsAllotedStockOrders.Value.AddRange(groupAllotedStockOrder.ToList());
					}
				}
					
				else
				{
					var wmsAllotedStockOrders = new Dictionary<string, List<AllotedStockOrder>>();
					wmsAllotedStockOrders.Add(f050801.WMS_ORD_NO, groupAllotedStockOrder.ToList());
					pickOrdRelatedWmsOrds.Add(new PickOrdRelatedWmsOrd { PickOrdNo = pickOrdNo, Floor = groupAllotedStockOrder.Key.Floor, WarehouseId = groupAllotedStockOrder.Key.WarehouseId, Area = groupAllotedStockOrder.Key.AreaCode, TmprType = groupAllotedStockOrder.Key.TmprType, IsVirtualItem = isVirtual, WmsAllotedStockOrders = wmsAllotedStockOrders });
				}
				// 拆揀貨單方式為非儲區則為一P一O 將出貨單設為此揀貨單
				if (enumSplitPickType != SplitPickType.Area)
					f050801.PICK_ORD_NO = pickOrdNo;
			}
			
			return f050801;
		}

		/// <summary>
		/// 建立出貨單
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="ordNos">建立在同一出貨單的訂單</param>
		/// <param name="allotedStockOrders">建立在同一出貨單的配庫明細</param>
		private F050801 CreateWmsOrder(string gupCode, string custCode, string dcCode, Stack<string> newWmsOrdNosStack, List<string> ordNos, List<AllotedStockOrder> allotedStockOrders, List<F050301> f050301s, List<F1903> f1903s, string pickTime, out List<F050802> f050802s, ref List<F05030101> f05030101s, bool isVirtual, ref List<string> hasGenerateWmsOrderNoList, ref List<F05030202> f05030202s)
		{
			//出貨單號
			var wmsOrdNo = newWmsOrdNosStack.Pop();
			var f050301 = f050301s.Where(a => a.ORD_NO == ordNos.First()).Single();
			var allotedStockOrderFirst = allotedStockOrders.First();
			//判斷明細是否有易碎品
			var hasFragile = (from a in allotedStockOrders
												join b in f1903s on a.F050302.ITEM_CODE equals b.ITEM_CODE
												where b.FRAGILE == "1"
												select a).Any();
			//判斷明細是否有保證書
			var hasGuarantee = (from a in allotedStockOrders
													join b in f1903s on a.F050302.ITEM_CODE equals b.ITEM_CODE
													where b.LG == "1"
													select a).Any();
            //判斷是否需派車(里程碑要裝車且非自取且非內部交易同DC且非虛擬商品，且不為互賣(互賣SP_DELV為'02')) 且非超取 且貨主主檔設為裝車
            //因已無f050301.SOURCE_TYPE == "09"此類型，因此直接指定false
            var isLoading = false;

			//是否列印箱明細，來源單據為銷毀單的才不需要列印
			var isPrintBox = (f050301.SOURCE_TYPE != "08");
			//是否列印送貨單，為B2B且不為銷毀單
			var f1909 = GetF1909(gupCode, custCode);
			var isPrintDelv = (f050301.ORD_TYPE == "0" && f050301.SOURCE_TYPE != "08" && f1909.ISPRINTDELV == "1");
			//是否列印託運單，不為銷毀單且不為自取 且不為專車，且需派車
			var isPrintPass = IsPrintPass(f050301) && isLoading;
			//加總SA數量
			var saQty = f050301s.Where(a => ordNos.Contains(a.ORD_NO)).Sum(a => a.SA_QTY);
			var saCheckQty = f050301s.Where(a => ordNos.Contains(a.ORD_NO)).Sum(a => a.SA_CHECK_QTY);
			//加總代收金額
			var collectAmt = f050301s.Where(a => ordNos.Contains(a.ORD_NO) && a.COLLECT_AMT != null).Sum(a => a.COLLECT_AMT);
			//如果此訂單已經產生過出貨單 SAQTY就設為0,代收金額設為0
			if (hasGenerateWmsOrderNoList.Any(o => o == f050301.ORD_NO))
			{
				saQty = 0;
				saCheckQty = 0;
				collectAmt = 0;
			}
			else
				hasGenerateWmsOrderNoList.Add(f050301.ORD_NO);
			//溫層
			var tmprType = f1903s.Where(a => a.ITEM_CODE == allotedStockOrderFirst.F050302.ITEM_CODE).Select(a => a.TMPR_TYPE).Single();

			//20210512 調整原箱與一般商品需在同一張出貨單上 (只要出貨單上有原箱商品，設定為含有原箱商品出貨單)
			var itemCodes = allotedStockOrders.Select(x => x.F050302.ITEM_CODE).Distinct().ToList();
			var allowordItem = f1903s.Any(x => itemCodes.Contains(x.ITEM_CODE) &&  x.ALLOWORDITEM == "1");


			var f050801 = new F050801
			{
				WMS_ORD_NO = wmsOrdNo,
				ALL_ID = f050301.ALL_ID,
				ALLOWORDITEM = (allowordItem) ? "1" : "0",
				AGE = f050301.AGE,
				ARRIVAL_DATE = f050301.ARRIVAL_DATE,
				CAN_FAST = f050301.CAN_FAST,
				CUST_CODE = custCode,
				CUST_NAME = f050301.CUST_NAME,
				DC_CODE = dcCode,
				DELV_DATE = DateTime.Today,
				FRAGILE_LABEL = (hasFragile) ? "1" : "0",
				GENDER = f050301.GENDER,
				GUARANTEE = (hasGuarantee) ? "1" : "0",
				GUP_CODE = gupCode,
				HELLO_LETTER = f050301.HELLO_LETTER,
				NO_AUDIT = (isLoading) ? "0" : "1",
				NO_LOADING = (isLoading) ? "0" : "1",
				ORD_PROP = f050301.ORD_PROP,
				ORD_TYPE = f050301.ORD_TYPE,
				PICK_TIME = pickTime,
				PRINT_BOX = (isPrintBox) ? "1" : "0",
				PRINT_DELV = (isPrintDelv) ? "1" : "0",
				PRINT_FLAG = "0",
				PRINT_PASS = (isPrintPass) ? "1" : "0",
				RETAIL_CODE = f050301.RETAIL_CODE,
				SA = f050301.SA,
				SA_QTY = (short)(saQty ?? 0),
				SA_CHECK_QTY = (short)saCheckQty,
				SELF_TAKE = f050301.SELF_TAKE,
				SPECIAL_BUS = f050301.SPECIAL_BUS,
				STATUS = 0,
				SOURCE_TYPE = f050301.SOURCE_TYPE,
				TMPR_TYPE = tmprType,
				VIRTUAL_ITEM = (isVirtual) ? "1" : "0",
				ZIP_CODE = f050301.ZIP_CODE,
				COLLECT_AMT = (isVirtual) ? 0 : collectAmt,
				DELV_PERIOD = f050301.DELV_PERIOD,
				CVS_TAKE = f050301.CVS_TAKE,
				CHECK_CODE = f050301.CHECK_CODE,
				SELFTAKE_CHECKCODE = "0",
				A_ARRIVAL_DATE = DateTime.Today.AddDays(1),
				ROUND_PIECE = f050301.ROUND_PIECE
			};


			//新增出貨單明細
			f050802s = new List<F050802>();
			var gAllotedStockOrders = allotedStockOrders.GroupBy(a => new { a.F050302.ITEM_CODE, a.SerialNo });
			var wmsOrdSeq = 1;
			foreach (var gAllotedStockOrder in gAllotedStockOrders)
			{
				var sumItemQty = (gAllotedStockOrder.Key.SerialNo != "0") ? 1 : gAllotedStockOrder.Sum(a => a.F050302.ORD_QTY);
				var f050802 = new F050802
				{
					A_DELV_QTY = 0,
					B_DELV_QTY = sumItemQty,
					CUST_CODE = custCode,
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					ITEM_CODE = gAllotedStockOrder.Key.ITEM_CODE,
					ORD_QTY = sumItemQty,
					SERIAL_NO = (gAllotedStockOrder.Key.SerialNo == "0") ? "" : gAllotedStockOrder.Key.SerialNo,
					WMS_ORD_NO = wmsOrdNo,
					WMS_ORD_SEQ = wmsOrdSeq.ToString("0000")
				};
				f050802s.Add(f050802);
				CreateF05030202(gAllotedStockOrder.Select(x => x.F050302).ToList(), f050802, ref f05030202s);
				wmsOrdSeq++;
			}
			//總材積
			f050801.VOLUMN = GetTotalItemsVolume(dcCode, gupCode,custCode, f050802s);
			//總重量
			f050801.WEIGHT = GetTotalItemsWeight(gupCode,custCode, f050802s);

			foreach (var ordNo in ordNos)
			{
				//新增出貨單與訂單關聯的資訊
				var f05030101 = new F05030101
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					ORD_NO = ordNo,
					WMS_ORD_NO = wmsOrdNo
				};
				f05030101s.Add(f05030101);
			}
			return f050801;
		}
		/// <summary>
		/// 是否列印託運單，不為銷毀單且不為自取 且不為專車，且若為內部交易單，不為同DC
		/// </summary>
		/// <param name="f050301"></param>
		/// <returns></returns>
		private bool IsPrintPass(F050301 f050301)
		{
			var isPrintPass = (f050301.SELF_TAKE == "0" && f050301.SOURCE_TYPE != "08" && f050301.SPECIAL_BUS == "0");
			return isPrintPass;
		}

		private F051201 CreatePickOrder(string gupCode, string custCode, string dcCode, string pickOrdNo, Dictionary<string, List<AllotedStockOrder>> pickWmsOrderAllotedStockOrders, bool isB2b, string pickTime, SplitPickType enumSplitPickType, string warehouseId, string areaCode, out List<F051202> f051202s, out List<F1511> f1511s, out F05120101 f05120101)
		{
			var f051201 = new F051201
			{
				CUST_CODE = custCode,
				DC_CODE = dcCode,
				DELV_DATE = DateTime.Today,
				DEVICE_PRINT = "0",
				GUP_CODE = gupCode,
				ISDEVICE = "0",
				ISPRINTED = "0",
				ORD_TYPE = (isB2b) ? "0" : "1",
				PICK_ORD_NO = pickOrdNo,
				PICK_STATUS = 0,
				PICK_TIME = pickTime
			};

			f051202s = new List<F051202>();
			f1511s = new List<F1511>();
			f05120101 = null;
			var pickOrdSeq = 1;
			foreach (var pickWmsOrderAllotedStockOrder in pickWmsOrderAllotedStockOrders)
			{
				foreach (var allotedStockOrder in pickWmsOrderAllotedStockOrder.Value)
				{
					if (allotedStockOrder.F050302.ORD_QTY <= 0)
						continue;

					var f051202 = new F051202
					{
						B_PICK_QTY = allotedStockOrder.F050302.ORD_QTY,
						CUST_CODE = custCode,
						DC_CODE = dcCode,
						ENTER_DATE = allotedStockOrder.EnterDate,
						GUP_CODE = gupCode,
						ITEM_CODE = allotedStockOrder.F050302.ITEM_CODE,
						PICK_LOC = allotedStockOrder.LocCode,
						PICK_ORD_NO = pickOrdNo,
						PICK_ORD_SEQ = pickOrdSeq.ToString("0000"),
						PICK_STATUS = "0",
						SERIAL_NO = allotedStockOrder.SerialNo == "0" ? "" : allotedStockOrder.SerialNo,//0代表無指定序號,不Insert序號值
						VALID_DATE = allotedStockOrder.ValidDate,
						VNR_CODE = allotedStockOrder.VnrCode,
						BOX_CTRL_NO = allotedStockOrder.BoxCtrlNo,
						PALLET_CTRL_NO = allotedStockOrder.PalletCtrlNo,
						WMS_ORD_NO = pickWmsOrderAllotedStockOrder.Key,
						MAKE_NO = string.IsNullOrWhiteSpace(allotedStockOrder.MakeNo) ? "0" : allotedStockOrder.MakeNo
					};
					f051202s.Add(f051202);

					//扣F1913庫存數					
					_f1913Rep.MinusQty(dcCode, gupCode, custCode, allotedStockOrder.F050302.ITEM_CODE, allotedStockOrder.LocCode,
							allotedStockOrder.ValidDate, allotedStockOrder.EnterDate, allotedStockOrder.VnrCode, allotedStockOrder.SerialNo, allotedStockOrder.F050302.ORD_QTY,
							Current.Staff, Current.StaffName, allotedStockOrder.BoxCtrlNo, allotedStockOrder.PalletCtrlNo, allotedStockOrder.MakeNo);
					//寫入F1511虛擬儲位					 
					var f1511 = new F1511
					{
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						ORDER_NO = pickOrdNo,
						ORDER_SEQ = pickOrdSeq.ToString("0000"),
						STATUS = "0",
						B_PICK_QTY = allotedStockOrder.F050302.ORD_QTY,
						A_PICK_QTY = 0,
						ITEM_CODE = allotedStockOrder.F050302.ITEM_CODE,
						VALID_DATE = allotedStockOrder.ValidDate,
						ENTER_DATE = allotedStockOrder.EnterDate,
						SERIAL_NO = allotedStockOrder.SerialNo,
						LOC_CODE = allotedStockOrder.LocCode,
						BOX_CTRL_NO = allotedStockOrder.BoxCtrlNo,
						PALLET_CTRL_NO = allotedStockOrder.PalletCtrlNo,
						MAKE_NO = string.IsNullOrWhiteSpace(allotedStockOrder.MakeNo) ? "0" : allotedStockOrder.MakeNo
					};
					f1511s.Add(f1511);

					pickOrdSeq++;
				}
			}
			var f1909 = GetF1909(gupCode, custCode);
			if (f1909.CAL_CUFT == "1") //計算 出貨單 的 各品項+效期 之 重量、整數箱、零數箱、材積
			{
				CalCuft(dcCode, gupCode, custCode, f051202s);
			}

			if (enumSplitPickType == SplitPickType.Area)
			{
				var f191902 = GetF191902(dcCode, gupCode, custCode, warehouseId, areaCode);
				if (f191902 != null)
				{
					f05120101 = new F05120101
					{
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						PICK_ORD_NO = pickOrdNo,
						WAREHOUSE_ID = f191902.WAREHOUSE_ID,
						AREA_CODE = f191902.AREA_CODE,
						PICK_TYPE = f191902.PICK_TYPE,
						PICK_TOOL = f191902.PICK_TOOL,
						PUT_TOOL = f191902.PUT_TOOL,
						PICK_SEQ = f191902.PICK_SEQ,
						SORT_BY = f191902.SORT_BY,
						SINGLE_BOX = f191902.SINGLE_BOX,
						PICK_CHECK = f191902.PICK_CHECK,
						PICK_UNIT = f191902.PICK_UNIT,
						PICK_MARTERIAL = f191902.PICK_MARTERIAL,
						DELIVERY_MARTERIAL = f191902.DELIVERY_MARTERIAL
					};
				}
			}
			return f051201;
		}

		private string GetPickTime(string sourceType)
		{
			DateTime? pickDateTime;
			if (!_pickDateTimeMax.HasValue)
			{
				pickDateTime = DateTime.Now;
			}
			else
			{
				//如果目前時間比最大批次時段大5分鐘則取目前時間,否則取最大批次時段+5
				var diffMin = new TimeSpan(DateTime.Now.Ticks - _pickDateTimeMax.Value.Ticks).TotalMinutes;
				pickDateTime = diffMin >= 5 ? DateTime.Now : _pickDateTimeMax.Value.AddMinutes(5);
			}
			_pickDateTimeMax = pickDateTime;

			return pickDateTime.Value.ToString("HH:mm");
		}

		/// <summary>
		/// 取得目前批次最新時段
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="delvDate"></param>
		/// <returns></returns>
		private DateTime? GetMaxPickTime(string dcCode, string gupCode, string custCode, DateTime delvDate)
		{
			var f0513S = _f0513Rep.GetData(dcCode, gupCode, custCode, DateTime.Today).ToList();
			if (f0513S.Any())
			{
				return DateTime.Parse(string.Format("{0} {1}", delvDate.ToString("yyyy-MM-dd"), f0513S.Max(n => n.PICK_TIME)));
			}
			return null;
		}

		private void CreatePickTimeTable(string gupCode, string custCode, string dcCode, string pickTime, Dictionary<F050801, List<F050802>> f050801Dic)
		{
			if (f050801Dic.Count == 0)
				return;

			var f0513Data = new F0513()
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				DELV_DATE = DateTime.Today,
				PICK_TIME = pickTime,
				ORD_TYPE = f050801Dic.Keys.First().ORD_TYPE,
				RETAIL_QTY = (short)f050801Dic.Keys.Count,
				PROC_FLAG = "1",
				PICK_CNT = f050801Dic.SelectMany(f050802S => f050802S.Value).Aggregate(0, (current, f050802) => current + f050802.B_DELV_QTY ?? 0),
				SOURCE_TYPE = f050801Dic.Keys.First().SOURCE_TYPE

			};
			_f0513Rep.Add(f0513Data);
		}
		#endregion 拆併單

		#region 配庫
		private List<AllotedStockOrder> AllotStockForOrder(string gupCode, string custCode, string dcCode, List<F050301> f050301s, List<F050302> f050302s)
		{
			var allotStockOrderDetails = (from m in f050301s
																		join d in f050302s
																		on new { m.DC_CODE, m.GUP_CODE, m.CUST_CODE, m.ORD_NO } equals new { d.DC_CODE, d.GUP_CODE, d.CUST_CODE, d.ORD_NO }
																		select new AllotStockOrderDetail
																		{
																			F050301 = m,
																			F050302 = d
																		}).ToList();
			if (!allotStockOrderDetails.Any())
				return new List<AllotedStockOrder>();

			var osap = new OrderStockCheckParam
			{
				DcCode = dcCode,
				GupCode = gupCode,
				CustCode = custCode,
				TicketId = f050301s.First().TICKET_ID,
				AllotStockOrderDetails = allotStockOrderDetails,
				IsTrialCalculation = false
			};
			var result = NewOrderStockCheck(osap);
			return NewOrderAllot(result);
		}

		private void AddNoCarPeriodMessage(string gupCode, string custCode, string dcCode, List<F050301> f050301s, IEnumerable<RetailCarPeriod> retailCarPeriods)
		{
			var noCarPeriodRetails = f050301s.Where(a => !string.IsNullOrEmpty(a.RETAIL_CODE) && !retailCarPeriods.Select(r => r.RETAIL_CODE).Contains(a.RETAIL_CODE)).Select(a => a.RETAIL_CODE).ToList();
			if (noCarPeriodRetails.Any())
			{
				//丟到訊息池
				var f1901 = GetF1901(dcCode);
				var f1929 = GetF1929(gupCode);
				var f1909 = GetF1909(gupCode, custCode);

				var msgNo = "AAM00021"; //物流中心:「{0}」業主:「{1}」貨主:「{2}」門市:「{3}」尚未設定出車時段
				var f0020 = GetF0020(msgNo);
				var msg = string.Format(f0020.MSG_CONTENT,
										(f1901 == null) ? "" : f1901.DC_NAME,
										(f1929 == null) ? "" : f1929.GUP_NAME,
										(f1909 == null) ? "" : f1909.SHORT_NAME,
										string.Join("、", noCarPeriodRetails));

				AddMessagePoolForInside("9", dcCode, gupCode, custCode, msgNo, msg);
			}
		}

		#region 配庫訂單排序
		/// <summary>
		/// 不允許部分出貨訂單排序
		/// </summary>
		/// <param name="f050302s"></param>
		/// <param name="gF050301"></param>
		/// <returns></returns>
		private IEnumerable<OrdNoQty> SortOrderNotPartAllowed(List<F050302> f050302s, IGrouping<string, F050301> gF050301)
		{
			//依有指定序號、總揀次多及訂單編號小排序做配庫
			var oOrdNos = from a in f050302s
										join b in gF050301.AsEnumerable() on a.ORD_NO equals b.ORD_NO
										group a by a.ORD_NO into g
										let sumOrdQty = g.Sum(s => s.ORD_QTY)
										orderby f050302s.Any(a1 => a1.ORD_NO == g.Key && !string.IsNullOrEmpty(a1.SERIAL_NO)) descending,
														sumOrdQty descending,
														g.Key
										select new OrdNoQty { OrdNo = g.Key, SumOrdQty = sumOrdQty };

			return oOrdNos;
		}

		/// <summary>
		/// 允許部分出貨，訂單先進先出訂單排序
		/// </summary>
		/// <param name="f050302s"></param>
		/// <param name="gF050301"></param>
		/// <returns></returns>
		private IEnumerable<OrdNoQty> SortOrderPartAllowedByOrdNo(List<F050302> f050302s, IGrouping<string, F050301> gF050301)
		{
			//允許部分出貨，訂單先進先出
			var oOrdNos = from a in gF050301.AsEnumerable()
										orderby a.ORD_NO
										select new OrdNoQty { OrdNo = a.ORD_NO };

			return oOrdNos;
		}

		private IEnumerable<OrdNoQty> SortOrderPartAllowedByCarPeriod(string dcCode, string gupCode, string custCode, List<F050302> f050302s, IGrouping<string, F050301> gF050301, out IEnumerable<RetailCarPeriod> retailCarPeriods)
		{
			var repF194716 = new F194716Repository(Schemas.CoreSchema, _wmsTransaction);
			var retailCodes = gF050301.AsEnumerable().Select(a => a.RETAIL_CODE).ToList();
			retailCarPeriods = repF194716.GetRetailCarPeriods(dcCode, gupCode, custCode, retailCodes);
			//允許部分出貨，依出貨時段
			var oOrdNos = (from a in gF050301.AsEnumerable()
										 join c in retailCarPeriods on a.RETAIL_CODE equals c.RETAIL_CODE into j
										 from c in j.DefaultIfEmpty()
										 select new OrdNoQty { OrdNo = a.ORD_NO, CarPeriod = (c == null ? "Z" : c.CAR_PERIOD) })
																	.OrderBy(a => a.CarPeriod).ThenBy(a => a.OrdNo).AsEnumerable();

			return oOrdNos;
		}

		#region 計算平均分攤的分攤數量
		private void CalcEquallyDistributed(IEnumerable<OrdNoQty> oOrdNos, List<ItemLocPriorityInfo> pickLocPriorityInfos, List<F050301> f050301s, List<F050302> f050302s, bool isB2B, F1909 f1909, ref List<OrdDetailPartItemQty> ordDetailPartItemQtys)
		{
			//訂單依出車時段分群
			var gOrdNos = oOrdNos.GroupBy(a => a.CarPeriod).OrderBy(g => g.Key);
			//合計品項可出數量
			var itemSumQtys = (from a in pickLocPriorityInfos
												 group a by a.ITEM_CODE into g
												 let sumQty = g.Sum(s => s.QTY)
												 select new ItemSumQty { ITEM_CODE = g.Key, SumQty = sumQty }).ToList();


			foreach (var gOrdNo in gOrdNos)
			{
				var ordNos = gOrdNo.Select(a => a.OrdNo).ToList();
				//取得群組(出車時段)訂單的商品及數量
				var partOrdItemQtys = from a in f050302s
															join b in gOrdNo on a.ORD_NO equals b.OrdNo
															group a by new { a.ORD_NO, a.ITEM_CODE } into g
															let qty = g.Sum(s => s.ORD_QTY)
															select new { g.Key.ORD_NO, g.Key.ITEM_CODE, Qty = qty };
				//取得群組(出車時段)各商品的總數量
				var partOrdItemSumQtys = from a in partOrdItemQtys
																 group a by a.ITEM_CODE into g
																 let sumQry = g.Sum(s => s.Qty)
																 select new ItemSumQty { ITEM_CODE = g.Key, SumQty = sumQry };

				//判斷各商品數量總數是否滿足
				foreach (var partOrdItemSumQty in partOrdItemSumQtys)
				{
					var ordNos2 = f050302s.Where(a => a.ITEM_CODE == partOrdItemSumQty.ITEM_CODE).Select(a => a.ORD_NO).ToList();
					var itemSumQty = itemSumQtys.SingleOrDefault(a => a.ITEM_CODE == partOrdItemSumQty.ITEM_CODE);
					if (itemSumQty == null || itemSumQty.SumQty <= 0)
					{
						//此商品可出數量為0時，設定此群組此商品的數量為0
						ordDetailPartItemQtys.Where(a => ordNos.Contains(a.OrdNo) && a.ItemCode == partOrdItemSumQty.ITEM_CODE).ToList()
								.ForEach(a => a.PartItemQty = 0);
					}
					//大於表示全部滿足，可分配數扣除此群組總數
					else if (itemSumQty.SumQty >= partOrdItemSumQty.SumQty)
					{
						itemSumQty.SumQty = itemSumQty.SumQty - ordDetailPartItemQtys.Where(a => ordNos.Contains(a.OrdNo) && a.ItemCode == partOrdItemSumQty.ITEM_CODE).Sum(a => a.PartItemQty);
					}
					//當可出數量少於群組商品總數時須平均分攤
					else if (itemSumQty.SumQty < partOrdItemSumQty.SumQty)
					{
						CalcEquallyDistributedEmpl(itemSumQty, ordNos2, partOrdItemSumQty, ref ordDetailPartItemQtys);
					}
				}
			}
		}

		private void CalcEquallyDistributedEmpl(ItemSumQty itemSumQty, List<string> ordNos, ItemSumQty partOrdItemSumQty, ref List<OrdDetailPartItemQty> ordDetailPartItemQtys)
		{
			//取可出數的平均數
			var itemAverageQty = (int)((itemSumQty.SumQty ?? 0) / ordNos.Count);

			//取得少等於平均數的訂單商品，這些訂單的商品可完全滿足，不須再計算數量
			var enoughOrdDetailPartItemQtys = ordDetailPartItemQtys.Where(a => ordNos.Contains(a.OrdNo) && a.ItemCode == partOrdItemSumQty.ITEM_CODE && a.PartItemQty <= itemAverageQty);
			if (enoughOrdDetailPartItemQtys.Any())
			{
				itemSumQty.SumQty = itemSumQty.SumQty - enoughOrdDetailPartItemQtys.Sum(a => a.PartItemQty);

				//剩餘訂單須重新計算平均數做分攤
				var ordNos2 = ordNos.Where(a => !enoughOrdDetailPartItemQtys.Select(b => b.OrdNo).Distinct().ToList().Contains(a)).ToList();
				CalcEquallyDistributedEmpl(itemSumQty, ordNos2, partOrdItemSumQty, ref ordDetailPartItemQtys);
			}
			else
			{
				//先分配給每張訂單平均數
				ordDetailPartItemQtys.Where(a => ordNos.Contains(a.OrdNo) && a.ItemCode == partOrdItemSumQty.ITEM_CODE).ToList()
						.ForEach(a => a.PartItemQty = itemAverageQty);

				//取餘數
				var modQty = (int)((itemSumQty.SumQty ?? 0) % ordNos.Count);
				//餘數的前幾筆各+1 (將剩餘數量平均分配在前幾個訂單)
				ordDetailPartItemQtys.Where(a => ordNos.Contains(a.OrdNo) && a.ItemCode == partOrdItemSumQty.ITEM_CODE)
						.OrderBy(a => a.OrdNo).Take(modQty).ToList()
						.ForEach(a => a.PartItemQty++);

				//因數量不足，已全數分攤，所以這邊歸0
				itemSumQty.SumQty = 0;
			}
		}


		/// <summary>
		/// 已配庫但未全配完的訂單明細List
		/// </summary>
		private List<AllotedPartOrdDetailInfo> _allotedPartOrdDetailInfos = new List<AllotedPartOrdDetailInfo>();

		#region 有允出天數限制的平均分攤配庫
		//概念
		//希望盡可能將可出的庫存配出去，因此希望最小效期的庫存盡量配給允許最小效期的訂單明細
		//庫存依效期分群組，短效期優先配
		//找出允許配此效期的訂單明細，取平均數分配(邏輯如沒有允出限制的平均分攤)
		//因訂單的各明細的允許最小效期在所有訂單中的排序未必一樣，所以要打散各群組(出車時段)的訂單明細來配庫
		private void AllotedEquallyDistributedAllowDelvDay(IEnumerable<OrdNoQty> oOrdNos, List<F050301> f050301s, List<F050302> details, List<ItemLocPriorityInfo> pickLocPriorityInfos, ref List<AllotedStockOrder> allotedStockOrders)
		{
			var itemLimitValidDays = new List<ItemLimitValidDay>();
			var gRetailF050301s = f050301s.GroupBy(a => a.RETAIL_CODE);
			foreach (var gRetailF050301 in gRetailF050301s)
			{
				var f050301First = gRetailF050301.First();
				var ordNos = gRetailF050301.Select(a => a.ORD_NO);
				var itemCodes = details.Where(a => ordNos.Contains(a.ORD_NO)).Select(a => a.ITEM_CODE).Distinct().ToList();
				itemLimitValidDays.AddRange(GetItemLimitValidDays(f050301First.GUP_CODE, f050301First.CUST_CODE, gRetailF050301.Key, itemCodes));
			}
			itemLimitValidDays = itemLimitValidDays.Distinct().ToList();

			//依出車時段分群訂單
			var gOrdNos = oOrdNos.GroupBy(a => a.CarPeriod).OrderBy(a => a.Key);
			foreach (var gOrdNo in gOrdNos)
			{
				var ordNos = gOrdNo.Select(a => a.OrdNo);
				//依品項分群訂單明細
				var gDetails = details.Where(a => ordNos.Contains(a.ORD_NO)).GroupBy(a => a.ITEM_CODE);
				foreach (var gDetail in gDetails)
				{

					//先過濾此品項的庫存資料，再依儲區別將庫存分群排序(黃金先於一般)
					var gATypePickLocPriorityInfos = pickLocPriorityInfos.Where(a => a.ITEM_CODE == gDetail.Key).GroupBy(a => a.ATYPE_CODE);
					foreach (var gATypePickLocPriorityInfo in gATypePickLocPriorityInfos)
					{
						//不同的庫存群組視為另一次的配庫，所以將_allotedPartOrdDetailInfos清除重來
						_allotedPartOrdDetailInfos.Clear();
						//  再依效期分群
						var validDatePickLocPriorityInfos = gATypePickLocPriorityInfo.Where(a => a.QTY > 0).GroupBy(a => a.VALID_DATE).OrderBy(a => a.Key);
						foreach (var validDatePickLocPriorityInfo in validDatePickLocPriorityInfos)
						{
							//validDateDetails = 取得允許配此效期的訂單明細
							var validDateDetails = (from a in gDetail
																			join b in f050301s on new { a.GUP_CODE, a.DC_CODE, a.CUST_CODE, a.ORD_NO } equals new { b.GUP_CODE, b.DC_CODE, b.CUST_CODE, b.ORD_NO }
																			join c in itemLimitValidDays on new { a.ITEM_CODE, b.RETAIL_CODE } equals new { c.ITEM_CODE, c.RETAIL_CODE } into j
																			from c in j.DefaultIfEmpty()
																			where (c == null || validDatePickLocPriorityInfo.Key >= DateTime.Today.AddDays(c.DELIVERY_DAY))
																					&& a.ORD_QTY > 0
																			select a).Distinct().ToList();
							var sortValidDatePickLocPriorityInfos = validDatePickLocPriorityInfo.OrderBy(a => a.ENTER_DATE)
																			.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE);
							AllotedEquallyDistributedAllowDelvDayEmpl(validDateDetails, sortValidDatePickLocPriorityInfos, ref allotedStockOrders);
						}
					}
				}
			}

		}

		/// <summary>
		/// 有允出天數限制的平均分攤配庫實作
		/// 因會包含允許上一效期的訂單明細，允許上一效期的訂單明細已配部分數量
		/// 若排除已配部分數量的訂單明細所算得的平均數小於等於之前最小的部分配庫數量，則此次只配未配庫的部分(因為配給這些明細的數量最多跟先前配的部分數量一樣多)。
		/// 若平均數大於之前最小的部分配庫數量(表示應該要再配一些量給先前的明細)，將前已配的數量與這次可配的數量加總再取一次平均數(遞迴取得最終的平均數)，
		/// 取的的平均數在扣掉之前已配的數量進行配庫
		/// </summary>
		/// <param name="validDateDetails"></param>
		/// <param name="sortValidDatePickLocPriorityInfos"></param>
		/// <param name="allotedStockOrders"></param>
		private void AllotedEquallyDistributedAllowDelvDayEmpl(List<F050302> validDateDetails, IEnumerable<ItemLocPriorityInfo> sortValidDatePickLocPriorityInfos, ref List<AllotedStockOrder> allotedStockOrders)
		{
			if (validDateDetails.Count == 0)
				return;

			//計算可出庫存總數
			var stockSumQty = sortValidDatePickLocPriorityInfos.Sum(a => a.QTY);

			var itemAverageQty = 0;
			var modQty = 0;
			var tmpValidDateDetails = validDateDetails.Select(a => a).ToList();
			if (!_allotedPartOrdDetailInfos.Any())
			{
				//取可出數的平均數
				itemAverageQty = (int)(stockSumQty / tmpValidDateDetails.Count);
				//取餘數
				modQty = (int)(stockSumQty % tmpValidDateDetails.Count);
			}
			else
			{
				//計算平均數
				CalcAverageQty(validDateDetails, stockSumQty, out itemAverageQty, out modQty, out tmpValidDateDetails);
			}

			//取得少等於平均數的訂單明細，這些訂單明細可完全滿足，不須再計算數量
			var allotedOrdNos = _allotedPartOrdDetailInfos.Select(a => a.F050302.ORD_NO).ToList();
			var enoughOrdDetailPartItemQtys = tmpValidDateDetails.Where(a => a.ORD_QTY <= itemAverageQty && !allotedOrdNos.Contains(a.ORD_NO)).
					Union(_allotedPartOrdDetailInfos.Where(a => a.F050302.ORD_QTY + a.AllotedQty <= itemAverageQty).Select(a => a.F050302)).ToList();
			if (enoughOrdDetailPartItemQtys.Any())
			{
				//對 enoughOrdDetailPartItemQtys 執行配庫
				//配庫中會扣除可配庫的庫存數量
				foreach (var f050302 in enoughOrdDetailPartItemQtys)
				{
					var ordQty = f050302.ORD_QTY;
					CreateAllotedStockOrder(sortValidDatePickLocPriorityInfos, f050302, ref ordQty, ref allotedStockOrders);
					//扣除這次配庫數(原則上此時ordQty應該為0)
					f050302.ORD_QTY = ordQty;

					var allotedOrdDetailInfo = _allotedPartOrdDetailInfos.SingleOrDefault(a => a.F050302.Equals(f050302));
					//由已配庫但未全配完的訂單明細List移除已完全配完的訂單明細
					if (f050302.ORD_QTY == 0 && allotedOrdDetailInfo != null)
						_allotedPartOrdDetailInfos.Remove(allotedOrdDetailInfo);
				}

				stockSumQty = stockSumQty - enoughOrdDetailPartItemQtys.Sum(a => a.ORD_QTY);

				//剩餘訂單須重新計算平均數做分攤
				var validDateDetails2 = tmpValidDateDetails.Where(a => !enoughOrdDetailPartItemQtys.Select(b => b.ORD_NO).Distinct().ToList().Contains(a.ORD_NO)).ToList();
				AllotedEquallyDistributedAllowDelvDayEmpl(validDateDetails2, sortValidDatePickLocPriorityInfos, ref allotedStockOrders);
			}
			else
			{
				//先分配給每張訂單明細平均數 執行配庫
				//配庫中會扣除可配庫的庫存數量
				foreach (var f050302 in tmpValidDateDetails)
				{
					var allotedOrdDetailInfo = _allotedPartOrdDetailInfos.SingleOrDefault(a => a.F050302.Equals(f050302));
					//取的的平均數在扣掉之前已配的數量進行配庫
					var ordQty = itemAverageQty - ((allotedOrdDetailInfo == null) ? 0 : allotedOrdDetailInfo.AllotedQty);
					CreateAllotedStockOrder(sortValidDatePickLocPriorityInfos, f050302, ref ordQty, ref allotedStockOrders);
					//扣除這次配庫數(原則上此時ordQty應該為0)
					f050302.ORD_QTY = f050302.ORD_QTY - itemAverageQty + ordQty;

					if (allotedOrdDetailInfo == null)
					{
						allotedOrdDetailInfo = new AllotedPartOrdDetailInfo { AllotedQty = itemAverageQty - ordQty, F050302 = f050302 };
						_allotedPartOrdDetailInfos.Add(allotedOrdDetailInfo);
					}
					else
						allotedOrdDetailInfo.AllotedQty += itemAverageQty - ordQty;
				}

				//餘數的前幾筆各+1 (將剩餘數量平均分配在前幾個訂單)
				var validDateDetails2 = tmpValidDateDetails
						.OrderBy(a => a.ORD_NO).Take(modQty).ToList();
				foreach (var f050302 in validDateDetails2)
				{
					var ordQty = 1;
					CreateAllotedStockOrder(sortValidDatePickLocPriorityInfos, f050302, ref ordQty, ref allotedStockOrders);
					//扣除已配庫數(原則上此時ordQty應該為0)
					f050302.ORD_QTY = f050302.ORD_QTY - 1 + ordQty;

					var allotedOrdDetailInfo = _allotedPartOrdDetailInfos.Single(a => a.F050302.Equals(f050302));
					//由已配庫但未全配完的訂單明細List移除已完全配完的訂單明細
					if (f050302.ORD_QTY == 0)
						_allotedPartOrdDetailInfos.Remove(allotedOrdDetailInfo);
					else
						allotedOrdDetailInfo.AllotedQty += 1 - ordQty;
				}
			}
		}

		/// <summary>
		/// 有允出天數限制的平均分攤配庫計算平均數
		/// 將前已配的數量與這次可配的數量加總再取一次平均數(遞迴取得最終的平均數)
		/// 若此次
		/// </summary>
		/// <param name="validDateDetails"></param>
		/// <param name="stockSumQty"></param>
		/// <param name="itemAverageQty"></param>
		/// <param name="modQty"></param>
		/// <param name="tmpValidDateDetails"></param>
		/// <param name="preMinAllotedQty"></param>
		private void CalcAverageQty(List<F050302> validDateDetails, long stockSumQty, out int itemAverageQty, out int modQty, out List<F050302> tmpValidDateDetails, int preMinAllotedQty = 0)
		{
			//先排除大於上一次遞迴的最小已配庫數(即取得未配庫及小於等於上一次遞迴的最小已配庫數)的訂單明細 (本次要計算的明細)
			var allotedOrdNos = _allotedPartOrdDetailInfos.Where(a => a.AllotedQty > preMinAllotedQty).Select(a => a.F050302.ORD_NO);
			tmpValidDateDetails = validDateDetails.Where(a => !allotedOrdNos.Contains(a.ORD_NO)).ToList();

			//由已配庫但未全配完的訂單明細List中，過濾小於等於上一次遞迴的最小已配庫數的訂單明細，計算已配庫數
			var tmpAllotedOrdDetailInfos = _allotedPartOrdDetailInfos.Where(a => a.AllotedQty <= preMinAllotedQty);
			var tmpAllotedQty = tmpAllotedOrdDetailInfos.Sum(a => a.AllotedQty);

			//將前已配的數量與這次可配的數量加總再取一次平均數
			itemAverageQty = 0;
			modQty = 0;
			if (tmpValidDateDetails.Any())
			{
				itemAverageQty = (int)((stockSumQty + tmpAllotedQty) / tmpValidDateDetails.Count);
				modQty = (int)((stockSumQty + tmpAllotedQty) % tmpValidDateDetails.Count);
			}

			var nowAllotedPartOrdDetailInfos = _allotedPartOrdDetailInfos.Where(a => a.AllotedQty > preMinAllotedQty);
			if (nowAllotedPartOrdDetailInfos.Any())
			{
				var minAllotedQty = nowAllotedPartOrdDetailInfos.Min(a => a.AllotedQty);
				//已無前面已配庫訂單明細或平均數小於等於tmpValidDateDetails前面已配庫訂單明細的最少量，則全配在tmpValidDateDetails中
				//若平均數大於於tmpValidDateDetails前面已配庫訂單明細的最少量，則須重新計算平均數
				if (!tmpValidDateDetails.Any() || itemAverageQty > minAllotedQty && allotedOrdNos.Any())
				{
					//計算
					CalcAverageQty(validDateDetails, stockSumQty, out itemAverageQty, out modQty, out tmpValidDateDetails, minAllotedQty);
				}
			}
		}
		#endregion 有允出天數限制的平均分攤配庫


		#endregion 計算平均分攤的分攤數量
		#endregion

		private void CreateAllotedStockOrder(IEnumerable<ItemLocPriorityInfo> oItemPickLocPriorityInfos, F050302 f050302, ref int ordQty, ref List<AllotedStockOrder> allotedStockOrders)
		{
			foreach (var itemPickLocPriorityInfo in oItemPickLocPriorityInfos)
			{
				var isLocEnough = true;
				var allotQty = ordQty;
				if (itemPickLocPriorityInfo.QTY >= ordQty) //儲位數量足夠
				{
					//扣除儲位數量
					itemPickLocPriorityInfo.QTY -= ordQty;
					ordQty = 0;
				}
				else //儲位數量不足
				{
					allotQty = (int)itemPickLocPriorityInfo.QTY; //本儲位配庫的數量
					ordQty -= (int)itemPickLocPriorityInfo.QTY; //剩餘未配庫的數量
					itemPickLocPriorityInfo.QTY = 0; //儲位數量全數配庫，因此歸0
					isLocEnough = false; //表示本儲位不足
				}
				var allotF050302 = AutoMapper.Mapper.DynamicMap<F050302>(f050302);
				allotF050302.ORD_QTY = allotQty;
				allotedStockOrders.Add(new AllotedStockOrder
				{
					F050302 = allotF050302,
					EnterDate = itemPickLocPriorityInfo.ENTER_DATE,
					Floor = itemPickLocPriorityInfo.FLOOR,
					LocCode = itemPickLocPriorityInfo.LOC_CODE,
					SerialNo = itemPickLocPriorityInfo.SERIAL_NO,
					TmprType = itemPickLocPriorityInfo.TMPR_TYPE,
					ValidDate = itemPickLocPriorityInfo.VALID_DATE,
					VnrCode = itemPickLocPriorityInfo.VNR_CODE,
					BoxCtrlNo = itemPickLocPriorityInfo.BOX_CTRL_NO,
					PalletCtrlNo = itemPickLocPriorityInfo.PALLET_CTRL_NO,
					WarehouseId = itemPickLocPriorityInfo.WAREHOUSE_ID,
					AreaCode = itemPickLocPriorityInfo.AREA_CODE,
					MakeNo = itemPickLocPriorityInfo.MAKE_NO,
					Orginal_OrdQty = f050302.ORD_QTY
				});
				if (isLocEnough) //儲位數量已滿足則跳開，若不足則找下一順序儲位配庫
					break;
			}
		}




		/// <summary>
		/// 透過商品不足的數量與現有虛擬儲位庫存的數量，計算出要從補貨區取得的數量
		/// </summary>
		/// <param name="notEnoughItems"></param>
		/// <param name="virtualQtyItems"></param>
		/// <returns></returns>
		private static List<ItemQty> GetResupplyQtyItems(List<ItemQty> notEnoughItems, List<ItemQty> virtualQtyItems)
		{
			var query = from notEnoughItem in notEnoughItems
									join virtualQtyItem in virtualQtyItems
									on notEnoughItem.ItemCode equals virtualQtyItem.ItemCode into b
									from c in b.DefaultIfEmpty()
										// 虛擬儲位庫存數量: 用來在 select 時，取得應該要從補貨區取得的數量計算
										// 1.若虛擬儲位沒庫存
										// 2.若虛擬儲位庫存大於不足的揀貨區庫存數，則只要取不足的揀貨數量
										// 3.若虛擬儲位庫存小於不足的揀貨區庫存數，則取虛擬儲位庫存數
									let virtualQty = (c == null) ? 0
																									 : (c.Qty > notEnoughItem.Qty) ? notEnoughItem.Qty
																																							 : c.Qty
									// 如果虛擬儲位庫存數大於不足的揀貨庫存數，就不用從補貨區取得，所以只需要有 不足的揀貨庫存數 > 虛擬儲位庫存數
									where notEnoughItem.Qty > virtualQty
									select new ItemQty
									{
										ItemCode = notEnoughItem.ItemCode,
										LocCode = notEnoughItem.LocCode,
										Qty = notEnoughItem.Qty - virtualQty
									};

			return query.ToList();
		}

		/// <summary>
		/// 檢查訂單是否符合商品限制效期
		/// </summary>
		/// <param name="f050301">訂單主檔</param>
		/// <param name="f050302s">訂單明細</param>
		/// <param name="pickLocPriorityInfos">庫存資料</param>
		/// <param name="itemLimitValidDays">商品限制效期天數清單</param>
		/// <returns></returns>
		private bool CheckValidDate(F050301 f050301, IEnumerable<F050302> f050302s, List<ItemLocPriorityInfo> pickLocPriorityInfos)
		{
			var msgNo = "AAM00024"; //訂單編號:{0} 商品:{1} 符合允出天數的庫存不足
			var f0020 = GetF0020(msgNo);

			var itemLimitValidDays = GetItemLimitValidDays(f050301.GUP_CODE, f050301.CUST_CODE, f050301.RETAIL_CODE, f050302s.Select(x => x.ITEM_CODE).Distinct().ToList()).ToList();
			var groupDetail = f050302s.GroupBy(x => x.ITEM_CODE);
			var isEnougthQty = true;
			var message = new List<string>();
			foreach (var detail in groupDetail)
			{
				var itemLimitValidDay = itemLimitValidDays.FirstOrDefault(x => x.ITEM_CODE == detail.Key);

				if (itemLimitValidDay != null)
				{
					var stockQty = pickLocPriorityInfos.Where(x => x.ITEM_CODE == detail.Key && x.VALID_DATE >= DateTime.Today.AddDays(itemLimitValidDay.DELIVERY_DAY)).Sum(x => x.QTY);
					var detailQty = detail.Sum(x => x.ORD_QTY);
					//庫存數小於訂購數 此訂單不配庫
					if (stockQty < detailQty)
					{
						message.Add(string.Format(f0020.MSG_CONTENT, f050301.ORD_NO, detail.Key));
						isEnougthQty = false;
					}
				}
			}
			if (message.Any())
			{
				//寫入訊息池
				AddMessagePoolForInside("9", f050301.DC_CODE, f050301.GUP_CODE, f050301.CUST_CODE, msgNo, string.Join(Environment.NewLine, message));
			}
			if (isEnougthQty)
			{
				foreach (var detail in groupDetail)
				{
					var itemLimitValidDay = itemLimitValidDays.FirstOrDefault(x => x.ITEM_CODE == detail.Key);
					var stocks = (itemLimitValidDay != null) ? pickLocPriorityInfos.Where(x => x.ITEM_CODE == detail.Key && x.VALID_DATE >= DateTime.Today.AddDays(itemLimitValidDay.DELIVERY_DAY)).OrderBy(x => x.VALID_DATE) : pickLocPriorityInfos.Where(x => x.ITEM_CODE == detail.Key).OrderBy(x => x.VALID_DATE);
					long detailQty = detail.Sum(x => x.ORD_QTY);
					foreach (var st in stocks)
					{
						if (st.QTY < detailQty)
						{
							detailQty -= st.QTY;
							st.QTY = 0;
						}
						else
						{
							st.QTY -= detailQty;
							detailQty = 0;
						}
						if (detailQty == 0)
							break;
					}
				}
			}
			return isEnougthQty;
		}

		/// <summary>
		/// 檢查訂單是否符合商品限制效期
		/// </summary>
		/// <param name="f050001">訂單主檔</param>
		/// <param name="f050002s">訂單明細</param>
		/// <returns></returns>
		private bool CheckValidDate(F050001 f050001, IEnumerable<F050002> f050002s, List<ItemLocPriorityInfo> pickLocPriorityInfos)
		{
			var msgNo = "AAM00024"; //訂單編號:{0} 商品:{1} 符合允出天數的庫存不足
			var f0020 = GetF0020(msgNo);

			var itemLimitValidDays = GetItemLimitValidDays(f050001.GUP_CODE, f050001.CUST_CODE, f050001.RETAIL_CODE, f050002s.Select(x => x.ITEM_CODE).Distinct().ToList()).ToList();
			var groupDetail = f050002s.GroupBy(x => x.ITEM_CODE);
			var isEnougthQty = true;
			var message = new List<string>();
			foreach (var detail in groupDetail)
			{
				var itemLimitValidDay = itemLimitValidDays.FirstOrDefault(x => x.ITEM_CODE == detail.Key);

				if (itemLimitValidDay != null)
				{
					var stockQty = pickLocPriorityInfos.Where(x => x.ITEM_CODE == detail.Key && x.VALID_DATE >= DateTime.Today.AddDays(itemLimitValidDay.DELIVERY_DAY)).Sum(x => x.QTY);
					var detailQty = detail.Sum(x => x.ORD_QTY);
					//庫存數小於訂購數 此訂單不配庫
					if (stockQty < detailQty)
					{
						message.Add(string.Format(f0020.MSG_CONTENT, f050001.ORD_NO, detail.Key));
						isEnougthQty = false;
					}
				}
			}
			if (message.Any())
			{
				//寫入訊息池
				AddMessagePoolForInside("9", f050001.DC_CODE, f050001.GUP_CODE, f050001.CUST_CODE, msgNo, string.Join(Environment.NewLine, message));
			}

			if (isEnougthQty)
			{
				foreach (var detail in groupDetail)
				{
					var itemLimitValidDay = itemLimitValidDays.FirstOrDefault(x => x.ITEM_CODE == detail.Key);
					var stocks = (itemLimitValidDay != null) ? pickLocPriorityInfos.Where(x => x.ITEM_CODE == detail.Key && x.VALID_DATE >= DateTime.Today.AddDays(itemLimitValidDay.DELIVERY_DAY)).OrderBy(x => x.VALID_DATE) : pickLocPriorityInfos.Where(x => x.ITEM_CODE == detail.Key).OrderBy(x => x.VALID_DATE);
					long detailQty = detail.Sum(x => x.ORD_QTY);
					foreach (var st in stocks)
					{
						if (st.QTY < detailQty)
						{
							detailQty -= st.QTY;
							st.QTY = 0;
						}
						else
						{
							st.QTY -= detailQty;
							detailQty = 0;
						}
						if (detailQty == 0)
							break;
					}
				}
			}
			return isEnougthQty;
		}
		#endregion 配庫

		#region 派車
		private void DistributeCar(string gupCode, string custCode, string dcCode, Dictionary<F050801, List<F050802>> f050801Dic, List<F050301> f050301s, List<F05030101> f05030101s, bool isB2b)
		{
			//過濾掉自取、專車、超取及不需派車(含內部交易且同DC不需派車)
			var f050801s = f050801Dic.Keys.Where(a => a.SELF_TAKE != "1" && a.SPECIAL_BUS != "1" && a.NO_LOADING != "1" && a.CVS_TAKE != "1").ToList();
			var dF050801s = f050801s.Where(a => !string.IsNullOrEmpty(a.ALL_ID)).GroupBy(a => new { a.ALL_ID }).ToDictionary(g => g.Key.ALL_ID, g => g.ToList());
			var allIdWmsOrdTakeTimes = new List<AllIdWmsOrdTakeTime>();
			var noAllIdF050801s = f050801s.Where(a => string.IsNullOrEmpty(a.ALL_ID)).ToList();
			if (noAllIdF050801s.Any())
			{
				//為無指定配送商的出貨單，找出適合的配送商
				var allId = string.Empty;
				var f050301First = f050301s.First();
				var f19000104Rep = new F19000104Repository(Schemas.CoreSchema, _wmsTransaction);
				var f1947Rep = new F1947Repository(Schemas.CoreSchema, _wmsTransaction);
				//先取貨主單據的配送商
				var allIds = f19000104Rep.GetDatasByTrueAndCondition(a => a.TICKET_ID == f050301First.TICKET_ID && a.DC_CODE == dcCode).Select(a => a.ALL_ID).ToList();
				if (allIds == null || !allIds.Any()) //若無設定貨主單據配送商，取貨主配送商
					allIds = f1947Rep.GetAllowedF1947s(dcCode, gupCode, custCode).ToList().Where(a => !string.IsNullOrEmpty(a.CONSIGN_REPORT) && a.TYPE == "0").Select(a => a.ALL_ID).ToList();
				else
					allIds = f1947Rep.InWithTrueAndCondition("ALL_ID", allIds).ToList().Where(a => !string.IsNullOrEmpty(a.CONSIGN_REPORT)).Select(a => a.ALL_ID).ToList();
				if (allIds == null || !allIds.Any()) //若無設定貨主配送商，取所有配送商
					allIds = f1947Rep.GetDatasByTrueAndCondition(a => a.DC_CODE == dcCode).ToList().Where(a => !string.IsNullOrEmpty(a.CONSIGN_REPORT) && a.TYPE == "0").Select(a => a.ALL_ID).ToList();

				var gNoAllIdF050801s = noAllIdF050801s.GroupBy(a => new { a.CAN_FAST, a.TMPR_TYPE, a.ZIP_CODE });
				foreach (var gNoAllIdF050801 in gNoAllIdF050801s)
				{
					GetDistrCarWmsOrds(dcCode, allIds, gNoAllIdF050801.ToList(), f050801Dic, ref allIdWmsOrdTakeTimes, isB2b);
				}
			}

			foreach (var dF050801 in dF050801s)
			{
				var gAllIdF050801s = dF050801.Value.GroupBy(a => new { a.CAN_FAST, a.TMPR_TYPE, a.ZIP_CODE });
				foreach (var gAllIdF050801 in gAllIdF050801s)
				{
					GetDistrCarWmsOrds(dcCode, new List<string> { dF050801.Key }, gAllIdF050801.ToList(), f050801Dic, ref allIdWmsOrdTakeTimes, isB2b);
				}
			}
			//產生託運單
			CreateConsign(allIdWmsOrdTakeTimes, f050301s, f05030101s);

			//依配送商、派車時段，產生不同的派車單(已過濾掉沒產生託運單的出貨單)
			var distrCars = CreateDistributeCar(gupCode, custCode, dcCode, allIdWmsOrdTakeTimes, f050801Dic, f050301s, f05030101s);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="allIdWmsOrdTakeTimes">配送商出貨單對應 Dictionary</param>
		/// <param name="f050801Dic">出貨單與出貨單明細對應 Dictionary</param>
		/// <param name="f050301s"></param>
		/// <param name="f05030101s"></param>
		/// <returns></returns>
		private List<DistrCar> CreateDistributeCar(string gupCode, string custCode, string dcCode, List<AllIdWmsOrdTakeTime> allIdWmsOrdTakeTimes, Dictionary<F050801, List<F050802>> f050801Dic, List<F050301> f050301s, List<F05030101> f05030101s)
		{
			var distrCars = new List<DistrCar>();
			var f1947Rep = new F1947Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801Rep = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1947s = f1947Rep.InWithTrueAndCondition("ALL_ID", allIdWmsOrdTakeTimes.Select(a => a.AllId).Distinct().ToList(), a => a.DC_CODE == dcCode);

			//依配送商，分不同的派車單
			foreach (var allIdWmsOrdTakeTime in allIdWmsOrdTakeTimes)
			{
				//更改尚未指定配送商的出貨單的配送商編號
				var wmsOrdNos = allIdWmsOrdTakeTime.WmsOrdTakeTimes.Where(a => string.IsNullOrEmpty(a.F050801.ALL_ID)).Select(a => a.F050801.WMS_ORD_NO).ToList();
				f050801Rep.UpdateAllId(gupCode, custCode, dcCode, wmsOrdNos, allIdWmsOrdTakeTime.AllId);

				var f1947 = f1947s.Single(a => a.DC_CODE == dcCode && a.ALL_ID == allIdWmsOrdTakeTime.AllId);
				//依派車日期、時段，分不同的派車單
				var gWmsOrdTakeTimes = allIdWmsOrdTakeTime.WmsOrdTakeTimes.GroupBy(a => new { a.TakeDate, a.TakeTime, a.F050801.WMS_ORD_NO });
				foreach (var gWmsOrdTakeTime in gWmsOrdTakeTimes)
				{
					var f700101 = new F700101
					{
						ALL_ID = allIdWmsOrdTakeTime.AllId,
						CHARGE_CUST = "1",
						CHARGE_DC = "0",
						DC_CODE = dcCode,
						FEE = 0, //之後會依實際派車回檔後計算實際費用再填入
						PIER_CODE = f1947.PIER_CODE,
						ORD_PIER_CODE = f1947.PIER_CODE,
						SP_CAR = "0",
						STATUS = "0",
						TAKE_DATE = gWmsOrdTakeTime.Key.TakeDate,
						CHARGE_GUP_CODE = gupCode,
						CHARGE_CUST_CODE = custCode,
						DISTR_SOURCE = "0",
						HAVE_WMS_NO = "1"
					};

					var f700102s = new List<F700102>();
					short seq = 1;
					foreach (var wmsOrdTakeTime in gWmsOrdTakeTime.ToList())
					{
						var f050301 = (from a in f050301s
													 join b in f05030101s on a.ORD_NO equals b.ORD_NO
													 where b.WMS_ORD_NO == wmsOrdTakeTime.F050801.WMS_ORD_NO
													 select a).First();

						var f050801 = wmsOrdTakeTime.F050801;

						var f050802s = f050801Dic[f050801];
						var f700102 = new F700102
						{
							ADDRESS = f050301.ADDRESS,
							CAN_FAST = (wmsOrdTakeTime.CanFast) ? "1" : "0",
							CONTACT = f050301.CONTACT,
							CONTACT_TEL = f050301.CONTACT_TEL,
							CUST_CODE = custCode,
							CUST_NAME = f050301.CUST_NAME,
							DC_CODE = dcCode,
							DISTR_CAR_SEQ = seq,
							DISTR_USE = "01",
							GUP_CODE = gupCode,
							ITEM_QTY = (f050802s.Sum(a => a.B_DELV_QTY) ?? 0),
							ORD_TYPE = "O",
							RETAIL_CODE = f050301.RETAIL_CODE,
							TAKE_TIME = wmsOrdTakeTime.TakeTime,
							WMS_NO = wmsOrdTakeTime.F050801.WMS_ORD_NO,
							ZIP_CODE = wmsOrdTakeTime.F050801.ZIP_CODE,
							VOLUMN = f050801.VOLUMN,
							DELV_EFFIC = wmsOrdTakeTime.DelvEffic,
							DELV_TIMES = wmsOrdTakeTime.DelvTimes,
							DISTR_TYPE = "08"   // 其他
						};
						f700102s.Add(f700102);


						//wmsOrdTakeTime.Address = (!string.IsNullOrEmpty(f050301.ADDRESS_PARSE)) ? f050301.ADDRESS_PARSE : f050301.ADDRESS;
						seq++;
					}

					var distrCarWmsOrder = new DistrCar { Master = f700101, Details = f700102s };
					distrCars.Add(distrCarWmsOrder);
				}
			}

			// 延遲設定大量單號
			if (distrCars.Any())
			{
				var distrCarNoStack = _sharedService.GetNewOrdStackCodes("ZC", distrCars.Count());
				foreach (var distrCar in distrCars)
				{
					distrCar.Master.DISTR_CAR_NO = distrCarNoStack.Pop();
					distrCar.Details.ForEach(x => x.DISTR_CAR_NO = distrCar.Master.DISTR_CAR_NO);
				}

				// 並用 BulkInsert 一次大量新增派車單
				_sharedService.CreateDistributeCar(distrCars.Select(x => x.Master), distrCars.SelectMany(x => x.Details));

			}
			return distrCars;
		}

		private void GetDistrCarWmsOrds(string dcCode, List<string> allIds, List<F050801> f050801s, Dictionary<F050801, List<F050802>> f050801Dic, ref List<AllIdWmsOrdTakeTime> allIdWmsOrdTakeTimes, bool isB2b)
		{
			var f050801First = f050801s.First();
			var canFast = (f050801First.CAN_FAST == "1");
			var tmprType = (f050801First.TMPR_TYPE == "01") ? "A" : "B";
			//依各家配送商，取得各家配送時段
			var allIdDelvTimeAreas = GetAllIdDelvTimeAreas(dcCode, allIds, canFast, tmprType, f050801First.ZIP_CODE);

			if (!allIdDelvTimeAreas.Any())
			{
				var msgNo = "AAM00013"; //出貨單:{0}，找不到出貨時段!
				var f0020 = GetF0020(msgNo);
				var wmsOrdNos = f050801s.Select(a => a.WMS_ORD_NO).ToArray();
				var messageContent = string.Format(f0020.MSG_CONTENT, string.Join("、", wmsOrdNos));
				//寫入訊息池
				AddMessagePoolForInside("9", dcCode, f050801First.GUP_CODE, f050801First.CUST_CODE, msgNo, messageContent);
				return;
			}

			var minDelvDateTime = allIdDelvTimeAreas.Select(a => new { a.TakeDate, a.DelvTimeArea.DELV_TIME, a.DelvTimeArea.DELV_EFFIC, a.DelvTimeArea.SORT }).OrderBy(a => a.TakeDate).ThenBy(a => a.DELV_TIME).ThenBy(a => a.SORT).First();
			if (canFast)
			{
				//快速到貨，先取為快速到貨的時段
				var minFastDelvDateTime = allIdDelvTimeAreas.Where(a => a.CanFast).Select(a => new { a.TakeDate, a.DelvTimeArea.DELV_TIME, a.DelvTimeArea.DELV_EFFIC, a.DelvTimeArea.SORT }).OrderBy(a => a.TakeDate).ThenBy(a => a.DELV_TIME).ThenBy(a => a.SORT).FirstOrDefault();
				if (minFastDelvDateTime != null) //有取到快速到貨的時段，以快速到貨的時段，沒有則以一般的時段
					minDelvDateTime = minFastDelvDateTime;
			}

			var minAllIdDelvTimeAreas = allIdDelvTimeAreas.Where(a => a.TakeDate == minDelvDateTime.TakeDate && a.DelvTimeArea.DELV_TIME == minDelvDateTime.DELV_TIME).ToList();

			if (minAllIdDelvTimeAreas.Count == 1)
			{
				var wmsOrdTakeTimes = new List<WmsOrdTakeTime>();
				//最小出車時段只有一個時，不需再依計費取配送商
				var minAllIdDelvTimeAreaFirst = minAllIdDelvTimeAreas.First();
				wmsOrdTakeTimes = (from a in f050801s
													 select new WmsOrdTakeTime
													 {
														 F050801 = a,
														 TakeTime = minAllIdDelvTimeAreaFirst.DelvTimeArea.DELV_TIME,
														 CanFast = canFast,
														 TakeDate = minAllIdDelvTimeAreaFirst.TakeDate,
														 DelvEffic = minAllIdDelvTimeAreaFirst.DelvTimeArea.DELV_EFFIC,
														 DelvTimes = minAllIdDelvTimeAreaFirst.DelvTimeArea.DELV_TIMES
													 }).ToList();

				var allIdWmsOrdTakeTime = allIdWmsOrdTakeTimes.Where(a => a.AllId == minAllIdDelvTimeAreaFirst.AllId).SingleOrDefault();

				if (allIdWmsOrdTakeTime != null)
					allIdWmsOrdTakeTime.WmsOrdTakeTimes.AddRange(wmsOrdTakeTimes);
				else
					allIdWmsOrdTakeTimes.Add(new AllIdWmsOrdTakeTime { WmsOrdTakeTimes = wmsOrdTakeTimes, AllId = minAllIdDelvTimeAreaFirst.AllId }); //依配送商，分不同的 List
			}
			else
			{
				/* 如果有多個配送商的選擇可能，才延遲載入要計算的資料，載入一次後，之後就不會再重複載入*/
				// 取得所有出貨單的配送商與郵遞區號，一次Select出所有F194707要計算的費用
				var wholeAllIds = f050801Dic.Keys.Select(x => x.ALL_ID).Concat(allIds).Distinct().Where(x => x != null).ToList();
				var wholeZipCodes = f050801Dic.Keys.Select(x => x.ZIP_CODE).Distinct().ToList();
				LoadF194707WithF19470801s(dcCode, wholeAllIds, wholeZipCodes);

				// 載入F050801品項相關的尺寸、材積、重量資訊
				LoadF050801ItemsInfoDict(f050801Dic);

				//行事曆設有設假日，即為假日(B)，否則為平日(A)
				var accType = IsHoliday(dcCode, minAllIdDelvTimeAreas.First().TakeDate, "H") ? "B" : "A";

				var msgNo = "AAM00014"; //物流中心:「{0}」送商:「{1}」配送效率「{2}」溫層「{3}」計費類型「{4}」計費方式「{5}」單據類型「{6}」物流類別「{7}」郵遞區號「{8}」沒有設定配送費用。
				var f0020 = GetF0020(msgNo);
				//最小出車時段多個時依計費取得配送商
				foreach (var f050801 in f050801s)
				{
					var allIdDelvTimeArea = GetAllIdDelvTimeAreaByFee(f050801, minAllIdDelvTimeAreas, accType, isB2b);
					if (allIdDelvTimeArea == null)
					{
						var f1901 = GetF1901(dcCode);
						//寫入訊息池
						var msgs = new List<string>();
						foreach (var minAllIdDelvTimeArea in minAllIdDelvTimeAreas)
						{
							var f1947 = GetF1947(dcCode, minAllIdDelvTimeArea.AllId);
							var f000904Effic = GetF000904("F194707", "DELV_EFFIC", minAllIdDelvTimeArea.DelvTimeArea.DELV_EFFIC);
							var f000904Tmpr = GetF000904("F194707", "DELV_TMPR", minAllIdDelvTimeArea.DelvTimeArea.DELV_TMPR);
							var f000904AccType = GetF000904("F194707", "ACC_TYPE", accType);
							var f000904AccKind = GetF000904("F194707", "ACC_KIND", f1947.ACC_KIND);
							var f000904LogiType = GetF000904("F194707", "LOGI_TYPE", "01");
							msgs.Add(string.Format(f0020.MSG_CONTENT,
							(f1901 == null) ? "" : f1901.DC_NAME,
							(f1947 == null) ? "" : f1947.ALL_COMP,
							(f000904Effic == null) ? "" : f000904Effic.NAME,
							(f000904Tmpr == null) ? "" : f000904Tmpr.NAME,
							(f000904AccType == null) ? "" : f000904AccType.NAME,
							(f000904AccKind == null) ? "" : f000904AccKind.NAME,
							isB2b ? "B2B" : "B2C",
							(f000904LogiType == null) ? "" : f000904LogiType.NAME,
							minAllIdDelvTimeArea.DelvTimeArea.ZIP_CODE));
						}
						if (msgs.Any())
							AddMessagePoolForInside("9", dcCode, f050801.GUP_CODE, f050801.CUST_CODE, msgNo, string.Join("\n", msgs), false, true);
						allIdDelvTimeArea = minAllIdDelvTimeAreas.First();
					}
					var wmsOrdTakeTime = new WmsOrdTakeTime
					{
						F050801 = f050801,
						TakeTime = allIdDelvTimeArea.DelvTimeArea.DELV_TIME,
						CanFast = canFast,
						TakeDate = allIdDelvTimeArea.TakeDate,
						DelvEffic = allIdDelvTimeArea.DelvTimeArea.DELV_EFFIC,
						DelvTimes = allIdDelvTimeArea.DelvTimeArea.DELV_TIMES
					};

					var allIdWmsOrdTakeTime = allIdWmsOrdTakeTimes.Where(a => a.AllId == allIdDelvTimeArea.AllId).SingleOrDefault();

					if (allIdWmsOrdTakeTime != null)
						allIdWmsOrdTakeTime.WmsOrdTakeTimes.Add(wmsOrdTakeTime);
					else
						allIdWmsOrdTakeTimes.Add(new AllIdWmsOrdTakeTime
						{
							WmsOrdTakeTimes = new List<WmsOrdTakeTime> { wmsOrdTakeTime },
							AllId = allIdDelvTimeArea.AllId
						}); //依配送商，分不同的 List
				}
			}
		}


		/// <summary>
		/// 依各家配送商，取得各家配送時段
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="allIds"></param>
		/// <param name="canFast"></param>
		/// <param name="tmprType"></param>
		/// <param name="zipCode"></param>
		/// <returns></returns>
		private List<AllIdDelvTimeArea> GetAllIdDelvTimeAreas(string dcCode, List<string> allIds, bool canFast, string tmprType, string zipCode)
		{
			string maxDelvTime = null;
			var f0005Rep = new F0005Repository(Schemas.CoreSchema, _wmsTransaction);
			var pastMax = f0005Rep.Find(a => a.SET_NAME == "PAST_MAX" && a.DC_CODE == dcCode).SET_VALUE;
			int intPastMax;
			int.TryParse(pastMax, out intPastMax);
			string minDelvTime = DateTime.Now.AddMinutes(intPastMax).ToString("HH:mm");
			if (canFast)
			{
				//快速到貨，依物流中心設定的最大快速到貨時間，取最大時間
				var fastMax = f0005Rep.Find(a => a.SET_NAME == "FAST_MAX" && a.DC_CODE == dcCode).SET_VALUE;
				int intFastMax;
				int.TryParse(fastMax, out intFastMax);
				maxDelvTime = DateTime.Now.AddMinutes(intFastMax).ToString("HH:mm");
			}
			List<DelvTimeArea> delvTimeAreas;
			var allIdDelvTimeAreas = new List<AllIdDelvTimeArea>();
			foreach (var allId in allIds)
			{
				var takeDate = DateTime.Today;
				var tmpCanFast = canFast;
				delvTimeAreas = GetDelvTimeAreas(dcCode, allId, canFast, tmprType, zipCode, takeDate, minDelvTime, maxDelvTime);
				var hasFast = allIdDelvTimeAreas.Any(a => a.CanFast);
				//若為快速到貨，且尚未取到快速到貨時段，且於上限時間內沒取到時段，則取當天一般的時段
				if (canFast && !hasFast && !delvTimeAreas.Any())
				{
					tmpCanFast = false;
					delvTimeAreas = GetDelvTimeAreas(dcCode, allId, false, tmprType, zipCode, takeDate, minDelvTime);
				}
				//若為快速到貨，且已取到快速到貨時段，則跳過此段
				if (!(canFast && hasFast))
				{
					//若沒取到時段，取明後天一般的時段
					DateTime? minTakeDate = null;
					if (allIdDelvTimeAreas.Any())
						minTakeDate = allIdDelvTimeAreas.Min(a => a.TakeDate);
					var d = 0;
					while (!delvTimeAreas.Any() && d < 2)
					{
						d++;
						tmpCanFast = false;
						takeDate = DateTime.Today.AddDays(d);
						if (minTakeDate.HasValue && takeDate > minTakeDate.Value) //超過目前已取得最小出貨日，則跳出
							break;
						delvTimeAreas = GetDelvTimeAreas(dcCode, allId, false, tmprType, zipCode, takeDate, "00:00");
					};
				}

				if (!delvTimeAreas.Any())
					continue;
				var delvTimeArea = delvTimeAreas.OrderBy(a => a.DELV_TIME).ThenBy(a => a.SORT).First();
				allIdDelvTimeAreas.Add(new AllIdDelvTimeArea { AllId = allId, TakeDate = takeDate, CanFast = tmpCanFast, DelvTimeArea = delvTimeArea });
			}
			return allIdDelvTimeAreas;
		}

		/// <summary>
		/// 依照費用最低的配送商出車時段
		/// </summary>
		/// <param name="f050801"></param>
		/// <param name="allIdDelvTimeAreas"></param>
		/// <param name="accType"></param>
		/// <param name="isB2b"></param>
		/// <returns></returns>
		private AllIdDelvTimeArea GetAllIdDelvTimeAreaByFee(F050801 f050801, List<AllIdDelvTimeArea> allIdDelvTimeAreas, string accType, bool isB2b)
		{
			var custType = (isB2b) ? "0" : "1";

			var query = allIdDelvTimeAreas.Select(tmpAllIdDelvTimeArea =>
			{
				var delvTimeArea = tmpAllIdDelvTimeArea.DelvTimeArea;

				var f194707WithF19470801s = _f194707WithF19470801s.Where(x => x.DC_CODE == delvTimeArea.DC_CODE
																																					&& x.ALL_ID == delvTimeArea.ALL_ID
																																					&& x.DELV_EFFIC == delvTimeArea.DELV_EFFIC
																																					&& x.DELV_TMPR == delvTimeArea.DELV_TMPR
																																					&& x.CUST_TYPE == custType
																																					&& x.ZIP_CODE == delvTimeArea.ZIP_CODE
																																					&& x.ACC_TYPE == accType)
																																	.ToList();
							// 取得該配送商符合的所有計價方案後的最小費用
							decimal? minFee = null;
				if (f194707WithF19470801s.Any())
					minFee = f194707WithF19470801s.Select(f194707WithF19470801 => GetFee(f194707WithF19470801, f050801)).Min();

				return new
				{
					MinFee = minFee,
					AllIdDelvTimeArea = tmpAllIdDelvTimeArea
				};
			});

			// 回傳最小費用的配送商
			return query.ToList()
									.Where(x => x.MinFee.HasValue)
									.OrderBy(x => x.MinFee)
									.Select(x => x.AllIdDelvTimeArea)
									.FirstOrDefault();
		}

		/// <summary>
		/// 取得相同配送費用設定，但為重量計費方式
		/// </summary>
		/// <param name="f194707WithF19470801"></param>
		/// <returns></returns>
		private F194707WithF19470801 GetSameF194707WithF19470801ForWeight(F194707WithF19470801 f194707WithF19470801)
		{
			return _f194707WithF19470801s.Where(x => x.DC_CODE == f194707WithF19470801.DC_CODE
																							&& x.ALL_ID == f194707WithF19470801.ALL_ID
																							&& x.ACC_AREA_ID == f194707WithF19470801.ACC_AREA_ID
																							&& x.DELV_EFFIC == f194707WithF19470801.DELV_EFFIC
																							&& x.DELV_TMPR == f194707WithF19470801.DELV_TMPR
																							&& x.CUST_TYPE == f194707WithF19470801.CUST_TYPE
																							&& x.LOGI_TYPE == f194707WithF19470801.LOGI_TYPE
																							&& x.ACC_DELVNUM_ID == f194707WithF19470801.ACC_DELVNUM_ID
																							&& x.ACC_TYPE == f194707WithF19470801.ACC_TYPE
																							&& x.ACC_KIND == "E")   // 計費方式為重量
																			.OrderBy(x => x.OVER_UNIT_FEE)
																			.FirstOrDefault();
		}

		/// <summary>
		/// 取得費用
		/// </summary>
		/// <param name="f194707WithF19470801"></param>
		/// <param name="f050801"></param>
		/// <returns></returns>
		private decimal GetFee(F194707WithF19470801 f194707WithF19470801, F050801 f050801)
		{
			decimal overSize, overWeight;

			// 每一種計價都有基本費用
			decimal feeSum = f194707WithF19470801.FEE;

			// 接著算超出的費用
			var f050801ItemsInfo = _f050801ItemsInfoDict[f050801];
			switch (f194707WithF19470801.ACC_KIND)
			{
				case "C":   //計費方式(C:尺寸)
				case "D":   //計費方式(D:材積)
					overSize = (f194707WithF19470801.ACC_KIND == "C") ? f050801ItemsInfo.CubicCentimetre
																															: f050801ItemsInfo.Cuft;
					// 計算超尺寸的費用
					overSize -= f194707WithF19470801.BASIC_VALUE;
					if (overSize > 0 && f194707WithF19470801.OVER_VALUE.HasValue && f194707WithF19470801.OVER_UNIT_FEE.HasValue)
					{
						feeSum += (overSize / f194707WithF19470801.OVER_VALUE.Value) * f194707WithF19470801.OVER_UNIT_FEE.Value;
					}

					// 計算超重的費用
					overWeight = f050801ItemsInfo.Weight - f194707WithF19470801.MAX_WEIGHT;
					if (overWeight > 0)
					{
						// 有相同的計價設定條件才計算
						var f194707WithF19470801ForWeight = GetSameF194707WithF19470801ForWeight(f194707WithF19470801);
						if (f194707WithF19470801ForWeight != null && f194707WithF19470801ForWeight.OVER_VALUE.HasValue && f194707WithF19470801ForWeight.OVER_UNIT_FEE.HasValue)
						{
							feeSum += (overWeight / f194707WithF19470801ForWeight.OVER_VALUE.Value) * f194707WithF19470801.OVER_UNIT_FEE.Value;
						}
					}

					break;
				case "E":   //計費方式(E:重量)
					overWeight = f050801ItemsInfo.Weight - f194707WithF19470801.BASIC_VALUE;
					if (overWeight > 0 && f194707WithF19470801.OVER_VALUE.HasValue && f194707WithF19470801.OVER_UNIT_FEE.HasValue)
					{
						feeSum += (overWeight / f194707WithF19470801.OVER_VALUE.Value) * f194707WithF19470801.OVER_UNIT_FEE.Value;
					}
					break;
				case "F":   //計費方式(F:均一價)
										// 不用計算，就是一開始的 FEE
					break;
			}

			return feeSum;
		}

		/// <summary>
		/// 計算商品材積, 取回所符合的最小號箱子回來提示使用者
		/// </summary>
		private F1905 GetPackageByVolume(string dcCode, string gupCode, string custCode, List<F050802> f050802s)
		{
			// 商品總體積
			var unitVolume = GetTotalItemsVolume(dcCode, gupCode,custCode, f050802s);

			// 取得預設容積率
      if (_commonService == null)
      {
        _commonService = new CommonService();
      }

			var tmpBoxRate = _commonService.GetSysGlobalValue(dcCode, "BoxRate");
			decimal boxRate = 1;
			if (!decimal.TryParse(tmpBoxRate, out boxRate)) boxRate = 1;

			// 取得符合的紙箱大小
			var boxF1905s = GetBoxF1905s(gupCode, custCode);
			var tmpBox = boxF1905s.Where(x => x.PACK_HIGHT * x.PACK_LENGTH * x.PACK_WIDTH * boxRate >= unitVolume)
					.OrderBy(x => x.PACK_HIGHT * x.PACK_LENGTH * x.PACK_WIDTH).FirstOrDefault();
			// 找不到，表示超材，取最大紙箱
			if (tmpBox == null)
				tmpBox = boxF1905s.OrderByDescending(x => x.PACK_HIGHT * x.PACK_LENGTH * x.PACK_WIDTH).FirstOrDefault();

			return tmpBox;
		}

		private decimal GetTotalItemsSize(string dcCode, string gupCode,string custCode, List<F050802> f050802s)
		{
			var itemCodes = f050802s.Select(a => a.ITEM_CODE).Distinct().ToList();
			var f1905s = GetF1905s(gupCode, custCode, itemCodes);
			// 商品總尺寸
			var totalSize = (from a in f1905s
											 join b in f050802s on a.ITEM_CODE equals b.ITEM_CODE
											 select new { a, b })
																			.Sum(x => (x.a.PACK_HIGHT + x.a.PACK_LENGTH + x.a.PACK_WIDTH) * (x.b.ORD_QTY ?? 0));
			return totalSize;
		}

		private decimal GetTotalItemsVolume(string dcCode, string gupCode,string custCode, List<F050802> f050802s)
		{
			var itemCodes = f050802s.Select(a => a.ITEM_CODE).Distinct().ToList();
			var f1905s = GetF1905s(gupCode, custCode, itemCodes);
			// 商品總體積
			var totalVolume = (from a in f1905s
												 join b in f050802s on a.ITEM_CODE equals b.ITEM_CODE
												 select new { a, b })
																			.Sum(x => x.a.PACK_HIGHT * x.a.PACK_LENGTH * x.a.PACK_WIDTH * (x.b.ORD_QTY ?? 0));
			return totalVolume;
		}

		private decimal GetTotalItemsWeight(string gupCode,string custCode, List<F050802> f050802s)
		{
			var itemCodes = f050802s.Select(a => a.ITEM_CODE).Distinct().ToList();
			var f1905s = GetF1905s(gupCode, custCode, itemCodes);
			// 商品總重量
			var totalWeight = (from a in f1905s
												 join b in f050802s on a.ITEM_CODE equals b.ITEM_CODE
												 select new { a, b })
																			.Sum(x => x.a.PACK_WEIGHT * (x.b.ORD_QTY ?? 0));
			return totalWeight;
		}

		private decimal GetTotalItemsVolumn(string gupCode,string custCode, List<F050302> f050302s)
		{
			var itemCodes = f050302s.Select(a => a.ITEM_CODE).Distinct().ToList();
			var f1905s = GetF1905s(gupCode,custCode, itemCodes);
			// 商品總體積
			var totalVolume = (from a in f1905s
												 join b in f050302s on a.ITEM_CODE equals b.ITEM_CODE
												 select new { a, b })
																			.Sum(x => x.a.PACK_HIGHT * x.a.PACK_LENGTH * x.a.PACK_WIDTH * (x.b.ORD_QTY));
			return totalVolume;
		}

		private decimal GetTotalItemsWeight(string gupCode, string custCode, List<F050302> f050302s)
		{
			var itemCodes = f050302s.Select(a => a.ITEM_CODE).Distinct().ToList();
			var f1905s = GetF1905s(gupCode,custCode, itemCodes);
			// 商品總重量
			var totalWeight = (from a in f1905s
												 join b in f050302s on a.ITEM_CODE equals b.ITEM_CODE
												 select new { a, b })
																			.Sum(x => x.a.PACK_WEIGHT * (x.b.ORD_QTY));
			return totalWeight;
		}

		/// <summary>
		/// 計算 出貨單 的 各品項+效期 之 重量、整數箱、零數箱、材積
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="f050802s"></param>
		private void CalCuft(string dcCode, string gupCode, string custCode, List<F051202> f051202s)
		{
			var itemService = new ItemService();
			var f511001Repo = new F511001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f511002Repo = new F511002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1909 = GetF1909(gupCode, custCode);
			var itemUnits = itemService.GetItemUnitList(gupCode, f051202s.Select(x => x.ITEM_CODE).ToList());
			var f1905s = GetF1905s(gupCode,custCode, f051202s.Select(x => x.ITEM_CODE).Distinct().ToList());
			var f511001List = new List<F511001>();
			var f511002List = new List<F511002>();
			var group1 = from o in f051202s
									 group o by new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.WMS_ORD_NO, o.ITEM_CODE, o.VALID_DATE } into g
									 select g;
			var countItemFullAndBulkCaseQtyList = itemService.CountItemFullAndBulkCaseQty(gupCode, custCode, group1.Select(x => new ItemCodeQtyModel { ItemCode = x.Key.ITEM_CODE, Qty = x.Sum(z => z.B_PICK_QTY) }).ToList());
			foreach (var g in group1)
			{
				var curItemUnits = itemUnits.Where(x => x.ITEM_CODE == g.Key.ITEM_CODE).ToList();
				var f1905 = f1905s.FirstOrDefault();
				//計算商品重量
				decimal itemTotalWeight = 0;
				if (f1905 != null)
					itemTotalWeight = g.Sum(x => x.B_PICK_QTY) * f1905.PACK_WEIGHT;

				//計算整數箱、零數箱
				long fullCaseQty = 0;
				long bulkCaseQty = 0;
				var currFullBulk = countItemFullAndBulkCaseQtyList.Where(x => x.ItemCode == g.Key.ITEM_CODE).FirstOrDefault();
				if (currFullBulk != null)
				{
					fullCaseQty = currFullBulk.FullCaseQty;
					bulkCaseQty = currFullBulk.BulkCaseQty;
				}

				var f511002 = new F511002
				{
					DC_CODE = g.Key.DC_CODE,
					GUP_CODE = g.Key.GUP_CODE,
					CUST_CODE = g.Key.CUST_CODE,
					WMS_NO = g.Key.WMS_ORD_NO,
					ITEM_CODE = g.Key.ITEM_CODE,
					VALID_DATE = g.Key.VALID_DATE,
					QTY = g.Sum(x => x.B_PICK_QTY),
					//重量
					WEIGHT = itemTotalWeight,
					FULL_BOX_QTY = fullCaseQty,
					BULK_BOX_QTY = bulkCaseQty

				};

				f511002List.Add(f511002);
			}
			var group2 = from o in f511002List
									 group o by new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.WMS_NO, o.ITEM_CODE } into g
									 select g;
			foreach (var g in group2)
			{
				var curItemUnits = itemUnits.Where(x => x.ITEM_CODE == g.Key.ITEM_CODE).ToList();
				//計算整數箱、零數箱
				long fullCaseQty = 0;
				long bulkCaseQty = 0;
				var currFullBulk = countItemFullAndBulkCaseQtyList.Where(x => x.ItemCode == g.Key.ITEM_CODE).FirstOrDefault();
				if (currFullBulk != null)
				{
					fullCaseQty = currFullBulk.FullCaseQty;
					bulkCaseQty = currFullBulk.BulkCaseQty;
				}

				var f511001 = new F511001
				{
					DC_CODE = g.Key.DC_CODE,
					GUP_CODE = g.Key.GUP_CODE,
					CUST_CODE = g.Key.CUST_CODE,
					WMS_NO = g.Key.WMS_NO,
					ITEM_CODE = g.Key.ITEM_CODE,
					QTY = g.Sum(x => x.QTY),
					//材積係數(整數箱)
					CUFT_FACTOR = f1909.CUFT_FACTOR,
					//材積係數(零散箱)
					CUFT_BLUK = f1909.CUFT_BLUK,
					//重量
					WEIGHT = g.Sum(x => x.WEIGHT),
					FULL_BOX_QTY = fullCaseQty,
					BULK_BOX_QTY = bulkCaseQty,
					//計算單箱材積
					SINGLEBOX_VOLUME = itemService.GetVolume(g.Key.GUP_CODE, g.Key.CUST_CODE, g.Key.ITEM_CODE, f1909.CUFT_FACTOR)
				};
				f511001List.Add(f511001);
			}
			f511001Repo.BulkInsert(f511001List);
			f511002Repo.BulkInsert(f511002List);
		}


		private void CreateConsign(List<AllIdWmsOrdTakeTime> allIdWmsOrdTakeTimes, List<F050301> f050301s, List<F05030101> f05030101s)
		{
			List<F050901> bulkInsertF050901List = new List<F050901>();

			foreach (var allIdWmsOrdTakeTime in allIdWmsOrdTakeTimes)
			{
				foreach (var wmsOrdTakeTime in allIdWmsOrdTakeTime.WmsOrdTakeTimes)
				{
					var f050301 = (from a in f050301s
												 join b in f05030101s on a.ORD_NO equals b.ORD_NO
												 where b.WMS_ORD_NO == wmsOrdTakeTime.F050801.WMS_ORD_NO
												 select a).First();
					wmsOrdTakeTime.Address = (!string.IsNullOrEmpty(f050301.ADDRESS_PARSE)) ? f050301.ADDRESS_PARSE : f050301.ADDRESS;
					wmsOrdTakeTime.OrdNo = f050301.ORD_NO;
				}

				var datas = allIdWmsOrdTakeTime.WmsOrdTakeTimes.GroupBy(o => new { o.F050801.DC_CODE, o.F050801.GUP_CODE, o.F050801.CUST_CODE, o.F050801.ALL_ID, o.F050801.COLLECT_AMT, o.OrdNo });
				var allOrdAddresses = new List<OrdAddress>();
				foreach (var item in datas)
				{


					var ordAddresses = item.Select(a => new OrdAddress
					{
						WmsNo = a.F050801.WMS_ORD_NO,
						Address = a.Address,
						ZipCode = a.F050801.ZIP_CODE,
						DelvTimes = a.DelvTimes,
						DistrUse = "01" // 送件:01
					}).ToList();

					var f194704 = GetF194704(item.Key.DC_CODE, item.Key.GUP_CODE, item.Key.CUST_CODE, item.Key.ALL_ID);
					//如果此配送商為外部取號 則不產生託運單
					if (f194704 != null && f194704.GET_CONSIGN_NO == "3")
					{
						_consignService.GetOrdRoutes(item.Key.DC_CODE, item.Key.ALL_ID, ordAddresses);
						allOrdAddresses.AddRange(ordAddresses);
						continue;
					}
					_consignService.CreateConsign(item.Key.DC_CODE,
																			item.Key.GUP_CODE,
																			item.Key.CUST_CODE,
																			item.Key.ALL_ID,
																			ordAddresses,
																			item.Key.COLLECT_AMT.ToString(), item.Key.OrdNo);
					if (ordAddresses.Any(x => !string.IsNullOrWhiteSpace(x.ErrorMessage)))
					{
						var msgNo = "AAM00020"; //訂單編號{0}託運單數量不足，原因{1}
						var f0020 = GetF0020(msgNo);
						var messageContent = string.Format(f0020.MSG_CONTENT, item.Key.OrdNo, string.Join(Environment.NewLine, ordAddresses.Select(x => x.ErrorMessage)));
						AddMessagePoolForInside("9", item.Key.DC_CODE, item.Key.GUP_CODE, item.Key.CUST_CODE, msgNo, messageContent, false, true);
					}
					else
					{
						bulkInsertF050901List.AddRange(ordAddresses.Where(x => x.AddBoxGetConsignNo == "0").Select(x => x.F050901));
						var noRouteWmsOrdNos = (from a in item
																		join b in ordAddresses on a.F050801.WMS_ORD_NO equals b.WmsNo
																		where !b.HasFindZipCodeWithDelvTimes
																		select a.F050801.WMS_ORD_NO).ToList();

						if (noRouteWmsOrdNos.Any())
						{
							var msgNo = "AAM00015"; //單據：「{0}」找不到路線
							var f0020 = GetF0020(msgNo);
							var messageContent = string.Format(f0020.MSG_CONTENT, string.Join("、", noRouteWmsOrdNos));
							AddMessagePoolForInside("9", item.Key.DC_CODE, item.Key.GUP_CODE, item.Key.CUST_CODE, msgNo, messageContent, false, true);
						}
						allOrdAddresses.AddRange(ordAddresses);
					}
				}
				//只取有產生託運單的出貨單
				allIdWmsOrdTakeTime.WmsOrdTakeTimes = (from a in allIdWmsOrdTakeTime.WmsOrdTakeTimes
																							 join b in allOrdAddresses on a.F050801.WMS_ORD_NO equals b.WmsNo
																							 where b.HasFindZipCodeWithDelvTimes
																							 select a).ToList();
			}

			if (bulkInsertF050901List.Any())
				_f050901Rep.BulkInsert(bulkInsertF050901List, "CONSIGN_ID");
		}
		#endregion 派車

		#region 訊息池


		/// <summary>
		/// 新增內部(DC)的訊息池
		/// </summary>
		/// <param name="ticketType"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="messageContent"></param>
		private void AddMessagePoolForInside(string ticketType, string dcCode, string gupCode, string custCode, string msgNo, string messageContent, bool isNoTransaction = false, bool isSuccess = false)
		{
			_sharedService.AddMessagePool(ticketType, dcCode, gupCode, custCode, msgNo, messageContent, _notifyOrdNo, "0", "AA", isNoTransaction);
			_exeResults.Add(new ExecuteResult { IsSuccessed = isSuccess, Message = messageContent });
		}

		/// <summary>
		/// 新增貨主的訊息池
		/// </summary>
		/// <param name="ticketType"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="msgNo"></param>
		/// <param name="messageContent"></param>
		/// <param name="isNoTransaction"></param>
		/// <param name="isSuccess"></param>
		private void AddMessagePoolForCust(string ticketType, string dcCode, string gupCode, string custCode, string msgNo, string messageContent, bool addExeResult, bool isNoTransaction = false, bool isSuccess = false)
		{
			_sharedService.AddMessagePool(ticketType, dcCode, gupCode, custCode, msgNo, messageContent, _notifyOrdNo, "1", custCode, isNoTransaction);
			if (addExeResult)
				_exeResults.Add(new ExecuteResult { IsSuccessed = isSuccess, Message = messageContent });
		}
		#endregion 訊息池
	}

}