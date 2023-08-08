using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using System.Collections.ObjectModel;

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P160201Service
	{
        /// <summary>
        /// Import 廠退資料
        /// </summary>
        public ExecuteResult ImportF1602Data(string dcCode, string gupCode, string custCode
                                            , string fileName, ObservableCollection<F1602ImportData> importData)
        {
            ExecuteResult result = new ExecuteResult();
            string errorMessage = string.Empty;
            string dataContent = string.Empty;
            int successCtn = 0;
            int errorCtn = 0;
            result.IsSuccessed = true;

            var f1903Repo = new F1903Repository(Schemas.CoreSchema);
            var f1951Repo = new F1951Repository(Schemas.CoreSchema);
            var f000903Repo = new F000903Repository(Schemas.CoreSchema);
            var f160203Repo = new F160203Repository(Schemas.CoreSchema);
            var f1908Repo = new F1908Repository(Schemas.CoreSchema);
            var f1980Repo = new F1980Repository(Schemas.CoreSchema);
            var f1912Repo = new F1912Repository(Schemas.CoreSchema);

            //先把所有基本檔撈出來. 必免每次都進 DB
            var f000903Datas = f000903Repo.Filter(o => 1 == 1).ToList();
            var f1951Datas = f1951Repo.Filter(o => o.UCT_ID == "RV").ToList();
            var f160203Datas = f160203Repo.Filter(o => 1 == 1).ToList();
            var f1908Datas = f1908Repo.Filter(o => o.GUP_CODE == gupCode).ToList();
            var f1980Datas = f1980Repo.Filter(o => 1 == 1).ToList();

            var groupData = importData.GroupBy(o => new { o.DC_CODE, o.VNR_CODE,o.ORD_PROP,o.RTN_VNR_TYPE_ID,o.RTN_VNR_CAUSE,o.MEMO }).ToList();
            foreach (var groupItem in groupData)
            {

                #region 檢查規則

                //檢查作業類別
                if (string.IsNullOrEmpty(groupItem.Key.ORD_PROP))
                {
                    errorMessage += Properties.Resources.P160101Service_WorkType_Empty;
                }
                else
                {
					if (f000903Datas.FirstOrDefault(o => o.ORD_PROP == groupItem.Key.ORD_PROP) == null)
                        errorMessage += Properties.Resources.P160201Servicepart1_WorkTypeNO_NotFound+ groupItem.Key.ORD_PROP + " ;";
                }

                //廠退類型
                if (string.IsNullOrEmpty(groupItem.Key.RTN_VNR_TYPE_ID))
                {
                    errorMessage += Properties.Resources.P160201Servicepart1_VNR_RTN_Type_Empty;
                }
                else
                {
                    if (f160203Datas.FirstOrDefault(o => o.RTN_VNR_TYPE_ID == groupItem.Key.RTN_VNR_TYPE_ID) == null)
                        errorMessage += Properties.Resources.P160201Servicepart1_VNR_RTN_Type_NotFound+ groupItem.Key.RTN_VNR_TYPE_ID + " ;";
                }

                //廠退原因
                if (string.IsNullOrEmpty(groupItem.Key.RTN_VNR_CAUSE))
                {
                    errorMessage += Properties.Resources.P160201Servicepart1_VNR_RTN_Cause_Empty;
                }
                else
                {
                    if (f1951Datas.FirstOrDefault(o => o.UCC_CODE == groupItem.Key.RTN_VNR_CAUSE) == null)
                        errorMessage += Properties.Resources.P160201Servicepart1_VNR_RTN_CauseNo_NotFound+ groupItem.Key.RTN_VNR_CAUSE + " ;";
                }

                //廠商編號
                if (string.IsNullOrEmpty(groupItem.Key.VNR_CODE))
                {
                    errorMessage += Properties.Resources.P160201Servicepart1_VNR_CODE_Empty;
                }
                else
                {
                    if (f1908Datas.FirstOrDefault(o => o.VNR_CODE == groupItem.Key.VNR_CODE) == null)
                        errorMessage += Properties.Resources.P160201Servicepart1_VNR_CODE_NotFound+ groupItem.Key.VNR_CODE + " ;";
                }

                var f160202s = new ObservableCollection<F160202>();
                foreach (var item in importData.Where(o => o.VNR_CODE == groupItem.Key.VNR_CODE).GroupBy(o => new { o.DC_CODE,o.ITEM_CODE,o.WAREHOUSE_ID,o.LOC_CODE,o.RTN_VNR_QTY }))
                {
                    //來源倉別
                    if (string.IsNullOrEmpty(item.Key.WAREHOUSE_ID))
                    {
                        errorMessage += Properties.Resources.P160201Servicepart1_SRC_WarehouseID_Empty;
                    }
                    else
                    {
                        if (f1980Datas.FirstOrDefault(o => o.WAREHOUSE_ID == item.Key.WAREHOUSE_ID && o.DC_CODE ==item.Key.DC_CODE) == null)
                            errorMessage += Properties.Resources.P160201Servicepart1_SRC_WarehouseID_NotFound + item.Key.WAREHOUSE_ID + " ;";
                    }

                    //品號
                    if (string.IsNullOrEmpty(item.Key.ITEM_CODE))
                    {
                        errorMessage += Properties.Resources.P160101Service_Item_Code_Empty;
                    }
                    else
                    {
                        if (f1903Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ITEM_CODE == item.Key.ITEM_CODE) == null)
                            errorMessage += Properties.Resources.P160201Servicepart1_Item_Code_NotFound + item.Key.ITEM_CODE + " ;";
                    }

                    //儲位
                    if (string.IsNullOrEmpty(item.Key.LOC_CODE))
                    {
                        errorMessage += Properties.Resources.P160201Servicepart1_Loc_Code_Empty;
                    }
                    else
                    {
                        if (f1912Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.LOC_CODE == item.Key.LOC_CODE) == null)
                            errorMessage += Properties.Resources.P160201Servicepart1_Loc_Code_NotFound + item.Key.LOC_CODE + " ;";
                    }

                    //廠退數量
                    if (item.Key.RTN_VNR_QTY == 0)
                        errorMessage += Properties.Resources.P160201Servicepart1_VNR_RTN_Zero;

                    var f160202 = new F160202();
                    f160202.WAREHOUSE_ID = item.Key.WAREHOUSE_ID;
                    f160202.ITEM_CODE = item.Key.ITEM_CODE;
                    f160202.LOC_CODE = item.Key.LOC_CODE;
                    f160202.RTN_VNR_QTY = item.Key.RTN_VNR_QTY;
                    f160202s.Add(f160202);

                    //Log DataContent 欄位
                    dataContent = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}"
                                                , groupItem.Key.DC_CODE, groupItem.Key.ORD_PROP, groupItem.Key.RTN_VNR_TYPE_ID
                                                , groupItem.Key.RTN_VNR_CAUSE, groupItem.Key.VNR_CODE, groupItem.Key.MEMO
                                                , item.Key.WAREHOUSE_ID, item.Key.ITEM_CODE,item.Key.LOC_CODE, item.Key.RTN_VNR_QTY);

                    UpdateF0060Log(fileName, "6", item.Key.ITEM_CODE, dataContent, errorMessage, dcCode, gupCode, custCode);

                    if (string.IsNullOrEmpty(errorMessage))
                        successCtn += 1;
                    else
                        errorCtn += 1;
                }
                #endregion

                if (string.IsNullOrEmpty(errorMessage))
                {
                    //寫入廠退主檔 
                    var f160201 = new F160201();
                    f160201.RTN_VNR_DATE = DateTime.Today;
                    f160201.POSTING_DATE = DateTime.Today;
                    f160201.GUP_CODE = gupCode;
                    f160201.CUST_CODE = custCode;
                    f160201.DC_CODE = groupItem.Key.DC_CODE;
                    f160201.ORD_PROP = groupItem.Key.ORD_PROP;
                    f160201.RTN_VNR_TYPE_ID = groupItem.Key.RTN_VNR_TYPE_ID;
                    f160201.RTN_VNR_CAUSE = groupItem.Key.RTN_VNR_CAUSE;
                    f160201.VNR_CODE = groupItem.Key.VNR_CODE;
                    f160201.MEMO = groupItem.Key.MEMO;
					f160201.TYPE_ID = "R"; // 廠退單匯入預設為R

					InsertF160201(f160201, f160202s.ToArray());
                }

                errorMessage = string.Empty;
            }

            result.Message = string.Format(Properties.Resources.P160101Service_ImportResult, importData.Count, successCtn, errorCtn);
            return result;
        }

        #region F0060 Log
        /// <summary>
        /// 匯入 Log 記錄表
        /// </summary>
        /// <param name="fileType">匯入類型(0商品主檔1進倉單2訂單3退貨單4派車單)F000904</param>	
        public bool UpdateF0060Log(string fileName, string fileType, string dataKey, string dataContent
                                    , string message, string dcCode, string gupCode, string custCode)
        {

            var f0060Repo = new F0060Repository(Schemas.CoreSchema, _wmsTransaction);
            F0060 f0060 = new F0060
            {
                FILE_NAME = fileName,
                FILE_TYPE = fileType,
                DATA_KEY = dataKey,
                DATA_CONTENT = dataContent,
                MESSAGE = message,
                STATUS = string.IsNullOrEmpty(message) ? "1" : "9",
                DC_CODE = dcCode,
                GUP_CODE = gupCode,
                CUST_CODE = custCode
            };

            f0060Repo.Add(f0060);


            return true;
        }
        #endregion
    }
}
