
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using System.Collections.ObjectModel;

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P160101Service
	{
		
		/// <summary>
		/// Import 退貨資料 For Hiiir
		/// </summary>
		public ExecuteResult ImportF1612ForHiiir(string dcCode, string gupCode, string custCode
											, string fileName, List<F1612ImportData> importData)
		{
			ExecuteResult result = new ExecuteResult();
			string errorMessage = string.Empty;
			string dataContent = string.Empty;
			int successCtn = 0;
			int errorCtn = 0;
			result.IsSuccessed = true;

			var f1903Repo = new F1903Repository(Schemas.CoreSchema);

            var groupData = importData.GroupBy(o => new {o.CUST_ORD_NO,o.CONTACT,o.TEL,o.ADDRESS,o.MEMO }).ToList();
            foreach (var groupItem in groupData)
            {

                #region 檢查規則

                //貨主單號
                if (string.IsNullOrEmpty(groupItem.Key.CUST_ORD_NO))
                    errorMessage += Properties.Resources.P160101Servicepart1_RTN_No_Empty;

                //聯絡人
                if (string.IsNullOrEmpty(groupItem.Key.CONTACT))
                    errorMessage += Properties.Resources.P160101Service_Contactor_Empty;

                //電話
                if (string.IsNullOrEmpty(groupItem.Key.TEL))
                    errorMessage += Properties.Resources.P160101Service_TEL_Empty;

                //地址
                if (string.IsNullOrEmpty(groupItem.Key.ADDRESS))
                    errorMessage += Properties.Resources.P160101Service_ADR_Empty;

                var f161202s = new ObservableCollection<F161202>();
                foreach (var item in importData.Where(o => o.CUST_ORD_NO == groupItem.Key.CUST_ORD_NO).GroupBy(o => new { o.CUST_ORD_NO, o.ITEM_CODE,o.RTN_QTY }))
                {
                    var item_code = "";

                    //商品品號
                    if (string.IsNullOrEmpty(item.Key.ITEM_CODE))
                    {
                        errorMessage += Properties.Resources.P160101Service_Item_Code_Empty;
                    }
                    //Hiiir商品編號中必須包含左右括弧
                    else if (item.Key.ITEM_CODE.ToString().IndexOf('(') == -1 || item.Key.ITEM_CODE.ToString().IndexOf(')') == -1)
                    {
                        errorMessage += Properties.Resources.P160101Servicepart1_ItemCodeFormatError_LF;
                    }
                    else if (item.Key.ITEM_CODE.ToString().IndexOf('(') != 0)
                    {
                        errorMessage += Properties.Resources.P160101Servicepart1_ItemCodeFormatError_L;
                    }
                    else
                    {
                        var tItemCode = item.Key.ITEM_CODE;
                        item_code = tItemCode.Substring(tItemCode.IndexOf('(') + 1, tItemCode.IndexOf(')') - tItemCode.IndexOf('(') - 1);
                        //var tItemName = tItemCode.Substring(tItemCode.IndexOf(')') + 1).Replace("\n", "");

                        if (f1903Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ITEM_CODE == item_code) == null)
                            errorMessage += Properties.Resources.P160101Service_Item_Code_NotFound;
                    }

                    var f161202 = new F161202();
                    f161202.ITEM_CODE = item_code;
                    f161202.RTN_QTY = item.Key.RTN_QTY;
                    f161202s.Add(f161202);

                    //Log DataContent 欄位
                    dataContent = string.Format("{0},{1},{2},{3},{4},{5}"
                                                , groupItem.Key.CUST_ORD_NO, groupItem.Key.CONTACT, groupItem.Key.TEL
                                                , groupItem.Key.ADDRESS, groupItem.Key.MEMO, item.Key.ITEM_CODE);

                    UpdateF0060Log(fileName, "3", item_code, dataContent, errorMessage, dcCode, gupCode, custCode);

                    if (string.IsNullOrEmpty(errorMessage))
                        successCtn += 1;
                    else
                        errorCtn += 1;
                }
                #endregion

                if (string.IsNullOrEmpty(errorMessage))
                {
                    //寫入退貨主檔 
                    var f161201 = new F161201();
                    f161201.DC_CODE = dcCode;
                    f161201.GUP_CODE = gupCode;
                    f161201.CUST_CODE = custCode;
                    f161201.CUST_ORD_NO = groupItem.Key.CUST_ORD_NO;
                    f161201.CONTACT = groupItem.Key.CONTACT;
                    f161201.TEL = groupItem.Key.TEL;
                    f161201.ADDRESS = groupItem.Key.ADDRESS;
                    f161201.MEMO = groupItem.Key.MEMO;
                    f161201.ORD_PROP = "R1";
                    f161201.COST_CENTER = string.Empty;
                    f161201.DISTR_CAR = "Y";
                    f161201.RTN_TYPE_ID = "2";
                    f161201.RTN_CAUSE = "999";

                    InserF1612DataByGroup(f161201, f161202s.ToArray());
                }

                errorMessage = string.Empty;

            }

           
			result.Message = string.Format(Properties.Resources.P160101Service_ImportResult, importData.Count, successCtn, errorCtn);
			return result;
		}


        #region 新增退貨主檔-同貨主單號會合併
        private void InserF1612DataByGroup(F161201 addF161201, F161202[] addF161202s)
        {
            var f161201Repo = new F161201Repository(Schemas.CoreSchema, _wmsTransaction);
            var f161202Repo = new F161202Repository(Schemas.CoreSchema, _wmsTransaction);
            var sharedService = new SharedService();
            var newOrdCode = sharedService.GetNewOrdCode("R");
            F161201 f161201 = new F161201()
            {
                RETURN_NO = newOrdCode,
                RETURN_DATE = DateTime.Today,
                POSTING_DATE = null,
                STATUS = "0",
                CUST_ORD_NO = addF161201.CUST_ORD_NO,
                WMS_ORD_NO = "",
                RTN_CUST_CODE = string.IsNullOrEmpty(addF161201.RTN_CUST_CODE) || addF161201.RTN_CUST_CODE == "B2C" ? null : addF161201.RTN_CUST_CODE,
                RTN_CUST_NAME = string.IsNullOrEmpty(addF161201.RTN_CUST_CODE) || addF161201.RTN_CUST_CODE == "B2C" ? addF161201.CONTACT : addF161201.RTN_CUST_NAME,
                RTN_TYPE_ID = addF161201.RTN_TYPE_ID,
                RTN_CAUSE = addF161201.RTN_CAUSE,
                SOURCE_TYPE = null,
                SOURCE_NO = null,
                DISTR_CAR = addF161201.DISTR_CAR == "1" ? "1" : "0",
                COST_CENTER = addF161201.COST_CENTER,
                ADDRESS = addF161201.ADDRESS,
                CONTACT = addF161201.CONTACT,
                TEL = addF161201.TEL,
                MEMO = addF161201.MEMO,
                ORD_PROP = addF161201.ORD_PROP,
                DC_CODE = addF161201.DC_CODE,
                GUP_CODE = addF161201.GUP_CODE,
                CUST_CODE = addF161201.CUST_CODE
            };
            f161201Repo.Add(f161201);

            int seq = 0;
            foreach (var item in addF161202s)
            {
                seq++;
                item.RETURN_NO = newOrdCode;
                item.RETURN_SEQ = seq.ToString();
                item.ITEM_CODE = item.ITEM_CODE;
                item.RTN_QTY = item.RTN_QTY;
                item.DC_CODE = addF161201.DC_CODE;
                item.GUP_CODE = addF161201.GUP_CODE;
                item.CUST_CODE = addF161201.CUST_CODE;

                f161202Repo.Add(item);
            }
        }
        #endregion


    }
}