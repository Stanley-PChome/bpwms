using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
    public class CommonReturnService
    {
        private WmsTransaction _wmsTransation;
        public CommonReturnService(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;
        }

        #region Private Property
        /// <summary>
        /// 門市資料清單
        /// </summary>
        private List<F1910> _retailList;

        /// <summary>
        /// 出貨單存在編號清單
        /// </summary>
        private List<string> _wmsOrdNoList;

        /// <summary>
        /// 退貨類型資料清單
        /// </summary>
        private List<F161203> _returnTypeList;

        /// <summary>
        /// 退貨原因資料清單
        /// </summary>
        private List<F1951> _returnCauseList;

        /// <summary>
        /// 品號清單
        /// </summary>
        private List<string> _itemCodeList;

        /// <summary>
        /// 單據狀態
        /// </summary>
        private string _proc_status;

        /// <summary>
        /// 客戶退貨單主檔
        /// </summary>
        private List<F161201> _f161201List = new List<F161201>();

        /// <summary>
        /// 客戶退貨單明細
        /// </summary>
        private List<F161202> _f161202List = new List<F161202>();

        /// <summary>
        /// 已存在客戶退貨單編號清單
        /// </summary>
        private List<ThirdPartReturns> _thirdPartReturnsList;

        /// <summary>
        /// 失敗客戶退貨單數
        /// </summary>
        private int _failCnt = 0;

        /// <summary>
        /// 暫存訊息池清單
        /// </summary>
        private List<AddMessageReq> _addMessageTempList = new List<AddMessageReq>();
        #endregion

        #region Main Method
        /// <summary>
        /// Func1
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult RecevieApiDatas(PostCreateReturnsReq req)
        {
            CheckTransApiService ctaService = new CheckTransApiService();
            TransApiBaseService tacService = new TransApiBaseService();
            SharedService sharedService = new SharedService();
            CommonService commonService = new CommonService();
            ApiResult res = new ApiResult { IsSuccessed = true };

            #region 資料檢核1

            // 檢核物流中心 必填、是否存在
            ctaService.CheckDcCode(ref res, req);
            if (!res.IsSuccessed)
                return res;

            // 檢核貨主編號 必填、是否存在
            ctaService.CheckCustCode(ref res, req);
            if (!res.IsSuccessed)
                return res;

            // 檢核Returns
            ctaService.CheckResult(ref res, req);
            if (!res.IsSuccessed)
                return res;

            // 檢核Returns
            if (req.Result.Returns == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

            // 檢核資料筆數
            int reqTotal = req.Result.Total != null ? Convert.ToInt32(req.Result.Total) : 0;
            if (req.Result.Returns == null || (req.Result.Returns != null && !tacService.CheckDataCount(reqTotal, req.Result.Returns.Count)))
                return new ApiResult { IsSuccessed = false, MsgCode = "20054", MsgContent = string.Format(tacService.GetMsg("20054"), reqTotal, req.Result.Returns.Count) };

            // 檢核總明細筆數是否超過[客戶退貨明細最大筆數]筆
            int rdMaxCnt = Convert.ToInt32(commonService.GetSysGlobalValue("RDMaxCnt"));
            int detailTotalCnt = req.Result.Returns.Where(x => x.ReturnDetails != null).Sum(x => x.ReturnDetails.Count);
            if (detailTotalCnt > rdMaxCnt)
                return new ApiResult { IsSuccessed = false, MsgCode = "20055", MsgContent = string.Format(tacService.GetMsg("20055"), detailTotalCnt) };
            #endregion

            // 取得業主編號
            string gupCode = commonService.GetGupCode(req.CustCode);

            // 資料處理1
            return ProcessApiDatas(req.DcCode, gupCode, req.CustCode, req.Result.Returns);
        }

        /// <summary>
        /// 資料處理1
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="returns">客戶退貨單資料物件清單</param>
        /// <returns></returns>
        public ApiResult ProcessApiDatas(string dcCode, string gupCode, string custCode, List<PostCreateReturnsModel> returns)
        {
            F161201Repository f161201Repo = new F161201Repository(Schemas.CoreSchema, _wmsTransation);
            F161202Repository f161202Repo = new F161202Repository(Schemas.CoreSchema, _wmsTransation);
            F161203Repository f161203Repo = new F161203Repository(Schemas.CoreSchema);
            F1951Repository f1951Repo = new F1951Repository(Schemas.CoreSchema);
            F1903Repository f1903Repo = new F1903Repository(Schemas.CoreSchema);
            TransApiBaseService tacService = new TransApiBaseService();
            ReturnService rtService = new ReturnService(_wmsTransation);
            CommonService commonService = new CommonService();
            SharedService sharedService = new SharedService(_wmsTransation);
            int insertCnt = 0;

            #region Private Property

            // 取得門市資料清單
            _retailList = commonService.GetRetailList(gupCode, custCode, returns.Select(x => x.ReturnCustCode).Distinct().ToList());

            // 取得出貨單號清單
            _wmsOrdNoList = tacService.GetWmsOrderData(dcCode, gupCode, custCode, returns.Select(x => x.WmsOrdNo).Distinct().ToList());

            // 取得退貨類型清單
            _returnTypeList = f161203Repo.GetALLF161203().ToList();

            // 取得退貨原因清單
            _returnCauseList = f1951Repo.GetDatasByTrueAndCondition(o => o.UCT_ID == "RT").ToList();

            // 取得品號清單
            _itemCodeList = f1903Repo.GetDatasByTrueAndCondition(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode).Select(x => x.ITEM_CODE).ToList();

            // 取得已存在退貨單編號清單
            _thirdPartReturnsList = f161201Repo.GetDatasByCustOrdNo(dcCode, gupCode, custCode, returns.Select(x => x.CustReturnNo).ToList()).Select(x => new ThirdPartReturns { CUST_ORD_NO = x.CUST_ORD_NO, RETURN_NO = x.RETURN_NO }).ToList();
            #endregion

            #region Foreach [客戶退貨單資料物件] 檢核
            var res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            returns.ForEach(currReturn =>
            {
                // 資料處理2
                var res1 = CheckReturn(dcCode, gupCode, custCode, currReturn);

                if (!res1.IsSuccessed)
                {
                    data.AddRange((List<ApiResponse>)res1.Data);

                    // 若驗證失敗，不取消客戶退貨單，所以將驗證失敗的進倉單剃除
                    _thirdPartReturnsList = _thirdPartReturnsList.Where(x => x.CUST_ORD_NO != currReturn.CustReturnNo).ToList();
                }
            });
            #endregion

            #region 處理純新增客戶退貨單
            // 暫存新增的客戶退貨單清單排除[貨主單號已產生WMS訂單]
            var addF161201List = _f161201List.Where(x => !_thirdPartReturnsList.Select(z => z.CUST_ORD_NO).Contains(x.CUST_ORD_NO)).ToList();

            if (addF161201List.Any())
            {
                // 暫存新增的客戶退貨單明細清單.Contain(addF010201List.STOCK_NO)
                var addF161202List = _f161202List.Where(x => addF161201List.Select(z => z.RETURN_NO).Contains(x.RETURN_NO)).ToList();

                addF161201List.ForEach(addF161201 =>
                {
                    // [取得客戶退貨單號]
                    var returnNo = sharedService.GetNewOrdCode("R");

                    // 寫入行事曆訊息池
                    AddMessagePool(_wmsTransation, addF161201.RETURN_NO, returnNo);

                    addF161202List.Where(x => x.RETURN_NO == addF161201.RETURN_NO).ToList().ForEach(addF161202 =>
                    {
                        addF161202.RETURN_NO = returnNo;
                    });

                    addF161201.RETURN_NO = returnNo;
                });

                // BulkInsert addF161201List, addF010202List
                if (addF161201List.Any())
                {
                    f161201Repo.BulkInsert(addF161201List);
                    insertCnt = addF161201List.Count;
                }

                if (addF161202List.Any())
                    f161202Repo.BulkInsert(addF161202List);

                if (addF161201List.Any() || addF161202List.Any())
                    _wmsTransation.Complete();
            }
            #endregion

            #region 已存在客戶退貨單(by 每一筆客戶退貨單commit)

            // 暫存新增的客戶退貨單清單篩選含[貨主單號已產生WMS客戶退貨單]
            var cancelF161201List = _f161201List.Where(x => _thirdPartReturnsList.Select(z => z.CUST_ORD_NO).Contains(x.CUST_ORD_NO)).ToList();
            //    Foreach[找到的WMS單] in [貨主單號已產生WMS客戶退貨單]
            _thirdPartReturnsList.ForEach(o =>
            {
                var wmsTransation2 = new WmsTransaction();
                sharedService = new SharedService(wmsTransation2);
                rtService = new ReturnService(wmsTransation2);

                var returnTmp = returns.Where(x => x.CustReturnNo == o.CUST_ORD_NO).LastOrDefault();

                // 取消客戶退貨單
                var isOk = rtService.CancelNotProcessReturn(dcCode, gupCode, custCode, o.RETURN_NO, returnTmp.ProcFlag);

                // var cancelF010201 = CancelF010201List.Where(x => CUST_ORD_NO =[找到的WMS單].CUST_ORD_NO)
                var cancelF161201 = cancelF161201List.Where(x => x.CUST_ORD_NO == o.CUST_ORD_NO).SingleOrDefault();

                if (isOk)
                {
                    if (cancelF161201 == null)
                        // 寫入行事曆訊息池
                        sharedService.AddMessagePool("9", dcCode, gupCode, custCode, "API20956", string.Format(tacService.GetMsg("20956"), o.RETURN_NO), "", "0", "SCH");
                }
                else
                {
                    // [單號{0}]此單據WMS客戶退貨單已在作業中，無法取消或更新單據
                    data.Add(new ApiResponse { MsgCode = "20073", MsgContent = string.Format(tacService.GetMsg("20073"), o.CUST_ORD_NO), No = o.CUST_ORD_NO });

                    _failCnt++;

                    if (cancelF161201 != null)
                    {
                        // 暫存新增的客戶退貨單清單.Remove(cancelF010201)
                        _f161201List.Remove(cancelF161201);

                        // var cancelF010202 = 暫存新增的客戶退貨單明細清單.Contain(cancelF010201.STOCK_NO)
                        var cancelF161202 = _f161202List.Where(x => x.RETURN_NO == cancelF161201.RETURN_NO);

                        // 暫存新增的客戶退貨明細清單.Except(cancelF010202)
                        _f161202List = _f161202List.Except(cancelF161202).ToList();
                    }
                }

                wmsTransation2.Complete();
            });

            #endregion

            #region 寫入行事曆訊息池
            var wmsTransation3 = new WmsTransaction();
            sharedService = new SharedService(wmsTransation3);

            // 取得訊息內容 20955
            int total = returns.Count;
            int successedCnt = total - _failCnt;

            string msgContent = string.Format(tacService.GetMsg("20955"),
                successedCnt,
                _failCnt,
                total);

            // 寫入行事曆訊息池
            sharedService.AddMessagePool("9", dcCode, gupCode, custCode, "API20955", msgContent, "", "0", "SCH");

            wmsTransation3.Complete();
            #endregion

            _wmsTransation.Complete();

            res.IsSuccessed = !data.Any();
            res.MsgCode = "20955";
            res.MsgContent = msgContent;
            res.InsertCnt = insertCnt;
            res.UpdateCnt = total - insertCnt - _failCnt;
            res.FailureCnt = _failCnt;
            res.TotalCnt = total;

            res.Data = data.Any() ? data : null;

            return res;
        }

        /// <summary>
        /// 寫入行事曆訊息池
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="returnNo"></param>
        private void AddMessagePool(WmsTransaction wmsTransaction, string guid, string returnNo)
        {
            SharedService sharedService = new SharedService(wmsTransaction);

            // 寫入行事曆訊息池
            var addMessageList = _addMessageTempList.Where(x => x.Guid == guid).ToList();
            addMessageList.ForEach(o =>
            {
                o.MessageContent = o.MessageContent.Replace(guid, returnNo);
                sharedService.AddMessagePool(o.TicketType, o.DcCode, o.GupCode, o.CustCode, o.MsgNo, o.MessageContent, o.NotifyOrdNo, o.TargetType, o.TargetCode);
            });
        }

        /// <summary>
        /// 資料處理2
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="returns">客戶退貨單資料物件</param>
        /// <returns></returns>
        private ApiResult CheckReturn(string dcCode, string gupCode, string custCode, PostCreateReturnsModel returns)
        {
            ApiResult result = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            // 預設proc_status = 0
            _proc_status = "0";

            // 預設值設定
            data.AddRange((List<ApiResponse>)CheckDefaultSetting(dcCode, gupCode, custCode, returns).Data);

            // 共用欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(dcCode, gupCode, custCode, returns).Data);

            // 貨主自訂欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckCustomColumnType(dcCode, gupCode, custCode, returns).Data);

            // 如果以上檢核成功
            if (!data.Any())
            {
                // 共用欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCommonColumnData(dcCode, gupCode, custCode, returns).Data);

                // 貨主自訂欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCustomColumnValue(dcCode, gupCode, custCode, returns).Data);

                // 如果以上檢核成功
                if (!data.Any())
                {
                    // 產生退貨單資料
                    CreateReturn(dcCode, gupCode, custCode, returns);
                }
                else
                {
                    _failCnt++;
                }
            }
            else
            {
                _failCnt++;
            }

            result.IsSuccessed = !data.Any();
            result.Data = data;

            return result;
        }

        /// <summary>
        /// 產生退貨單資料
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="returns">客戶退貨單資料物件</param>
        private void CreateReturn(string dcCode, string gupCode, string custCode, PostCreateReturnsModel returns)
        {
            // 如果<參數5>.Status<> D 才往下執行
            if (returns.ProcFlag != "D")
            {
                // 建立客戶退貨單主檔F161201
                F161201 f161201 = CreateF161201(dcCode, gupCode, custCode, returns);

                // 建立客戶退貨單明細檔F161202
                List<F161202> f161202List = CreateF161202List(returns, f161201);

                _f161201List.Add(f161201);
                _f161202List.AddRange(f161202List);
            }
        }
        #endregion

        #region Protected 檢核
        /// <summary>
        /// 預設值設定
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="returns">客戶退貨單資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, PostCreateReturnsModel returns)
        {
            // 請預留方法
            ApiResult res = new ApiResult();
            res.Data = new List<ApiResponse>();

            if (string.IsNullOrWhiteSpace(returns.ZipCode))
                returns.ZipCode = "000";

            if (string.IsNullOrWhiteSpace(returns.Address))
                returns.Address = "NA";

            if (string.IsNullOrWhiteSpace(returns.Contact))
                returns.Contact = "NA";

            if (string.IsNullOrWhiteSpace(returns.PhoneNo))
                returns.PhoneNo = "NA";

            return res;
        }

        /// <summary>
        /// 共用欄位格式檢核
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="returns">客戶退貨單資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, PostCreateReturnsModel returns)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            #region 定義需檢核欄位、必填、型態、長度
            // 客戶退貨單資料物件
            List<ApiCkeckColumnModel> warehouseInsCheckColumnList = new List<ApiCkeckColumnModel>();

            // 客戶退貨單明細資料
            List<ApiCkeckColumnModel> warehouseInDetailCheckColumnList = new List<ApiCkeckColumnModel>();

            if (returns.ProcFlag == "D")
            {
                // 刪除
                warehouseInsCheckColumnList = new List<ApiCkeckColumnModel>
                {
                    new ApiCkeckColumnModel{  Name = "CustReturnNo",    Type = typeof(string),   MaxLength = 20,  Nullable = false },
                    new ApiCkeckColumnModel{  Name = "ProcFlag",        Type = typeof(string),   MaxLength = 1,   Nullable = false }
                };
            }
            else if (returns.ProcFlag == "0")
            {
                warehouseInsCheckColumnList = new List<ApiCkeckColumnModel>
                {
                    new ApiCkeckColumnModel{  Name = "CustReturnNo",    Type = typeof(string),   MaxLength = 20,  Nullable = false },
                    new ApiCkeckColumnModel{  Name = "ReturnDate",      Type = typeof(DateTime), MaxLength = 0,   Nullable = false },
                    new ApiCkeckColumnModel{  Name = "Type",            Type = typeof(string),   MaxLength = 1,   Nullable = false },
                    new ApiCkeckColumnModel{  Name = "WmsOrdNo",        Type = typeof(string),   MaxLength = 20 },
                    new ApiCkeckColumnModel{  Name = "ReturnCustCode",  Type = typeof(string),   MaxLength = 20,  Nullable = returns.Type != "0" },
                    new ApiCkeckColumnModel{  Name = "ReturnType",      Type = typeof(string),   MaxLength = 5 ,  Nullable = false },
                    new ApiCkeckColumnModel{  Name = "ReturnCause",     Type = typeof(string),   MaxLength = 5 ,  Nullable = false},
                    new ApiCkeckColumnModel{  Name = "BatchNo",         Type = typeof(string),   MaxLength = 50 },
                    new ApiCkeckColumnModel{  Name = "ZipCode",         Type = typeof(string),   MaxLength = 6,   Nullable = false },
                    new ApiCkeckColumnModel{  Name = "Address",         Type = typeof(string),   MaxLength = 200, Nullable = false },
                    new ApiCkeckColumnModel{  Name = "Contact",         Type = typeof(string),   MaxLength = 20,  Nullable = false },
                    new ApiCkeckColumnModel{  Name = "PhoneNo",         Type = typeof(string),   MaxLength = 20,  Nullable = false },
                    new ApiCkeckColumnModel{  Name = "CustCategory",    Type = typeof(string),   MaxLength = 20 },
                    new ApiCkeckColumnModel{  Name = "Memo",            Type = typeof(string),   MaxLength = 100 },
                    new ApiCkeckColumnModel{  Name = "ProcFlag",        Type = typeof(string),   MaxLength = 1,   Nullable = false }
                };

                warehouseInDetailCheckColumnList = new List<ApiCkeckColumnModel>
                {
                    new ApiCkeckColumnModel{  Name = "ItemCode",        Type = typeof(string),   MaxLength = 20,  Nullable = false },
                    new ApiCkeckColumnModel{  Name = "ItemSeq",         Type = typeof(string),   MaxLength = 4,   Nullable = false },
                    new ApiCkeckColumnModel{  Name = "Qty",             Type = typeof(int),      MaxLength = 6,   Nullable = false }
                };
            }

            #endregion

            #region 檢查客戶退貨單欄位必填、最大長度
            List<string> returnIsNullList = new List<string>();
            List<ApiCkeckColumnModel> returnIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

            // 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
            warehouseInsCheckColumnList.ForEach(column =>
            {
                // 必填
                if (!column.Nullable)
                {
                    if (!DataCheckHelper.CheckRequireColumn(returns, column.Name))
                        returnIsNullList.Add(column.Name);
                }

                // 最大長度
                if (column.MaxLength > 0)
                {
                    if (!DataCheckHelper.CheckDataMaxLength(returns, column.Name, column.MaxLength))
                        returnIsExceedMaxLenthList.Add(column);
                }
            });

            // 必填訊息
            if (returnIsNullList.Any())
            {
                data.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20058", MsgContent = string.Format(tacService.GetMsg("20058"), returns.CustReturnNo, string.Join("、", returnIsNullList)) });
            }

            // 最大長度訊息
            if (returnIsExceedMaxLenthList.Any())
            {
                List<string> errorMsgList = returnIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

                string errorMsg = string.Join("、", errorMsgList);

                data.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20059", MsgContent = string.Format(tacService.GetMsg("20059"), returns.CustReturnNo, errorMsg) });
            }

            #endregion

            #region 檢查客戶退貨單明細欄位必填、最大長度
            if (returns.ProcFlag != "D")
            {
                List<string> returnDetailIsNullList;
                List<ApiCkeckColumnModel> returnDetailIsExceedMaxLenthList;

                if (returns.ReturnDetails != null && returns.ReturnDetails.Any())
                {
                    for (int i = 0; i < returns.ReturnDetails.Count; i++)
                    {
                        var currDetail = returns.ReturnDetails[i];

                        returnDetailIsNullList = new List<string>();
                        returnDetailIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

                        warehouseInDetailCheckColumnList.ForEach(o =>
                        {
                        // 必填
                        if (!o.Nullable)
                            {
                                if (!DataCheckHelper.CheckRequireColumn(currDetail, o.Name))
                                    returnDetailIsNullList.Add(o.Name);
                            }

                        // 最大長度
                        if (o.MaxLength > 0)
                            {
                                if (!DataCheckHelper.CheckDataMaxLength(currDetail, o.Name, o.MaxLength))
                                    returnDetailIsExceedMaxLenthList.Add(o);
                            }
                        });

                        // 必填訊息
                        if (returnDetailIsNullList.Any())
                        {
                            data.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20058", MsgContent = string.Format(tacService.GetMsg("20058"), $"{returns.CustReturnNo}第{i + 1}筆明細", string.Join("、", returnDetailIsNullList)) });
                        }

                        // 最大長度訊息
                        if (returnDetailIsExceedMaxLenthList.Any())
                        {
                            List<string> errorMsgList = returnDetailIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

                            string errorMsg = string.Join("、", errorMsgList);

                            data.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20059", MsgContent = string.Format(tacService.GetMsg("20059"), $"{returns.CustReturnNo}第{i + 1}筆明細", errorMsg) });
                        }
                    }
                }
            }
            #endregion

            res.Data = data;

            return res;
        }

        /// <summary>
        /// 貨主自訂欄位格式檢核
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="returns">客戶退貨單資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, PostCreateReturnsModel returns)
        {
            // 請預留方法
            ApiResult res = new ApiResult();
            res.Data = new List<ApiResponse>();
            return res;
        }

        /// <summary>
        /// 共用欄位資料檢核
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="returns">客戶退貨單資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, PostCreateReturnsModel returns)
        {
            CheckReturnService crService = new CheckReturnService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            #region 主檔欄位資料檢核

            // 檢查ProcFlag
            crService.CheckProcFlag(data, returns);

            // 檢查貨主單號是否存在
            crService.CheckCustExistForThirdPart(data, _thirdPartReturnsList, returns);

            // 檢核單據類型是否存在
            crService.CheckTypeExist(data, returns);

            // 檢核出貨單號是否存在
            crService.CheckWmsOrdNoExist(data, _wmsOrdNoList, returns);

            // 檢查貨主單號是否存在
            crService.CheckCustExist(ref _f161201List, ref _f161202List, dcCode, gupCode, custCode, returns);

            // 檢查門市編號
            crService.CheckReturnCustCode(data, returns, _retailList);

            // 檢查退貨類型
            crService.CheckReturnType(data, returns, _returnTypeList);

            // 檢查退貨原因
            crService.CheckReturnCause(data, returns, _returnCauseList);
            #endregion

            #region 明細欄位資料檢核

            // 檢查明細筆數
            crService.CheckDetailCnt(data, returns);

            // 檢核項次必須大於0，且同一張單據內的序號不可重複
            crService.CheckDetailSeq(data, returns);

            // 檢查明細客戶退貨數量
            crService.CheckDetailQty(data, returns);

            #endregion

            #region 檢查資料是否完整
            crService.CheckReturnData(data, gupCode, custCode, returns);
            #endregion

            res.Data = data;

            return res;
        }

        /// <summary>
        /// 貨主自訂欄位資料檢核
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="returns">客戶退貨單資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, PostCreateReturnsModel returns)
        {
            // 請預留方法
            ApiResult res = new ApiResult();
            res.Data = new List<ApiResponse>();
            return res;
        }
        #endregion

        #region Protected 建立客戶退貨單主檔、明細、檢核資料
        /// <summary>
        /// 建立客戶退貨單主檔
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="returns"></param>
        /// <returns></returns>
        protected F161201 CreateF161201(string dcCode, string gupCode, string custCode, PostCreateReturnsModel returns)
        {
            var f1910 = _retailList.Where(x => x.RETAIL_CODE == returns.ReturnCustCode).SingleOrDefault();

            var f161201 = new F161201
            {
                RETURN_NO = Guid.NewGuid().ToString(),
                RETURN_DATE = returns.ReturnDate ?? Convert.ToDateTime(returns.ReturnDate),
                POSTING_DATE = null,
                STATUS = _proc_status,
                CUST_ORD_NO = returns.CustReturnNo,
                WMS_ORD_NO = returns.WmsOrdNo,
                RTN_CUST_CODE = returns.ReturnCustCode,
                RTN_CUST_NAME = f1910 == null ? string.Empty : f1910.RETAIL_NAME,
                RTN_TYPE_ID = returns.Type == "0" ? "3" : "4",
                RTN_CAUSE = returns.Type == "0" ? "105" : "106",
                SOURCE_TYPE = null,
                SOURCE_NO = null,
                DISTR_CAR = "0",
                COST_CENTER = null,
                ADDRESS = string.IsNullOrWhiteSpace(returns.Address) && f1910 != null ? f1910.ADDRESS : $"{(string.IsNullOrWhiteSpace(returns.ZipCode) ? "000" : returns.ZipCode)} {(string.IsNullOrWhiteSpace(returns.Address) ? "NA" : returns.Address)}",
                CONTACT = string.IsNullOrWhiteSpace(returns.Contact) && f1910 != null ? f1910.CONTACT : returns.Contact,
                TEL = string.IsNullOrWhiteSpace(returns.PhoneNo) && f1910 != null ? f1910.TEL : returns.PhoneNo,
                MEMO = returns.Memo,
                DC_CODE = dcCode,
                GUP_CODE = gupCode,
                CUST_CODE = custCode,
                ORD_PROP = returns.Type == "0" ? "R2" : "R1",
                DISTR_CAR_NO = null,
                EXCHANGEID = null,
                FOREIGN_WMSNO = returns.CustReturnNo,
                BATCH_NO = returns.BatchNo,
                SHIPWAY = null,
                PROC_FLAG = "0",
                IMPORT_FLAG = "0"
            };

            if (string.IsNullOrWhiteSpace(f161201.CONTACT))
                f161201.CONTACT = "NA";

            if (string.IsNullOrWhiteSpace(f161201.TEL))
                f161201.TEL = "NA";

            return f161201;
        }

        /// <summary>
        /// 建立客戶退貨單明細
        /// </summary>
        /// <param name="returns"></param>
        /// <param name="f161201"></param>
        /// <returns></returns>
        protected List<F161202> CreateF161202List(PostCreateReturnsModel returns, F161201 f161201)
        {
            List<F161202> result = new List<F161202>();

            if (returns.ReturnDetails != null || (returns.ReturnDetails != null && returns.ReturnDetails.Any()))
            {
                result = returns.ReturnDetails.Select(x => new F161202
                {
                    RETURN_NO = f161201.RETURN_NO,
                    RETURN_SEQ = x.ItemSeq,
                    ITEM_CODE = x.ItemCode,
                    RTN_QTY = x.Qty,
                    DC_CODE = f161201.DC_CODE,
                    GUP_CODE = f161201.GUP_CODE,
                    CUST_CODE = f161201.CUST_CODE,
                    RTN_CUS_FLAG = "0"
                }).ToList();
            }

            return result;
        }
        #endregion
    }
}
