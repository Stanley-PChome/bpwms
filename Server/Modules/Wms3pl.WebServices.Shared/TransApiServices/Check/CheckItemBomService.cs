using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
    public class CheckItemBomService
    {
        private TransApiBaseService _tacService;
        private CommonService _commonService;

        public CheckItemBomService()
        {
            _tacService = new TransApiBaseService();
            _commonService = new CommonService();
        }

        #region 主檔檢核
        /// <summary>
        /// 檢核商品單位編號
        /// </summary>
        /// <param name="res"></param>
        /// <param name="itemBom"></param>
        /// <param name="unitIdList"></param>
        public void CheckUnitId(List<ApiResponse> res, PostItemBomModel itemBom, List<string> unitIdList)
        {
            if (!unitIdList.Contains(itemBom.UnitId))
                res.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "21078", MsgContent = string.Format(_tacService.GetMsg("21078"), itemBom.BomCode, itemBom.UnitId) });
        }

        /// <summary>
        /// 檢核組合類別
        /// </summary>
        /// <param name="res"></param>
        /// <param name="itemBom"></param>
        public void CheckBomType(List<ApiResponse> res, PostItemBomModel itemBom)
        {
            List<string> bomTypes = new List<string> { "0", "1" };
            if (!bomTypes.Contains(itemBom.BomType))
                res.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "20078", MsgContent = string.Format(_tacService.GetMsg("20078"), itemBom.BomCode) });
        }

        /// <summary>
        /// 檢核是否加工
        /// </summary>
        /// <param name="res"></param>
        /// <param name="itemBom"></param>
        public void CheckIsProcess(List<ApiResponse> res, PostItemBomModel itemBom)
        {
            List<string> isProcess = new List<string> { "0", "1" };
            if (!isProcess.Contains(itemBom.BomType))
                res.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "20079", MsgContent = string.Format(_tacService.GetMsg("20079"), itemBom.BomCode) });
        }

        /// <summary>
        /// 檢核組合商品狀態
        /// </summary>
        /// <param name="res"></param>
        /// <param name="itemBom"></param>
        public void CheckStatus(List<ApiResponse> res, PostItemBomModel itemBom)
        {
            List<string> status = new List<string> { "0", "9" };
            if (!status.Contains(itemBom.Status))
                res.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "20080", MsgContent = string.Format(_tacService.GetMsg("20080"), itemBom.BomCode) });
        }

        /// <summary>
        /// 檢核商品編號是否存在
        /// </summary>
        /// <param name="res"></param>
        /// <param name="itemBom"></param>
        /// <param name="itemCodeList"></param>
        public void CheckFgCode(List<ApiResponse> res, PostItemBomModel itemBom, List<string> itemCodeList)
        {
            if (!itemCodeList.Contains(itemBom.FgCode))
                res.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "20081", MsgContent = string.Format(_tacService.GetMsg("20081"), itemBom.BomCode, itemBom.FgCode) });
        }

        /// <summary>
        /// 檢核明細
        /// </summary>
        /// <param name="res"></param>
        /// <param name="itemBom"></param>
        /// <param name="itemCodeList"></param>
        public void CheckDetail(List<ApiResponse> res, PostItemBomModel itemBom)
        {
            if (itemBom.BomDetails == null || !itemBom.BomDetails.Any())
                res.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "20082", MsgContent = string.Format(_tacService.GetMsg("20082"), itemBom.BomCode) });
        }

        #endregion

        #region 明細檢核

        /// <summary>
        /// 檢核組合順序
        /// </summary>
        /// <param name="res"></param>
        /// <param name="itemBom"></param>
        public void CheckCombinSeq(List<ApiResponse> res, PostItemBomModel itemBom)
        {
            if (itemBom.BomDetails != null)
            {
                List<string> failSeq = new List<string>();

                var groupData = itemBom.BomDetails.GroupBy(x => x.CombinSeq).Select(x => new { Seq = x.Key.ToString(), Count = x.Count() }).ToList();
                groupData.ForEach(obj =>
                {
                    if (obj.Count > 1)
                        failSeq.Add(obj.Seq);
                });

                if (failSeq.Any())
                    res.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "20083", MsgContent = string.Format(_tacService.GetMsg("20083"), itemBom.BomCode, string.Join("、", failSeq)) });
            }
        }

        /// <summary>
        /// 檢核商品編號是否存在
        /// </summary>
        /// <param name="res"></param>
        /// <param name="itemBom"></param>
        /// <param name="itemCodeList"></param>
        public void CheckMaterialCode(List<ApiResponse> res, PostItemBomModel itemBom, List<string> itemCodeList)
        {
            if (itemBom.BomDetails != null)
            {
                var detailData = itemBom.BomDetails.GroupBy(x => x.MaterialCode).Select(x => new { MaterialCode = x.Key, Count = x.Count() }).ToList();
                var materialCodes = detailData.Select(x => x.MaterialCode).ToList();
                var notExist = materialCodes.Where(x => !itemCodeList.Contains(x)).ToList();
                if (notExist.Any())
                    res.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "20084", MsgContent = string.Format(_tacService.GetMsg("20084"), itemBom.BomCode, string.Join("、", notExist)) });

                var repeatMaterialCodes = new List<string>();
                detailData.ForEach(o=> 
                {
                    if (o.Count > 1)
                        repeatMaterialCodes.Add(o.MaterialCode);
                });
                if(repeatMaterialCodes.Any())
                    res.Add(new ApiResponse { No = itemBom.BomCode, MsgCode = "20085", MsgContent = string.Format(_tacService.GetMsg("20085"), itemBom.BomCode, string.Join("、", repeatMaterialCodes)) });
            }
        }

        /// <summary>
        /// 檢核明細數量是否大於0
        /// </summary>
        /// <param name="res"></param>
        /// <param name="itemBom"></param>
        public void CheckDetailValueNotZero(List<ApiResponse> res, PostItemBomModel itemBom)
        {
            if (itemBom.BomDetails != null)
            {
                for (int i = 0; i < itemBom.BomDetails.Count; i++)
                {
                    var currData = itemBom.BomDetails[i];

                    if (!DataCheckHelper.CheckDataNotZero(currData, "Qty"))
                        res.Add(new ApiResponse
                        {
                            No = itemBom.BomCode,
                            MsgCode = "20019",
                            MsgContent = string.Format(_tacService.GetMsg("20019"), $"{itemBom.BomCode}第 {i + 1} 筆明細", "QTY")
                        });
                }
            }
        }
        #endregion
    }
}
