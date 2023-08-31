using System;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
  public class CheckTransWcsApiService
  {
    private TransApiBaseService _tacService;
    private CommonService _commonService;
    public CommonService CommonService
    {
      get { return _commonService == null ? _commonService = new CommonService() : _commonService; }
      set { _commonService = value; }
    }

    public CheckTransWcsApiService()
    {
      _tacService = new TransApiBaseService();
      _commonService = new CommonService();
    }

    /// <summary>
    /// 檢核物流中心必填、是否存在
    /// </summary>
    /// <param name="res"></param>
    /// <param name="req"></param>
    public void CheckDcCode(ref ApiResult res, object req)
    {
      // 檢核必填欄位 DcCode
      if (res.IsSuccessed && !DataCheckHelper.CheckRequireColumn(req, "DcCode"))
        res = new ApiResult { IsSuccessed = false, MsgCode = "20051", MsgContent = _tacService.GetMsg("20051") };

      // 檢核物流中心是否存在 DcCode
      if (res.IsSuccessed)
      {
        string dcCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "DcCode"));

        if (!_commonService.CheckDcExist(dcCode))
        {
          res = new ApiResult { IsSuccessed = false, MsgCode = "20051", MsgContent = _tacService.GetMsg("20051") };
        }
      }
    }

    /// <summary>
    /// 檢核貨主編號必填、是否存在
    /// </summary>
    /// <param name="res"></param>
    /// <param name="req"></param>
    public void CheckOwnerCode(ref ApiResult res, object req)
    {
      // 檢核必填欄位 OwnerCode
      if (res.IsSuccessed && !DataCheckHelper.CheckRequireColumn(req, "OwnerCode"))
        res = new ApiResult { IsSuccessed = false, MsgCode = "20097", MsgContent = _tacService.GetMsg("20097") };

      // 檢核貨主編號是否存在 OwnerCode
      if (res.IsSuccessed)
      {
        string custCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "OwnerCode"));

        if (!_commonService.CheckCustExist(custCode))
        {
          res = new ApiResult { IsSuccessed = false, MsgCode = "20097", MsgContent = _tacService.GetMsg("20097") };
        }
      }
    }

    /// <summary>
    /// 檢核倉庫編號必填、是否存在
    /// </summary>
    /// <param name="res"></param>
    /// <param name="req"></param>
    public void CheckZoneCode(ref ApiResult res, object req)
    {
      // 檢核必填欄位 OwnerCode
      if (res.IsSuccessed && !DataCheckHelper.CheckRequireColumn(req, "ZoneCode"))
        res = new ApiResult { IsSuccessed = false, MsgCode = "20092", MsgContent = _tacService.GetMsg("20092") };

      // 檢核貨主編號是否存在 OwnerCode
      if (res.IsSuccessed)
      {
        string dcCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "DcCode"));
        string zoneCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "ZoneCode"));

        if (!_commonService.CheckZoneCodeExist(dcCode, zoneCode))
          res = new ApiResult { IsSuccessed = false, MsgCode = "20092", MsgContent = _tacService.GetMsg("20092") };
      }
    }

    /// <summary>
    /// 檢核分揀機編號必填、是否存在
    /// </summary>
    /// <param name="res"></param>
    /// <param name="req"></param>
    public void CheckSorterCode(ref ApiResult res, object req)
    {
      // 檢核必填欄位 SorterCode
      if (res.IsSuccessed && !DataCheckHelper.CheckRequireColumn(req, "SorterCode"))
        res = new ApiResult { IsSuccessed = false, MsgCode = "20127", MsgContent = _tacService.GetMsg("20127") };

      // 檢核貨主編號是否存在 OwnerCode
      if (res.IsSuccessed)
      {
        string sorterCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "SorterCode"));

        if (!_commonService.CheckSorterCodeExist(sorterCode))
          res = new ApiResult { IsSuccessed = false, MsgCode = "20127", MsgContent = _tacService.GetMsg("20127") };
      }
    }

    /// <summary>
    /// 檢核異常類型必填、是否存在
    /// </summary>
    /// <param name="res"></param>
    /// <param name="req"></param>
    public void CheckAbnormalType(ref ApiResult res, object req)
    {
      // 檢核必填欄位 OwnerCode
      if (res.IsSuccessed && !DataCheckHelper.CheckRequireColumn(req, "AbnormalType"))
        res = new ApiResult { IsSuccessed = false, MsgCode = "20128", MsgContent = _tacService.GetMsg("20128") };

      // 檢核異常類型是否存在 OwnerCode
      if (res.IsSuccessed)
      {
        string abnormalType = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "AbnormalType"));
        if (!_commonService.CheckAbnormalTypeExist(abnormalType))
          res = new ApiResult { IsSuccessed = false, MsgCode = "20128", MsgContent = _tacService.GetMsg("20128") };
      }
    }
  }
}
