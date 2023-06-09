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
    public class CommonItemLevelService
    {
        private WmsTransaction _wmsTransation;
        public CommonItemLevelService(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;
        }

        #region Private Property
        /// <summary>
        /// 模組名稱
        /// </summary>
        private readonly string _moduleName = "商品階層檔";

        /// <summary>
        /// 商品清單
        /// </summary>
        private List<string> _itemCodeList;

        /// <summary>
        /// 商品單位清單
        /// </summary>
        private List<string> _unitIdList;

        /// <summary>
        /// 商品階層清單
        /// </summary>
        private List<F190301> _f190301List = new List<F190301>();
        #endregion

        #region Main Method
        /// <summary>
        /// Func1
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult RecevieApiDatas(PostItemLevelReq req)
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
            if (req.Result.ItemLevels == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

            // 檢核資料筆數
            int reqTotal = req.Result.Total != null ? Convert.ToInt32(req.Result.Total) : 0;
            if (req.Result.ItemLevels == null || (req.Result.ItemLevels != null && !tacService.CheckDataCount(reqTotal, req.Result.ItemLevels.Count)))
                return new ApiResult { IsSuccessed = false, MsgCode = "20054", MsgContent = string.Format(tacService.GetMsg("20054"), reqTotal, req.Result.ItemLevels.Count) };

            // 檢核商品階層主檔筆數是否超過[商品階層檔最大筆數]
            int itemLevelMaxCnt = Convert.ToInt32(commonService.GetSysGlobalValue("ItemLevelMaxCnt"));
            if (req.Result.ItemLevels.Count > itemLevelMaxCnt)
                return new ApiResult { IsSuccessed = false, MsgCode = "20018", MsgContent = string.Format(tacService.GetMsg("20018"), _moduleName, req.Result.ItemLevels.Count, itemLevelMaxCnt) };
            #endregion

            // 取得業主編號
            string gupCode = commonService.GetGupCode(req.CustCode);

            // 資料處理1
            return ProcessApiDatas(gupCode, req.CustCode, req.Result.ItemLevels);
        }

        /// <summary>
        /// 資料處理1
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="itemLevels">商品階層資料物件清單</param>
        /// <returns></returns>
        public ApiResult ProcessApiDatas(string gupCode, string custCode, List<PostItemLevelLevelsModel> itemLevels)
        {
            var res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();
            int addCnt = 0;
            int updCnt = 0;
            F190301Repository f190301Repo = new F190301Repository(Schemas.CoreSchema, _wmsTransation);
            F91000302Repository f91000302Repo = new F91000302Repository(Schemas.CoreSchema);
            TransApiBaseService tacService = new TransApiBaseService();
            CommonService commonService = new CommonService();

            #region Private Property

            var itemCodes = itemLevels.Select(x => x.ItemCode).Distinct().ToList();
            var unitIds = itemLevels.Select(x => x.UnitId.PadLeft(2, '0')).Distinct().ToList();

            // 取得商品清單
            _itemCodeList = commonService.GetProductList(gupCode, custCode, itemCodes).Select(x => x.ITEM_CODE).ToList();

            // 取得商品單位清單
            _unitIdList = f91000302Repo.GetDatas("001", unitIds).Select(x => x.ACC_UNIT).ToList();

            // 取得已存在商品階層檔清單
            var thirdPartF190301List = f190301Repo.GetDatas(gupCode, custCode, itemCodes, unitIds).ToList();

            // 將傳入資料Group取得重複商品編號&商品單位編號的最後一筆以及重複幾筆
            List<PostItemLevelsGroupModel> itemLevelDatas = new List<PostItemLevelsGroupModel>();
            if (itemLevels != null && itemLevels.Any())
            {
                itemLevelDatas = itemLevels.GroupBy(x => new { x.ItemCode, UnitId = x.UnitId.PadLeft(2, '0') }).Select(x => new PostItemLevelsGroupModel
                {
                    Count = x.Count(),
                    LastData = x.Last()
                }).ToList();
            }
            #endregion

            #region 檢查同商品單位編號是否有重複階層 or 新增/修改 F190301

            var repeatItemCodes = itemLevelDatas.GroupBy(x => new { x.LastData.ItemCode, x.LastData.ItemLevel })
                                                .Where(x => x.Count() > 1)
                                                .Select(x => new { x.Key.ItemCode, x.Key.ItemLevel })
                                                .ToList();

            var exclude = itemLevelDatas.Where(x => repeatItemCodes.Any(z => z.ItemCode == x.LastData.ItemCode && z.ItemLevel == x.LastData.ItemLevel));

            itemLevelDatas = itemLevelDatas.Except(exclude).ToList();

            repeatItemCodes.GroupBy(x => x.ItemCode)
                           .Select(x => new { ItemCode = x.Key, ItemLevels = string.Join("、", x.Select(z => z.ItemLevel.ToString()).ToList()) })
                           .ToList()
                           .ForEach(item =>
                           {
                               data.Add(new ApiResponse
                               {
                                   No = item.ItemCode,
                                   MsgCode = "20772",
                                   MsgContent = string.Format(tacService.GetMsg("20772"), item.ItemCode, item.ItemLevels)
                               });
                           });
            #endregion

            #region Foreach [商品階層資料物件] 檢核
            List<F190301> exceptData = new List<F190301>();

            itemLevelDatas.ForEach(item =>
            {
                // 資料處理2
                var res1 = CheckItemLevel(gupCode, custCode, item.LastData);

                if (!res1.IsSuccessed)
                {
                    data.AddRange((List<ApiResponse>)res1.Data);

                    var excludeData = thirdPartF190301List.Where(x => x.ITEM_CODE == item.LastData.ItemCode &&
                                                                      x.UNIT_ID == item.LastData.UnitId.PadLeft(2, '0')).SingleOrDefault();

                    if (excludeData != null)
                        exceptData.Add(excludeData);
                }
                else
                {
                    _f190301List.Add(CreateF190301(gupCode, custCode, item.LastData));
                }
            });

            if (exceptData.Any())
                thirdPartF190301List = thirdPartF190301List.Except(exceptData).ToList();
            #endregion

            #region Insert
            var thirdPartTmp = thirdPartF190301List.Select(z => new { z.ITEM_CODE, z.UNIT_ID });

            var addDatas = _f190301List.Where(x => !thirdPartTmp.Any(z => z.ITEM_CODE == x.ITEM_CODE &&
                                                                          z.UNIT_ID == x.UNIT_ID)).ToList();

            if (addDatas.Any())
            {
                f190301Repo.BulkInsert(addDatas);

                // 計算新增數
                addCnt += itemLevelDatas.Where(x => addDatas.Any(z => z.ITEM_CODE == x.LastData.ItemCode &&
                                                                      z.UNIT_ID == x.LastData.UnitId.PadLeft(2, '0'))).Sum(x => x.Count);
            }
            #endregion

            #region Update
            List<F190301> updDatas = new List<F190301>();

            var f190301Tmp = _f190301List.Select(x => new { x.ITEM_CODE, x.UNIT_ID }).ToList();

            var updF190301 = thirdPartF190301List.Where(x => f190301Tmp.Any(z => z.ITEM_CODE == x.ITEM_CODE &&
                                                             z.UNIT_ID == x.UNIT_ID)).ToList();

            updF190301.ForEach(updData =>
            {
                var currData = _f190301List.Where(z => z.ITEM_CODE == updData.ITEM_CODE &&
                                                       z.UNIT_ID == updData.UNIT_ID).SingleOrDefault();
                if (currData != null)
                {
                    updData.UNIT_LEVEL = Convert.ToInt16(currData.UNIT_LEVEL);
                    updData.UNIT_QTY = Convert.ToInt32(currData.UNIT_QTY);
                    updData.LENGTH = currData.LENGTH;
                    updData.WIDTH = currData.WIDTH;
                    updData.HIGHT = currData.HIGHT;
                    updData.WEIGHT = currData.WEIGHT;
                    updData.SYS_UNIT = string.IsNullOrWhiteSpace(currData.SYS_UNIT) ? null : currData.SYS_UNIT;

                    updDatas.Add(updData);
                }
            });

            if (updDatas.Any())
            {
                f190301Repo.BulkUpdate(updDatas);

                // 計算修改數
                updCnt += itemLevelDatas.Where(x => updDatas.Any(z => z.ITEM_CODE == x.LastData.ItemCode &&
                                                                      z.UNIT_ID == x.LastData.UnitId.PadLeft(2, '0'))).Sum(x => x.Count);
            }
            #endregion

            #region Commit
            if (addDatas.Any() || updDatas.Any())
                _wmsTransation.Complete();
            #endregion

            #region 回傳資料
            // 取得訊息內容 10003
            int total = itemLevels.Count;
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
        /// <param name="itemLevel">商品階層資料物件</param>
        /// <returns></returns>
        private ApiResult CheckItemLevel(string gupCode, string custCode, PostItemLevelLevelsModel itemLevel)
        {
            ApiResult result = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            // 預設值設定
            data.AddRange((List<ApiResponse>)CheckDefaultSetting(gupCode, custCode, itemLevel).Data);

            // 共用欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(gupCode, custCode, itemLevel).Data);

            // 貨主自訂欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckCustomColumnType(gupCode, custCode, itemLevel).Data);

            // 如果以上檢核成功
            if (!data.Any())
            {
                // 共用欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCommonColumnData(gupCode, custCode, itemLevel).Data);

                // 貨主自訂欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCustomColumnValue(gupCode, custCode, itemLevel).Data);
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
        protected ApiResult CheckDefaultSetting(string gupCode, string custCode, PostItemLevelLevelsModel itemLevel)
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
        /// <param name="itemLevel">商品階層資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckColumnNotNullAndMaxLength(string gupCode, string custCode, PostItemLevelLevelsModel itemLevel)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            #region 定義需檢核欄位、必填、型態、長度

            // 商品階層資料物件
            List<ApiCkeckColumnModel> itemLevelCheckColumnList = new List<ApiCkeckColumnModel>
            {
                new ApiCkeckColumnModel{  Name = "ItemCode",    Type = typeof(string),   MaxLength = 20,   Nullable = false },
                new ApiCkeckColumnModel{  Name = "ItemLevel",   Type = typeof(int),      MaxLength = 2,    Nullable = false },
                new ApiCkeckColumnModel{  Name = "UnitId",      Type = typeof(string),   MaxLength = 2,    Nullable = false },
                new ApiCkeckColumnModel{  Name = "UnitQty",     Type = typeof(int),      MaxLength = 6,    Nullable = false },
                new ApiCkeckColumnModel{  Name = "Length",      Type = typeof(decimal),  MaxLength = 8 ,   Nullable = false },
                new ApiCkeckColumnModel{  Name = "Width",       Type = typeof(decimal),  MaxLength = 8 ,   Nullable = false },
                new ApiCkeckColumnModel{  Name = "Height",       Type = typeof(decimal),  MaxLength = 8 ,   Nullable = false },
                new ApiCkeckColumnModel{  Name = "Weight",      Type = typeof(decimal),  MaxLength = 10 ,  Nullable = false },
                new ApiCkeckColumnModel{  Name = "SysUnit",     Type = typeof(string),   MaxLength = 2 }
            };
            #endregion

            #region 檢查商品階層欄位必填、最大長度
            List<string> itemLevelIsNullList = new List<string>();
            List<ApiCkeckColumnModel> itemLevelIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

            // 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
            itemLevelCheckColumnList.ForEach(column =>
            {
                var hasValue = DataCheckHelper.CheckRequireColumn(itemLevel, column.Name);

                // 必填
                if (!column.Nullable && !hasValue)
                    itemLevelIsNullList.Add(column.Name);

                // 最大長度
                if (column.MaxLength > 0 && hasValue)
                {
                    bool checkResult = true;
                    switch (column.Name)
                    {
                        case "Length":
                        case "Width":
                        case "Height":
                            // 檢核是否符合decimal(8,2)
                            checkResult = DataCheckHelper.CheckDataIsDecimal(itemLevel, column.Name, 6, 2);
                            break;
                        case "Weight":
                            // 檢核是否符合decimal(10,2)
                            checkResult = DataCheckHelper.CheckDataIsDecimal(itemLevel, column.Name, 8, 2);
                            break;
                        default:
                            checkResult = DataCheckHelper.CheckDataMaxLength(itemLevel, column.Name, column.MaxLength);
                            break;
                    }

                    if (!checkResult)
                        itemLevelIsExceedMaxLenthList.Add(column);
                }
            });

            // 必填訊息
            if (itemLevelIsNullList.Any())
            {
                data.Add(new ApiResponse { No = itemLevel.ItemCode, MsgCode = "20016", MsgContent = string.Format(tacService.GetMsg("20016"), itemLevel.ItemCode, string.Join("、", itemLevelIsNullList)) });
            }

            // 最大長度訊息
            if (itemLevelIsExceedMaxLenthList.Any())
            {
                List<string> errorMsgList = itemLevelIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

                string errorMsg = string.Join("、", errorMsgList);

                data.Add(new ApiResponse { No = itemLevel.ItemCode, MsgCode = "20017", MsgContent = string.Format(tacService.GetMsg("20017"), itemLevel.ItemCode, errorMsg) });
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
        /// <param name="itemLevel">商品階層資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnType(string gupCode, string custCode, PostItemLevelLevelsModel itemLevel)
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
        /// <param name="itemLevel">商品階層資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCommonColumnData(string gupCode, string custCode, PostItemLevelLevelsModel itemLevel)
        {
            CheckItemLevelService cilService = new CheckItemLevelService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            // 檢核商品階層、數量、長度、寬度、高度、重量
            cilService.CheckValueNotZero(data, itemLevel);

            // 檢核商品編號
            cilService.CheckItemCode(data, itemLevel, _itemCodeList);

            // 檢核商品單位編號
            cilService.CheckUnitId(data, itemLevel, _unitIdList);

            // 檢核系統單位
            cilService.CheckSysUnit(data, itemLevel);

            res.Data = data;
            return res;
        }

        /// <summary>
        /// 貨主自訂欄位資料檢核
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="itemLevel">商品階層資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnValue(string gupCode, string custCode, PostItemLevelLevelsModel itemLevel)
        {
            // 請預留方法
            ApiResult res = new ApiResult();
            res.Data = new List<ApiResponse>();
            return res;
        }
        #endregion

        #region Protected 建立商品階層主檔資料
        /// <summary>
        /// 建立商品階層主檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="returns"></param>
        /// <returns></returns>
        protected F190301 CreateF190301(string gupCode, string custCode, PostItemLevelLevelsModel itemLevel)
        {
            return new F190301
            {
                ITEM_CODE = itemLevel.ItemCode,
                UNIT_LEVEL = Convert.ToInt16(itemLevel.ItemLevel),
                UNIT_ID = itemLevel.UnitId.PadLeft(2, '0'),
                UNIT_QTY = Convert.ToInt32(itemLevel.UnitQty),
                LENGTH = itemLevel.Length,
                WIDTH = itemLevel.Width,
                HIGHT = itemLevel.Height,
                WEIGHT = itemLevel.Weight,
                GUP_CODE = gupCode,
                CUST_CODE = custCode,
                SYS_UNIT = string.IsNullOrWhiteSpace(itemLevel.SysUnit) ? null : itemLevel.SysUnit
            };
        }
        #endregion
    }
}
