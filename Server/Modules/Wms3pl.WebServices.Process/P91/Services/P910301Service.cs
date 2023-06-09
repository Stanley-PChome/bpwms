using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P91.Services
{
	public partial class P910301Service
	{
		private WmsTransaction _wmsTransaction;
		public P910301Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult DeleteContract(string dcCode, string gupCode, string contractNo)
		{
			var repo910301 = new F910301Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo910302 = new F910302Repository(Schemas.CoreSchema, _wmsTransaction);

			// 刪除主檔
			var f910301 = repo910301.Find(x => x.DC_CODE == dcCode &&
												x.GUP_CODE == gupCode &&
												x.CONTRACT_NO == contractNo);
			if (f910301 == null)
			{
				return new ExecuteResult(false, Properties.Resources.P910301Service_DataContractNotExists);
			}

			if (f910301.ENABLE_DATE <= DateTime.Today)
			{
				return new ExecuteResult(false, Properties.Resources.P910301Service_ENABLE_CONTRACT_CANTDELETE);
			}

			if (repo910302.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode
														&& x.GUP_CODE == gupCode
														&& x.CONTRACT_NO == contractNo).Any())
			{
				return new ExecuteResult(false, Properties.Resources.P910301Service_NO_CONTRACT_DETAIL_TO_DELETE);
			}


			repo910301.Delete(x => x.DC_CODE == dcCode &&
									x.GUP_CODE == gupCode &&
									x.CONTRACT_NO == contractNo);

			// 刪除副檔
			//var f910302s = repo910302.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode) &&
			//										x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode) &&
			//										x.CONTRACT_NO == EntityFunctions.AsNonUnicode(contractNo))
			//										.ToList();
			//if (f910302s == null || !f910302s.Any())
			//{
			//	return new ExecuteResult(false, "資料合約項目不存在");
			//}

			repo910302.Delete(x => x.DC_CODE == dcCode &&
									x.GUP_CODE == gupCode &&
									x.CONTRACT_NO == contractNo);

			return new ExecuteResult(true);
		}
	}
}

