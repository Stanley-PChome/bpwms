using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	public partial class P190104Service
	{
		private WmsTransaction _wmsTransaction;
		public P190104Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 新增集貨格類型
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public ExecuteResult InsertOrUpdateF0002(string dcCode, string logisticCode, string logisticName, string isPierRecvPoint, string isVendorReturn, string typeMode)
		{
      logisticCode = logisticCode?.ToUpper();
      logisticName = logisticName?.ToUpper();
      var f0002Repo = new F0002Repository(Schemas.CoreSchema, _wmsTransaction);
			if (string.IsNullOrWhiteSpace(dcCode.Trim()))
			{
				return new ExecuteResult { IsSuccessed = false, Message = "請輸入物流中心" };
			}
			if (string.IsNullOrWhiteSpace(logisticCode.Trim()))
			{
				return new ExecuteResult { IsSuccessed = false, Message = "請輸入物流商編號" };
			}
			if (logisticCode.Trim().Length>10)
			{
				return new ExecuteResult { IsSuccessed = false, Message = "物流商編號必須小於10個字元" };
			}
			if (string.IsNullOrWhiteSpace(logisticName?.Trim()))
			{
				return new ExecuteResult { IsSuccessed = false, Message = "請輸入物流商名稱" };
			}
			if (logisticName.Trim().Length > 20)
			{
				return new ExecuteResult { IsSuccessed = false, Message = "物流商名稱必須小於20個字元" };
			}

			var f0002 = f0002Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode
			&& x.LOGISTIC_CODE == logisticCode).FirstOrDefault();
			if(typeMode == "Add")
			{
				if (f0002 != null)
				{
					return new ExecuteResult { IsSuccessed = false, Message = "物流商編號已存在" };
				}

				f0002Repo.Add(new F0002
				{
					DC_CODE = dcCode,
					LOGISTIC_CODE = logisticCode,
					LOGISTIC_NAME = logisticName,
					IS_PIER_RECV_POINT = isPierRecvPoint,
					IS_VENDOR_RETURN = isVendorReturn,
				});
			}
			else if(typeMode == "Edit")
			{
				if (f0002 == null)
				{
					return new ExecuteResult { IsSuccessed = false, Message = "物流商編號不存在" };
				}

				f0002.LOGISTIC_NAME = logisticName;
                f0002.IS_PIER_RECV_POINT = isPierRecvPoint;
				f0002.IS_VENDOR_RETURN = isVendorReturn;
				f0002Repo.Update(f0002);
			}
			return new ExecuteResult(true);

		}

		/// <summary>
		/// 刪除物流商資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="logisticCode"></param>
		/// <returns></returns>
		public ExecuteResult DeleteF0002(string dcCode,string logisticCode)
		{
			var f0002Repo = new F0002Repository(Schemas.CoreSchema, _wmsTransaction);
			
			f0002Repo.DeleteF0002(dcCode, logisticCode);

			return new ExecuteResult(true);
		}
	}
}
