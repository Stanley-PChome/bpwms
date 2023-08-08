using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.ToWmsWebApi.Business.mssql.Services
{
	/// <summary>
	/// 5.11	外部包裝資料處理排程(12)
	/// </summary>
	public class OutSidePackageService : BaseService
	{
		F060209Repository _f060209Repo;
		F06020901Repository _f06020901Repo;
		F06020902Repository _f06020902Repo;
		WmsTransaction _wmsTransaction;
		F050801 _curF050801;
    F060209 _curF060209;
		List<F06020901> _curF06020901List;
		List<F06020902> _curF06020902List;
		Dictionary<string, OutSidePackageBoxDetailResult> _shipBoxDetails = null;
		Dictionary<string, CheckOutSidePackageDataResult> _checkShipResults = null;
		string _empName;
		public OutSidePackageService()
		{
		}

		#region 入口點與主流程
		/// <summary>
		/// 外部包裝資料處理後轉入WMS包裝紀錄
		/// </summary>
		/// <param name="req"></param>
		public ApiResult OutSidePackageProcess(WcsImportReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			var data = new List<ApiResponse>();
			// 取得物流中心服務貨主檔
			var commonService = new CommonService();
			var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
			dcCustList.ForEach(item =>
			{
				var result = OutSidePackageExecute(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, req.ProcFlag);
				data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE} PROC_FLAG={req.ProcFlag}" });
				if (result.Data!=null && result.Data is List<ApiResponse>)
					data.AddRange((List<ApiResponse>)result.Data);
			});
			res.Data = JsonConvert.SerializeObject(data);
			return res;
		}

		/// <summary>
		/// 執行外部包裝資料處理後轉入WMS包裝紀錄
		/// 每一筆處理完成後Commit，請包try…catch
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="procFlag"></param>
		/// <returns></returns>
		private ApiResult OutSidePackageExecute(string dcCode, string gupCode, string custCode, string procFlag)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			var commonService = new CommonService();
			// 取得排程處理最大筆數設定值
			var maxRecordStr = commonService.GetSysGlobalValue("OutSidePackageMaxRecord");
			// 若沒有設定預設20筆
			var maxRecord = !string.IsNullOrWhiteSpace(maxRecordStr) ? int.Parse(maxRecordStr) : 20;

			var f060209Repo = new F060209Repository(Schemas.CoreSchema);
			List<F060209> f060209List = null;
			if (!string.IsNullOrEmpty(procFlag))
				f060209List = f060209Repo.GetDatas(dcCode, gupCode, custCode, procFlag, maxRecord).ToList();
			else
			{
				res.IsSuccessed = false;
				res.MsgCode = "99999";
				res.MsgContent = "未傳入PROC_FLAG";
				return res;
			}
			if (f060209List != null && !f060209List.Any())
			{
				res.MsgCode = "10005";
				res.MsgContent = string.Format(_tacService.GetMsg("10005"), "外部包裝資料處理排程(12)", "0", "0", "0");
				return res;
			}
			var docIds = f060209List.Select(x => x.DOC_ID).ToList();
			res = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSSH_F009008, dcCode, gupCode, custCode, $"OutSidePackage_ProcFlag_{procFlag}", new { DC_CODE = dcCode, GUP_CODE = gupCode, CUST_CODE = custCode, PROC_FLAG = procFlag }, () =>
							{
								var result = new ApiResult { IsSuccessed = true };
								var datas = new List<ApiResponse>();
								var f06020901Repo = new F06020901Repository(Schemas.CoreSchema);
								var f06020902Repo = new F06020902Repository(Schemas.CoreSchema);
								var f050801Repo = new F050801Repository(Schemas.CoreSchema);
								var f06020901List = f06020901Repo.GetDatas(docIds).ToList();
								var f06020902List = f06020902Repo.GetDatas(docIds).ToList();
								var successCnt = 0;
								var failureCnt = 0;
								var totalCnt = f060209List.Count();
								foreach (var curF060209 in f060209List)
								{
									try
									{
										var res2 = new ApiResult { IsSuccessed = true };
										_wmsTransaction = new WmsTransaction();
										_f060209Repo = new F060209Repository(Schemas.CoreSchema, _wmsTransaction);
										_f06020901Repo = new F06020901Repository(Schemas.CoreSchema, _wmsTransaction);
										_f06020902Repo = new F06020902Repository(Schemas.CoreSchema, _wmsTransaction);
										_curF060209 = curF060209;
										_curF06020901List = f06020901List.Where(x => x.DOC_ID == curF060209.DOC_ID).ToList();
										_curF06020902List = f06020902List.Where(x => x.DOC_ID == curF060209.DOC_ID).ToList();
										_curF050801 = f050801Repo.GetF050801ByWmsOrdNo(curF060209.DC_CODE, curF060209.GUP_CODE, curF060209.CUST_CODE, curF060209.WMS_NO);
										if (_curF050801 == null)
										{
											UnLock("出貨單號不存在");
											res2.IsSuccessed = false;
										}
                    if (!_curF06020901List.Any())
                    {
                      UnLock("無外部出貨包裝紀錄頭檔資料");
											res2.IsSuccessed = false;
										}
										if (!_curF06020902List.Any())
										{
											UnLock("無外部出貨包裝紀錄身檔資料");
											res2.IsSuccessed = false;
										}
										_empName = commonService.GetEmpName(_curF060209.OPERATOR);
                    if (res2.IsSuccessed)
										{
											switch (procFlag)
											{
												case "3": // 取消宅配單失敗-重新執行
																	// 執行資料處理二
													res2 = OutSidePackageDataProcess2();
													break;
												case "8": // 異常-已完成宅單申請但寫入出貨失敗而需重新執行
																	// 執行資料處理三
													res2 = OutSidePackageDataProcess3();
													break;
												case "0": // 待處理-首次執行外部包裝資料處理轉入WMS包裝紀錄，其中需呼叫LMS進行宅配單申請
																	// 執行資料處理四
													res2 = OutSidePackageDataProcess4();
													break;
												default:
													break;
											}
										}

										if (res2.IsSuccessed)
											successCnt++;
										else
											failureCnt++;

										datas.Add(new ApiResponse { MsgCode = res2.MsgCode, MsgContent = res2.MsgContent,No = _curF060209.WMS_NO });
									}
									catch (Exception ex)
									{
										failureCnt++;
										datas.Add(new ApiResponse { MsgCode = "99999", MsgContent = ex.Message });

										// 已取得宅配單
										if (_curF060209.PROC_FLAG == 1)
											// 8 = 異常
											UpdateF060209ForException(8, "已取得宅配單，但系統例外錯誤，由下一次排程啟動時重新執行");

										if (_curF060209.PROC_FLAG == 0 && _curF06020901List.Any(x => x.PROC_FLAG == 1))
											// 3 = 取消宅配單
											UpdateF060209ForException(3, "部分宅配單取得失敗，因系統例外錯誤需進行取消宅配單");
									}
								}
								result.MsgCode = "10005";
								result.MsgContent = string.Format(_tacService.GetMsg("10005"), "外部包裝資料處理排程(12)", successCnt, failureCnt, totalCnt);
								result.Data = datas;
								return result;
							}, true);
			return res;
		}

		#endregion

		#region Common 
		/// <summary>
		/// 作業取消，並解鎖UI包裝限制
		/// 限定未取得宅配單，或已經取消宅配單申請才可使用
		/// </summary>
		private void UnLock(string MsgContent)
		{
			// 更新處理結果
			_curF060209.PROC_FLAG = 9; // 9=作業取消
			_curF060209.MSG_CONTENT = MsgContent;
			_f060209Repo.Update(_curF060209);
			_wmsTransaction.Complete();

			// 如果出貨單出貨模式= 外部出貨包裝紀錄(3) ，解鎖UI包裝限制
			if (_curF050801 != null && _curF050801.SHIP_MODE == "3")
			{
				var shipPackageService = new ShipPackageService();
				shipPackageService.CheckPackageMode(_curF050801, "0");
			}
		}

		/// <summary>
		/// 更新外部出貨包裝單據主檔[F060209]處理結果 
		/// </summary>
		/// <param name="procFlag"></param>
		/// <param name="msgContent"></param>
		private void UpdateF060209(int procFlag, string msgContent)
		{
			_curF060209.PROC_FLAG = procFlag;

			if (!string.IsNullOrWhiteSpace(msgContent))
				_curF060209.MSG_CONTENT = msgContent;

			_f060209Repo.Update(_curF060209);
			_wmsTransaction.Complete();
		}

		/// <summary>
		/// 發生系統錯誤，更新外部出貨包裝單據主檔[F060209]處理結果 
		/// </summary>
		/// <param name="procFlag"></param>
		/// <param name="msgContent"></param>
		private void UpdateF060209ForException(int procFlag, string msgContent)
		{
			var f060209Repo = new F060209Repository(Schemas.CoreSchema);
			_curF060209.PROC_FLAG = procFlag;

			if (!string.IsNullOrWhiteSpace(msgContent))
				_curF060209.MSG_CONTENT = msgContent;

			f060209Repo.Update(_curF060209);
		}

    /// <summary>
    /// 資料檢核
    /// </summary>
    private CheckOutSidePackageDataResult CheckOutSidePackageData()
    {
      if (_checkShipResults == null)
        _checkShipResults = new Dictionary<string, CheckOutSidePackageDataResult>();

      // 檢查出貨單是否已經有檢核，如果有就抓Cache就好 節省重複跑的時間
      var findCheckShipResult = _checkShipResults.FirstOrDefault(x => x.Key == _curF060209.WMS_NO);
      if (!findCheckShipResult.Equals(default(KeyValuePair<string, CheckOutSidePackageDataResult>)))
        return findCheckShipResult.Value;

      var result = new CheckOutSidePackageDataResult { IsSuccessed = true };
      var f050301Repo = new F050301Repository(Schemas.CoreSchema);
      var f050301 = f050301Repo.GetDataByWmsOrdNo(_curF060209.DC_CODE, _curF060209.GUP_CODE, _curF060209.CUST_CODE, _curF060209.WMS_NO);
      if (f050301 == null)
      {
        if (_curF060209.PROC_FLAG == 1 || _curF060209.PROC_FLAG == 8)
          OutSidePackageDataProcess2("訂單不存在");
        else
          UnLock("訂單不存在");

        result.IsSuccessed = false;
        return result;
      }
      if (f050301 != null && f050301.PROC_FLAG == "9") // 訂單取消
      {
        if (_curF060209.PROC_FLAG == 1 || _curF060209.PROC_FLAG == 8)
          OutSidePackageDataProcess2("訂單已取消");
        else
          UnLock("訂單已取消");

        result.IsSuccessed = false;
        return result;
      }
      if (_curF050801 != null && _curF050801.STATUS == 9) // 出貨單已取消
      {
        if (_curF060209.PROC_FLAG == 1 || _curF060209.PROC_FLAG == 8)
          OutSidePackageDataProcess2("出貨單已取消");
        else
          UnLock("出貨單已取消");

        result.IsSuccessed = false;
        return result;
      }
      if (_curF050801 != null && _curF050801.STATUS == 2) // 出貨單已包裝完成
      {
        UnLock("出貨單已包裝完成");
        result.IsSuccessed = false;
        return result;
      }
      if (_curF050801 != null && _curF050801.STATUS == 5) // 出貨單已出貨扣帳
      {
        UnLock("出貨單已出貨扣帳");
        result.IsSuccessed = false;
        return result;
      }
      var f051201Repo = new F051201Repository(Schemas.CoreSchema);
      var f051201 = f051201Repo.GetF051201(_curF060209.DC_CODE, _curF060209.GUP_CODE, _curF060209.CUST_CODE, _curF060209.PICK_NO);
      if (f051201 == null) // 揀貨單不存在
      {
        if (_curF060209.PROC_FLAG == 1 || _curF060209.PROC_FLAG == 8)
          OutSidePackageDataProcess2("揀貨單不存在");
        else
          UnLock("揀貨單不存在");

        result.IsSuccessed = false;
        return result;
      }
      if (f051201 != null && f051201.PICK_STATUS == 9) // 揀貨單已取消
      {
        if (_curF060209.PROC_FLAG == 1 || _curF060209.PROC_FLAG == 8)
          OutSidePackageDataProcess2("揀貨單已取消");
        else
          UnLock("揀貨單已取消");

        result.IsSuccessed = false;
        return result;
      }
      if (f051201 != null && (f051201.PICK_STATUS == 0 || f051201.PICK_STATUS == 1)) // 揀貨單待揀貨/揀貨單揀貨中
      {
        UpdateF060209(0, "揀貨單尚未揀貨完成");
        result.IsSuccessed = false;
        return result;
      }
			var f05120601Repo = new F05120601Repository(Schemas.CoreSchema);
			if (f05120601Repo.GetDatasByWmsOrdNo(_curF050801.DC_CODE, _curF050801.GUP_CODE, _curF050801.CUST_CODE, _curF050801.WMS_ORD_NO).Any())
			{
				UnLock( "有缺貨待配庫不可包裝");
				result.IsSuccessed = false;
				return result;
			}

			var f051206Repo = new F051206Repository(Schemas.CoreSchema);
			if(f051206Repo.GetNotDeleteDatasByWmsOrdNo(_curF050801.DC_CODE,_curF050801.GUP_CODE,_curF050801.CUST_CODE,_curF050801.WMS_ORD_NO).Any())
			{
				UnLock("有缺貨不可包裝");
				result.IsSuccessed = false;
				return result;
			}
      var f2501List = new List<F2501>();
      var snList = new List<string>();
      _curF06020902List.ForEach(x =>
      {
        if (!string.IsNullOrWhiteSpace(x.SERIAL_NO_LIST))
          snList.AddRange(x.SERIAL_NO_LIST.Split(','));
      });

      if (snList.Any())
      {
        var f2501Repo = new F2501Repository(Schemas.CoreSchema);
        f2501List = f2501Repo.GetDatas(_curF060209.GUP_CODE, _curF060209.CUST_CODE, snList).ToList();
      }

      if (f2501List.Any())
      {
        // 非在庫序號清單
        var notInWareHouseSnList = f2501List.Where(x => x.STATUS == "C1").Select(x => x.SERIAL_NO).ToList();
        if (notInWareHouseSnList.Any())
        {
          if (_curF060209.PROC_FLAG == 1 || _curF060209.PROC_FLAG == 8)
            OutSidePackageDataProcess2($"出貨包裝記錄中有{notInWareHouseSnList.Count}筆商品序號已出庫");
          else
            UnLock($"出貨包裝記錄中有{notInWareHouseSnList.Count}筆商品序號已出庫");

          result.IsSuccessed = false;
          return result;
        }
      }

      result.F050301 = f050301;
      result.F051201 = f051201;
      result.F2501List = f2501List;

      _checkShipResults.Add(_curF060209.WMS_NO, result);

      return result;

    }

    /// <summary>
    /// 使用外部包裝紀錄產生出貨箱明細資料與刷讀紀錄
    /// <paramref name="f2501List">序號資料</paramref>
    /// </summary>
    /// <returns></returns>
    private OutSidePackageBoxDetailResult CreateBoxDetail(List<F2501> f2501List)
		{
			var commonService = new CommonService();
			if (_shipBoxDetails == null)
				_shipBoxDetails = new Dictionary<string, OutSidePackageBoxDetailResult>();

			// 檢查是否有分配過資料，如果有就抓Cache就好 節省重複跑的時間
			var findShipBoxDetailResult = _shipBoxDetails.FirstOrDefault(x => x.Key == _curF060209.WMS_NO);
			if (!findShipBoxDetailResult.Equals(default(KeyValuePair<string, OutSidePackageBoxDetailResult>)))
				return findShipBoxDetailResult.Value;

			var now = DateTime.Now;

			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema);
			// 取得訂單分配結果
			var allotOrders = f05030202Repo.GetDatasByWmsOrdNo(_curF060209.DC_CODE, _curF060209.GUP_CODE, _curF060209.CUST_CODE, _curF060209.WMS_NO).ToList();

			var boxlist = new List<OutSidePackageBoxDetail>();
			var scanRecordList = new List<F05500101>();
			var isOk = true;
			var recordSeq = 1;
			#region 新增包裝刷讀紀錄-開始包裝 
			scanRecordList.Add(new F05500101
			{
				WMS_ORD_NO = _curF060209.WMS_NO,
				LOG_SEQ = recordSeq,
				DC_CODE = _curF060209.DC_CODE,
				GUP_CODE = _curF060209.GUP_CODE,
				CUST_CODE = _curF060209.CUST_CODE,
				PACKAGE_BOX_NO = 0,
				PACKAGE_STAFF = _curF060209.OPERATOR,
				PACKAGE_NAME = _empName,
				ITEM_CODE = null,
				SERIAL_NO = null,
				STATUS = null,
				ISPASS = "1",
				MESSAGE = "開始包裝",
				SCAN_CODE = null,
				FLAG = "0",
				CRT_DATE = now,
				CRT_STAFF = _curF060209.OPERATOR,
				CRT_NAME = _empName
			});
			recordSeq++;
			#endregion

			foreach (var x in _curF06020901List)
			{
				var boxDetail = new OutSidePackageBoxDetail();
				#region 產生出貨箱頭檔
				boxDetail.F055001 = new F055001
				{
					WMS_ORD_NO = _curF060209.WMS_NO,
					PACKAGE_BOX_NO = (short)x.BOX_SEQ,
					DELV_DATE = _curF050801.DELV_DATE,
					PICK_TIME = _curF050801.PICK_TIME,
					PRINT_FLAG = string.IsNullOrWhiteSpace(x.CONSIGN_NO) ? 0 : 1,
					PRINT_DATE = string.IsNullOrWhiteSpace(x.CONSIGN_NO) ? null : x.UPD_DATE,
					BOX_NUM = x.BOX_NO,
					PAST_NO = x.CONSIGN_NO,
					DC_CODE = _curF060209.DC_CODE,
					GUP_CODE = _curF060209.GUP_CODE,
					CUST_CODE = _curF060209.CUST_CODE,
					PACKAGE_STAFF = _curF060209.OPERATOR,
					PACKAGE_NAME = _empName,
					STATUS = "0",
					IS_CLOSED = string.IsNullOrWhiteSpace(x.CONSIGN_NO) ? "0" : "1",
          IS_ORIBOX = x.BOX_NO == "ORI" ? "1" : "0",
          CRT_DATE = now,
          CRT_STAFF = _curF060209.OPERATOR,
          CRT_NAME = _empName,
          ORG_BOX_NUM = x.BOX_NO,
          ORG_PAST_NO = x.CONSIGN_NO,
          PACK_CLIENT_PC = "Shuttle"
        };
        #endregion

        // 有託運單產生F050901
        if (!string.IsNullOrWhiteSpace(x.CONSIGN_NO))
					boxDetail.F050901 = CreateF050901(x,now);

				boxDetail.F055002List = new List<F055002>();

				var curBoxSeqDetail = _curF06020902List.Where(y => y.BOX_SEQ == x.BOX_SEQ).ToList();
				var boxDetailSeq = 1;

				foreach (var d in curBoxSeqDetail)
				{
					var snList = new List<string> { null };
					if (!string.IsNullOrWhiteSpace(d.SERIAL_NO_LIST))
						snList = d.SERIAL_NO_LIST.Split(',').ToList();

					
					foreach (var sn in snList)
					{
						#region 進行分配訂單項次產生出貨箱明細[F055002]
						var qty = string.IsNullOrWhiteSpace(sn) ? d.SKU_QTY : 1;
						do
						{
							// 取得該品號還有未分配數(預計出貨數>實際出貨數)  資料排序:以訂單項次小的先配
							var allotOrder = allotOrders.Where(item => item.ITEM_CODE.ToUpper() == d.SKU_CODE.ToUpper() && item.B_DELV_QTY > item.PACKAGE_QTY).OrderBy(a => a.ORD_SEQ).FirstOrDefault();
							// 分配不足夠，代表超出訂單出貨數量就取消分配
							if (allotOrder == null)
							{
								isOk = false;
								break;
							}
							var canAllotQty = allotOrder.B_DELV_QTY - allotOrder.PACKAGE_QTY;
							var allotQty = 0;
							if(canAllotQty >= qty)
							{
								allotQty = qty;
								allotOrder.PACKAGE_QTY += qty;
								qty = 0;
							}
							else
							{
								allotQty = canAllotQty;
								allotOrder.PACKAGE_QTY += canAllotQty;
								qty -= canAllotQty;
							}
							boxDetail.F055002List.Add(new F055002
							{
								WMS_ORD_NO = boxDetail.F055001.WMS_ORD_NO,
								PACKAGE_BOX_NO = boxDetail.F055001.PACKAGE_BOX_NO,
								PACKAGE_BOX_SEQ = boxDetailSeq,
								ITEM_CODE = d.SKU_CODE.ToUpper(),
								SERIAL_NO = string.IsNullOrWhiteSpace(sn) ? null : sn.ToUpper(),
								PACKAGE_QTY = allotQty,
								DC_CODE = _curF060209.DC_CODE,
								GUP_CODE = _curF060209.GUP_CODE,
								CUST_CODE = _curF060209.CUST_CODE,
								CLIENT_PC = "Shuttle",
								ORD_NO = allotOrder.ORD_NO,
								ORD_SEQ = allotOrder.ORD_SEQ,
								CRT_DATE = now,
								CRT_STAFF = _curF060209.OPERATOR,
								CRT_NAME = _empName
							});
							boxDetailSeq++;
						}
						while (qty > 0);

						//
						if (!isOk)
							break;
						#endregion

						#region 產生包裝刷讀紀錄[F05500101]
						scanRecordList.Add(new F05500101
						{
							WMS_ORD_NO = boxDetail.F055001.WMS_ORD_NO,
							LOG_SEQ = recordSeq,
							DC_CODE = boxDetail.F055001.DC_CODE,
							GUP_CODE = boxDetail.F055001.GUP_CODE,
							CUST_CODE = boxDetail.F055001.CUST_CODE,
							PACKAGE_BOX_NO = 0,
							PACKAGE_STAFF = boxDetail.F055001.PACKAGE_STAFF,
							PACKAGE_NAME = boxDetail.F055001.PACKAGE_NAME,
							ITEM_CODE = d.SKU_CODE.ToUpper(),
							SERIAL_NO = string.IsNullOrWhiteSpace(sn) ? null : sn.ToUpper(),
							STATUS = string.IsNullOrWhiteSpace(sn) ? null : f2501List.FirstOrDefault(s=> s.SERIAL_NO.ToUpper() == sn.ToUpper())?.STATUS,
							ISPASS = "1",
							MESSAGE = null,
							SCAN_CODE = string.IsNullOrWhiteSpace(sn) ? d.SKU_CODE : sn,
							FLAG = "0",
							CRT_DATE = now,
							CRT_STAFF = _curF060209.OPERATOR,
							CRT_NAME = _empName,
							ORG_SERIAL_WMS_NO = string.IsNullOrWhiteSpace(sn) ? null : f2501List.FirstOrDefault(s => s.SERIAL_NO.ToUpper() == sn.ToUpper())?.WMS_NO,
						});
						recordSeq++;
						#endregion
					}
					if (!isOk)
						break;
					
				}
				if (!isOk)
					break;

				#region 產生包裝刷讀紀錄[F05500101] --找到紙箱紀錄 (如果是ORI 原箱就不產生紀錄)
				if(x.BOX_NO.ToUpper() != "ORI")
				{
					scanRecordList.Add(new F05500101
					{
						WMS_ORD_NO = boxDetail.F055001.WMS_ORD_NO,
						LOG_SEQ = recordSeq,
						DC_CODE = boxDetail.F055001.DC_CODE,
						GUP_CODE = boxDetail.F055001.GUP_CODE,
						CUST_CODE = boxDetail.F055001.CUST_CODE,
						PACKAGE_BOX_NO = 0,
						PACKAGE_STAFF = boxDetail.F055001.PACKAGE_STAFF,
						PACKAGE_NAME = boxDetail.F055001.PACKAGE_NAME,
						ITEM_CODE = x.BOX_NO.ToUpper(),
						SERIAL_NO = null,
						STATUS = null,
						ISPASS = "1",
						MESSAGE = "找到紙箱",
						SCAN_CODE = x.BOX_NO,
						FLAG = "0",
						CRT_DATE = now,
						CRT_STAFF = _curF060209.OPERATOR,
						CRT_NAME = _empName
					});
					recordSeq++;
				}
				
				#endregion

				boxlist.Add(boxDetail);
			}

			if (!isOk)
			{
				// 如果PROC_FLAG=1 OR PROC_FLAG=8 代表有取過宅配單 要執行取消宅配單申請
				if (_curF060209.PROC_FLAG == 1 || _curF060209.PROC_FLAG == 8)
					OutSidePackageDataProcess2("包裝數超過訂單可出貨數");
				else
					UnLock("包裝數超過訂單可出貨數");
			}
			else
			{
				#region 新增包裝刷讀紀錄-完成包裝 
				scanRecordList.Add(new F05500101
				{
					WMS_ORD_NO = _curF060209.WMS_NO,
					LOG_SEQ = recordSeq,
					DC_CODE = _curF060209.DC_CODE,
					GUP_CODE = _curF060209.GUP_CODE,
					CUST_CODE = _curF060209.CUST_CODE,
					PACKAGE_BOX_NO = 0,
					PACKAGE_STAFF = _curF060209.OPERATOR,
					PACKAGE_NAME = _empName,
					ITEM_CODE = null,
					SERIAL_NO = null,
					STATUS = null,
					ISPASS = "1",
					MESSAGE = "完成包裝",
					SCAN_CODE = null,
					FLAG = "0",
					CRT_DATE = now,
					CRT_STAFF = _curF060209.OPERATOR,
					CRT_NAME = _empName
				});
				recordSeq++;
				#endregion
			}
			var result = new OutSidePackageBoxDetailResult { IsSuccessed = isOk, OutSidePackageBoxDetailList = isOk ? boxlist : new List<OutSidePackageBoxDetail>(), F05500101List = isOk ? scanRecordList : new List<F05500101>() };
			_shipBoxDetails.Add(_curF060209.WMS_NO, result);
			return result;
		}

		// 建立F050901
		private F050901 CreateF050901(F06020901 f06020901,DateTime? now = null)
		{
			return new F050901
			{
				DC_CODE = _curF060209.DC_CODE,
				GUP_CODE = _curF060209.GUP_CODE,
				CUST_CODE = _curF060209.CUST_CODE,
				WMS_NO = _curF060209.WMS_NO,
				CONSIGN_NO = f06020901.CONSIGN_NO,
				STATUS = "0",
				CUST_EDI_STATUS = "0",
				DISTR_EDI_STATUS = "0",
				CUST_EDI_QTY = 0,
				DISTR_USE = "01",
				DISTR_SOURCE = "0",
				DELIVID_SEQ_NAME = f06020901.TRANSPORT_PROVIDER,
				BOXQTY = 1,
				CRT_DATE = now.HasValue ? now.Value : DateTime.Now,
				CRT_STAFF = _curF060209.OPERATOR,
				CRT_NAME = _empName
			};
		}

		

		#endregion

		#region 資料處理二 處理外部出貨包裝單據主檔狀態為[3=取消宅配單]處理

		/// <summary>
		/// 資料處理二
		/// 處理外部出貨包裝單據主檔狀態為[3=取消宅配單]處理
		/// </summary>
		/// <param name="msgContent">指定要加入的訊息內容</param>
		/// <returns></returns>
		private ApiResult OutSidePackageDataProcess2(string msgContent = null)
		{
			if (string.IsNullOrWhiteSpace(msgContent))
				msgContent = string.Empty;
			else
				msgContent += "，";

			var res = new ApiResult { IsSuccessed = true };
			var consignService = new Shared.Lms.Services.ConsignService();
			// 呼叫LMS 取消宅配單API
			var result = consignService.CancelConsign(_curF060209.DC_CODE, _curF060209.GUP_CODE, _curF060209.CUST_CODE, _curF060209.WMS_NO);
			// LMS 回傳取消宅配單成功
			if (result.IsSuccessed)
			{
				_curF06020901List.ForEach(x =>
				{
					x.PROC_FLAG = 3; //宅單取消成功
				});
				_f06020901Repo.BulkUpdate(_curF06020901List);
				UnLock(msgContent+"取部分宅單失敗，已通知LMS取消");
				res.MsgCode = _curF060209.PROC_FLAG.ToString();
				res.MsgContent = msgContent+ "取部分宅單失敗，已通知LMS取消";
			}
			// LMS回傳取消宅配單失敗
			else
			{
				if(_curF060209.PROC_FLAG != 3) // 原狀態非取消宅單失敗才更新
					// 3 = 取消宅單失敗
					UpdateF060209(3, msgContent + "取部分宅單失敗，但取消失敗，" + result.Message);
					
				res.IsSuccessed = false;
				res.MsgCode = _curF060209.PROC_FLAG.ToString();
				res.MsgContent = msgContent + "取部分宅單失敗，但取消失敗，" + result.Message;
			}
			return res;
		}

		#endregion

		#region  資料處理三 處理外部出貨包裝單據主檔狀態為[8=異常]、[1=已取宅配單]處理

		/// <summary>
		/// 資料處理三
		/// 處理外部出貨包裝單據主檔狀態為[8=異常]、[1=已取宅配單]處理
		/// </summary>
		/// <returns></returns>
		private ApiResult OutSidePackageDataProcess3()
		{
			var res = new ApiResult { IsSuccessed = true };
			// 資料檢核
			var checkData = CheckOutSidePackageData();
			if(!checkData.IsSuccessed)
			{
				res.IsSuccessed = false;
				res.MsgCode = _curF060209.PROC_FLAG.ToString();
				res.MsgContent = _curF060209.MSG_CONTENT;
				return res;
			}
			// 產生WMS包裝箱資料
			var wmsBoxDetailResult = CreateBoxDetail(checkData.F2501List);
			if (!wmsBoxDetailResult.IsSuccessed)
			{
				res.IsSuccessed = false;
				res.MsgCode = _curF060209.PROC_FLAG.ToString();
				res.MsgContent = _curF060209.MSG_CONTENT;
				return res;
			}

			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f055002Repo = new F055002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05500101Repo = new F05500101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
			var f050305Repo = new F050305Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction);
			var containerService = new ContainerService(_wmsTransaction);
			var shipPackageService = new ShipPackageService(_wmsTransaction);

			// 更新序號狀態為出庫C1、系統單號=出貨單、異動原因=出貨單的異動原因
			foreach (var f2501 in checkData.F2501List)
			{
				f2501.STATUS = "C1";
				f2501.WMS_NO = _curF050801.WMS_ORD_NO;
				f2501.ORD_PROP = _curF050801.ORD_PROP;
				f2501.UPD_DATE = DateTime.Now;
				f2501.UPD_STAFF = _curF060209.OPERATOR;
				f2501.UPD_NAME = _empName;
			}

			var insertF050305s = new List<F050305>();
			//產生訂單回檔紀錄[包裝開始=2]
			var startRecord = f05030101Repo.GetOrderRtnInsertDatas(_curF050801.DC_CODE, _curF050801.GUP_CODE, _curF050801.CUST_CODE, "2", new List<string> { _curF050801.WMS_ORD_NO }).FirstOrDefault();
			if (startRecord != null)
			{
				startRecord.CRT_DATE = DateTime.Parse(_curF060209.START_TIME);
				startRecord.WMS_ORD_NO = _curF050801.WMS_ORD_NO;
				startRecord.CRT_STAFF = Current.Staff;
				startRecord.CRT_NAME = Current.StaffName;
				insertF050305s.Add(startRecord);
			}

			// 新增出貨箱頭檔
			f055001Repo.BulkInsert(wmsBoxDetailResult.OutSidePackageBoxDetailList.Select(x => x.F055001).ToList(),true);
			// 新增出貨箱明細檔
			f055002Repo.BulkInsert(wmsBoxDetailResult.OutSidePackageBoxDetailList.SelectMany(x => x.F055002List).ToList(), true);
			// 新增出貨包裝紀錄檔
			f05500101Repo.BulkInsert(wmsBoxDetailResult.F05500101List, true);
			// 新增F050901
			f050901Repo.BulkInsert(wmsBoxDetailResult.OutSidePackageBoxDetailList.Select(x => x.F050901).ToList(), true, "CONSIGN_ID");

			// 更新序號檔
			if (checkData.F2501List.Any())
				f2501Repo.BulkUpdate(checkData.F2501List,true);

			// 釋放容器
			containerService.DelContainer(_curF050801.DC_CODE, _curF050801.GUP_CODE, _curF050801.CUST_CODE, _curF050801.WMS_ORD_NO);

			// 刪除F051301
			f051301Repo.DeleteWmsNo(_curF050801.DC_CODE, _curF050801.GUP_CODE, _curF050801.CUST_CODE, _curF050801.WMS_ORD_NO);

			// 扣除紙箱庫存
			wmsBoxDetailResult.OutSidePackageBoxDetailList.ForEach(x =>
			{
				shipPackageService.UpdateBoxStock(x.F055001.DC_CODE, x.F055001.GUP_CODE, x.F055001.CUST_CODE, x.F055001.BOX_NUM);
			});

      // 	更新出貨單狀態為已稽核[F050802.STATUS=2],PRINT_FLAG=1，排除出貨單狀態已取消避免被覆蓋
      f050801Repo.UpdateStatusWithNoCancelAndHasPrintFlag(_curF050801.DC_CODE, _curF050801.GUP_CODE, _curF050801.CUST_CODE, _curF050801.WMS_ORD_NO, 2, DateTime.Parse(_curF060209.START_TIME), DateTime.Parse(_curF060209.COMPLETE_TIME));

      //產生訂單回檔紀錄[包裝結束=3]
      var finishRecord = f05030101Repo.GetOrderRtnInsertDatas(_curF050801.DC_CODE, _curF050801.GUP_CODE, _curF050801.CUST_CODE, "3", new List<string> { _curF050801.WMS_ORD_NO }).FirstOrDefault();
			if (finishRecord != null)
			{
				finishRecord.CRT_DATE = DateTime.Parse(_curF060209.COMPLETE_TIME);
				finishRecord.WMS_ORD_NO = _curF050801.WMS_ORD_NO;
				finishRecord.CRT_STAFF = Current.Staff;
				finishRecord.CRT_NAME = Current.StaffName;
				insertF050305s.Add(finishRecord);
			}

			// 寫入訂單回檔紀錄
			if (insertF050305s.Any())
				f050305Repo.BulkInsert(insertF050305s,true);

			UpdateF060209(2, "已完成包裝");

			res.MsgCode = _curF060209.PROC_FLAG.ToString();
			res.MsgContent = _curF060209.MSG_CONTENT;

			return res;
		}

		#endregion

		#region 資料處理四 處理外部出貨包裝單據主檔狀態為[0=待處理]處理
		/// <summary>
		/// 資料處理四
		/// 處理外部出貨包裝單據主檔狀態為[0=待處理]處理
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		private ApiResult OutSidePackageDataProcess4()
		{
			var res = new ApiResult { IsSuccessed = true };
			// 資料檢核
			var checkData = CheckOutSidePackageData();
			if (!checkData.IsSuccessed)
			{
				res.IsSuccessed = false;
				res.MsgCode = _curF060209.PROC_FLAG.ToString();
				res.MsgContent = _curF060209.MSG_CONTENT;
				return res;
			}
			// 產生WMS包裝箱資料
			var wmsBoxDetailResult = CreateBoxDetail(checkData.F2501List);
			if (!wmsBoxDetailResult.IsSuccessed)
			{
				res.IsSuccessed = false;
				res.MsgCode = _curF060209.PROC_FLAG.ToString();
				res.MsgContent = _curF060209.MSG_CONTENT;
				return res;
			}
			var consignService = new Shared.Lms.Services.ConsignService();
			var f06020901Repo = new F06020901Repository(Schemas.CoreSchema);
			// 呼叫LMS宅單申請API
			foreach (var wmsBoxDetail in wmsBoxDetailResult.OutSidePackageBoxDetailList)
			{
				var f06020901 = _curF06020901List.First(x => x.BOX_SEQ == wmsBoxDetail.F055001.PACKAGE_BOX_NO);
				var req = consignService.GetApplyRequestData(checkData.F050301, wmsBoxDetail.F055001, wmsBoxDetail.F055002List, _curF050801.SUG_BOX_NO);
				var apiResult = consignService.LmsApplyConsign(req, _curF060209.GUP_CODE);
				if(apiResult.IsSuccessed)
				{
					var rtnData = (LmsDataResult)apiResult.Data;
					f06020901.PROC_FLAG = 1; //已取宅配單
					f06020901.CONSIGN_NO = rtnData.TransportCode;
					f06020901.TRANSPORT_PROVIDER = rtnData.TransportProvider;
					wmsBoxDetail.F055001.PAST_NO = rtnData.TransportCode;
					wmsBoxDetail.F055001.PRINT_DATE = DateTime.Now;
					wmsBoxDetail.F055001.PRINT_FLAG = 1;
					wmsBoxDetail.F055001.IS_CLOSED = "1";
          wmsBoxDetail.F055001.ORG_PAST_NO = rtnData.TransportCode;
          wmsBoxDetail.F055001.ORG_LOGISTIC_CODE = rtnData.TransportProvider;
          wmsBoxDetail.F055001.LOGISTIC_CODE = rtnData.TransportProvider;
          wmsBoxDetail.F055001.CLOSEBOX_TIME = DateTime.Now;
          wmsBoxDetail.F050901 = CreateF050901(f06020901);
				}
				else
				{
					f06020901.PROC_FLAG = 2; //取宅單失敗
				}
				// 直接更新F06020901
				f06020901Repo.Update(f06020901);
			}

			// 如果所有箱宅配單都取得成功
			if (_curF06020901List.All(x=> x.PROC_FLAG == 1))
			{
				_curF060209.PROC_FLAG = 1;//已取得宅配單
				// 執行資料處理三
				return OutSidePackageDataProcess3();
			}
			// 如果所有箱宅配單都取得失敗
			else if (_curF06020901List.All(x=> x.PROC_FLAG == 2))
			{
        UnLock("該出貨單無法取得宅單");
				res.MsgCode = _curF060209.PROC_FLAG.ToString();
				res.MsgContent = _curF060209.MSG_CONTENT;
				return res;
			}
			else
			{
				// 執行資料處理二
				return OutSidePackageDataProcess2();
			}
		}
		#endregion
	}
}
