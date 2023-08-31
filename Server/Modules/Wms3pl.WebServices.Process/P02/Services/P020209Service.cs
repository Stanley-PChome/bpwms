using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P02.Services
{
  public partial class P020209Service
  {
    private WmsTransaction _wmsTransaction;
		private WarehouseInService _warehouseInService;
    private CommonService _commonService;

    private CommonService CommonService
    {
      get
      {
        if (_commonService == null)
          _commonService = new CommonService();
        return _commonService;
      }
    }

    private SharedService _sharedService;
    public SharedService SharedService
    {
      get
      {
        if (_sharedService == null)
          _sharedService = new SharedService(_wmsTransaction);
        return _sharedService;
      }
    }

    public P020209Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
			_warehouseInService = new WarehouseInService(_wmsTransaction);
		}

    public IQueryable<RecvRecords> GetF020209RecvRecord(string dcCode, string gupCode, string custCode, DateTime RecvDateBegin, DateTime RecvDateEnd, string PurchaseNo,
      string CustOrdNo, string PrintMode, string PalletLocation, string ItemCode, string RecvStaff)
    {
      var f020201Repo = new F020201Repository(Schemas.CoreSchema);

      return f020201Repo.GetF020209RecvRecord(dcCode, gupCode, custCode, RecvDateBegin, RecvDateEnd, PurchaseNo, CustOrdNo, PrintMode, PalletLocation, ItemCode, RecvStaff);
    }

    public IQueryable<ItemLabelData> GetP020209ItemLabelData(string dcCode, string gupCode, string custCode, string rtNos)
    {
      var f020201Repo = new F020201Repository(Schemas.CoreSchema);

      List<string> rtNoList = rtNos.Split(',').ToList();

      return f020201Repo.GetP020209ItemLabelData(dcCode, gupCode, custCode, rtNoList);
    }
  }
}
