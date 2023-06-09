using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
    public class CommonItemCategoryService
    {
        private WmsTransaction _wmsTransation;
        public CommonItemCategoryService(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;
        }

        #region Private Property

        /// <summary>
        /// 商品大分類清單
        /// </summary>
        private List<F1915> _f1915List = new List<F1915>();

        /// <summary>
        /// 程式下拉選單參數設定對應檔清單
        /// </summary>
        private List<F000904> _f000904List = new List<F000904>();

        /// <summary>
        /// 程式下拉選單參數設定語系對應檔清單
        /// </summary>
        private List<F000904_I18N> _f000904_I18NList = new List<F000904_I18N>();

        /// <summary>
        /// 商品中分類清單
        /// </summary>
        private List<F1916> _f1916List = new List<F1916>();

        /// <summary>
        /// 商品小分類清單
        /// </summary>
        private List<F1917> _f1917List = new List<F1917>();

        /// <summary>
        /// 已存在商品大分類清單
        /// </summary>
        private List<F1915> _thirdPartF1915List;

        /// <summary>
        /// 程式下拉選單參數設定對應檔清單
        /// </summary>
        private List<F000904> _thirdPartF000904List = new List<F000904>();

        /// <summary>
        /// 程式下拉選單參數設定語系對應檔清單
        /// </summary>
        private List<F000904_I18N> _thirdPartF000904_I18NList = new List<F000904_I18N>();

        /// <summary>
        /// 已存在商品中分類清單
        /// </summary>
        private List<F1916> _thirdPartF1916List;

        /// <summary>
        /// 已存在商品小分類清單
        /// </summary>
        private List<F1917> _thirdPartF1917List;

        /// <summary>
        /// 商品大分類代碼清單
        /// </summary>
        private List<string> _lCategoryList;

        /// <summary>
        /// 商品中分類代碼清單
        /// </summary>
        private List<MCategorysModel> _mCategoryList;

        /// <summary>
        /// 商品小分類代碼清單
        /// </summary>
        private List<SCategorysModel> _sCategoryList;

        private List<string> _langs = new List<string>();
        #endregion

        #region Main Method
        /// <summary>
        /// Func1
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult RecevieApiDatas(PostItemCategoryReq req)
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

            // 大中小物件清單Null，將轉為空List
            if (req.Result.LCategorys == null)
                req.Result.LCategorys = new List<PostItemCategoryLCategorysModel>();

            if (req.Result.MCategorys == null)
                req.Result.MCategorys = new List<PostItemCategoryMCategorysModel>();

            if (req.Result.SCategorys == null)
                req.Result.SCategorys = new List<PostItemCategorySCategorysModel>();

            // 檢核大、中、小分類資料筆數是否有對應到各個Total
            string totalMsg = tacService.GetMsg("20054");
            List<string> totalMsgContents = new List<string>();

            // 大分類
            int lTotal = req.Result.LTotal != null ? Convert.ToInt32(req.Result.LTotal) : 0;
            if (req.Result.LCategorys == null || (req.Result.LCategorys != null && !tacService.CheckDataCount(lTotal, req.Result.LCategorys.Count)))
                totalMsgContents.Add($"大分類{ string.Format(totalMsg, lTotal, req.Result.LCategorys.Count) }");

            // 中分類
            int mTotal = req.Result.MTotal != null ? Convert.ToInt32(req.Result.MTotal) : 0;
            if (req.Result.MCategorys == null || (req.Result.MCategorys != null && !tacService.CheckDataCount(mTotal, req.Result.MCategorys.Count)))
                totalMsgContents.Add($"中分類{ string.Format(totalMsg, mTotal, req.Result.MCategorys.Count) }");

            // 小分類
            int sTotal = req.Result.STotal != null ? Convert.ToInt32(req.Result.STotal) : 0;
            if (req.Result.SCategorys == null || (req.Result.SCategorys != null && !tacService.CheckDataCount(sTotal, req.Result.SCategorys.Count)))
                totalMsgContents.Add($"小分類{ string.Format(totalMsg, sTotal, req.Result.SCategorys.Count) }");

            if (totalMsgContents.Any())
                return new ApiResult { IsSuccessed = false, MsgCode = "20054", MsgContent = string.Join("；", totalMsgContents) };

            // 檢核商品大中小分類主檔筆數是否超過[商品分類主檔最大筆數]
            string msg = tacService.GetMsg("20018");
            List<string> msgContents = new List<string>();

            // 取得商品大分類主檔最大筆數
            int lCategoryMaxCnt = Convert.ToInt32(commonService.GetSysGlobalValue("LCategoryMaxCnt"));
            if (req.Result.LCategorys.Count > lCategoryMaxCnt)
                msgContents.Add(string.Format(msg, "商品大分類主檔", req.Result.LCategorys.Count, lCategoryMaxCnt));

            // 取得商品中分類主檔最大筆數
            int mCategoryMaxCnt = Convert.ToInt32(commonService.GetSysGlobalValue("MCategoryMaxCnt"));
            if (req.Result.MCategorys.Count > mCategoryMaxCnt)
                msgContents.Add(string.Format(msg, "商品中分類主檔", req.Result.MCategorys.Count, mCategoryMaxCnt));

            // 取得商品小分類主檔最大筆數
            int sCategoryMaxCnt = Convert.ToInt32(commonService.GetSysGlobalValue("SCategoryMaxCnt"));
            if (req.Result.SCategorys.Count > sCategoryMaxCnt)
                msgContents.Add(string.Format(msg, "商品小分類主檔", req.Result.SCategorys.Count, sCategoryMaxCnt));

            if (msgContents.Any())
                return new ApiResult { IsSuccessed = false, MsgCode = "20018", MsgContent = string.Join("；", msgContents) };
            #endregion

            // 取得業主編號
            string gupCode = commonService.GetGupCode(req.CustCode);

            // 資料處理1
            return ProcessApiDatas(gupCode, req.CustCode, req.Result.LCategorys, req.Result.MCategorys, req.Result.SCategorys);
        }

        /// <summary>
        /// 資料處理1
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="lCategorys">商品大分類資料物件清單</param>
        /// <param name="mCategorys">商品中分類資料物件清單</param>
        /// <param name="sCategorys">商品小分類資料物件清單</param>
        /// <returns></returns>
        public ApiResult ProcessApiDatas(string gupCode, string custCode,
            List<PostItemCategoryLCategorysModel> lCategorys,
            List<PostItemCategoryMCategorysModel> mCategorys,
            List<PostItemCategorySCategorysModel> sCategorys)
        {
            #region 變數
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();
            int addF1915Cnt = 0;
            int updF1915Cnt = 0;
            int addF1916Cnt = 0;
            int updF1916Cnt = 0;
            int addF1917Cnt = 0;
            int updF1917Cnt = 0;
            F1915Repository f1915Repo = new F1915Repository(Schemas.CoreSchema, _wmsTransation);
            F1916Repository f1916Repo = new F1916Repository(Schemas.CoreSchema, _wmsTransation);
            F1917Repository f1917Repo = new F1917Repository(Schemas.CoreSchema, _wmsTransation);
            F000904Repository f000904Repo = new F000904Repository(Schemas.CoreSchema, _wmsTransation);
            F000904_I18NRepository f000904_I18NRepo = new F000904_I18NRepository(Schemas.CoreSchema, _wmsTransation);
            TransApiBaseService tacService = new TransApiBaseService();
            #endregion

            #region Private Property
            _lCategoryList = lCategorys.Where(x => !string.IsNullOrWhiteSpace(x.LCode)).Select(x => x.LCode).Distinct().ToList();
            _mCategoryList = mCategorys.Where(x => !string.IsNullOrWhiteSpace(x.LCode) && !string.IsNullOrWhiteSpace(x.MCode)).Select(a => AutoMapper.Mapper.DynamicMap<MCategorysModel>(a)).ToList();
            _sCategoryList = sCategorys.Where(x => !string.IsNullOrWhiteSpace(x.LCode) && !string.IsNullOrWhiteSpace(x.MCode) && !string.IsNullOrWhiteSpace(x.SCode)).Select(a => AutoMapper.Mapper.DynamicMap<SCategorysModel>(a)).ToList();

            // 取得已存在大分類資料
            _thirdPartF1915List = f1915Repo.GetDatasByACode(gupCode, custCode, _lCategoryList).ToList();

            // 取得已存在中分類資料
            _thirdPartF1916List = f1916Repo.GetDatasByMCategory(gupCode, custCode, mCategorys).ToList();

            // 取得已存在小分類資料
            _thirdPartF1917List = f1917Repo.GetDatasBySCategory(gupCode, custCode, sCategorys).ToList();

            // 取得已存在的程式下拉選單參數設定對應檔資料
            _thirdPartF000904List = f000904Repo.GetDatasBySubTopic("F1903", "TYPE", _lCategoryList).ToList();

            // 取得程式下拉選單參數設定對應檔清單
            var f000904_I18NData = f000904_I18NRepo.GetDatas("F1903", "TYPE");
            _thirdPartF000904_I18NList = f000904_I18NData.Where(x => _lCategoryList.Contains(x.VALUE)).ToList();

            // 語系
            _langs = f000904_I18NData.Select(x => x.LANG.Trim()).Distinct().ToList();

            // 將傳入資料Group取得重複大分類代碼的最後一筆以及重複幾筆
            List<PostItemCategoryLCategorysGroupModel> lCategorysDatas = new List<PostItemCategoryLCategorysGroupModel>();
            if (lCategorys.Any())
            {
                lCategorysDatas = lCategorys.GroupBy(x => x.LCode).Select(x => new PostItemCategoryLCategorysGroupModel
                {
                    Count = x.Count(),
                    LastData = x.Last()
                }).ToList();
            }

            // 將傳入資料Group取得重複大中分類代碼的最後一筆以及重複幾筆
            List<PostItemCategoryMCategorysGroupModel> mCategorysDatas = new List<PostItemCategoryMCategorysGroupModel>();
            if (mCategorys.Any())
            {
                mCategorysDatas = mCategorys.GroupBy(x => new { x.LCode, x.MCode }).Select(x => new PostItemCategoryMCategorysGroupModel
                {
                    Count = x.Count(),
                    LastData = x.Last()
                }).ToList();
            }

            // 將傳入資料Group取得重複大中小分類代碼的最後一筆以及重複幾筆
            List<PostItemCategorySCategorysGroupModel> sCategorysDatas = new List<PostItemCategorySCategorysGroupModel>();
            if (sCategorys.Any())
            {
                sCategorysDatas = sCategorys.GroupBy(x => new { x.LCode, x.MCode, x.SCode }).Select(x => new PostItemCategorySCategorysGroupModel
                {
                    Count = x.Count(),
                    LastData = x.Last()
                }).ToList();
            }
            #endregion

            #region 檢核 & 產生新增/修改資料 (大分類)
            List<F1915> exceptF1915Data = new List<F1915>();
            List<F000904> exceptF000904Data = new List<F000904>();
            List<F000904_I18N> exceptF000904_I18NData = new List<F000904_I18N>();

            lCategorysDatas.ForEach(item =>
            {
                // 資料處理2
                var res1 = CheckLCategorys(gupCode, custCode, item.LastData);

                if (!res1.IsSuccessed)
                {
                    data.AddRange((List<ApiResponse>)res1.Data);

                    // 需要被排除的F1915資料
                    var excludeF1915Data = _thirdPartF1915List.Where(x => x.ACODE == item.LastData.LCode).SingleOrDefault();
                    if (excludeF1915Data != null)
                        exceptF1915Data.Add(excludeF1915Data);
                    // 需要被排除的F000904資料
                    var excludeF000904Data = _thirdPartF000904List.Where(x => x.VALUE == item.LastData.LCode).SingleOrDefault();
                    if (excludeF000904Data != null)
                        exceptF000904Data.Add(excludeF000904Data);
                    // 需要被排除的F000904資料
                    var excludeF000904_I18NData = _thirdPartF000904_I18NList.Where(x => x.VALUE == item.LastData.LCode).ToList();
                    if (excludeF000904_I18NData != null)
                        exceptF000904_I18NData.AddRange(excludeF000904_I18NData);
                }
            });

            // 排除資料
            if (exceptF1915Data.Any())
                _thirdPartF1915List = _thirdPartF1915List.Except(exceptF1915Data).ToList();
            if (exceptF000904Data.Any())
                _thirdPartF000904List = _thirdPartF000904List.Except(exceptF000904Data).ToList();
            if (exceptF000904_I18NData.Any())
                _thirdPartF000904_I18NList = _thirdPartF000904_I18NList.Except(exceptF000904_I18NData).ToList();
            #endregion

            #region 檢核 & 產生新增/修改資料 (中分類)
            List<F1916> exceptF1916Data = new List<F1916>();

            mCategorysDatas.ForEach(item =>
            {
                // 資料處理2
                var res1 = CheckMCategorys(gupCode, custCode, item.LastData);

                if (!res1.IsSuccessed)
                {
                    data.AddRange((List<ApiResponse>)res1.Data);

                    // 需要被排除的F1916資料
                    var excludeF1916Data = _thirdPartF1916List.Where(x => x.ACODE == item.LastData.LCode && x.BCODE == item.LastData.MCode).SingleOrDefault();
                    if (excludeF1916Data != null)
                        exceptF1916Data.Add(excludeF1916Data);
                }
            });

            // 排除資料
            if (exceptF1916Data.Any())
                _thirdPartF1916List = _thirdPartF1916List.Except(exceptF1916Data).ToList();
            #endregion

            #region 檢核 & 產生新增/修改資料 (小分類)
            List<F1917> exceptF1917Data = new List<F1917>();

            sCategorysDatas.ForEach(item =>
            {
                // 資料處理2
                var res1 = CheckSCategorys(gupCode, custCode, item.LastData);

                if (!res1.IsSuccessed)
                {
                    data.AddRange((List<ApiResponse>)res1.Data);

                    // 需要被排除的F1917資料
                    var excludeF1917Data = _thirdPartF1917List.Where(x => x.ACODE == item.LastData.LCode && x.BCODE == item.LastData.MCode && x.CCODE == item.LastData.SCode).SingleOrDefault();
                    if (excludeF1917Data != null)
                        exceptF1917Data.Add(excludeF1917Data);
                }
            });

            // 排除資料
            if (exceptF1917Data.Any())
                _thirdPartF1917List = _thirdPartF1917List.Except(exceptF1917Data).ToList();
            #endregion

            #region 各個已存在資料過濾Key欄位
            var thirdPartF1915Conditions = _thirdPartF1915List.Select(z => z.ACODE).ToList();
            var thirdPartF000904Conditions = _thirdPartF000904List.Select(z => z.VALUE).ToList();
            var thirdPartF000904_I81NConditions = _thirdPartF000904_I18NList.Select(z => new { z.VALUE, z.LANG }).ToList();
            var thirdPartF1916Conditions = _thirdPartF1916List.Select(z => new { z.ACODE, z.BCODE }).ToList();
            var thirdPartF1917Conditions = _thirdPartF1917List.Select(z => new { z.ACODE, z.BCODE, z.CCODE }).ToList();
            #endregion

            #region Insert 大分類(F1915、F000904、F000904_I18N)
            // 新增F1915
            var addF1915Datas = _f1915List.Where(x => !thirdPartF1915Conditions.Contains(x.ACODE)).ToList();
            if (addF1915Datas.Any())
            {
                f1915Repo.BulkInsert(addF1915Datas);
                // 計算新增數
                addF1915Cnt += lCategorysDatas.Where(x => addF1915Datas.Select(z => z.ACODE).Contains(x.LastData.LCode)).Sum(x => x.Count);

                // 新增F000904
                var addF000904Datas = _f000904List.Where(x => !thirdPartF000904Conditions.Contains(x.VALUE)).ToList();
                if (addF000904Datas.Any())
                    f000904Repo.BulkInsert(addF000904Datas);

                // 新增F000904_I18N
                var addF1905Datas = _f000904_I18NList.Where(x => !thirdPartF000904_I81NConditions.Any(z => x.VALUE == z.VALUE &&
                                                                                                           x.LANG == z.LANG)).ToList();
                if (addF1905Datas.Any())
                    f000904_I18NRepo.BulkInsert(addF1905Datas);
            }
            #endregion

            #region Insert 大分類(F1916)
            // 新增F1916
            var addF1916Datas = _f1916List.Where(x => !thirdPartF1916Conditions.Any(z => x.ACODE == z.ACODE &&
                                                                                         x.BCODE == z.BCODE)).ToList();
            if (addF1916Datas.Any())
            {
                f1916Repo.BulkInsert(addF1916Datas);
                // 計算新增數
                addF1916Cnt += mCategorysDatas.Where(x => addF1916Datas.Any(z => x.LastData.LCode == z.ACODE &&
                                                                                 x.LastData.MCode == z.BCODE)).Sum(x => x.Count);
            }
            #endregion

            #region Insert 大分類(F1917)
            // 新增F1917
            var addF1917Datas = _f1917List.Where(x => !thirdPartF1917Conditions.Any(z => x.ACODE == z.ACODE &&
                                                                                         x.BCODE == z.BCODE &&
                                                                                         x.CCODE == z.CCODE)).ToList();
            if (addF1917Datas.Any())
            {
                f1917Repo.BulkInsert(addF1917Datas);
                // 計算新增數
                addF1917Cnt += sCategorysDatas.Where(x => addF1917Datas.Any(z => x.LastData.LCode == z.ACODE &&
                                                                                 x.LastData.MCode == z.BCODE &&
                                                                                 x.LastData.SCode == z.CCODE)).Sum(x => x.Count);
            }
            #endregion

            #region Update 大分類(F1915、F000904、F000904_I18N)

            #region BulkUpdateF1915 & UpdateCnt
            List<F1915> updF1915Datas = new List<F1915>();

            var updF1915 = _thirdPartF1915List.Where(x => _f1915List.Select(z => z.ACODE).Contains(x.ACODE)).ToList();

            updF1915.ForEach(updData =>
            {
                var currData = _f1915List.Where(z => z.ACODE == updData.ACODE).SingleOrDefault();
                if (currData != null)
                {
                    if (currData.CLA_NAME != updData.CLA_NAME ||
                        currData.CHECK_PERCENT != updData.CHECK_PERCENT)
                    {
                        // 修改
                        updData.CLA_NAME = currData.CLA_NAME;
                        updData.CHECK_PERCENT = currData.CHECK_PERCENT;
                        updF1915Datas.Add(updData);
                    }

                    // 計算修改數
                    updF1915Cnt += lCategorysDatas.Where(x => x.LastData.LCode == currData.ACODE).Sum(x => x.Count);
                }
            });

            if (updF1915Datas.Any())
                f1915Repo.BulkUpdate(updF1915Datas);
            #endregion

            #region BulkUpdateF000904
            List<F000904> updF000904Datas = new List<F000904>();

            var updF000904 = _thirdPartF000904List.Where(x => _f000904List.Select(z => z.VALUE).Contains(x.VALUE)).ToList();

            updF000904.ForEach(updData =>
            {
                var currData = _f000904List.Where(z => z.VALUE == updData.VALUE).SingleOrDefault();
                if (currData != null)
                {
                    if (updData.NAME != currData.NAME)
                    {
                        // 修改
                        updData.NAME = currData.NAME;
                        updF000904Datas.Add(updData);
                    }
                }
            });

            if (updF000904Datas.Any())
                f000904Repo.BulkUpdate(updF000904Datas);

            #endregion

            #region BulkUpdateF000904_I18N
            List<F000904_I18N> updF000904_I18NDatas = new List<F000904_I18N>();

            var updF000904_I18N = _thirdPartF000904_I18NList.Where(x => _f000904_I18NList.Any(z => x.VALUE == z.VALUE &&
                                                                                                   x.LANG == z.LANG)).ToList();

            updF000904_I18N.ForEach(updData =>
            {
                var currData = _f000904_I18NList.Where(x => x.VALUE == updData.VALUE &&
                                                            x.LANG == updData.LANG).SingleOrDefault();
                if (currData != null)
                {
                    if (updData.NAME != currData.NAME)
                    {
                        // 修改
                        updData.NAME = currData.NAME;
                        updF000904_I18NDatas.Add(updData);
                    }
                }
            });

            if (updF000904_I18NDatas.Any())
                f000904_I18NRepo.BulkUpdate(updF000904_I18NDatas);

            #endregion

            #endregion

            #region Update 大分類(F1916)

            #region BulkUpdateF1916 & UpdateCnt
            List<F1916> updF1916Datas = new List<F1916>();

            var updF1916 = _thirdPartF1916List.Where(x => _f1916List.Any(z => x.ACODE == z.ACODE &&
                                                                              x.BCODE == z.BCODE)).ToList();

            updF1916.ForEach(updData =>
            {
                var currData = _f1916List.Where(z => z.ACODE == updData.ACODE && z.BCODE == updData.BCODE).SingleOrDefault();
                if (currData != null)
                {
                    if (currData.CLA_NAME != updData.CLA_NAME ||
                        currData.CHECK_PERCENT != updData.CHECK_PERCENT)
                    {
                        // 修改
                        updData.CLA_NAME = currData.CLA_NAME;
                        updData.CHECK_PERCENT = currData.CHECK_PERCENT;
                        updF1916Datas.Add(updData);
                    }

                    // 計算修改數
                    updF1916Cnt += mCategorysDatas.Where(x => x.LastData.LCode == currData.ACODE && x.LastData.MCode == currData.BCODE).Sum(x => x.Count);
                }
            });

            if (updF1916Datas.Any())
                f1916Repo.BulkUpdate(updF1916Datas);
            #endregion

            #endregion

            #region Update 大分類(F1917)

            #region BulkUpdateF1917 & UpdateCnt
            List<F1917> updF1917Datas = new List<F1917>();

            var updF1917 = _thirdPartF1917List.Where(x => _f1917List.Any(z => x.ACODE == z.ACODE &&
                                                                              x.BCODE == z.BCODE &&
                                                                              x.CCODE == z.CCODE)).ToList();

            updF1917.ForEach(updData =>
            {
                var currData = _f1917List.Where(z => z.ACODE == updData.ACODE && z.BCODE == updData.BCODE && z.CCODE == updData.CCODE).SingleOrDefault();
                if (currData != null)
                {
                    if (currData.CLA_NAME != updData.CLA_NAME ||
                        currData.CHECK_PERCENT != updData.CHECK_PERCENT)
                    {
                        // 修改
                        updData.CLA_NAME = currData.CLA_NAME;
                        updData.CHECK_PERCENT = currData.CHECK_PERCENT;
                        updF1917Datas.Add(updData);
                    }

                    // 計算修改數
                    updF1917Cnt += sCategorysDatas.Where(x => x.LastData.LCode == currData.ACODE && x.LastData.MCode == currData.BCODE && x.LastData.SCode == currData.CCODE).Sum(x => x.Count);
                }
            });

            if (updF1917Datas.Any())
                f1917Repo.BulkUpdate(updF1917Datas);
            #endregion

            #endregion

            #region Commit
            _wmsTransation.Complete();
            #endregion

            #region 回傳資料
            List<string> msgContentList = new List<string>();
            string msg = tacService.GetMsg("10003");

            // 大分類
            int f1915Total = lCategorys.Count;
            int failF1915Cnt = f1915Total - addF1915Cnt - updF1915Cnt;
            msgContentList.Add(string.Format(msg, "商品大分類", addF1915Cnt, updF1915Cnt, failF1915Cnt, f1915Total));

            // 中分類
            int f1916Total = mCategorys.Count;
            int failF1916Cnt = f1916Total - addF1916Cnt - updF1916Cnt;
            msgContentList.Add(string.Format(msg, "商品中分類", addF1916Cnt, updF1916Cnt, failF1916Cnt, f1916Total));

            // 小分類
            int f1917Total = sCategorys.Count;
            int failF1917Cnt = f1917Total - addF1917Cnt - updF1917Cnt;
            msgContentList.Add(string.Format(msg, "商品小分類", addF1917Cnt, updF1917Cnt, failF1917Cnt, f1917Total));

            res.IsSuccessed = !data.Any();
            res.MsgCode = "10003";
            res.MsgContent = string.Join("；", msgContentList);
            res.InsertCnt = addF1915Cnt + addF1916Cnt + addF1917Cnt;
            res.UpdateCnt = updF1915Cnt + updF1916Cnt + updF1917Cnt;
            res.FailureCnt = failF1915Cnt + failF1916Cnt + failF1917Cnt;
            res.TotalCnt = f1915Total + f1916Total + f1917Total;
            res.Data = data.Any() ? data : null;
            #endregion

            return res;
        }

        /// <summary>
        /// 大分類檢核 & 資料處理
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="lCategorys">商品大分類資料物件</param>
        /// <returns></returns>
        private ApiResult CheckLCategorys(string gupCode, string custCode, PostItemCategoryLCategorysModel lCategory)
        {
            ApiResult result = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            // 預設值設定
            data.AddRange((List<ApiResponse>)CheckDefaultSettingL(gupCode, custCode, lCategory).Data);

            // 共用欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLengthL(gupCode, custCode, lCategory).Data);

            // 貨主自訂欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckCustomColumnTypeL(gupCode, custCode, lCategory).Data);

            // 如果以上檢核成功
            if (!data.Any())
            {
                // 共用欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCommonColumnDataL(gupCode, custCode, lCategory).Data);

                // 貨主自訂欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCustomColumnValueL(gupCode, custCode, lCategory).Data);

                // 如果以上檢核成功
                if (!data.Any())
                {
                    CreateLCategoryData(gupCode, custCode, lCategory);
                }
            }

            result.IsSuccessed = !data.Any();
            result.Data = data;

            return result;
        }

        /// <summary>
        /// 中分類檢核 & 資料處理
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="mCategorys">商品中分類資料物件</param>
        /// <returns></returns>
        private ApiResult CheckMCategorys(string gupCode, string custCode, PostItemCategoryMCategorysModel mCategory)
        {
            ApiResult result = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            // 預設值設定
            data.AddRange((List<ApiResponse>)CheckDefaultSettingM(gupCode, custCode, mCategory).Data);

            // 共用欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLengthM(gupCode, custCode, mCategory).Data);

            // 貨主自訂欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckCustomColumnTypeM(gupCode, custCode, mCategory).Data);

            // 如果以上檢核成功
            if (!data.Any())
            {
                // 共用欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCommonColumnDataM(gupCode, custCode, mCategory).Data);

                // 貨主自訂欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCustomColumnValueM(gupCode, custCode, mCategory).Data);

                // 如果以上檢核成功
                if (!data.Any())
                {
                    CreateMCategoryData(gupCode, custCode, mCategory);
                }
            }

            result.IsSuccessed = !data.Any();
            result.Data = data;

            return result;
        }

        /// <summary>
        /// 小分類檢核 & 資料處理
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="sCategorys">商品小分類資料物件</param>
        /// <returns></returns>
        private ApiResult CheckSCategorys(string gupCode, string custCode, PostItemCategorySCategorysModel sCategory)
        {
            ApiResult result = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            // 預設值設定
            data.AddRange((List<ApiResponse>)CheckDefaultSettingS(gupCode, custCode, sCategory).Data);

            // 共用欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLengthS(gupCode, custCode, sCategory).Data);

            // 貨主自訂欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckCustomColumnTypeS(gupCode, custCode, sCategory).Data);

            // 如果以上檢核成功
            if (!data.Any())
            {
                // 共用欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCommonColumnDataS(gupCode, custCode, sCategory).Data);

                // 貨主自訂欄位資料檢核
                data.AddRange((List<ApiResponse>)CheckCustomColumnValueS(gupCode, custCode, sCategory).Data);

                // 如果以上檢核成功
                if (!data.Any())
                {
                    CreateSCategoryData(gupCode, custCode, sCategory);
                }
            }

            result.IsSuccessed = !data.Any();
            result.Data = data;

            return result;
        }

        #endregion

        #region Protected 檢核 (大分類)
        /// <summary>
        /// 預設值設定
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="lCategory">商品大分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckDefaultSettingL(string gupCode, string custCode, PostItemCategoryLCategorysModel lCategory)
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
        /// <param name="lCategory">商品大分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckColumnNotNullAndMaxLengthL(string gupCode, string custCode, PostItemCategoryLCategorysModel lCategory)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            #region 定義需檢核欄位、必填、型態、長度
            // 商品大分類資料物件
            List<ApiCkeckColumnModel> lCategoryCheckColumnList = new List<ApiCkeckColumnModel>
            {
                new ApiCkeckColumnModel{  Name = "LCode",    Type = typeof(string),   MaxLength = 20,  Nullable = false },
                new ApiCkeckColumnModel{  Name = "LName",    Type = typeof(string),   MaxLength = 20, Nullable = false },
                new ApiCkeckColumnModel{  Name = "LPercent", Type = typeof(decimal),  MaxLength = 14 }
            };
            #endregion

            #region 檢查商品欄位必填、最大長度
            List<string> lCategoryIsNullList = new List<string>();
            List<ApiCkeckColumnModel> lCategoryIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

            // 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
            lCategoryCheckColumnList.ForEach(column =>
            {
                var hasValue = DataCheckHelper.CheckRequireColumn(lCategory, column.Name);

                // 必填
                if (!column.Nullable && !hasValue)
                    lCategoryIsNullList.Add(column.Name);

                // 最大長度
                if (column.MaxLength > 0 && hasValue)
                {
                    switch (column.Name)
                    {
                        case "LPercent":
                            // 檢核是否符合decimal(14,11)
                            if (!DataCheckHelper.CheckDataIsDecimal(lCategory, column.Name, 3, 11))
                                lCategoryIsExceedMaxLenthList.Add(column);
                            break;
                        default:
                            if (!DataCheckHelper.CheckDataMaxLength(lCategory, column.Name, column.MaxLength))
                                lCategoryIsExceedMaxLenthList.Add(column);
                            break;
                    }
                }
            });

            // 必填訊息
            if (lCategoryIsNullList.Any())
            {
                data.Add(new ApiResponse { No = lCategory.LCode, MsgCode = "20020", MsgContent = string.Format(tacService.GetMsg("20020"), $"大分類編號{lCategory.LCode}", string.Join("、", lCategoryIsNullList)) });
            }

            // 最大長度訊息
            if (lCategoryIsExceedMaxLenthList.Any())
            {
                List<string> errorMsgList = lCategoryIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

                string errorMsg = string.Join("、", errorMsgList);

                data.Add(new ApiResponse { No = lCategory.LCode, MsgCode = "20021", MsgContent = string.Format(tacService.GetMsg("20021"), $"大分類編號{lCategory.LCode}", errorMsg) });
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
        /// <param name="lCategory">商品大分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnTypeL(string gupCode, string custCode, PostItemCategoryLCategorysModel lCategory)
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
        /// <param name="lCategory">商品大分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCommonColumnDataL(string gupCode, string custCode, PostItemCategoryLCategorysModel lCategory)
        {
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();
            res.Data = data;
            return res;
        }

        /// <summary>
        /// 貨主自訂欄位資料檢核
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="lCategory">商品大分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnValueL(string gupCode, string custCode, PostItemCategoryLCategorysModel lCategory)
        {
            // 請預留方法
            ApiResult res = new ApiResult();
            res.Data = new List<ApiResponse>();
            return res;
        }
        #endregion

        #region Protected 檢核 (中分類)
        /// <summary>
        /// 預設值設定
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="mCategory">商品大分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckDefaultSettingM(string gupCode, string custCode, PostItemCategoryMCategorysModel mCategory)
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
        /// <param name="mCategory">商品中分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckColumnNotNullAndMaxLengthM(string gupCode, string custCode, PostItemCategoryMCategorysModel mCategory)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            #region 定義需檢核欄位、必填、型態、長度
            // 商品中分類資料物件
            List<ApiCkeckColumnModel> mCategoryCheckColumnList = new List<ApiCkeckColumnModel>
            {
                new ApiCkeckColumnModel{  Name = "LCode",    Type = typeof(string),   MaxLength = 20,  Nullable = false },
                new ApiCkeckColumnModel{  Name = "MCode",    Type = typeof(string),   MaxLength = 20,  Nullable = false },
                new ApiCkeckColumnModel{  Name = "MName",    Type = typeof(string),   MaxLength = 80, Nullable = false },
                new ApiCkeckColumnModel{  Name = "MPercent", Type = typeof(decimal),  MaxLength = 14 }
            };
            #endregion

            #region 檢查商品欄位必填、最大長度
            List<string> mCategoryIsNullList = new List<string>();
            List<ApiCkeckColumnModel> mCategoryIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

            // 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
            mCategoryCheckColumnList.ForEach(column =>
            {
                var hasValue = DataCheckHelper.CheckRequireColumn(mCategory, column.Name);

                // 必填
                if (!column.Nullable && !hasValue)
                    mCategoryIsNullList.Add(column.Name);

                // 最大長度
                if (column.MaxLength > 0 && hasValue)
                {
                    switch (column.Name)
                    {
                        case "MPercent":
                            // 檢核是否符合decimal(13,11)
                            if (!DataCheckHelper.CheckDataIsDecimal(mCategory, column.Name, 3, 11))
                                mCategoryIsExceedMaxLenthList.Add(column);
                            break;
                        default:
                            if (!DataCheckHelper.CheckDataMaxLength(mCategory, column.Name, column.MaxLength))
                                mCategoryIsExceedMaxLenthList.Add(column);
                            break;
                    }
                }
            });

            // 必填訊息
            if (mCategoryIsNullList.Any())
            {
                data.Add(new ApiResponse { No = mCategory.LCode, MsgCode = "20020", MsgContent = string.Format(tacService.GetMsg("20020"), $"大分類編號{mCategory.LCode} 中分類編號{mCategory.MCode}", string.Join("、", mCategoryIsNullList)) });
            }

            // 最大長度訊息
            if (mCategoryIsExceedMaxLenthList.Any())
            {
                List<string> errorMsgList = mCategoryIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

                string errorMsg = string.Join("、", errorMsgList);

                data.Add(new ApiResponse { No = mCategory.LCode, MsgCode = "20021", MsgContent = string.Format(tacService.GetMsg("20021"), $"大分類編號{mCategory.LCode} 中分類編號{mCategory.MCode}", errorMsg) });
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
        /// <param name="mCategory">商品中分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnTypeM(string gupCode, string custCode, PostItemCategoryMCategorysModel mCategory)
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
        /// <param name="mCategory">商品中分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCommonColumnDataM(string gupCode, string custCode, PostItemCategoryMCategorysModel mCategory)
        {
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();
            res.Data = data;
            return res;
        }

        /// <summary>
        /// 貨主自訂欄位資料檢核
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="mCategory">商品中分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnValueM(string gupCode, string custCode, PostItemCategoryMCategorysModel mCategory)
        {
            // 請預留方法
            ApiResult res = new ApiResult();
            res.Data = new List<ApiResponse>();
            return res;
        }
        #endregion

        #region Protected 檢核 (小分類)
        /// <summary>
        /// 預設值設定
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="sCategory">商品小分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckDefaultSettingS(string gupCode, string custCode, PostItemCategorySCategorysModel sCategory)
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
        /// <param name="sCategory">商品小分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckColumnNotNullAndMaxLengthS(string gupCode, string custCode, PostItemCategorySCategorysModel sCategory)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            #region 定義需檢核欄位、必填、型態、長度
            // 商品中分類資料物件
            List<ApiCkeckColumnModel> sCategoryCheckColumnList = new List<ApiCkeckColumnModel>
            {
                new ApiCkeckColumnModel{  Name = "LCode",    Type = typeof(string),   MaxLength = 20,  Nullable = false },
                new ApiCkeckColumnModel{  Name = "MCode",    Type = typeof(string),   MaxLength = 20,  Nullable = false },
                new ApiCkeckColumnModel{  Name = "SCode",    Type = typeof(string),   MaxLength = 20,  Nullable = false },
                new ApiCkeckColumnModel{  Name = "SName",    Type = typeof(string),   MaxLength = 80, Nullable = false },
                new ApiCkeckColumnModel{  Name = "SPercent", Type = typeof(decimal),  MaxLength = 14 }
            };
            #endregion

            #region 檢查商品欄位必填、最大長度
            List<string> sCategoryIsNullList = new List<string>();
            List<ApiCkeckColumnModel> sCategoryIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

            // 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
            sCategoryCheckColumnList.ForEach(column =>
            {
                var hasValue = DataCheckHelper.CheckRequireColumn(sCategory, column.Name);

                // 必填
                if (!column.Nullable && !hasValue)
                    sCategoryIsNullList.Add(column.Name);

                // 最大長度
                if (column.MaxLength > 0 && hasValue)
                {
                    switch (column.Name)
                    {
                        case "SPercent":
                            // 檢核是否符合decimal(13,11)
                            if (!DataCheckHelper.CheckDataIsDecimal(sCategory, column.Name, 3, 11))
                                sCategoryIsExceedMaxLenthList.Add(column);
                            break;
                        default:
                            if (!DataCheckHelper.CheckDataMaxLength(sCategory, column.Name, column.MaxLength))
                                sCategoryIsExceedMaxLenthList.Add(column);
                            break;
                    }
                }
            });

            // 必填訊息
            if (sCategoryIsNullList.Any())
            {
                data.Add(new ApiResponse { No = sCategory.LCode, MsgCode = "20020", MsgContent = string.Format(tacService.GetMsg("20020"), $"大分類編號{sCategory.LCode} 中分類編號{sCategory.MCode} 小分類編號{sCategory.SCode}", string.Join("、", sCategoryIsNullList)) });
            }

            // 最大長度訊息
            if (sCategoryIsExceedMaxLenthList.Any())
            {
                List<string> errorMsgList = sCategoryIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

                string errorMsg = string.Join("、", errorMsgList);

                data.Add(new ApiResponse { No = sCategory.LCode, MsgCode = "20021", MsgContent = string.Format(tacService.GetMsg("20021"), $"大分類編號{sCategory.LCode} 中分類編號{sCategory.MCode} 小分類編號{sCategory.SCode}", errorMsg) });
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
        /// <param name="sCategory">商品小分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnTypeS(string gupCode, string custCode, PostItemCategorySCategorysModel sCategory)
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
        /// <param name="sCategory">商品小分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCommonColumnDataS(string gupCode, string custCode, PostItemCategorySCategorysModel sCategory)
        {
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();
            res.Data = data;
            return res;
        }

        /// <summary>
        /// 貨主自訂欄位資料檢核
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="sCategory">商品小分類資料物件</param>
        /// <returns></returns>
        protected ApiResult CheckCustomColumnValueS(string gupCode, string custCode, PostItemCategorySCategorysModel sCategory)
        {
            // 請預留方法
            ApiResult res = new ApiResult();
            res.Data = new List<ApiResponse>();
            return res;
        }
        #endregion

        #region Protected 建立商品大、中、小分類資料

        /// <summary>
        /// 產生商品大分類資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="lCategory"></param>
        /// <returns></returns>
        public void CreateLCategoryData(string gupCode, string custCode, PostItemCategoryLCategorysModel lCategory)
        {
            // 建立商品大分類檔F1915
            _f1915List.Add(CreateF1915(gupCode, custCode, lCategory));

            // 建立程式下拉選單參數設定對應檔F000904
            _f000904List.Add(CreateF000904(gupCode, custCode, lCategory));

            // 程式下拉選單參數設定語系對應檔清單F000904_I18
            _langs.ForEach(lang =>
            {
                _f000904_I18NList.Add(CreateF000904_I18N(gupCode, custCode, lCategory, lang));
            });
        }

        /// <summary>
        /// 建立商品大分類檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="lCategory"></param>
        /// <returns></returns>
        protected F1915 CreateF1915(string gupCode, string custCode, PostItemCategoryLCategorysModel lCategory)
        {
            return new F1915
            {
                GUP_CODE = gupCode,
                CUST_CODE = custCode,
                ACODE = lCategory.LCode,
                CLA_NAME = lCategory.LName,
                CHECK_PERCENT = lCategory.LPercent
            };
        }

        /// <summary>
        /// 建立程式下拉選單參數設定對應檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="lCategory"></param>
        /// <returns></returns>
        protected F000904 CreateF000904(string gupCode, string custCode, PostItemCategoryLCategorysModel lCategory)
        {
            return new F000904
            {
                TOPIC = "F1903",
                SUBTOPIC = "TYPE",
                SUB_NAME = "商品類別",
                VALUE = lCategory.LCode,
                NAME = lCategory.LName,
                ISUSAGE = "1"
            };
        }

        /// <summary>
        /// 建立商品材積階層檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="lCategory"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        protected F000904_I18N CreateF000904_I18N(string gupCode, string custCode, PostItemCategoryLCategorysModel lCategory, string lang)
        {
            return new F000904_I18N
            {
                TOPIC = "F1903",
                SUBTOPIC = "TYPE",
                VALUE = lCategory.LCode,
                NAME = lCategory.LName,
                SUB_NAME = "商品類別",
                LANG = lang
            };
        }
        #endregion

        #region Protected 建立商品中分類資料
        /// <summary>
        /// 產生商品中分類資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="mCategory"></param>
        /// <returns></returns>
        public void CreateMCategoryData(string gupCode, string custCode, PostItemCategoryMCategorysModel mCategory)
        {
            // 建立商品中分類檔F1916
            _f1916List.Add(CreateF1916(gupCode, custCode, mCategory));
        }

        /// <summary>
        /// 建立商品中分類檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="mCategory"></param>
        /// <returns></returns>
        protected F1916 CreateF1916(string gupCode, string custCode, PostItemCategoryMCategorysModel mCategory)
        {
            return new F1916
            {
                GUP_CODE = gupCode,
                CUST_CODE = custCode,
                ACODE = mCategory.LCode,
                BCODE = mCategory.MCode,
                CLA_NAME = mCategory.MName,
                CHECK_PERCENT = mCategory.MPercent
            };
        }

        #endregion

        #region Protected 建立商品小分類資料

        /// <summary>
        /// 產生商品小分類資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="sCategory"></param>
        /// <returns></returns>
        public void CreateSCategoryData(string gupCode, string custCode, PostItemCategorySCategorysModel sCategory)
        {
            // 建立商品小分類檔F1917
            _f1917List.Add(CreateF1917(gupCode, custCode, sCategory));
        }

        /// <summary>
        /// 建立商品小分類檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="sCategory"></param>
        /// <returns></returns>
        protected F1917 CreateF1917(string gupCode, string custCode, PostItemCategorySCategorysModel sCategory)
        {
            return new F1917
            {
                GUP_CODE = gupCode,
                CUST_CODE = custCode,
                ACODE = sCategory.LCode,
                BCODE = sCategory.MCode,
                CCODE = sCategory.SCode,
                CLA_NAME = sCategory.SName,
                CHECK_PERCENT = sCategory.SPercent
            };
        }
        #endregion
    }
}
