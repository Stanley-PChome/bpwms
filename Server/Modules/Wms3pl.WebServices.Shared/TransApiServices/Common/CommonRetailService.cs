using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
    public class CommonRetailService
    {
        private WmsTransaction _wmsTransation;
        public CommonRetailService(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;
        }

        #region Private Property
        /// <summary>
        /// 模組名稱
        /// </summary>
        private readonly string _moduleName = "門市主檔";

        /// <summary>
        /// 門市主檔清單
        /// </summary>
        private List<F1910> _f1910List = new List<F1910>();
        #endregion

        #region Main Method
        /// <summary>
        /// Func1
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult RecevieApiDatas(PostRetailDataReq req)
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

            // 檢核Sales
            if (req.Result.Sales == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

            // 檢核資料筆數
            int reqTotal = req.Result.Total != null ? Convert.ToInt32(req.Result.Total) : 0;
            if (req.Result.Sales == null || (req.Result.Sales != null && !tacService.CheckDataCount(reqTotal, req.Result.Sales.Count)))
                return new ApiResult { IsSuccessed = false, MsgCode = "20054", MsgContent = string.Format(tacService.GetMsg("20054"), reqTotal, req.Result.Sales.Count) };


            // 檢核門市主檔筆數是否超過[門市主檔最大筆數]
            int retailMaxCnt = Convert.ToInt32(commonService.GetSysGlobalValue("RetailMaxCnt"));
            if (req.Result.Sales.Count > retailMaxCnt)
                return new ApiResult { IsSuccessed = false, MsgCode = "20018", MsgContent = string.Format(tacService.GetMsg("20018"), _moduleName, req.Result.Sales.Count, retailMaxCnt) };

            #endregion

            // 取得業主編號
            string gupCode = commonService.GetGupCode(req.CustCode);

            // 資料處理1
            return ProcessApiDatas(gupCode, req.CustCode, req.Result.Sales);
        }

        /// <summary>
        /// 資料處理1
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="sales">門市資料物件清單</param>
        /// <returns></returns>
        public ApiResult ProcessApiDatas(string gupCode, string custCode, List<PostRetailDataSalesModel> sales)
        {
            var res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();
            int addCnt = 0;
            int updCnt = 0;
            F1910Repository f1910Repo = new F1910Repository(Schemas.CoreSchema);
            TransApiBaseService tacService = new TransApiBaseService();

            #region Private Property

            var salesBaseNos = sales.Select(x => x.SalesBaseNo).Distinct().ToList();

            // 取得已存在門市清單
            var thirdPartRetailList = f1910Repo.GetDatasForRetail(gupCode, custCode, salesBaseNos).ToList();

            // 將傳入資料Group取得重複門市編號的最後一筆以及重複幾筆
            List<PostRetailSalesGroupModel> saleDatas = new List<PostRetailSalesGroupModel>();
            if (sales != null && sales.Count > 0)
            {
                saleDatas = sales.GroupBy(x => x.SalesBaseNo).Select(x => new PostRetailSalesGroupModel
                {
                    Count = x.Count(),
                    LastData = x.Last()
                }).ToList();
            }
            #endregion

            #region Foreach [門市資料物件] 檢核 and 新增 or 修改 F1910
            List<F1910> exceptData = new List<F1910>();

            saleDatas.ForEach(item =>
            {
                // 資料處理2
                var res1 = CheckSale(gupCode, custCode, item.LastData);

                if (!res1.IsSuccessed)
                {
                    data.AddRange((List<ApiResponse>)res1.Data);

                    var excludeData = thirdPartRetailList.Where(x => x.RETAIL_CODE == item.LastData.SalesBaseNo).SingleOrDefault();

                    if (excludeData != null)
                        exceptData.Add(excludeData);
                }
                else
                {
                    _f1910List.Add(CreateF1910(gupCode, custCode, item.LastData));
                }
            });

            if (exceptData.Any())
                thirdPartRetailList = thirdPartRetailList.Except(exceptData).ToList();
            #endregion

            #region Insert
            var retailCodes = thirdPartRetailList.Select(z => z.RETAIL_CODE).ToList();

            var addDatas = _f1910List.Where(x => !retailCodes.Contains(x.RETAIL_CODE)).ToList();

            if (addDatas.Any())
            {
                f1910Repo.BulkInsert(addDatas);

                // 計算新增數
                addCnt += saleDatas.Where(x => addDatas.Select(z => z.RETAIL_CODE).Contains(x.LastData.SalesBaseNo)).Sum(x => x.Count);
            }
            #endregion

            #region Update
            List<F1910> updDatas = new List<F1910>();

            var updF1910 = thirdPartRetailList.Where(x => _f1910List.Select(z => z.RETAIL_CODE).Contains(x.RETAIL_CODE)).ToList();

            updF1910.ForEach(updData =>
            {
                var currData = _f1910List.Where(z => z.RETAIL_CODE == updData.RETAIL_CODE).SingleOrDefault();
                if (currData != null)
                {
                    // 修改
                    updData.ADDRESS = currData.ADDRESS;
                    updData.TEL = currData.TEL;
                    updData.MAIL = currData.MAIL;
                    updData.RETAIL_NAME = currData.RETAIL_NAME;
                    updData.CONTACT = currData.CONTACT;
                    updData.SALES_BASE_GROUP = currData.SALES_BASE_GROUP;
                    updData.SHORT_SALESBASE_NAME = currData.SHORT_SALESBASE_NAME;
                    updData.UNIFIED_BUSINESS_NO = currData.UNIFIED_BUSINESS_NO;

                    updDatas.Add(updData);
                }
            });

            if (updDatas.Any())
            {
                f1910Repo.BulkUpdate(updDatas);

                // 計算修改數
                updCnt += saleDatas.Where(x => updDatas.Select(z => z.RETAIL_CODE).Contains(x.LastData.SalesBaseNo)).Sum(x => x.Count);
            }
            #endregion

            #region Commit
            if (addDatas.Any() || updDatas.Any())
                _wmsTransation.Complete();
            #endregion

            #region 回傳資料
            // 取得訊息內容 10003
            int total = sales.Count;
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
        /// <param name="sale">門市資料物件</param>
        /// <returns></returns>
        private ApiResult CheckSale(string gupCode, string custCode, PostRetailDataSalesModel sale)
        {
            ApiResult result = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            // 預設值設定
            data.AddRange((List<ApiResponse>)CheckDefaultSetting(gupCode, custCode, sale).Data);

            // 共用欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(gupCode, custCode, sale).Data);

            // 貨主自訂欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckCustomColumnType(gupCode, custCode, sale).Data);

            // 如果以上檢核成功
            if (!data.Any())
            {
                // 共用欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCommonColumnData(gupCode, custCode, sale).Data);

                // 貨主自訂欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCustomColumnValue(gupCode, custCode, sale).Data);
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
        /// <param name="sale">門市資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckDefaultSetting(string gupCode, string custCode, PostRetailDataSalesModel sale)
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
        /// <param name="sale">門市資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckColumnNotNullAndMaxLength(string gupCode, string custCode, PostRetailDataSalesModel sale)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            #region 定義需檢核欄位、必填、型態、長度
            // 門市資料物件
            List<ApiCkeckColumnModel> saleCheckColumnList = new List<ApiCkeckColumnModel>
            {
                new ApiCkeckColumnModel{  Name = "SalesBaseNo",         Type = typeof(string),   MaxLength = 20,    Nullable = false },
                new ApiCkeckColumnModel{  Name = "SalesBaseName",       Type = typeof(string),   MaxLength = 150,   Nullable = false },
                new ApiCkeckColumnModel{  Name = "Contact",             Type = typeof(string),   MaxLength = 50,    Nullable = false },
                new ApiCkeckColumnModel{  Name = "Tel",                 Type = typeof(string),   MaxLength = 40,    Nullable = false },
                new ApiCkeckColumnModel{  Name = "Mail",                Type = typeof(string),   MaxLength = 80 ,   Nullable = false },
                new ApiCkeckColumnModel{  Name = "Address",             Type = typeof(string),   MaxLength = 300 ,  Nullable = false },
                new ApiCkeckColumnModel{  Name = "ShortSalesBaseName",  Type = typeof(string),   MaxLength = 30 },
                new ApiCkeckColumnModel{  Name = "UnifiedBusinessNo",   Type = typeof(string),   MaxLength = 10 },
                new ApiCkeckColumnModel{  Name = "SalesBaseGroup",      Type = typeof(string),   MaxLength = 50 }
            };
            #endregion

            #region 檢查門市欄位必填、最大長度
            List<string> saleIsNullList = new List<string>();
            List<ApiCkeckColumnModel> saleIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

            // 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
            saleCheckColumnList.ForEach(column =>
            {
                var hasValue = DataCheckHelper.CheckRequireColumn(sale, column.Name);

                // 必填
                if (!column.Nullable && !hasValue)
                    saleIsNullList.Add(column.Name);

                // 最大長度
                if (column.MaxLength > 0 && hasValue)
                {
                    if (!DataCheckHelper.CheckDataMaxLength(sale, column.Name, column.MaxLength))
                        saleIsExceedMaxLenthList.Add(column);
                }
            });

            // 必填訊息
            if (saleIsNullList.Any())
            {
                data.Add(new ApiResponse { No = sale.SalesBaseNo, MsgCode = "20016", MsgContent = string.Format(tacService.GetMsg("20016"), sale.SalesBaseNo, string.Join("、", saleIsNullList)) });
            }

            // 最大長度訊息
            if (saleIsExceedMaxLenthList.Any())
            {
                List<string> errorMsgList = saleIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

                string errorMsg = string.Join("、", errorMsgList);

                data.Add(new ApiResponse { No = sale.SalesBaseNo, MsgCode = "20017", MsgContent = string.Format(tacService.GetMsg("20017"), sale.SalesBaseNo, errorMsg) });
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
        /// <param name="sale">門市資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnType(string gupCode, string custCode, PostRetailDataSalesModel sale)
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
        /// <param name="sale">門市資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCommonColumnData(string gupCode, string custCode, PostRetailDataSalesModel sale)
        {
            // 請預留方法
            ApiResult res = new ApiResult();
            res.Data = new List<ApiResponse>();
            return res;
        }

        /// <summary>
        /// 貨主自訂欄位資料檢核
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="sale">門市資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnValue(string gupCode, string custCode, PostRetailDataSalesModel sale)
        {
            // 請預留方法
            ApiResult res = new ApiResult();
            res.Data = new List<ApiResponse>();
            return res;
        }
        #endregion

        #region Protected 建立門市主檔資料
        /// <summary>
        /// 建立門市主檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="returns"></param>
        /// <returns></returns>
        protected F1910 CreateF1910(string gupCode, string custCode, PostRetailDataSalesModel sale)
        {
            return new F1910
            {
                GUP_CODE = gupCode,
                CUST_CODE = custCode,
                RETAIL_CODE = sale.SalesBaseNo,
                RETAIL_NAME = sale.SalesBaseName,
                CHANNEL = "00",
                CONTACT = sale.Contact,
                TEL = sale.Tel,
                MAIL = sale.Mail,
                ADDRESS = sale.Address,
                SHORT_SALESBASE_NAME = sale.ShortSalesBaseName,
                UNIFIED_BUSINESS_NO = sale.UnifiedBusinessNo,
                SALES_BASE_GROUP = sale.SalesBaseGroup,
                TEL2 = null,
                FAX = null,
                CUSTOM_DELVDAYS_TYPE = null,
                DELV_DAYS = null,
                DELV_DAYS_LIMIT = null,
                DELV_DAYS_INFO = null,
                SELF_TAKE = "0",
                NEED_SHIPPING_MARK = "0",
                NOTE = null,
                DELV_NO = null
            };
        }
        #endregion
    }
}
