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
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710105Service
	{
		private WmsTransaction _wmsTransaction;
		public P710105Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		string ValidateF1942(F1942Ex locType)
		{
			//檢查容積率<=100%
			if (!CheckVolumnRate(locType.VOLUME_RATE))
			{
				return "容積率必需<=100%";
			}
			//檢查便利性1~3
			if (!CheckHandy(locType.HANDY))
			{
				return "便利性必需介於1~3";
			}

			return string.Empty;
		}

		public ExecuteResult SaveLocType(F1942Ex locType, bool isCreate)
		{
			var repo = new F1942Repository(Schemas.CoreSchema, _wmsTransaction);
			decimal oldUsefulVolumn = 0;

			var error = ValidateF1942(locType);
			if (!string.IsNullOrEmpty(error))
			{
				return new ExecuteResult(false, error);
			}

			if (isCreate)
			{
				if (repo.Find(x => x.LOC_TYPE_ID.Equals(locType.LOC_TYPE_ID)) != null)
					return new ExecuteResult(false, "此料架代碼已存在");
			}

			F1942 f1942 = null;
			if (isCreate)
			{
				//Insert
				f1942 = new F1942();
				SetValue(ref f1942, locType);
				repo.Add(f1942);
			}
			else
			{
				//Update
				f1942 = repo.Find(x => x.LOC_TYPE_ID.Equals(locType.LOC_TYPE_ID));
				if (f1942 == null)
					return new ExecuteResult(false, "此料架代碼已被刪除!");

                // 舊的可用容積 = 長*深*高*容積率
                oldUsefulVolumn = SharedService.GetUsefulColumn(f1942.LENGTH, f1942.DEPTH, f1942.HEIGHT, f1942.VOLUME_RATE);

                SetValue(ref f1942, locType);
				repo.Update(f1942);
			}

            // 新的可用容積 = 長*深*高*容積率
            var newUsefulVolumn = SharedService.GetUsefulColumn(locType.LENGTH, locType.DEPTH, locType.HEIGHT, locType.VOLUME_RATE);

            if (newUsefulVolumn >= 100000000000000000)
			{
				return new ExecuteResult(false, string.Format("輸入的可用容積({0})不可超過100000000", newUsefulVolumn));
			}

			var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);

			if (!isCreate)
			{
				// 如果新的可用容積小於舊的可用容積，就必須判斷是否沒有儲位使用此料架，且已用容積須為0，不然讓編輯更小的可用容積會讓調撥或揀貨完成後，計算數量上會錯誤。
				if (newUsefulVolumn < oldUsefulVolumn)
				{
					var usedCount = f1912Repo.Filter(x => x.LOC_TYPE_ID == EntityFunctions.AsNonUnicode(f1942.LOC_TYPE_ID) && x.USED_VOLUMN != 0).Count();
					if (usedCount > 0)
					{
						return new ExecuteResult(false, string.Format("使用此料架的儲位已用容積必須為0才可修改，目前尚有{0}個儲位已用容積不等於0!", usedCount));
					}
				}
			}

			f1912Repo.UpdateUsefulVolumn(f1942.LOC_TYPE_ID, newUsefulVolumn);

			return new ExecuteResult(true);
		}

		private void SetValue(ref F1942 newItem, F1942Ex locType)
		{
			newItem.LOC_TYPE_NAME = locType.LOC_TYPE_NAME;
			//newItem.LENGTH = Convert.ToInt16(locType.LENGTH);
			//newItem.DEPTH = Convert.ToInt16(locType.DEPTH);
			//newItem.HEIGHT = Convert.ToInt16(locType.HEIGHT);
			//newItem.WEIGHT = Convert.ToDecimal(locType.WEIGHT);
			newItem.LENGTH = locType.LENGTH;
			newItem.DEPTH = locType.DEPTH;
			newItem.HEIGHT = locType.HEIGHT;
			newItem.WEIGHT = (float)locType.WEIGHT;
			newItem.VOLUME_RATE = locType.VOLUME_RATE;
			newItem.HANDY = locType.HANDY;

			if (!string.IsNullOrWhiteSpace(locType.LOC_TYPE_ID))
				newItem.LOC_TYPE_ID = locType.LOC_TYPE_ID;
		}

		private bool CheckHandy(string handy)
		{
			if (string.IsNullOrEmpty(handy))
			{
				// 允許 NULL
				return true;
			}

			string strNum1 = "1";
			string strNum2 = "2";
			string strNum3 = "3";
			// 檢查是否輸入1,2,3
			bool num1 = handy.Equals(strNum1);
			bool num2 = handy.Equals(strNum2);
			bool num3 = handy.Equals(strNum3);
			if (num1 || num2 || num3)
				return true;
			return false;
		}

		private bool CheckVolumnRate(decimal? volumnRate)
		{
			int Rate = 0;
			Int32.TryParse(volumnRate.ToString(), out Rate);
			if (Rate > 100 || Rate < 0)
				return false;
			return true;
		}
		public ExecuteResult DeleteF1942(string locTypeId)
		{
			var repo = new F1942Repository(Schemas.CoreSchema, _wmsTransaction);

			// 0. 先檢查該群組是否仍存在
			if (repo.GetByLocTypeId(locTypeId) == null)
			{
				return new ExecuteResult() { IsSuccessed = true, Message = "架料基本資料已被刪除, 請重新查詢" };
			}

			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			var usedCount = f1912Repo.Filter(x => x.LOC_TYPE_ID == EntityFunctions.AsNonUnicode(locTypeId) && x.USED_VOLUMN != 0).Count();
			if (usedCount > 0)
			{
				return new ExecuteResult(false, string.Format("使用此料架的儲位已用容積必須為0才可刪除，目前尚有{0}個儲位已用容積不等於0!", usedCount));
			}

			repo.Delete(locTypeId);

			return new ExecuteResult() { IsSuccessed = true, Message = "已刪除" };
		}
	}
}
