using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	public partial class P190191Service
	{
		private WmsTransaction _wmsTransaction;
		public P190191Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 新增工作站設定
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public ExecuteResult InsertF1946(F1946 f1946Data)
		{
			var f1946Repo = new F1946Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1946 = f1946Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f1946Data.DC_CODE
			&& x.WORKSTATION_CODE == f1946Data.WORKSTATION_CODE).FirstOrDefault();
			if (f1946 != null)
			{
				return new ExecuteResult { IsSuccessed = false, Message = "資料已存在" };
			}

			f1946Repo.Add(new F1946
			{
				DC_CODE = f1946Data.DC_CODE,
				WORKSTATION_CODE = f1946Data.WORKSTATION_CODE,
				WORKSTATION_TYPE = f1946Data.WORKSTATION_TYPE,
				WORKSTATION_GROUP = f1946Data.WORKSTATION_GROUP,
				STATUS = "0"
			});
			return new ExecuteResult(true);

		}

		/// <summary>
		/// 刪除工作站設定
		/// </summary>
		/// <param name="f1946Data"></param>
		/// <returns></returns>
		public ExecuteResult DeleteF1946(F1946 f1946Data)
		{
			var reuslt = new ExecuteResult();
			var f1946Repo = new F1946Repository(Schemas.CoreSchema, _wmsTransaction);
			var f910501Repo = new F910501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f910501 = f910501Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f1946Data.DC_CODE
			&& x.WORKSTATION_CODE == f1946Data.WORKSTATION_CODE);
			if (f910501.Any())
			{
				reuslt.IsSuccessed = false;
				reuslt.Message = "該工作站已經被設定，不可以刪除";
				return new ExecuteResult { IsSuccessed = false, Message = "該工作站已經被設定，不可以刪除" };
			}
			f1946Repo.Delete(x => x.DC_CODE == f1946Data.DC_CODE
				&& x.WORKSTATION_GROUP == f1946Data.WORKSTATION_GROUP
				&& x.WORKSTATION_TYPE == f1946Data.WORKSTATION_TYPE
				&& x.WORKSTATION_CODE == f1946Data.WORKSTATION_CODE);
			return new ExecuteResult(true);

		}
	}
}
