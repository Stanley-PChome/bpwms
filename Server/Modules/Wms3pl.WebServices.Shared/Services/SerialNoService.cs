using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Wms3pl.Common.Enums;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
	public partial class SerialNoService
	{
		private WmsTransaction _wmsTransaction;
		private CommonService _commonSerivce;
		public CommonService CommonService
		{
			get {
				if (_commonSerivce == null)
					_commonSerivce = new CommonService();
				return _commonSerivce;
			}
			set
			{
				_commonSerivce = value;
			}
		}

		#region Repository
		private F1903Repository _f1903Repo = null;
		private F2501Repository _f2501Repo = null;
		private F000904Repository _f000904Repo = new F000904Repository(Schemas.CoreSchema);
		private F192404Repository _f192404Repo = new F192404Repository(Schemas.CoreSchema);
		#endregion

		#region 凍結序號快取
		private List<F250102Data> f250102Cahces = null;
		#endregion

		#region 序號狀態表
		private List<F000904Data> _f000904DataList = null;

		public List<F000904Data> F000904DataList
		{
			get
			{
				if (_f000904DataList == null)
					_f000904DataList = _f000904Repo.GetF000904Data("F2501", "STATUS").ToList();

				return _f000904DataList;
			}
		}
		#endregion


		public SerialNoService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
			_f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			_f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
		}

		#region Common
		/// <summary>
		/// 取得商品名稱
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="itemCode">商品編號</param>
		/// <returns>商品名稱</returns>
		public string GetItemName(string gupCode, string custCode, string itemCode)
		{
			var f1903 = CommonService.GetProduct(gupCode, custCode, itemCode);
			return f1903 == null ? string.Empty : f1903.ITEM_NAME;
		}

		#endregion

		#region 判斷條碼
		/// <summary>
		/// 條碼格式檢查
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="barcode">英數字條碼</param>
		/// <returns>儲值卡盒號:0 ,盒號:1 ,箱號:2 ,序號:3</returns>
		public BarcodeData BarcodeInspection(string gupCode, string custCode, string barcode)
		{
			var returnCodeType = BarcodeType.None;
			if (!string.IsNullOrEmpty(gupCode) && !string.IsNullOrEmpty(custCode) && !string.IsNullOrEmpty(barcode))
			{
				//switch (barcode.Trim().Length)
				//{
				//	case 14:
				//		if (barcode.StartsWith("BOX"))
				//			returnCodeType = BarcodeType.BoxSerial; //盒號
				//		else if (barcode.StartsWith("GOX"))
				//			returnCodeType = BarcodeType.CaseNo;    //箱號
				//		else if (barcode.Trim().EndsWith("000")) //儲值卡盒號格式固定後三碼為0
				//			returnCodeType = BarcodeType.BatchNo;
				//		else
				//			returnCodeType = BarcodeType.SerialNo;  //序號
				//		break;
				//	default:
				//		if (barcode.Trim().EndsWith("000")) //儲值卡盒號格式固定後三碼為0
				//			returnCodeType = BarcodeType.BatchNo;
				//		else
				//			returnCodeType = BarcodeType.SerialNo;  //序號
				//		break;
				//}
				returnCodeType = BarcodeType.SerialNo;  //序號
			}
			return new BarcodeData(returnCodeType);
		}


		#endregion

		#region 序號檢查
		public string GetItemCodeByEntity(F2501 f2501)
		{
			if (f2501 == null)
				return string.Empty;

			if (!string.IsNullOrWhiteSpace(f2501.BOUNDLE_ITEM_CODE))
				return f2501.BOUNDLE_ITEM_CODE;

			return f2501.ITEM_CODE;
		}

		public string GetItemCodeBySerialNo(string gupCode, string custCode, string serialNo)
		{
			var f2501 = CommonService.GetItemSerialList(gupCode, custCode, new[] { serialNo }.ToList()).FirstOrDefault();
			//_f2501Repo.Find(x => x.GUP_CODE.Equals(gupCode) && x.CUST_CODE.Equals(custCode) && x.SERIAL_NO.Equals(serialNo));
			return GetItemCodeByEntity(f2501);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="serialNo"></param>
		/// <param name="itemName"></param>
		/// <returns></returns>
		public SerialNoResult CheckSeralNoBase(string gupCode, string custCode, string itemCode, string serialNo, string itemName = "")
		{
			const string trueText = "0";
			const string falseText = "1";
			var result = new SerialNoResult
			{
				Checked = false,
				ItemCode = itemCode,
				ItemName = string.IsNullOrEmpty(itemName) ? GetItemName(gupCode, custCode, itemCode) : itemName,
				SerialNo = serialNo,
				Message = string.Empty,
				CurrentlyStatus = string.Empty
			};
			//查詢該商品的序號設定資料
			if (string.IsNullOrWhiteSpace(serialNo))
			{
				result.Message = "序號不能為空!";
				return result;
			}
			var serialNoSetting = CommonService.GetProduct(gupCode, custCode, itemCode);

			#region 設定檔資料 檢查
			if (serialNoSetting == null)
			{
				result.Message = "該序號查無設定資料!";
				return result;
			}

      #endregion

      #region 序號商品 檢查
      //if (serialNoSetting.BUNDLE_SERIALNO != "1")
      //{
      //    result.Message = string.Format("該料號{0}不是序號商品!", itemCode);
      //    return result;
      //}
      #endregion

      #region 序號碼數 檢查

      if (serialNoSetting.SERIALNO_DIGIT != null)
			{
				if (serialNo.Trim().Length != serialNoSetting.SERIALNO_DIGIT)
				{
					result.Message = string.Format("序號碼數需為 {0}碼!", serialNoSetting.SERIALNO_DIGIT);
					return result;
				}
			}

			if(serialNo.Trim().Length > 50)
			{
				result.Message = string.Format("序號長度不可超過50碼", 50);
				return result;
			}

			#endregion

			#region 序號為純數/非純數 檢查

			if (!string.IsNullOrWhiteSpace(serialNoSetting.SERIAL_RULE))
			{
				switch (serialNoSetting.SERIAL_RULE)
				{
					case trueText: //需為純數
						if (!IsNumber(serialNo))
						{
							result.Message = "序號需為純數字!";
							return result;
						}
						break;
					case falseText: //不需為純數
						break;
					default:
						//不應該設定除此之外的值
						break;
				}
			}//else -> SERIAL_RULE - null代表不用判斷規則

			#endregion

			#region 序號開頭 檢查

			if (!string.IsNullOrWhiteSpace(serialNoSetting.SERIAL_BEGIN))
			{
				if (serialNo.StartsWith(serialNoSetting.SERIAL_BEGIN))//符合開頭
				{

				}
				else//不符合開頭
				{
					result.Message = string.Format("序號開頭需為 {0} !", serialNoSetting.SERIAL_BEGIN);
					return result;
				}
			}//else SERIAL_BEGIN - null代表不用判斷開頭

			#endregion
			result.Checked = true;
			return result;//至此檢查成功，回傳
		}

		/// <summary>
		/// 序號匯入檢查(包含序號設定)
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="itemCode">商品編號</param>
		/// <param name="serialNo">商品序號</param>
		/// <param name="status">變更後序號狀態</param>
		/// <param name="ignoreCheckOfStatus">忽略檢查的狀態</param>
		/// <param name="f2501s">此方法若在其他迴圈中被呼叫，此迴圈所有所需的f2501的物件集合(於迴圈外先一起取回的f2501的物件集合)</param>
		/// <returns>StringArray: 0-商品序號, 1-序號狀態, 2-品號, 3-品名, 4-是否通過(1/0), 5-錯誤訊息</returns>
		public SerialNoResult SerialNoStatusCheckAll(string gupCode, string custCode, string itemCode, string serialNo, string status, string ignoreCheckOfStatus = "", IEnumerable<F2501> f2501s = null)
		{
			var result = new SerialNoResult
			{
				Checked = false,
				ItemCode = itemCode,
				ItemName = GetItemName(gupCode, custCode, itemCode),
				SerialNo = serialNo,
				Message = string.Empty,
				CurrentlyStatus = string.Empty
			};

			#region 輸入資料基本檢查
		
      if (string.IsNullOrWhiteSpace(status))
			{
				result.Message = "請輸入欲變更狀態!";
				return result;
			}
			#endregion

			result = CheckSeralNoBase(gupCode, custCode, itemCode, serialNo, result.ItemName);
			if (!result.Checked)
				return result;

			#region 變更序號狀態檢查

			var checkStatus = SerialNoStatusCheck(gupCode, custCode, serialNo, status, ignoreCheckOfStatus, result.ItemName, f2501s);
			if (!checkStatus.Checked)
				return checkStatus;
			#endregion

			result.BatchNo = checkStatus.BatchNo;
			result.CurrentlyStatus = checkStatus.CurrentlyStatus;
			result.Puk = checkStatus.Puk;
			result.CellNum = checkStatus.CellNum;
			result.BoxSerail = checkStatus.BoxSerail;
			result.CaseNo = checkStatus.CaseNo;
			result.CombinNo = checkStatus.CombinNo;
			result.Checked = checkStatus.Checked;
			result.BatchNo = checkStatus.BatchNo;
			result.Message = checkStatus.Message;

			return result;
		}

		private SerialNoResult CreateNewSerialNoResult()
		{
			return new SerialNoResult
			{
				Checked = false,
				CurrentlyStatus = string.Empty,
				ItemCode = string.Empty,
				ItemName = string.Empty,
				SerialNo = string.Empty,
				BoxSerail = string.Empty,
				CaseNo = string.Empty,
				CellNum = string.Empty,
				Puk = string.Empty,
			};
		}

		private string GetSerialNoStatusErrorMsg(string serialNo, string orginalStatus, string status)
		{
			//查詢[狀態代碼]對照表
			var statusData = F000904DataList;
			var statusName = statusData.FirstOrDefault(x => x.VALUE == status) != null
																					? statusData.First(x => x.VALUE == status).NAME
																					: string.Empty;

			return string.Format(@"序號{0}({1})商品狀態不可轉為{2}({3})!", serialNo, orginalStatus, statusName, status);
		}

		/// <summary>
		/// 序號狀態變更檢查
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="serialNo">序號</param>
		/// <param name="status">欲變更的狀態</param>
		/// <param name="ignoreCheckOfStatus">忽略檢查的狀態</param>
		/// <param name="itemName">序號商品品名(避免Function重複查詢品名)</param>
		/// <param name="f2501s">此方法若在其他迴圈中被呼叫，此迴圈所有所需的f2501的物件集合(於迴圈外先一起取回的f2501的物件集合)</param>
		/// <returns></returns>
		public SerialNoResult SerialNoStatusCheck(string gupCode, string custCode, string serialNo, string status, string ignoreCheckOfStatus = "", string itemName = "", IEnumerable<F2501> f2501s = null)
		{
			//查詢序號原本狀態資料
			F2501 serialNoData;
			if (f2501s != null)
			{
				serialNoData = f2501s.FirstOrDefault(x => x.GUP_CODE.Equals(gupCode)
																				&& x.CUST_CODE.Equals(custCode)
																				&& x.SERIAL_NO.Equals(serialNo, StringComparison.CurrentCultureIgnoreCase));
			}
			else
			{
				serialNoData = CommonService.GetItemSerialList(gupCode, custCode, new[] { serialNo }.ToList()).FirstOrDefault();
				//_f2501Repo.GetF2501s(gupCode, custCode, new string[] { serialNo }).FirstOrDefault();

				//serialNoData = _f2501Repo.Find(x => x.GUP_CODE.Equals(gupCode)
				//                                && x.CUST_CODE.Equals(custCode)
				//                                && x.SERIAL_NO.Equals(serialNo, StringComparison.CurrentCultureIgnoreCase));
			}

			var result = SerialNoStatusCheckImpl(serialNoData, serialNo, status, ignoreCheckOfStatus, itemName);

			//if (result.Checked)
			//{
			//	// 檢查序號是否有凍結
			//	var checkSerialFreezeResults = CheckSerialFreezeByStatusChange(gupCode, custCode, status, new List<string> { serialNo });
			//	if (checkSerialFreezeResults.Any())
			//	{
			//		result.Checked = false;
			//		result.Message = GetFreezeMsg(checkSerialFreezeResults.Select(x => x.SERIAL_NO).FirstOrDefault());
			//	}
			//}

			return result;
		}

		public List<SerialNoResult> SerialNoStatusCheck(string gupCode, string custCode, List<string> serialNos, string status, string ignoreCheckOfStatus = "", string itemName = "")
		{
			var f2501s = CommonService.GetItemSerialList(gupCode, custCode, serialNos);
			var results = f2501s.Select(serialNoData => SerialNoStatusCheckImpl(serialNoData, serialNoData.SERIAL_NO, status, ignoreCheckOfStatus, itemName))
													.ToList();

			//if (results.Any(x => x.Checked))
			//{
			//	// 檢查序號是否有凍結
			//	var checkSerialFreezeResults = CheckSerialFreezeByStatusChange(gupCode, custCode, status, serialNos);

			//	checkSerialFreezeResults.ForEach(f =>
			//	{
			//		var result = results.FirstOrDefault(x => x.Checked && x.SerialNo == f.SERIAL_NO);
			//		if (result == null)
			//			return;

			//		result.Checked = false;
			//		result.Message = GetFreezeMsg(f.SERIAL_NO);
			//	});
			//}

			return results;
		}

		/// <summary>
		/// 序號狀態檢查實作，只包含狀態的檢核
		/// </summary>
		/// <param name="serialNoData"></param>		
		/// <param name="serialNo"></param>
		/// <param name="status"></param>
		/// <param name="ignoreCheckOfStatus"></param>
		/// <param name="itemName"></param>
		/// <returns></returns>
		private SerialNoResult SerialNoStatusCheckImpl(F2501 serialNoData, string serialNo, string status, string ignoreCheckOfStatus = "", string itemName = "")
		{
			var result = CreateNewSerialNoResult();
			result.SerialNo = serialNo;

			#region 變更序號狀態檢查
			if (serialNoData != null)
			{
				result.ItemCode = serialNoData.ITEM_CODE;
				result.ItemName = itemName;
				result.CurrentlyStatus = serialNoData.STATUS;
				result.Puk = serialNoData.PUK;
				result.CellNum = serialNoData.CELL_NUM;
				result.BatchNo = serialNoData.BATCH_NO;
				result.BoxSerail = serialNoData.BOX_SERIAL;
				result.CaseNo = serialNoData.CASE_NO;
				result.CombinNo = serialNoData.COMBIN_NO;

				if (ignoreCheckOfStatus != null && ignoreCheckOfStatus.Contains(serialNoData.STATUS))
				{
					result.Checked = true;
					return result;
				}

				switch (status)//欲變更狀態
				{
					case "A1"://進貨
						if ((string.IsNullOrWhiteSpace(serialNoData.STATUS) || serialNoData.STATUS == "C1") == false)
						{
							result.Message = GetSerialNoStatusErrorMsg(serialNo, serialNoData.STATUS, status);
							return result;
						}
						break;
					case "C1"://出貨
						if ((serialNoData.STATUS == "A1") == false)
						{
							result.Message = GetSerialNoStatusErrorMsg(serialNo, serialNoData.STATUS, status);
							return result;
						}
						break;
					case "D2"://報廢
						if ((serialNoData.STATUS == "A1") == false)
						{
							result.Message = GetSerialNoStatusErrorMsg(serialNo, serialNoData.STATUS, status);
							return result;
						}
						break;
					default:
						result.Message = "查無符合變更後之序號狀態!";
						return result;
				}

				result.Message = string.Empty;//若無異常狀態直接return，代表驗證通過
			}
			else
			{
				if (status != "A1") //更改為進貨或調入可允許通過
				{
					result.Message = "查無符合序號之商品!";
					return result;
				}
			}
			#endregion

			result.Checked = true;
			return result;//至此檢查成功，回傳
		}

		/// <summary>
		/// 由即將要改變的狀態來檢查序號是否已凍結
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="serialNo"></param>
		/// <param name="statusChange"></param>
		/// <returns></returns>
		public List<F250102Data> CheckSerialFreezeByStatusChange(string gupCode, string custCode, string statusChange, IEnumerable<string> serialNos)
		{
			var control = ConvertStatusToControl(statusChange);
			if (string.IsNullOrEmpty(control))
				return new List<F250102Data>();

			return CheckSerialIsFreeze(gupCode, custCode, control, serialNos);
		}

		private string GetFreezeMsg(string serialNo)
		{
			return string.Format(@"序號{0}商品已凍結! ", serialNo);
		}

		/// <summary>
		/// 轉換狀態為管制作業代碼
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		private string ConvertStatusToControl(string status)
		{
			switch (status)//欲變更狀態
			{
				case "B1"://調出
					return "03";
				case "B2"://調入
					return "03";
				case "E1"://加工
					return "05";
				case "C1"://出貨
					return "02";
				case "C2"://客退
					return "04";
			}

			return string.Empty;
		}

		/// <summary>
		/// 檢查是否被凍結   true:已凍結
		/// </summary>
		/// <param name="controlType">管制作業(01進項02銷項03調撥04退貨05加工)(F000904)</param>
		/// <returns></returns>
		public List<F250102Data> CheckSerialIsFreeze(string gupCode, string custCode, string controlType, IEnumerable<string> serialNos)
		{
			if (f250102Cahces == null)
				f250102Cahces = new List<F250102Data>();

			// 若有一個序號沒有在凍結快取紀錄中
			var notExistsFreezeSerialNos = serialNos.Where(serialNo => !f250102Cahces.Any(x => x.GUP_CODE == gupCode
																																											&& x.CUST_CODE == custCode
																																											&& x.SERIAL_NO == serialNo
																																											&& x.CONTROL == controlType)).ToList();
			// 那就得重撈這些沒在快取序號是否有凍結紀錄
			if (notExistsFreezeSerialNos.Any())
			{
				var f250102Datas = _f2501Repo.GetSerialIsFreeze(gupCode, custCode, controlType, notExistsFreezeSerialNos);
				f250102Cahces.AddRange(f250102Datas);
			}

			// 回傳有被凍結的序號紀錄
			return f250102Cahces.Where(x => x.GUP_CODE == gupCode
																	 && x.CUST_CODE == custCode
																	 && serialNos.Contains(x.SERIAL_NO)
																	 && x.CONTROL == controlType)
													.ToList();
		}

		/// <summary>
		/// 檢查字串是否為數字(序號可能位數過大，故特意拆字元檢查)
		/// </summary>
		private bool IsNumber(string text)
		{
			return text.ToCharArray().All(char.IsNumber);
		}

		/// <summary>
		/// 序號、盒號、儲值卡盒號、箱號取得商品
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="barCode">序號、盒號、儲值卡盒號、箱號</param>
		/// <returns></returns>
		public SerialNoResult GetSerialItem(string gupCode, string custCode, string barCode, bool isCombinCheck = false)
		{
			var serialNoResult = CreateNewSerialNoResult();
			//取得此序號類型
			var barcodeData = BarcodeInspection(gupCode, custCode, barCode);
			F2501 f2501 = null;
			switch (barcodeData.Barcode)
			{
				case BarcodeType.BatchNo:
					f2501 = _f2501Repo.GetFirstDataByBatchNo(gupCode, custCode, barCode);
					if (f2501 != null)
						serialNoResult.BatchNo = barCode;
					break;
				case BarcodeType.BoxSerial:
					f2501 = _f2501Repo.GetFirstDataByBoxSerial(gupCode, custCode, barCode);
					if (f2501 != null)
						serialNoResult.BoxSerail = barCode;
					break;
				case BarcodeType.CaseNo:
					f2501 = _f2501Repo.GetFirstDataByCaseNo(gupCode, custCode, barCode);
					if (f2501 != null)
						serialNoResult.CaseNo = barCode;
					break;
			}
			if (f2501 == null)
			{
				f2501 = _f2501Repo.Find(
										o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.SERIAL_NO == barCode);
			}
			if (f2501 != null)
			{
				if (isCombinCheck)
				{
					var combinF2501List = new List<F2501>();
					var combinItemCode = string.Empty;
					bool isCombinItem = IsCombinItem(gupCode, custCode, barCode, out combinF2501List, out combinItemCode);
					if (isCombinItem)
					{
						serialNoResult.ItemCode = combinItemCode;
						serialNoResult.ItemName = GetItemName(f2501.GUP_CODE, f2501.CUST_CODE, combinItemCode);
						serialNoResult.Checked = true;
						return serialNoResult;
					}
				}
				serialNoResult.ItemCode = !string.IsNullOrWhiteSpace(f2501.BOUNDLE_ITEM_CODE) ? f2501.BOUNDLE_ITEM_CODE : f2501.ITEM_CODE;
				serialNoResult.ItemName = GetItemName(f2501.GUP_CODE, f2501.CUST_CODE, serialNoResult.ItemCode);
				serialNoResult.Checked = true;
			}
			else
				serialNoResult.Message = barcodeData.Barcode == BarcodeType.None ? barcodeData.BarcodeText : string.Format("無此{0}商品", barcodeData.BarcodeText);
			return serialNoResult;
		}

		/// <summary>
		/// 序號、盒號、儲值卡盒號、箱號檢核
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="itemCode">品號</param>
		/// <param name="barCode">序號、盒號、儲值卡盒號、箱號</param>
		/// <param name="status">變更後序號狀態</param>
		/// <param name="actionNo">加工動作:加工作業裝箱/盒是否需檢查此序號是否已綁定盒號或箱號</param>
		/// <param name="ignoreCheckOfStatus">忽略檢查的狀態</param>
		/// <param name="f2501s">此方法若在其他迴圈中被呼叫，此迴圈所有所需的f2501的物件集合(於迴圈外先一起取回的f2501的物件集合)</param>
		/// <returns>回傳序號檢核結果</returns>
		public List<SerialNoResult> CheckSerialNoFull(string dcCode, string gupCode, string custCode, string itemCode, string barCode, string status, ProcessWork actionNo = ProcessWork.ScanSerial, string ignoreCheckOfStatus = "", IEnumerable<F2501> f2501s = null)
		{
			//取得此序號類型
			var barcodeData = BarcodeInspection(gupCode, custCode, barCode);
			var seralNoResultList = new List<SerialNoResult>();
			var serialList = new List<F2501>();
			var batchNo = string.Empty;
			var boxSerial = string.Empty;
			var caseNo = string.Empty;
			//如果是序號是儲值卡盒號規則 但是商品非儲值卡 就視為是序號
			if (barcodeData.Barcode == BarcodeType.BatchNo && !IsBatchNoItem(gupCode, custCode, itemCode))
				barcodeData.Barcode = BarcodeType.SerialNo;
			switch (barcodeData.Barcode)
			{
				case BarcodeType.BatchNo:
					batchNo = barCode;
					serialList = _f2501Repo.GetDatasByBatchNo(gupCode, custCode, batchNo).ToList();
					if (!serialList.Any() && !(status == "C1" || status == "D2"))
						for (int i = 1; i <= 200; i++)
							serialList.Add(new F2501()
							{
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								ITEM_CODE = itemCode,
								SERIAL_NO = batchNo.Trim().Substring(0, batchNo.Trim().Length - 3) + i.ToString().PadLeft(3, '0'),
								BATCH_NO = barCode
							});
					break;
				case BarcodeType.BoxSerial:
					boxSerial = barCode;
					serialList = _f2501Repo.GetDatasByBoxSerial(gupCode, custCode, boxSerial).ToList();
					break;
				case BarcodeType.CaseNo:
					caseNo = barCode;
					serialList = _f2501Repo.GetDatasByCaseNo(gupCode, custCode, caseNo).ToList();
					break;
				case BarcodeType.SerialNo:
					serialList.Add(new F2501() { GUP_CODE = gupCode, CUST_CODE = custCode, ITEM_CODE = itemCode, SERIAL_NO = barCode });
					break;
				case BarcodeType.None:
					break;
			}
			if (serialList.Any())
			{
				foreach (var serialNo in serialList)
				{
					SerialNoResult serialNoResult = CreateNewSerialNoResult();
					serialNoResult.SerialNo = barCode;
					serialNoResult.ItemCode = itemCode;
					serialNoResult.BatchNo = batchNo;
					serialNoResult.BoxSerail = boxSerial;
					serialNoResult.CaseNo = caseNo;

					F2501 f2501;
					if (f2501s != null)
					{
						f2501 = f2501s.FirstOrDefault(x => x.GUP_CODE.Equals(gupCode) && x.CUST_CODE.Equals(custCode) && x.SERIAL_NO.Equals(serialNo.SERIAL_NO));
					}
					else
					{
						f2501 = CommonService.GetItemSerialList(gupCode, custCode, new List<string> { serialNo.SERIAL_NO }).FirstOrDefault();
					}
					var serialItemCode = GetItemCodeByEntity(f2501);

					if (barcodeData.Barcode == BarcodeType.SerialNo && string.IsNullOrEmpty(itemCode) && string.IsNullOrEmpty(serialItemCode))
					{
						seralNoResultList.Clear();
						serialNoResult.Message = "查無此序號的商品";
						seralNoResultList.Add(serialNoResult);
						break;
					}
					if (!string.IsNullOrEmpty(serialItemCode) && !string.IsNullOrEmpty(itemCode) && serialItemCode != itemCode)
					{
						seralNoResultList.Clear();
						serialNoResult.Message = barcodeData.Barcode == BarcodeType.None ? barcodeData.BarcodeText : string.Format("序號{0}({1})非本商品{2}(原品號為{3})", serialNo.SERIAL_NO, (f2501 != null ? f2501.STATUS : ""), barcodeData.BarcodeText, serialItemCode);
						seralNoResultList.Add(serialNoResult);
						break;
					}
					//有可能參數預設無品號,因此找到品號後更新
					if (!string.IsNullOrEmpty(serialItemCode))
					{
						serialNoResult.ItemCode = serialItemCode;
						itemCode = serialItemCode;
					}

					serialNoResult = SerialNoStatusCheckAll(gupCode, custCode, itemCode, serialNo.SERIAL_NO, status, ignoreCheckOfStatus, f2501s);
					if (!serialNoResult.Checked)
					{
						seralNoResultList.Clear();
						serialNoResult.SerialNo = barCode;
						seralNoResultList.Add(serialNoResult);
						break;
					}
					else if (barcodeData.Barcode == BarcodeType.BatchNo)
					{
						serialNoResult.BatchNo = batchNo;
					}

					#region 加工作業裝箱/盒需檢查此序號是否已綁定盒號或箱號
					if (actionNo == ProcessWork.Boxing || actionNo == ProcessWork.Goxing)
					{
						//刷讀
						if (barcodeData.Barcode == BarcodeType.SerialNo)
						{
							if (!string.IsNullOrEmpty(serialNoResult.BoxSerail))
							{
								seralNoResultList.Clear();
								serialNoResult.Checked = false;
								serialNoResult.SerialNo = barCode;
								serialNoResult.Message = "已綁定盒號，不可裝盒";
								seralNoResultList.Add(serialNoResult);
								break;
							}
						}
						else if (barcodeData.Barcode == BarcodeType.SerialNo || barcodeData.Barcode == BarcodeType.BoxSerial)
						{
							if (!string.IsNullOrEmpty(serialNoResult.CaseNo))
							{
								seralNoResultList.Clear();
								serialNoResult.Checked = false;
								serialNoResult.SerialNo = barCode;
								serialNoResult.Message = "已綁定箱號，不可裝盒/裝箱";
								seralNoResultList.Add(serialNoResult);
								break;
							}
						}
					}
					#endregion

					#region 加工組合商品需檢查此序號是否無組合過
					if (actionNo == ProcessWork.CombinItem)
					{
						if (serialNoResult.CombinNo != null)
						{
							seralNoResultList.Clear();
							serialNoResult.Checked = false;
							serialNoResult.SerialNo = barCode;
							serialNoResult.Message = "此序號已組合，不可再次組合";
							seralNoResultList.Add(serialNoResult);
							break;
						}
					}
					#endregion

					#region 加工拆解商品需檢查此序號是否已組合過
					if (actionNo == ProcessWork.Disassemble)
					{
						if (serialNoResult.CombinNo == null)
						{
							seralNoResultList.Clear();
							serialNoResult.Checked = false;
							serialNoResult.SerialNo = barCode;
							serialNoResult.Message = "無組合編號，不可拆解";
							seralNoResultList.Add(serialNoResult);
							break;
						}
					}
					#endregion

					seralNoResultList.Add(serialNoResult);
				}
			}
			else
			{
				var serialNoResult = CreateNewSerialNoResult();
				serialNoResult.SerialNo = barCode;
				serialNoResult.Message = barcodeData.Barcode == BarcodeType.None ? barcodeData.BarcodeText : string.Format("無此{0}", barcodeData.BarcodeText);
				seralNoResultList.Add(serialNoResult);
			}
			return seralNoResultList;
		}

		/// <summary>
		/// 大量序號、盒號、儲值卡盒號、箱號檢核
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="itemCode">品號</param>
		/// <param name="barCode">序號、盒號、儲值卡盒號、箱號</param>
		/// <param name="status">變更後序號狀態</param>
		/// <param name="actionNo">加工動作:加工作業裝箱/盒是否需檢查此序號是否已綁定盒號或箱號</param>
		/// <param name="ignoreCheckOfStatus">忽略檢查的狀態</param>
		/// <returns>回傳序號檢核結果</returns>
		public IEnumerable<SerialNoResult> CheckLargeSerialNoFull(string dcCode, string gupCode, string custCode,
																														 string itemCode, string[] largeSerialNo,
																														 string status, ProcessWork actionNo = ProcessWork.ScanSerial,
																														 string ignoreCheckOfStatus = "")
		{
			var f2501s = CommonService.GetItemSerialList(gupCode, custCode, largeSerialNo.ToList());

			foreach (var serialNo in largeSerialNo)
			{
				var serialNoResults = CheckSerialNoFull(dcCode, gupCode, custCode,
																								itemCode, serialNo, status,
																								actionNo, ignoreCheckOfStatus, f2501s);
				foreach (var serialNoResult in serialNoResults)
				{
					// 使用迭代回傳處理大量結果，可避免瞬間被占住大量記憶體
					yield return serialNoResult;
				}
			}
		}
		#endregion

		#region 序號更新(請先序號檢核通過在執行更新)

		/// <summary>
		/// 序號更新 (請先序號檢核通過在執行更新)
		/// </summary>
		/// <param name="dcCode">DC(為了取得工作站與錄影台設定檔,可空值)</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="status">變更後序號狀態</param>
		/// <param name="serialNoResult">序號檢核結果</param>
		/// <param name="wmsNo">系統單號</param>
		/// <param name="vnrCode">廠商F1908</param>
		/// <param name="validDate">效期</param>
		/// <param name="combinNo">組合編號(null表示不更新,0表示清空)</param>
		/// <param name="poNo">PO單號</param>
		/// <param name="ordProp">作業類別F000903</param>
		/// <param name="retailCode">客戶編號</param>
		/// <param name="overwritePuk">若有填，則會覆寫PUK</param>
		/// <param name="overwriteCellNum">若有填，則會覆寫門號</param>
		/// <param name="overwriteBatchNo">若有填，則會覆寫儲值卡盒號</param>
		/// <param name="boundleItemCode">加工組合成品編號</param>
		/// <returns></returns>
		public ExecuteResult UpdateSerialNoFull(ref List<F2501> addF2501s, ref List<F2501> updF2501s, ref List<string> delSnLists,string dcCode, string gupCode, string custCode, string status,
				SerialNoResult serialNoResult, string wmsNo, string vnrCode, DateTime? validDate, long? combinNo = null,
				string poNo = null, string ordProp = null, string retailCode = null, string overwritePuk = null,
				string overwriteCellNum = null, string overwriteBatchNo = null, string boundleItemCode = null, Boolean needMarkActivated = false)
		{
			
			var executeResult = new ExecuteResult { IsSuccessed = true };
			//如果Status為空,表示刪除序號
			if (string.IsNullOrEmpty(status))
			{
				delSnLists.Add(serialNoResult.SerialNo);
				return executeResult;
			}

			string deviceIp = Current.DeviceIp;
			var f2501 = CommonService.GetItemSerialList(gupCode,custCode, new List<string> { serialNoResult.SerialNo }).FirstOrDefault();
			F2501 orif2501 = null;

			bool isCreate = false;
			if (f2501 == null)
				isCreate = true;
			else
				orif2501 = JsonConvert.DeserializeObject<F2501>(JsonConvert.SerializeObject(f2501));
			f2501 = f2501 ?? new F2501();
			f2501.STATUS = status;
			f2501.ITEM_CODE = serialNoResult.ItemCode;
			f2501.CELL_NUM = serialNoResult.CellNum;
			if (overwriteCellNum != null)
			{
				f2501.CELL_NUM = overwriteCellNum;
			}

			f2501.PUK = serialNoResult.Puk;
			if (overwritePuk != null)
			{
				f2501.PUK = overwritePuk;
			}

			f2501.WMS_NO = wmsNo;

			f2501.VNR_CODE = vnrCode;

			f2501.BATCH_NO = serialNoResult.BatchNo;
			if (overwriteBatchNo != null)
			{
				f2501.BATCH_NO = overwriteBatchNo;
			}

			f2501.BOX_SERIAL = serialNoResult.BoxSerail;
			f2501.CASE_NO = serialNoResult.CaseNo;
			f2501.CLIENT_IP = deviceIp;

			if (combinNo != null)
			{
				f2501.COMBIN_NO = combinNo == 0 ? null : combinNo;
			}

			if (ordProp != null)
			{
				f2501.ORD_PROP = ordProp;
			}

			if (retailCode != null)
			{
				f2501.RETAIL_CODE = retailCode;
			}


			if (boundleItemCode != null)
			{
				f2501.BOUNDLE_ITEM_CODE = boundleItemCode;
			}

			if (status == "A1")
			{
				f2501.PROCESS_NO = wmsNo;
			}

			if (isCreate || validDate.HasValue)
			{
				f2501.VALID_DATE = validDate;
			}

			if (status == "C1" || status == "D2")
			{
				f2501.BATCH_NO = "";
				f2501.BOX_SERIAL = "";
				f2501.CASE_NO = "";
			}

			if (orif2501?.STATUS == "C1" && status == "A1")
			{
        f2501.IN_DATE = DateTime.Today;   // #2149 新建的才指定進倉日期
        f2501.PO_NO = poNo;
      }

			if (isCreate)
			{
				if (poNo != null && !string.IsNullOrEmpty(poNo))
				{
					f2501.PO_NO = poNo;
				}

        f2501.IN_DATE = DateTime.Today;
				f2501.SERIAL_NO = serialNoResult.SerialNo;
				f2501.GUP_CODE = gupCode;
				f2501.CUST_CODE = custCode;
				f2501.ACTIVATED = needMarkActivated ? "1" : "0";
				f2501.SEND_CUST = "0";
        f2501.IS_ASYNC = "N";    //20230323 KK #2149  1. 序號寫入F2501 後，若是新增資料，預設IS_ASYNC=N
        addF2501s.Add(f2501);
			}
			else
			{
				if (string.IsNullOrWhiteSpace(f2501.CRT_NAME))
				{
					f2501.CRT_DATE = DateTime.Now;
					f2501.CRT_NAME = Current.StaffName;
					f2501.CRT_STAFF = Current.Staff;
      
        }
        if (status=="A1")
        {
          f2501.IN_DATE = DateTime.Today;  //20230418  調入(C1->A1)後，F2501.IN_DATE應該更新系統日
          f2501.IS_ASYNC = "N";    //20230323 KK #2149  2. 若序號資料已存在但該序號可以改為進貨，請更新IS_ASYNC=N
					f2501.ACTIVATED = needMarkActivated ? "1" : "0"; // 重新進來時，要更新是否為不良品序號狀態
				}
          
				updF2501s.Add(f2501);
			}

			return executeResult;
		}

		/// <summary>
		/// 序號更新，更新內容不在此作業，會將異動的資料丟出，方便一次性打包更新 (請先序號檢核通過在執行更新)
		/// </summary>
		/// <param name="dcCode">DC(為了取得工作站與錄影台設定檔,可空值)</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="status">變更後序號狀態</param>
		/// <param name="serialNoResult">序號檢核結果</param>
		/// <param name="wmsNo">系統單號</param>
		/// <param name="vnrCode">廠商F1908</param>
		/// <param name="validDate">效期</param>
		/// <param name="combinNo">組合編號(null表示不更新,0表示清空)</param>
		/// <param name="poNo">PO單號</param>
		/// <param name="ordProp">作業類別F000903</param>
		/// <param name="retailCode">客戶編號</param>
		/// <param name="overwritePuk">若有填，則會覆寫PUK</param>
		/// <param name="overwriteCellNum">若有填，則會覆寫門號</param>
		/// <param name="overwriteBatchNo">若有填，則會覆寫儲值卡盒號</param>
		/// <param name="boundleItemCode">加工組合成品編號</param>
		/// <returns></returns>
		public BulkUpdateF2501Result UpdateSerialNoFullForBulk(string dcCode, string gupCode, string custCode, string status,
				SerialNoResult serialNoResult, string wmsNo, string vnrCode, DateTime? validDate, long? combinNo = null,
				string poNo = null, string ordProp = null, string retailCode = null, string overwritePuk = null,
				string overwriteCellNum = null, string overwriteBatchNo = null, string boundleItemCode = null)
		{
      BulkUpdateF2501Result result;
      List<F2501> addF2501s = new List<F2501>();
      List<F2501> updF2501s = new List<F2501>();
      List<string> delSnLists = new List<string>();
      var procResult = UpdateSerialNoFull(ref addF2501s, ref updF2501s, ref delSnLists, dcCode, gupCode, custCode, status, serialNoResult, wmsNo, vnrCode, validDate, combinNo, poNo, ordProp, retailCode, overwritePuk, overwriteCellNum, overwriteBatchNo, boundleItemCode);

      result = new BulkUpdateF2501Result()
      {
        IsSuccessed = procResult.IsSuccessed,
        Message = procResult.Message
      };

      if (addF2501s.Any())
      {
        result.ModifyMode = Datas.Shared.Enums.ModifyMode.Add;
        result.f2501 = addF2501s.First();
      }
      if (updF2501s.Any())
      {
        result.ModifyMode = Datas.Shared.Enums.ModifyMode.Edit;
        result.f2501 = updF2501s.First();
      }
      if (delSnLists.Any())
      {
        result.ModifyMode = Datas.Shared.Enums.ModifyMode.Delete;
        result.f2501 = new F2501() { GUP_CODE = gupCode, CUST_CODE = custCode, SERIAL_NO = delSnLists.First() };
      }


      return result;
		}

		#endregion

		/// <summary>
		/// 檢查序號是否為與傳入的品號相同
		/// </summary>
		/// <param name="itemCode">傳入的品號</param>		
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="serialNo"></param>
		/// <returns></returns>
		public ExecuteResult CheckSerialIsSameItemCode(string itemCode, string gupCode, string custCode, string serialNo)
		{
			var scanItemCode = GetItemCodeBySerialNo(gupCode, custCode, serialNo);
			return scanItemCode != itemCode ? new ExecuteResult(false, "刷讀品項錯誤,請刷讀品項或條碼或序號") : new ExecuteResult(true);
		}

		public ExecuteResult CheckItemSerialNo(string gupCode, string custCode, string itemCode, string serialNo)
		{
			var barcode = BarcodeInspection(gupCode, custCode, serialNo);
			//如果是barcode是儲值卡盒號規則 但是商品非儲值卡 就視為是序號
			if (barcode.Barcode == BarcodeType.BatchNo && !IsBatchNoItem(gupCode, custCode, itemCode))
				barcode.Barcode = BarcodeType.SerialNo;
			if (barcode.Barcode != BarcodeType.SerialNo)
				return new ExecuteResult { IsSuccessed = false, Message = "必須為序號" };
			var serialNoResult = CheckSeralNoBase(gupCode, custCode, itemCode, serialNo);
			if (!serialNoResult.Checked)
				return new ExecuteResult { IsSuccessed = false, Message = serialNoResult.Message };
			var item = GetItemCodeBySerialNo(gupCode, custCode, serialNo);
			if (string.IsNullOrEmpty(item))
				return new ExecuteResult { IsSuccessed = false, Message = "序號不存在" };
			if (item != itemCode)
				return new ExecuteResult { IsSuccessed = false, Message = "序號非此商品序號" };
			return new ExecuteResult { IsSuccessed = true, Message = "" };

		}

		#region 序號檢查-在庫序號

		public ExecuteResult CheckSerialByInHouse(string gupCode, string custCode, string itemCode, string serialNo)
		{
			var result = CheckItemSerialNo(gupCode, custCode, itemCode, serialNo);
			if (!result.IsSuccessed)
				return result;

			var serialNoData = _f2501Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.SERIAL_NO == serialNo);
			if (serialNoData.STATUS != "A1")
			{
				//查詢[狀態代碼]對照表
				var statusData = F000904DataList.FirstOrDefault(x => x.VALUE == serialNoData.STATUS);
				var statusName = statusData != null ? statusData.NAME : string.Empty;
				return new ExecuteResult { IsSuccessed = false, Message = string.Format("{0} 此序號狀態為{1},必須為在庫序號", serialNo, statusName) };
			}
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}


		#endregion

		/// <summary>
		/// 是否商品類別為儲值卡盒號
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="itemCode">品號</param>
		/// <returns></returns>
		public bool IsBatchNoItem(string gupCode, string custCode, string itemCode)
		{
			var item = CommonService.GetProduct(gupCode, custCode, itemCode);
			if (item != null)
				return item.TYPE == "03";
			return false;
		}

		/// <summary>
		/// 揀貨/包裝/退貨/調撥 刷讀盒/箱/批號(儲值卡盒號)/序號 檢查
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="itemCode">品號</param>
		/// <param name="barCode">盒/箱/批號(儲值卡盒號)/序號</param>
		/// <param name="isCheckInHouse">是否檢查在庫序號</param>
		/// <returns>檢查成功 Message=回傳序號(以逗點分隔)</returns>
		public ExecuteResult CheckBarCode(string gupCode, string custCode, string itemCode, string barCode, bool isCheckInHouse = true)
		{
			var barCodeData = BarcodeInspection(gupCode, custCode, barCode);
			//如果是barcode是儲值卡盒號規則 但是商品非儲值卡 就視為是序號
			if (barCodeData.Barcode == BarcodeType.BatchNo && !IsBatchNoItem(gupCode, custCode, itemCode))
				barCodeData.Barcode = BarcodeType.SerialNo;
			SerialNoResult check;
			if (barCodeData.Barcode == BarcodeType.SerialNo)
			{
				check = CheckSeralNoBase(gupCode, custCode, itemCode, barCode);
				if (!check.Checked)
					return new ExecuteResult { IsSuccessed = false, Message = check.Message };
			}
			check = GetSerialItem(gupCode, custCode, barCode);
			if (!check.Checked)
				return new ExecuteResult { IsSuccessed = false, Message = check.Message };
			if (check.ItemCode != itemCode)
			{
				check.Checked = false;
				check.Message = string.Format("非此商品{0}", barCodeData.BarcodeText);
				return new ExecuteResult { IsSuccessed = false, Message = check.Message };
			}
			var serialNoList = new List<string>();
			switch (barCodeData.Barcode)
			{
				case BarcodeType.BatchNo:
					var batchSerialNo = _f2501Repo.GetDatasByBatchNo(gupCode, custCode, barCode).Select(o => o.SERIAL_NO);
					for (int i = 1; i <= 200; i++)
					{
						var serialNo = barCode.Trim().Substring(0, barCode.Trim().Length - 3) + i.ToString().PadLeft(3, '0');
						if (batchSerialNo.Contains(serialNo))
							serialNoList.Add(serialNo);
					}
					if (serialNoList.Count != 200) //當儲值卡序號不等於200個 就視為是序號
					{
						serialNoList.Clear();
						serialNoList.Add(barCode);
						barCodeData.Barcode = BarcodeType.SerialNo;
					}
					break;
				case BarcodeType.BoxSerial:
					serialNoList.AddRange(_f2501Repo.GetDatasByBoxSerial(gupCode, custCode, barCode).Select(o => o.SERIAL_NO));
					break;
				case BarcodeType.CaseNo:
					serialNoList.AddRange(_f2501Repo.GetDatasByCaseNo(gupCode, custCode, barCode).Select(o => o.SERIAL_NO));
					break;
				case BarcodeType.SerialNo:
					serialNoList.Add(barCode);
					break;
			}
			if (!serialNoList.Any())
				return new ExecuteResult { IsSuccessed = false, Message = string.Format("此{0}不存在", barCodeData.BarcodeText) };

			// 只有一個序號就繼續採用快取，就不用存取db，多個序號就大量取得來檢核
			var f2501s = (serialNoList.Count == 1) ? new List<F2501> { _f2501Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.SERIAL_NO == serialNoList.First()) }
																						 : _f2501Repo.InWithTrueAndCondition("SERIAL_NO", serialNoList, o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode).ToList();

			// 檢查序號存不存在
			var notExistsSerialNo = serialNoList.Where(s => !f2501s.Any(x => x.SERIAL_NO == s)).FirstOrDefault();
			if (notExistsSerialNo != null)
				return new ExecuteResult { IsSuccessed = false, Message = string.Format("此{0}不存在", notExistsSerialNo) };

			// 檢查序號是否在庫
			if (isCheckInHouse)
			{
				foreach (var serialNo in serialNoList)
				{
					var result = CheckSerialByInHouse(gupCode, custCode, itemCode, serialNo);
					if (!result.IsSuccessed)
						return result;
				}
			}
			return new ExecuteResult { IsSuccessed = true, Message = string.Join(",", serialNoList.ToArray()) };
		}
		/// <summary>
		///  清除 已拆開序號的箱號/盒號/儲值卡盒號
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="wmsNo">單號</param>
		/// <param name="type"> TD:調撥下架,TU:調撥上架  O:出貨包裝</param>
		public void ClearSerialByBoxOrCaseNo(string dcCode, string gupCode, string custCode, string wmsNo, string type)
		{
			var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
			switch (type)
			{
				case "TD"://調撥下架
					var itemsTD = f151002Repo.GetDatasByTrueAndCondition(
							o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == wmsNo).Select(o => o.ITEM_CODE).Distinct().ToList();
					var clearDataByCaseNoD = _f2501Repo.GetcClearSerialBoxOrCaseNoesByAllocation(dcCode, gupCode, custCode, wmsNo, itemsTD, "CASE_NO", false);
					foreach (var clearSerialBoxOrCaseNo in clearDataByCaseNoD)
						UpdateCaseNoEmpty(gupCode, custCode, clearSerialBoxOrCaseNo.BoxOrCaseNo);
					var clearDataByBoxSerialD = _f2501Repo.GetcClearSerialBoxOrCaseNoesByAllocation(dcCode, gupCode, custCode, wmsNo, itemsTD, "BOX_SERIAL", false);
					foreach (var clearSerialBoxOrCaseNo in clearDataByBoxSerialD)
						UpdateBoxSerialEmpty(gupCode, custCode, clearSerialBoxOrCaseNo.BoxOrCaseNo);
					var clearDataByBatchNoD = _f2501Repo.GetcClearSerialBoxOrCaseNoesByAllocation(dcCode, gupCode, custCode, wmsNo, itemsTD, "BATCH_NO", false);
					foreach (var clearSerialBoxOrCaseNo in clearDataByBatchNoD)
						UpdateBatchNoEmpty(gupCode, custCode, clearSerialBoxOrCaseNo.BoxOrCaseNo);
					break;
				case "TU"://調撥上架
					var itemsTU = f151002Repo.GetDatasByTrueAndCondition(
							o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == wmsNo)
							.Select(o => o.ITEM_CODE)
							.Distinct()
							.ToList();
					var clearDataByCaseNoU = _f2501Repo.GetcClearSerialBoxOrCaseNoesByAllocation(dcCode, gupCode, custCode, wmsNo,
									itemsTU, "CASE_NO", true);
					foreach (var clearSerialBoxOrCaseNo in clearDataByCaseNoU)
						UpdateCaseNoEmpty(gupCode, custCode, clearSerialBoxOrCaseNo.BoxOrCaseNo);
					var clearDataByBoxSerialU = _f2501Repo.GetcClearSerialBoxOrCaseNoesByAllocation(dcCode, gupCode, custCode, wmsNo,
							itemsTU, "BOX_SERIAL", true);
					foreach (var clearSerialBoxOrCaseNo in clearDataByBoxSerialU)
						UpdateBoxSerialEmpty(gupCode, custCode, clearSerialBoxOrCaseNo.BoxOrCaseNo);
					var clearDataByBatchNoU = _f2501Repo.GetcClearSerialBoxOrCaseNoesByAllocation(dcCode, gupCode, custCode, wmsNo,
							itemsTU, "BATCH_NO", true);
					foreach (var clearSerialBoxOrCaseNo in clearDataByBatchNoU)
						UpdateBatchNoEmpty(gupCode, custCode, clearSerialBoxOrCaseNo.BoxOrCaseNo);
					break;
				case "O"://出貨包裝
					ClearSerialByBoxOrCaseNoByWmsOrdNo(dcCode, gupCode, custCode, wmsNo);
					break;
			}
		}


		public void ClearSerialByBoxOrCaseNoByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f055002Repo = new F055002Repository(Schemas.CoreSchema, _wmsTransaction);

			var f055002WithF2501s = f055002Repo.GetF055002WithF2501s(dcCode, gupCode, custCode, wmsOrdNo);

			// 找出某個序號的品號是否一次有裝兩箱以上的盒號
			var boxSerialsQuery = from item in f055002WithF2501s
														where !string.IsNullOrEmpty(item.BOX_SERIAL)
														group item by new { item.ITEM_CODE, item.BOX_SERIAL } into g
														where g.Select(x => x.PACKAGE_BOX_NO).Distinct().Count() > 1
														select g.Key;

			var boxSerials = boxSerialsQuery.ToList();

			// 找出某個序號的品號是否一次有裝兩箱以上的箱號
			var caseNosQuery = from item in f055002WithF2501s
												 where !string.IsNullOrEmpty(item.CASE_NO)
												 group item by new { item.ITEM_CODE, item.CASE_NO } into g
												 where g.Select(x => x.PACKAGE_BOX_NO).Distinct().Count() > 1
												 select g.Key;

			var caseNos = caseNosQuery.ToList();

			// 找出某個序號的品號是否一次有裝兩箱以上的儲值卡盒號
			var batchNosQuery = from item in f055002WithF2501s
													where !string.IsNullOrEmpty(item.BATCH_NO)
													group item by new { item.ITEM_CODE, item.BATCH_NO } into g
													where g.Select(x => x.PACKAGE_BOX_NO).Distinct().Count() > 1
													select g.Key;

			var batchNos = batchNosQuery.ToList();

			// 清除盒號
			if (boxSerials.Any())
			{
				f2501Repo.UpdateFieldsInWithTrueAndCondition(SET: new { BOX_SERIAL = string.Empty },
																										 WHERE: x => x.GUP_CODE == gupCode
																															&& x.CUST_CODE == custCode,
																										 InFieldName: x => x.BOX_SERIAL,
																										 InValues: boxSerials.Select(x => x.BOX_SERIAL).Distinct());
			}

			// 清除箱號
			if (caseNos.Any())
			{
				f2501Repo.UpdateFieldsInWithTrueAndCondition(SET: new { CASE_NO = string.Empty },
																										 WHERE: x => x.GUP_CODE == gupCode
																															&& x.CUST_CODE == custCode,
																										 InFieldName: x => x.CASE_NO,
																										 InValues: caseNos.Select(x => x.CASE_NO).Distinct());
			}

			// 清除儲值卡盒號
			if (batchNos.Any())
			{
				f2501Repo.UpdateFieldsInWithTrueAndCondition(SET: new { BATCH_NO = string.Empty },
																										 WHERE: x => x.GUP_CODE == gupCode
																															&& x.CUST_CODE == custCode,
																										 InFieldName: x => x.BATCH_NO,
																										 InValues: batchNos.Select(x => x.BATCH_NO).Distinct());
			}
		}

		public void UpdateCaseNoEmpty(string gupCode, string custCode, string caseNo)
		{
			if (!string.IsNullOrWhiteSpace(caseNo))
			{
				var data = _f2501Repo.AsForUpdate().GetDatasByCaseNo(gupCode, custCode, caseNo).ToList();
				foreach (var f2501 in data)
				{
					f2501.CASE_NO = "";
					_f2501Repo.Update(f2501);
				}
			}
		}
		public void UpdateBoxSerialEmpty(string gupCode, string custCode, string boxSerial)
		{
			if (!string.IsNullOrWhiteSpace(boxSerial))
			{
				var data = _f2501Repo.AsForUpdate().GetDatasByBoxSerial(gupCode, custCode, boxSerial).ToList();
				foreach (var f2501 in data)
				{
					f2501.BOX_SERIAL = "";
					_f2501Repo.Update(f2501);
				}
			}
		}
		public void UpdateBatchNoEmpty(string gupCode, string custCode, string batchNo)
		{
			if (!string.IsNullOrWhiteSpace(batchNo))
			{
				var data = _f2501Repo.AsForUpdate().GetDatasByBatchNo(gupCode, custCode, batchNo).ToList();
				foreach (var f2501 in data)
				{
					f2501.BATCH_NO = "";
					_f2501Repo.Update(f2501);
				}
			}
		}

		public bool IsCombinItem(string gupCode, string custCode, string serialNo, out List<F2501> combinF2501List, out string combinItemCode)
		{
			var item = _f2501Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.SERIAL_NO == serialNo);
			combinItemCode = "";
			combinF2501List = null;
			if (item != null && item.COMBIN_NO.HasValue && !string.IsNullOrEmpty(item.BOUNDLE_ITEM_CODE))
			{
				combinItemCode = item.BOUNDLE_ITEM_CODE;
				combinF2501List = _f2501Repo.GetDatasByCombinNo(gupCode, custCode, item.COMBIN_NO.Value).ToList();
				return true;
			}
			return false;
		}


		public BoxPalletNoData GetBarCodePalletOrBoxNo()
		{
			var PalletOrBox = new BoxPalletNoData();
			return PalletOrBox;
		}

    #region 檢查序號綁除位是否為此儲位商品

    #endregion
    /// <summary>
    /// 檢核入庫前商品單筆序號
    /// </summary>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="itemCode"></param>
    /// <param name="serialNo"></param>
    /// <returns></returns>
    public List<SerialNoResult> CheckItemSingleSerialWithBeforeInWarehouse(string gupCode, string custCode, string itemCode,  string serialNo)
    {
      return CheckItemLargeSerialWithBeforeInWarehouse(gupCode, custCode, itemCode, new List<string>{ serialNo} );      
      }
    /// <summary>
    /// 檢核入庫前商品多筆序號
    /// </summary>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="itemCode"></param>
    /// <param name="serialNoList"></param>
    /// <returns></returns>
    #region 檢核入庫前商品多筆序號
    public List<SerialNoResult> CheckItemLargeSerialWithBeforeInWarehouse(string gupCode, string custCode, string itemCode, List<string> serialNoList)
    {
      SerialNoResult check=null;
     
      string itemName = GetItemName(gupCode, custCode, itemCode);
      List<F2501> f2501s = CommonService.GetItemSerialList(gupCode, custCode, serialNoList);
			var serialNoResList = new List<SerialNoResult>();
      foreach( string serialNo in serialNoList)
      {
        check = CheckSeralNoBase(gupCode, custCode,itemCode, serialNo, itemName);
				if (!check.Checked)
					serialNoResList.Add(check);
				F2501 f2501 = f2501s.Where(x => x.SERIAL_NO == serialNo).FirstOrDefault();
        if (f2501 == null)
        {
					serialNoResList.Add( new SerialNoResult
          {
            Checked = true,
            ItemCode = itemCode,
            ItemName = itemName,
            SerialNo = serialNo,
          });
        }
        else
        {
          check= SerialNoStatusCheckImpl(f2501, serialNo, "A1", null, itemName);
          if (!check.Checked)
						serialNoResList.Add(check);
					else
          {
						if(f2501.ITEM_CODE != itemCode)
						{
							serialNoResList.Add(new SerialNoResult
							{
								Checked = false,
								ItemCode = itemCode,
								ItemName = itemName,
								SerialNo = serialNo,
								Message = string.Format("此商品序號原品號為{0}，與新品號{1}不同，不允許更換品號", f2501.ITEM_CODE, itemCode)
							});
						}
						serialNoResList.Add(new SerialNoResult
            {
              Checked = true,
              ItemCode = itemCode,
              ItemName = itemName,
              SerialNo = serialNo,
            });
          }
        }
      }
			return serialNoResList;
    }
    #endregion
  }
}
