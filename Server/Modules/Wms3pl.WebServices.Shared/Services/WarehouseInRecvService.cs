using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Wcssr.Services;

namespace Wms3pl.WebServices.Shared.Services
{
  /// <summary>
  /// 商品檢驗 / PDA進貨檢驗共用函數
  /// </summary>
  public class WarehouseInRecvService
  {
    #region Repo & 變數設定
    private WmsTransaction _wmsTransaction;
    #region Service
    private CommonService _commonService;
    public CommonService CommonService
    {
      get { return _commonService == null ? _commonService = new CommonService() : _commonService; }
      set { _commonService = value; }
    }

    private SharedService _sharedService;
    public SharedService SharedService
    {
      get { return _sharedService == null ? _sharedService = new SharedService(_wmsTransaction) : _sharedService; }
      set { _sharedService = value; }
    }

    private SerialNoService _serialnoService;
    public SerialNoService SerialNoService
    {
      get { return _serialnoService == null ? _serialnoService = new SerialNoService(_wmsTransaction) : _serialnoService; }
      set { _serialnoService = value; }
    }

    private WarehouseInService _warehouseInService;
    public WarehouseInService WarehouseInService
    {
      get { return _warehouseInService == null ? _warehouseInService = new WarehouseInService(_wmsTransaction) : _warehouseInService; }
      set { _warehouseInService = value; }
    }
		#region RecvItemService
		private RecvItemService _recvItemService;
		public RecvItemService RecvItemService
		{
			get { return _recvItemService == null ? _recvItemService = new RecvItemService() : _recvItemService; }
			set { _recvItemService = value; }
		}
		#endregion
		#region StowShelfAreaService
		private StowShelfAreaService _stowShelfAreaService;
    public StowShelfAreaService StowShelfAreaService
    {
      get { return _stowShelfAreaService == null ? _stowShelfAreaService = new StowShelfAreaService(_wmsTransaction) : _stowShelfAreaService; }
      set { _stowShelfAreaService = value; }
    }
    #endregion

    #region DoubleCheckService
    private DoubleCheckService _doubleCheckService;
    public DoubleCheckService DoubleCheckService
    {
      get { return _doubleCheckService == null ? _doubleCheckService = new DoubleCheckService() : _doubleCheckService; }
      set { _doubleCheckService = value; }
    }
    #endregion

    #region ItemService
    private ItemService _itemService;
    public ItemService ItemService
    {
      get { return _itemService == null ? _itemService = new ItemService() : _itemService; }
      set { _itemService = value; }
    }
    #endregion

    #region UpdateItemInfoService
    private UpdateItemInfoService _updatdeItemInfoService;
    public UpdateItemInfoService UpdatdeItemInfoService
    {
      get { return _updatdeItemInfoService == null ? _updatdeItemInfoService = new UpdateItemInfoService() : _updatdeItemInfoService; }
      set { _updatdeItemInfoService = value; }
    }
    #endregion

    #endregion Service

    #region Repository
    private F020302Repository _f020302Repo;
    public F020302Repository F020302Repo
    {
      get { return _f020302Repo == null ? _f020302Repo = new F020302Repository(Schemas.CoreSchema, _wmsTransaction) : _f020302Repo; }
      set { _f020302Repo = value; }
    }

    private F010201Repository _f010201Repo;
    public F010201Repository F010201Repo
    {
      get { return _f010201Repo == null ? _f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction) : _f010201Repo; }
      set { _f010201Repo = value; }
    }

    private F010202Repository _f010202Repo;
    public F010202Repository F010202Repo
    {
      get { return _f010202Repo == null ? _f010202Repo = new F010202Repository(Schemas.CoreSchema, _wmsTransaction) : _f010202Repo; }
      set { _f010202Repo = value; }
    }

    private F020201Repository _f020201Repo;
    public F020201Repository F020201Repo
    {
      get { return _f020201Repo == null ? _f020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction) : _f020201Repo; }
      set { _f020201Repo = value; }
    }

    private F02020101Repository _f02020101Repo;
    public F02020101Repository F02020101Repo
    {
      get { return _f02020101Repo == null ? _f02020101Repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction) : _f02020101Repo; }
      set { _f02020101Repo = value; }
    }

    private F020501Repository _f020501Repo;
    public F020501Repository F020501Repo
    {
      get { return _f020501Repo == null ? _f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction) : _f020501Repo; }
      set { _f020501Repo = value; }
    }

    #region F0003Repository
    private F0003Repository _f0003Repo;
    public F0003Repository F0003Repo
    {
      get { return _f0003Repo == null ? _f0003Repo = new F0003Repository(Schemas.CoreSchema, _wmsTransaction) : _f0003Repo; }
      set { _f0003Repo = value; }
    }
    #endregion

    #region F020301Repository
    private F020301Repository _f020301Repo;
    public F020301Repository F020301Repo
    {
      get { return _f020301Repo == null ? _f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransaction) : _f020301Repo; }
      set { _f020301Repo = value; }
    }
    #endregion

    #region F02020109Repository
    private F02020109Repository _f02020109Repo;
    public F02020109Repository F02020109Repo
    {
      get { return _f02020109Repo == null ? _f02020109Repo = new F02020109Repository(Schemas.CoreSchema, _wmsTransaction) : _f02020109Repo; }
      set { _f02020109Repo = value; }
    }
    #endregion

    #region F190904Repository
    private F190904Repository _f190904Repo;
    public F190904Repository F190904Repo
    {
      get { return _f190904Repo == null ? _f190904Repo = new F190904Repository(Schemas.CoreSchema, _wmsTransaction) : _f190904Repo; }
      set { _f190904Repo = value; }
    }
    #endregion

    #region F02020104Repository
    private F02020104Repository _f02020104Repo;
    public F02020104Repository F02020104Repo
    {
      get { return _f02020104Repo == null ? _f02020104Repo = new F02020104Repository(Schemas.CoreSchema, _wmsTransaction) : _f02020104Repo; }
      set { _f02020104Repo = value; }
    }
    #endregion

    #region F2501Repository
    private F2501Repository _f2501Repo;
    public F2501Repository F2501Repo
    {
      get { return _f2501Repo == null ? _f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction) : _f2501Repo; }
      set { _f2501Repo = value; }
    }
    #endregion

    #region F1903Repository
    private F1903Repository _f1903Repo;
    public F1903Repository F1903Repo
    {
      get { return _f1903Repo == null ? _f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction) : _f1903Repo; }
      set { _f1903Repo = value; }
    }
    #endregion

    #region F1913Repository
    private F1913Repository _f1913Repo;
    public F1913Repository F1913Repo
    {
      get { return _f1913Repo == null ? _f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction) : _f1913Repo; }
      set { _f1913Repo = value; }
    }
    #endregion

    #region F02020107Repository
    private F02020107Repository _f02020107Repo;
    public F02020107Repository F02020107Repo
    {
      get { return _f02020107Repo == null ? _f02020107Repo = new F02020107Repository(Schemas.CoreSchema, _wmsTransaction) : _f02020107Repo; }
      set { _f02020107Repo = value; }
    }
		#endregion


		#region F010205Repository
		private F010205Repository _f010205Repo;
    public F010205Repository F010205Repo
    {
      get { return _f010205Repo == null ? _f010205Repo = new F010205Repository(Schemas.CoreSchema, _wmsTransaction) : _f010205Repo; }
      set { _f010205Repo = value; }
    }
    #endregion

    #region F190303Repository
    private F190303Repository _f190303Repo;
    public F190303Repository F190303Repo
    {
      get { return _f190303Repo == null ? _f190303Repo = new F190303Repository(Schemas.CoreSchema, _wmsTransaction) : _f190303Repo; }
      set { _f190303Repo = value; }
    }
    #endregion

    #region F020103Repository
    private F020103Repository _f020103Repo;
    public F020103Repository F020103Repo
    {
      get { return _f020103Repo == null ? _f020103Repo = new F020103Repository(Schemas.CoreSchema, _wmsTransaction) : _f020103Repo; }
      set { _f020103Repo = value; }
    }
    #endregion

    #region F0205Repository
    private F0205Repository _f0205Repo;
    public F0205Repository F0205Repo
    {
      get { return _f0205Repo == null ? _f0205Repo = new F0205Repository(Schemas.CoreSchema, _wmsTransaction) : _f0205Repo; }
      set { _f0205Repo = value; }
    }
    #endregion

    #region F1909Repository
    private F1909Repository _f1909Repo;
    public F1909Repository F1909Repo
    {
      get { return _f1909Repo == null ? _f1909Repo = new F1909Repository(Schemas.CoreSchema, _wmsTransaction) : _f1909Repo; }
      set { _f1909Repo = value; }
    }
    #endregion

    #region F910501Repository
    private F910501Repository _f910501Repo;
    public F910501Repository F910501Repo
    {
      get { return _f910501Repo == null ? _f910501Repo = new F910501Repository(Schemas.CoreSchema, _wmsTransaction) : _f910501Repo; }
      set { _f910501Repo = value; }
    }
    #endregion

    #region F010204Repository
    private F010204Repository _F010204Repo;
    public F010204Repository F010204Repo
    {
      get { return _F010204Repo == null ? _F010204Repo = new F010204Repository(Schemas.CoreSchema, _wmsTransaction) : _F010204Repo; }
      set { _F010204Repo = value; }
    }
    #endregion

    #region F02020102Repository
    private F02020102Repository _F02020102Repo;
    public F02020102Repository F02020102Repo
    {
      get { return _F02020102Repo == null ? _F02020102Repo = new F02020102Repository(Schemas.CoreSchema, _wmsTransaction) : _F02020102Repo; }
      set { _F02020102Repo = value; }
    }
    #endregion

    #region F1905Repository
    private F1905Repository _F1905Repo;
    public F1905Repository F1905Repo
    {
      get { return _F1905Repo == null ? _F1905Repo = new F1905Repository(Schemas.CoreSchema, _wmsTransaction) : _F1905Repo; }
      set { _F1905Repo = value; }
    }
    #endregion

    #endregion Repository

    #endregion


    public WarehouseInRecvService(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

		#region 進倉單取得或產生新的一批驗收暫存檔

		public ApiResult GetOrCreateRecvData(F010201 f010201,bool isVideoCombinIn,string workStationCode =null,string palletLocation = null, string purchaseSeq = null)
		{
			var apiInfoList = new List<string>();
			var dcCode = f010201.DC_CODE;
			var gupCode = f010201.GUP_CODE;
			var custCode = f010201.CUST_CODE;
			var purchaseNo = f010201.STOCK_NO;
			var nowTime = DateTime.Now;
			//(1)	撈F02020101 by dc+gup+cust+stocku_no=WmsNo
			var f02020101s = F02020101Repo.GetDatas(dcCode, gupCode, custCode, purchaseNo).ToList();
			F02020101 currentF02020101 = null;
			var rtNo = string.Empty;
			//(2)	當資料存在
			if (f02020101s != null && f02020101s.Any())
			{
				if(!string.IsNullOrEmpty(purchaseSeq))
				{
					//[B]=取得該筆明細資料 WHERE STOCK_SEQ = StockSeq
					currentF02020101 = f02020101s.FirstOrDefault(x => x.PURCHASE_SEQ == purchaseSeq);
					//若[B]不存在，回傳[此進倉單正在驗收中，但無此商品明細可進行驗收]
					if (currentF02020101 == null)
						return new ApiResult { IsSuccessed = false, MsgCode = "21927", MsgContent = "此進倉單正在驗收中，但無此商品明細可進行驗收" };
					//若[B]存在，但[B].CHECK_DETAIL=1(已檢驗完成)，回傳[此商品已檢驗完成]
					else if (currentF02020101.CHECK_DETAIL == "1")
						return new ApiResult { IsSuccessed = false, MsgCode = "21928", MsgContent = "此商品已檢驗完成" };
				}
			}
			//(3)	若資料不存在，則
			else
			{
				//[S]=檢查進倉單是否有驗收單還在綁容器
				var f020201 = F020201Repo.GetLastOrder(dcCode, gupCode,custCode, purchaseNo);
				//若[S]存在，回傳[此進倉單尚有驗收單未完成容器綁定，不可作業]
				if (f020201 != null)
					return new ApiResult { IsSuccessed = false, MsgCode = "21929", MsgContent = "此進倉單尚有驗收單未完成容器綁定，不可作業" };
				//[C]=取得F010204 by DC+GUP+CUST+StockNo
						var f010204s = F010204Repo.GetDatasByStockNos(dcCode, gupCode, custCode, new List<string> { purchaseNo }).ToList();
				var f010202s = F010202Repo.GetDatas(dcCode, gupCode, custCode, purchaseNo).ToList();
				List<NotAcceptData> notAcceptList = new List<NotAcceptData>();
				//若[C]存在，檢查是否還有未驗收數可進行驗收
				if (f010204s != null && f010204s.Any())
				{
					f010204s.ForEach(x =>
					{
						if (x.STOCK_QTY > x.TOTAL_REC_QTY)
						{
							var currentF010202 = f010202s.Find(f => f.ITEM_CODE == x.ITEM_CODE && f.STOCK_SEQ == x.STOCK_SEQ);
							notAcceptList.Add(new NotAcceptData
							{
								ITEM_CODE = x.ITEM_CODE,
								STOCK_SEQ = x.STOCK_SEQ.ToString(),
								STOCK_QTY = x.STOCK_QTY,
								NOT_STOCK_QTY = x.STOCK_QTY - x.TOTAL_REC_QTY,
								VALI_DATE = currentF010202.VALI_DATE.HasValue ? currentF010202.VALI_DATE.Value : new DateTime(9999, 12, 31),
							});
						}
					});
				}
				//若[C]不存在
				else
				{
					f010202s.ForEach(x => notAcceptList.Add(new NotAcceptData
					{
						ITEM_CODE = x.ITEM_CODE,
						STOCK_SEQ = x.STOCK_SEQ.ToString(),
						STOCK_QTY = x.STOCK_QTY,
						NOT_STOCK_QTY = x.STOCK_QTY,
						VALI_DATE = x.VALI_DATE.HasValue ? x.VALI_DATE.Value : new DateTime(9999, 12, 31),
					}));
				}
				//若有未驗收數，每一筆都要呼叫[LMS上架倉別指示API]
				f02020101s = new List<F02020101>();
				rtNo = SharedService.GetRtNo(dcCode, gupCode, custCode, Current.Staff);
				if (notAcceptList.Any())
				{
					#region Lms上架倉別指示

					var custInNo = f010201.CUST_ORD_NO;
#if (DEBUG || TEST)
					if (string.IsNullOrWhiteSpace(custInNo))
						custInNo = f010201.STOCK_NO;
#endif
					var jsondata = new List<Datas.Shared.ApiEntities.StowShelfAreaGuideData>();
					var lmsRes = StowShelfAreaService.StowShelfAreaGuide(dcCode, gupCode, custCode, "1", custInNo, notAcceptList.Select(x => x.ITEM_CODE).Distinct().ToList());
					if (lmsRes.IsSuccessed)
						// A.請求成功(Http code = 200，Code = 200)，得到[上架倉別指示的回應資料]，IsSuccessed = true
						jsondata = JsonConvert.DeserializeObject<List<Datas.Shared.ApiEntities.StowShelfAreaGuideData>>(
																JsonConvert.SerializeObject(lmsRes.Data));
					else
						// B.請求失敗，(Http code = 200，Code != 200)，紀錄ApiInfo =”LMS回覆: ”+Msg +“，雖然無法取得上架倉別的指示，但仍然可視為收貨成功”，IsSuccessed = true
						// PS.雖然請求失敗，但仍然可以視為收貨成功
						apiInfoList.Add($"LMS回覆:{lmsRes.MsgContent} 雖然無法取得上架倉別的指示，但仍然可視為收貨成功");
					#endregion
					//每一筆還有未驗收數明細都要新增F02020101
					int rtSeq = 1;
					foreach (var notAcceptData in notAcceptList)
					{
						var f1903 = CommonService.GetProduct(gupCode, custCode, notAcceptData.ITEM_CODE);
						var tarWhId = jsondata.FirstOrDefault(x => x.ItemCode == notAcceptData.ITEM_CODE)?.ShelfAreaCode;

						var addF02020101 = new F02020101
						{
							DC_CODE = f010201.DC_CODE,
							GUP_CODE = f010201.GUP_CODE,
							CUST_CODE = f010201.CUST_CODE,
							PURCHASE_NO = f010201.STOCK_NO,
							PURCHASE_SEQ = notAcceptData.STOCK_SEQ,
							VNR_CODE = f010201.VNR_CODE,
							ITEM_CODE = notAcceptData.ITEM_CODE,
							RECE_DATE = null,
							VALI_DATE = notAcceptData.VALI_DATE,
							MADE_DATE = null,
							STATUS = "0",
							CHECK_ITEM = "0",
							CHECK_SERIAL = "0",
							ISPRINT = "0",
							ISUPLOAD = "0",
							ISSPECIAL = "0",
							SPECIAL_DESC = null,
							SPECIAL_CODE = null,
							RT_NO = rtNo,
							RT_SEQ = rtSeq.ToString(),
							IN_DATE = nowTime,
							TARWAREHOUSE_ID = tarWhId,
							QUICK_CHECK = "0",
							ORDER_QTY = notAcceptData.STOCK_QTY,
							RECV_QTY = notAcceptData.NOT_STOCK_QTY,
							CHECK_QTY = SharedService.GetQtyByRatio(notAcceptData.NOT_STOCK_QTY, notAcceptData.ITEM_CODE, f010201.GUP_CODE, f010201.CUST_CODE, f010201.STOCK_NO),
							DEVICE_MODE = "2",
							PALLET_LOCATION = palletLocation,
							CHECK_DETAIL = "0",
							IS_PRINT_ITEM_ID = "0",
							DEFECT_QTY = 0
						};
						f02020101s.Add(addF02020101);
						rtSeq++;
					}
				}
				if (f02020101s.Any())
					F02020101Repo.BulkInsert(f02020101s);
				if(!string.IsNullOrEmpty(purchaseSeq))
				{
					//若產生後F02020101.PURCHASE_SEQ 不存在WmsSeq，回傳[此商品已無可驗收數量]。
					currentF02020101 = f02020101s.FirstOrDefault(x => x.PURCHASE_SEQ == purchaseSeq);
					if (currentF02020101 == null)
						return new ApiResult { IsSuccessed = false, MsgCode = "21924", MsgContent = "此商品已無可驗收數量" };
				}

				//4.	如果有開啟影資[K=true]
				if (isVideoCombinIn)
				{
					var notifyReq = new RecvItemNotifyReq()
					{
						WhId =dcCode,   //倉庫代碼
						OrderNo = f010201.STOCK_NO, //收貨單號
						WorkStationId = workStationCode,  //工作站ID
																									//SkuId ="",  //商品ID(不用填寫)
						TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") //時間戳記
					};
					var apiresult = RecvItemService.RecvItemNotify(dcCode, gupCode, custCode, notifyReq);
					if (!apiresult.IsSuccessed)
						apiInfoList.Add($"{apiresult.MsgContent}");
				}
			}

			//2.	若 F010201.status 為3(已點收)，更新F010201.STATUS=1 (驗收中) by  by stock_no = WmsNo
			if (f010201.STATUS == "3")
				F010201Repo.UpdateStatusByStockNo(dcCode, gupCode,custCode, f010201.STOCK_NO, "1");

			//3.	新增開始驗收的回檔紀錄(不存在才寫入)
			if (!F010205Repo.IsExistRecord(dcCode, gupCode,custCode, f010201.STOCK_NO, "5"))
			{
				F010205Repo.Add(new F010205
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					STOCK_NO = f010201.STOCK_NO,
					RT_NO = f02020101s.Any() ? f02020101s.First().RT_NO : rtNo,
					STATUS = "5",
					PROC_FLAG = "0",
					TRANS_DATE = nowTime,
				});
			}
			return new ApiResult
			{
				IsSuccessed = true,
				MsgCode = "10005",
				MsgContent = "執行成功",
				Data = new GetOrCreateRecvDataRes
				{
					F02020101List = f02020101s,
					ApiInfoList =apiInfoList
				}
			};
		}

		#endregion

    #region 進倉序號處理

		public List<string> GetCollectSerialList(string dcCode, string gupCode, string custCode, string purchaseNo, string poNo, string itemCode)
		{
			return F020302Repo.GetCollectionSerials(dcCode,gupCode,custCode,purchaseNo,poNo,itemCode).ToList();

		}

		public int GetIsPassSerialCnt(string dcCode,string gupCode,string custCode,string purchaseNo,string purchaseSeq,string rtNo)
		{
			return F02020104Repo.GetIsPassSerialCnt(dcCode,gupCode,custCode,purchaseNo,purchaseSeq,rtNo);
		}

		private List<string> CheckSerialPassInScanLog(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq, string rtNo,List<string> largeSerialList)
		{
			var totalCount = largeSerialList.Count();
			var range = 2000;
			int loopCnt = totalCount / range + (totalCount % range > 0 ? 1 : 0);
			var repeatSerialNoList = new List<string>();
			for (var i = 0; i < loopCnt; i++)
			{
				var serailRepeatQuery = F02020104Repo.IsExistSerialPassInScanLog(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, rtNo, largeSerialList).ToList();
				repeatSerialNoList.AddRange(serailRepeatQuery);
			}
			return repeatSerialNoList;
		}

		private List<string> CheckSerialInCollectRepeat(string dcCode, string gupCode, string custCode, string purchaseNo, string poNo, string itemCode,List<string> largeSerialList)
		{
			var totalCount = largeSerialList.Count();
			var range = 2000;
			int loopCnt = totalCount / range + (totalCount % range > 0 ? 1 : 0);
			var repeatSerialNoList = new List<string>();
			for (var i = 0; i < loopCnt; i++)
			{
				var serailRepeatQuery = F020302Repo.GetCollectionSerials(dcCode, gupCode, custCode, purchaseNo, poNo, itemCode, largeSerialList).ToList();
				repeatSerialNoList.AddRange(serailRepeatQuery);
			}
			return repeatSerialNoList;
		}

		/// <summary>
		/// 序號收集
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="purchaseSeq"></param>
		/// <param name="poNo"></param>
		/// <param name="rtNo"></param>
		/// <param name="itemCode"></param>
		/// <param name="largeSerialList"></param>
		/// <returns></returns>
		public ApiResult AddCollectSerialAndScanLog(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq, string poNo, string rtNo, string itemCode, List<string> largeSerialList)
		{
			var checkExistPassScanLogRepeatSerialNoList = CheckSerialPassInScanLog(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, rtNo, largeSerialList);
			if(checkExistPassScanLogRepeatSerialNoList.Any())
				return new ApiResult { IsSuccessed = false, MsgCode = "21948", MsgContent = "已存在相同序號，不可重複刷讀",Data = checkExistPassScanLogRepeatSerialNoList.Select(x=> new SerialNoResult { Checked =false,SerialNo = x,Message="序號重複" }).ToList() };
			var checkExistCollectRepeatSerialNoList = CheckSerialInCollectRepeat(dcCode, gupCode, custCode, purchaseNo, poNo, itemCode, largeSerialList);
			if (checkExistCollectRepeatSerialNoList.Any())
				return new ApiResult { IsSuccessed = false, MsgCode = "21948", MsgContent = "已存在相同序號，不可重複刷讀", Data = checkExistPassScanLogRepeatSerialNoList.Select(x => new SerialNoResult { Checked = false, SerialNo = x, Message = "序號重複" }).ToList() };

			var checkSerialRes = SerialNoService.CheckItemLargeSerialWithBeforeInWarehouse(gupCode, custCode, itemCode, largeSerialList.ToList()).ToList();
			if (checkSerialRes.Any(x => !x.Checked))
				return new ApiResult { IsSuccessed = false, MsgCode = "21904", MsgContent = "序號檢核失敗", Data = checkSerialRes.Where(x => !x.Checked).ToList() };

			var addF02020104List = new List<F02020104>();
			var addF020302List = new List<F020302>();
			var MaxLogSeq = F02020104Repo.GetMaxSeq(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, rtNo) + 1;
			var fileSeq = F020301Repo.GetF020301FileSeq(dcCode, gupCode, custCode, purchaseNo, poNo) + 1;
			var fileName = string.Format("USERCHK99_{0}{1}", purchaseNo, fileSeq.ToString("D2"));
			var f020301 = new F020301
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				PURCHASE_NO = purchaseNo,
				FILE_NAME = fileName
			};
			foreach (var item in checkSerialRes)
			{
				addF02020104List.Add(CreateF02020104(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, itemCode, item.SerialNo.ToUpper(), item.CurrentlyStatus, "1", "", MaxLogSeq, rtNo, null));
				addF020302List.Add(new F020302
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					FILE_NAME = fileName,
					PO_NO = poNo,
					ITEM_CODE = itemCode,
					SERIAL_NO = item.SerialNo.ToUpper(),
					SERIAL_LEN = (short)item.SerialNo.Length,
					VALID_DATE = DateTime.MaxValue.Date,
					STATUS = "0",
					BATCH_NO = null
				});
			}
			F02020104Repo.BulkInsert(addF02020104List);
			F020301Repo.Add(f020301);
			F020302Repo.BulkInsert(addF020302List);
			return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = "執行成功" };

		}

    /// <summary>
    /// 序號抽驗
    /// </summary>
    /// <param name="p020203Data"></param>
    /// <param name="rtNo"></param>
    /// <param name="newSerialNo"></param>
    /// <returns></returns>
    public ApiResult RandomCheckSerial(string dcCode, string gupCode, string custCode, string itemCode, string purchaseNo, string purchaseSeq, string poNo, string rtNo, string serialNo)
    {
			var checkExistCollectRepeatSerialNoList = CheckSerialInCollectRepeat(dcCode, gupCode, custCode, purchaseNo, poNo, itemCode, new List<string> { serialNo });
			var MaxLogSeq = F02020104Repo.GetMaxSeq(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, rtNo) + 1;
			var f2501 = CommonService.GetItemSerialList(gupCode, custCode, new List<string> { serialNo }).FirstOrDefault();
			if (!checkExistCollectRepeatSerialNoList.Any())
			{
				F02020104Repo.Add(CreateF02020104(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, itemCode, serialNo, f2501 != null ? f2501.STATUS : null, "0", "刷讀的序號不存在進貨序號檔", MaxLogSeq, rtNo, null));
				return new ApiResult { IsSuccessed = false, MsgCode = "21947", MsgContent = "刷讀的序號不存在進貨序號檔" };
			}

			var checkExistPassScanLogRepeatSerialNoList = CheckSerialPassInScanLog(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, rtNo,  new List<string> { serialNo });
			if (checkExistPassScanLogRepeatSerialNoList.Any())
				return new ApiResult { IsSuccessed = false, MsgCode = "21948", MsgContent = "已存在相同序號，不可重複刷讀" };


			F02020104Repo.Add(CreateF02020104(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, itemCode, serialNo, f2501 != null ? f2501.STATUS : null, "1", "", MaxLogSeq, rtNo, null));
			return new ApiResult() { IsSuccessed = true, MsgCode = "10005", MsgContent = "執行成功" };
		}


		private F02020104 CreateF02020104(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq, string itemCode, string serialNo, string status, string isPass, string message, int logSeq, string rtNo, string batchNo)
    {
      return new F02020104
      {
        DC_CODE = dcCode,
        GUP_CODE = gupCode,
        CUST_CODE = custCode,
        ISPASS = isPass,
        ITEM_CODE = itemCode,
        PURCHASE_NO = purchaseNo,
        PURCHASE_SEQ = purchaseSeq,
        SERIAL_NO = serialNo,
        STATUS = string.IsNullOrWhiteSpace(status) ? null : status,
        LOG_SEQ = logSeq,
        RT_NO = rtNo,
        BATCH_NO = batchNo,
        MESSAGE = message
      };
    }

		public ApiResult RemoveScanSerialLog(string dcCode,string gupCode,string custCode,string purchaseNo,string purchaseSeq,string rtNo,string serialNo)
		{
			F02020104Repo.UpdateFields(new { ISPASS = "0", MESSAGE = "人員移除序號" }, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PURCHASE_NO == purchaseNo && x.PURCHASE_SEQ == purchaseSeq && x.RT_NO == rtNo && x.SERIAL_NO == serialNo && x.ISPASS == "1");
			return new ApiResult() { IsSuccessed = true, MsgCode = "10005" };
		}

		public ApiResult RemoveCollectSerialAndScanLog(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq, string rtNo, string itemCode, string serialNo,string poNo)
		{
			F020302Repo.DeleteF020302(dcCode, gupCode, custCode, purchaseNo, poNo, itemCode, serialNo);
			F02020104Repo.UpdateFields(new { ISPASS = "0", MESSAGE = "人員移除序號" }, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PURCHASE_NO == purchaseNo && x.PURCHASE_SEQ == purchaseSeq && x.RT_NO == rtNo && x.SERIAL_NO == serialNo && x.ISPASS == "1");
			bool isFind = F02020109Repo.IsFindF02020109(dcCode, gupCode, custCode, purchaseNo, int.Parse(purchaseSeq), serialNo);
			if(isFind)
			{
				F02020109Repo.DeleteF02020109(dcCode, gupCode, custCode, purchaseNo, int.Parse(purchaseSeq), serialNo);
				F02020101Repo.UpdateDefectQty(dcCode, gupCode, custCode, purchaseNo, purchaseSeq);
			}
			return new ApiResult() { IsSuccessed = true, MsgCode = "10005", MsgContent = "執行成功" };
		}


		#endregion

		#region 不良品資料處理

		public ApiResult AddDefect(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq,string poNo, string warehouseId, string uccCode, string itemCode, int qty, string serialNo, string memo)
		{
			var stockSeq = int.Parse(purchaseSeq);
			if (!string.IsNullOrEmpty(serialNo))
			{
				var isFindCollectSerial = F020302Repo.GetCollectionSerials(dcCode, gupCode, custCode, purchaseNo, poNo, itemCode, new List<string> { serialNo }).Any();
				if (!isFindCollectSerial)
					return new ApiResult { IsSuccessed = false, MsgCode = "21954", MsgContent = "刷讀的不良品序號不存在進貨序號檔，請先進行序號刷讀" };
				bool isFindDefectSerial = F02020109Repo.IsFindF02020109(dcCode, gupCode, custCode, purchaseNo, stockSeq, serialNo);
				if(isFindDefectSerial)
					return new ApiResult { IsSuccessed = false, MsgCode = "21955", MsgContent = "刷讀的不良品序號已存在，不可重複刷讀" };
			}

			//新增不良品紀錄
			F02020109Repo.Add(new F02020109
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				STOCK_NO = purchaseNo,
				STOCK_SEQ = stockSeq,
				WAREHOUSE_ID = warehouseId,
				UCC_CODE = uccCode,
				DEFECT_QTY = qty,
				SERIAL_NO = !string.IsNullOrEmpty(serialNo) ? serialNo : null,
				CAUSE = memo,
			});
			// 更新不良品倉別
			F02020109Repo.UpdateWarehouse(dcCode, gupCode, custCode, purchaseNo, stockSeq, warehouseId);

			// 更新驗收暫存檔不良品數量
			F02020101Repo.UpdateDefectQty(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, qty);

			return new ApiResult { IsSuccessed = true, MsgCode = "10005" };
		}
		public ApiResult DelDefect(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq, long DefectId)
		{
			var f02020109 = F02020109Repo.GetDataById(DefectId);
			if (f02020109 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21946", MsgContent = "不良品明細不存在" };

			//新增不良品紀錄
			F02020109Repo.Delete(x => x.ID == DefectId);

			// 更新驗收暫存檔不良品數量
			F02020101Repo.UpdateDefectQty(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, -f02020109.DEFECT_QTY ?? 0);

			return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = "執行成功" };
		}

    #endregion


    /// <summary>
    /// 釋放進倉單鎖定
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public void UnLockAcceptenceOrder(UnLockAcceptenceOrderReq req)
    {
			var f075110Repo = new F075110Repository(Schemas.CoreSchema, _wmsTransaction);
			f075110Repo.UpdateFields(
        new { STATUS = "1" },       // 人員驗收完成
        x => x.DC_CODE == req.DcCode &&
             x.GUP_CODE == req.GupCode &&
             x.CUST_CODE == req.CustCode &&
             x.WMS_NO == req.StockNo &&
             x.STATUS == "0");
    }

    /// <summary>
    /// 鎖定進貨單控管表
    /// </summary>
    /// <param name="req">傳入參數</param>
    /// <returns></returns>
    public ApiResult LockAcceptenceOrder(LockAcceptenceOrderReq req)
    {
			var f075110Repo = new F075110Repository(Schemas.CoreSchema);
      #region 進貨單檢查&鎖定
      var f075110 = f075110Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
        () =>
        {
          var lockF075110 = f075110Repo.LockF075110();
          var chkF075110 = f075110Repo.Find(x => x.DC_CODE == req.DcCode && x.GUP_CODE == req.GupCode && x.CUST_CODE == req.CustCode && x.WMS_NO == req.StockNo && x.STATUS == "0");
          if (chkF075110 == null)
          {
            var newF075110 = new F075110()
            {
              DC_CODE = req.DcCode,
              GUP_CODE = req.GupCode,
              CUST_CODE = req.CustCode,
              WMS_NO = req.StockNo,
              STATUS = "0",
              DEVICE_TOOL = req.DeviceTool,
              CRT_DATE = DateTime.Now,
              CRT_STAFF = Current.Staff,
              CRT_NAME = Current.StaffName
            };
						f075110Repo.Add(newF075110, true);
            return newF075110;
          }
          else //chkF075111 != null
          {
            if (chkF075110.DEVICE_TOOL == req.DeviceTool && chkF075110.CRT_STAFF == Current.Staff)
            {
              return chkF075110;
            }
						// 電腦版不同人作業允許直接更換不需要確認
						else if(chkF075110.DEVICE_TOOL == req.DeviceTool && req.DeviceTool =="0" && chkF075110.CRT_STAFF != Current.Staff)
						{
							chkF075110.STATUS = "2";
							f075110Repo.UpdateFields(new { chkF075110.STATUS }, x => x.ID == chkF075110.ID);
							var newF075110 = new F075110()
							{
								DC_CODE = req.DcCode,
								GUP_CODE = req.GupCode,
								CUST_CODE = req.CustCode,
								WMS_NO = req.StockNo,
								STATUS = "0",
								DEVICE_TOOL = req.DeviceTool,
								CRT_DATE = DateTime.Now,
								CRT_STAFF = Current.Staff,
								CRT_NAME = Current.StaffName
							};
							f075110Repo.Add(newF075110, true);
							return newF075110;
						}
            else
            {
              chkF075110.STATUS = "1";  //不可使用
              return chkF075110;
            }
          }
        });

      if (f075110.STATUS == "1")
        //此進倉單已被{0}使用電腦版/PDA作業，不可使用
        return new ApiResult { IsSuccessed = false, MsgCode = "21940", MsgContent = string.Format("這張進倉單已被{0}使用{1}作業，不可使用", f075110.CRT_NAME, f075110.DEVICE_TOOL == "0" ? "電腦版" :"PDA") };

      return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = "執行成功" };

      #endregion
    }

    /// <summary>
    /// 更新驗收單列印資訊
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="rtNoList"></param>
    public ExecuteResult UpdateRecvNotePrintInfo(string dcCode, string gupCode, string custCode, List<string> rtNoList)
    {
      var f020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);
      var datas = f020201Repo.GetDatasByRtNoList(dcCode, gupCode, custCode, rtNoList).ToList();
      datas.ForEach(x =>
      {
        x.PRINT_MODE = "2"; //已列印
        x.PRINT_STAFF = Current.Staff;
        x.PRINT_NAME = Current.StaffName;
        x.PRINT_TIME = DateTime.Now;
      });
      f020201Repo.BulkUpdate(datas);
      return new ExecuteResult(true);
    }

    #region 驗收完成
    /// <summary>
    /// 驗收完成
    /// </summary>
    /// <param name="acp"></param>
    /// <returns></returns>
    public ExecuteResult AcceptanceConfirm(AcceptanceConfirmPara acp)
    {
      List<F02020104> f02020104s = null;
      List<F020201> addF020201List = new List<F020201>();
      var addF02020104List = new List<F02020104>();
      var updF020302List = new List<F020302>();
      var updF02020109List = new List<F02020109>();
      var addF0205List = new List<F0205>();
      var returnStocks = new List<F1913>();
      var updF02020101List = new List<F02020101>();
      ReturnNewAllocationResult allocationResult = null;

      var today = DateTime.Today;
      //取得來源儲位
      var srcLoc = SharedService.GetSrcLoc(acp.DcCode, acp.GupCode, acp.CustCode, "I");//I:進貨暫存倉
      if (srcLoc == null)
        return new ExecuteResult() { IsSuccessed = false, Message = "找不到可用的進貨暫存倉儲位" };
      var srcLocCode = srcLoc.LOC_CODE;
      //取得虛擬商品的儲位	
      var f190904Item = F190904Repo.Find(o => o.DC_CODE == acp.DcCode && o.GUP_CODE == acp.GupCode && o.CUST_CODE == acp.CustCode);
      string virtualItemLocCode = f190904Item == null ? string.Empty : f190904Item.LOC_CODE;

      //取得進倉單主檔
      var f010201 = acp.f010201; //F010201Repo.GetDatasByStockNo(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo);

      //取得此驗收單號本次進貨驗收暫存資料
      var tmpList = acp.f02020101s;

      //取得不良品暫存檔資料
      var f02020109s = F02020109Repo.GetDatasByStockNo(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo).ToList();
      //22.	[E]=取得設定檔[是否要綁容器]
      var rtMode = CommonService.GetSysGlobalValue(acp.DcCode, acp.GupCode, acp.CustCode, "RecvItemNeedContainer") == "0" ? "0" : "1";

      var IsPrintRecvnote = CommonService.GetSysGlobalValue(acp.DcCode, acp.GupCode, acp.CustCode, "AutoPrintReports") != "0";
      ////16.	針對[D].CHECK_SERIAL=1商品，取得F020302進貨商品序號
      var f020302s = F020301Repo.AsForUpdate().GetF020302s(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo).ToList();

      var virtualtmpList = new List<F02020101>();
      foreach (var tmp in tmpList)
      {
        //更新驗收暫存檔
        tmp.STATUS = "1"; //已驗收待上傳
        updF02020101List.Add(tmp);

        //即時新增/更新 驗收批號的流水號紀錄檔(F020203)，用以回填F02020101批號
        var currRtSeq = GetItemMakeRtSeq(tmp);

        // 回填批號
        tmp.MAKE_NO = $"{today.ToString("yyMMdd")}{Convert.ToString(currRtSeq).PadLeft(3, '0') }";

        //14.	由[D]產生F020201資料
        addF020201List.Add(new F020201
        {
          RT_NO = tmp.RT_NO,
          RT_SEQ = tmp.RT_SEQ,
          PURCHASE_NO = tmp.PURCHASE_NO,
          PURCHASE_SEQ = tmp.PURCHASE_SEQ,
          DC_CODE = tmp.DC_CODE,
          GUP_CODE = tmp.GUP_CODE,
          CUST_CODE = tmp.CUST_CODE,
          VNR_CODE = tmp.VNR_CODE,
          ITEM_CODE = tmp.ITEM_CODE,
          RECE_DATE = tmp.RECE_DATE,
          VALI_DATE = tmp.VALI_DATE,
          MADE_DATE = tmp.MADE_DATE,
          ORDER_QTY = tmp.ORDER_QTY,
          RECV_QTY = tmp.RECV_QTY,
          CHECK_QTY = tmp.CHECK_QTY,
          STATUS = rtMode == "1" && tmp.RECV_QTY > 0 ? "3" : "2",
          CHECK_ITEM = tmp.CHECK_ITEM,
          CHECK_SERIAL = tmp.CHECK_SERIAL,
          ISPRINT = tmp.ISPRINT,
          ISUPLOAD = tmp.ISUPLOAD,
          ISSPECIAL = tmp.ISSPECIAL,
          SPECIAL_DESC = tmp.SPECIAL_DESC,
          SPECIAL_CODE = tmp.SPECIAL_CODE,
          IN_DATE = tmp.IN_DATE,
          TARWAREHOUSE_ID = tmp.TARWAREHOUSE_ID,
          QUICK_CHECK = tmp.QUICK_CHECK,
          MAKE_NO = tmp.MAKE_NO,
          RT_MODE = rtMode,
          DEVICE_MODE = tmp.DEVICE_MODE,
          PALLET_LOCATION = tmp.PALLET_LOCATION,
          IS_PRINT_ITEM_ID = tmp.IS_PRINT_ITEM_ID,
          IS_PRINT_RECVNOTE = IsPrintRecvnote ? "1" : "0",
          PRINT_MODE = (tmp.IS_PRINT_ITEM_ID == "1" || IsPrintRecvnote) ? "1" : "0",
          CUST_ORD_NO = f010201.CUST_ORD_NO,
		  DEFECT_QTY = tmp.DEFECT_QTY,
        });
        //15.	更新F02020109 驗收單號與驗收項次
        updF02020109List.AddRange(f02020109s.Where(x => x.DC_CODE == tmp.DC_CODE
                                                     && x.GUP_CODE == tmp.GUP_CODE
                                                     && x.CUST_CODE == tmp.CUST_CODE
                                                     && x.STOCK_NO == tmp.PURCHASE_NO
                                                     && x.STOCK_SEQ == Convert.ToInt32(tmp.PURCHASE_SEQ)
                                                     && string.IsNullOrWhiteSpace(x.RT_NO))
                                            .Select(x => new F02020109
                                            {
                                              ID = x.ID,
                                              DC_CODE = x.DC_CODE,
                                              GUP_CODE = x.GUP_CODE,
                                              CUST_CODE = x.CUST_CODE,
                                              STOCK_NO = x.STOCK_NO,
                                              STOCK_SEQ = x.STOCK_SEQ,
                                              DEFECT_QTY = x.DEFECT_QTY,
                                              SERIAL_NO = x.SERIAL_NO,
                                              UCC_CODE = x.UCC_CODE,
                                              CAUSE = x.CAUSE,
                                              CRT_DATE = x.CRT_DATE,
                                              CRT_NAME = x.CRT_NAME,
                                              CRT_STAFF = x.CRT_STAFF,
                                              RT_NO = tmp.RT_NO,
                                              RT_SEQ = tmp.RT_SEQ,
                                              WAREHOUSE_ID = x.WAREHOUSE_ID
                                            }).ToList());

        var f1903 = CommonService.GetProduct(tmp.GUP_CODE, tmp.CUST_CODE, tmp.ITEM_CODE);
        #region 虛擬商品儲位檢核
        //是否為虛擬商品
        bool isVirtualItem = !string.IsNullOrEmpty(f1903.VIRTUAL_TYPE);
        if (isVirtualItem && string.IsNullOrEmpty(virtualItemLocCode))
          return new ExecuteResult() { IsSuccessed = false, Message = "找不到可用的虛擬商品儲位" };

        if (isVirtualItem)
          virtualtmpList.Add(tmp);
        #endregion

        //依商品驗收數量取得F020302序號筆數
        var itemF020302s = f020302s.Where(x => x.ITEM_CODE == tmp.ITEM_CODE && x.STATUS == "0").Take(tmp.RECV_QTY ?? 0).ToList();

        #region 序號商品更新狀態
        if (tmp.CHECK_SERIAL == "1")
        {
          var maxSeq = 0;
          if (itemF020302s.Any())
          {
            if (f02020104s == null)
            {
              f02020104s = F02020104Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == tmp.DC_CODE && x.GUP_CODE == tmp.GUP_CODE
                                                                    && x.CUST_CODE == tmp.CUST_CODE && x.PURCHASE_NO == tmp.PURCHASE_NO).ToList();
              maxSeq = f02020104s.Max(x => x.LOG_SEQ);
            }
            // 增加寫入F02020104 此序號，如果不存在才寫入 SERAIL_NO = 序號,ISPASS = 1,RT_NO = 驗收單號
            var serialNoData = f02020104s.Where(x => x.PURCHASE_SEQ == tmp.PURCHASE_SEQ).Select(x => x.SERIAL_NO).ToList();
            var f020302Data = itemF020302s.Where(x => !serialNoData.Contains(x.SERIAL_NO)).ToList();
            var f2501Data = CommonService.GetItemSerialList(tmp.GUP_CODE, tmp.CUST_CODE, serialNoData).ToList();
            addF02020104List.AddRange(CreateF02020104sExistF020302s(tmp, f020302Data, f2501Data, maxSeq));

            //更新序號狀態
            var checkResult = UpdateF2501(tmp, f010201.ORD_PROP, itemF020302s, f02020109s);
            if (!checkResult.IsSuccessed)
              return new ExecuteResult { IsSuccessed = false, Message = checkResult.Message };
          }
        }
        #endregion

        #region 產生進貨暫存倉庫存資料(不綁容器才跑)
        if (rtMode == "0")
          InsertOrUpdateStock(tmp, isVirtualItem ? virtualItemLocCode : srcLocCode, itemF020302s, ref returnStocks);
        #endregion

        if (itemF020302s.Any())
          updF020302List.AddRange(itemF020302s);
      }

      var notVirtualItemList = tmpList.Except(virtualtmpList).ToList();
      var hasRecvList = notVirtualItemList.Where(x => (x.RECV_QTY ?? 0) > 0).ToList();

      #region 不良品拆單
      CreateDefectItem(ref hasRecvList, f02020109s);
      #endregion

      if (hasRecvList.Any() && rtMode == "0")
      {
        #region 產生調撥單
        // 因不良品序號若沒有在後面，後面產生調撥單Group將會錯亂把不良序號 歸類在良品倉扣帳
        // 找出不良品序號
        var defectSerialNos = f02020109s.Where(x => !string.IsNullOrWhiteSpace(x.SERIAL_NO)).Select(x => x.SERIAL_NO);
        if (defectSerialNos.Any())
        {
          // 將序號綁儲位不良品放到最後
          var defectReturnStocks = returnStocks.Where(x => defectSerialNos.Contains(x.SERIAL_NO));

          returnStocks = returnStocks.Except(defectReturnStocks).ToList();

          returnStocks.AddRange(defectReturnStocks);
        }

        allocationResult = CreateAcceptanceAllocation(acp, srcLoc, returnStocks.Where(x => x.LOC_CODE != virtualItemLocCode).ToList(), hasRecvList);
        if (!allocationResult.Result.IsSuccessed)
          return new ExecuteResult() { IsSuccessed = false, Message = allocationResult.Result.Message };
        #endregion

        #region 設定驗收單與調撥單關係
        var f02020107s = F02020107Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.PURCHASE_NO == acp.PurchaseNo && x.RT_NO == acp.RTNo).ToList();
        foreach (var item in allocationResult.AllocationList)
        {
          if (!f02020107s.Any(x => x.ALLOCATION_NO == item.Master.ALLOCATION_NO))
          {
            var f02020107 = new F02020107
            {
              DC_CODE = acp.DcCode,
              GUP_CODE = acp.GupCode,
              CUST_CODE = acp.CustCode,
              ALLOCATION_NO = item.Master.ALLOCATION_NO,
              PURCHASE_NO = acp.PurchaseNo,
              RT_NO = acp.RTNo
            };
            F02020107Repo.Add(f02020107);
          }
        }

        #endregion
      }

      #region 產生虛擬商品庫存
      if (virtualtmpList.Any())
      {
        var virtalItemReturnStocks = returnStocks.Where(x => x.LOC_CODE == virtualItemLocCode).ToList();
        var createStocks = virtalItemReturnStocks.Where(x => string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();
        var updateStocks = virtalItemReturnStocks.Where(x => !string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();
        if (createStocks.Any())
          F1913Repo.BulkInsert(createStocks);
        if (updateStocks.Any())
          F1913Repo.BulkUpdate(updateStocks);

        List<F02020108> f02020108s = new List<F02020108>();
        List<F020201> f020201List = addF020201List.Where(x => virtualtmpList.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE)).ToList();

        // 新增進倉驗收上架結果表[F010204]
        WarehouseInService.CreateF010204s(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo, f020201List, updF02020109List, true);

        if (rtMode == "0")
        {
          // 新增進倉單明細、驗收單明細與調撥單明細的關聯表[F02020108]
          WarehouseInService.CreateF02020108s(acp.DcCode, acp.GupCode, acp.CustCode, f020201List, null, null, ref f02020108s, f02020109s, true);

          // 新增進倉回檔歷程紀錄表[F010205]
          var f02020108sByCrtF010205 = f02020108s.Select(x => new F02020108
          {
            ALLOCATION_SEQ = x.ALLOCATION_SEQ,
            CUST_CODE = x.CUST_CODE,
            DC_CODE = x.DC_CODE,
            GUP_CODE = x.GUP_CODE,
            REC_QTY = x.REC_QTY,
            RT_NO = x.RT_NO,
            RT_SEQ = x.RT_SEQ,
            STOCK_NO = x.STOCK_NO,
            STOCK_SEQ = x.STOCK_SEQ,
            TAR_QTY = x.TAR_QTY
          }).ToList();
          WarehouseInService.CreateF010205s(f02020108sByCrtF010205, status: "2");

          // 新增進倉回檔歷程紀錄表[F010205]
          WarehouseInService.CreateF010205s(f02020108s, status: "3");

          // 新增進倉上架歷程表[F020202]
          WarehouseInService.CreateF020202s(f020201List, returnStocks);
        }
      }
      #endregion

      if (rtMode == "0")
      {
        #region 整批寫入調撥單
        if (allocationResult != null)
        {
          var allocationExecResult = SharedService.BulkInsertAllocation(allocationResult.AllocationList, allocationResult.StockList);
          if (!allocationExecResult.IsSuccessed)
            return new ExecuteResult { IsSuccessed = false, Message = allocationExecResult.Message };

          List<F02020108> f02020108s = new List<F02020108>();
          List<F020201> f020201List = addF020201List.Where(x => !virtualtmpList.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE)).ToList();
          // 新增進倉單明細、驗收單明細與調撥單明細的關聯表[F02020108]
          allocationResult.AllocationList.ForEach(newAllocation =>
          {
            WarehouseInService.CreateF02020108s(acp.DcCode, acp.GupCode, acp.CustCode, f020201List, newAllocation.Master, newAllocation.Details, ref f02020108s, f02020109s);
          });

          // 新增進倉驗收上架結果表[F010204]
          WarehouseInService.CreateF010204s(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo, f020201List, updF02020109List);

          // 新增進倉回檔歷程紀錄表[F010205]
          var f02020108sByCrtF010205 = f02020108s.Select(x => new F02020108
          {
            ALLOCATION_SEQ = x.ALLOCATION_SEQ,
            CUST_CODE = x.CUST_CODE,
            DC_CODE = x.DC_CODE,
            GUP_CODE = x.GUP_CODE,
            REC_QTY = x.REC_QTY,
            RT_NO = x.RT_NO,
            RT_SEQ = x.RT_SEQ,
            STOCK_NO = x.STOCK_NO,
            STOCK_SEQ = x.STOCK_SEQ,
            TAR_QTY = x.TAR_QTY
          }).ToList();
          WarehouseInService.CreateF010205s(f02020108sByCrtF010205, status: "2");
        }
        else
        {
          #region 產生一單一品進貨暫存倉庫存(進貨全都是一單一品要從進貨暫存倉出貨)
          var notVirtalItemReturnStocks = returnStocks.Where(x => x.LOC_CODE != virtualItemLocCode).ToList();
          var createStocks = notVirtalItemReturnStocks.Where(x => string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();
          var updateStocks = notVirtalItemReturnStocks.Where(x => !string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();
          if (createStocks.Any())
            F1913Repo.BulkInsert(createStocks);
          if (updateStocks.Any())
            F1913Repo.BulkUpdate(updateStocks);
          #endregion
        }
				#endregion

				//(8)	釋放F75110.STATUS=1(人員驗收完成) By DC+GUP+CUST+STOCK_NO+STAUTS=0
				UnLockAcceptenceOrder(new UnLockAcceptenceOrderReq
				{
					DcCode = acp.DcCode,
					GupCode = acp.GupCode,
					CustCode = acp.CustCode,
					StockNo = acp.PurchaseNo,
				});
			}
      else
      {
        List<F020201> f020201List = addF020201List.Where(x => !virtualtmpList.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE)).ToList();
        // 新增進倉驗收上架結果表[F010204]
        WarehouseInService.CreateF010204s(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo, f020201List, updF02020109List);

        //計算良品數量=驗收總量扣除不良品數量
        var CheckOKItems = addF020201List.Select(x => new
        {
          x.DC_CODE,
          x.GUP_CODE,
          x.CUST_CODE,
          x.ITEM_CODE,
          x.PURCHASE_NO,
          x.PURCHASE_SEQ,
          x.RT_NO,
          x.RT_SEQ,
          x.RECV_QTY,
          OK_QTY = x.RECV_QTY - updF02020109List.Where(a => x.DC_CODE == a.DC_CODE && x.GUP_CODE == a.GUP_CODE && x.CUST_CODE == a.CUST_CODE && x.PURCHASE_NO == a.STOCK_NO && x.PURCHASE_NO == a.STOCK_NO && x.PURCHASE_SEQ == a.STOCK_SEQ.ToString())?.Sum(a => a.DEFECT_QTY) ?? 0,
          NG_QTY = updF02020109List.Where(a => x.DC_CODE == a.DC_CODE && x.GUP_CODE == a.GUP_CODE && x.CUST_CODE == a.CUST_CODE && x.PURCHASE_NO == a.STOCK_NO && x.PURCHASE_NO == a.STOCK_NO && x.PURCHASE_SEQ == a.STOCK_SEQ.ToString())?.Sum(a => a.DEFECT_QTY) ?? 0,
          RetrunWarehouseID = updF02020109List.FirstOrDefault(a => x.DC_CODE == a.DC_CODE && x.GUP_CODE == a.GUP_CODE && x.CUST_CODE == a.CUST_CODE && x.PURCHASE_NO == a.STOCK_NO && x.PURCHASE_NO == a.STOCK_NO && x.PURCHASE_SEQ == a.STOCK_SEQ.ToString())?.WAREHOUSE_ID
        }).ToList();

        foreach (var item in CheckOKItems.Where(x => x.OK_QTY > 0))
        {
          //(3)	呼叫LMS上架倉別分配
          #region 呼叫LMS上架倉別分配
          var stowShelfAreaResult = StowShelfAreaService.StowShelfAreaAssign(item.DC_CODE, item.CUST_CODE, f010201.CUST_ORD_NO, item.ITEM_CODE, item.OK_QTY);
          if (!stowShelfAreaResult.IsSuccessed)
          {
            return new ExecuteResult { IsSuccessed = false, Message = $"[LMS上架倉別分配]{stowShelfAreaResult.MsgCode} {stowShelfAreaResult.MsgContent}" };
          }
          if (stowShelfAreaResult.Data == null)
          {
            return new ExecuteResult { IsSuccessed = false, Message = $"[LMS上架倉別分配]回傳結果無分配資料" };
          }
          #endregion

          //(4)	呼叫LMS複驗比例確認
          #region 呼叫複驗比例確認API
          var doubleCheckConfirmReq = new DoubleCheckConfirmReq()
          {
            DcCode = acp.DcCode,
            CustCode = acp.CustCode,
            CustInNo = f010201.CUST_ORD_NO,
            ItemList = new List<DoubleCheckConfirmItem>() { new DoubleCheckConfirmItem() { ItemCode = item.ITEM_CODE, Qty = item.OK_QTY } }
          };
          var doubleCheckResult = DoubleCheckService.DoubleCheckConfirm(acp.GupCode, doubleCheckConfirmReq);
          if (!doubleCheckResult.IsSuccessed)
          {
            return new ExecuteResult { IsSuccessed = false, Message = $"[LMS 複驗比例確認]{doubleCheckResult.MsgCode}{doubleCheckResult.MsgContent}" };
          }
          #endregion

          //(5)	產生F0205(揀區、補區、不良品區)
          var stowShelfAreaAssignData = JsonConvert.DeserializeObject<List<StowShelfAreaAssignData>>(JsonConvert.SerializeObject(stowShelfAreaResult.Data));
          var doubleCheckConfirmData = JsonConvert.DeserializeObject<List<DoubleCheckConfirmData>>(JsonConvert.SerializeObject(doubleCheckResult.Data)).First();

          if (stowShelfAreaAssignData.Any())
          {
            foreach (var stowitem in stowShelfAreaAssignData)
            {
              addF0205List.Add(new F0205()
              {
                DC_CODE = item.DC_CODE,
                GUP_CODE = item.GUP_CODE,
                CUST_CODE = item.CUST_CODE,
                STOCK_NO = item.PURCHASE_NO,
                STOCK_SEQ = item.PURCHASE_SEQ,
                RT_NO = item.RT_NO,
                RT_SEQ = item.RT_SEQ,
                PICK_WARE_ID = stowitem.ShelfAreaCode,
                TYPE_CODE = stowitem.Type,
                ITEM_CODE = item.ITEM_CODE,
                NEED_DOUBLE_CHECK = int.Parse(doubleCheckConfirmData.IsNeedDoubleCheck),
                B_QTY = stowitem.Qty,
                A_QTY = 0,
                STATUS = "0",
              });
            }
          }
          else
          {
            return new ExecuteResult { IsSuccessed = false, Message = $"[LMS 複驗比例確認]回傳結果無分配資料" };
          }
        }

        if (CheckOKItems.Any(x => x.NG_QTY > 0))
        {
          foreach (var item in CheckOKItems.Where(x => x.NG_QTY > 0))
          {

            addF0205List.Add(new F0205()
            {
              DC_CODE = item.DC_CODE,
              GUP_CODE = item.GUP_CODE,
              CUST_CODE = item.CUST_CODE,
              STOCK_NO = item.PURCHASE_NO,
              STOCK_SEQ = item.PURCHASE_SEQ,
              RT_NO = item.RT_NO,
              RT_SEQ = item.RT_SEQ,
              PICK_WARE_ID = item.RetrunWarehouseID,
              TYPE_CODE = "R",
              ITEM_CODE = item.ITEM_CODE,
              NEED_DOUBLE_CHECK = 0,
              B_QTY = item.NG_QTY,
              A_QTY = 0,
              STATUS = "0",
            });
          }
        }
        //(6)	如果不需複驗，產生F010205.STATUS=2(驗收回檔)
        #region 處理F010205
        if (addF0205List != null && addF0205List.Any() && addF0205List.All(x => x.NEED_DOUBLE_CHECK == 0))
        {
          F010205Repo.Add(new F010205
          {
            DC_CODE = acp.DcCode,
            GUP_CODE = acp.GupCode,
            CUST_CODE = acp.CustCode,
            STOCK_NO = acp.PurchaseNo,
            RT_NO = acp.RTNo,
            STATUS = "2",
            PROC_FLAG = "0"
          });
        }
        #endregion 處理F010205

      }

      //17.	產生商品與產生對應表[F190303]
      #region 新增/更新 商品廠商對應表(F190303)
      AddorUpdateF190303Data(tmpList.GroupBy(x => new { x.GUP_CODE, x.CUST_CODE, x.ITEM_CODE, x.VNR_CODE, x.PURCHASE_NO }).Select(x => new F190303
      {
        GUP_CODE = x.Key.GUP_CODE,
        CUST_CODE = x.Key.CUST_CODE,
        ITEM_CODE = x.Key.ITEM_CODE,
        VNR_CODE = x.Key.VNR_CODE,
        SOURCE_NO = x.Key.PURCHASE_NO
      }).ToList());
      #endregion

      //18.	更新進場管理離場時間[F020103]
      #region 更新進場管理離場時間
      var updF020103s = F020103Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.PURCHASE_NO == acp.PurchaseNo).ToList();
      var outTime = DateTime.Now.ToString("HHmm");
      updF020103s.ForEach((x) => { if (string.IsNullOrEmpty(x.OUTTIME)) { x.OUTTIME = outTime; } });
      if (updF020103s.Any())
        F020103Repo.BulkUpdate(updF020103s);
      #endregion

      //19.	更新進倉單狀態，入庫狀態
      #region 更新入庫狀態
      f010201.STATUS = "1";
      f010201.CHECKCODE_EDI_STATUS = "1";
      F010201Repo.Update(f010201);
      #endregion

      #region 進倉單結案
      //貨主驗收後不進行上傳檔案處理
      var f1909 = F1909Repo.GetF1909(acp.GupCode, acp.CustCode);
      //是否驗收後上傳檔案 如果值為0, 且進貨數等於驗收總量則直接更新進倉單狀態至結案
      if (rtMode == "1" || (f1909 != null && f1909.ISUPLOADFILE == "0"))
      {
        var f010202s = F010202Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.STOCK_NO == acp.PurchaseNo).ToList();
        //取得該進貨單的驗收上架結果表
        var f010204s = F010204Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.STOCK_NO == acp.PurchaseNo).ToList();

        // 刪除暫存驗收檔
        F02020101Repo.Delete(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo, acp.RTNo);

        //檢核進倉單是否所有商品都已經驗收完成 如果都完成就結案 
        var isAllRecv = true;
        foreach (var item in f010202s)
        {
          //取得該進貨項次已驗收數
          var totalRecQty = f010204s.Where(x => x.STOCK_SEQ == item.STOCK_SEQ).Sum(x => x.TOTAL_REC_QTY);
          //取得該進貨項次的本次驗收數
          var sumRecvQty = addF020201List.Where(x => x.PURCHASE_SEQ == item.STOCK_SEQ.ToString()).Sum(x => x.RECV_QTY);
          if (totalRecQty + sumRecvQty < item.STOCK_QTY)
          {
            isAllRecv = false;
            break;
          }
        }
        if (isAllRecv)
        {
          //更新進倉單為已結案
          f010201.STATUS = "2";//結案
          F010201Repo.Update(f010201);
          //更新來源單據狀態為已結案
          SharedService.UpdateSourceNoStatus(SourceType.Stock, acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo, f010201.STATUS);
        }
      }
      #endregion

      if (addF0205List.Any())
        F0205Repo.BulkInsert(addF0205List);
      if (updF02020101List.Any())
        F02020101Repo.BulkUpdate(updF02020101List);
      if (addF02020104List.Any())
        F02020104Repo.BulkInsert(addF02020104List);
      if (addF020201List.Any())
        F020201Repo.BulkInsert(addF020201List);
      if (updF020302List.Any())
        F020302Repo.BulkUpdate(updF020302List);
      if (updF02020109List.Any())
        F02020109Repo.BulkUpdate(updF02020109List);

      return new ExecuteResult(true);
    }

    #region GetItemMakeRtSeq
    /// <summary>
    /// 即時新增/更新 驗收批號的流水號紀錄檔(F020203)
    /// </summary>
    /// <param name="acp"></param>
    /// <param name="f020203Repo"></param>
    /// <param name="today"></param>
    /// <param name="tmp"></param>
    /// <returns></returns>
    public int GetItemMakeRtSeq(F02020101 tmp)
    {
      var f020203Repo = new F020203Repository(Schemas.CoreSchema);
      var f020203 = f020203Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
              () =>
              {
                var lockF020203 = f020203Repo.LockF020203();
                var currF020203 = f020203Repo.GetDataByKey(tmp.DC_CODE, tmp.GUP_CODE, tmp.CUST_CODE, tmp.ITEM_CODE, DateTime.Today);
                if (currF020203 == null)
                {
                  currF020203 = new F020203
                  {
                    DC_CODE = tmp.DC_CODE,
                    GUP_CODE = tmp.GUP_CODE,
                    CUST_CODE = tmp.CUST_CODE,
                    ITEM_CODE = tmp.ITEM_CODE,
                    RT_DATE = DateTime.Today,
                    RT_SEQ = 1
                  };
                  // 若沒有在資料庫則新增 目前已使用流水號
                  f020203Repo.Add(currF020203);
                }
                else
                {
                  currF020203.RT_SEQ++;
                  f020203Repo.Update(currF020203);
                }
                return currF020203;
              });
      return f020203.RT_SEQ;
    }
    #endregion

    #region CreateF02020104sExistF020302s
    public List<F02020104> CreateF02020104sExistF020302s(F02020101 tmp, List<F020302> f020302Data, List<F2501> f2501Data, int maxLogSeq)
    {
      List<F02020104> res = new List<F02020104>();

      for (int index = 0; index < f020302Data.Count; index++)
      {
        var currData = f020302Data[index];

        var status = f2501Data.Where(y => y.SERIAL_NO == currData.SERIAL_NO).Select(z => z.STATUS).SingleOrDefault();

        res.Add(new F02020104
        {
          PURCHASE_NO = tmp.PURCHASE_NO,
          PURCHASE_SEQ = tmp.PURCHASE_SEQ,
          LOG_SEQ = index + 1 + maxLogSeq,
          ITEM_CODE = currData.ITEM_CODE,
          SERIAL_NO = currData.SERIAL_NO,
          STATUS = string.IsNullOrWhiteSpace(status) ? null : status,
          ISPASS = "1",
          DC_CODE = currData.DC_CODE,
          GUP_CODE = currData.GUP_CODE,
          CUST_CODE = currData.CUST_CODE,
          RT_NO = tmp.RT_NO,
          BATCH_NO = currData.BATCH_NO
        });
      }

      return res;
    }
    #endregion

    #region 序號檢核與更新序號狀態
    /// <summary>
    /// 序號檢核與更新序號狀態
    /// </summary>
    /// <param name="tmp"></param>
    /// <param name="ordProp"></param>
    /// <param name="f020302Datas"></param>
    /// <returns></returns>
    protected ExecuteResult UpdateF2501(F02020101 tmp, string ordProp, List<F020302> f020302s, List<F02020109> f02020109s)
    {
      var logSvc = new LogService("商品檢驗_" + DateTime.Now.ToString("yyyyMMdd"));
      SerialNoService.CommonService = CommonService;
      var result = SerialNoService.CheckItemLargeSerialWithBeforeInWarehouse(tmp.GUP_CODE, tmp.CUST_CODE, tmp.ITEM_CODE, f020302s.Select(x => x.SERIAL_NO).Distinct().ToList()).ToList();
      if (result.Any(x => !x.Checked))
      {
        return new ExecuteResult(false, string.Join(Environment.NewLine, result.Select(x => x.Message)));
      }
      else
      {
        var addF2501List = new List<F2501>();
        var updF2501List = new List<F2501>();
        var delSnList = new List<string>();
        Boolean needMarkActivated;
        foreach (var item in f020302s)
        {
          var logDt = DateTime.Now;
          var checkSerialList = result.First(x => x.SerialNo == item.SERIAL_NO);

          // No.2091 若當前序號在F02020109中查得到，更新F2501需要註記為不良品
          if (f02020109s.Select(o => o.SERIAL_NO).Contains(item.SERIAL_NO))
            needMarkActivated = true;
          else
            needMarkActivated = false;

          //更新F2501
          var updResult = SerialNoService.UpdateSerialNoFull(ref addF2501List, ref updF2501List, ref delSnList, item.DC_CODE, item.GUP_CODE, item.CUST_CODE, "A1", checkSerialList,
                            tmp.PURCHASE_NO, tmp.VNR_CODE, item.VALID_DATE, null,
                            item.PO_NO, ordProp, null, item.PUK,
                            item.CELL_NUM, item.BATCH_NO, null, needMarkActivated);
          if (!updResult.IsSuccessed)
            return updResult;
          else
            item.STATUS = "1";
          logSvc.Log($"更新序號{item.SERIAL_NO} {logSvc.DateDiff(logDt, DateTime.Now)}");
        }

        if (delSnList.Any())
          F2501Repo.DeleteBySnList(tmp.GUP_CODE, tmp.CUST_CODE, delSnList);
        if (addF2501List.Any())
          F2501Repo.BulkInsert(addF2501List);
        if (updF2501List.Any())
          F2501Repo.BulkUpdate(updF2501List);
      }
      return new ExecuteResult(true);
    }
    #endregion

    #region 產生進貨暫存倉庫存資料
    Func<F1913, string, string, string, string, string, DateTime, DateTime, string, string, string, string, string, bool> F1913Func = Find1913;
    private static bool Find1913(F1913 f1913, string dcCode, string gupCode, string custCode, string itemCode, string locCode, DateTime validDate, DateTime enterDate, string vnrCode, string serialNo, string boxCtrlNo, string palletCtrlNo, string makeNo)
    {
      return f1913.DC_CODE == dcCode && f1913.GUP_CODE == gupCode && f1913.CUST_CODE == custCode && f1913.LOC_CODE == locCode && f1913.ITEM_CODE == itemCode && f1913.VALID_DATE == validDate && f1913.ENTER_DATE == enterDate && f1913.VNR_CODE == vnrCode && f1913.SERIAL_NO == serialNo && f1913.BOX_CTRL_NO == boxCtrlNo && f1913.PALLET_CTRL_NO == palletCtrlNo && f1913.MAKE_NO == makeNo;
    }

    /// <summary>
    /// 產生進貨暫存倉庫存資料
    /// </summary>
    /// <param name="tmp"></param>
    /// <param name="locCode"></param>
    /// <param name="f020302s"></param>
    /// <param name="returnStocks"></param>
    protected void InsertOrUpdateStock(F02020101 tmp, string locCode, List<F020302> f020302s, ref List<F1913> returnStocks)
    {
      var validDate = DateTime.MaxValue.Date;
      if (tmp.VALI_DATE.HasValue)
        validDate = tmp.VALI_DATE.Value;
      var enterDate = DateTime.Today;
      var vnrCode = "000000";
      var palletCtrlNo = "0";
      var makeNo = tmp.MAKE_NO?.Trim();
      if (string.IsNullOrWhiteSpace(makeNo))
        makeNo = "0";
      var boxCtrlNo = "0";

      var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
      var serialData = new List<string>() { "0" };
      var f1903 = CommonService.GetProduct(tmp.GUP_CODE, tmp.CUST_CODE, tmp.ITEM_CODE);
      if (f1903.BUNDLE_SERIALLOC == "1") //序號綁儲位
      {
        serialData = f020302s.Select(o => o.SERIAL_NO).ToList();
        if (!serialData.Any())
          serialData.Add("0");
      }
      bool hasSerial = serialData.Any(x => x != "0");

      foreach (var serialNo in serialData)
      {
        var returnStock = returnStocks.FirstOrDefault(o => F1913Func(o, tmp.DC_CODE, tmp.GUP_CODE, tmp.CUST_CODE, tmp.ITEM_CODE, locCode, validDate, enterDate, vnrCode, serialNo, boxCtrlNo, palletCtrlNo, makeNo));
        var f1913 = returnStock ??
              f1913Repo.Find(o => o.DC_CODE == tmp.DC_CODE && o.GUP_CODE == tmp.GUP_CODE && o.CUST_CODE == tmp.CUST_CODE && o.ITEM_CODE == tmp.ITEM_CODE && o.LOC_CODE == locCode && o.VALID_DATE == validDate && o.ENTER_DATE == enterDate && o.VNR_CODE == vnrCode && o.SERIAL_NO == serialNo && o.BOX_CTRL_NO == boxCtrlNo && o.PALLET_CTRL_NO == palletCtrlNo && o.MAKE_NO == makeNo);
        if (f1913 != null)
        {
          f1913.QTY += (!hasSerial) ? (tmp.RECV_QTY ?? 0) : 1;
          if (returnStock == null)
            returnStocks.Add(f1913);
        }
        else
        {
          f1913 = new F1913
          {
            DC_CODE = tmp.DC_CODE,
            GUP_CODE = tmp.GUP_CODE,
            CUST_CODE = tmp.CUST_CODE,
            ITEM_CODE = tmp.ITEM_CODE,
            LOC_CODE = locCode,
            VALID_DATE = validDate,
            ENTER_DATE = enterDate,
            VNR_CODE = vnrCode,
            SERIAL_NO = serialNo,
            QTY = (!hasSerial) ? (tmp.RECV_QTY ?? 0) : 1,
            BOX_CTRL_NO = boxCtrlNo,
            PALLET_CTRL_NO = palletCtrlNo,
            MAKE_NO = makeNo
          };
          returnStocks.Add(f1913);
        }
      }
    }

    #endregion

    #region 產生商品驗收調撥單
    /// <summary>
    /// 產生商品驗收調撥單
    /// </summary>
    /// <returns></returns>
    protected ReturnNewAllocationResult CreateAcceptanceAllocation(AcceptanceConfirmPara acp, F1912 srcLoc, List<F1913> returnStocks, List<F02020101> f02020101s)
    {

      var group = f02020101s.GroupBy(x => new { x.ITEM_CODE, x.VALI_DATE, x.MAKE_NO, x.TARWAREHOUSE_ID });
      var newAllocationParam = new NewAllocationItemParam
      {
        GupCode = acp.GupCode,
        CustCode = acp.CustCode,
        AllocationDate = DateTime.Today,
        SourceType = "04",
        SourceNo = acp.PurchaseNo,
        IsExpendDate = true,
        SrcDcCode = acp.DcCode,
        SrcWarehouseId = srcLoc.WAREHOUSE_ID,
        TarDcCode = acp.DcCode,
        AllocationType = AllocationType.Both,
        AllocationTypeCode = "0",
        ReturnStocks = returnStocks,
        isIncludeResupply = true,
        SrcStockFilterDetails = group.Select((x, rowIndex) => new StockFilter
        {
          DataId = rowIndex,
          ItemCode = x.Key.ITEM_CODE,
          LocCode = srcLoc.LOC_CODE,
          Qty = x.Sum(y => y.RECV_QTY) ?? 0,
          ValidDates = x.Key.VALI_DATE.HasValue ? new List<DateTime> { x.Key.VALI_DATE.Value } : new List<DateTime>(),
          EnterDates = new List<DateTime> { DateTime.Today },
          BoxCtrlNos = new List<string> { "0" },
          MakeNos = string.IsNullOrWhiteSpace(x.Key.MAKE_NO) ? new List<string> { "0" } : new List<string> { x.Key.MAKE_NO?.Trim() },
        }).ToList(),
        SrcLocMapTarLocs = group.Select((x, rowIndex) => new SrcLocMapTarLoc
        {
          DataId = rowIndex,
          ItemCode = x.Key.ITEM_CODE,
          SrcLocCode = srcLoc.LOC_CODE,
          TarWarehouseId = x.Key.TARWAREHOUSE_ID,
          ValidDate = x.Key.VALI_DATE,
          MakeNo = x.Key.MAKE_NO
        }).ToList()
      };

      // 將序號的ItemCode 填入SerialNos
      var serialItemCodes = returnStocks.Where(x => x.SERIAL_NO != "0").Select(x => x.ITEM_CODE).Distinct().ToList();

      serialItemCodes.ForEach(itemCode =>
      {
        int index = 0;

        // 找出符合ItemCode執行迴圈
        newAllocationParam.SrcStockFilterDetails.Where(x => x.ItemCode == itemCode).ToList().ForEach(item =>
        {
          // 找出符合的商品
          var serialNos = returnStocks.Where(x => x.ITEM_CODE == item.ItemCode &&
                                                  x.LOC_CODE == item.LocCode &&
                                                  x.VALID_DATE == item.ValidDates.FirstOrDefault() &&
                                                  x.ENTER_DATE == item.EnterDates.FirstOrDefault() &&
                                                  x.BOX_CTRL_NO == item.BoxCtrlNos.FirstOrDefault() &&
                                                  x.MAKE_NO == item.MakeNos.FirstOrDefault());

          if (serialNos.Any())
          {
            // 因上層有將不良品往最後擺，所以只需依順序找出SerialNo
            int takeCnt = Convert.ToInt32(item.Qty);
            item.SerialNos = serialNos.Skip(index).Take(takeCnt).Select(x => x.SERIAL_NO).ToList();
            index += takeCnt;
          }
        });
      });

      //7.設定預設的建議儲位
      if (acp.IsPickLocFirst)
      {
        SetDefaultSugLocCode(acp.DcCode, acp.GupCode, acp.CustCode, newAllocationParam.SrcLocMapTarLocs);
      }

      var returnAllocationResult = SharedService.CreateOrUpdateAllocation(newAllocationParam);
      if (returnAllocationResult.Result.IsSuccessed)
        SharedService.BulkAllocationToAllDown(returnAllocationResult.AllocationList);
      return returnAllocationResult;
    }

    #endregion

    #region 設定預設建議揀貨儲位
    /// <summary>
    /// 設定預設建議儲位(取得此商品已存在庫存的第一個儲位)
    /// 必須倉別為良品倉
    /// 必須儲區型態為揀貨區(A)
    /// 必須數量>0
    /// 必須儲位非凍結進(02)和凍結進出(04)
    /// </summary>
    /// <param name="dcCode">物流中心</param>
    /// <param name="gupCode">業主</param>
    /// <param name="custCode">貨主</param>
    /// <param name="srcLocMapTarLocs">來源商品儲位對應目的儲位設定</param>
    private void SetDefaultSugLocCode(string dcCode, string gupCode, string custCode, List<SrcLocMapTarLoc> srcLocMapTarLocs)
    {
      var items = F1913Repo.GetSuggestLocsByStock(dcCode, gupCode, custCode, srcLocMapTarLocs.Select(o => o.ItemCode).Distinct().ToList());
      foreach (var item in srcLocMapTarLocs)
      {
        SuggestLocItem loc;
        if (string.IsNullOrWhiteSpace(item.TarWarehouseId)) // 未指定上架倉庫 =>取得此商品庫存第一個儲位
          loc = items.FirstOrDefault(o => o.ITEM_CODE == item.ItemCode);
        else //指定上架倉庫 =>取得此指定上架倉庫此商品庫存第一個儲位
          loc = items.FirstOrDefault(x => x.WAREHOUSE_ID == item.TarWarehouseId && x.ITEM_CODE == item.ItemCode);
        //如果有找到儲位 設定此商品目的儲位
        if (loc != null)
        {
          item.TarLocCode = loc.LOC_CODE;
          item.TarWarehouseId = loc.WAREHOUSE_ID;
        }
      }
    }
    #endregion

    #region 不良品拆單
    public void CreateDefectItem(ref List<F02020101> hasRecvList, List<F02020109> f02020109s)
    {
      hasRecvList = (from A in hasRecvList
                     join B in f02020109s on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, STOCK_NO = A.PURCHASE_NO, STOCK_SEQ = Convert.ToInt32(A.PURCHASE_SEQ) }
                     equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.STOCK_NO, B.STOCK_SEQ } into g
                     from C in g.DefaultIfEmpty()
                     select new
                     {
                       // 建立正常調撥單
                       A.PURCHASE_NO,
                       A.PURCHASE_SEQ,
                       A.VNR_CODE,
                       A.ITEM_CODE,
                       A.RECE_DATE,
                       A.VALI_DATE,
                       A.MADE_DATE,
                       A.ORDER_QTY,
                       RECV_QTY = A.RECV_QTY - g.Sum(x => x.DEFECT_QTY ?? 1),
                       A.CHECK_QTY,
                       A.STATUS,
                       A.CHECK_ITEM,
                       A.CHECK_SERIAL,
                       A.ISPRINT,
                       A.ISUPLOAD,
                       A.DC_CODE,
                       A.GUP_CODE,
                       A.CUST_CODE,
                       A.CRT_STAFF,
                       A.CRT_DATE,
                       A.UPD_STAFF,
                       A.UPD_DATE,
                       A.CRT_NAME,
                       A.UPD_NAME,
                       A.ISSPECIAL,
                       A.SPECIAL_DESC,
                       A.SPECIAL_CODE,
                       A.RT_NO,
                       A.RT_SEQ,
                       A.IN_DATE,
                       A.TARWAREHOUSE_ID,
                       A.QUICK_CHECK,
                       A.MAKE_NO
                     }).Distinct().Select(x => new F02020101
                     {
                       PURCHASE_NO = x.PURCHASE_NO,
                       PURCHASE_SEQ = x.PURCHASE_SEQ,
                       VNR_CODE = x.VNR_CODE,
                       ITEM_CODE = x.ITEM_CODE,
                       RECE_DATE = x.RECE_DATE,
                       VALI_DATE = x.VALI_DATE,
                       MADE_DATE = x.MADE_DATE,
                       ORDER_QTY = x.ORDER_QTY,
                       RECV_QTY = x.RECV_QTY,
                       CHECK_QTY = x.CHECK_QTY,
                       STATUS = x.STATUS,
                       CHECK_ITEM = x.CHECK_ITEM,
                       CHECK_SERIAL = x.CHECK_SERIAL,
                       ISPRINT = x.ISPRINT,
                       ISUPLOAD = x.ISUPLOAD,
                       DC_CODE = x.DC_CODE,
                       GUP_CODE = x.GUP_CODE,
                       CUST_CODE = x.CUST_CODE,
                       CRT_STAFF = x.CRT_STAFF,
                       CRT_DATE = x.CRT_DATE,
                       UPD_STAFF = x.UPD_STAFF,
                       UPD_DATE = x.UPD_DATE,
                       CRT_NAME = x.CRT_NAME,
                       UPD_NAME = x.UPD_NAME,
                       ISSPECIAL = x.ISSPECIAL,
                       SPECIAL_DESC = x.SPECIAL_DESC,
                       SPECIAL_CODE = x.SPECIAL_CODE,
                       RT_NO = x.RT_NO,
                       RT_SEQ = x.RT_SEQ,
                       IN_DATE = x.IN_DATE,
                       TARWAREHOUSE_ID = x.TARWAREHOUSE_ID,
                       QUICK_CHECK = x.QUICK_CHECK,
                       MAKE_NO = x.MAKE_NO
                     }).Concat(from D in hasRecvList
                               join E in f02020109s on new { D.DC_CODE, D.GUP_CODE, D.CUST_CODE, STOCK_NO = D.PURCHASE_NO, STOCK_SEQ = Convert.ToInt32(D.PURCHASE_SEQ) }
                               equals new { E.DC_CODE, E.GUP_CODE, E.CUST_CODE, E.STOCK_NO, E.STOCK_SEQ }
                               group E by new
                               {
                                 //建立不良品調撥單
                                 D.PURCHASE_NO,
                                 D.PURCHASE_SEQ,
                                 D.VNR_CODE,
                                 D.ITEM_CODE,
                                 D.RECE_DATE,
                                 D.VALI_DATE,
                                 D.MADE_DATE,
                                 D.ORDER_QTY,
                                 D.RECV_QTY,
                                 D.CHECK_QTY,
                                 D.STATUS,
                                 D.CHECK_ITEM,
                                 D.CHECK_SERIAL,
                                 D.ISPRINT,
                                 D.ISUPLOAD,
                                 D.DC_CODE,
                                 D.GUP_CODE,
                                 D.CUST_CODE,
                                 D.CRT_STAFF,
                                 D.CRT_DATE,
                                 D.UPD_STAFF,
                                 D.UPD_DATE,
                                 D.CRT_NAME,
                                 D.UPD_NAME,
                                 D.ISSPECIAL,
                                 D.SPECIAL_DESC,
                                 D.SPECIAL_CODE,
                                 D.RT_NO,
                                 D.RT_SEQ,
                                 D.IN_DATE,
                                 TARWAREHOUSE_ID = E.WAREHOUSE_ID,
                                 D.QUICK_CHECK,
                                 D.MAKE_NO,
                                 DEFECT_QTY = E.DEFECT_QTY ?? 1
                               } into g
                               select new F02020101
                               {
                                 PURCHASE_NO = g.Key.PURCHASE_NO,
                                 PURCHASE_SEQ = g.Key.PURCHASE_SEQ,
                                 VNR_CODE = g.Key.VNR_CODE,
                                 ITEM_CODE = g.Key.ITEM_CODE,
                                 RECE_DATE = g.Key.RECE_DATE,
                                 VALI_DATE = g.Key.VALI_DATE,
                                 MADE_DATE = g.Key.MADE_DATE,
                                 ORDER_QTY = g.Key.ORDER_QTY,
                                 RECV_QTY = g.Sum(x => x.DEFECT_QTY),
                                 CHECK_QTY = g.Key.CHECK_QTY,
                                 STATUS = g.Key.STATUS,
                                 CHECK_ITEM = g.Key.CHECK_ITEM,
                                 CHECK_SERIAL = g.Key.CHECK_SERIAL,
                                 ISPRINT = g.Key.ISPRINT,
                                 ISUPLOAD = g.Key.ISUPLOAD,
                                 DC_CODE = g.Key.DC_CODE,
                                 GUP_CODE = g.Key.GUP_CODE,
                                 CUST_CODE = g.Key.CUST_CODE,
                                 CRT_STAFF = g.Key.CRT_STAFF,
                                 CRT_DATE = g.Key.CRT_DATE,
                                 UPD_STAFF = g.Key.UPD_STAFF,
                                 UPD_DATE = g.Key.UPD_DATE,
                                 CRT_NAME = g.Key.CRT_NAME,
                                 UPD_NAME = g.Key.UPD_NAME,
                                 ISSPECIAL = g.Key.ISSPECIAL,
                                 SPECIAL_DESC = g.Key.SPECIAL_DESC,
                                 SPECIAL_CODE = g.Key.SPECIAL_CODE,
                                 RT_NO = g.Key.RT_NO,
                                 RT_SEQ = g.Key.RT_SEQ,
                                 IN_DATE = g.Key.IN_DATE,
                                 TARWAREHOUSE_ID = g.Key.TARWAREHOUSE_ID,
                                 QUICK_CHECK = g.Key.QUICK_CHECK,
                                 MAKE_NO = g.Key.MAKE_NO
                               }).ToList();
    }
    #endregion

    #region AddorUpdateF190303Data
    Func<F190303, string, string, string, string, bool> F190303Func = FindF190303;
    private static bool FindF190303(F190303 f190303, string gupCode, string custCode, string itemCode, string vnrCode)
    {
      return f190303.GUP_CODE == gupCode && f190303.CUST_CODE == custCode && f190303.ITEM_CODE == itemCode && f190303.VNR_CODE == vnrCode;
    }
    public ExecuteResult AddorUpdateF190303Data(List<F190303> datas)
    {
      var addF190303List = new List<F190303>();
      var updF190303List = new List<F190303>();
      foreach (var item in datas)
      {
        bool isDbUpdate = false;
        var f190303 = addF190303List.Find(x => F190303Func(x, item.GUP_CODE, item.CUST_CODE, item.ITEM_CODE, item.VNR_CODE));
        if (f190303 == null)
          f190303 = updF190303List.Find(x => F190303Func(x, item.GUP_CODE, item.CUST_CODE, item.ITEM_CODE, item.VNR_CODE));
        if (f190303 == null)
        {
          f190303 = F190303Repo.Find(x => x.GUP_CODE == item.GUP_CODE && x.CUST_CODE == item.CUST_CODE && x.ITEM_CODE == item.ITEM_CODE && x.VNR_CODE == item.VNR_CODE);
          isDbUpdate = true;
        }
        if (f190303 == null)
          addF190303List.Add(item);
        else
        {
          f190303.SOURCE_NO = item.SOURCE_NO;
          if (isDbUpdate)
            updF190303List.Add(f190303);
        }
      }
      F190303Repo.BulkInsert(addF190303List);
      F190303Repo.BulkUpdate(updF190303List);
      return new ExecuteResult(true);
    }
    #endregion

    #endregion

    #region 刪除驗收紀錄
    /// <summary>
    /// 刪除驗收紀錄
    /// </summary>
    /// <param name="dap"></param>
    /// <returns></returns>
    public ApiResult DeleteAcceptanceData(DeleteAcceptanceDataParam dap)
    {
      //取得進倉單資料
      var f010201 = F010201Repo.GetDatasByStockNo(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo);
      if (f010201 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21902", MsgContent = "進倉單資料不存在" };
      //取得進倉驗收檔
      var f020201s = F020201Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dap.DcCode && x.GUP_CODE == dap.GupCode && x.CUST_CODE == dap.CustCode
                                                            && x.PURCHASE_NO == dap.PurchaseNo && x.RT_NO == dap.RTNo).ToList();
      if (f020201s.Any())
        return new ApiResult { IsSuccessed = false, MsgCode = "21945", MsgContent = "此驗收單已完成驗收，無法取消驗收" };

      List<F02020101> otherF02020101s = null;
      //Pda需傳入"PurchaseSeq進貨項次"
      if (!string.IsNullOrWhiteSpace(dap.PurchaseSeq))
        otherF02020101s = F02020101Repo.GetOtherSeqFinishCheckDatas(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo, dap.PurchaseSeq, dap.RTNo).ToList();

      //取得進倉暫存檔其他已驗收項次
      //只要存在已驗收的其他項次，只更新目前該項次暫存檔的資料
      if (otherF02020101s != null && otherF02020101s.Any())
      {
        //與"驗收明細資料查詢"新增F02020101時相同，取得初始未驗收數與效期
        var f010204 = F010204Repo.GetData(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo, Convert.ToInt32(dap.PurchaseSeq));
        var f010202 = F010202Repo.GetData(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo, Convert.ToInt32(dap.PurchaseSeq));
        int recvQty = 0;
        DateTime valiDate = new DateTime(9999, 12, 31);
        if (f010204 != null)
        {
          if (f010204.STOCK_QTY > f010204.TOTAL_REC_QTY)
          {
            recvQty = f010204.STOCK_QTY - f010204.TOTAL_REC_QTY;
            valiDate = f010202.VALI_DATE.HasValue ? f010202.VALI_DATE.Value : new DateTime(9999, 12, 31);
          }
          else
            return new ApiResult { IsSuccessed = false, MsgCode = "21924", MsgContent = "此商品已無可驗收數量" };
        }
        else
        {
          recvQty = f010202.STOCK_QTY;
          valiDate = f010202.VALI_DATE.HasValue ? f010202.VALI_DATE.Value : new DateTime(9999, 12, 31);
        }

        F02020101Repo.UpdateForCancelAcceptance(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo, dap.PurchaseSeq, dap.RTNo, recvQty, valiDate);
        F02020102Repo.Delete(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo, dap.PurchaseSeq, dap.RTNo);
        F02020109Repo.Delete(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo, dap.PurchaseSeq);
        ////先刪除F020302 再刪F020301 最後才刪F02020104 有順序性 
        F020302Repo.DeleteWithCancelAcceptance(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo, dap.PurchaseSeq, dap.RTNo);
        F020301Repo.Delete(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo);
        F02020104Repo.DeleteF02020104(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo, dap.PurchaseSeq, dap.RTNo);
      }
      //不存在其他已驗收的項次，則刪除所有驗收資料，並解鎖該進倉單(STATUS='3':取消驗收)
      else
      {
        F02020101Repo.Delete(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo, dap.RTNo);
        F02020102Repo.Delete(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo, dap.RTNo);
        F02020109Repo.Delete(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo);
        //先刪除F020302 再刪F020301 最後才刪F02020104 有順序性 
        F020302Repo.DeleteWithCancelAcceptance(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo, dap.RTNo);
        F020301Repo.Delete(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo);
        F02020104Repo.Delete(dap.DcCode, dap.GupCode, dap.CustCode, dap.PurchaseNo, dap.RTNo);
      }
      return new ApiResult { IsSuccessed = true, MsgCode = "10005",MsgContent = "執行成功" };
    }
    #endregion


    #region 儲存商品驗收結果
    /// <summary>
    /// 儲存商品驗收結果
    /// </summary>
    /// <param name="dap"></param>
    /// <returns></returns>
    public ApiResult SaveRecvItem(SaveRecvItemParam sip)
    {
      //2.	[B]=撈出進倉驗收暫存資料
      var f02020101 = F02020101Repo.GetF02020101ByRt(sip.DcCode, sip.GupCode, sip.CustCode, sip.RtNo, sip.RtSeq);
      if (f02020101 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21937", MsgContent = "進倉單驗收暫存資料不存在" };

      //檢查驗收數量有沒有超過訂購數
      var f010204s = F010204Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == sip.DcCode && x.GUP_CODE == sip.GupCode && x.CUST_CODE == sip.CustCode && x.STOCK_NO == sip.PurchaseNo && x.STOCK_SEQ == Convert.ToInt32(sip.PurchaseSeq)).ToList();
      if (f010204s != null && f010204s.Any())
      {
        if (f010204s.Sum(x => x.STOCK_QTY) - f010204s.Sum(x => x.TOTAL_REC_QTY) - sip.RecvQty < 0)
          return new ApiResult { IsSuccessed = false, MsgCode = "21944", MsgContent = "驗收總數量不得超過進貨數" };
      }
      else if (f02020101.ORDER_QTY - sip.RecvQty < 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "21944", MsgContent = "驗收總數量不得超過進貨數" };

      // 驗收數量必須大於等於已設定不良品數量總和
      if (f02020101.DEFECT_QTY > sip.RecvQty)
        return new ApiResult { IsSuccessed = false, MsgCode = "21951", MsgContent = "驗收數量必須大於等於已設定不良品數量總和" };
      //4.	[C]=取得商品主檔
      var f1903 = CommonService.GetProduct(f02020101.GUP_CODE, f02020101.CUST_CODE, f02020101.ITEM_CODE);

      if (!f1903.FIRST_IN_DATE.HasValue && !f1903.ALL_DLN.HasValue && !f1903.ALL_SHP.HasValue && sip.NeedExpired == "1")
      {
        //(1)	[D]=呼叫ItemService.GetItemAllDlnAndAllShp(SaveDay)
        var getAllDlnAndAllShp = ItemService.GetItemAllDlnAndAllShp(sip.SaveDay);
				sip.AllowDelvDay = (short)getAllDlnAndAllShp.ALL_DLN;
        sip.AllowShipDay = getAllDlnAndAllShp.ALL_SHP;
      }

      //3.	如果為效期商品NeedExpired=1且不良品數!=驗收數[B]. DEFECT_QTY!= RecvQty，檢查允收天數
      if (sip.NeedExpired == "1" && f02020101.DEFECT_QTY != sip.RecvQty)
      {
        if (sip.ValidDate.AddDays(-1 * sip.AllowDelvDay.Value) < DateTime.Today)
          return new ApiResult { IsSuccessed = false, MsgCode = "21938", MsgContent = string.Format("該商品效期必須大於允收日期{0}，不符合正常商品允收規則，請利用設定不良品驗退", DateTime.Today.AddDays(sip.AllowDelvDay.Value).ToString("yyyy/MM/dd")) };
        if (sip.ValidDate > DateTime.Today.AddDays(sip.SaveDay.Value))
          return new ApiResult { IsSuccessed = false, MsgCode = "21939", MsgContent = string.Format("本商品的總保存天數為{0}，商品效期不可以大於{1}", sip.SaveDay, DateTime.Today.AddDays(sip.SaveDay.Value).ToString("yyyy/MM/dd")) };
      }
      //5.	如果為商品主檔首次驗收[FIRST_IN_DATE=null or 空白]  AND 商品主檔允收天數[F1903.ALL_DLN]=NULL AND 商品主檔警示天數[F1903.ALL_SHP]=NULL AND 傳入參數.NeedExpired=1時，計算允收天數與警示天數
      //6.	檢查商品主檔是否序號商品!= 傳入的BundleSerialNo
      if (f1903.BUNDLE_SERIALNO != sip.BundleSerialNo)
      {
        #region 呼叫更新商品主檔API
        var apiRes = UpdatdeItemInfoService.UpdateItemInfo(
          f1903.CUST_CODE,
          f1903.CUST_ITEM_CODE,
          sip.BundleSerialNo,
          string.IsNullOrWhiteSpace(f1903.EAN_CODE1) ? f1903.EAN_CODE4 : f1903.EAN_CODE1);
        if (!apiRes.IsSuccessed)
          return new ApiResult { IsSuccessed = false, MsgCode = "20052", MsgContent = apiRes.Message };
        #endregion
      }
      //7.	[E]= 取得商品材積檔
      var f1905 = F1905Repo.GetData(f02020101.GUP_CODE, f02020101.CUST_CODE, f02020101.ITEM_CODE);
      string newItemSize = f1903.ITEM_SIZE;
      //8.	檢查商品材積檔相關欄位是否有任一個欄位要異動，若有更新商品材積檔，若無不更新
      if (f1905.PACK_LENGTH != sip.PackLength || f1905.PACK_WIDTH != sip.PackWidth
        || f1905.PACK_HIGHT != sip.PackHight || f1905.PACK_WEIGHT != sip.PackWeight)
      {
        F1905Repo.UpdateFields(
          new { PACK_LENGTH = sip.PackLength, PACK_WIDTH = sip.PackWidth, PACK_HIGHT = sip.PackHight, PACK_WEIGHT = sip.PackWeight },
          x => x.GUP_CODE == f02020101.GUP_CODE &&
               x.CUST_CODE == f02020101.CUST_CODE &&
               x.ITEM_CODE == f02020101.ITEM_CODE);

        //10.	若商品材積檔有更新，需增加更新商品主檔尺寸= 商品長+*+商品寬+*+商品高(組合成文字)
        newItemSize = string.Format("{0}*{1}*{2}", sip.PackLength, sip.PackWidth, sip.PackHight);
      }
      //9.	檢查商品主檔相關欄位是否有任一個欄位要異動，若有更新商品主檔，若無不更新
      if (f1903.EAN_CODE1 != sip.EanCode1
        || f1903.EAN_CODE2 != sip.EanCode2
        || f1903.EAN_CODE3 != sip.EanCode3
        || f1903.NEED_EXPIRED != sip.NeedExpired
        || (sip.SaveDay > 0 && f1903.SAVE_DAY.Value != sip.SaveDay)
        || (sip.AllowDelvDay > 0 && (!f1903.ALL_DLN.HasValue || f1903.ALL_DLN != sip.AllowDelvDay))
        || (sip.AllowShipDay > 0 && (!f1903.ALL_SHP.HasValue || f1903.ALL_SHP != sip.AllowShipDay))
        || f1903.BUNDLE_SERIALNO != sip.BundleSerialNo
        || f1903.TMPR_TYPE != sip.TmprType
        || f1903.ISAPPLE != sip.IsApple
        || f1903.IS_PRECIOUS != sip.IsPrecious
        || f1903.FRAGILE != sip.IsFragile
        || f1903.IS_EASY_LOSE != sip.IsEasyLose
        || f1903.IS_MAGNETIC != sip.IsMagentic
        || f1903.SPILL != sip.IsSpill
        || f1903.IS_PERISHABLE != sip.IsPerishable
        || f1903.IS_TEMP_CONTROL != sip.IsTempControl
        || !f1903.FIRST_IN_DATE.HasValue
        || f1903.ITEM_SIZE != newItemSize)
      {
        F1903Repo.UpdateFields(
          new
          {
            EAN_CODE1 = sip.EanCode1,
            EAN_CODE2 = sip.EanCode2,
            EAN_CODE3 = sip.EanCode3,
            NEED_EXPIRED = sip.NeedExpired,
            SAVE_DAY = sip.SaveDay,
            ALL_DLN = sip.AllowDelvDay,
            ALL_SHP = sip.AllowShipDay,
            BUNDLE_SERIALNO = sip.BundleSerialNo,
						BUNDLE_SERIALLOC = sip.BundleSerialNo =="0" ? "0" : f1903.BUNDLE_SERIALLOC,
						TMPR_TYPE = sip.TmprType,
            ISAPPLE = sip.IsApple,
            IS_PRECIOUS = sip.IsPrecious,
            FRAGILE = sip.IsFragile,
            IS_EASY_LOSE = sip.IsEasyLose,
            IS_MAGNETIC = sip.IsMagentic,
            SPILL = sip.IsSpill,
            IS_PERISHABLE = sip.IsPerishable,
            IS_TEMP_CONTROL = sip.IsTempControl,
            FIRST_IN_DATE = !f1903.FIRST_IN_DATE.HasValue ? DateTime.Now : f1903.FIRST_IN_DATE.Value,
            ITEM_SIZE = newItemSize,
          },
          x => x.GUP_CODE == f1903.GUP_CODE &&
               x.CUST_CODE == f1903.CUST_CODE &&
               x.ITEM_CODE == f1903.ITEM_CODE);
      }
      if (f02020101.RECV_QTY != sip.RecvQty)
      {
				var newCheckQty = SharedService.GetQtyByRatio(sip.RecvQty, f02020101.ITEM_CODE, f02020101.GUP_CODE, f02020101.CUST_CODE, f02020101.PURCHASE_NO);
				var checkSerial = f02020101.CHECK_SERIAL;
				var defectQty = f02020101.DEFECT_QTY;
        if (f02020101.CHECK_ITEM == "1" && f02020101.RECV_QTY > sip.RecvQty)
        {
          ////先刪除F020302 再刪F020301 最後才刪F02020104 有順序性 
          F020302Repo.DeleteWithCancelAcceptance(sip.DcCode, sip.GupCode, sip.CustCode, sip.PurchaseNo, sip.PurchaseSeq, sip.RtNo);
          F020301Repo.Delete(sip.DcCode, sip.GupCode, sip.CustCode, sip.PurchaseNo);
          F02020104Repo.DeleteF02020104(sip.DcCode, sip.GupCode, sip.CustCode, sip.PurchaseNo, sip.PurchaseSeq, sip.RtNo);

          // 清除不良品設定
          F02020109Repo.Delete(sip.DcCode, sip.GupCode, sip.CustCode, sip.PurchaseNo, sip.PurchaseSeq);
					checkSerial = "0";
					defectQty = 0;
				}
				// 因為不良品會先設定和刷讀序號才改驗收數量，所以要再檢查一次序號數是否滿足，若滿足更新序號狀態
				if(f02020101.CHECK_ITEM =="0" && sip.BundleSerialNo =="1" && f02020101.RECV_QTY > sip.RecvQty && f02020101.CHECK_SERIAL == "0")
				{
					// 3. 取得已收集未驗收序號清單
					var collectSerialList =GetCollectSerialList(sip.DcCode, sip.GupCode, sip.CustCode,sip.PurchaseNo,sip.PoNo,sip.ItemNo);
					var collectSerialCnt = collectSerialList.Count;
					// 4. 取得已刷讀並通過序號清單
					var scanSerialCnt = GetIsPassSerialCnt(sip.DcCode, sip.GupCode, sip.CustCode, sip.PurchaseNo, sip.PurchaseSeq,sip.RtNo);
					// 5. 序號處理模式 (0: 序號抽驗模式、1: 序號收集模式) 
					//    如果已收集序號數 > 已刷讀並通過數量 且已收集序號數>=驗收數 則為序號抽驗模式 else 序號收集模式
					var checkMode = collectSerialCnt > scanSerialCnt && collectSerialCnt >= sip.RecvQty ? "0" : "1";
					// 6. 應收數 = 如果已收集序號數> 已刷讀並通過數量 則應刷數=抽驗數 else 應刷數=驗收數 
					var needQty = checkMode == "0" ? newCheckQty  : sip.RecvQty;
					if (scanSerialCnt >= needQty)
						checkSerial = "1";
				}
        //11.	更新驗收暫存檔F02020101
        F02020101Repo.UpdateFields(
            new
            {
              TARWAREHOUSE_ID = string.IsNullOrWhiteSpace(sip.TarWarehouseId) ? f02020101.TARWAREHOUSE_ID : sip.TarWarehouseId,
              IS_PRINT_ITEM_ID = sip.IsPrintItemId,
              RECV_QTY = sip.RecvQty,
              CHECK_QTY = newCheckQty,
              CHECK_ITEM = "1",
              RECE_DATE = DateTime.Today,
              CHECK_SERIAL = checkSerial,
              VALI_DATE = sip.ValidDate,
			  DEFECT_QTY = defectQty,
			},
            x => x.DC_CODE == f02020101.DC_CODE &&
                 x.GUP_CODE == f02020101.GUP_CODE &&
                 x.CUST_CODE == f02020101.CUST_CODE &&
                 x.PURCHASE_NO == f02020101.PURCHASE_NO &&
                 x.PURCHASE_SEQ == f02020101.PURCHASE_SEQ);
      }
      else
      {
        //11.	更新驗收暫存檔F02020101
        F02020101Repo.UpdateFields(
            new
            {
              TARWAREHOUSE_ID = string.IsNullOrWhiteSpace(sip.TarWarehouseId) ? f02020101.TARWAREHOUSE_ID : sip.TarWarehouseId,
              IS_PRINT_ITEM_ID = sip.IsPrintItemId,
              CHECK_ITEM = "1",
              RECE_DATE = DateTime.Today,
              VALI_DATE = sip.ValidDate,
            },
            x => x.DC_CODE == f02020101.DC_CODE &&
                 x.GUP_CODE == f02020101.GUP_CODE &&
                 x.CUST_CODE == f02020101.CUST_CODE &&
                 x.PURCHASE_NO == f02020101.PURCHASE_NO &&
                 x.PURCHASE_SEQ == f02020101.PURCHASE_SEQ);
      }

      //12.	新增該筆驗收單檢驗項目(不存在才新增)
      var f02020102s = F02020102Repo.GetDatas(sip.DcCode, sip.GupCode, sip.CustCode, sip.PurchaseNo, sip.PurchaseSeq, sip.RtNo).ToList();

      var AddCheckNoList = sip.CheckItemList.Select(s => s.CheckNo).Except(f02020102s.Select(x => x.CHECK_NO)).ToList();
      foreach (var checkItem in AddCheckNoList)
      {
        var f02020102 = new F02020102
        {
          PURCHASE_NO = sip.PurchaseNo,
          PURCHASE_SEQ = sip.PurchaseSeq,
          ITEM_CODE = sip.ItemNo,
          CHECK_NO = checkItem,
          DC_CODE = sip.DcCode,
          GUP_CODE = sip.GupCode,
          CUST_CODE = sip.CustCode,
          RT_NO = sip.RtNo,
        };
        F02020102Repo.Add(f02020102);
      }
      return new ApiResult { IsSuccessed = true, MsgCode = "10005" };
    }
    #endregion
  }
}
