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
    public class CommonVendorService
    {
        private WmsTransaction _wmsTransation;
        public CommonVendorService(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;
        }

        #region Private Property
        /// <summary>
        /// 模組名稱
        /// </summary>
        private readonly string _moduleName = "供應商主檔";
        /// <summary>
        /// 供應商主檔清單
        /// </summary>
        private List<F1908> _f1908List = new List<F1908>();
        #endregion

        #region Main Method
        /// <summary>
        /// Func1
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult RecevieApiDatas(PostVendorDataReq req)
        {
            CheckTransApiService ctaService = new CheckTransApiService();
            TransApiBaseService tacService = new TransApiBaseService();
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

            // 檢核ItemLevels
            if (req.Result.Vendors == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

            // 檢核資料筆數
            int reqTotal = req.Result.Total != null ? Convert.ToInt32(req.Result.Total) : 0;
            if (req.Result.Vendors == null || (req.Result.Vendors != null && !tacService.CheckDataCount(reqTotal, req.Result.Vendors.Count)))
                return new ApiResult { IsSuccessed = false, MsgCode = "20054", MsgContent = string.Format(tacService.GetMsg("20054"), reqTotal, req.Result.Vendors.Count) };


            // 檢核供應商主檔筆數是否超過[供應商主檔最大筆數]
            int vendorMaxCnt = Convert.ToInt32(commonService.GetSysGlobalValue("VendorMaxCnt"));
            if (req.Result.Vendors.Count > vendorMaxCnt)
                return new ApiResult { IsSuccessed = false, MsgCode = "20018", MsgContent = string.Format(tacService.GetMsg("20018"), _moduleName, req.Result.Vendors.Count, vendorMaxCnt) };
            #endregion

            // 取得業主編號
            string gupCode = commonService.GetGupCode(req.CustCode);

            // 資料處理1
            return ProcessApiDatas(gupCode, req.CustCode, req.Result.Vendors);
        }

        /// <summary>
        /// 資料處理1
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="vendors">供應商主檔資料物件清單</param>
        /// <returns></returns>
        public ApiResult ProcessApiDatas(string gupCode, string custCode, List<PostVendorDataVendorsModel> vendors)
        {
            var res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();
            int addCnt = 0;
            int updCnt = 0;
            F1908Repository f1908Repo = new F1908Repository(Schemas.CoreSchema, _wmsTransation);
            TransApiBaseService tacService = new TransApiBaseService();
            CommonService commonService = new CommonService();

            #region Private Property

            var vendorCodes = vendors.Select(x => x.VnrCode).Distinct().ToList();

            // 取得已存在供應商主檔清單
            var thirdPartF1908List = f1908Repo.GetDatas(gupCode, custCode, vendorCodes);

            // 將傳入資料Group取得重複供應商編號的最後一筆以及重複幾筆
            List<PostVendorGroupModel> vnrDatas = new List<PostVendorGroupModel>();
            if (vendors != null && vendors.Count > 0)
            {
                vnrDatas = vendors.GroupBy(x => x.VnrCode).Select(x => new PostVendorGroupModel
                {
                    Count = x.Count(),
                    LastData = x.Last()
                }).ToList();
            }
            #endregion

            #region Foreach [供應商資料物件] 檢核
            vnrDatas.ForEach(item =>
            {
                // 資料處理2
                var res1 = CheckVendor(gupCode, custCode, item.LastData);

                if (!res1.IsSuccessed)
                {
                    data.AddRange((List<ApiResponse>)res1.Data);

                    thirdPartF1908List = thirdPartF1908List.Where(x => x.VNR_CODE != item.LastData.VnrCode);
                }
                else
                {
                    _f1908List.Add(CreateF1908(gupCode, custCode, item.LastData));
                }
            });
            #endregion

            #region Insert
            var vnrCodes = thirdPartF1908List.Select(z => z.VNR_CODE).ToList();

            var addDatas = _f1908List.Where(x => !vnrCodes.Contains(x.VNR_CODE)).ToList();

            if (addDatas.Any())
            {
                f1908Repo.BulkInsert(addDatas);

                // 計算新增數
                addCnt += vnrDatas.Where(x => addDatas.Select(z => z.VNR_CODE).Contains(x.LastData.VnrCode)).Sum(x => x.Count);
            }
            #endregion

            #region Update
            List<F1908> updDatas = new List<F1908>();
            thirdPartF1908List.Where(x => _f1908List.Select(z => z.VNR_CODE).Contains(x.VNR_CODE)).ToList().ForEach(updData =>
            {
                var currData = _f1908List.Where(z => z.VNR_CODE == updData.VNR_CODE).SingleOrDefault();
                if (currData != null)
                {
                    updData.VNR_NAME = currData.VNR_NAME;
                    updData.STATUS = currData.STATUS;
                    updData.UNI_FORM = currData.UNI_FORM;
                    updData.BOSS = currData.BOSS;
                    updData.TEL = currData.TEL;
                    updData.ZIP = currData.ZIP;
                    updData.ADDRESS = currData.ADDRESS;
                    updData.ITEM_CONTACT = currData.ITEM_CONTACT;
                    updData.ITEM_TEL = currData.ITEM_TEL;
                    updData.ITEM_CEL = currData.ITEM_CEL;
                    updData.ITEM_MAIL = currData.ITEM_MAIL;
                    updData.BILL_CONTACT = currData.BILL_CONTACT;
                    updData.BILL_TEL = currData.BILL_TEL;
                    updData.BILL_CEL = currData.BILL_CEL;
                    updData.BILL_MAIL = currData.BILL_MAIL;
                    updData.INV_ZIP = currData.INV_ZIP;
                    updData.INV_ADDRESS = currData.INV_ADDRESS;
                    updData.TAX_TYPE = currData.TAX_TYPE;
                    updData.CHECKPERCENT = currData.CHECKPERCENT;
                    updData.RET_MEMO = currData.RET_MEMO;
										updData.DELIVERY_WAY = currData.DELIVERY_WAY;
										updDatas.Add(updData);
                }
            });

            if (updDatas.Any())
            {
                f1908Repo.BulkUpdate(updDatas);

                // 計算修改數
                updCnt += vnrDatas.Where(x => updDatas.Select(z => z.VNR_CODE).Contains(x.LastData.VnrCode)).Sum(x => x.Count);
            }
            #endregion

            #region Commit
            if (addDatas.Any() || updDatas.Any())
                _wmsTransation.Complete();
            #endregion

            #region 回傳資料
            // 取得訊息內容 10003
            int total = vendors.Count;
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
        /// <param name="vendor">供應商主檔資料物件</param>
        /// <returns></returns>
        private ApiResult CheckVendor(string gupCode, string custCode, PostVendorDataVendorsModel vendor)
        {
            ApiResult result = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            // 預設值設定
            data.AddRange((List<ApiResponse>)CheckDefaultSetting(gupCode, custCode, vendor).Data);

            // 共用欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(gupCode, custCode, vendor).Data);

            // 貨主自訂欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckCustomColumnType(gupCode, custCode, vendor).Data);

            // 如果以上檢核成功
            if (!data.Any())
            {
                // 共用欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCommonColumnData(gupCode, custCode, vendor).Data);

                // 貨主自訂欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCustomColumnValue(gupCode, custCode, vendor).Data);
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
        /// <param name="itemLevel">商品階層資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckDefaultSetting(string gupCode, string custCode, PostVendorDataVendorsModel vendor)
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
        /// <param name="vendor">供應商主檔資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckColumnNotNullAndMaxLength(string gupCode, string custCode, PostVendorDataVendorsModel vendor)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            #region 定義需檢核欄位、必填、型態、長度

            // 供應商主檔資料物件
            List<ApiCkeckColumnModel> vendorCheckColumnList = new List<ApiCkeckColumnModel>
            {
                new ApiCkeckColumnModel{  Name = "VnrCode",      Type = typeof(string),   MaxLength = 20,   Nullable = false },
                new ApiCkeckColumnModel{  Name = "VnrName",      Type = typeof(string),   MaxLength = 60,   Nullable = false },
                new ApiCkeckColumnModel{  Name = "Status",       Type = typeof(string),   MaxLength = 1,    Nullable = false },
                new ApiCkeckColumnModel{  Name = "UniForm",      Type = typeof(string),   MaxLength = 16 },
                new ApiCkeckColumnModel{  Name = "Boss",         Type = typeof(string),   MaxLength = 40 },
                new ApiCkeckColumnModel{  Name = "Tel",          Type = typeof(string),   MaxLength = 40 },
                new ApiCkeckColumnModel{  Name = "Zip",          Type = typeof(string),   MaxLength = 10 },
                new ApiCkeckColumnModel{  Name = "Address",      Type = typeof(string),   MaxLength = 120 },
                new ApiCkeckColumnModel{  Name = "ItemContact",  Type = typeof(string),   MaxLength = 20 },
                new ApiCkeckColumnModel{  Name = "ItemTel",      Type = typeof(string),   MaxLength = 40 },
                new ApiCkeckColumnModel{  Name = "ItemCel",      Type = typeof(string),   MaxLength = 40 },
                new ApiCkeckColumnModel{  Name = "ItemMail",     Type = typeof(string),   MaxLength = 80 },
                new ApiCkeckColumnModel{  Name = "BillContact",  Type = typeof(string),   MaxLength = 20 },
                new ApiCkeckColumnModel{  Name = "BillTel",      Type = typeof(string),   MaxLength = 40 },
                new ApiCkeckColumnModel{  Name = "BillCel",      Type = typeof(string),   MaxLength = 40 },
                new ApiCkeckColumnModel{  Name = "BillMail",     Type = typeof(string),   MaxLength = 80 },
                new ApiCkeckColumnModel{  Name = "InvZip",       Type = typeof(string),   MaxLength = 10 },
                new ApiCkeckColumnModel{  Name = "InvAddress",   Type = typeof(string),   MaxLength = 120 },
                new ApiCkeckColumnModel{  Name = "TaxType",      Type = typeof(string),   MaxLength = 2,   Nullable = false },
                new ApiCkeckColumnModel{  Name = "Checkpercent", Type = typeof(decimal),  MaxLength = 14 },
                new ApiCkeckColumnModel{  Name = "Memo",         Type = typeof(string),   MaxLength = 255 },
								new ApiCkeckColumnModel{  Name = "DeliveryWay",         Type = typeof(string),   MaxLength = 1 , Nullable = false }
						};
            #endregion

            #region 檢查欄位必填、最大長度
            List<string> vendorIsNullList = new List<string>();
            List<ApiCkeckColumnModel> vendorIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

            // 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
            vendorCheckColumnList.ForEach(column =>
            {
                // 是否有資料
                bool hasValue = DataCheckHelper.CheckRequireColumn(vendor, column.Name);

                // 必填
                if (!column.Nullable && !hasValue)
                    vendorIsNullList.Add(column.Name);

                // 最大長度
                if (column.MaxLength > 0 && hasValue)
                {
                    bool checkResult = true;
                    switch (column.Name)
                    {
                        case "Checkpercent":
                            // 檢核是否符合decimal(14,11)
                            checkResult = DataCheckHelper.CheckDataIsDecimal(vendor, column.Name, 3, 11);
                            break;
                        default:
                            checkResult = DataCheckHelper.CheckDataMaxLength(vendor, column.Name, column.MaxLength);
                            break;
                    }

                    if (!checkResult)
                        vendorIsExceedMaxLenthList.Add(column);
                }
            });

            // 必填訊息
            if (vendorIsNullList.Any())
            {
                data.Add(new ApiResponse { No = vendor.VnrCode, MsgCode = "20016", MsgContent = string.Format(tacService.GetMsg("20016"), vendor.VnrCode, string.Join("、", vendorIsNullList)) });
            }

            // 最大長度訊息
            if (vendorIsExceedMaxLenthList.Any())
            {
                List<string> errorMsgList = vendorIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

                string errorMsg = string.Join("、", errorMsgList);

                data.Add(new ApiResponse { No = vendor.VnrCode, MsgCode = "20017", MsgContent = string.Format(tacService.GetMsg("20017"), vendor.VnrCode, errorMsg) });
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
        /// <param name="vendor">供應商主檔資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnType(string gupCode, string custCode, PostVendorDataVendorsModel vendor)
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
        /// <param name="vendor">供應商主檔資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCommonColumnData(string gupCode, string custCode, PostVendorDataVendorsModel vendor)
        {
            CheckVendorService cvService = new CheckVendorService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            // 檢核抽驗比例是否大於0
            cvService.CheckValueNotZero(data, vendor);

            // 檢核狀態是否正確
            cvService.CheckStatus(data, vendor);

            // 檢核稅別是否正確
            cvService.CheckTaxType(data, vendor);

			      // 檢核配送方式
						cvService.CheckDeliveryWay(data, vendor);

            res.Data = data;
            return res;
        }

        /// <summary>
        /// 貨主自訂欄位資料檢核
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="vendor">供應商主檔資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnValue(string gupCode, string custCode, PostVendorDataVendorsModel vendor)
        {
            // 請預留方法
            ApiResult res = new ApiResult();
            res.Data = new List<ApiResponse>();
            return res;
        }
        #endregion

        #region Protected 建立供應商主檔資料
        /// <summary>
        /// 建立商品階層主檔
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="vendor">供應商主檔資料物件</param>
        /// <returns></returns>
        protected F1908 CreateF1908(string gupCode, string custCode, PostVendorDataVendorsModel vendor)
        {
            return new F1908
            {
                VNR_CODE = vendor.VnrCode,
                VNR_NAME = vendor.VnrName,
                STATUS = vendor.Status,
                UNI_FORM = vendor.UniForm,
                BOSS = vendor.Boss,
                TEL = vendor.Tel,
                ZIP = vendor.Zip,
                ADDRESS = string.IsNullOrWhiteSpace(vendor.Address) ? "NA" : vendor.Address,
                ITEM_CONTACT = vendor.ItemContact,
                ITEM_TEL = vendor.ItemTel,
                ITEM_CEL = vendor.ItemCel,
                ITEM_MAIL = vendor.ItemMail,
                BILL_CONTACT = vendor.BillContact,
                BILL_TEL = vendor.BillTel,
                BILL_CEL = vendor.BillCel,
                BILL_MAIL = vendor.BillMail,
                INV_ZIP = vendor.InvZip,
                INV_ADDRESS = vendor.InvAddress,
                TAX_TYPE = vendor.TaxType,
                CURRENCY = "NTD",
                PAY_FACTOR = "0",
                PAY_TYPE = "0",
                BANK_CODE = null,
                BANK_NAME = null,
                BANK_ACCOUNT = null,
                LEADTIME = null,
                ORD_DATE = null,
                ORD_CIRCLE = null,
                DELV_TIME = null,
                VNR_LIM_QTY = null,
                ORD_STOCK_QTY = null,
                DELI_TYPE = null,
                INVO_TYPE = "0",
                CHECKPERCENT = vendor.Checkpercent,
                VNR_TYPE = "0",
                RET_MEMO = vendor.Memo,
                EXTENSION_A = null,
                EXTENSION_B = null,
                EXTENSION_C = null,
                EXTENSION_D = null,
                EXTENSION_E = null,
                CUST_CODE = custCode,
                GUP_CODE = gupCode,
								DELIVERY_WAY = vendor.DeliveryWay
            };
        }
        #endregion
    }
}
