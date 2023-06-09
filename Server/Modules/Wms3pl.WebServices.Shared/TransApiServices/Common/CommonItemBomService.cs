using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
    public class CommonItemBomService
    {
        private WmsTransaction _wmsTransation;
        public CommonItemBomService(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;
        }

        #region Private Property
        /// <summary>
        /// 模組名稱
        /// </summary>
        private readonly string _moduleName = "商品組合主檔";

        /// <summary>
        /// 商品組合主檔清單
        /// </summary>
        private List<F910101> _f910101List = new List<F910101>();

        /// <summary>
        /// 商品組合明細檔清單
        /// </summary>
        private List<F910102> _f910102List = new List<F910102>();

        /// <summary>
        /// 商品編號清單
        /// </summary>
        private List<string> _itemCodeList;

        /// <summary>
        /// 商品單位編號清單
        /// </summary>
        private List<F91000302> _thirdPartF91000302List;

        /// <summary>
        /// 單位名稱清單
        /// </summary>
        private List<string> _unitIdList;
        #endregion

        #region Main Method
        /// <summary>
        /// Func1
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult RecevieApiDatas(PostItemBomReq req)
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

            // 檢核ItemBoms
            if (req.Result.Boms == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

            // 檢核資料筆數
            int reqTotal = req.Result.Total != null ? Convert.ToInt32(req.Result.Total) : 0;
            if (req.Result.Boms == null || (req.Result.Boms != null && !tacService.CheckDataCount(reqTotal, req.Result.Boms.Count)))
                return new ApiResult { IsSuccessed = false, MsgCode = "20054", MsgContent = string.Format(tacService.GetMsg("20054"), reqTotal, req.Result.Boms.Count) };

            // 檢核商品組合主檔筆數是否超過[商品組合主檔最大筆數]
            int itemBomMaxCnt = Convert.ToInt32(commonService.GetSysGlobalValue("ItemBomMaxCnt"));
            if (req.Result.Boms.Count > itemBomMaxCnt)
                return new ApiResult { IsSuccessed = false, MsgCode = "20018", MsgContent = string.Format(tacService.GetMsg("20018"), _moduleName, req.Result.Boms.Count, itemBomMaxCnt) };
            #endregion

            // 取得業主編號
            string gupCode = commonService.GetGupCode(req.CustCode);

            // 資料處理1
            return ProcessApiDatas(gupCode, req.CustCode, req.Result.Boms);
        }

        /// <summary>
        /// 資料處理1
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="itemBoms">商品組合資料物件清單</param>
        /// <returns></returns>
        public ApiResult ProcessApiDatas(string gupCode, string custCode, List<PostItemBomModel> itemBoms)
        {
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();
            int addCnt = 0;
            int updCnt = 0;
            F1903Repository f1903Repo = new F1903Repository(Schemas.CoreSchema);
            F910003Repository f910003Repo = new F910003Repository(Schemas.CoreSchema);
            F91000302Repository f91000302Repo = new F91000302Repository(Schemas.CoreSchema);
            F910101Repository f910101Repo = new F910101Repository(Schemas.CoreSchema, _wmsTransation);
            F910102Repository f910102Repo = new F910102Repository(Schemas.CoreSchema, _wmsTransation);
            TransApiBaseService tacService = new TransApiBaseService();

            #region Private Property
            var bomCodes = itemBoms.Where(x => !string.IsNullOrWhiteSpace(x.BomCode)).Select(x => x.BomCode).Distinct().ToList();
            var fgCodes = itemBoms.Where(x => !string.IsNullOrWhiteSpace(x.FgCode)).Select(x => x.FgCode).Distinct().ToList();
            var materialCodes = itemBoms.Where(x => x.BomDetails != null).SelectMany(x => x.BomDetails.Select(z => z.MaterialCode)).Distinct().ToList();
            var unitId = itemBoms.Where(x => !string.IsNullOrWhiteSpace(x.UnitId)).Select(x => x.UnitId).Distinct().ToList();

            // 取得商品代碼清單
            _itemCodeList = f1903Repo.GetDatasByItems(gupCode, custCode, fgCodes.Union(materialCodes).ToList()).Select(x => x.ITEM_CODE).ToList();

            // 取得已存在商品組合主檔清單
            var thirdPartF910101List = f910101Repo.GetF910101ByBomNoDatas(gupCode, custCode, bomCodes).ToList();

            // 取得已存在商品組合明細檔清單
            var thirdPartF910102List = f910102Repo.GetF910102ByBomNoDatas(gupCode, custCode, bomCodes).ToList();

            // 取得已存在的單位資料
            var itemTypeIds = f910003Repo.GetDatas("加工計價").Select(x => x.ITEM_TYPE_ID).ToList();
            _thirdPartF91000302List = f91000302Repo.GetDatasByItemTypeIds(itemTypeIds, unitId).ToList();

            // 取得單位名稱清單
            _unitIdList = _thirdPartF91000302List.Select(x => x.ACC_UNIT).ToList();

            // 將傳入資料Group取得重複商品編號的最後一筆以及重複幾筆
            List<PostItemBomGroupModel> itemBomDatas = new List<PostItemBomGroupModel>();
            if (itemBoms != null && itemBoms.Any())
            {
                itemBomDatas = itemBoms.GroupBy(x => x.BomCode).Select(x => new PostItemBomGroupModel
                {
                    Count = x.Count(),
                    LastData = x.Last()
                }).ToList();
            }
            #endregion

            #region 檢核
            List<F910101> exceptF910101Data = new List<F910101>();
            List<F910102> exceptF910102Data = new List<F910102>();

            itemBomDatas.ForEach(item =>
            {
                // 資料處理2
                var res1 = CheckItemBom(gupCode, custCode, item.LastData);

                if (!res1.IsSuccessed)
                {
                    data.AddRange((List<ApiResponse>)res1.Data);

                    #region 因驗證失敗要存入需被排除的F910101、F910102資料
                    // 需要被排除的F910101資料
                    var excludeF910101Data = thirdPartF910101List.Where(x => x.BOM_NO == item.LastData.BomCode).SingleOrDefault();
                    if (excludeF910101Data != null)
                        exceptF910101Data.Add(excludeF910101Data);
                    // 需要被排除的F910102資料
                    var excludeF910102Data = thirdPartF910102List.Where(x => x.BOM_NO == item.LastData.BomCode).ToList();
                    if (excludeF910102Data.Any())
                        exceptF910102Data.AddRange(excludeF910102Data);
                    #endregion
                }
            });

            // 排除資料
            if (exceptF910101Data.Any())
                thirdPartF910101List = thirdPartF910101List.Except(exceptF910101Data).ToList();
            if (exceptF910102Data.Any())
                thirdPartF910102List = thirdPartF910102List.Except(exceptF910102Data).ToList();
            #endregion

            #region 
            var thirdPartF910101Conditions = thirdPartF910101List.Select(z => z.BOM_NO).ToList();
            var thirdPartF910102Conditions = thirdPartF910102List.Select(z => new { z.BOM_NO, z.MATERIAL_CODE }).ToList();
            #endregion

            #region Delete F910102 Insert F910101 F910102

            // 新增F910101
            var addF910101Datas = _f910101List.Where(x => !thirdPartF910101Conditions.Contains(x.BOM_NO)).ToList();
            if (addF910101Datas.Any())
            {
                f910101Repo.BulkInsert(addF910101Datas);
                // 計算新增數
                addCnt += itemBomDatas.Where(x => addF910101Datas.Select(z => z.BOM_NO).Contains(x.LastData.BomCode)).Sum(x => x.Count);
            }

            // 刪除F910102
            if (thirdPartF910102List.Any())
                f910102Repo.SqlBulkDeleteForAnyCondition(thirdPartF910102List, "F910102", new List<string> { "BOM_NO", "MATERIAL_CODE", "CUST_CODE", "GUP_CODE" });

            // 新增F910102
            if (_f910102List.Any())
                f910102Repo.BulkInsert(_f910102List);

            #endregion

            #region BulkUpdateF910101 & UpdateCnt
            List<F910101> updF910101Datas = new List<F910101>();

            var updF910101 = thirdPartF910101List.Where(x => _f910101List.Select(z => z.BOM_NO).Contains(x.BOM_NO)).ToList();

            updF910101.ForEach(updData =>
            {
                var currData = _f910101List.Where(z => z.BOM_NO == updData.BOM_NO).SingleOrDefault();
                if (currData != null)
                {
                    // 修改
                    updData.ITEM_CODE = currData.ITEM_CODE;
                    updData.BOM_TYPE = currData.BOM_TYPE;
                    updData.BOM_NAME = currData.BOM_NAME;
                    updData.UNIT_ID = currData.UNIT_ID;
                    updData.CHECK_PERCENT = currData.CHECK_PERCENT;
                    updData.SPEC_DESC = currData.SPEC_DESC;
                    updData.PACKAGE_DESC = currData.PACKAGE_DESC;
                    updData.STATUS = currData.STATUS;
                    updData.ISPROCESS = currData.ISPROCESS;

                    updF910101Datas.Add(updData);
                }
            });

            if (updF910101Datas.Any())
            {
                f910101Repo.BulkUpdate(updF910101Datas);
                // 計算修改數
                updCnt += itemBomDatas.Where(x => updF910101Datas.Select(z => z.BOM_NO).Contains(x.LastData.BomCode)).Sum(x => x.Count);
            }
            #endregion

            #region Commit
            _wmsTransation.Complete();
            #endregion

            #region 回傳資料
            // 取得訊息內容 10003
            int total = itemBoms.Count;
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
        /// <param name="itemBom">商品組合資料物件</param>
        /// <returns></returns>
        private ApiResult CheckItemBom(string gupCode, string custCode, PostItemBomModel itemBom)
        {
            ApiResult result = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            // 預設值設定
            data.AddRange((List<ApiResponse>)CheckDefaultSetting(gupCode, custCode, itemBom).Data);

            // 共用欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(gupCode, custCode, itemBom).Data);

            // 貨主自訂欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckCustomColumnType(gupCode, custCode, itemBom).Data);

            // 如果以上檢核成功
            if (!data.Any())
            {
                // 共用欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCommonColumnData(gupCode, custCode, itemBom).Data);

                // 貨主自訂欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCustomColumnValue(gupCode, custCode, itemBom).Data);

                // 如果以上檢核成功
                if (!data.Any())
                {
                    CreateItemBomData(gupCode, custCode, itemBom);
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
        protected ApiResult CheckDefaultSetting(string gupCode, string custCode, PostItemBomModel itemBom)
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
        protected ApiResult CheckColumnNotNullAndMaxLength(string gupCode, string custCode, PostItemBomModel itemBom)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            #region 定義需檢核欄位、必填、型態、長度
            // 商品組合資料物件
            List<ApiCkeckColumnModel> itemBomCheckColumnList = new List<ApiCkeckColumnModel>
            {
                new ApiCkeckColumnModel{  Name = "BomCode",       Type = typeof(string),   MaxLength = 10,   Nullable = false },
                new ApiCkeckColumnModel{  Name = "BomName",       Type = typeof(string),   MaxLength = 100,  Nullable = false },
                new ApiCkeckColumnModel{  Name = "BomType",       Type = typeof(string),   MaxLength = 1,    Nullable = false },
                new ApiCkeckColumnModel{  Name = "ChkPercent",    Type = typeof(decimal),  MaxLength = 5,    Nullable = false },
                new ApiCkeckColumnModel{  Name = "IsProcess",     Type = typeof(string),   MaxLength = 1 ,   Nullable = false },
                new ApiCkeckColumnModel{  Name = "FgCode",        Type = typeof(string),   MaxLength = 20 ,  Nullable = false },
                new ApiCkeckColumnModel{  Name = "SpecDesc",      Type = typeof(string),   MaxLength = 200 },
                new ApiCkeckColumnModel{  Name = "PackDescr",     Type = typeof(string),   MaxLength = 500 },
                new ApiCkeckColumnModel{  Name = "UnitId",        Type = typeof(string),   MaxLength = 15 ,  Nullable = false },
                new ApiCkeckColumnModel{  Name = "Status",        Type = typeof(string),   MaxLength = 1 ,   Nullable = false }
            };

            // 商品組合明細資料物件
            List<ApiCkeckColumnModel> itemBomDetailCheckColumnList = new List<ApiCkeckColumnModel>
            {
                new ApiCkeckColumnModel{  Name = "CombinSeq",       Type = typeof(int),     MaxLength = 4,   Nullable = false },
                new ApiCkeckColumnModel{  Name = "MaterialCode",    Type = typeof(string),  MaxLength = 20,  Nullable = false },
                new ApiCkeckColumnModel{  Name = "Qty",             Type = typeof(int),     MaxLength = 9,   Nullable = false }
            };
            #endregion

            #region 檢查商品欄位必填、最大長度
            List<string> itemBomIsNullList = new List<string>();
            List<ApiCkeckColumnModel> itemBomIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

            // 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
            itemBomCheckColumnList.ForEach(column =>
            {
                var hasValue = DataCheckHelper.CheckRequireColumn(itemBom, column.Name);

                // 必填
                if (!column.Nullable && !hasValue)
                    itemBomIsNullList.Add(column.Name);

                // 最大長度
                if (column.MaxLength > 0 && hasValue)
                {
                    switch (column.Name)
                    {
                        case "ChkPercent":
                            // 檢核是否符合decimal(5,2)
                            if (!DataCheckHelper.CheckDataIsDecimal(itemBom, column.Name, 3, 2))
                                itemBomIsExceedMaxLenthList.Add(column);
                            break;
                        default:
                            if (!DataCheckHelper.CheckDataMaxLength(itemBom, column.Name, column.MaxLength))
                                itemBomIsExceedMaxLenthList.Add(column);
                            break;
                    }
                }
            });

            // 必填訊息
            if (itemBomIsNullList.Any())
            {
                data.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "20016", MsgContent = string.Format(tacService.GetMsg("20016"), itemBom.BomCode, string.Join("、", itemBomIsNullList)) });
            }

            // 最大長度訊息
            if (itemBomIsExceedMaxLenthList.Any())
            {
                List<string> errorMsgList = itemBomIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

                string errorMsg = string.Join("、", errorMsgList);

                data.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "20017", MsgContent = string.Format(tacService.GetMsg("20017"), itemBom.BomCode, errorMsg) });
            }

            #endregion

            #region 檢查訂單明細欄位必填、最大長度

            List<string> itemBomDetailIsNullList;
            List<ApiCkeckColumnModel> itemBomDetailIsExceedMaxLenthList;

            if (itemBom.BomDetails != null && itemBom.BomDetails.Any())
            {
                for (int i = 0; i < itemBom.BomDetails.Count; i++)
                {
                    var currDetail = itemBom.BomDetails[i];

                    itemBomDetailIsNullList = new List<string>();
                    itemBomDetailIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

                    itemBomDetailCheckColumnList.ForEach(o =>
                    {
                        // 必填
                        if (!o.Nullable)
                        {
                            if (!DataCheckHelper.CheckRequireColumn(currDetail, o.Name))
                                itemBomDetailIsNullList.Add(o.Name);
                        }

                        // 最大長度
                        if (o.MaxLength > 0)
                        {
                            if (!DataCheckHelper.CheckDataMaxLength(currDetail, o.Name, o.MaxLength))
                                itemBomDetailIsExceedMaxLenthList.Add(o);
                        }
                    });

                    // 必填訊息
                    if (itemBomDetailIsNullList.Any())
                    {
                        // 檢查進倉單明細必填欄位(參考2.8.2) ,如果檢核失敗，回傳 & 訊息內容[20058, < 參數4 >.CustInNo, 必填欄位未填寫的欄位清單(格式:[欄位名稱1]、[欄位名稱2])]
                        data.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "20058", MsgContent = string.Format(tacService.GetMsg("20058"), $"{itemBom.BomCode}第{i + 1}筆明細", string.Join("、", itemBomDetailIsNullList)) });
                    }

                    // 最大長度訊息
                    if (itemBomDetailIsExceedMaxLenthList.Any())
                    {
                        List<string> errorMsgList = itemBomDetailIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

                        string errorMsg = string.Join("、", errorMsgList);

                        // 檢查進倉單明細必填欄位(參考2.8.2) , 如果檢核失敗，回傳 & 訊息內容[20059, < 參數4 >.CustInNo, 欄位格式錯誤清單(格式:[欄位名稱1]格式錯誤必須為X1、[欄位名稱2]格式錯誤必須為X2)]
                        data.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "20059", MsgContent = string.Format(tacService.GetMsg("20059"), $"{itemBom.BomCode}第{i + 1}筆明細", errorMsg) });
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
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="item">商品資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnType(string gupCode, string custCode, PostItemBomModel itemBom)
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
        protected ApiResult CheckCommonColumnData(string gupCode, string custCode, PostItemBomModel itemBom)
        {
            CheckItemBomService cibervice = new CheckItemBomService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            // 檢核組合類別
            cibervice.CheckBomType(data, itemBom);

            // 檢核是否加工
            cibervice.CheckIsProcess(data, itemBom);

            // 檢核組合商品狀態
            cibervice.CheckStatus(data, itemBom);

            // 檢核商品單位編號
            cibervice.CheckUnitId(data, itemBom, _unitIdList);

            // 檢核商品編號是否存在
            cibervice.CheckFgCode(data, itemBom, _itemCodeList);

            // 檢核明細資料
            cibervice.CheckDetail(data, itemBom);

            // 檢核明細數量是否大於0
            cibervice.CheckDetailValueNotZero(data, itemBom);

            // 檢核組合順序
            cibervice.CheckCombinSeq(data, itemBom);

            // 檢核組合單品編號是否存在
            cibervice.CheckMaterialCode(data, itemBom, _itemCodeList);

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
        protected ApiResult CheckCustomColumnValue(string gupCode, string custCode, PostItemBomModel itemBom)
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
        public void CreateItemBomData(string gupCode, string custCode, PostItemBomModel itemBom)
        {
            // 建立商品組合主檔
            _f910101List.Add(CreateF910101(gupCode, custCode, itemBom));

            // 建立商品組合明細檔
            _f910102List.AddRange(CreateF910102(gupCode, custCode, itemBom));
        }

        /// <summary>
        /// 建立商品組合主檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="itemBom"></param>
        /// <returns></returns>
        protected F910101 CreateF910101(string gupCode, string custCode, PostItemBomModel itemBom)
        {
            var f91000302 = _thirdPartF91000302List.Where(x => x.ACC_UNIT == itemBom.UnitId).SingleOrDefault();

            return new F910101
            {
                BOM_NO = itemBom.BomCode,
                ITEM_CODE = itemBom.FgCode,
                BOM_TYPE = itemBom.BomType,
                BOM_NAME = itemBom.BomName,
                UNIT_ID = f91000302.ACC_UNIT,
                CHECK_PERCENT = Convert.ToSingle(itemBom.ChkPercent),
                SPEC_DESC = string.IsNullOrWhiteSpace(itemBom.SpecDesc) ? null : itemBom.SpecDesc,
                PACKAGE_DESC = string.IsNullOrWhiteSpace(itemBom.PackDescr) ? null : itemBom.PackDescr,
                STATUS = itemBom.Status,
                CUST_CODE = custCode,
                GUP_CODE = gupCode,
                ISPROCESS = itemBom.IsProcess
            };
        }

        /// <summary>
        /// 建立商品組合明細檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="itemBom"></param>
        /// <returns></returns>
        protected List<F910102> CreateF910102(string gupCode, string custCode, PostItemBomModel itemBom)
        {
            return itemBom.BomDetails.Select(x => new F910102
            {
                BOM_NO = itemBom.BomCode,
                MATERIAL_CODE = x.MaterialCode,
                COMBIN_ORDER = Convert.ToInt16(x.CombinSeq),
                BOM_QTY = x.Qty,
                CUST_CODE = custCode,
                GUP_CODE = gupCode
            }).ToList();
        }
        #endregion
    }
}
