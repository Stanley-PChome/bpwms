
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	/// <summary>
	/// 系統功能
	/// </summary>
	public partial class P190504Service
	{
		private WmsTransaction _wmsTransaction;
		public P190504Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult UpdateP190504(string groupId, string groupName, string groupDesc, string showInfo
			, List<string> funCodeList, List<string> scheduleList, string userId)
		{
			ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
			var f1953Repo = new F1953Repository(Schemas.CoreSchema, _wmsTransaction);
			var f195301Repo = new F195301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1954Repo = new F1954Repository(Schemas.CoreSchema, _wmsTransaction);
			var f195302Repo = new F195302Repository(Schemas.CoreSchema, _wmsTransaction);

			decimal tmpGroupId;
			// 寫入主檔
			if (Decimal.TryParse(groupId, out tmpGroupId))
			{
				var f1953Data = f1953Repo.Find(x => x.GRP_ID.Equals(tmpGroupId));
				if (f1953Data == null)
				{
					result.IsSuccessed = false;
					result.Message = Properties.Resources.P190504Service_ItemHasDeleted;
				}
				else
				{
					f1953Data.GRP_DESC = groupDesc;
					f1953Data.GRP_NAME = groupName;
					f1953Data.SHOWINFO = showInfo;
					f1953Repo.Update(f1953Data);
				}
			}
			else
			{
				return new ExecuteResult{ IsSuccessed = false };
			}

			// 回寫F195301
			// 1. 刪除未勾選的資料
			var delF195301Datas = f195301Repo.GetDelDatas(funCodeList, tmpGroupId);
			if (delF195301Datas.Any())
				f195301Repo.BulkDelete(tmpGroupId, delF195301Datas.Select(x => x.FUN_CODE).ToList());

			// 檢查該fun_code是否在其它地方有使用, 如果沒有, 則將F1954的授權狀態改掉
			var updF1954Datas = f1954Repo.GetOtherGrpIdDatas(funCodeList, tmpGroupId).ToList();
			if (updF1954Datas.Any())
			{
				updF1954Datas.ForEach(obj =>{obj.STATUS = "0";});
				f1954Repo.BulkUpdate(updF1954Datas);
			}

			// 2. 寫入新資料
			var addF195301Datas = f195301Repo.AddP190504Detail(funCodeList, tmpGroupId);
			if (addF195301Datas.Any())
				f195301Repo.BulkInsert(addF195301Datas);

			// 取得要更新F1954狀態為已授權的F1954
			var updF1954DatasTmp = f1954Repo.GetDataByFunCodes(funCodeList).ToList();
			updF1954DatasTmp.ForEach(obj => { obj.STATUS = "1"; });
			if(updF1954DatasTmp.Any())
				f1954Repo.BulkUpdate(updF1954DatasTmp);

			// 3. F195302-刪除未勾選的資料
			f195302Repo.BulkDeleteByNotCheckDatas(tmpGroupId, scheduleList);

			// 4. F195302-寫入新資料
			var addF195302Datas = f195302Repo.AddP195302Detail(scheduleList, tmpGroupId);
			if (addF195302Datas.Any())
				f195302Repo.BulkInsert(addF195302Datas);

			return result;
		}

		public ExecuteResult DeleteP190504(decimal groupId, string userId)
		{
			ExecuteResult result = new ExecuteResult() { IsSuccessed = false };
			var f1953Repo = new F1953Repository(Schemas.CoreSchema, _wmsTransaction);
			var f195301Repo = new F195301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1954Repo = new F1954Repository(Schemas.CoreSchema, _wmsTransaction);
			var f195302Repo = new F195302Repository(Schemas.CoreSchema, _wmsTransaction);

			// 先判斷F1953存不存在該項目, 不存在就回傳該資料已糟刪除
			F1953 f1953 = f1953Repo.Find(x => x.GRP_ID.Equals(groupId));
			if (f1953 == null)
			{
				result.IsSuccessed = false;
				result.Message = Properties.Resources.P190101Service_DataDeleted;
			}
			else
			{
				// 取得該Group已設定的所有FunCode, 然後去更新F1954狀態
				foreach (var funCode in f195301Repo.Filter(x => x.GRP_ID.Equals(groupId)))
				{
					// 檢查該fun_code是否在其它地方有使用, 如果沒有, 則將F1954的授權狀態改掉
					// 這裡要濾掉自己本身的GroupId, 因為此時取到的資料是未寫入前的資料
					if (f195301Repo.Filter(x => x.FUN_CODE.Equals(funCode.FUN_CODE) && !x.GRP_ID.Equals(groupId)).Count() == 0)
					{
						// 更新F1954狀態為未授權
						var f1954Data = f1954Repo.Find(x => x.FUN_CODE == EntityFunctions.AsNonUnicode(funCode.FUN_CODE));
						if (f1954Data != null)
						{
							f1954Data.STATUS = "0";
							f1954Repo.Update(f1954Data);
						}
					}
				}
				// 刪除F1953的資料
				f1953.ISDELETED = "1";
				f1953Repo.Update(f1953);

				// 刪除F195301資料 - 整批刪除, 所以用SQL做
				f195301Repo.Delete(groupId);
				// 刪除F195302資料 - 整批刪除
				f195302Repo.Delete(groupId);

				result.IsSuccessed = true;
			}
			return result;
		}

		/// <summary>
		/// 新增工作群組 Step 2: 新增工作群組的程式功能.
		/// 分2步驟做因為NewID必須於主檔Commit後才能取得
		/// </summary>
		public ExecuteResult AddP190504Detail(List<string> funCodeList, decimal grpId)
		{
			var f195301Repo = new F195301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1954Repo = new F1954Repository(Schemas.CoreSchema, _wmsTransaction);
			var newId = grpId;

			// 取得於DB查無資料時再寫入的F195301
			var addF195301Datas = f195301Repo.AddP190504Detail(funCodeList, grpId);

			// 取得要更新F1954狀態為已授權的F1954
			var updF1954Datas = f1954Repo.GetDataByFunCodes(funCodeList).ToList();
			updF1954Datas.ForEach(obj => { obj.STATUS = "1"; });

			if (addF195301Datas.Any())
				f195301Repo.BulkInsert(addF195301Datas);
			if (updF1954Datas.Any())
				f1954Repo.BulkUpdate(updF1954Datas);

			return new ExecuteResult { IsSuccessed = true };
		}

		public ExecuteResult AddP195302Detail(List<string> scheduleIdList, decimal grpId)
		{
			var f195302Repo = new F195302Repository(Schemas.CoreSchema, _wmsTransaction);
			// 取得需要新增的F195302
			var addF195302Datas = f195302Repo.AddP195302Detail(scheduleIdList, grpId);

			if (addF195302Datas.Any())
				f195302Repo.BulkInsert(addF195302Datas);

			return new ExecuteResult { IsSuccessed = true };
		}

		public decimal? GetF1953_GRP_ID(string userId, DateTime saveTime)
		{           
            var f1953Repo = new F1953Repository(Schemas.CoreSchema, _wmsTransaction);
            

            var newItem = f1953Repo.Filter(x => x.CRT_STAFF.Equals(userId) && x.CRT_DATE.Equals(saveTime))
				.OrderByDescending(x => x.GRP_ID).FirstOrDefault();

			return newItem != null ? newItem.GRP_ID : (decimal?)null;
		}

		public ExecuteResult AddP190504(string groupName, string groupDesc, string showInfo, string userId, DateTime saveTime)
		{
			ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
			var f1953repo = new F1953Repository(Schemas.CoreSchema, _wmsTransaction);
			var f195301repo = new F195301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1954repo = new F1954Repository(Schemas.CoreSchema, _wmsTransaction);

			// 寫入F1953
			//f1953repo.Insert(groupId, groupName, groupDesc, showInfo, userId);
			// 寫入F1953, 改用新方法寫 (DB Trigger/ Sequence產生序號)
			f1953repo.Add(new F1953()
			{
				GRP_DESC = groupDesc,
				GRP_NAME = groupName,
				ISDELETED = "0",
				SHOWINFO = showInfo,
				CRT_DATE = saveTime,
				CRT_STAFF = userId
			}, "GRP_ID");

			// 逐筆寫入F195301 - 20141219 取消, 拆2步做
			//foreach (var p in funCodeList)
			//{
			//	f195301repo.Insert(p, groupId, userId);
			//	// 更新F1954狀態為已授權
			//	f1954repo.Update(p, "1", userId);
			//}

			return result;
		}

	}
}

