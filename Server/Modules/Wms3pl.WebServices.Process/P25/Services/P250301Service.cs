using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P25.Services
{
  public partial class P250301Service
  {
    private WmsTransaction _wmsTransaction;
    public P250301Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    public IQueryable<P250301QueryItem> GetP250301QueryData(string gupCode, string custCode, string clientIp)
    {
      var repF250105 = new F250105Repository(Schemas.CoreSchema);
      return repF250105.GetF250105Data(gupCode, custCode, clientIp);
    }

    public void InsertF2501DataToF250105(string gupCode, string custCode,
      string serialBegin, string serialEnd, DateTime? validDateBegin, DateTime? validDateEnd,
      string clientIp, string userId, string userName)
    {
      //取得F2501資料
      var repF2501 = new F2501Repository(Schemas.CoreSchema);
      var listF2501 = repF2501.GetF2501DataBySerialNoAndValidDate(custCode, gupCode, serialBegin, serialEnd,
        validDateBegin, validDateEnd).ToList();

      var repF250105 = new F250105Repository(Schemas.CoreSchema, _wmsTransaction);
      repF250105.DeleteF250105(gupCode, custCode, clientIp, userId, userName);

      var serialNoService = new SerialNoService();
      foreach (var f2501 in listF2501)
      {
        var result = serialNoService.CheckSerialByInHouse(gupCode, custCode, f2501.ITEM_CODE, f2501.SERIAL_NO);
        InsertF250105(repF250105, f2501, result, clientIp, gupCode, custCode, userId, userName);
      }
    }


    private void InsertF250105(F250105Repository rep, F2501 f2501, ExecuteResult result, string clientIp, string gupCode, string custCode, string crtSatff, string crtName)
    {
      //var repF250105 = new F250105Repository(Schemas.CoreSchema, _wmsTransaction);
      var f250105 = new F250105
      {
        SERIAL_NO = f2501.SERIAL_NO,
        ITEM_CODE = f2501.ITEM_CODE,
        ORG_VALID_DATE = f2501.VALID_DATE ?? DateTime.MinValue,
        SERIAL_STATUS = f2501.STATUS,
        ISPASS = result.IsSuccessed ? "1" : "0",
        MESSAGE = result.Message,
        STATUS = "0",
        CLIENT_IP = clientIp,
        GUP_CODE = gupCode,
        CUST_CODE = custCode,
        CRT_STAFF = crtSatff,
        CRT_NAME = crtName,
        CRT_DATE = DateTime.Now
      };
      rep.Add(f250105);
    }


    public void ExtendValidDate(List<string> listSerialNo, DateTime validDate, string gupCode, string custCode,
      string userId, string userName)
    {
      var repF2501 = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
      repF2501.UpdateF2501ValidDate(listSerialNo, validDate, gupCode, custCode, userId, userName);

      var repF250105 = new F250105Repository(Schemas.CoreSchema, _wmsTransaction);
      repF250105.UpdateF250105ExtendData(listSerialNo, gupCode, custCode, userId, userName);

      var repF1913 = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
      repF1913.UpdateF1913ValidDate(listSerialNo, validDate, gupCode, custCode, userId, userName);
    }
  }
}
