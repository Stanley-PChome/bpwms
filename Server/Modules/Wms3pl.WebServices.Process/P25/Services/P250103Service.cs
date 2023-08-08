
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P25.Services
{
	public partial class P250103Service
	{
		private WmsTransaction _wmsTransaction;
		public P250103Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		private bool IsEnglishOrNumber(string text)
		{
			return string.IsNullOrWhiteSpace(text) || Regex.IsMatch(text, @"^[a-zA-Z0-9]+$");
		}
    /// <summary>
    /// 檢核字元(英文字數字破折號及斜線){a-z,A-Z,0-9,-,/}
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private bool IsEnglishOrNumberOrDashOrSlash(string text)
    {
      return string.IsNullOrWhiteSpace(text) || Regex.IsMatch(text, @"^[a-zA-Z0-9-/]+$");
    }

    public bool IsNumber(string text)
		{
			return string.IsNullOrWhiteSpace(text) || Regex.IsMatch(text, @"^\d+$");
		}


    public F250103Verification InsertOrUpdate(F2501WcfData d, int index, out BulkUpdateF2501Result bulkUpdateF2501Result)
    {
      var f2501Repo = new F2501Repository(Schemas.CoreSchema);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema);
      var commonService = new CommonService();
      var serialNoService = new SerialNoService();
			serialNoService.CommonService = commonService;
			var result = new F250103Verification
			{
				ROWNUM = index,
				SerialNo = d.SERIAL_NO,
				Verification = Properties.Resources.P2501Service_NotPass,
				Status = d.STATUS
			};

      bulkUpdateF2501Result = new BulkUpdateF2501Result();

      try
      {

        #region 必填欄位檢查
        //業主、貨主、商品編號、序號、序號狀態、供應商編號
        if (string.IsNullOrWhiteSpace(d.GUP_CODE) || string.IsNullOrWhiteSpace(d.CUST_CODE) ||
            string.IsNullOrWhiteSpace(d.ITEM_CODE) || string.IsNullOrWhiteSpace(d.SERIAL_NO) ||
            string.IsNullOrWhiteSpace(d.STATUS) || string.IsNullOrWhiteSpace(d.VNR_CODE))
        {
          result.Message = Properties.Resources.P2501Service_RequiredColumnlack;
					return result;
				}
				#endregion

				#region 中文排除

				d.LOC_CODE = (d.LOC_CODE ?? string.Empty).Trim().Replace("-", string.Empty);

				if (!IsEnglishOrNumber(d.ITEM_CODE.Replace("-","")) ||
					!IsEnglishOrNumber(d.CELL_NUM) ||
					!IsEnglishOrNumber(d.WMS_NO) ||
					!IsEnglishOrNumber(d.BATCH_NO) ||
					!IsEnglishOrNumber(d.LOC_CODE)
					)
				{
					result.Message = Properties.Resources.P2501Service_CannotInputCorSpecialType;
					return result;
				}
        //20230331 #2149_1 因商品檢驗之序號收集 不擋序號字元
        //if (!IsEnglishOrNumberOrDashOrSlash(d.SERIAL_NO) )
        //{
        //  result.Message = "欄位請勿輸入英文字數字破折號及斜線以外字元!";
        //  //result.Message = Properties.Resources.P2501Service_CannotInputOutEnglishOrNumberOrDashOrSlashType;
        //  return result;
        //}

        #endregion



        #region BOX_SERIAL、BATCH_NO 檢查(盒號or儲值卡盒號) 
        //若有輸入，則要檢查F2501效期
        if ((string.IsNullOrWhiteSpace(d.BOX_SERIAL) && string.IsNullOrWhiteSpace(d.BATCH_NO)) == false)
        {
          List<F2501> f2501VaildDate;
          if (!string.IsNullOrWhiteSpace(d.BOX_SERIAL))
          {
            f2501VaildDate = f2501Repo.GetDatasByBoxSerial(d.GUP_CODE, d.CUST_CODE, d.BOX_SERIAL).ToList();
          }
          else if (!string.IsNullOrWhiteSpace(d.BATCH_NO))
          {
            f2501VaildDate = f2501Repo.GetDatasByBatchNo(d.GUP_CODE, d.CUST_CODE, d.BATCH_NO).ToList();
          }
          else
          {
            f2501VaildDate = f2501Repo.GetDatasByBatchNoAndBoxSerial(d.GUP_CODE, d.CUST_CODE, d.BOX_SERIAL, d.BATCH_NO).ToList();
          }


          if (f2501VaildDate.Any() && f2501VaildDate.Select(x => x.VALID_DATE).FirstOrDefault() != d.VALID_DATE)
          {
            result.Message = Properties.Resources.P2501Service_BatchNo_VALID_DATE_Incorrect;
            return result;
          }
        }
        #endregion

        #region 供應商檢查
        // 供應商可為 null
        if (!string.IsNullOrEmpty(d.VNR_CODE))
				{
					var f1908Data = f1908Repo.Find(x => x.GUP_CODE == d.GUP_CODE && x.VNR_CODE == d.VNR_CODE  && x.CUST_CODE == d.CUST_CODE);
					if (f1908Data == null)
					{
						result.Message = Properties.Resources.P2501Service_SupplyNotExist;
						return result;
					}
				}
        #endregion

        #region 序號綁儲位檢查
        if (d.BUNDLE_SERIALLOC == "1")
        {
          result.Message = "序號綁儲位商品，請用異動調整作業處理序號資料";
          return result;
        }
        #endregion

        string statusCheckItemCode = serialNoService.GetItemCodeBySerialNo(d.GUP_CODE, d.CUST_CODE, d.SERIAL_NO);


				if (d.IsChangeItemCode)
				{
					// 確定真的是要強制更變料號
					d.IsChangeItemCode = d.ITEM_CODE != statusCheckItemCode;
				}

				if (string.IsNullOrEmpty(statusCheckItemCode))
				{
					// 若找不到該序號的品號，表示是要匯入新的序號
					statusCheckItemCode = d.ITEM_CODE;
				}

        #region Status檢查
        // Status 狀態檢查 - ItemCode丟原本序號(不能丟強制變更料號的ItemCode)
        // 如果是更換品號，則要變更的狀態為原始狀態的話，就不檢查狀態
        var f2501 = commonService.GetItemSerialList(d.GUP_CODE, d.CUST_CODE, new[] { d.SERIAL_NO }.ToList()).FirstOrDefault();
          //f2501Repo.Find(o => o.GUP_CODE == d.GUP_CODE && o.CUST_CODE == d.CUST_CODE && o.SERIAL_NO == d.SERIAL_NO);
				var ignoreCheckOfStatus = f2501?.STATUS == d.STATUS ? d.STATUS : string.Empty;
        var checkResult = serialNoService.SerialNoStatusCheckAll(d.GUP_CODE, d.CUST_CODE, statusCheckItemCode, d.SERIAL_NO, d.STATUS, ignoreCheckOfStatus);
        if (!checkResult.Checked)
				{
					result.Message = checkResult.Message;
					return result;
				}
				// 若從F2501找不到該序號，則就不會有品號，表示是新序號
				if (string.IsNullOrEmpty(checkResult.ItemCode))
				{
					checkResult.ItemCode = statusCheckItemCode;

          if (!string.IsNullOrEmpty(d.CELL_NUM))
          {
            if (!IsNumber(d.CELL_NUM))
            {
              result.Message = Properties.Resources.P2501Service_Cell_Num_MustBeNumber;
              return result;
            }

            checkResult.CellNum = d.CELL_NUM;
          }

          if (!string.IsNullOrEmpty(d.BOX_SERIAL))
          {
            if (serialNoService.BarcodeInspection(d.GUP_CODE, d.CUST_CODE, d.BOX_SERIAL).Barcode != Wms3pl.Common.Enums.BarcodeType.BoxSerial)
            {
              result.Message = Properties.Resources.P2501Service_BoxSerialNo_FormatError;
              return result;
            }
            checkResult.BoxSerail = d.BOX_SERIAL;
          }

          if (!string.IsNullOrEmpty(d.BATCH_NO))
          {
            if (!serialNoService.IsBatchNoItem(d.GUP_CODE, d.CUST_CODE, statusCheckItemCode))
            {
              result.Message = Properties.Resources.P2501Service_ItemType_NotBatchNo;
              return result;
            }
            if (serialNoService.BarcodeInspection(d.GUP_CODE, d.CUST_CODE, d.BATCH_NO).Barcode != Wms3pl.Common.Enums.BarcodeType.BatchNo)
            {
              result.Message = Properties.Resources.P2501Service_BatchNo_FormatError;
              return result;
            }
            checkResult.BatchNo = d.BATCH_NO;
          }
        }

        if (f2501 != null)
        {
          if (f2501.STATUS == "A1")
          {
            result.Message = "序號已存在不可更新";
            return result;
          }
          if (f2501.STATUS == "C1")
          {
            if (f2501.ITEM_CODE != d.ITEM_CODE)
            {
              result.Message = $"此序號的品號為{f2501.ITEM_CODE}，與提供的品號不同，不可更新";
              return result;
            }
          }
        }
        #endregion

        var updateResult = serialNoService.UpdateSerialNoFullForBulk(d.DC_CODE, d.GUP_CODE, d.CUST_CODE, d.STATUS, checkResult, d.WMS_NO, d.VNR_CODE,
          d.VALID_DATE, ordProp: "J1");

        bulkUpdateF2501Result.f2501 = updateResult.f2501;
        bulkUpdateF2501Result.ModifyMode = updateResult.ModifyMode;
        bulkUpdateF2501Result.IsSuccessed = updateResult.IsSuccessed;

        //其實失敗就會return，所以此處根本只會有通過
        result.Verification = updateResult.IsSuccessed ? Properties.Resources.P2501Service_Pass : Properties.Resources.P2501Service_Fail;
				result.Message = updateResult.Message;

			}
			catch (Exception ex)
			{
				result.Verification = Properties.Resources.P2501Service_NotPass;
				result.Message = ex.Message;
			}

			return result;
		}

		/// <summary>
		/// 檢查序號綁儲位的欄位是否有錯誤訊息
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		private string CheckFieldsByBundleSerialLoc(F2501WcfData d)
		{
			var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1980Repo = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);

			// 不是序號綁儲位就不用檢查
			if (d.BUNDLE_SERIALLOC != "1")
				return string.Empty;

			if (!d.VALID_DATE.HasValue)
				return Properties.Resources.P2501Service_LOC_CODE_VliadDate_Required;

			if (string.IsNullOrEmpty(d.LOC_CODE))
				return Properties.Resources.P2501Service_LOC_CODE_Required;

			if (string.IsNullOrEmpty(d.WAREHOUSE_ID))
				return Properties.Resources.P2501Service_WarehouseId_Required;

			var f1912Data = f1912Repo.Find(x => x.DC_CODE == d.DC_CODE && x.LOC_CODE == d.LOC_CODE);
			if (f1912Data == null)
				return string.Format(Properties.Resources.P2501Service_LOC_CODE_NotExist, d.LOC_CODE);

			if (f1912Data.WAREHOUSE_ID != d.WAREHOUSE_ID)
				return string.Format(Properties.Resources.P2501Service_LOC_CODE_NotExistAt, d.LOC_CODE, f1980Repo.GetDatasByTrueAndCondition(x => x.WAREHOUSE_ID == d.WAREHOUSE_ID && x.DC_CODE == d.DC_CODE).Select(x => x.WAREHOUSE_NAME).FirstOrDefault());

			return string.Empty;
		}

		private void UpdateF1913ItemCode(F2501WcfData d)
		{
			// 若只是變更品號，則將存在F1913的庫存都刪除
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			f1913Repo.UpdateFields(SET: new { ITEM_CODE = d.ITEM_CODE },
								   WHERE: x => x.DC_CODE == d.DC_CODE
										&& x.GUP_CODE == d.GUP_CODE
										&& x.CUST_CODE == d.CUST_CODE
										&& x.SERIAL_NO == d.SERIAL_NO);
		}

		/// <summary>
		/// 匯入序號成功與否，皆會記錄
		/// </summary>
		public void SerialNoLog(F2501WcfData d, bool isPass, string message)
		{
      if (string.IsNullOrWhiteSpace(d.GUP_CODE) || string.IsNullOrWhiteSpace(d.CUST_CODE))
        return;

      var f250103Repo = new F250103Repository(Schemas.CoreSchema, _wmsTransaction);
			var f250103 = new F250103
			{
				SERIAL_NO = d.SERIAL_NO,
				STATUS = d.STATUS,
				ISPASS = isPass ? "1" : "0",
				MESSAGE = message,
				GUP_CODE = d.GUP_CODE,
				CUST_CODE = d.CUST_CODE
			};
			f250103Repo.Add(f250103);
		}

		private IQueryable<F1903> CreateF1903Query(string itemCode, string gupCode, string custCode)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			return f1903Repo.Filter(x => x.ITEM_CODE == EntityFunctions.AsNonUnicode(itemCode)
									&& x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
                                    && x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode));
		}

		private void InsertF1913ByF2501(F2501WcfData data)
		{
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1913InsertData = new F1913
			{
				DC_CODE = data.DC_CODE,
				GUP_CODE = data.GUP_CODE,
				CUST_CODE = data.CUST_CODE,
				LOC_CODE = data.LOC_CODE,
				ITEM_CODE = data.ITEM_CODE,
				VALID_DATE = data.VALID_DATE.Value.Date,
				ENTER_DATE = DateTime.Today,
				VNR_CODE = "000000",
				SERIAL_NO = data.SERIAL_NO,
				QTY = 1,
				BOX_CTRL_NO = string.IsNullOrEmpty(data.BOX_CTRL_NO) ? "0" : data.BOX_CTRL_NO,
				PALLET_CTRL_NO = string.IsNullOrEmpty(data.PALLET_CTRL_NO) ? "0" : data.PALLET_CTRL_NO,
				MAKE_NO = data.MAKE_NO
				//REMARK = data.REMARK,
			};
			f1913Repo.Add(f1913InsertData);
		}

		public void DeleteF1913ByF2501(F2501WcfData data)
		{
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			f1913Repo.Delete(x => x.SERIAL_NO == data.SERIAL_NO && x.GUP_CODE == data.GUP_CODE && x.CUST_CODE == data.CUST_CODE);
		}

		
	}
}

