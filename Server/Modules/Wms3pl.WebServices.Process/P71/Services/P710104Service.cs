using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710104Service
	{
		private WmsTransaction _wmsTransaction;
		public P710104Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 傳回儲位清單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseId"></param>
		/// <param name="areaId"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<F1912StatusEx> GetLocListForLocControl(string dcCode, string gupCode, string custCode
			, string warehouseType, string warehouseId, string areaId, string channel, string itemCode, string account)
		{
			var repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
			var result = repo.GetLocListForLocControl(dcCode, gupCode, custCode, warehouseType, warehouseId, areaId, channel, itemCode, account);
			return result.AsQueryable();
		}

		/// <summary>
		/// 傳回儲位清單. 以ItemCode查詢.
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseId"></param>
		/// <param name="areaId"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<F1912StatusEx2> GetLocListForLocControlByItemCode(string dcCode, string gupCode, string custCode
			, string itemCode, string account)
		{
			var repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
			var result = repo.GetLocListForLocControlByItemCode(dcCode, gupCode, custCode, itemCode, account);
			return result.AsQueryable();
		}

		/// <summary>
		/// 更新儲位控制
		/// </summary>
		/// <param name="locList"></param>
		/// <param name="locStatus"></param>
		/// <param name="uccCode"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public ExecuteResult UpdateLocControl(List<F1912StatusEx> locList, string locStatus, string uccCode, string userId)
		{
			var repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF191202 = new F191202Repository(Schemas.CoreSchema, _wmsTransaction);

			// 依序取得原始資料以備更新, 更新時要一併記錄Log到F191202
			foreach (var p in locList)
			{
				// 查找原始資料
				var tmp = repo.Find(x => x.LOC_CODE.Equals(EntityFunctions.AsNonUnicode(p.LOC_CODE))
					&& x.DC_CODE.Equals(EntityFunctions.AsNonUnicode(p.DC_CODE))
					&& x.WAREHOUSE_ID.Equals(EntityFunctions.AsNonUnicode(p.WAREHOUSE_ID)));

				if (tmp == null) return new ExecuteResult() { IsSuccessed = false, Message = "資料已被刪除, 請重新查詢" };

				// Log F191202 - 更新前
				repoF191202.Log(tmp, userId, "2", "0");

				// 將資料寫入
				tmp.PRE_STATUS_ID = tmp.NOW_STATUS_ID;
				tmp.NOW_STATUS_ID = locStatus;
				tmp.UCC_CODE = locStatus == "01" ? string.Empty : uccCode;	// 若設為使用中(01)，則預設清除原因
				tmp.UPD_STAFF = userId;
				tmp.UPD_DATE = DateTime.Now;
				repo.Update(tmp);

				// Log F191202 - 更新後
				repoF191202.Log(tmp, userId, "2", "1");
			}

			return new ExecuteResult() { IsSuccessed = true };
		}
	}
}
