
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

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P160101Service
	{
		private WmsTransaction _wmsTransaction;
		public P160101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 建立退貨單
		/// </summary>
		/// <param name="addF161201"></param>
		/// <param name="addF161202s"></param>
		/// <returns></returns>
		public ExecuteResult InsertP160101(F161201 addF161201, F161202[] addF161202s)
		{
			if (addF161201 == null || addF161202s == null || !addF161202s.Any())
				return new ExecuteResult() { Message = Properties.Resources.P160101Service_addParam_Required };

			if (string.IsNullOrEmpty(addF161201.DC_CODE) || string.IsNullOrEmpty(addF161201.GUP_CODE) || string.IsNullOrEmpty(addF161201.CUST_CODE))
				return new ExecuteResult() { Message = Properties.Resources.P160101Service_DC_CUSTCODE_GUPCODE_Required };

			if (!string.IsNullOrEmpty(addF161201.RETURN_NO))
				return new ExecuteResult() { Message = Properties.Resources.P160101Service_RTN_NO_Duplicate };

			var sharedService = new SharedService();
			var f161201Repo = new F161201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161202Repo = new F161202Repository(Schemas.CoreSchema, _wmsTransaction);

			var newOrdCode = sharedService.GetNewOrdCode("R");
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var statusValue = f000904Repo.GetF000904Data("F161201", "STATUS").Where(a => a.VALUE == "0").Select(a => a.VALUE).FirstOrDefault();

			addF161201.STATUS = statusValue;

			addF161201.RETURN_NO = newOrdCode;
			f161201Repo.Add(addF161201);

			int seq = 0;
			foreach (var item in addF161202s)
			{
				seq++;
				item.RETURN_NO = newOrdCode;
				item.RETURN_SEQ = seq.ToString();
				item.DC_CODE = addF161201.DC_CODE;
				item.GUP_CODE = addF161201.GUP_CODE;
				item.CUST_CODE = addF161201.CUST_CODE;

				f161202Repo.Add(item);
			}


			return new ExecuteResult() { IsSuccessed = true, Message = newOrdCode };
		}

		/// <summary>
		/// 編輯退貨單
		/// </summary>
		/// <param name="editF161201"></param>
		/// <param name="editF161202s"></param>
		/// <returns></returns>
		public ExecuteResult UpdateP160101(F161201 editF161201, F161202[] editF161202s, bool isP160101)
		{
			if (editF161201 == null || editF161202s == null || !editF161202s.Any())
				return new ExecuteResult() { Message = Properties.Resources.P160101Service_editParam_Required };

			var f161201Repo = new F161201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161202Repo = new F161202Repository(Schemas.CoreSchema, _wmsTransaction);

			var f161201 = f161201Repo.Find(item => item.RETURN_NO == EntityFunctions.AsNonUnicode(editF161201.RETURN_NO)
												&& item.DC_CODE == EntityFunctions.AsNonUnicode(editF161201.DC_CODE)
												&& item.GUP_CODE == EntityFunctions.AsNonUnicode(editF161201.GUP_CODE)
												&& item.CUST_CODE == EntityFunctions.AsNonUnicode(editF161201.CUST_CODE));

			var error = ValidateF161201(f161201);
			if (!string.IsNullOrEmpty(error))
				return new ExecuteResult { Message = error };

			if (isP160101 && !IsCanEditOrDelete(f161201))
				return new ExecuteResult(false, Properties.Resources.P160101Service_RTN_SRC_NO_UnabledEdit);
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var statusValue = f000904Repo.GetF000904Data("F161201", "STATUS").Where(a => a.VALUE == "0").Select(a => a.VALUE).FirstOrDefault();
			f161201.STATUS = statusValue;
			f161201.CUST_ORD_NO = editF161201.CUST_ORD_NO;
			f161201.WMS_ORD_NO = editF161201.WMS_ORD_NO;
			f161201.RTN_CUST_CODE = editF161201.RTN_CUST_CODE;

			if (!string.IsNullOrEmpty(editF161201.RTN_CUST_NAME))
				f161201.RTN_CUST_NAME = editF161201.RTN_CUST_NAME;

			if (!string.IsNullOrEmpty(editF161201.RTN_TYPE_ID))
				f161201.RTN_TYPE_ID = editF161201.RTN_TYPE_ID;

			if (!string.IsNullOrEmpty(editF161201.RTN_CAUSE))
				f161201.RTN_CAUSE = editF161201.RTN_CAUSE;

			if (!string.IsNullOrEmpty(editF161201.DISTR_CAR))
				f161201.DISTR_CAR = editF161201.DISTR_CAR;

			if (!string.IsNullOrEmpty(editF161201.ADDRESS))
				f161201.ADDRESS = editF161201.ADDRESS;

			if (!string.IsNullOrEmpty(editF161201.CONTACT))
				f161201.CONTACT = editF161201.CONTACT;

			if (!string.IsNullOrEmpty(editF161201.TEL))
				f161201.TEL = editF161201.TEL;

			f161201.MEMO = editF161201.MEMO;
			f161201.COST_CENTER = editF161201.COST_CENTER;

			f161202Repo.Delete(item => item.RETURN_NO == editF161201.RETURN_NO
											&& item.DC_CODE == editF161201.DC_CODE
											&& item.GUP_CODE == editF161201.GUP_CODE
											&& item.CUST_CODE == editF161201.CUST_CODE);


			int seq = 0;
			foreach (var item in editF161202s)
			{
				seq++;
				item.RETURN_NO = f161201.RETURN_NO;
				item.RETURN_SEQ = seq.ToString();
				item.DC_CODE = f161201.DC_CODE;
				item.GUP_CODE = f161201.GUP_CODE;
				item.CUST_CODE = f161201.CUST_CODE;

				f161202Repo.Add(item);
			}




			f161201.SOURCE_TYPE = editF161201.SOURCE_TYPE;
			f161201.SOURCE_NO = editF161201.SOURCE_NO;
			f161201Repo.Update(f161201);

			return new ExecuteResult() { IsSuccessed = true, Message = Properties.Resources.P160101Service_EditRtn };
		}

		private string ValidateF161201(F161201 f161201)
		{
			if (f161201 == null)
			{
				return Properties.Resources.P160101Service_RTN_NotFound;
			}

			switch (f161201.STATUS)
			{
				case "9":
					return Properties.Resources.P160101Service_RTN_Deleted;
				case "2":
					return Properties.Resources.P160101Service_RTN_Close;
			}

			return string.Empty;
		}

		bool IsCanEditOrDelete(F161201 f161201)
		{
			// 若為退貨類型R3，或沒有填來源單號才可編輯或刪除
			return f161201.ORD_PROP == "R3" || string.IsNullOrEmpty(f161201.SOURCE_NO);
		}

		public ExecuteResult DeleteP160101(string returnNo, string gupCode, string custCode, string dcCode)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f161201Repo = new F161201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161201 = f161201Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode &&
							 o.CUST_CODE == custCode && o.RETURN_NO == returnNo);
			if (f161201 == null)
				return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P160101Service_RTN_NotFound };
			//只能狀態為0的才可刪除
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var statusValue0 = f000904Repo.GetF000904Data("F161201", "STATUS").Where(a => a.VALUE == "0").Select(a => a.VALUE).FirstOrDefault();

			if (f161201.STATUS != statusValue0)
				return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P160101Service_Pending };

			if (!IsCanEditOrDelete(f161201))
				return new ExecuteResult(false, Properties.Resources.P160101Service_RTN_SRC_NO_UnabledDelete);
			var statusValue9 = f000904Repo.GetF000904Data("F161201", "STATUS").Where(a => a.VALUE == "9").Select(a => a.VALUE).FirstOrDefault();
			f161201.STATUS = statusValue9; //更新為9取消
			f161201Repo.Update(f161201);


			if (result == null)
				result = new ExecuteResult { IsSuccessed = true };

			return result;
		}

		/// <summary>
		/// 新增退貨單時，可用出貨單號來匯入出貨單的 Item
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo">F050802的出貨單號</param>
		/// <returns></returns>
		public IQueryable<F161201DetailDatas> GetItemsByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			return f1903Repo.GetItemsByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo);
		}

		/// <summary>
		/// Import 退貨資料
		/// </summary>
		public ExecuteResult ImportF1612Data(string dcCode, string gupCode, string custCode
											, string fileName, List<F1612ImportData> importData)
		{
			//1. 一個檔案只能有一種貨主
			//2. 業主、貨主值對照到業主、貨主主檔的系統業主編號、系統貨主編號
			//3. 檢查作業類別、退貨類型、退貨原因是否存在，不存在不可匯入此筆
			//4. 客戶編號如果不是B2C則檢查F1910門市主檔是否有此編號
			//5. 必填欄位的檢核
			ExecuteResult result = new ExecuteResult();
			string errorMessage = string.Empty;
			string dataContent = string.Empty;
			int successCtn = 0;
			int errorCtn = 0;
			result.IsSuccessed = true;
			//一個檔案只能有一種貨主
			if (importData.GroupBy(o => o.GUP_CODE).Count() > 1)
			{
				errorMessage = Properties.Resources.P160101Service_ONE_CUST_CODE_Required;
				errorCtn = importData.Count;
				UpdateF0060Log(fileName, "3", "", "", errorMessage, dcCode, gupCode, custCode);
				result.Message = string.Format(Properties.Resources.P160101Service_ImportResult
												, importData.Count, successCtn, importData.Count);

				return result;
			}

			var f1929Repo = new F1929Repository(Schemas.CoreSchema);
			var f1909Repo = new F1909Repository(Schemas.CoreSchema);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f1910Repo = new F1910Repository(Schemas.CoreSchema);
			var f1951Repo = new F1951Repository(Schemas.CoreSchema);
			var f0000903Repo = new F000903Repository(Schemas.CoreSchema);
			var f161203Repo = new F161203Repository(Schemas.CoreSchema);

			//先把所有基本檔撈出來. 必免每次都進 DB
			var f1929Datas = f1929Repo.Filter(o => 1 == 1).ToList();
			var f1909Datas = f1909Repo.Filter(o => 1 == 1).ToList();
			var f1000903Datas = f0000903Repo.Filter(o => 1 == 1).ToList();
			var f1951Datas = f1951Repo.Filter(o => o.UCT_ID == "RT").ToList();
			var f161203Datas = f161203Repo.Filter(o => 1 == 1).ToList();

			var successData = new List<F1612ImportData>();

			foreach (var item in importData)
			{

				#region 檢查規則
				//檢查業主
				if (f1929Datas.Where(o => o.GUP_CODE == gupCode && o.GUP_CODE == item.GUP_CODE).Count() == 0)
					errorMessage = Properties.Resources.P160101Service_GUP_SYS_NO_NotFound;
				//檢查貨主
				if (f1909Datas.Where(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.CUST_CODE == item.CUST_CODE).Count() == 0)
					errorMessage += Properties.Resources.P160101Service_CUST_SYS_NO_NotFound;

				//檢查作業類別
				if (string.IsNullOrEmpty(item.ORD_PROP))
				{
					errorMessage += Properties.Resources.P160101Service_WorkType_Empty;
				}
				else
				{
					if (f1000903Datas.Where(o => o.ORD_PROP == item.ORD_PROP).Count() == 0)
						errorMessage += Properties.Resources.P160101Service_WorkType_NotFound;
				}

				//檢查客戶編號
				if (string.IsNullOrEmpty(item.RETAIL_CODE))
				{
					errorMessage += Properties.Resources.P160101Service_CUST_CODE_Empty;
				}
				else if (item.RETAIL_CODE != "B2C")
				{
					var f1909 = f1909Datas.FirstOrDefault(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
					var filterCustCode = custCode;
					if (f1909 != null && f1909.ALLOWGUP_RETAILSHARE == "1")
						filterCustCode = "0";
					var f1910Data = f1910Repo.Find(o => o.GUP_CODE == gupCode && o.RETAIL_CODE == item.RETAIL_CODE && o.CUST_CODE == filterCustCode);
					if (f1910Data == null)
						errorMessage += Properties.Resources.P160101Service_Retail_Code_NotFound;
					else
						item.RETAIL_CODE_NAME = f1910Data.RETAIL_NAME;
				}

				//退貨類型
				if (string.IsNullOrEmpty(item.RTN_TYPE_ID))
				{
					errorMessage += Properties.Resources.P160101Service_RTN_Type_Empty;
				}
				else
				{
					if (f161203Datas.Where(o => o.RTN_TYPE_ID == item.RTN_TYPE_ID) == null)
						errorMessage += Properties.Resources.P160101Service_RTN_Type_NotFound;
				}

				//退貨原因
				if (string.IsNullOrEmpty(item.RTN_CAUSE))
				{
					errorMessage += Properties.Resources.P160101Service_RTN_Casuse_Empty;
				}
				else
				{
					if (f1951Datas.Where(o => o.UCC_CODE == item.RTN_CAUSE) == null)
						errorMessage += Properties.Resources.P160101Service_RTN_Cause_NotFound;
				}

				//商品品號
				if (string.IsNullOrEmpty(item.ITEM_CODE))
				{
					errorMessage += Properties.Resources.P160101Service_Item_Code_Empty;
				}
				else
				{
					if (f1903Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ITEM_CODE == item.ITEM_CODE) == null)
						errorMessage += Properties.Resources.P160101Service_Item_Code_NotFound;
				}

				//聯絡人
				if (string.IsNullOrEmpty(item.CONTACT))
					errorMessage += Properties.Resources.P160101Service_Contactor_Empty;

				//電話
				if (string.IsNullOrEmpty(item.TEL))
					errorMessage += Properties.Resources.P160101Service_TEL_Empty;

				//地址
				if (string.IsNullOrEmpty(item.ADDRESS))
					errorMessage += Properties.Resources.P160101Service_ADR_Empty;

				//退貨量
				if (item.RTN_QTY == 0)
					errorMessage += Properties.Resources.P160101Service_RTN_Count_Zero;


				#endregion

				if (string.IsNullOrEmpty(errorMessage))
				{
					successCtn += 1;
					//寫入退貨主檔 
					successData.Add(item);
					//InserF1612Data(dcCode, gupCode, custCode, item);
				}
				else
					errorCtn += 1;


				//Log DataContent 欄位
				dataContent = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}"
											, item.GUP_CODE, item.CUST_CODE, item.ORD_PROP
											, item.COST_CENTER, item.RETAIL_CODE, item.CONTACT
											, item.TEL, item.ADDRESS, item.DISTR_CAR
											, item.MEMO, item.CUST_ORD_NO, item.ITEM_CODE
											, item.RTN_QTY, item.RTN_TYPE_ID, item.RTN_CAUSE);

				UpdateF0060Log(fileName, "3", item.ITEM_CODE, dataContent, errorMessage, dcCode, gupCode, custCode);
				errorMessage = string.Empty;
			}
			InserF1612Data(dcCode, gupCode, custCode, successData);


			result.Message = string.Format(Properties.Resources.P160101Service_ImportResult, importData.Count, successCtn, errorCtn);
			return result;
		}

		#region 新增退貨主檔 by key 產生多筆退貨單
		private void InserF1612Data(string dcCode, string gupCode, string custCode, List<F1612ImportData> items)
		{
			var f161201Repo = new F161201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161202Repo = new F161202Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService();

			var groupItems = from o in items
							 group o by new { o.CUST_ORD_NO, o.RETAIL_CODE, o.CONTACT, o.RETAIL_CODE_NAME, o.RTN_TYPE_ID, o.RTN_CAUSE, o.DISTR_CAR, o.COST_CENTER, o.ADDRESS, o.TEL, o.MEMO, o.ORD_PROP }
											 into g
							 select g;
			var f161201List = new List<F161201>();
			var f161202List = new List<F161202>();
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);

			var statusValue0 = f000904Repo.GetF000904Data("F161201", "STATUS").Where(a => a.VALUE == "0").Select(a => a.VALUE).FirstOrDefault();
			string tmpStatus = statusValue0; //0:待處理
			foreach (var groupItem in groupItems)
			{
				var newOrdCode = sharedService.GetNewOrdCode("R");
                F161201 f161201 = new F161201()
                {
                    RETURN_NO = newOrdCode,
                    RETURN_DATE = DateTime.Today,
                    POSTING_DATE = null,
                    STATUS = tmpStatus,
                    CUST_ORD_NO = groupItem.Key.CUST_ORD_NO,
                    WMS_ORD_NO = "",
                    RTN_CUST_CODE = string.IsNullOrEmpty(groupItem.Key.RETAIL_CODE) || groupItem.Key.RETAIL_CODE == "B2C" ? null : groupItem.Key.RETAIL_CODE,
                    RTN_CUST_NAME = string.IsNullOrEmpty(groupItem.Key.RETAIL_CODE) || groupItem.Key.RETAIL_CODE == "B2C" ? groupItem.Key.CONTACT : groupItem.Key.RETAIL_CODE_NAME,
                    RTN_TYPE_ID = groupItem.Key.RTN_TYPE_ID,
                    RTN_CAUSE = groupItem.Key.RTN_CAUSE,
                    SOURCE_TYPE = null,
                    SOURCE_NO = null,
                    DISTR_CAR = groupItem.Key.DISTR_CAR == "Y" ? "1" : "0",
                    COST_CENTER = groupItem.Key.COST_CENTER,
                    ADDRESS = groupItem.Key.ADDRESS,
                    CONTACT = groupItem.Key.CONTACT,
                    TEL = groupItem.Key.TEL,
                    MEMO = groupItem.Key.MEMO,
                    ORD_PROP = groupItem.Key.ORD_PROP,
                    DC_CODE = dcCode,
                    GUP_CODE = gupCode,
                    CUST_CODE = custCode,
                    PROC_FLAG = "0"
				};
				f161201List.Add(f161201);

				int index = 1;
				foreach (var f1612ImportData in groupItem)
				{
					F161202 f161202 = new F161202()
					{
						RETURN_NO = newOrdCode,
						RETURN_SEQ = index.ToString(),
						ITEM_CODE = f1612ImportData.ITEM_CODE,
						RTN_QTY = f1612ImportData.RTN_QTY,
						RTN_CUS_FLAG = "0",
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode
					};
					f161202List.Add(f161202);
					index++;
				}
			}
			f161201Repo.BulkInsert(f161201List);
			f161202Repo.BulkInsert(f161202List);
		}
		#endregion

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