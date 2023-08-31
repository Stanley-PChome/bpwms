using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
	public class CommonService
	{

		#region Cache
		/// <summary>
		/// Cache_物流中心資料清單
		/// </summary>
		private List<F1901> _f1901CacheList;

		/// <summary>
		/// Cache_貨主資料清單
		/// </summary>
		private List<F1909> _f1909CacheList;

		/// <summary>
		/// Cache_儲位資料清單
		/// </summary>
		private List<F1912> _f1912CacheList;

		/// <summary>
		/// Cache_業主資料清單
		/// </summary>
		private List<F1929> _f1929CacheList;

		/// <summary>
		/// Cache_廠商資料清單
		/// </summary>
		private List<F1908> _f1908CacheList;

		/// <summary>
		/// Cache_門市資料清單
		/// </summary>
		private List<F1910> _f1910CacheList;

		/// <summary>
		/// Cache_商品資料清單
		/// </summary>
		private List<CommonProduct> _f1903CacheList;

		/// <summary>
		/// Cache_選單資料清單
		/// </summary>
		private List<F000904> _f000904CacheList;

		/// <summary>
		/// Cache_訊息內容清單
		/// </summary>
		private List<F0020> _f0020List;

		/// <summary>
		/// Cache_系統設定清單
		/// </summary>
		private List<F0003> _f0003List;

		/// <summary>
		/// Cache_超取服務商清單
		/// </summary>
		private List<F194713> _f194713List;

		/// <summary>
		/// Cache 倉庫資料清單
		/// </summary>
		private List<F1980> _f1980CacheList;

		/// <summary>
		/// 商品材積檔
		/// </summary>
		private List<F1905> _f1905CacheList;
		/// <summary>
		/// Cache 商品階層檔
		/// </summary>
		private List<F190301WithF91000302> _tmepF190301s;

		/// <summary>
		/// Cache 倉別類型檔
		/// </summary>
		private List<F198001> _f198001CacheList;
		/// <summary>
		/// Cache 商品序號清單
		/// </summary>
		private List<F2501> _f2501CacheLList;

		/// <summary>
		/// Cache 已找過DB但不存在DB的序號清單，避免重複找DB
		/// </summary>
		private List<NotDbFindSerial> _notDbFindSerialCacheList;
    #endregion

		private F1901Repository _f1901Repo;
		public F1901Repository F1901Repo
		{
			get { return _f1901Repo == null ? _f1901Repo = new F1901Repository(Schemas.CoreSchema) : _f1901Repo; }
			set { _f1901Repo = value; }
		}

		private F1909Repository _f1909Repo;
		public F1909Repository F1909Repo
		{
			get { return _f1909Repo == null ? _f1909Repo = new F1909Repository(Schemas.CoreSchema) : _f1909Repo; }
			set { _f1909Repo = value; }
		}

		private F0003Repository _f0003Repo;
		public F0003Repository F0003Repo
		{
			get { return _f0003Repo == null ? _f0003Repo = new F0003Repository(Schemas.CoreSchema) : _f0003Repo; }
			set { _f0003Repo = value; }
		}

		private F0020Repository _f0020Repo;
		public F0020Repository F0020Repo
		{
			get { return _f0020Repo == null ? _f0020Repo = new F0020Repository(Schemas.CoreSchema) : _f0020Repo; }
			set { _f0020Repo = value; }
		}

		/// <summary>
		/// 取得物流中心資料
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <returns></returns>
		public F1901 GetDc(string dcCode)
		{
			if (_f1901CacheList == null)
				_f1901CacheList = new List<F1901>();

			var f1901 = _f1901CacheList.FirstOrDefault(x => x.DC_CODE == dcCode);

			if (f1901 == null)
			{
				f1901 = F1901Repo.Find(x => x.DC_CODE == dcCode);

				if (f1901 != null)
					_f1901CacheList.Add(f1901);
			}
			return f1901;
		}

		public List<string> GetDcCodeList(string dcCode)
		{
			return F1901Repo.GetDcCodesByDcCode(dcCode);
		}

		/// <summary>
		/// 取得業主資料
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <returns></returns>
		public F1929 GetGup(string gupCode)
		{
			if (_f1929CacheList == null)
				_f1929CacheList = new List<F1929>();

			var f1929 = _f1929CacheList.FirstOrDefault(x => x.GUP_CODE == gupCode);

			if (f1929 == null)
			{
				var f1929Repo = new F1929Repository(Schemas.CoreSchema);

				f1929 = f1929Repo.Find(x => x.GUP_CODE == gupCode);

				if (f1929 != null)
					_f1929CacheList.Add(f1929);
			}
			return f1929;
		}

		/// <summary>
		/// 取得貨主資料
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">業主編號</param>
		/// <returns></returns>
		public F1909 GetCust(string gupCode, string custCode)
		{
			if (_f1909CacheList == null)
				_f1909CacheList = new List<F1909>();

			var f1909 = _f1909CacheList.FirstOrDefault(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);

			if (f1909 == null)
			{
				f1909 = F1909Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);

				if (f1909 != null)
					_f1909CacheList.Add(f1909);
			}
			return f1909;
		}

		/// <summary>
		/// 取得儲位資料
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="locCode">儲位編號</param>
		/// <returns></returns>
		public F1912 GetLoc(string dcCode, string locCode)
		{
			if (_f1912CacheList == null)
				_f1912CacheList = new List<F1912>();

			var f1912 = _f1912CacheList.FirstOrDefault(x => x.DC_CODE == dcCode && x.LOC_CODE == locCode);

			if (f1912 == null)
			{
				var f1912Repo = new F1912Repository(Schemas.CoreSchema);

				f1912 = f1912Repo.Find(x => x.DC_CODE == dcCode && x.LOC_CODE == locCode);

				if (f1912 != null)
					_f1912CacheList.Add(f1912);
			}
			return f1912;
		}

		/// <summary>
		/// 取得交易類型資料
		/// </summary>
		/// <param name="tag">異動類型類別代碼</param>
		/// <returns></returns>
		public List<F000903> GetTranCodeList(string tag)
		{
			var f000903Repo = new F000903Repository(Schemas.CoreSchema);

			var result = f000903Repo.GetDatasStartsWithOrdProp(tag).ToList();

			return result;
		}

		/// <summary>
		/// 取得廠商資料清單(多筆)
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="vnrCodeList">廠商編號清單</param>
		/// <returns></returns>
		public List<F1908> GetVnrList(string gupCode, string custCode, List<string> vnrCodeList)
		{
			var list = new List<F1908>();

			if (_f1908CacheList == null)
				_f1908CacheList = new List<F1908>();

			var datas = _f1908CacheList.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && vnrCodeList.Contains(x.VNR_CODE)).ToList();

			list.AddRange(datas);

			var existsVnrCode = datas.Select(x => x.VNR_CODE).Distinct().ToList();

			var noExistsVnrCode = vnrCodeList.Except(existsVnrCode).ToList();

			if (noExistsVnrCode.Any())
			{
				var f1908Repo = new F1908Repository(Schemas.CoreSchema);

				var data = f1908Repo.GetDatas(gupCode, custCode, vnrCodeList).ToList();

				_f1908CacheList.AddRange(data);

				list.AddRange(data);
			}
			return list;
		}

		/// <summary>
		/// 取得廠商資料(單筆)
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="vnrCode">廠商編號</param>
		/// <returns></returns>
		public F1908 GetVnr(string gupCode, string custCode, string vnrCode)
		{
			return GetVnrList(gupCode, custCode, new List<string> { vnrCode }).FirstOrDefault();
		}

		/// <summary>
		/// 取得門市資料清單
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="retailCodeList">門市編號清單</param>
		/// <returns></returns>
		public List<F1910> GetRetailList(string gupCode, string custCode, List<string> retailCodeList)
		{
			var list = new List<F1910>();

			if (_f1910CacheList == null)
				_f1910CacheList = new List<F1910>();

			var datas = _f1910CacheList.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && retailCodeList.Contains(x.RETAIL_CODE)).ToList();

			list.AddRange(datas);

			var existsRetailCode = datas.Select(x => x.RETAIL_CODE).Distinct().ToList();

			var noExistsRetail = retailCodeList.Except(existsRetailCode).ToList();

			if (noExistsRetail.Any())
			{
				var f1910Repo = new F1910Repository(Schemas.CoreSchema);

				var data = f1910Repo.GetDatas(gupCode, custCode, retailCodeList).ToList();

				_f1910CacheList.AddRange(data);

				list.AddRange(data);
			}
			return list;
		}

		/// <summary>
		/// 取得門市資料(單筆)
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="retailCode">門市編號</param>
		/// <returns></returns>
		public F1910 GetRetail(string gupCode, string custCode, string retailCode)
		{
			return GetRetailList(gupCode, custCode, new List<string> { retailCode }).FirstOrDefault();
		}

		/// <summary>
		/// 取得商品資料清單
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="itemCodeList">商品編號清單</param>
		/// <returns></returns>
		public List<CommonProduct> GetProductList(string gupCode, string custCode, List<string> itemCodeList)
		{

			var list = new List<CommonProduct>();

			if (_f1903CacheList == null)
				_f1903CacheList = new List<CommonProduct>();

			// 如果商品編號清單筆數超過1000 則批次1000筆取回資料
			int range = 1000;

			IEnumerable<string> itemCodes = itemCodeList.Distinct();

			int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(itemCodes.Count()) / range));

			for (int i = 0; i < index; i++)
			{
				var currItemCodes = itemCodes.Skip(i * range).Take(range).ToList();

				var datas = _f1903CacheList.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && currItemCodes.Contains(x.ITEM_CODE)).ToList();

				list.AddRange(datas);

				var existsItemCode = datas.Select(x => x.ITEM_CODE).Distinct().ToList();

				var noExistsItemCode = currItemCodes.Except(existsItemCode).ToList();

				if (noExistsItemCode.Any())
				{
					var f1903Repo = new F1903Repository(Schemas.CoreSchema);

					var currData = f1903Repo.GetCommonProductsByItemCodes(gupCode, custCode, noExistsItemCode).ToList();

					_f1903CacheList.AddRange(currData);

					list.AddRange(currData);
				}
			}

			return list;
		}

		/// <summary>
		/// 取得商品資料(單筆)
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="itemCode">商品編號</param>
		/// <returns></returns>
		public CommonProduct GetProduct(string gupCode, string custCode, string itemCode)
		{
			return GetProductList(gupCode, custCode, new List<string> { itemCode }).FirstOrDefault();
		}

		/// <summary>
		/// 取得訊息內容
		/// </summary>
		/// <param name="msgNo">訊息代碼</param>
		/// <returns></returns>
		public string GetMsg(string msgNo)
		{
			if (msgNo == "API20057")
				return "主機授權碼錯誤";

			if (_f0020List == null)
				_f0020List = new List<F0020>();

			var f0020 = _f0020List.FirstOrDefault(x => x.MSG_NO == msgNo);

			if (f0020 == null)
			{
				f0020 = F0020Repo.Find(x => x.MSG_NO == msgNo);

				if (f0020 != null)
					_f0020List.Add(f0020);
			}
			return f0020 != null ? f0020.MSG_CONTENT : string.Empty;
		}

		/// <summary>
		/// 取得系統設定值
		/// </summary>
		/// <param name="apName">設定名稱</param>
		/// <returns></returns>
		public string GetSysGlobalValue(string apName)
		{
			if (_f0003List == null)
				_f0003List = new List<F0003>();

			var f0003 = _f0003List.FirstOrDefault(x => x.DC_CODE == "00" && x.GUP_CODE == "00" && x.CUST_CODE == "00" && x.AP_NAME == apName);

			if (f0003 == null)
			{
				f0003 = F0003Repo.Find(o => o.DC_CODE == "00" &&
																		o.GUP_CODE == "00" &&
																		o.CUST_CODE == "00" &&
																		o.AP_NAME == apName);

				if (f0003 != null)
					_f0003List.Add(f0003);
			}
			return f0003 != null ? f0003.SYS_PATH : string.Empty;
		}

		/// <summary>
		/// 取得系統設定值
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="apName"></param>
		/// <returns></returns>
		public string GetSysGlobalValue(string dcCode, string gupCode, string custCode, string apName)
		{
			if (_f0003List == null)
				_f0003List = new List<F0003>();

			var f0003 = _f0003List.FirstOrDefault(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.AP_NAME == apName);
			if (f0003 == null)
			{
				f0003 = F0003Repo.Find(o => o.DC_CODE == dcCode &&
																	 o.GUP_CODE == gupCode &&
																	 o.CUST_CODE == custCode &&
																	 o.AP_NAME == apName);
				if (f0003 != null)
					_f0003List.Add(f0003);
			}
			if (f0003 == null)
				return GetSysGlobalValue(apName);

			return f0003 != null ? f0003.SYS_PATH : string.Empty;
		}

		/// <summary>
		/// 取得系統設定值
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="apName"></param>
		/// <returns></returns>
		public string GetSysGlobalValue(string dcCode, string apName)
		{
			if (_f0003List == null)
				_f0003List = new List<F0003>();

			var f0003 = _f0003List.FirstOrDefault(x => x.DC_CODE == dcCode && x.GUP_CODE == "00" && x.CUST_CODE == "00" && x.AP_NAME == apName);

			if (f0003 == null)
			{
				f0003 = F0003Repo.Find(o => o.DC_CODE == dcCode &&
																		o.GUP_CODE == "00" &&
																		o.CUST_CODE == "00" &&
																		o.AP_NAME == apName);

				if (f0003 != null)
					_f0003List.Add(f0003);
			}
			return f0003 != null ? f0003.SYS_PATH : string.Empty;

		}

    /// <summary>
    /// 取得業主編號
    /// </summary>
    /// <param name="custCode">貨主編號</param>
    /// <returns></returns>
    public string GetGupCode(string custCode)
    {
      if (string.IsNullOrWhiteSpace(custCode))
      {
        return null;
      }

      if (_f1909CacheList == null)
        _f1909CacheList = new List<F1909>();

      var f1909 = _f1909CacheList.FirstOrDefault(x => x.CUST_CODE == custCode);

      if (f1909 == null)
      {
        f1909 = F1909Repo.Find(x => x.CUST_CODE == custCode);

        if (f1909 != null)
          _f1909CacheList.Add(f1909);
      }

      return f1909 != null ? f1909.GUP_CODE : string.Empty;
		}

    /// <summary>
    /// 檢核物流中心是否存在
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <returns></returns>
    public bool CheckDcExist(string dcCode)
		{
			var dc = GetDc(dcCode);
			return dc != null;
		}

		/// <summary>
		/// 檢核貨主是否存在
		/// </summary>
		/// <param name="custCode">貨主編號</param>
		/// <returns></returns>
		public bool CheckCustExist(string custCode)
		{
			var gupCode = GetGupCode(custCode);
			var cust = GetCust(gupCode, custCode);
			return cust != null;
		}

		/// <summary>
		/// 檢核儲位編號是否存在
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="locCode">儲位編號</param>
		/// <returns></returns>
		public bool CheckLocExist(string dcCode, string locCode)
		{
			var loc = GetLoc(dcCode, locCode);
			return loc != null;
		}

		/// <summary>
		/// 檢核品號是否存在
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public bool CheckItemCodeExist(string gupCode, string custCode, string itemCode)
		{
			var f1903 = GetProduct(gupCode, custCode, itemCode);
			return f1903 != null;
		}

		/// <summary>
		/// 檢核門市編號是否存在
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="retailCode">門市編號 (同為客戶編號)</param>
		/// <returns></returns>
		public bool CheckRetailCodeExist(string gupCode, string custCode, string retailCode)
		{
			return GetRetail(gupCode, custCode, retailCode) != null;
		}

		/// <summary>
		/// 取得超取服務商資料
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="allIdList">配送商編號</param>
		/// <param name="eserviceList">配送商編號清單</param>
		/// <returns></returns>
		public List<F194713> GetEServiceList(string dcCode, string gupCode, string custCode, List<string> allIdList, List<string> eserviceList)
		{
			var list = new List<F194713>();

			if (_f194713List == null)
				_f194713List = new List<F194713>();

			var datas = _f194713List.Where(x => allIdList.Contains(x.ALL_ID) && eserviceList.Contains(x.ESERVICE)).ToList();

			list.AddRange(datas);

			var existsData = datas.Select(x => new { x.ALL_ID, x.ESERVICE }).Distinct().ToList();

			var noExistsData = existsData.Where(x => !allIdList.Contains(x.ALL_ID) && !eserviceList.Contains(x.ESERVICE));

			if (!_f194713List.Any() || noExistsData.Any())
			{
				var f194713Repo = new F194713Repository(Schemas.CoreSchema);

				var data = f194713Repo.GetDatas(dcCode, gupCode, custCode, allIdList, eserviceList).ToList();

				_f194713List.AddRange(data);

				list.AddRange(data);
			}
			return list;
		}

		/// <summary>
		/// 取得出貨倉別資料
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="warehouseIdList">倉別清單</param>
		/// <returns></returns>
		public List<F1980> GetWarhouseList(string dcCode, List<string> warehouseTypeList)
		{
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			return f1980Repo.GetDatasForWarehouseTypes(dcCode, warehouseTypeList).ToList();
		}

		/// <summary>
		/// 取得物流中心服務貨主檔
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <returns></returns>
		public List<GetDcCustRes> GetDcCustList(string dcCode, string gupCode, string custCode)
		{
			var f190101Repo = new F190101Repository(Schemas.CoreSchema);

			var data = f190101Repo.GetDcCustList(dcCode, gupCode, custCode).Select(x => new GetDcCustRes { CUST_CODE = x.CUST_CODE, DC_CODE = x.DC_CODE, GUP_CODE = x.GUP_CODE });

			return data.ToList();
		}

		/// <summary>
		/// 取得配送商資料
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <returns></returns>
		public List<F1947> GetLogisticProviderList(string dcCode)
		{
			var f1947Repo = new F1947Repository(Schemas.CoreSchema);

			var data = f1947Repo.GetDatas(dcCode);

			return data.ToList();
		}

		/// <summary>
		/// 檢核是否在倉庫資料中
		/// </summary>
		/// <returns></returns>
		public bool CheckZoneCodeExist(string dcCode, string warehouseId)
		{
			return GetWarehouse(dcCode, warehouseId) != null;
		}

		public string GetEmpName(string empId)
		{
			var f1924Repository = new F1924Repository(Schemas.CoreSchema);
			var empName = f1924Repository.GetEmpName(empId);
			return empName;
		}

		public List<F1912> GetLocListUseSplit(String dcCode, List<string> LocList)
		{
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			var result = new List<F1912>();
			int chunkSize = 1000;
			var splits = new List<List<string>>();
			var sourceCount = LocList.Count;
			for (var i = 0; i < sourceCount; i += chunkSize)
			{
				splits.Add(LocList.GetRange(i, Math.Min(chunkSize, sourceCount - i)));
			}

			foreach (var item in splits)
				result.AddRange(f1912Repo.GetF1912DataSQL(dcCode, item));

			return result;
		}

		public List<F2501> GetItemSerialList(string gupCode, string custCode, List<string> serialNos)
		{
			if (_f2501CacheLList == null)
				_f2501CacheLList = new List<F2501>();
			if (_notDbFindSerialCacheList == null)
				_notDbFindSerialCacheList = new List<NotDbFindSerial>();

			// 排除已經找過DB且不存在DB的序號，降低DB重複撈資料
			var existNotDbFindSnList = _notDbFindSerialCacheList.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && serialNos.Contains(x.SERIAL_NO)).Select(x => x.SERIAL_NO).ToList();
			serialNos = serialNos.Except(existNotDbFindSnList).ToList();

			if (!serialNos.Any())
				return new List<F2501>();

			var list = new List<F2501>();
			
			int pageSize = 1000;
			serialNos = serialNos.Distinct().ToList();
			var existsSerialNos = _f2501CacheLList.Select(x => x.SERIAL_NO).Distinct().ToList();
			var findSerialNos = serialNos.Except(existsSerialNos).ToList();
			if(findSerialNos.Any())
			{
				var f2501Repo = new F2501Repository(Schemas.CoreSchema);
				int page = Convert.ToInt32(Math.Ceiling((decimal)findSerialNos.Count() / pageSize));
				for (int i = 0; i < page; i++)
				{
					var currSerialNos = findSerialNos.Skip(i * pageSize).Take(pageSize).ToList();

					var f2501s = f2501Repo.GetDatas(gupCode, custCode, currSerialNos).ToList();
					// 不存在DB的序號，加入已找過DB但不存在DB的序號清單
					_notDbFindSerialCacheList.AddRange(currSerialNos.Except(f2501s.Select(x => x.SERIAL_NO)).Select(x => new NotDbFindSerial { GUP_CODE = gupCode, CUST_CODE = custCode, SERIAL_NO = x }));
					_f2501CacheLList.AddRange(f2501s);
				}
			}
		
			return _f2501CacheLList.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && serialNos.Contains(x.SERIAL_NO)).ToList();
		}

		public bool CheckSorterCodeExist(string storterCode)
		{
			// 先不檢查StortCode，目前只做紀錄
			return true;
		}

		public bool CheckAbnormalTypeExist(string abnormalType)
		{
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var abnormalTypeList = f000904Repo.GetF000904Data("SorterAbnormalNotify", "AbnormalType").Select(x => x.VALUE).ToList();
			return abnormalTypeList.Contains(abnormalType.ToString());

		}

		/// <summary>
		/// 取得倉別資料
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="warehouseId">倉庫編號</param>
		/// <returns></returns>
		public F1980 GetWarehouse(string dcCode, string warehouseId)
		{
			if (_f1980CacheList == null)
				_f1980CacheList = new List<F1980>();

			var f1980 = _f1980CacheList.FirstOrDefault(x => x.DC_CODE == dcCode && x.WAREHOUSE_ID == warehouseId);

			if (f1980 == null)
			{
				var f1980Repo = new F1980Repository(Schemas.CoreSchema);

				f1980 = f1980Repo.Find(x => x.DC_CODE == dcCode && x.WAREHOUSE_ID == warehouseId);

				if (f1980 != null)
					_f1980CacheList.Add(f1980);
			}
			return f1980;
		}
		public F1912 GetWarehouseFirstLoc(string dcCode, string warehouseId)
		{
			if (_f1912CacheList == null)
				_f1912CacheList = new List<F1912>();

			var f1912 = _f1912CacheList.FirstOrDefault(x => x.DC_CODE == dcCode && x.WAREHOUSE_ID == warehouseId);
			if (f1912 == null)
			{
				var f1912Repo = new F1912Repository(Schemas.CoreSchema);
				f1912 = f1912Repo.GetFirstLocByWarehouseId(dcCode, warehouseId);
				if (f1912 != null)
					_f1912CacheList.Add(f1912);
			}
			return f1912;
		}

		/// <summary>
		/// 取得商品資料清單
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="itemCodeList">商品編號清單</param>
		/// <returns></returns>
		public List<F1905> GetProductSizeList(string gupCode, string custCode, List<string> itemCodeList)
		{
			
			var list = new List<F1905>();

			if (_f1905CacheList == null)
				_f1905CacheList = new List<F1905>();

			// 如果商品編號清單筆數超過1000 則批次1000筆取回資料
			int range = 1000;

			IEnumerable<string> itemCodes = itemCodeList.Distinct();

			int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(itemCodes.Count()) / range));

			for (int i = 0; i < index; i++)
			{
				var currItemCodes = itemCodes.Skip(i * range).Take(range).ToList();

				var datas = _f1905CacheList.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && currItemCodes.Contains(x.ITEM_CODE)).ToList();

				list.AddRange(datas);

				var existsItemCode = datas.Select(x => x.ITEM_CODE).Distinct().ToList();

				var noExistsItemCode = currItemCodes.Except(existsItemCode).ToList();

				if (noExistsItemCode.Any())
				{
					var f1905Repo = new F1905Repository(Schemas.CoreSchema);
					var currData = f1905Repo.GetF1905ByItemCodes(gupCode, custCode, noExistsItemCode).ToList();

					_f1905CacheList.AddRange(currData);

					list.AddRange(currData);
				}
			}

			return list;
		}

		/// <summary>
		/// 取得商品資料(單筆)
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="itemCode">商品編號</param>
		/// <returns></returns>
		public F1905 GetProductSize(string gupCode, string custCode, string itemCode)
		{
			return GetProductSizeList(gupCode, custCode, new List<string> { itemCode }).FirstOrDefault();
		}

		/// <summary>
		/// 取得商品階層檔
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="itemCodes">品號清單</param>
		/// <param name="accUnitNameList">指定階層名稱清單</param>
		/// <returns></returns>
		public List<F190301WithF91000302> GetF190301WithF91000302s(string gupCode, List<string> itemCodes, List<string> accUnitNameList = null)
		{
			if (_tmepF190301s == null)
				_tmepF190301s = new List<F190301WithF91000302>();

			var findF190301s = new List<F190301WithF91000302>();
			var existItems = _tmepF190301s.Where(x => x.GUP_CODE == gupCode && itemCodes.Any(y => y == x.ITEM_CODE));
			findF190301s.AddRange(existItems);
			var noExistItemCodes = itemCodes.Except(existItems.Select(x => x.ITEM_CODE)).ToList();
			if (noExistItemCodes.Any())
			{
				var f190301Repo = new F190301Repository(Schemas.CoreSchema);
				findF190301s.AddRange(f190301Repo.GetUnitQtyDatas(gupCode, accUnitNameList, noExistItemCodes));
			}
			return findF190301s;
		}

		/// <summary>
		/// 是否為自動倉
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="warehouseId"></param>
		/// <returns></returns>
		public bool IsAutoWarehouse(string dcCode, string warehouseId)
		{
			var f1980 = GetWarehouse(dcCode, warehouseId);
			if (f1980 == null)
				return false;

			return string.IsNullOrWhiteSpace(f1980.DEVICE_TYPE) ? false : f1980.DEVICE_TYPE != "0";
		}
		public List<F000904> GetF000904s(string topic, string subtopic, List<string> valueList = null)
		{

			if (_f000904CacheList == null)
				_f000904CacheList = new List<F000904>();

			var datas = _f000904CacheList.Where(x => x.TOPIC == topic && x.SUBTOPIC == subtopic).ToList();
			if (!datas.Any())
			{
				var f000904Repo = new F000904Repository(Schemas.CoreSchema);
				datas = f000904Repo.GetDatas(topic, subtopic).ToList();
				_f000904CacheList.AddRange(datas);
			}

			if (valueList != null && valueList.Any())
			{
				return datas.Where(x => valueList.Contains(x.VALUE)).ToList();
			}
			return datas;
		}
		public F000904 GetF000904(string topic, string subtopic, string value)
		{
			return GetF000904s(topic, subtopic, new List<string> { value }).FirstOrDefault();
		}

		public F198001 GetF198001(string warehouseType)
		{
			if (_f198001CacheList == null)
				_f198001CacheList = new List<F198001>();

			var item = _f198001CacheList.FirstOrDefault(x => x.TYPE_ID == warehouseType);
			if (item == null)
			{
				var f198001Repo = new F198001Repository(Schemas.CoreSchema);
				item = f198001Repo.GetDatasByTrueAndCondition(x => x.TYPE_ID == warehouseType).FirstOrDefault();
				_f198001CacheList.Add(item);
			}
			return item;
		}
	}
}
