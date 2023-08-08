using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
    /// <summary>
    /// 出貨更換物流商
    /// </summary>
    public class CommonChangeTransportProviderService
    {
        private WmsTransaction _wmsTransation;

        public CommonChangeTransportProviderService(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;
        }

       
        public ApiResult RecevieApiDatas(ChangeTransportProviderReq req)
        {
            CheckTransApiService ctaService = new CheckTransApiService();
            TransApiBaseService tacService = new TransApiBaseService();
            SharedService sharedService = new SharedService();
            CommonService commonService = new CommonService();
            ApiResult res = new ApiResult
            {
                IsSuccessed = true,
                MsgCode = "10003",
                MsgContent = string.Format(tacService.GetMsg("10001"), "出貨更換物流商", 0, 1, 0, 1),
            };

            // 取得業主編號
            string gupCode = commonService.GetGupCode(req.CustCode);

            #region 資料檢核1

            // 檢核物流中心 必填、是否存在
            ctaService.CheckDcCode(ref res, req);
            if (!res.IsSuccessed)
            {
                return res;
            }

            // 檢核貨主編號 必填、是否存在
            ctaService.CheckCustCode(ref res, req);
            if (!res.IsSuccessed)
            {
                return res;
            }

						// 檢核PackingList
						if (req.PackingList == null)
						{
							 return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };
						}

            // 檢核資料筆數 = 0
            if (req.PackingList.Count() == 0)
            {
                return new ApiResult { IsSuccessed = false, MsgCode = "20071", MsgContent = string.Format(tacService.GetMsg("20071"), req.CustOrdNo) };
            }

            // 檢核出貨類型
            res = CheckOrderType(req);
            if (!res.IsSuccessed)
            {
                return res;
            }

            //檢查貨主出貨單號
            res = CheckCustOrdNo(req, gupCode);
            if (!res.IsSuccessed)
            {
                return res;
            }
            #endregion

            #region 資料處理
            res = ProcessApiDatas(req, gupCode);
            #endregion
           
            return res;
        }

        #region 資料檢核1-出貨類型
        /// <summary>
        /// 資料檢核1-出貨類型
        /// </summary>
        /// <param name="req">api資料</param>
        /// <returns></returns>
        private ApiResult CheckOrderType(ChangeTransportProviderReq req)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult res = new ApiResult { IsSuccessed = true };
            //是否為空值
            if (string.IsNullOrEmpty(req.OrderType))
            {
                res = new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };
            }
            // 是否等於1
            if (req.OrderType != "1")
            {
                res = new ApiResult { IsSuccessed = false, MsgCode = "20024", MsgContent = tacService.GetMsg("20024") };
            }
            return res;
        }
        #endregion

        #region 資料存在檢核-貨主出貨單號是否存在
        /// <summary>
        /// 資料存在檢核-貨主出貨單號是否存在
        /// </summary>
        /// <param name="req">api資料</param>
        /// <param name="gupCode">業主編號</param>
        /// <returns></returns>
        private ApiResult CheckCustOrdNo(ChangeTransportProviderReq req, string gupCode)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult res = new ApiResult { IsSuccessed = true };
            F050301Repository f050301Repo = new F050301Repository(Schemas.CoreSchema);

            if (!f050301Repo.IsExistF050301CustOrdNo(req.DcCode, gupCode, req.CustCode, req.CustOrdNo))
            {
                res = new ApiResult { IsSuccessed = false, MsgCode = "20962", MsgContent = string.Format(tacService.GetMsg("20962"), req.CustOrdNo) };
            }
            return res;
        }
        #endregion

        #region 資料處理
        /// <summary>
        /// 資料處理
        /// </summary>
        /// <param name="req">api資料</param>
        /// <param name="gupCode">業主編號</param>
        /// <returns></returns>
        private ApiResult ProcessApiDatas(ChangeTransportProviderReq req, string gupCode)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult res = new ApiResult { IsSuccessed = true };
            List<ApiResponse> data = new List<ApiResponse>();

            var packingList = req.PackingList;
            packingList.ForEach(pack =>
            {
                // res = CheckOrder(req.DcCode, gupCode, req.CustCode, req.CustOrdNo, pack);
                //res = resl;
                data.AddRange((List<ApiResponse>)CheckOrder(req.DcCode, gupCode, req.CustCode, req.CustOrdNo, pack).Data);
            });
            res.IsSuccessed = !data.Any();
						if (!res.IsSuccessed)
						{
							res.InsertCnt = 0;
							res.UpdateCnt = 0;
							res.FailureCnt = 1;
							res.TotalCnt = 1;
							res.Data = data;
						}
						else
						{
							res = new ApiResult
							{
								IsSuccessed = true,
								MsgCode = "10003",
								MsgContent = string.Format(tacService.GetMsg("10003"), "出貨更換物流商", 0, 1, 0, 1),
								InsertCnt = 0,
								UpdateCnt = 1,
								FailureCnt = 0,
								TotalCnt = 1
							};
						}
            return res;
        }

        private ApiResult CheckOrder(string dcCode, string gupCode, string custCode, string custOrdNo, Result pack)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult result = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();
            F055001Repository f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransation);
            F050901Repository f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransation);

            // 訂單資料物件
            List<ApiCkeckColumnModel> orderCheckColumnList = new List<ApiCkeckColumnModel>();
            orderCheckColumnList = new List<ApiCkeckColumnModel>
                {
                    new ApiCkeckColumnModel{  Name = "WmsNo",             Type = typeof(string),   MaxLength = 20, Nullable = false },
                    new ApiCkeckColumnModel{  Name = "BoxNo",              Type = typeof(int),   MaxLength = 9, Nullable = false },
                    new ApiCkeckColumnModel{  Name = "TransportCode",              Type = typeof(string),   MaxLength = 32, Nullable = false },
                    new ApiCkeckColumnModel{  Name = "TransportProvider",              Type = typeof(string),   MaxLength = 20, Nullable = false }
                };

            List<string> orderIsNullList = new List<string>();
            List<ApiCkeckColumnModel> orderIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();
            List<string> failColunList = new List<string>();
            orderCheckColumnList.ForEach(column =>
            {
                // 必填
                if (!column.Nullable)
                {
                    if (!DataCheckHelper.CheckRequireColumn(pack, column.Name))
                    {
                        orderIsNullList.Add(column.Name);
                    }
                }

                if (column.MaxLength > 0)
                {
                    if (!DataCheckHelper.CheckDataMaxLength(pack, column.Name, column.MaxLength))
                    {
                        orderIsExceedMaxLenthList.Add(column);
                    }
                }

                if (column.Name == "BoxNo")
                {
                    //檢核必須大於0且為正整數
                    if (!DataCheckHelper.CheckDataNotZero(pack, "BoxNo"))
                    {
                        failColunList.Add(column.Name);
                    }
                }
            });

            // 必填訊息
            if (orderIsNullList.Any())
            {
                // 回傳訊息內容[20058, < 參數4 >.CustInNo, 必填欄位未填寫的欄位清單(格式:[欄位名稱1]、[欄位名稱2])]
                data.Add(new ApiResponse { No = pack.WmsNo, MsgCode = "20058", MsgContent = string.Format(tacService.GetMsg("20058"), pack.WmsNo, string.Join("、", orderIsNullList)) });
            }

            // 最大長度訊息
            if (orderIsExceedMaxLenthList.Any())
            {
                List<string> errorMsgList = orderIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

                string errorMsg = string.Join("、", errorMsgList);

                // 檢查進倉單欄位格式(參考2.8.2),如果檢核失敗，回傳 & 訊息內容[20059, < 參數4 >.CustInNo, 欄位格式錯誤清單(格式:[欄位名稱1]格式錯誤必須為X1、[欄位名稱2]格式錯誤必須為X2)]
                data.Add(new ApiResponse { No = custOrdNo, MsgCode = "20059", MsgContent = string.Format(tacService.GetMsg("20059"), custOrdNo, errorMsg) });
            }

            if (failColunList.Any())
            {
                data.Add(new ApiResponse
                {
                    No = pack.WmsNo,
                    MsgCode = "20019",
                    MsgContent = string.Format(tacService.GetMsg("20019"), pack.WmsNo, "BoxNo")
                });

            }
            else
            {
                var f055001 = f055001Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.WMS_ORD_NO == pack.WmsNo && o.PACKAGE_BOX_NO == pack.BoxNo).FirstOrDefault();
                if (f055001 == null)
                {
                    data.Add(new ApiResponse { MsgCode = "20025", MsgContent = string.Format(tacService.GetMsg("20025"), custOrdNo, pack.WmsNo, pack.BoxNo) });
                    //result = new ApiResult { IsSuccessed = false, MsgCode = "10003", MsgContent = string.Format(tacService.GetMsg("10003"), "出貨更換物流商", 0, 0, 1, 1), Data = data };
                }
                else
                {
                    var f050901 = f050901Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f055001.DC_CODE && o.GUP_CODE == f055001.GUP_CODE && o.CUST_CODE == f055001.CUST_CODE && o.CONSIGN_NO == f055001.PAST_NO && o.WMS_NO == f055001.WMS_ORD_NO).FirstOrDefault();
                    if (f050901 != null)
                    {
                        /*
                           如果存在 (1)	更新F050901.CONSIGN_NO= pack.TransportCode,
                                        F050901.DELIVID_SEQ_NAME = pack.TransportProvider
                        */
                        f050901.CONSIGN_NO = pack.TransportCode;
                        f050901.DELIVID_SEQ_NAME = pack.TransportProvider;
                        f050901Repo.Update(f050901);

                        /* (2)	更新F055001.PAST_NO =  pack.TransportCode*/
                        f055001.PAST_NO = pack.TransportCode;
                        f055001.LOGISTIC_CODE = pack.TransportProvider;
                        f055001Repo.Update(f055001);
                    }
                }
            }


            result.IsSuccessed = !data.Any();
            result.Data = data;


            return result;
        }
        #endregion
    }
}
