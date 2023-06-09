
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
	/// 系統功能
	/// </summary>
	public partial class P190505Service
	{
		private WmsTransaction _wmsTransaction;
		public P190505Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 傳回F1924 + F192403 List
		/// </summary>
		/// <param name="workgroupId"></param>
		/// <param name="empId"></param>
		/// <param name="empName"></param>
		/// <returns></returns>
		public IQueryable<F1924Data> F1924WithF192403()
		{
			var f1924repo = new F1924Repository(Schemas.CoreSchema, _wmsTransaction);

			var result = f1924repo.F1924WithF192403();
			
			return result;
		}

		/// <summary>
		/// 回寫P190505
		/// </summary>
		/// <param name="groupId"></param>
		/// <param name="empId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public ExecuteResult UpdateP190505(string groupId, List<string> empId, string userId)
		{
			ExecuteResult result = new ExecuteResult() { IsSuccessed = false };
			var repo = new F192401Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo1953 = new F1953Repository(Schemas.CoreSchema, _wmsTransaction);

			// 先檢查F1953是否仍存在該群組
			decimal tmpGroupId = Convert.ToDecimal(groupId);
			if (repo1953.Find(x => x.GRP_ID.Equals(tmpGroupId)) == null)
			{
				result.IsSuccessed = false;
				result.Message = Properties.Resources.P190505Service_WorkGroupDeleted;
			}
			else
			{
				var tmp = repo.Filter(x => x.GRP_ID.Equals(tmpGroupId)).ToList().AsQueryable();
				// 刪除不在清單內的項目
				foreach (var p in tmp)
				{
					if (!empId.Contains(p.EMP_ID)) repo.Delete(p.EMP_ID, tmpGroupId);
				}
				// 新增不在資料庫內的項目
				foreach (var p in empId.Where(x => !string.IsNullOrEmpty(x)))
				{
					if (repo.Find(x => x.EMP_ID.Equals(p) && x.GRP_ID.Equals(tmpGroupId)) == null)
					{
						var tmpData = new F192401()
						{
							GRP_ID = tmpGroupId,
							EMP_ID = p
						};
						repo.Add(tmpData);
					}
				}
				result.IsSuccessed = true;
			}
			return result;
		}
	}
}

