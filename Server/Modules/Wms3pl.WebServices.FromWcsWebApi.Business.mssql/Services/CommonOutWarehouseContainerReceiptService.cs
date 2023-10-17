using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Services
{
	/// <summary>
	/// 出庫結果回報(按箱)
	/// </summary>
	public class CommonOutWarehouseContainerReceiptService
	{
    #region Service
    private TransApiBaseService _tacService;
    public TransApiBaseService TacService
    {
      get { return _tacService == null ? _tacService = new TransApiBaseService() : _tacService; }
      set { _tacService = value; }
    }

    #endregion

    #region 定義需檢核欄位、必填、型態、長度
    // 出庫結果回報(按箱)資訊檢核設定
    private List<ApiCkeckColumnModel> receiptCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "OwnerCode",       Type = typeof(string),		MaxLength = 12, Nullable = false },
			new ApiCkeckColumnModel{  Name = "DcCode",			Type = typeof(string),		MaxLength = 10,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "ZoneCode",		Type = typeof(string),		MaxLength = 5,  Nullable = false },
		};

		// 出庫結果回報(按箱)(裝箱資訊)資訊細檢核設定
		private List<ApiCkeckColumnModel> containerCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "ContainerCode",		Type = typeof(string),			MaxLength = 32,		Nullable = false },
			new ApiCkeckColumnModel{  Name = "Operator",			Type = typeof(string),			MaxLength = 20,		Nullable = false },
			new ApiCkeckColumnModel{  Name = "WorkStationNo",		Type = typeof(string),			MaxLength = 32,		Nullable = true },
			new ApiCkeckColumnModel{  Name = "SeedBinCode",			Type = typeof(string),			MaxLength = 32,		Nullable = true },
			new ApiCkeckColumnModel{  Name = "SkuTotal",			Type = typeof(int),				MaxLength = 0,		Nullable = false }
		};

		private List<ApiCkeckColumnModel> skuCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "OrderCode",			Type = typeof(string),			MaxLength = 32,		Nullable = false },
			new ApiCkeckColumnModel{  Name = "RowNum",				Type = typeof(int),				MaxLength = 0,		Nullable = false },
			new ApiCkeckColumnModel{  Name = "SkuCode",				Type = typeof(string),			MaxLength = 20,		Nullable = false },
			new ApiCkeckColumnModel{  Name = "SkuQty",				Type = typeof(int),				MaxLength = 0,		Nullable = false },
			new ApiCkeckColumnModel{  Name = "SkuLevel",			Type = typeof(int),				MaxLength = 0,		Nullable = false },
			new ApiCkeckColumnModel{  Name = "ExpiryDate",			Type = typeof(string),			MaxLength = 10,		Nullable = true,	IsDate = true },
			new ApiCkeckColumnModel{  Name = "OutBatchCode",		Type = typeof(string),			MaxLength = 20,		Nullable = true },
			new ApiCkeckColumnModel{  Name = "BinCode",				Type = typeof(string),			MaxLength = 20,		Nullable = true },
			new ApiCkeckColumnModel{  Name = "CompleteTime",        Type = typeof(string),			MaxLength = 19,		Nullable = false,	IsDateTime = true},
			new ApiCkeckColumnModel{  Name = "IsLastContainer",     Type = typeof(int),				MaxLength = 0,		Nullable = false },
			new ApiCkeckColumnModel{  Name = "ContainerTotal",      Type = typeof(int),				MaxLength = 0,		Nullable = true },
		};

		private List<ApiCkeckColumnModel> shelfBinCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "ShelfCode",           Type = typeof(string),          MaxLength = 20,     Nullable = true },
			new ApiCkeckColumnModel{  Name = "BinCode",             Type = typeof(string),          MaxLength = 20,     Nullable = true },
			new ApiCkeckColumnModel{  Name = "SkuQty",				Type = typeof(int),				MaxLength = 0,		Nullable = false },
			
		};
		#endregion

		#region Private Property
		/// <summary>
		/// 出庫任務派發作業清單
		/// </summary>
		private List<F060201> _f060201List;

		/// <summary>
		/// 調撥單明細清單
		/// </summary>
		private List<F151002> _f151002List;


		/// <summary>
		/// 揀貨單明細By出貨單號
		/// </summary>
		private List<F051202> _f051202ListByO;

		/// <summary>
		/// 揀貨單明細By揀貨單號
		/// </summary>
		private List<F051203> _f051203ListByP;

		#endregion


		#region Main Method
		/// <summary>
		/// Func1
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult RecevieApiDatas(OutWarehouseContainerReceiptReq req)
		{
			CheckTransWcsApiService ctaService = new CheckTransWcsApiService();
			CommonService commonService = new CommonService();
			ApiResult res = new ApiResult { IsSuccessed = true };

			#region 資料檢核
			// 檢核參數
			if (req == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = TacService.GetMsg("20056") };

			// 檢核物流中心 必填、是否存在
			ctaService.CheckDcCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核貨主編號 必填、是否存在
			ctaService.CheckOwnerCode(ref res, req);
			if (!res.IsSuccessed)
				return res;
			#endregion

			// 取得業主編號
			string gupCode = commonService.GetGupCode(req.OwnerCode);

			// 資料處理1
			return ProcessApiDatas(req.DcCode, gupCode, req.OwnerCode, req.ZoneCode, req.ContainerList);
		}

		/// <summary>
		/// 資料處理
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas(string dcCode, string gupCode, string custCode, string warehouseId, List<OutWarehouseContainerReceiptContainerModel> containerList)
		{
			#region 變數
			var f060201Repo = new F060201Repository(Schemas.CoreSchema);
			var f151001Repo = new F151001Repository(Schemas.CoreSchema);
			var f151002Repo = new F151002Repository(Schemas.CoreSchema);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			var f051203Repo = new F051203Repository(Schemas.CoreSchema);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema);

			var orderCodes = containerList.SelectMany(x => x.SkuList).Select(x => x.OrderCode).ToList();
			
			// 取得出庫任務派發作業清單
			_f060201List = f060201Repo.GetDatas(dcCode, gupCode, custCode, "1", orderCodes).ToList();

			// 取得出庫單所有單號
			var wmsNos = _f060201List.Select(x => x.WMS_NO).ToList();

			// 單號清單(調撥單號、訂單單號、揀貨單號)
			var allNos = wmsNos.Where(wmsNo => wmsNo.StartsWith("T")).ToList();
			var ordNos = wmsNos.Where(wmsNo => wmsNo.StartsWith("O")).ToList();
			var pickNos = wmsNos.Where(wmsNo => wmsNo.StartsWith("P")).ToList();

			_f151002List = new List<F151002>();
			_f051202ListByO = new List<F051202>();
			_f051203ListByP = new List<F051203>();
			if (allNos.Any())
				// 取得調撥單明細資料
				_f151002List = f151002Repo.GetDatasNoTracking(dcCode, gupCode, custCode, allNos).ToList();

			if(ordNos.Any())
			{
				var pickNoList = _f060201List.Select(x => x.PICK_NO).ToList();
				// 取得揀貨明細
				_f051202ListByO = f051202Repo.GetDataByPickNos(dcCode, gupCode, custCode, pickNoList).ToList();
			}
			if(pickNos.Any())
				// 取得揀貨彙總明細
			  _f051203ListByP = f051203Repo.GetDatasByPickNos(dcCode, gupCode, custCode, pickNos).ToList();

			// 取得參數序號資料
			var itemSerialNos = containerList.SelectMany(x => x.SkuList).Where(x => x.SerialNumList != null).Select(x => new WcsApiOutWarehouseReceiptItemSerialModel
			{
				ItemCode = x.SkuCode,
				SerialNos = x.SerialNumList
			}).ToList();

      var commonService = new CommonService();
			var res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();
			List<string> successedReceiptCodes = new List<string>();
			#endregion

			#region 新增資料
			containerList.ForEach(container =>
			{
				var res1 = CheckOutWarehouseContainerReceipt(dcCode, gupCode, custCode, container);
				if (!res1.IsSuccessed)
				{
					var currDatas = (List<ApiResponse>)res1.Data;
					data.AddRange(currDatas);
					//return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056"), Data = currDatas };
				}
				else
				{
					var f060207RepoLock = new F060207Repository(Schemas.CoreSchema);
					var f060207 = f060207RepoLock.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
						new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
						() =>
						{
							long id = -1; // -1代表有重複資料

							var lockF060207 = f060207RepoLock.LockF060207();
              var hasData = f060207RepoLock.IsUnProcessData(dcCode, gupCode, custCode, container.ContainerCode);

              if (!hasData)
              {
                id = f060207RepoLock.GetF060207NextId();
							}
							else
							{
								data.Add(new ApiResponse { No = container.ContainerCode, ErrorColumn = "ContainerCode", MsgCode = "20093", MsgContent = TacService.GetMsg("20093") });
							}

							return new F060207 { ID = id };
						});
					if(f060207.ID!=-1)
					{
						var wmsTransation = new WmsTransaction();
						var f060207Repo = new F060207Repository(Schemas.CoreSchema, wmsTransation);
						var f06020701Repo = new F06020701Repository(Schemas.CoreSchema, wmsTransation);
						var f06020702Repo = new F06020702Repository(Schemas.CoreSchema, wmsTransation);

						var currF060207 = new F060207
						{
							ID = f060207.ID,
							DC_CODE = dcCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							WAREHOUSE_ID = warehouseId,
							CONTAINERCODE = container.ContainerCode,
							OPERATOR = container.Operator,
							WORKSTATION_NO = container.WorkStationNo,
							SEED_BINCODE = container.SeedBinCode,
							SKUTOTAL = Convert.ToInt16(container.SkuTotal),
							STATUS = "0"
						};
						f060207Repo.Add(currF060207);

						foreach (var sku in container.SkuList)
						{
							f06020701Repo.Add(new F06020701
							{
								F060207_ID = currF060207.ID,
								ORDERCODE = sku.OrderCode,
								ROWNUM = sku.RowNum,
								SKUCODE = sku.SkuCode,
								SKUQTY = sku.SkuQty,
								SKULEVEL = sku.SkuLevel,
								EXPIRYDATE = DateTime.TryParse(sku.ExpiryDate, out var date) ? date : (DateTime?)null,
								OUTBATCHCODE = sku.OutBatchCode,
                SERIALNUMLIST = (sku.SerialNumList == null || !sku.SerialNumList.Any()) ? null : string.Join(",", sku.SerialNumList),
                BINCODE = sku.BinCode,
								COMPLETE_TIME = Convert.ToDateTime(sku.CompleteTime),
								ISLASTCONTAINER = sku.IsLastContainer.Value,
								CONTAINER_TOTAL = sku.ContainerTotal
							});
						}

						foreach (var shelfBin in container.ShelfBinList)
						{

							f06020702Repo.Add(new F06020702
							{
								F060207_ID = currF060207.ID,
								SHELF_CODE = shelfBin.ShelfCode,
								BIN_CODE = shelfBin.BinCode,
								SKUQTY = shelfBin.SkuQty
							});
						}

						successedReceiptCodes.Add(container.ContainerCode);
						wmsTransation.Complete(true);
					}
				}
			});
			#endregion

			#region 組回傳資料
			res.SuccessCnt = successedReceiptCodes.Count;
			res.FailureCnt = containerList.Count() - successedReceiptCodes.Count;
			res.TotalCnt = containerList.Count;

			res.IsSuccessed = !data.Any();
			res.MsgCode = "10005";
			res.MsgContent = string.Format(TacService.GetMsg("10005"),
					"出貨結果回報(按箱)",
					res.SuccessCnt,
					res.FailureCnt,
					res.TotalCnt);

			res.Data = data.Any() ? data : null;
			#endregion

			return res;
		}

		/// <summary>
		/// 資料處理2
		/// </summary>
		/// <param name="currentPage"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		private ApiResult CheckOutWarehouseContainerReceipt(string dcCode, string gupCode, string custCode, OutWarehouseContainerReceiptContainerModel receipt)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(dcCode, gupCode, custCode, receipt).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(dcCode, gupCode, custCode, receipt).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(dcCode, gupCode, custCode, receipt).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(dcCode, gupCode, custCode, receipt, _f060201List).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(dcCode, gupCode, custCode, receipt).Data);
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
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, OutWarehouseContainerReceiptContainerModel receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位格式檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, OutWarehouseContainerReceiptContainerModel receipt)
		{
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();
			
			string nullErrorMsg = TacService.GetMsg("20058");
			string formatErrorMsg = TacService.GetMsg("20059");
			string dateErrorMsg = TacService.GetMsg("20075");
			string datetimeErrorMsg = TacService.GetMsg("20050");



			#region 檢查盤點單欄位必填、最大長度
			List<string> notDateColumn = new List<string>();

			// 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
			containerCheckColumnList.ForEach(column =>
			{
				var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(receipt, column.Name));
				
				// 必填
				if (!column.Nullable)
					if (!DataCheckHelper.CheckRequireColumn(receipt, column.Name))
						data.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = column.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, receipt.ContainerCode, column.Name) });

				// 最大長度
				if (column.MaxLength > 0)
					if (!DataCheckHelper.CheckDataMaxLength(receipt, column.Name, column.MaxLength))
						data.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = column.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, receipt.ContainerCode, column.Name) });

				// 判斷是否為日期格式(yyyy/MM/dd HH:mm:ss)字串
				if (column.IsDateTime && !string.IsNullOrWhiteSpace(value))
					if (!DataCheckHelper.CheckDataIsDateTime(receipt, column.Name))
						data.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = column.Name, MsgCode = "20050", MsgContent = string.Format(datetimeErrorMsg, receipt.ContainerCode, column.Name) });


			});
			#endregion

			#region 檢查明細欄位必填、最大長度
			if (receipt.SkuList != null && receipt.SkuList.Any())
			{
				for (int i = 0; i < receipt.SkuList.Count; i++)
				{
					var index = Convert.ToString(i + 1);

					var currSku = receipt.SkuList[i];

					skuCheckColumnList.ForEach(sku =>
					{
						var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(currSku, sku.Name));

						// 必填
						if (!sku.Nullable)
							if (!DataCheckHelper.CheckRequireColumn(currSku, sku.Name))
								data.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = sku.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, $"{receipt.ContainerCode}第{index}筆明細", sku.Name) });

						// 最大長度
						if (sku.MaxLength > 0)
							if (!DataCheckHelper.CheckDataMaxLength(currSku, sku.Name, sku.MaxLength))
								data.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = sku.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, $"{receipt.ContainerCode}第{index}筆明細", sku.Name) });

						// 判斷是否為日期格式(yyyy/MM/dd HH:mm:ss)字串
						if (sku.IsDateTime && !string.IsNullOrWhiteSpace(value))
							if (!DataCheckHelper.CheckDataIsDateTime(currSku, sku.Name))
								data.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = sku.Name, MsgCode = "20050", MsgContent = string.Format(datetimeErrorMsg, $"{receipt.ContainerCode}第{index}筆明細", sku.Name) });

						// 判斷是否為日期格式(yyyy/MM/dd)字串
						if (sku.IsDate && !string.IsNullOrWhiteSpace(value))
							if (!DataCheckHelper.CheckDataIsDate(currSku, sku.Name))
								data.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = sku.Name, MsgCode = "20075", MsgContent = string.Format(dateErrorMsg, $"{receipt.ContainerCode}第{index}筆明細", sku.Name) });
					});
				}
			}

			if (receipt.ShelfBinList != null && receipt.ShelfBinList.Any())
			{
				for (int i = 0; i < receipt.ShelfBinList.Count; i++)
				{
					var index = Convert.ToString(i + 1);

					var currShelfBin = receipt.ShelfBinList[i];

					shelfBinCheckColumnList.ForEach(shelfBin =>
					{
						var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(currShelfBin, shelfBin.Name));
						
						// 必填
						if (!shelfBin.Nullable)
							if (!DataCheckHelper.CheckRequireColumn(currShelfBin, shelfBin.Name))
								data.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = shelfBin.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, $"{receipt.ContainerCode}第{index}筆明細", shelfBin.Name) });

						// 最大長度
						if (shelfBin.MaxLength > 0)
							if (!DataCheckHelper.CheckDataMaxLength(currShelfBin, shelfBin.Name, shelfBin.MaxLength))
								data.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = shelfBin.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, $"{receipt.ContainerCode}第{index}筆明細", shelfBin.Name) });

					});
				}
			}
			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位格式檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, OutWarehouseContainerReceiptContainerModel receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位資料檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, OutWarehouseContainerReceiptContainerModel receipt,List<F060201> f060201s)
		{
			CheckOutWarehouseContainerReceiptService ciwService = new CheckOutWarehouseContainerReceiptService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			
			#region 明細欄位資料檢核

			// 明細數與裝箱明細數是否相等
			ciwService.CheckSkuTotalEqualSkuListCount(data, receipt);


			// 裝箱明細檢核
			for (int index = 0; index < receipt.SkuList.Count; index++)
			{
				var sku = receipt.SkuList[index];
				// 檢查[裝箱數量]是否有大於0
				ciwService.CheckSkuQty(data, sku, receipt.ContainerCode, index);

				// 檢查出貨單號(OrderCode)是否存在F060201
				ciwService.CheckOrderCodeExist(data, sku, dcCode, gupCode, custCode, receipt.ContainerCode,index, f060201s,_f151002List,_f051202ListByO,_f051203ListByP);

				// 檢查商品序號清單(SerialNumList)內所有資料以逗號連接總長度是否超過8000
				ciwService.CheckSkuSerialNum(data, sku, receipt.ContainerCode, index);

				// 檢查該明細序號長度是否與裝箱數量相同
				ciwService.CheckSkuSerialNumEquelSkuQty(data, sku, receipt.ContainerCode, index);
			}
			
			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, OutWarehouseContainerReceiptContainerModel receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion

		
	}
}