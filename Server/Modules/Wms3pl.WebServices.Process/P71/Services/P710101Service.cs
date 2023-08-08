
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using AutoMapper;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710101Service
	{
		private WmsTransaction _wmsTransaction;
		public P710101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F1980Data> GetF1980Datas(string dcCode, string gupCode, string custCode, string typeId, string account)
		{
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			return f1980Repo.GetF1980Datas(dcCode, gupCode, custCode, typeId, account);
		}

		public ExecuteResult CheckF1980Data(bool isNew, F1980Data f1980Data, List<F1912> f1912Data)
		{
			var result = new ExecuteResult();
			if (f1980Data.WAREHOUSE_ID == null)
				f1980Data.WAREHOUSE_ID = "-1";
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);

			if (isNew)
			{
				if (f1980Repo.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(f1980Data.DC_CODE)
									  && x.WAREHOUSE_NAME == f1980Data.WAREHOUSE_Name
									  && x.WAREHOUSE_ID != EntityFunctions.AsNonUnicode(f1980Data.WAREHOUSE_ID)).Any())
					return new ExecuteResult(false, string.Format("{0}倉別名稱不能重複!", f1980Data.WAREHOUSE_Name));
			}
            //同一個DC 倉別不可有重複的儲位區間範圍
            var splistList = SplitList(f1912Data.Select(o => o.LOC_CODE).ToList(), 2000);
            var data = new List<P710101DetailData>();
            foreach (var item in splistList)
            {
                data.AddRange(f1912Repo.GetLocDetailData(f1980Data.DC_CODE, item));

            }
			var otherData = data.Where(o => o.WAREHOUSE_ID != f1980Data.WAREHOUSE_ID).ToList();
			if (isNew)
				result.IsSuccessed = !data.Any();
			else
				result.IsSuccessed = !otherData.Any();
			if (!result.IsSuccessed)
			{
				if (isNew)
				{
					var minLocCode = f1912Data.OrderBy(o => o.LOC_CODE).FirstOrDefault().LOC_CODE;
					var maxLocCode = f1912Data.OrderByDescending(o => o.LOC_CODE).FirstOrDefault().LOC_CODE;
					result.Message = "【儲位範圍重複，請重新設定】" + Environment.NewLine + " 重複如下:";
					result.Message += Environment.NewLine + string.Format("[倉別]:{0} {1} [業主]:{2} [貨主]:{3} [儲區範圍]:{4} ~ {5}", f1980Data.WAREHOUSE_ID, f1980Data.WAREHOUSE_Name, f1980Data.GUP_CODE, f1980Data.CUST_CODE, minLocCode, maxLocCode);
				}
				else
				{
					var minLocCode = otherData.OrderBy(o => o.LOC_CODE).FirstOrDefault().LOC_CODE;
					var maxLocCode = otherData.OrderByDescending(o => o.LOC_CODE).FirstOrDefault().LOC_CODE;
					result.Message = "【儲位範圍重複，請重新設定】" + Environment.NewLine + " 重複如下:";
					result.Message += Environment.NewLine + string.Format("[倉別]:{0} {1} [業主]:{2} [貨主]:{3} [儲區範圍]:{4} ~ {5}", f1980Data.WAREHOUSE_ID, f1980Data.WAREHOUSE_Name, f1980Data.GUP_CODE, f1980Data.CUST_CODE, minLocCode, maxLocCode);
				}
			}
			else
			{
				if (f1980Data.WAREHOUSE_ID != "-1")
				{
					//新儲位資料
					var newWarehousef1912Data = GetNewWareHouseF1912Datas(f1980Data, string.Empty, f1912Data);
					//原儲位資料
					var orginalWarehouseF1912Data = GetOrginalWareHouseF1912Datas(f1980Data.DC_CODE,f1980Data.WAREHOUSE_ID).ToList();

					//取得原儲位不存在新儲位區間上儲位
					var allLocCodeList = newWarehousef1912Data.Select(o => o.LOC_CODE).Except(orginalWarehouseF1912Data.Select(o => o.LOC_CODE)).ToList();
					var f1913Repo = new F1913Repository(Schemas.CoreSchema);
                    var splitLoc = SplitList(allLocCodeList);
                    List<F1913> getF1913 = new List<F1913>();
                    foreach (var item in splitLoc)
                        getF1913.AddRange(f1913Repo.Filter(o => o.DC_CODE == EntityFunctions.AsNonUnicode(f1980Data.DC_CODE)
                                                            && item.Contains(o.LOC_CODE)));
                    result.IsSuccessed = !getF1913.Any();
                    //result.IsSuccessed = !f1913Repo.Filter(o => o.DC_CODE == EntityFunctions.AsNonUnicode(f1980Data.DC_CODE)
                    //                                        && allLocCodeList.Contains(o.LOC_CODE)).Any();
                    if (!result.IsSuccessed)
						result.Message = "儲區已存在商品";
				}
			}
			return result;

		}

		private int ChangeEnglishToNumber(string english)
		{
			var upString = english.ToUpper();
			int result = 0;
			for (int i = 0; i < english.Length; i++)
			{
				int letter = (int)upString[i];
				int a = 1;

				for (int x = 1; x < (english.Length - i); x++)
					a *= 36;
				if (i + 1 != english.Length)
				{
					if (letter >= 65 && letter <= 90)
						result += (letter - 55) * a;
					else if (letter >= 48 && letter <= 57)
						result += (letter - 48) * a;
				}
				else
				{
					if (letter >= 65 && letter <= 90)
						result += (letter - 55);
					else if (letter >= 48 && letter <= 57)
						result += (letter - 48);
				}
			}

			return result;
		}

		public ExecuteResult InsertF1980Data(F1980Data f1980Data, string userId, List<F1912> f1912Data)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f1980Repo = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);
			//新增倉別設定
			var f1980 = new F1980
			{
				WAREHOUSE_ID = f1980Data.WAREHOUSE_ID,
				WAREHOUSE_NAME = f1980Data.WAREHOUSE_Name,
				DC_CODE = f1980Data.DC_CODE,
				WAREHOUSE_TYPE = f1980Data.WAREHOUSE_TYPE,
				TMPR_TYPE = f1980Data.TMPR_TYPE,
				CAL_STOCK = f1980Data.CAL_STOCK,
				CAL_FEE = f1980Data.CAL_FEE,
				CRT_STAFF = userId,
				CRT_DATE = DateTime.Now,
				UPD_STAFF = userId,
				UPD_DATE = DateTime.Now,
				LOC_TYPE_ID = f1980Data.LOC_TYPE_ID,
				HOR_DISTANCE = f1980Data.HOR_DISTANCE,
				RENT_BEGIN_DATE = f1980Data.RENT_BEGIN_DATE,
				RENT_END_DATE = f1980Data.RENT_END_DATE,
				DEVICE_TYPE = f1980Data.DEVICE_TYPE,
			    PICK_FLOOR = f1980Data.PICK_FLOOR
			};
			f1980Repo.Add(f1980);

			//新增儲位設定
			var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
			var f191202Repo = new F191202Repository(Schemas.CoreSchema, _wmsTransaction);
			//f1980Data.WAREHOUSE_ID = warehouseId;
			var newF1912List = GetNewWareHouseF1912Datas(f1980Data, userId, f1912Data);
			if (!newF1912List.Any())
			{
				return new ExecuteResult { IsSuccessed = false, Message = "此倉庫必須設定儲位" };
			}
			var newF191202List = new List<F191202>();
			foreach (var f1912 in newF1912List)
				AddF191202Log(userId, "0", "0", f1912, newF191202List);

			f1912Repo.BulkInsert(newF1912List, "NOW_CUST_CODE");
			f191202Repo.BulkInsert(newF191202List, "TRANS_NO");
			return result;
		}

		public ExecuteResult UpdateF1980Data(F1980Data f1980Data, string userId, List<F1912> f1912Data)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f1980Repo = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1980Item = f1980Repo.Find(o => o.WAREHOUSE_ID == f1980Data.WAREHOUSE_ID && o.DC_CODE == f1980Data.DC_CODE);
			f1980Item.TMPR_TYPE = f1980Data.TMPR_TYPE;
			f1980Item.CAL_STOCK = f1980Data.CAL_STOCK;
			f1980Item.CAL_FEE = f1980Data.CAL_FEE;
			if (!string.IsNullOrEmpty(f1980Data.LOC_TYPE_ID))
				f1980Item.LOC_TYPE_ID = f1980Data.LOC_TYPE_ID;
			if (f1980Data.HOR_DISTANCE.HasValue)
				f1980Item.HOR_DISTANCE = f1980Data.HOR_DISTANCE;
			if (f1980Data.IsModifyDate)
				f1980Item.RENT_BEGIN_DATE = f1980Data.RENT_BEGIN_DATE;
			if (f1980Data.IsModifyDate)
				f1980Item.RENT_END_DATE = f1980Data.RENT_END_DATE;
			f1980Item.UPD_STAFF = userId;
			f1980Item.UPD_DATE = DateTime.Now;
			f1980Item.DEVICE_TYPE = f1980Data.DEVICE_TYPE;
			f1980Item.PICK_FLOOR = f1980Data.PICK_FLOOR;
			f1980Repo.Update(f1980Item);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);

			var newF1912List = GetNewWareHouseF1912Datas(f1980Data, userId, f1912Data);
			//原儲位資料
			var orginalWarehouseF1912Data = GetOrginalWareHouseF1912Datas(f1980Data.DC_CODE,f1980Data.WAREHOUSE_ID).ToList();

			//取得原儲位不存在新儲位區間上儲位
			var allLocCodeList = orginalWarehouseF1912Data.Select(o => o.LOC_CODE).Except(newF1912List.Select(o => o.LOC_CODE)).ToList();

			var f191202List = new List<F191202>();
            //刪除儲位
            //var f1912List = f1912Repo.Filter(o => allLocCodeList.Contains(o.LOC_CODE) && o.DC_CODE == f1980Data.DC_CODE && o.GUP_CODE == f1980Data.GUP_CODE && o.CUST_CODE == f1980Data.CUST_CODE).ToList();
            var f1912List = f1912Repo.GetLocCodeDataSQL(f1980Data.DC_CODE, f1980Data.GUP_CODE, f1980Data.CUST_CODE, allLocCodeList);

            var f191202Repo = new F191202Repository(Schemas.CoreSchema, _wmsTransaction);
			var listAreaCode = new List<string>();
			foreach (var f1912 in f1912List)
			{
				if (!listAreaCode.Contains(f1912.AREA_CODE) && !string.IsNullOrEmpty(f1912.AREA_CODE))
					listAreaCode.Add(f1912.AREA_CODE);
				//f1912Repo.Delete(o => o.DC_CODE == f1912.DC_CODE && o.LOC_CODE == f1912.LOC_CODE);
				//寫入異動檔
				AddF191202Log(userId, "1", "0", f1912, f191202List);
			}
			f1912Repo.DeleteLocByLocCode(f1980Data.DC_CODE, f1980Data.GUP_CODE, f1980Data.CUST_CODE, f1980Data.WAREHOUSE_ID, f1912List.Select(o => o.LOC_CODE).ToList());
			var f1919Repo = new F1919Repository(Schemas.CoreSchema, _wmsTransaction);

			//刪除儲區
			foreach (var areaCode in listAreaCode)
			{
				var areaF1912List = f1912Repo.GetF1912DatasByAreaCode(f1980Data.DC_CODE, areaCode).ToList();
				//當儲區排除刪除儲位後已經沒有儲位則刪除儲區
				if (!areaF1912List.Any(o => !allLocCodeList.Contains(o.LOC_CODE)))
					f1919Repo.Delete(o => o.DC_CODE == f1980Data.DC_CODE && o.WAREHOUSE_ID == f1980Data.WAREHOUSE_ID && o.AREA_CODE == areaCode);
			}

			//更新儲位
			var updateF1912Data = orginalWarehouseF1912Data.Where(o => !allLocCodeList.Contains(o.LOC_CODE)).ToList();
			bool isModify = (!string.IsNullOrEmpty(f1980Data.LOC_TYPE_ID)) || (f1980Data.HOR_DISTANCE.HasValue) ||
							(f1980Data.IsModifyDate);
			if (isModify)
			{
				var f1912upList = new List<F1912>();
				foreach (var f1912 in updateF1912Data)
				{
					bool isModify1 = false;
					bool isModify2 = false;
					bool isModify3 = false;
					bool isModify4 = false;
					var newUsefulVolumn = newF1912List.First(o => o.DC_CODE == f1912.DC_CODE && o.LOC_CODE == f1912.LOC_CODE).USEFUL_VOLUMN;

					if (!string.IsNullOrEmpty(f1980Data.LOC_TYPE_ID))
						isModify1 = (f1912.LOC_TYPE_ID != f1980Data.LOC_TYPE_ID || f1912.USEFUL_VOLUMN != newUsefulVolumn);
					if (f1980Data.HOR_DISTANCE.HasValue)
						isModify2 = f1912.HOR_DISTANCE != f1980Data.HOR_DISTANCE;
					if (f1980Data.IsModifyDate)
						isModify3 = f1912.RENT_BEGIN_DATE != f1980Data.RENT_BEGIN_DATE;
					if (f1980Data.IsModifyDate)
						isModify4 = f1912.RENT_END_DATE != f1980Data.RENT_END_DATE;

					if (!isModify1 && !isModify2 && !isModify3 && !isModify4)
						continue;
					AddF191202Log(userId, "2", "0", f1912, f191202List);
					//f191202Repo.Log(f1912, userId, "2", "0");
					if (isModify1)
					{
						f1912.LOC_TYPE_ID = f1980Data.LOC_TYPE_ID;
						f1912.USEFUL_VOLUMN = newUsefulVolumn;
					}

					if (isModify2)
						f1912.HOR_DISTANCE = f1980Data.HOR_DISTANCE;

					if (isModify3)
						f1912.RENT_BEGIN_DATE = f1980Data.RENT_BEGIN_DATE;

					if (isModify4)
						f1912.RENT_END_DATE = f1980Data.RENT_END_DATE;

					f1912.UPD_DATE = DateTime.Now;
					f1912.UPD_STAFF = userId;
					//f1912Repo.Update(f1912);
					f1912upList.Add(f1912);
					//寫入異動檔
					AddF191202Log(userId, "2", "1", f1912, f191202List);
				}
				if (f1912upList.Any())
					f1912Repo.BulkUpdate(f1912upList);
			}

			//新增儲位
			var addF1912Data = newF1912List.Where(o => !orginalWarehouseF1912Data.Any(c => c.LOC_CODE == o.LOC_CODE)).ToList();

			foreach (var f1912 in addF1912Data)
			{
				//要把原來倉別預設值給新儲位
				f1912.LOC_TYPE_ID = f1980Item.LOC_TYPE_ID;
				f1912.HOR_DISTANCE = f1980Item.HOR_DISTANCE;
				f1912.RENT_BEGIN_DATE = f1980Item.RENT_BEGIN_DATE;
				f1912.RENT_END_DATE = f1980Item.RENT_END_DATE;
				//f1912Repo.Add(f1912);
				//寫入異動檔
				AddF191202Log(userId, "0", "0", f1912, f191202List);
				//f191202Repo.Log(f1912, userId, "0", "0");
			}
			if (addF1912Data.Any())
				f1912Repo.BulkInsert(addF1912Data, "NOW_CUST_CODE");
			if (f191202List.Any())
				f191202Repo.BulkInsert(f191202List, "TRANS_NO");
			return result;
		}

		private void AddF191202Log(string userId, string status, string way, F1912 f1912, List<F191202> f191202s)
		{
			var tmp = Mapper.DynamicMap<F191202>(f1912);
			tmp.TRANS_DATE = DateTime.Now;
			tmp.TRANS_STAFF = userId;
			tmp.TRANS_STATUS = status;
			tmp.TRANS_WAY = way;
			f191202s.Add(tmp);
		}

		/// <summary>
		/// 建立新倉別儲位資料
		/// </summary>
		/// <param name="f1980Data"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		private List<F1912> GetNewWareHouseF1912Datas(F1980Data f1980Data, string userId, List<F1912> f1912Data)
		{
			var list = new List<F1912>();
			decimal usefulVolumn = 0;
			var f1942repo = new F1942Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1919Repo = new F1919Repository(Schemas.CoreSchema, _wmsTransaction);
			var f191201Repo = new F191201Repository(Schemas.CoreSchema, _wmsTransaction);
			//計算可用容積量
			var f1942Data = f1942repo.Find(x => x.LOC_TYPE_ID.Equals(f1980Data.LOC_TYPE_ID));
			if (f1942Data != null)
			{
				usefulVolumn = SharedService.GetUsefulColumn(f1942Data.LENGTH , f1942Data.DEPTH , f1942Data.HEIGHT , f1942Data.VOLUME_RATE); // VOLUME_RATE 是0~100，所以要除100
			}

			string areaCode = "";
			var f1919 = f1919Repo.Filter(x => x.DC_CODE.Equals(f1980Data.DC_CODE) && x.WAREHOUSE_ID.Equals(f1980Data.WAREHOUSE_ID))
																.AsQueryable();
			if (f1919.Count() > 0)
				areaCode = f1919.FirstOrDefault().AREA_CODE;

			foreach (var newData in f1912Data)
			{
				var f1912 = new F1912()
				{
					//樓編號+通道編號+座數+層編號+儲位編號
					LOC_CODE = newData.LOC_CODE,
					FLOOR = newData.FLOOR,
					CHANNEL = newData.CHANNEL.PadLeft(2, '0'),
					PLAIN = newData.PLAIN.PadLeft(2, '0'),
					LOC_LEVEL = newData.LOC_LEVEL.PadLeft(2, '0'),
					LOC_TYPE = newData.LOC_TYPE.PadLeft(2, '0'),
					WAREHOUSE_ID = f1980Data.WAREHOUSE_ID,
					AREA_CODE = "-1",
					DC_CODE = f1980Data.DC_CODE,
					GUP_CODE = f1980Data.GUP_CODE,
					CUST_CODE = f1980Data.CUST_CODE,
					HOR_DISTANCE = f1980Data.HOR_DISTANCE,
					RENT_BEGIN_DATE = f1980Data.RENT_BEGIN_DATE,
					RENT_END_DATE = f1980Data.RENT_END_DATE,
					LOC_TYPE_ID = f1980Data.LOC_TYPE_ID,
					USEFUL_VOLUMN = usefulVolumn,
					NOW_STATUS_ID = "01",
					PRE_STATUS_ID = "01",
					USED_VOLUMN = 0,
					CRT_STAFF = userId,
					CRT_DATE = DateTime.Now,
					UPD_STAFF = userId,
					UPD_DATE = DateTime.Now, 
					NOW_CUST_CODE = "0"
				};
				list.Add(f1912);

			}

			return list;
		}

		/// <summary>
		/// 原倉別儲位資料
		/// </summary>
		/// <param name="wareHouseId">倉別編號</param>
		/// <returns></returns>
		private IQueryable<F1912> GetOrginalWareHouseF1912Datas(string dcCode,string wareHouseId)
		{
			var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
			return f1912Repo.GetF1912DatasByWareHouseId(dcCode,wareHouseId);
		}

		public IQueryable<P710101DetailData> GetLocDetailData(string dcCode, List<string> locCodes)
		{
			var f1912repo = new F1912Repository(Schemas.CoreSchema);

            // SqlParameter參數數量超過2100個，分割List
            var chunkSize = 2000;
            var result = new List<P710101DetailData>().AsQueryable();
            var splits= SplitList(locCodes, chunkSize);

            foreach (var item in splits)
            {
                result = result.Union(f1912repo.GetLocDetailData(dcCode, item));
            }

            return result;

        }

        private List<List<string>> SplitList(List<string> source, int chunkSize = 2000)
        {
            var result = new List<List<string>>();
            var sourceCount = source.Count;
            for (var i = 0; i < sourceCount; i += chunkSize)
            {
                result.Add(source.GetRange(i, Math.Min(chunkSize, sourceCount - i)));
            }
            return result;
        }
	}
}

