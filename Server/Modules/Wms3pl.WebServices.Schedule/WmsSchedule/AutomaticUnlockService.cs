using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;

namespace Wms3pl.WebServices.Schedule.WmsSchedule
{
	public class AutomaticUnlockService
	{
		public AutomaticUnlockService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = new WmsTransaction();
			//_tacService = new TransApiBaseService();
			//_commonService = new CommonService();
			//_sharedService = new SharedService(_wmsTransaction);
		}
		private WmsTransaction _wmsTransaction;


		public ApiResult ExecAutomaticUnlock()
		{
			return ApiLogHelper.CreateApiLogInfo("0", "0", "0", "AutomaticUnlock", new object { }, () => {
				UnlockF0501();
				var f0000Repo = new F0000Repository(Schemas.CoreSchema, _wmsTransaction);
        var f0000s = f0000Repo.GetLockDatas().ToList();
        var hasData = false;
        if (f0000s.Any())
        {
          hasData = true;
          f0000s.ForEach(x => x.IS_LOCK = "0");
          f0000Repo.BulkUpdate(f0000s);
        }

        if (hasData)
        {
          _wmsTransaction.Complete();
          return new ApiResult { IsSuccessed = true, MsgContent = "自動解鎖排程鎖定成功" };
        }
        return new ApiResult { IsSuccessed = true, MsgContent = "無排程被鎖定" };
			});
		}

		public void UnlockF0501()
		{
			var f0501Repo = new F0501Repository(Schemas.CoreSchema);
			var needUnlockBatchNos = f0501Repo.GetNeedUnlockBatchNos().ToList();
			foreach (var unlockBatchNo in needUnlockBatchNos)
			{
				try
				{
					f0501Repo.UnLockByAllotBatchNo(unlockBatchNo);
				}
				catch (Exception ex) { }
			}
		}
	}
}
