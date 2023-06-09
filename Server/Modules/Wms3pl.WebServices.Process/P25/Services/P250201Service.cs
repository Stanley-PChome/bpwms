using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P25.Services
{
  public class P250201Service
  {
    public WmsTransaction _wmsTransaction;
    public P250201Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    public IQueryable<F2501QueryData> Get2501QueryData(string gupCode, string custCode,
     string itemCode, string boxSerial, string batchNo, string serialNo, string cellNum, string poNo
    , string wmsNo, string status, string itemType, string retailCode, Int16? combinNo
    , string crtName, string crtSDate, string crtEDate, string updSDate, string updEDate)
    {
      var coverCrtSDate = (string.IsNullOrEmpty(crtSDate)) ? ((DateTime?)null) : Convert.ToDateTime(crtSDate);
      var coverCrtEDate = (string.IsNullOrEmpty(crtEDate)) ? ((DateTime?)null) : Convert.ToDateTime(crtEDate);
      var coverUpdSDate = (string.IsNullOrEmpty(updSDate)) ? ((DateTime?)null) : Convert.ToDateTime(updSDate);
      var coverUpdEDate = (string.IsNullOrEmpty(updEDate)) ? ((DateTime?)null) : Convert.ToDateTime(updEDate);

      var itemCodeArray = string.IsNullOrEmpty(itemCode) ? new string[] { } : itemCode.Split(',').ToArray();
      var wmsNoArray = string.IsNullOrEmpty(wmsNo) ? new string[] { } : wmsNo.Split(',').ToArray();
      var serialNoArray = string.IsNullOrEmpty(serialNo) ? new string[] { } : serialNo.Split(',').ToArray();

      var repF2501 = new F2501Repository(Schemas.CoreSchema);
      var f2501QueryData = repF2501.Get2501QueryData(gupCode, custCode, itemCodeArray, boxSerial, batchNo, serialNoArray, cellNum, poNo
                              , wmsNoArray, status, itemType, retailCode, combinNo
                              , crtName, coverCrtSDate, coverCrtEDate, coverUpdSDate, coverUpdEDate);
      return f2501QueryData;
    }
  }
}
