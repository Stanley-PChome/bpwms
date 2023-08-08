using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710901Service
	{
		private WmsTransaction _wmsTransaction;

		public P710901Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F1947Ex> GetF1947ExQuery(string dcCode, string gupCode, string custCode, string allID, string allComp)
		{
			var f1947Repository = new F1947Repository(Schemas.CoreSchema);
			return f1947Repository.GetF1947ExQuery(dcCode, gupCode, custCode, allID, allComp);
		}

		public ExecuteResult InsertF1947New(F1947 master, List<F194701> f194701List, List<F19470101> f19470101List, List<F194708> f194708List,
			List<F19470801> f19470801List, List<F194709> f194709List, List<F194703> f194703List)
		{
			// 0. 檢查傳入參數
			if (master == null || f194701List == null || f19470101List == null ||
					string.IsNullOrWhiteSpace(master.DC_CODE) || string.IsNullOrWhiteSpace(master.ALL_ID))
			{
				return new ExecuteResult() { Message = "缺少配送商主檔資料" };
			}

			var f1947Repository = new F1947Repository(Schemas.CoreSchema, _wmsTransaction);

			// 1. 先檢查資料庫的內容
			var f1947 = f1947Repository.Find(item => item.ALL_ID == EntityFunctions.AsNonUnicode(master.ALL_ID)
																							&& item.DC_CODE == EntityFunctions.AsNonUnicode(master.DC_CODE));
			if (f1947 != null)
			{
				return new ExecuteResult() { Message = "該配送商編號已經存在, 不可重複新增" };
			}

			// 2. 新增資料，若新增失敗，表示 Client 沒填必要欄位資料
			f1947Repository.Add(master);

			// 3. 明細的部分直接新增，
			UpdateF194701sNew(master, f194701List, f19470101List);

			//4.計費區域
			InsertOrUpdateF194708(master, f194708List, f19470801List);
			//5.單量維護
			InsertOrUpdateF194709(master, f194709List);

			//6.車輛配送費用
			InsertOrUpdateF194703(master, f194703List);
			return new ExecuteResult() { IsSuccessed = true, Message = "新增配送商主檔完成" };
		}


		public ExecuteResult UpdateF1947New(F1947 master, List<F194701> details, List<F19470101> f19470101List, List<F194708> f194708List,
			List<F19470801> f19470801List, List<F194709> f194709List, List<F194703> f194703List)
		{
			// 0. 檢查傳入參數
			if (master == null || details == null || f19470101List == null ||
					string.IsNullOrWhiteSpace(master.DC_CODE) || string.IsNullOrWhiteSpace(master.ALL_ID))
			{
				return new ExecuteResult() { Message = "缺少配送商主檔資料" };
			}

			var f1947Repository = new F1947Repository(Schemas.CoreSchema, _wmsTransaction);

			// 1. 先檢查資料庫的內容
			var f1947 = f1947Repository.Find(item => item.ALL_ID == EntityFunctions.AsNonUnicode(master.ALL_ID)
																							&& item.DC_CODE == EntityFunctions.AsNonUnicode(master.DC_CODE));
			if (f1947 == null)
			{
				return new ExecuteResult() { Message = "資料已被刪除, 請重新查詢" };
			}

			// 2. 設定變更的資料
			f1947.ALL_COMP = master.ALL_COMP;
			f1947.PIER_CODE = master.PIER_CODE;
			f1947.CHECK_ROUTE = master.CHECK_ROUTE;
			f1947.TYPE = master.TYPE;
			f1947.ALLOW_ROUND_PIECE = master.ALLOW_ROUND_PIECE;
			f1947Repository.Update(f1947);

			// 3. 明細的部分直接先刪除，再新增，這樣不用一個一個檢查並更新。
			UpdateF194701sNew(master, details, f19470101List);
			//4.計費區域
			InsertOrUpdateF194708(master, f194708List, f19470801List);
			//5.單量維護
			InsertOrUpdateF194709(master, f194709List);

			//6.車輛配送費用
			InsertOrUpdateF194703(master, f194703List);
			return new ExecuteResult() { IsSuccessed = true, Message = "已更新配送商主檔" };
		}


		private void UpdateF194701sNew(F1947 master,
																	List<F194701> F194701List,
																	List<F19470101> F19470101List)
		{
			// 3. 明細的部分直接先刪除，再新增，這樣不用一個一個檢查並更新。

			//F194701
			var F194701Repository = new F194701Repository(Schemas.CoreSchema, _wmsTransaction);

			F194701Repository.Delete(item => item.ALL_ID == master.ALL_ID
														&& item.DC_CODE == master.DC_CODE);
			// P7109010000_ViewModel.DoAddTimeCompleted 會將 Detail 必要的欄位填入，若新增後發生錯誤，則表示 Client 沒帶入資料
			foreach (var F194701 in F194701List)
			{
				F194701.ALL_ID = master.ALL_ID;
				F194701.DC_CODE = master.DC_CODE;
				F194701Repository.Add(F194701);
			}

			//F19470101
			var F19470101Repository = new F19470101Repository(Schemas.CoreSchema, _wmsTransaction);
			F19470101Repository.Delete(item => item.ALL_ID == master.ALL_ID
															&& item.DC_CODE == master.DC_CODE);
			foreach (var F19470101 in F19470101List)
			{
				F19470101.ALL_ID = master.ALL_ID;
				F19470101.DC_CODE = master.DC_CODE;
				F19470101Repository.Add(F19470101);
			}

		}

		private void InsertOrUpdateF194708(F1947 master, List<F194708> f194708List, List<F19470801> f19470801List)
		{
			if (master != null && f194708List != null && f19470801List != null)
			{
				var f194708Repo = new F194708Repository(Schemas.CoreSchema, _wmsTransaction);
				var f19470801Repo = new F19470801Repository(Schemas.CoreSchema, _wmsTransaction);
				foreach (var f194708 in f194708List)
				{
					var list = f19470801List.Where(o => o.ACC_AREA_ID == f194708.ACC_AREA_ID).ToList();
					f19470801Repo.Delete(o => o.DC_CODE == master.DC_CODE && o.ALL_ID == master.ALL_ID && o.ACC_AREA_ID == f194708.ACC_AREA_ID);
					var item = f194708Repo.Find(o => o.DC_CODE == master.DC_CODE && o.ALL_ID == master.ALL_ID && o.ACC_AREA_ID == f194708.ACC_AREA_ID);
					decimal accAreaId;
					if (item == null)
					{
						accAreaId = SharedService.GetTableSeqId("SEQ_F194708_ACC_AREA_ID");
						f194708.ACC_AREA_ID = accAreaId;
						f194708.DC_CODE = master.DC_CODE;
						f194708.ALL_ID = master.ALL_ID;
						f194708Repo.Add(f194708);
					}
					else
					{
						accAreaId = item.ACC_AREA_ID;
						item.ACC_AREA = f194708.ACC_AREA;
						f194708Repo.Update(item);
					}
					foreach (var f19470801 in list)
					{
						f19470801.ACC_AREA_ID = accAreaId;
						f19470801.ALL_ID = master.ALL_ID;
						f19470801.DC_CODE = master.DC_CODE;
						f19470801Repo.Add(f19470801);
					}
				}
			}
		}

		private void InsertOrUpdateF194709(F1947 master, List<F194709> f194709List)
		{
			if (master != null && f194709List != null)
			{
				var f194709Repo = new F194709Repository(Schemas.CoreSchema, _wmsTransaction);
				foreach (var f194709 in f194709List)
				{
					var item = f194709Repo.Find(o => o.DC_CODE == master.DC_CODE && o.ALL_ID == master.ALL_ID && o.ACC_DELVNUM_ID == f194709.ACC_DELVNUM_ID);
					if (item == null)
					{
						f194709.DC_CODE = master.DC_CODE;
						f194709.ALL_ID = master.ALL_ID;
						f194709Repo.Add(f194709, "ACC_DELVNUM_ID");
					}
					else
					{
						item.NUM = f194709.NUM;
						f194709Repo.Update(item);
					}
				}
			}
		}

		public ExecuteResult DeleteF1947(string dcCode, string allID)
		{
			if (string.IsNullOrWhiteSpace(dcCode) || string.IsNullOrWhiteSpace(allID))
			{
				return new ExecuteResult() { Message = "缺少配送商主檔資料" };
			}

			var f1947Repository = new F1947Repository(Schemas.CoreSchema, _wmsTransaction);

			// 1. 先檢查資料庫的內容
			var f1947 = f1947Repository.Find(item => item.ALL_ID == EntityFunctions.AsNonUnicode(allID)
													&& item.DC_CODE == EntityFunctions.AsNonUnicode(dcCode));
			if (f1947 == null)
			{
				return new ExecuteResult() { Message = "資料已被刪除, 請重新查詢" };
			}

			f1947Repository.Delete(item => item.ALL_ID == EntityFunctions.AsNonUnicode(allID)
													&& item.DC_CODE == EntityFunctions.AsNonUnicode(dcCode));

			var F194701Repository = new F194701Repository(Schemas.CoreSchema, _wmsTransaction);
			F194701Repository.Delete(item => item.ALL_ID == allID
														&& item.DC_CODE == dcCode);

			//delete F19470101
			var F19470101Repository = new F19470101Repository(Schemas.CoreSchema, _wmsTransaction);
			F19470101Repository.Delete(item => item.ALL_ID == allID
															&& item.DC_CODE == dcCode);

			var f194708Repo = new F194708Repository(Schemas.CoreSchema, _wmsTransaction);
			f194708Repo.Delete(o => o.ALL_ID == allID && o.DC_CODE == dcCode);


			var f19470801Repo = new F19470801Repository(Schemas.CoreSchema, _wmsTransaction);
			f19470801Repo.Delete(o => o.ALL_ID == allID && o.DC_CODE == dcCode);

			var f194709Repo = new F194709Repository(Schemas.CoreSchema, _wmsTransaction);
			f194709Repo.Delete(o => o.ALL_ID == allID && o.DC_CODE == dcCode);

			var f194703Repo = new F194703Repository(Schemas.CoreSchema, _wmsTransaction);
			f194703Repo.Delete(o => o.ALL_ID == allID && o.DC_CODE == dcCode);

			return new ExecuteResult() { IsSuccessed = true, Message = string.Format("已刪除 {0} 配送商", f1947.ALL_COMP) };
		}


		/// <summary>
		/// 設定屬性值，用於覆蓋相同型別的物件。應該用於新增的時候。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="target"></param>
		private static void SetPropertiesValue<T>(T source, T target)
		{
			foreach (var p in typeof(T).GetProperties())
			{
				switch (p.Name)
				{
					case "Item":
					case "Error":
						continue;
				}

				var sourceValue = p.GetValue(source, null);
				p.SetValue(target, sourceValue, null);
			}
		}

		/// <summary>
		/// 設定屬性值，用於 Master 的欄位指定給 Detail 相同欄位。
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TTarget"></typeparam>
		/// <param name="source"></param>
		/// <param name="target"></param>
		private static void SetPropertiesValue<TSource, TTarget>(TSource source, TTarget target)
		{
			foreach (var p in typeof(TSource).GetProperties())
			{
				if (!p.CanWrite)
					continue;

				var sourceValue = p.GetValue(source, null);
				var targetProperty = typeof(TTarget).GetProperty(p.Name);
				targetProperty.SetValue(target, sourceValue, null);
			}
		}

		private void InsertOrUpdateF194703(F1947 master, List<F194703> f194703List)
		{
			if (master == null || f194703List == null)
				return;

			var repo = new F194703Repository(Schemas.CoreSchema, _wmsTransaction);
			repo.Delete(o => o.DC_CODE == master.DC_CODE && o.ALL_ID == master.ALL_ID);

			foreach (var f194703 in f194703List)
			{
				f194703.DC_CODE = master.DC_CODE;
				f194703.ALL_ID = master.ALL_ID;
				repo.Add(f194703);
			}

		}
	}
}
