using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Schedule.WmsSchedule
{
  public class ShipFinishConfirmNotifyService
  {
    private WmsTransaction _wmsTransaction;

    #region Services

    #region CommonService
    private CommonService _CommonService;
    public CommonService CommonService
    {
      get { return _CommonService == null ? _CommonService = new CommonService() : _CommonService; }
      set { _CommonService = value; }
    }
    #endregion CommonService

    #endregion Services

    #region Repositorys

    #region F051301Repository
    private F051301Repository _F051301Repo;
    public F051301Repository F051301Repo
    {
      get { return _F051301Repo == null ? _F051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction) : _F051301Repo; }
      set { _F051301Repo = value; }
    }
    #endregion F051301Repository

    #region F051202Repository
    private F051202Repository _F051202Repo;
    public F051202Repository F051202Repo
    {
      get { return _F051202Repo == null ? _F051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction) : _F051202Repo; }
      set { _F051202Repo = value; }
    }
    #endregion F051301Repository

    #region F05120601Repository
    private F05120601Repository _F05120601Repo;
    public F05120601Repository F05120601Repo
    {
      get { return _F05120601Repo == null ? _F05120601Repo = new F05120601Repository(Schemas.CoreSchema, _wmsTransaction) : _F05120601Repo; }
      set { _F05120601Repo = value; }
    }
    #endregion F051301Repository

    #region F050306Repository
    private F050306Repository _F050306Repo;
    public F050306Repository F050306Repo
    {
      get { return _F050306Repo == null ? _F050306Repo = new F050306Repository(Schemas.CoreSchema, _wmsTransaction) : _F050306Repo; }
      set { _F050306Repo = value; }
    }
    #endregion F051301Repository

    #region F060702Repository
    private F060702Repository _F060702Repo;
    public F060702Repository F060702Repo
    {
      get { return _F060702Repo == null ? _F060702Repo = new F060702Repository(Schemas.CoreSchema, _wmsTransaction) : _F060702Repo; }
      set { _F060702Repo = value; }
    }
    #endregion F051301Repository

    #region F050801Repository
    private F050801Repository _F050801Repo;
    public F050801Repository F050801Repo
    {
      get { return _F050801Repo == null ? _F050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction) : _F050801Repo; }
      set { _F050801Repo = value; }
    }
    #endregion F050801Repository

    #endregion Repositorys

    public ShipFinishConfirmNotifyService()
    {
      _wmsTransaction = new WmsTransaction();
    }

    /// <summary>
    /// 出貨單據完成通知排程
    /// </summary>
    /// <returns></returns>
    public ApiResult ExecShipFinishConfirmNotify()
    {
      //總處理筆數
      var totalProcCnt = 0;
      //全部建立出貨單據完成通知排程出貨單號
      var totalCrtOrd = new List<string>();

      var res = ApiLogHelper.CreateApiLogInfo(null, null, null, "ShipFinishConfirmNotify", null,
        () =>
        {
          var execData = F051301Repo.GetShipFinishConfirmNotifyData().ToList();
          totalProcCnt = execData.Count;
          //針對每一張出貨單進行判斷 & Commit
          foreach (var curData in execData)
          {
            //檢查出貨單是否所有揀貨單還有明細未完成揀貨
            var isPickNotDone = F051202Repo.IsPickOrdNotDone(curData.DC_CODE, curData.GUP_CODE, curData.CUST_CODE, curData.WMS_NO);
            if (isPickNotDone)
              continue;

            //檢查出貨單是否有揀缺待配庫
            var isHavePickLackUnAllot = F05120601Repo.IsHavePickLackUnAllot(curData.DC_CODE, curData.GUP_CODE, curData.CUST_CODE, curData.WMS_NO);
            if (isHavePickLackUnAllot)
              continue;

            //檢查出貨單是否有已配庫待產生揀貨單資料
            var isHaveAllotData = F050306Repo.IsHaveAllotData(curData.DC_CODE, curData.GUP_CODE, curData.CUST_CODE, curData.WMS_NO);
            if (isHaveAllotData)
              continue;

            // 檢查出貨單是否取消
            var isOrderCanceled = F050801Repo.IsOrderCanceled(curData.DC_CODE, curData.GUP_CODE, curData.CUST_CODE, curData.WMS_NO);
            if (isOrderCanceled)
              continue;

            totalCrtOrd.Add(curData.WMS_NO);
            F060702Repo.Add(new F060702
            {
              DC_CODE = curData.DC_CODE,
              GUP_CODE = curData.GUP_CODE,
              CUST_CODE = curData.CUST_CODE,
              ORDER_CODE = curData.WMS_NO,
              ORI_ORDER_CODE = curData.WMS_NO,
              STATUS = 3,
              PROC_FLAG = "0"
            });

            //更新發送單據完成通知
            F051301Repo.UpdateFields(new { NOTIFY_MODE = "1" }, 
              x => x.DC_CODE == curData.DC_CODE && x.GUP_CODE == curData.GUP_CODE && x.CUST_CODE == curData.CUST_CODE && x.WMS_NO == curData.WMS_NO);

            _wmsTransaction.Complete();
          }
          return new ApiResult
          {
            IsSuccessed = true,
            MsgContent = $"檢查總筆數:{totalProcCnt} 建立的筆數:{totalCrtOrd.Count} 建立出貨完成通知單號為：{string.Join(",", totalCrtOrd)}"
          };
        }, true);

      return res;

    }
  }
}
