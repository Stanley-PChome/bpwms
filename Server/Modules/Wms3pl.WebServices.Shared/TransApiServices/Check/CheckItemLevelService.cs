using System.Collections.Generic;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
    public class CheckItemLevelService
    {
        private TransApiBaseService _tacService;
        private CommonService _commonService;

        public CheckItemLevelService()
        {
            _tacService = new TransApiBaseService();
            _commonService = new CommonService();
        }

        public void CheckValueNotZero(List<ApiResponse> res, PostItemLevelLevelsModel itemLevel)
        {
            List<string> failCols = new List<string>();
            List<string> chkCols = new List<string> { "ItemLevel", "UnitQty", "Length", "Width", "Height", "Weight" };

            chkCols.ForEach(colName =>
            {
                if (!DataCheckHelper.CheckDataNotZero(itemLevel, colName))
                    failCols.Add(colName);
            });

            if (failCols.Count > 0)
            {
                res.Add(new ApiResponse
                {
                    No = itemLevel.ItemCode,
                    MsgCode = "20019",
                    MsgContent = string.Format(_tacService.GetMsg("20019"),
                    $"{itemLevel.ItemCode}][商品單位編號{itemLevel.UnitId}" ,
                    string.Join("、", failCols))
                });
            }
        }

        /// <summary>
        /// 檢核商品編號是否存在
        /// </summary>
        /// <param name="res"></param>
        /// <param name="itemLevel"></param>
        /// <param name="itemCodeList"></param>
        public void CheckItemCode(List<ApiResponse> res, PostItemLevelLevelsModel itemLevel, List<string> itemCodeList)
        {
            if (!itemCodeList.Contains(itemLevel.ItemCode))
                res.Add(new ApiResponse { No = itemLevel.ItemCode, MsgCode = "20076", MsgContent = string.Format(_tacService.GetMsg("20076"), itemLevel.ItemCode) });
        }

        /// <summary>
        /// 檢核商品單位編號是否存在
        /// </summary>
        /// <param name="res"></param>
        /// <param name="order"></param>
        /// <param name="unitIdList"></param>
        public void CheckUnitId(List<ApiResponse> res, PostItemLevelLevelsModel itemLevel, List<string> unitIdList)
        {
            if (!unitIdList.Contains(itemLevel.UnitId.PadLeft(2, '0')))
                res.Add(new ApiResponse { No = itemLevel.ItemCode, MsgCode = "20077", MsgContent = string.Format(_tacService.GetMsg("20077"), itemLevel.UnitId) });
        }

        /// <summary>
        /// 檢核系統單位是否存在
        /// </summary>
        /// <param name="res"></param>
        /// <param name="order"></param>
        public void CheckSysUnit(List<ApiResponse> res, PostItemLevelLevelsModel itemLevel)
        {
            List<string> sysUnit = new List<string> { "01", "02" };

            if (!string.IsNullOrWhiteSpace(itemLevel.SysUnit) && !sysUnit.Contains(itemLevel.SysUnit))
                res.Add(new ApiResponse { No = itemLevel.ItemCode, MsgCode = "20771", MsgContent = string.Format(_tacService.GetMsg("20771"), itemLevel.ItemCode) });
        }
    }
}
