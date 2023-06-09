using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
  public class WorkstationService
  {
    private WmsTransaction _wmsTransaction;

    public WorkstationService(WmsTransaction wmsTransaction = null)
    { _wmsTransaction = wmsTransaction; }

    /// <summary>
    /// 檢查F910501.WROKSTATION_CODE是否符合作業類型
    /// </summary>
    /// <param name="f910501"></param>
    /// <param name="WorkstationGroupName">工作站群組名稱(F000904.NAME topic='F1946'and subtopic='GROUP')</param>
    /// <returns></returns>
    public ExecuteResult CheckWorkStationCode(F910501 f910501, string WorkstationGroupName)
    {
      var f000904Repo = new F000904Repository(Schemas.CoreSchema);

      var WorkstationGroup = f000904Repo.GetDatas("F1946", "GROUP").FirstOrDefault(x => x.NAME == WorkstationGroupName);
      if (WorkstationGroup == null)
        return new ExecuteResult(false, "無法識別的工作群組名稱["+ WorkstationGroupName+"]");
			if(f910501 == null)
				return new ExecuteResult(false, "該設備不存在於裝置維護設定，請到裝置設定維護設定");
			if (f910501 != null && string.IsNullOrEmpty(f910501.WORKSTATION_CODE))
				return new ExecuteResult(false, "未設定工作站編號，請到裝置設定維護設定");
      if (f910501 != null && f910501.WORKSTATION_CODE.Substring(0, 1) != WorkstationGroup.VALUE)
        return new ExecuteResult(false, $"工作站編號{f910501.WORKSTATION_CODE}不屬於{WorkstationGroupName}，必須為[{WorkstationGroup.VALUE}]開頭的，請至裝置設定維護設定");

      return new ExecuteResult(true);
    }


  }
}
