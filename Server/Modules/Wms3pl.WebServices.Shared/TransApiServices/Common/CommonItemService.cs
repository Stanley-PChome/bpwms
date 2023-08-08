using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
	public class CommonItemService
	{
		private WmsTransaction _wmsTransation;
		public CommonItemService(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
		}

		#region Private Property
		/// <summary>
		/// 模組名稱
		/// </summary>
		private readonly string _moduleName = "商品主檔";

		/// <summary>
		/// 貨主商品主檔清單
		/// </summary>
		private List<F1903> _f1903List = new List<F1903>();

		/// <summary>
		/// 商品材積階層檔清單
		/// </summary>
		private List<F190301> _f190301List = new List<F190301>();

		/// <summary>
		/// 商品材積資料檔清單
		/// </summary>
		private List<F1905> _f1905List = new List<F1905>();

		/// <summary>
		/// 商品棧板疊法設定檔清單
		/// </summary>
		private List<F190305> _f190305List = new List<F190305>();

		/// <summary>
		/// 商品大分類清單
		/// </summary>
		private List<string> _aCodeList;

		/// <summary>
		/// 商品中分類清單
		/// </summary>
		private List<string> _bCodeList;

		/// <summary>
		/// 商品小分類清單
		/// </summary>
		private List<string> _cCodeList;

		/// <summary>
		/// 倉別代碼清單
		/// </summary>
		private List<string> _typeIdList;

		/// <summary>
		/// 倉庫編號清單
		/// </summary>
		private List<string> _warehouseIdList;

		/// <summary>
		/// 計價單位設定檔清單
		/// </summary>
		private List<F91000302> _f91000302List;

		/// <summary>
		/// 計價單位編號清單
		/// </summary>
		private List<string> _accUnitList;

    /// <summary>
    /// 廠商主檔清單
    /// </summary>
    private List<string> _vnrCodeList;

		/// <summary>
		/// PCS代碼
		/// </summary>
		private string _pcsAccUnit;
		#endregion

		#region Main Method
		/// <summary>
		/// Func1
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult RecevieApiDatas(PostItemDataReq req)
		{
			CheckTransApiService ctaService = new CheckTransApiService();
			TransApiBaseService tacService = new TransApiBaseService();
			SharedService sharedService = new SharedService();
			CommonService commonService = new CommonService();
			ApiResult res = new ApiResult { IsSuccessed = true };

			#region 資料檢核1

			// 檢核貨主編號 必填、是否存在
			ctaService.CheckCustCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核Returns
			ctaService.CheckResult(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核Items
			if (req.Result.Items == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

			// 檢核資料筆數
			int reqTotal = req.Result.Total != null ? Convert.ToInt32(req.Result.Total) : 0;
			if (req.Result.Items == null || (req.Result.Items != null && !tacService.CheckDataCount(reqTotal, req.Result.Items.Count)))
				return new ApiResult { IsSuccessed = false, MsgCode = "20054", MsgContent = string.Format(tacService.GetMsg("20054"), reqTotal, req.Result.Items.Count) };

			// 檢核商品主檔筆數是否超過[商品主檔最大筆數]
			int itemMaxCnt = Convert.ToInt32(commonService.GetSysGlobalValue("ItemMaxCnt"));
			if (req.Result.Items.Count > itemMaxCnt)
				return new ApiResult { IsSuccessed = false, MsgCode = "20018", MsgContent = string.Format(tacService.GetMsg("20018"), _moduleName, req.Result.Items.Count, itemMaxCnt) };
			#endregion

			// 取得業主編號
			string gupCode = commonService.GetGupCode(req.CustCode);

      //先把要存入資料庫的內容該轉大寫的先轉好
      req.Result.Items.ForEach(x =>
      {
        x.ItemBarCode1 = x.ItemBarCode1?.ToUpper();
        x.ItemBarCode2 = x.ItemBarCode2?.ToUpper();
        x.ItemBarCode3 = x.ItemBarCode3?.ToUpper();
        x.ItemBarCode4 = x.ItemBarCode4?.ToUpper();
      });

			// 資料處理1
			return ProcessApiDatas(gupCode, req.CustCode, req.Result.Items);
		}

		/// <summary>
		/// 資料處理1
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="items">商品資料物件清單</param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas(string gupCode, string custCode, List<PostItemDataItemsModel> items)
    {
      ApiResult res = new ApiResult();
      List<ApiResponse> data = new List<ApiResponse>();
      int addCnt = 0;
      int updCnt = 0;
      F1903Repository f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransation);
      F190301Repository f190301Repo = new F190301Repository(Schemas.CoreSchema, _wmsTransation);
      F1905Repository f1905Repo = new F1905Repository(Schemas.CoreSchema, _wmsTransation);
      F190305Repository f190305Repo = new F190305Repository(Schemas.CoreSchema, _wmsTransation);
      F198001Repository f198001Repo = new F198001Repository(Schemas.CoreSchema);
      F1915Repository f1915Repo = new F1915Repository(Schemas.CoreSchema);
      F1916Repository f1916Repo = new F1916Repository(Schemas.CoreSchema);
      F1917Repository f1917Repo = new F1917Repository(Schemas.CoreSchema);
      F1980Repository f1980Repo = new F1980Repository(Schemas.CoreSchema);
      F1908Repository f1908Repo = new F1908Repository(Schemas.CoreSchema);
      F91000302Repository f91000302Repo = new F91000302Repository(Schemas.CoreSchema);
      TransApiBaseService tacService = new TransApiBaseService();

      #region Private Property
      var itemCodes = items.Where(x => !string.IsNullOrWhiteSpace(x.ItemCode)).Select(x => x.ItemCode).Distinct().ToList();
      var unitIds = items.Where(x => !string.IsNullOrWhiteSpace(x.UnitId)).Select(x => x.UnitId.PadLeft(2, '0')).Distinct().ToList();
      var aCodes = items.Where(x => !string.IsNullOrWhiteSpace(x.Ltype)).Select(x => x.Ltype).Distinct().ToList();
      var bCodes = items.Where(x => !string.IsNullOrWhiteSpace(x.Mtype)).Select(x => x.Mtype).Distinct().ToList();
      var cCodes = items.Where(x => !string.IsNullOrWhiteSpace(x.Stype)).Select(x => x.Stype).Distinct().ToList();
      var pickWarehouseIds = items.Where(x => !string.IsNullOrWhiteSpace(x.PickWarehouseId)).Select(x => x.PickWarehouseId).Distinct().ToList();
      var oriVnrCodes = items.Where(x => !string.IsNullOrWhiteSpace(x.OriVnrCode)).Select(x => x.OriVnrCode).Distinct().ToList();
      // 取得大分類代碼資料
      _aCodeList = f1915Repo.GetDatasByACode(gupCode, custCode, aCodes).Select(x => x.ACODE).ToList();

      // 取得中分類代碼資料
      _bCodeList = f1916Repo.GetDatasByBCode(gupCode, custCode, bCodes).Select(x => x.BCODE).ToList();

      // 取得小分類代碼資料
      _cCodeList = f1917Repo.GetDatasByCCode(gupCode, custCode, cCodes).Select(x => x.CCODE).ToList();

      // 取得倉別代碼清單
      _typeIdList = f198001Repo.GetDatasByItemPickWare("1").Select(x => x.TYPE_ID).ToList();

      // 取得倉庫編號清單
      _warehouseIdList = f1980Repo.GetDatasByWarehouseId(pickWarehouseIds).Select(x => x.WAREHOUSE_ID).ToList();

      // 取得計價單位設定檔清單
      _f91000302List = f91000302Repo.GetDatasByItemTypeId("001").ToList();

      // 取得計價單位編號清單
      _accUnitList = _f91000302List.Select(x => x.ACC_UNIT).ToList();

      // 取得廠商主檔清單
      _vnrCodeList = f1908Repo.GetDatas(gupCode, custCode, oriVnrCodes).Select(x => x.VNR_CODE).ToList();

      // 取得PCS編號
      var pcsData = _f91000302List.Where(x => x.ACC_UNIT_NAME == "PCS").SingleOrDefault();
      _pcsAccUnit = pcsData == null ? string.Empty : pcsData.ACC_UNIT;

      // 取得已存在貨主商品主檔清單
      var thirdPartF1903List = f1903Repo.GetDatasByItems(gupCode, custCode, itemCodes).ToList();

      // 取得商品階層主檔清單
      unitIds = unitIds.Select(x => _accUnitList.Contains(x) ? x : _pcsAccUnit).ToList();
      var thirdPartF190301List = f190301Repo.GetDatas(gupCode, custCode, itemCodes, unitIds).ToList();

      // 取得商品材積資料檔清單
      var thirdPartF1905List = f1905Repo.GetF1905ByItems(gupCode, custCode, itemCodes).ToList();

      // 取得商品棧板疊法設定檔清單
      var thirdPartF190305List = f190305Repo.GetDatas(gupCode, custCode, itemCodes).ToList();

      // 將傳入資料Group取得重複商品編號的最後一筆以及重複幾筆
      List<PostItemGroupModel> itemDatas = new List<PostItemGroupModel>();
      if (items != null && items.Any())
      {
        itemDatas = items.GroupBy(x => x.ItemCode).Select(x => new PostItemGroupModel
        {
          Count = x.Count(),
          LastData = x.Last()
        }).ToList();
      }
      #endregion

      #region 檢核
      List<F1903> exceptF1903Data = new List<F1903>();
      List<F190301> exceptF190301Data = new List<F190301>();
      List<F1905> exceptF1905Data = new List<F1905>();
      List<F190305> exceptF190305Data = new List<F190305>();

      itemDatas.ForEach(item =>
      {
        var unitId = item.LastData.UnitId.PadLeft(2, '0');
        item.LastData.UnitId = _accUnitList.Contains(unitId) ? unitId : _pcsAccUnit;
        // 20211129 - 若商品尺寸是空值時，存入組合字串 長*寬*高
        if (string.IsNullOrWhiteSpace(item.LastData.ItemSize))
        {
          item.LastData.ItemSize = $"{item.LastData.PackLength}*{item.LastData.PackWidth}*{item.LastData.PackHeight}";
        }

				// 如果首次日庫日為SqlDateTime最小值將首次入庫日轉為Null
				if (item.LastData.FirstInDate < Convert.ToDateTime(SqlDateTime.MinValue.ToString()))
					item.LastData.FirstInDate = null;

        // 資料處理2
        var res1 = CheckItem(gupCode, custCode, item.LastData);

        if (!res1.IsSuccessed)
        {
          data.AddRange((List<ApiResponse>)res1.Data);

          #region 因驗證失敗要存入需被排除的F1903、F190301、F1905、F190305資料
          // 需要被排除的F1903資料
          var excludeF1903Data = thirdPartF1903List.Where(x => x.ITEM_CODE == item.LastData.ItemCode).SingleOrDefault();
          if (excludeF1903Data != null)
            exceptF1903Data.Add(excludeF1903Data);
          // 需要被排除的F190301資料
          var excludeF190301Data = thirdPartF190301List.Where(x => x.ITEM_CODE == item.LastData.ItemCode && x.UNIT_ID == unitId).SingleOrDefault();
          if (excludeF190301Data != null)
            exceptF190301Data.Add(excludeF190301Data);
          // 需要被排除的F1905資料
          var excludeF1905Data = thirdPartF1905List.Where(x => x.ITEM_CODE == item.LastData.ItemCode).SingleOrDefault();
          if (excludeF1905Data != null)
            exceptF1905Data.Add(excludeF1905Data);
          // 需要被排除的F190305資料
          var excludeF190305Data = thirdPartF190305List.Where(x => x.ITEM_CODE == item.LastData.ItemCode).SingleOrDefault();
          if (excludeF190305Data != null)
            exceptF190305Data.Add(excludeF190305Data);
          #endregion
        }
      });

      // 排除資料
      if (exceptF1903Data.Any())
        thirdPartF1903List = thirdPartF1903List.Except(exceptF1903Data).ToList();
      if (exceptF190301Data.Any())
        thirdPartF190301List = thirdPartF190301List.Except(exceptF190301Data).ToList();
      if (exceptF1905Data.Any())
        thirdPartF1905List = thirdPartF1905List.Except(exceptF1905Data).ToList();
      if (exceptF190305Data.Any())
        thirdPartF190305List = thirdPartF190305List.Except(exceptF190305Data).ToList();
      #endregion

      #region 
      var thirdPartF1903Conditions = thirdPartF1903List.Select(z => z.ITEM_CODE).ToList();
      var thirdPartF190301Conditions = thirdPartF190301List.Select(z => new { z.ITEM_CODE, UNIT_ID = z.UNIT_ID.PadLeft(2, '0') }).ToList();
      var thirdPartF1905Conditions = thirdPartF1905List.Select(z => z.ITEM_CODE).ToList();
      var thirdPartF190305Conditions = thirdPartF190305List.Select(z => z.ITEM_CODE).ToList();
      #endregion

      #region Insert

      // 新增F1903
      var addF1903Datas = _f1903List.Where(x => !thirdPartF1903Conditions.Contains(x.ITEM_CODE)).ToList();
      if (addF1903Datas.Any())
      {
        f1903Repo.BulkInsert(addF1903Datas);
        // 計算新增數
        addCnt += itemDatas.Where(x => addF1903Datas.Select(z => z.ITEM_CODE).Contains(x.LastData.ItemCode)).Sum(x => x.Count);
      }

      // 新增F190301
      var addF190301Datas = _f190301List.Where(x => !thirdPartF190301Conditions.Any(z => z.ITEM_CODE == x.ITEM_CODE &&
                                                                                         z.UNIT_ID == x.UNIT_ID &&
                                                                                         x.UNIT_LEVEL == 1)).ToList();
      if (addF190301Datas.Any())
        f190301Repo.BulkInsert(addF190301Datas);

      // 新增F1905
      var addF1905Datas = _f1905List.Where(x => !thirdPartF1905Conditions.Contains(x.ITEM_CODE)).ToList();
      if (addF1905Datas.Any())
        f1905Repo.BulkInsert(addF1905Datas);

      // 新增F190305
      var addF190305Datas = _f190305List.Where(x => !thirdPartF190305Conditions.Contains(x.ITEM_CODE)).ToList();
      if (addF190305Datas.Any())
        f190305Repo.BulkInsert(addF190305Datas);
      #endregion

      #region Update

      #region BulkUpdateF1903 & UpdateCnt
      List<F1903> updF1903Datas = new List<F1903>();

      var updF1903 = thirdPartF1903List.Where(x => _f1903List.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE)).ToList();

      updF1903.ForEach(updData =>
      {
        var currData = _f1903List.Where(z => z.ITEM_CODE == updData.ITEM_CODE).SingleOrDefault();
        if (currData != null)
        {
          // 修改
          updData.VEN_ORD = currData.VEN_ORD;
          updData.ALL_DLN = currData.ALL_DLN;
          updData.PICK_WARE = currData.PICK_WARE;
          updData.C_D_FLAG = currData.C_D_FLAG;
          updData.ALLOW_ALL_DLN = currData.ALLOW_ALL_DLN;
          updData.MULTI_FLAG = currData.MULTI_FLAG;
          updData.MIX_BATCHNO = currData.MIX_BATCHNO;
          updData.ALLOWORDITEM = currData.ALLOWORDITEM;
          updData.BUNDLE_SERIALLOC = currData.BUNDLE_SERIALLOC;
          updData.BUNDLE_SERIALNO = currData.BUNDLE_SERIALLOC == "1" ? "1" : currData.BUNDLE_SERIALNO;
          updData.ORD_SAVE_QTY = currData.ORD_SAVE_QTY;
          updData.PICK_SAVE_QTY = currData.PICK_SAVE_QTY;
          updData.ITEM_RETURN = currData.ITEM_RETURN;
          updData.LOC_MIX_ITEM = currData.LOC_MIX_ITEM;
          updData.SERIALNO_DIGIT = currData.SERIALNO_DIGIT;
          updData.SERIAL_BEGIN = currData.SERIAL_BEGIN;
          updData.SERIAL_RULE = currData.SERIAL_RULE;
          updData.SAVE_DAY = currData.SAVE_DAY;
          updData.ITEM_STAFF = currData.ITEM_STAFF;
          updData.CHECK_PERCENT = currData.CHECK_PERCENT;
          updData.PICK_SAVE_ORD = currData.PICK_SAVE_ORD;
          updData.ISCARTON = currData.ISCARTON;
          updData.LTYPE = currData.LTYPE;
          updData.MTYPE = currData.MTYPE;
          updData.STYPE = currData.STYPE;
          updData.ITEM_NAME = currData.ITEM_NAME;
          updData.EAN_CODE1 = currData.EAN_CODE1;
          updData.EAN_CODE2 = currData.EAN_CODE2;
          if (!string.IsNullOrWhiteSpace(currData.EAN_CODE3)) { updData.EAN_CODE3 = currData.EAN_CODE3; }
          updData.ITEM_ENGNAME = currData.ITEM_ENGNAME;
          updData.ITEM_COLOR = currData.ITEM_COLOR;
          updData.ITEM_SIZE = currData.ITEM_SIZE;
          updData.TYPE = currData.LTYPE;
          updData.ITEM_HUMIDITY = currData.ITEM_HUMIDITY;
          updData.ITEM_SPEC = currData.ITEM_SPEC;
          updData.TMPR_TYPE = currData.TMPR_TYPE;
          updData.FRAGILE = currData.FRAGILE;
          updData.SPILL = currData.SPILL;
          updData.ITEM_UNIT = currData.ITEM_UNIT;
          updData.MEMO = currData.MEMO;
          updData.PICK_WARE_ID = currData.PICK_WARE_ID;
          updData.CUST_ITEM_NAME = currData.CUST_ITEM_NAME;
          updData.MAKENO_REQU = currData.MAKENO_REQU;
          updData.NEED_EXPIRED = currData.NEED_EXPIRED;
          updData.ALL_SHP = currData.ALL_SHP;
          updData.EAN_CODE4 = currData.EAN_CODE4;
          if (currData.FIRST_IN_DATE.HasValue)
          {
            updData.FIRST_IN_DATE = currData.FIRST_IN_DATE;
          }
          updData.CUST_ITEM_CODE = currData.CUST_ITEM_CODE;
          updData.VNR_CODE = currData.VNR_CODE;
          updData.RET_ORD = currData.RET_ORD;
          updData.IS_EASY_LOSE = currData.IS_EASY_LOSE;
          updData.IS_PRECIOUS = currData.IS_PRECIOUS;
          updData.IS_MAGNETIC = currData.IS_MAGNETIC;
          updData.IS_PERISHABLE = currData.IS_PERISHABLE;
          updData.IS_TEMP_CONTROL = currData.IS_TEMP_CONTROL;
          updData.IS_ASYNC = "N";
          updData.VNR_ITEM_CODE = currData.VNR_ITEM_CODE;
          updData.ORI_VNR_CODE = currData.ORI_VNR_CODE;
          updF1903Datas.Add(updData);
        }
      });

      if (updF1903Datas.Any())
      {
        f1903Repo.BulkUpdate(updF1903Datas);
        // 計算修改數
        updCnt += itemDatas.Where(x => updF1903Datas.Select(z => z.ITEM_CODE).Contains(x.LastData.ItemCode)).Sum(x => x.Count);
      }
      #endregion

      #region BulkUpdateF1905
      List<F1905> updF1905Datas = new List<F1905>();

      var updF1905 = thirdPartF1905List.Where(x => _f1905List.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE)).ToList();

      updF1905.ForEach(updData =>
      {
        var currData = _f1905List.Where(z => z.ITEM_CODE == updData.ITEM_CODE).SingleOrDefault();
        if (currData != null)
        {
          if (updData.PACK_LENGTH != currData.PACK_LENGTH ||
              updData.PACK_WIDTH != currData.PACK_WIDTH ||
              updData.PACK_HIGHT != currData.PACK_HIGHT ||
              updData.PACK_WEIGHT != currData.PACK_WEIGHT)
          {
            // 修改
            updData.PACK_LENGTH = currData.PACK_LENGTH;
            updData.PACK_WIDTH = currData.PACK_WIDTH;
            updData.PACK_HIGHT = currData.PACK_HIGHT;
            updData.PACK_WEIGHT = currData.PACK_WEIGHT;

            updF1905Datas.Add(updData);
          }
        }
      });

      if (updF1905Datas.Any())
        f1905Repo.BulkUpdate(updF1905Datas);

      #endregion

      #region BulkUpdateF190301
      List<F190301> updF190301Datas = new List<F190301>();

      var updF190301 = thirdPartF190301List.Where(x => _f190301List.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE)).ToList();

      updF190301.ForEach(updData =>
      {
        var currData = _f190301List.Where(x => x.ITEM_CODE == updData.ITEM_CODE &&
                                                     x.UNIT_ID == updData.UNIT_ID &&
                                                     x.UNIT_LEVEL == 1).SingleOrDefault();
        if (currData != null)
        {
          if (updData.UNIT_ID != currData.UNIT_ID ||
              updData.UNIT_QTY != currData.UNIT_QTY ||
              updData.LENGTH != currData.LENGTH ||
              updData.WIDTH != currData.WIDTH ||
              updData.HIGHT != currData.HIGHT ||
              updData.WEIGHT != currData.WEIGHT)
          {
            // 修改
            updData.UNIT_ID = currData.UNIT_ID;
            updData.UNIT_QTY = currData.UNIT_QTY;
            updData.LENGTH = currData.LENGTH;
            updData.WIDTH = currData.WIDTH;
            updData.HIGHT = currData.HIGHT;
            updData.WEIGHT = currData.WEIGHT;

            updF190301Datas.Add(updData);
          }
        }
      });

      if (updF190301Datas.Any())
        f190301Repo.BulkUpdate(updF190301Datas);

      #endregion

      #region BulkUpdateF190305
      List<F190305> updF190305Datas = new List<F190305>();

      var updF190305 = thirdPartF190305List.Where(x => _f190305List.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE)).ToList();

      updF190305.ForEach(updData =>
      {
        var currData = _f190305List.Where(x => x.ITEM_CODE == updData.ITEM_CODE).SingleOrDefault();
        if (currData != null)
        {
          if (updData.PALLET_LEVEL_CASEQTY != currData.PALLET_LEVEL_CASEQTY || updData.PALLET_LEVEL_CNT != currData.PALLET_LEVEL_CNT)
          {
            // 修改
            updData.PALLET_LEVEL_CASEQTY = currData.PALLET_LEVEL_CASEQTY;
            updData.PALLET_LEVEL_CNT = currData.PALLET_LEVEL_CNT;

            updF190305Datas.Add(updData);
          }
        }
      });

      if (updF190305Datas.Any())
        f190305Repo.BulkUpdate(updF190305Datas);

      #endregion

      #endregion

      #region Commit
      _wmsTransation.Complete();
      #endregion

      #region 回傳資料
      // 取得訊息內容 10003
      int total = items.Count;
      int failCnt = total - addCnt - updCnt;

      string msgContent = string.Format(tacService.GetMsg("10003"),
          _moduleName,
          addCnt,
          updCnt,
          failCnt,
          total);

      res.IsSuccessed = !data.Any();
      res.MsgCode = "10003";
      res.MsgContent = msgContent;
      res.InsertCnt = addCnt;
      res.UpdateCnt = updCnt;
      res.FailureCnt = failCnt;
      res.TotalCnt = total;
      res.Data = data.Any() ? data : null;
      #endregion

      return res;
    }

		/// <summary>
		/// 資料處理2
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="item">商品資料物件</param>
		/// <returns></returns>
		private ApiResult CheckItem(string gupCode, string custCode, PostItemDataItemsModel item)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(gupCode, custCode, item).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(gupCode, custCode, item).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(gupCode, custCode, item).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(gupCode, custCode, item).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(gupCode, custCode, item).Data);

				// 如果以上檢核成功
				if (!data.Any())
				{
					CreateItemData(gupCode, custCode, item);
				}
			}

			result.IsSuccessed = !data.Any();
			result.Data = data;

			return result;
		}

		#endregion

		#region Protected 檢核
		/// <summary>
		/// 預設值設定
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="item">商品資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckDefaultSetting(string gupCode, string custCode, PostItemDataItemsModel item)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位格式檢核
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="item">商品資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckColumnNotNullAndMaxLength(string gupCode, string custCode, PostItemDataItemsModel item)
		{
			TransApiBaseService tacService = new TransApiBaseService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 定義需檢核欄位、必填、型態、長度
			// 商品資料物件
			List<ApiCkeckColumnModel> saleCheckColumnList = new List<ApiCkeckColumnModel>
						{
								new ApiCkeckColumnModel{  Name = "ItemCode",            Type = typeof(string),   MaxLength = 20,  Nullable = false },
								new ApiCkeckColumnModel{  Name = "ItemName",            Type = typeof(string),   MaxLength = 300, Nullable = false },
								new ApiCkeckColumnModel{  Name = "ItemEngName",         Type = typeof(string),   MaxLength = 400 },
								new ApiCkeckColumnModel{  Name = "CustItemName",        Type = typeof(string),   MaxLength = 200 },
								new ApiCkeckColumnModel{  Name = "ItemBarCode1",        Type = typeof(string),   MaxLength = 26 },
								new ApiCkeckColumnModel{  Name = "ItemBarCode2",        Type = typeof(string),   MaxLength = 26 },
								new ApiCkeckColumnModel{  Name = "ItemBarCode3",        Type = typeof(string),   MaxLength = 26 },
								new ApiCkeckColumnModel{  Name = "PickWarehouse",       Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "PickWarehouseId",     Type = typeof(string),   MaxLength = 3 },
								new ApiCkeckColumnModel{  Name = "Ltype",               Type = typeof(string),   MaxLength = 20 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "Mtype",               Type = typeof(string),   MaxLength = 20 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "Stype",               Type = typeof(string),   MaxLength = 20 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "UnitId",              Type = typeof(string),   MaxLength = 3 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "UnitQty",             Type = typeof(int),      MaxLength = 6 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "ItemStaff",           Type = typeof(string),   MaxLength = 20 },
								new ApiCkeckColumnModel{  Name = "ItemColor",           Type = typeof(string),   MaxLength = 80 },
								new ApiCkeckColumnModel{  Name = "ItemSize",            Type = typeof(string),   MaxLength = 100 },
								new ApiCkeckColumnModel{  Name = "ItemSpec",            Type = typeof(string),   MaxLength = 80 },
								new ApiCkeckColumnModel{  Name = "TmprType",            Type = typeof(string),   MaxLength = 2 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "CheckPercent",        Type = typeof(decimal),  MaxLength = 14 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "PackLength",          Type = typeof(decimal),  MaxLength = 8 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "PackWidth",           Type = typeof(decimal),  MaxLength = 8 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "PackHeight",          Type = typeof(decimal),  MaxLength = 8 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "PackWeight",          Type = typeof(decimal),  MaxLength = 10 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "Memo",                Type = typeof(string),   MaxLength = 100 },
								//new ApiCkeckColumnModel{  Name = "POSafetyQty",         Type = typeof(int),      MaxLength = 10 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "PickSafetyQty",       Type = typeof(int),      MaxLength = 10 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "PickSaveOrd",         Type = typeof(int),      MaxLength = 9 },
								new ApiCkeckColumnModel{  Name = "AllDln",              Type = typeof(int),      MaxLength = 4 },
								//new ApiCkeckColumnModel{  Name = "AllowAllDln",         Type = typeof(int),      MaxLength = 4 },
								new ApiCkeckColumnModel{  Name = "SaveDay",             Type = typeof(int),      MaxLength = 5 },
								new ApiCkeckColumnModel{  Name = "VenOrd",              Type = typeof(int),      MaxLength = 6 },
								new ApiCkeckColumnModel{  Name = "RetOrd",              Type = typeof(int),      MaxLength = 6 },
								new ApiCkeckColumnModel{  Name = "IsFragile",           Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "IsSpill",             Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "MultiFlag",           Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "IsCarton",            Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "MixBatchno",          Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "LocMixItem",          Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "BundleSerialloc",     Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "BundleSerialno",      Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "SerialnoDigit",       Type = typeof(int),      MaxLength = 3 },
								new ApiCkeckColumnModel{  Name = "SerialBegin",         Type = typeof(string),   MaxLength = 10 },
								new ApiCkeckColumnModel{  Name = "SerialRule",          Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "CDFlag",              Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "AllowOrdItem",        Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "MakenoRequ",          Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "ItemReturn",          Type = typeof(string),   MaxLength = 1 , Nullable = false },
								new ApiCkeckColumnModel{  Name = "NeedExpired",         Type = typeof(string),   MaxLength = 1  },
								new ApiCkeckColumnModel{  Name = "AllShp",              Type = typeof(int),      MaxLength = 4  },
								new ApiCkeckColumnModel{  Name = "ItemBarCode4",        Type = typeof(string),   MaxLength = 26 },
								new ApiCkeckColumnModel{  Name = "FirstInDate",         Type = typeof(DateTime) },
								new ApiCkeckColumnModel{  Name = "VnrCode",             Type = typeof(string),   MaxLength = 20},
								new ApiCkeckColumnModel{  Name = "CustItemCode",        Type = typeof(string),   MaxLength = 20},
								new ApiCkeckColumnModel{  Name = "IsEasyLose",			    Type = typeof(string),   MaxLength = 1 ,    Nullable = false},
								new ApiCkeckColumnModel{  Name = "IsPrecious",			    Type = typeof(string),   MaxLength = 1 ,    Nullable = false},
								new ApiCkeckColumnModel{  Name = "IsMagnetic",			    Type = typeof(string),   MaxLength = 1 ,    Nullable = false},
                new ApiCkeckColumnModel{  Name = "IsPerishable",        Type = typeof(string),   MaxLength = 1 ,    Nullable = false},
								new ApiCkeckColumnModel{  Name = "IsTempControl",       Type = typeof(string),   MaxLength = 1 ,    Nullable = false},
								new ApiCkeckColumnModel{  Name = "VnrItemCode",         Type = typeof(string),   MaxLength = 50 ,   },
								new ApiCkeckColumnModel{  Name = "OriVnrCode",          Type = typeof(string),   MaxLength = 20 ,   },
						};
			#endregion

			#region 檢查商品欄位必填、最大長度
			List<string> itemIsNullList = new List<string>();
			List<ApiCkeckColumnModel> itemIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

			// 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
			saleCheckColumnList.ForEach(column =>
			{
				var hasValue = DataCheckHelper.CheckRequireColumn(item, column.Name);

				// 必填
				if (!column.Nullable && !hasValue)
					itemIsNullList.Add(column.Name);

				// 最大長度
				if (column.MaxLength > 0 && hasValue)
				{
					switch (column.Name)
					{
						case "CheckPercent":
							// 檢核是否符合decimal(14,11)
							if (!DataCheckHelper.CheckDataIsDecimal(item, column.Name, 3, 11))
								itemIsExceedMaxLenthList.Add(column);
							break;
						case "PackLength":
						case "PackWidth":
						case "PackHeight":
							// 檢核是否符合decimal(8,2)
							if (!DataCheckHelper.CheckDataIsDecimal(item, column.Name, 6, 2))
								itemIsExceedMaxLenthList.Add(column);
							break;
						case "PackWeight":
							// 檢核是否符合decimal(10,2)
							if (!DataCheckHelper.CheckDataIsDecimal(item, column.Name, 8, 2))
								itemIsExceedMaxLenthList.Add(column);
							break;
						default:
							if (!DataCheckHelper.CheckDataMaxLength(item, column.Name, column.MaxLength))
								itemIsExceedMaxLenthList.Add(column);
							break;
					}
				}
			});

			// 必填訊息
			if (itemIsNullList.Any())
				data.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20016", MsgContent = string.Format(tacService.GetMsg("20016"), item.ItemCode, string.Join("、", itemIsNullList)) });

			// 最大長度訊息
			if (itemIsExceedMaxLenthList.Any())
			{
				List<string> errorMsgList = itemIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

				string errorMsg = string.Join("、", errorMsgList);

				data.Add(new ApiResponse { No = item.ItemCode, MsgCode = "20017", MsgContent = string.Format(tacService.GetMsg("20017"), item.ItemCode, errorMsg) });
			}

			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位格式檢核
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="item">商品資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnType(string gupCode, string custCode, PostItemDataItemsModel item)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位資料檢核
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="item">商品資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckCommonColumnData(string gupCode, string custCode, PostItemDataItemsModel item)
		{
			CheckItemService ciService = new CheckItemService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 檢核商品階層、數量、長度、寬度、高度、重量
			ciService.CheckValueNotZero(data, item);

			// 檢核大分類是否存在
			ciService.CheckLType(data, item, _aCodeList);

			// 檢核中分類是否存在
			ciService.CheckMType(data, item, _bCodeList);

			// 檢核小分類是否存在
			ciService.CheckSType(data, item, _cCodeList);

			// 檢核商品單位編號
			ciService.CheckUnitId(data, item, _accUnitList);

			// 檢核上架倉別是否存在
			ciService.CheckWarehouse(data, item, _typeIdList);

			// 檢核揀貨倉庫編號是否存在
			ciService.CheckWarehouseId(data, item, _warehouseIdList);

			// 檢核序號檢查規則是否正確
			ciService.CheckSerialRule(data, item);

			// 檢核欄位是否為(0:否, 1:是)
			ciService.CheckBoolean(data, item);

			// 檢查首次進倉日檢查規則是否正確
			ciService.CheckFirstInDate(data, item);

			//檢查序號綁儲位商品規則
			ciService.CheckBoundleSerialLoc(data, item);

      // 檢核原廠商編號是否存在
      ciService.CheckOriVnrCode(data, item, _vnrCodeList);

      res.Data = data;
			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="item">商品資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(string gupCode, string custCode, PostItemDataItemsModel item)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion

		#region Protected 建立商品主檔資料

		/// <summary>
		/// 產生商品資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public void CreateItemData(string gupCode, string custCode, PostItemDataItemsModel item)
		{
			// 建立貨主商品主檔F1903
			_f1903List.Add(CreateF1903(gupCode, custCode, item));

			// 建立商品材積資料檔F1905
			_f1905List.Add(CreateF1905(gupCode, custCode, item));

			// 建立商品材積階層檔F190301
			_f190301List.Add(CreateF190301(gupCode, custCode, item));

			// 建立商品棧板疊法設定檔F190305
			_f190305List.Add(CreateF190305(gupCode, custCode, item));
		}

		/// <summary>
		/// 建立貨主商品主檔
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		protected F1903 CreateF1903(string gupCode, string custCode, PostItemDataItemsModel item)
		{
			var unitId = item.UnitId.PadLeft(2, '0');

			return new F1903
			{
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ITEM_CODE = item.ItemCode,
				VEN_ORD = item.VenOrd,
				RET_UNIT = null,
				RET_ORD = item.RetOrd,
				ALL_DLN = item.AllDln,
				SND_TYPE = null,
				PICK_WARE = item.PickWarehouse,
				C_D_FLAG = item.CDFlag,
				ALLOW_ALL_DLN = 0,
				MULTI_FLAG = item.MultiFlag,
				MIX_BATCHNO = item.MixBatchno,
				ALLOWORDITEM = item.AllowOrdItem,
				BUNDLE_SERIALLOC = item.BundleSerialloc,
				BUNDLE_SERIALNO = item.BundleSerialno,
				ORD_SAVE_QTY = 0,
				PICK_SAVE_QTY = Convert.ToInt64(item.PickSafetyQty),
				ITEM_EXCHANGE = null,
				ITEM_RETURN = item.ItemReturn,
				ITEM_MERGE = null,
				BORROW_DAY = 7,
				LOC_MIX_ITEM = item.LocMixItem,
				NO_PRICE = "0",
				EP_TAX = null,
				SERIALNO_DIGIT = item.SerialnoDigit,
				SERIAL_BEGIN = item.SerialBegin,
				SERIAL_RULE = item.SerialRule,
				CAN_SELL = "0",
				CAN_SPILT_IN = "0",
				LG = "0",
				SAVE_DAY = item.SaveDay,
				ITEM_STAFF = item.ItemStaff,
				CHECK_PERCENT = item.CheckPercent > 0 ? item.CheckPercent : Convert.ToDecimal(0.00000000001),
				PICK_SAVE_ORD = Convert.ToInt32(item.PickSaveOrd),
				DELV_QTY_AVG = 0,
				ISCARTON = item.IsCarton,
				ISAPPLE = null,
				LTYPE = item.Ltype,
				MTYPE = item.Mtype,
				STYPE = item.Stype,
				ITEM_NAME = item.ItemName,
				EAN_CODE1 = item.ItemBarCode1,
				EAN_CODE2 = item.ItemBarCode2,
				EAN_CODE3 = item.ItemBarCode3,
				ITEM_ENGNAME = item.ItemEngName,
				ITEM_COLOR = item.ItemColor,
				ITEM_SIZE = item.ItemSize,
				ITEM_HUMIDITY = string.IsNullOrWhiteSpace(item.TmprType) ? default(Int16) : Convert.ToInt16(item.TmprType),
				ITEM_NICKNAME = null,
				ITEM_ATTR = null,
				ITEM_SPEC = item.ItemSpec,
				TMPR_TYPE = item.TmprType,
				FRAGILE = item.IsFragile,
				SPILL = item.IsSpill,
				ITEM_TYPE = null,
				ITEM_UNIT = _accUnitList.Contains(unitId) ? unitId : _pcsAccUnit,
				ITEM_CLASS = null,
				SIM_SPEC = null,
				MEMO = item.Memo,
				VIRTUAL_TYPE = null,
				ITEM_SOURCE = null,
				PICK_WARE_ID = item.PickWarehouseId,
				STOP_DATE = null,
				BOUNDLE_SERIALREQ = "0",
				AMORTIZATION_NO = 0,
				CUST_ITEM_NAME = item.CustItemName,
				TARIFF_NO = null,
				CURRENCY = null,
				ISOEM = "0",
				ISBOX = "0",
				MAKENO_REQU = item.MakenoRequ,
				ACC_TYPE = "00",
				TYPE = item.Ltype,
				PICK_ORD = 0,
				CTNS = 0,
				NEED_EXPIRED = item.NeedExpired,
				ALL_SHP = item.AllShp,
				EAN_CODE4 = item.ItemBarCode4,
				FIRST_IN_DATE = item.FirstInDate,
				VNR_CODE = item.VnrCode,
				CUST_ITEM_CODE = item.CustItemCode,
				IS_EASY_LOSE = item.IsEasyLose,
				IS_PRECIOUS = item.IsPrecious,
				IS_MAGNETIC = item.IsMagnetic,
        IS_PERISHABLE = item.IsPerishable,
				IS_TEMP_CONTROL = item.IsTempControl,
        IS_ASYNC = "N",
        VNR_ITEM_CODE = item.VnrItemCode,
        ORI_VNR_CODE = item.OriVnrCode
      };
		}

		/// <summary>
		/// 建立商品材積資料檔
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		protected F1905 CreateF1905(string gupCode, string custCode, PostItemDataItemsModel item)
		{
			return new F1905
			{
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ITEM_CODE = item.ItemCode,
				PACK_LENGTH = Convert.ToDecimal(item.PackLength),
				PACK_WIDTH = Convert.ToDecimal(item.PackWidth),
				PACK_HIGHT = Convert.ToDecimal(item.PackHeight),
				PACK_WEIGHT = Convert.ToDecimal(item.PackWeight)
			};
		}

		/// <summary>
		/// 建立商品材積階層檔
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		protected F190301 CreateF190301(string gupCode, string custCode, PostItemDataItemsModel item)
		{
			var unitId = item.UnitId.PadLeft(2, '0');

			return new F190301
			{
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ITEM_CODE = item.ItemCode,
				UNIT_LEVEL = 1,
				UNIT_ID = _accUnitList.Contains(unitId) ? unitId : _pcsAccUnit,
				UNIT_QTY = Convert.ToInt32(item.UnitQty),
				LENGTH = item.PackLength,
				WIDTH = item.PackWidth,
				HIGHT = item.PackHeight,
				WEIGHT = item.PackWeight
			};
		}

		/// <summary>
		/// 建立商品棧板疊法設定檔
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		protected F190305 CreateF190305(string gupCode, string custCode, PostItemDataItemsModel item)
		{
			return new F190305
			{
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ITEM_CODE = item.ItemCode,
				PALLET_LEVEL_CASEQTY = 1,
				PALLET_LEVEL_CNT = 1
			};
		}
		#endregion
	}
}
