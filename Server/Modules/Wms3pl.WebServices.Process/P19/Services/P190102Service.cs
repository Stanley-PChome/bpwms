using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F19;
using System.Reflection;
using System.Data.Objects;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.Process.P16.Services;
using Wms3pl.WebServices.Shared.Lms.Services;

namespace Wms3pl.WebServices.Process.P19.Services
{
  public partial class P190102Service
  {
		private WmsTransaction _wmsTransaction;
		public P190102Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		//更新F1905
		private void UpdateF1905(F1905 f1905, F1905 f1905Data)
		{
			f1905.PACK_LENGTH = f1905Data.PACK_LENGTH;
			f1905.PACK_WIDTH = f1905Data.PACK_WIDTH;
			f1905.PACK_HIGHT = f1905Data.PACK_HIGHT;
			f1905.PACK_WEIGHT = f1905Data.PACK_WEIGHT;
		}

		private void UpdateF190301(F190301 f190301, F190301 f190301Data)
		{
			f190301.LENGTH = f190301Data.LENGTH;
			f190301.WIDTH = f190301Data.WIDTH;
			f190301.HIGHT = f190301Data.HIGHT;
			f190301.WEIGHT = f190301Data.WEIGHT;
		}

		private void UpdateF190305(F190305 f190305, F190305 f190305Data)
		{
			f190305.PALLET_LEVEL_CASEQTY = f190305Data.PALLET_LEVEL_CASEQTY;
			f190305.PALLET_LEVEL_CNT = f190305Data.PALLET_LEVEL_CNT;
		}

    public ExecuteResult UpdateP190102(F1903 mainItem, F1905 VolumeItem, F190305 palletLevel)
    {
			ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
			var repoF1903 = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF1905 = new F1905Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1924Repo = new F1924Repository(Schemas.CoreSchema);

      // 0. 先檢查是否已被刪除
      var f1903 = repoF1903.Find(x => x.GUP_CODE == mainItem.GUP_CODE
																	 && x.ITEM_CODE == mainItem.ITEM_CODE
																	 && x.CUST_CODE == mainItem.CUST_CODE);
			if (f1903 == null)
			{
				// 資料已被刪除
				result.IsSuccessed = false;
				result.Message = Properties.Resources.DataDelete;
				return result;
			}
			else
			{
				if (f1903.SND_TYPE != "0")
				{
					return new ExecuteResult(false, Properties.Resources.P190102Service_CustItemDataDeleted);
				}
			}

      #region 呼叫更新商品主檔API
      if (mainItem.BUNDLE_SERIALNO != f1903.BUNDLE_SERIALNO && mainItem.ISCARTON == "0")
      {
        UpdateItemInfoService updateItemInfoService = new UpdateItemInfoService();
        var apiRes = updateItemInfoService.UpdateItemInfo(
          mainItem.CUST_CODE,
          mainItem.CUST_ITEM_CODE,
          mainItem.BUNDLE_SERIALNO,
          string.IsNullOrWhiteSpace(mainItem.EAN_CODE1) ? mainItem.EAN_CODE4 : mainItem.EAN_CODE1);
        if (!apiRes.IsSuccessed)
          return apiRes;
      }
      #endregion

      // 1.1 設定F1903
      f1903 = SetObject(mainItem, f1903) as F1903;
			
			var errorMsg = ValidF1903Data(repoF1903, f1924Repo, mainItem, mainItem.GUP_CODE, mainItem.CUST_CODE, false);
			if (!string.IsNullOrEmpty(errorMsg))
				return new ExecuteResult(false, errorMsg);

			repoF1903.Update(f1903, exceptColumns: new List<string> { "FIRST_IN_DATE" });

			//更新190301
			var repoF190301 = new F190301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f190301 = repoF190301.AsForUpdate().GetF190301s(VolumeItem.GUP_CODE, VolumeItem.CUST_CODE, VolumeItem.ITEM_CODE, f1903.ITEM_UNIT).FirstOrDefault();
			if (f190301 == null)
			{
				repoF190301.Add(new F190301
				{
					GUP_CODE = f1903.GUP_CODE,
					ITEM_CODE = f1903.ITEM_CODE,
					UNIT_LEVEL = 1,
					UNIT_QTY = 1,
					LENGTH = VolumeItem.PACK_LENGTH,
					WIDTH = VolumeItem.PACK_WIDTH,
					HIGHT = VolumeItem.PACK_HIGHT,
					WEIGHT = VolumeItem.PACK_WEIGHT,
					UNIT_ID = f1903.ITEM_UNIT,
					CUST_CODE = f1903.CUST_CODE
				});
			}
			else
			{
				if (VolumeItem.PACK_LENGTH != f190301.LENGTH || VolumeItem.PACK_WIDTH != f190301.WIDTH
					|| VolumeItem.PACK_HIGHT != f190301.HIGHT || VolumeItem.PACK_WEIGHT != f190301.WEIGHT)
				{
					f190301.LENGTH = VolumeItem.PACK_LENGTH;
					f190301.WIDTH = VolumeItem.PACK_WIDTH;
					f190301.HIGHT = VolumeItem.PACK_HIGHT;
					f190301.WEIGHT = VolumeItem.PACK_WEIGHT;
					repoF190301.Update(f190301);
				}
			}

			// 3. 更新F1905 - 先檢查是否有資料
			var f1905 = repoF1905.Find(x => x.GUP_CODE == VolumeItem.GUP_CODE && x.CUST_CODE == VolumeItem.CUST_CODE && x.ITEM_CODE == VolumeItem.ITEM_CODE);
			if (f1905 == null)
			{
				//3.1. 新增F1905
				repoF1905.Add(VolumeItem);
			}
			else
			{
				if (VolumeItem.PACK_LENGTH != f1905.PACK_LENGTH || VolumeItem.PACK_WIDTH != f1905.PACK_WIDTH
					|| VolumeItem.PACK_HIGHT != f1905.PACK_HIGHT || VolumeItem.PACK_WEIGHT != f1905.PACK_WEIGHT)
				{
					//3.2.1 設定F1905
					f1905 = SetObject(VolumeItem, f1905) as F1905;
					//3.2.2 更新F1905
					repoF1905.Update(f1905);
				}
			}

			//4.1 設定F190305
			var repoF190305 = new F190305Repository(Schemas.CoreSchema, _wmsTransaction);
			var f190305 = repoF190305.AsForUpdate().GetDatasByTrueAndCondition(x => x.GUP_CODE == palletLevel.GUP_CODE && x.CUST_CODE == palletLevel.CUST_CODE && x.ITEM_CODE == palletLevel.ITEM_CODE).FirstOrDefault();
			if (f190305 == null)
			{
				repoF190305.Add(palletLevel);
			}
			else
			{
				if (palletLevel.PALLET_LEVEL_CASEQTY != f190305.PALLET_LEVEL_CASEQTY || palletLevel.PALLET_LEVEL_CNT != f190305.PALLET_LEVEL_CNT)
				{
					f190305 = SetObject(palletLevel, f190305) as F190305;
					repoF190305.Update(f190305);
				}
			}

			repoF1903 = null;
			repoF1905 = null;
			repoF190305 = null;
			repoF190301 = null;
			return result;
		}

		public ExecuteResult DeleteP190102(string gupCode, string custCode, string itemCode)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);

			var f1903 = f1903Repo.Find(x => x.ITEM_CODE == itemCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode);

			// 先判斷存不存在該項目, 不存在就回傳該資料已糟刪除
			if (f1903 == null || f1903.SND_TYPE != "0")
			{
				return new ExecuteResult(false, Properties.Resources.P190102Service_CustItemDataDeleted);
			}

			// 檢查進倉單和出貨單中是否有該商品，若沒有可以做刪除(刪除註記)，若有請出提示訊息""該商品仍然在使用，無法刪除"""
			if (f010201Repo.ExistsNonCancelByItemCode(gupCode, custCode, itemCode)
			|| f050801Repo.ExistsNonCancelByItemCode(gupCode, custCode, itemCode))
			{
				return new ExecuteResult(false, Properties.Resources.P190102Service_UsingItemCannotDelete);
			}

			f1903.SND_TYPE = "9";
			f1903Repo.Update(f1903);

			return new ExecuteResult(true);
		}

		private object SetObject(object sourceItem, object targetItem)
		{
			var props = sourceItem.GetType().GetProperties();
			foreach (PropertyInfo prop in props)
			{
				if (prop.Name.ToLower() == "item" || prop.Name.ToLower() == "error" || prop.Name.ToUpper() =="CRT_NAME" || prop.Name.ToUpper() == "CRT_STAFF" || prop.Name.ToUpper() == "CRT_DATE") continue;
				string propName = prop.Name;
				var sourceValue = prop.GetValue(sourceItem, null);
				var targetProp = targetItem.GetType().GetProperty(propName);
				if (targetProp == null) continue;
				targetProp.SetValue(targetItem, sourceValue);
			}
			return targetItem;
		}

		public IQueryable<F190102JoinF000904> GetF190102JoinF000904Datas(string DC_CODE, string TOPIC, string Subtopic)
		{
			var repoF190102 = new F190102Repository(Schemas.CoreSchema);
			var result = repoF190102.GetF190102JoinF000904Datas(DC_CODE, TOPIC, Subtopic);
			return result;
		}

		public ExecuteResult ImportData(string gupCode, string custCode, List<string[]> items, string fileName)
		{
			var log = new P160101Service(_wmsTransaction);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1905Repo = new F1905Repository(Schemas.CoreSchema, _wmsTransaction);
			var f190301Repo = new F190301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f190305Repo = new F190305Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1924Repo = new F1924Repository(Schemas.CoreSchema);
			var addF1903List = new List<F1903>();
			var addF1905List = new List<F1905>();
			var addF190301List = new List<F190301>();
			var addF190305List = new List<F190305>();
			var updF1903List = new List<F1903>();
			var updF1905List = new List<F1905>();
			var updF190301List = new List<F190301>();

			foreach (var item in items)
			{
				var col = item;
				var message = CheckImportData(gupCode, custCode, ref col);
				if (string.IsNullOrEmpty(message))
				{
					var f1903 = CreateF1903(gupCode, custCode, col);
					var itemCode = col[0];
					var findDbF1903 = f1903Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == itemCode);

					if (addF1903List.Any(o => o.ITEM_CODE == col[0]) || updF1903List.Any(x=> x.ITEM_CODE == col[0]))
						message = Properties.Resources.P190102Service_ImportItem_Duplicate;
					else if (findDbF1903 != null && findDbF1903.SND_TYPE == "9")
						message = Properties.Resources.P190102Service_ItemNo_DisContinuedItem_Duplicate;
					//檢查商品負責人是否在員工主檔
					else if (!string.IsNullOrEmpty(f1903.ITEM_STAFF) && 
						       !f1924Repo.Filter(x => x.EMP_ID == EntityFunctions.AsNonUnicode(f1903.ITEM_STAFF) &&
									 x.ISDELETED == EntityFunctions.AsNonUnicode("0")).Any()) 
							message = Properties.Resources.P190102Service_ItemInChargeNotFound;
					else
					message = string.Empty;

					if (string.IsNullOrWhiteSpace(message))
					{
						// 供應商最低訂量 預設0
						if (!f1903.VEN_ORD.HasValue || f1903.VEN_ORD <= 0)
							f1903.VEN_ORD = 0;
						var f1905 = CreateF1905(gupCode, custCode, col);
						var f190301 = CreateF190301(gupCode, custCode, col);
						var f190305 = CreateF190305(gupCode, custCode, col);
						if (findDbF1903 == null)
							addF1903List.Add(f1903);
						else
						{
							f1903 = SetObject(f1903, findDbF1903) as F1903;
							updF1903List.Add(f1903);
						}
							

						var findDbF1905 = f1905Repo.Find(x => x.GUP_CODE == f1903.GUP_CODE && x.CUST_CODE == f1903.CUST_CODE && x.ITEM_CODE == f1903.ITEM_CODE);
						if (findDbF1905 == null)
							addF1905List.Add(f1905);
						else
						{
							if (findDbF1905.PACK_LENGTH != f1905.PACK_LENGTH || findDbF1905.PACK_WIDTH != f1905.PACK_WIDTH || findDbF1905.PACK_HIGHT != f1905.PACK_HIGHT)
							{
								UpdateF1905(findDbF1905, f1905);
								updF1905List.Add(findDbF1905);
							}
						}
						var findDbF190301 = f190301Repo.Find(x => x.GUP_CODE == f1903.GUP_CODE && x.CUST_CODE == f1903.CUST_CODE && x.ITEM_CODE == f1903.ITEM_CODE && x.UNIT_LEVEL == 1);
						if (findDbF190301 == null)
							addF190301List.Add(f190301);
						else
						{
							if (findDbF190301.LENGTH != f190301.LENGTH || findDbF190301.WIDTH != f190301.WIDTH
								|| findDbF190301.HIGHT != f190301.HIGHT || findDbF190301.WEIGHT != f190301.WEIGHT)
							{
								UpdateF190301(findDbF190301, f190301);
								updF190301List.Add(findDbF190301);
							}
						}
						var findDbF190305 = f190305Repo.Find(x => x.GUP_CODE == f1903.GUP_CODE && x.CUST_CODE == f1903.CUST_CODE && x.ITEM_CODE == f1903.ITEM_CODE);
						if (findDbF190305 == null)
							addF190305List.Add(f190305);
					}
				}
				log.UpdateF0060Log(fileName, "0", col[0], string.Join(",", col), message, "", gupCode, custCode);
			}

			f1903Repo.BulkInsert(addF1903List);
			f1905Repo.BulkInsert(addF1905List);
			f190301Repo.BulkInsert(addF190301List);
			f190305Repo.BulkInsert(addF190305List);
			f1903Repo.BulkUpdate(updF1903List);
			f1905Repo.BulkUpdate(updF1905List);
			f190301Repo.BulkUpdate(updF190301List);

			return new ExecuteResult { IsSuccessed = true, Message = string.Format(Properties.Resources.P190102Service_ImportResult, addF190301List.Count, items.Count - addF190301List.Count - updF1903List.Count, updF1903List.Count, items.Count) };
		}


		public F1903 CreateF1903(string gupCode, string custCode, string[] col)
		{
			var f1903 = new F1903
			{
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ITEM_CODE = col[0],
				ITEM_NAME = col[1],
				ITEM_ENGNAME = col[2],
				ITEM_NICKNAME = col[3],
				ITEM_TYPE = col[4],
				ITEM_UNIT = col[5],
				ITEM_SPEC = col[6],
				ITEM_COLOR = col[7],
				ITEM_CLASS = col[8],
				SIM_SPEC = col[9],
				EAN_CODE1 = col[10],
				EAN_CODE2 = col[11],
				EAN_CODE3 = col[12],
				LTYPE = col[13],
				MTYPE = col[14],
				STYPE = col[15],
				TYPE = col[16],
				ITEM_ATTR = col[17],
				TMPR_TYPE = col[18],
				VIRTUAL_TYPE = col[20],
				MEMO = col[25],
				FRAGILE = (string.IsNullOrEmpty(col[26])) ? "0" : col[26],
				SPILL = (string.IsNullOrEmpty(col[27])) ? "0" : col[27],
				ISAPPLE = (string.IsNullOrEmpty(col[28])) ? "0" : col[28],
				PICK_SAVE_QTY = int.Parse(col[30]),
				PICK_SAVE_ORD = int.Parse(col[32]),
				ORD_SAVE_QTY = int.Parse(col[33]),
				SERIAL_BEGIN = col[37],
				SERIAL_RULE = col[38],
				PICK_WARE = col[39],
				CUST_ITEM_CODE = col[40],
				C_D_FLAG = col[41],
				BUNDLE_SERIALLOC = col[42],
				BUNDLE_SERIALNO = col[43],
				MIX_BATCHNO = col[44],
				LOC_MIX_ITEM = col[45],
				ALLOWORDITEM = col[46],
				NO_PRICE = col[47],
				ISCARTON = col[48],
				ITEM_STAFF = col[53],
				CAN_SPILT_IN = col[54],
				LG = col[55],
				ACC_TYPE = col[56],
				ITEM_SIZE = col[21] + "*" + col[22] + "*" + col[23],
				CAN_SELL = "1",
				SND_TYPE = "0",
				BOUNDLE_SERIALREQ = "0",
				AMORTIZATION_NO = 0,
				MAKENO_REQU = "0"
			};
			short col19;
			if (!string.IsNullOrEmpty(col[19]) && short.TryParse(col[19], out col19))
				f1903.ITEM_HUMIDITY = col19;
			decimal col29;
			if (!string.IsNullOrEmpty(col[29]) && decimal.TryParse(col[29], out col29))
				f1903.CHECK_PERCENT = col29;
			short col31;
			if (!string.IsNullOrEmpty(col[31]) && short.TryParse(col[31], out col31))
				f1903.SAVE_DAY = col31;
			short col34;
			if (!string.IsNullOrEmpty(col[34]) && short.TryParse(col[34], out col34))
				f1903.BORROW_DAY = col34;
			int col35;
			if (!string.IsNullOrEmpty(col[35]) && int.TryParse(col[35], out col35))
				f1903.EP_TAX = col35;
			short col36;
			if (!string.IsNullOrEmpty(col[36]) && short.TryParse(col[36], out col36))
				f1903.SERIALNO_DIGIT = col36;
			int col49;
			if (!string.IsNullOrEmpty(col[49]) && int.TryParse(col[49], out col49))
				f1903.VEN_ORD = col49;
			int col50;
			if (!string.IsNullOrEmpty(col[50]) && int.TryParse(col[50], out col50))
				f1903.RET_ORD = col50;
			short col51;
			if (!string.IsNullOrEmpty(col[51]) && short.TryParse(col[51], out col51))
				f1903.ALL_DLN = col51;
			short col52;
			if (!string.IsNullOrEmpty(col[52]) && short.TryParse(col[52], out col52))
				f1903.ALLOW_ALL_DLN = col52;
			short col57;
			if (!string.IsNullOrEmpty(col[57]) && short.TryParse(col[57], out col57))
				f1903.DELV_QTY_AVG = col57;
			return f1903;
		}

		public F1905 CreateF1905(string gupCode, string custCode, string[] col)
		{
			var f1905 = new F1905
			{
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ITEM_CODE = col[0],
				PACK_LENGTH = decimal.Parse(col[21]),
				PACK_WIDTH = decimal.Parse(col[22]),
				PACK_HIGHT = decimal.Parse(col[23]),
				PACK_WEIGHT = (string.IsNullOrEmpty(col[24])) ? 0 : decimal.Parse(col[24]),
			};
			return f1905;
		}

		public F190305 CreateF190305(string gupCode, string custCode, string[] col)
		{
			var f190305 = new F190305
			{
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ITEM_CODE = col[0],
				PALLET_LEVEL_CASEQTY = 1,
				PALLET_LEVEL_CNT = 1,
			};
			return f190305;
		}

		public F190301 CreateF190301(string gupCode,string custCode, string[] col)
		{
			return new F190301
			{
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ITEM_CODE = col[0],
				UNIT_LEVEL = 1,
				UNIT_QTY = 1,
				LENGTH = decimal.Parse(col[21]),
				WIDTH = decimal.Parse(col[22]),
				HIGHT = decimal.Parse(col[23]),
				WEIGHT = (string.IsNullOrEmpty(col[24])) ? 0 : decimal.Parse(col[24]),
				UNIT_ID = col[5]
			};
		}

		public string CheckImportData(string gupCode, string custCode, ref string[] col)
		{
			string message = string.Empty;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_ItemNo, ref col[0], 20, ref message, true))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_ItemName, ref col[1], 300, ref message, true))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_ItemEnglishName, ref col[2], 400, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_ItemHideName, ref col[3], 300, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_MaterialsType, ref col[4], 4, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_Unit, ref col[5], 5, ref message, true))
				return message;
			if (!CheckItemUnit(col[5], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_Spec, ref col[6], 80, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_Color, ref col[7], 80, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_Product_Hierarchy, ref col[8], 18, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_SIM_Spec, ref col[9], 40, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_EANCODE1, ref col[10], 26, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_EANCODE2, ref col[11], 26, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_EANCODE3, ref col[12], 26, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_LType, ref col[13], 6, ref message))
				return message;
			if (!CheckLType(gupCode, custCode, col[13], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_MType, ref col[14], 20, ref message))
				return message;
			if (!CheckMType(gupCode, custCode, col[13], col[14], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_SType, ref col[15], 20, ref message))
				return message;
			if (!CheckSType(gupCode, custCode, col[13], col[14], col[15], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_ItemType, ref col[16], 30, ref message, true))
				return message;
			if (!CheckF000904("F1903", "TYPE", Properties.Resources.P190102Service_ItemType, col[16], ref message, true))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_ItemAttribute, ref col[17], 2, ref message))
				return message;
			if (!CheckF000904("F1903", "ITEM_ATTR", Properties.Resources.P190102Service_ItemAttribute, col[17], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_TMPR_TYPE, ref col[18], 2, ref message, true))
				return message;
			if (!CheckF000904("F1903", "TMPR_TYPE", Properties.Resources.P190102Service_TMPR_TYPE, col[18], ref message, true))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_ITEM_HUMIDITY, ref col[19], 3, 0, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_VIRTUAL_TYPE, ref col[20], 10, ref message))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_PackLengthByView, ref col[21], 8, 2, ref message, true))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_PackWidthByView, ref col[22], 8, 2, ref message, true))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_PackHightByView, ref col[23], 8, 2, ref message, true))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_PackWeightByView, ref col[24], 10, 2, ref message, true))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_MEMO, ref col[25], 100, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_FRAGILE, ref col[26], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_FRAGILE, col[26], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_SPILL, ref col[27], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_SPILL, col[27], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_ISApple, ref col[28], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_ISApple, col[28], ref message))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_CHECK_PERCENT, ref col[29], 13, 11, ref message, true, "0.00000000001"))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_PICK_SAVE_QTY, ref col[30], 10, 0, ref message, false, "1"))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_SAVE_DAY, ref col[31], 5, 0, ref message))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_PICK_SAVE_ORD, ref col[32], 9, 0, ref message, false, "1"))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_ORD_SAVE_QTY, ref col[33], 10, 0, ref message, false, "1"))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_BORROW_DAY, ref col[34], 4, 0, ref message, true))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_EnviromentalTax, ref col[35], 9, 0, ref message))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_SERIALNO_DIGIT, ref col[36], 3, 0, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_SERIAL_BEGIN, ref col[37], 10, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_ItemSerialRule, ref col[38], 1, ref message, false, "0"))
				return message;
			if (!CheckF000904("F1903", "SERIAL_RULE", Properties.Resources.P190102Service_ItemSerialRule, col[38], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_PICK_WARE_ID, ref col[39], 2, ref message, true, "G"))
				return message;
			if (!CheckPickWare(Properties.Resources.P190102Service_PICK_WARE_ID, col[39], ref message, true))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_CUST_ITEM_CODE, ref col[40], 20, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_CrossItemMemo, ref col[41], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_CrossItemMemo, col[41], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_BUNDLE_SERIALLOC, ref col[42], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_BUNDLE_SERIALLOC, col[42], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_BUNDLE_SERIALNO, ref col[43], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_BUNDLE_SERIALNO, col[43], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_MIX_BATCHNO, ref col[44], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_MIX_BATCHNO, col[44], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_LOC_MIX_ITEM, ref col[45], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_LOC_MIX_ITEM, col[45], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_ALLOWORDITEM, ref col[46], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_ALLOWORDITEM, col[46], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_NO_PRICE, ref col[47], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_NO_PRICE, col[47], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_ISCARTON, ref col[48], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_ISCARTON, col[48], ref message))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_VEN_ORD, ref col[49], 6, 0, ref message, false, "0"))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_RET_ORD, ref col[50], 6, 0, ref message))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_ALL_DLN, ref col[51], 4, 0, ref message))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_ALLOW_ALL_DLN, ref col[52], 4, 0, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_ITEM_STAFF, ref col[53], 20, ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_CAN_SPILT_IN, ref col[54], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_CAN_SPILT_IN, col[54], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_LG, ref col[55], 1, ref message, false, "0"))
				return message;
			if (!CheckTrueFalse(Properties.Resources.P190102Service_LG, col[55], ref message))
				return message;
			if (!CheckLenghAndRequired(Properties.Resources.P190102Service_ACC_TYPE, ref col[56], 2, ref message))
				return message;
			if (!CheckF000904("F1903", "TYPE", Properties.Resources.P190102Service_ACC_TYPE, col[56], ref message, true))
				return message;
			if (!CheckLenghAndRequiredByNumber(Properties.Resources.P190102Service_DELV_QTY_AVG, ref col[57], 9, 0, ref message, false, "0"))
				return message;
			return message;
		}

		private bool CheckLenghAndRequired(string colName, ref string colValue, int maxLength, ref string message, bool required = false, string defaultValue = null)
		{
			if (colValue.Trim().Length == 0 && defaultValue != null)
				colValue = defaultValue;
			if (required && colValue.Trim().Length == 0)
				message = string.Format(Properties.Resources.P190102Service_Required, colName);
			else if (colValue.Trim().Length > maxLength)
				message = string.Format(Properties.Resources.P190102Service_LengthLessThan, colName, colValue, maxLength);
			return string.IsNullOrEmpty(message);
		}
		private bool CheckLenghAndRequiredByNumber(string colName, ref string colValue, int maxLength, int digitLength, ref string message, bool required = false, string defaultValue = null)
		{
			if (colValue.Trim().Length == 0 && defaultValue != null)
				colValue = defaultValue;
			var item = colValue.Split('.');
			if (required && colValue.Trim().Length == 0)
				message = string.Format(Properties.Resources.P190102Service_Required, colName);
			decimal value;
			if (colValue.Trim().Length > 0 && !decimal.TryParse(colValue.Trim(), out value))
				message = string.Format(Properties.Resources.P190102Service_MustBeNumber, colName);
			else if (colValue.Trim().Length > 0 && (item[0].Length > maxLength || (item.Length > 2 && item[1].Length > digitLength)))
				message = string.Format(Properties.Resources.P190102Service_Format, colName, maxLength, digitLength);
			return string.IsNullOrEmpty(message);
		}


		private bool CheckItemUnit(string colValue, ref string message)
		{
			var f91000302Repo = new F91000302Repository(Schemas.CoreSchema, _wmsTransaction);//_wmsTransaction為了使用Cache
			var item = f91000302Repo.Find(o => o.ITEM_TYPE_ID == "001" && o.ACC_UNIT == colValue);
			if (item == null)
				message = string.Format(Properties.Resources.P190102Service_UnitNotExist, colValue);
			return string.IsNullOrEmpty(message);
		}

		private bool CheckLType(string gupCode, string custCode, string colValue, ref string message)
		{
			if (colValue.Trim().Length == 0)
				return true;
			var f1915Repo = new F1915Repository(Schemas.CoreSchema, _wmsTransaction);//_wmsTransaction為了使用Cache
			var item = f1915Repo.Find(o => o.GUP_CODE == gupCode && o.ACODE == colValue && o.CUST_CODE == custCode);
			if (item == null)
				message = string.Format(Properties.Resources.P190102Service_LTypeNotExist, colValue);
			return string.IsNullOrEmpty(message);
		}

		private bool CheckMType(string gupCode, string custCode, string lType, string colValue, ref string message)
		{
			if (colValue.Trim().Length == 0)
				return true;
			var f1916Repo = new F1916Repository(Schemas.CoreSchema, _wmsTransaction);//_wmsTransaction為了使用Cache
			var item = f1916Repo.Find(o => o.GUP_CODE == gupCode && o.ACODE == lType && o.BCODE == colValue && o.CUST_CODE == custCode);
			if (item == null)
				message = string.Format(Properties.Resources.P190102Service_MTypeNotExist, colValue);
			return string.IsNullOrEmpty(message);
		}

		private bool CheckSType(string gupCode, string custCode, string lType, string mType, string colValue, ref string message)
		{
			if (colValue.Trim().Length == 0)
				return true;
			var f1917Repo = new F1917Repository(Schemas.CoreSchema, _wmsTransaction);//_wmsTransaction為了使用Cache
			var item = f1917Repo.Find(o => o.GUP_CODE == gupCode && o.ACODE == lType && o.BCODE == mType && o.CCODE == colValue && o.CUST_CODE == custCode);
			if (item == null)
				message = string.Format(Properties.Resources.P190102Service_STypeNotExist, colValue);
			return string.IsNullOrEmpty(message);
		}

		private bool CheckF000904(string topic, string subTopic, string colName, string colValue, ref string message, bool required = false)
		{
			if (!required && colValue.Trim().Length == 0)
				return true;
			var f000904Repo = new F000904Repository(Schemas.CoreSchema, _wmsTransaction);//_wmsTransaction為了使用Cache
			var item = f000904Repo.Find(o => o.TOPIC == topic && o.SUBTOPIC == subTopic && o.VALUE == colValue);
			if (item == null)
				message = string.Format(Properties.Resources.P190102Service_NotExist, colName, colValue);
			return string.IsNullOrEmpty(message);
		}

		private bool CheckTrueFalse(string colName, string colValue, ref string message)
		{
			if (colValue != "0" && colValue != "1")
				message = string.Format(Properties.Resources.P190102Service_MustGreaterThanZeroLessThanOne, colName);
			return string.IsNullOrEmpty(message);
		}

		private bool CheckPickWare(string colName, string colValue, ref string message, bool required = false)
		{
			if (!required && colValue.Trim().Length == 0)
				return true;
			var f198001Repo = new F198001Repository(Schemas.CoreSchema, _wmsTransaction);//_wmsTransaction為了使用Cache
			var item = f198001Repo.Find(o => o.TYPE_ID == colValue);
			if (item == null)
				message = string.Format(Properties.Resources.P190102Service_NotExist, colName, colValue);
			return string.IsNullOrEmpty(message);

		}

		// 改為只寫入F1903，以F1903為主要檔案
		public ExecuteResult InsertP190102(F1903 f1903Data, F1905 VolumeItem, F190305 palletLevel)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var f190301Repo = new F190301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1905Repo = new F1905Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1909repo = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1924Repo = new F1924Repository(Schemas.CoreSchema);

			// 商品主檔建立是以貨主為角度，故業主只是沒資料就新增，已有資料就更新。
			var f1903 = f1903Repo.Find(x => x.GUP_CODE == f1903Data.GUP_CODE
																	 && x.ITEM_CODE == f1903Data.ITEM_CODE
																	 && x.CUST_CODE == f1903Data.CUST_CODE);
			var isAddF1903 = (f1903 == null);
			if (isAddF1903)
			{
				f1903 = f1903Data;
				f1903.AMORTIZATION_NO = 0;
				f1903.BOUNDLE_SERIALREQ = "0";
			}

			var message = ValidF1903Data(f1903Repo, f1924Repo, f1903, f1903Data.GUP_CODE, f1903Data.CUST_CODE);
			if (!string.IsNullOrEmpty(message))
				return new ExecuteResult { IsSuccessed = false, Message = message };

			// 1. 新增主檔
			if (isAddF1903)
			{
				f1903Repo.Add(f1903);
			}
			else
			{
				f1903Repo.Update(f1903);
			}

			// 2. 新增材積主檔
			if (f1905Repo.Find(x => x.GUP_CODE == VolumeItem.GUP_CODE && x.CUST_CODE == VolumeItem.CUST_CODE && x.ITEM_CODE == VolumeItem.ITEM_CODE) == null)
				f1905Repo.Add(VolumeItem);

			// 3.建立新商品同時，自動建立一筆商品包裝資料F190301
			if (f190301Repo.Find(x => x.GUP_CODE == f1903Data.GUP_CODE
			                       && x.CUST_CODE == f1903Data.CUST_CODE
														 && x.ITEM_CODE == f1903Data.ITEM_CODE
														 && x.UNIT_ID == f1903.ITEM_UNIT) == null)
			{
				f190301Repo.Add(new F190301
				{
					GUP_CODE = f1903Data.GUP_CODE,
					ITEM_CODE = f1903Data.ITEM_CODE,
					UNIT_LEVEL = 1,
					UNIT_QTY = 1,
					LENGTH = VolumeItem.PACK_LENGTH,
					WIDTH = VolumeItem.PACK_WIDTH,
					HIGHT = VolumeItem.PACK_HIGHT,
					WEIGHT = VolumeItem.PACK_WEIGHT,
					UNIT_ID = f1903.ITEM_UNIT,
					CUST_CODE = f1903.CUST_CODE
				});
			}

			//4.1 設定F190305
			var repoF190305 = new F190305Repository(Schemas.CoreSchema, _wmsTransaction);
			var f190305 = repoF190305.AsForUpdate().GetDatasByTrueAndCondition(x => x.GUP_CODE == palletLevel.GUP_CODE && x.CUST_CODE == palletLevel.CUST_CODE && x.ITEM_CODE == palletLevel.ITEM_CODE).FirstOrDefault();
			if (f190305 == null)
			{
				repoF190305.Add(palletLevel);
			}


			return new ExecuteResult(true);
		}

		public string ValidF1903Data(F1903Repository f1903Repo, F1924Repository f1924Repo, F1903 f1903, string gupCode, string custCode, bool isAdd = true)
		{
			if (isAdd)
			{
				var f1903Item = f1903Repo.Find(o => o.GUP_CODE == f1903.GUP_CODE
																				 && o.CUST_CODE == f1903.CUST_CODE
																				 && o.ITEM_CODE == f1903.ITEM_CODE);
				//新增GoHappy商品主檔更新功能
				//var f1902Item = f1902Repo.Find(o => o.GUP_CODE == f1902.GUP_CODE && o.ITEM_CODE == f1902.ITEM_CODE);
				// var f1905 = CreateF1905(gupCode, col);
				if (f1903Item != null)
				{
					if (f1903Item.SND_TYPE == "9")
					{
						return (f1903Item.SND_TYPE == "9") ? Properties.Resources.P190102Service_ItemNo_DisContinuedItem_Duplicate : Properties.Resources.P190102Service_ItemNo_Duplicate;
					}
				}
			}
			// 供應商最低訂量 預設0
			if (!f1903.VEN_ORD.HasValue || f1903.VEN_ORD <= 0)
				f1903.VEN_ORD = 0;

			//檢查商品負責人是否在員工主檔
			if (!string.IsNullOrEmpty(f1903.ITEM_STAFF))
			{
				if (!f1924Repo.Filter(x => x.EMP_ID == EntityFunctions.AsNonUnicode(f1903.ITEM_STAFF)
																&& x.ISDELETED == EntityFunctions.AsNonUnicode("0"))
											.Any())
				{
					return Properties.Resources.P190102Service_ItemInChargeNotFound;
				}
			}

			//檢查商品國際條碼1~3
			if (DataCheckHelper.CheckStringIncludeWhiteSpace(f1903.EAN_CODE1))
			{
				return string.Format("刷入的國際條碼[{0}]中有空白，請重新刷入", f1903.EAN_CODE1);
			}
			if (DataCheckHelper.CheckStringIncludeWhiteSpace(f1903.EAN_CODE2))
			{
				return string.Format("刷入的條碼二[{0}]中有空白，請重新刷入", f1903.EAN_CODE2);
			}
			if (DataCheckHelper.CheckStringIncludeWhiteSpace(f1903.EAN_CODE3))
			{
				return string.Format("刷入的條碼三[{0}]中有空白，請重新刷入", f1903.EAN_CODE3);
			}

			return string.Empty;
		}
	}
}

