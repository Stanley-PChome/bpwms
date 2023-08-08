
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	/// <summary>
	/// 系統功能 - 作業群組人員設定
	/// </summary>
	public partial class P190508Service
	{
		private WmsTransaction _wmsTransaction;
		public P190508Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult UpdateP190508(decimal workgroupId, List<string> empList, string userId)
		{
			ExecuteResult result = new ExecuteResult() { IsSuccessed = false };
			var repo = new F192403Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF1963 = new F1963Repository(Schemas.CoreSchema, _wmsTransaction);

			// 先檢查F1963是否仍存在該群組
			if (repoF1963.Find(x => x.WORK_ID.Equals(workgroupId)) == null)
			{
				result.IsSuccessed = false;
				result.Message = Properties.Resources.P190508Service_WorkGroupDeleted;
			}
			else
			{
				var tmp = repo.Filter(x => x.WORK_ID.Equals(workgroupId)).ToList().AsQueryable();
				// 刪除不在清單內的項目
				foreach (var p in tmp)
				{
					if (!empList.Contains(p.EMP_ID))
						repo.Delete(x => x.EMP_ID == p.EMP_ID && x.WORK_ID == workgroupId);
				}
				// 新增不在資料庫內的項目
				foreach (var p in empList.Where(x => !string.IsNullOrEmpty(x)))
				{
					if (repo.Find(x => x.EMP_ID.Equals(p) && x.WORK_ID.Equals(workgroupId)) == null)
						repo.Add(new F192403()
						{
							WORK_ID = workgroupId,
							EMP_ID = p
						});
				}
				result.IsSuccessed = true;
			}
			return result;
		}
	}
}

