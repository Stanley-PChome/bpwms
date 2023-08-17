using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P25.Services;



namespace Wms3pl.WebServices.Process.P25.Services
{
  [ServiceContract]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public partial class P25WcfService
  {

    #region 範例用，以後移除

    [OperationContract]
    public IQueryable<ExecuteResult> GetTests(string gupCode)
    {
      return new List<ExecuteResult>().AsQueryable();
    }
    #endregion 範例用，以後移除

    #region P25

    [OperationContract]
    public List<F250103Verification> InsertUpdateF2501Data(List<F2501WcfData> datas)
    {
      var wmsTransaction = new WmsTransaction();
      var p25Service = new P250103Service(wmsTransaction);
			var result = p25Service.InsertOrUpdate_New(datas);
			wmsTransaction.Complete();
      return result;
    }
    #endregion

    [OperationContract]
    public ExecuteResult ExtendValidDate(List<string> listSerialNo, DateTime validDate, string gupCode, string custCode,
      string userId, string userName)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P250301Service(wmsTransaction);
      try
      {
        srv.ExtendValidDate(listSerialNo, validDate, gupCode, custCode, userId, userName);
        wmsTransaction.Complete();
        return new ExecuteResult(true, "");
      }
      catch (Exception ex)
      {
        return new ExecuteResult(false, ex.Message);
      }
    }

    #region P2503020000 序號更換

    [OperationContract]
    public void CheckNewSerialNo(string gupCode, string custCode, string itemCode, string oldSerialNo,
      string newSerialNo, string clientIp, string userId, string userName)
    {
      var srv = new P250302Service();
      srv.CheckNewSerialNo(gupCode, custCode, itemCode, oldSerialNo, newSerialNo, clientIp, userId, userName);
    }

    [OperationContract]
    public ExecuteResult ChangeSerialNo(List<P250302QueryItem> listData, string dcCode)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P250302Service(wmsTransaction);

      var result = srv.ChangeSerialNo(listData, dcCode);
      if (result.IsSuccessed)
        wmsTransaction.Complete();
      return result;
    }

    [OperationContract]
    public ExecuteResult CheckImportData(DataTable dt, string gupCode, string custCode, string clientIp, string userId,
      string userName)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P250302Service(wmsTransaction);

      try
      {
        srv.CheckImportData(dt, gupCode, custCode, clientIp, userId, userName);
        wmsTransaction.Complete();
        return new ExecuteResult(true, "");
      }
      catch (Exception ex)
      {
        return new ExecuteResult(false, ex.Message);
      }
    }

    #endregion


    [OperationContract]
    public IQueryable<F2501QueryData> Get2501QueryData(string gupCode, string custCode,
               string itemCode, string boxSerial, string batchNo, string serialNo, string cellNum, string poNo
              , string wmsNo, string status, string OrdProp, string retailCode, Int16? combinNo
              , string crtName, string crtSDate, string crtEDate, string updSDate, string updEDate)
    {

      var srv = new P250201Service();
      var result = srv.Get2501QueryData(gupCode, custCode, itemCode, boxSerial, batchNo, serialNo, cellNum, poNo
        , wmsNo, status, OrdProp, retailCode, combinNo
        , crtName, crtSDate, crtEDate, updSDate, updEDate);

      return result;
    }

	#region 序號刪除
		[OperationContract]
		public ExecuteResult DeleteSerialNo(string gupCode, string custCode, List<string> SnList)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P250303Service(wmsTransaction);
			var result = srv.DeleteSerialNo(gupCode, custCode, SnList);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}
    #endregion 序號刪除



  



    [OperationContract]
    public IQueryable<P2502QueryData> GetP2502QueryDatas(string gupCode, string custCode,
       string itemCode, string serialNo, string batchNo, string cellNum, string poNo, string wmsNo
      , string status, string retailCode, Int16? combinNo, string crtName, string updSDate
      , string updEDate, string boxSerial, string opItemType)
    {
      var srv = new P250202Service();
      var result = srv.GetP2502QueryDatas(gupCode, custCode, itemCode, serialNo, batchNo, cellNum, poNo, wmsNo
        , status, retailCode, combinNo, crtName, updSDate
        , updEDate, boxSerial, opItemType);

      return result;
    }

    [OperationContract]
    public  F2501WcfData GetF2501Data(string gupCode, string custCode, string serialNo)
    {
      var repo = new F2501Repository(Schemas.CoreSchema);
      return repo.GetF2501WcfData(gupCode, custCode, serialNo);
    }

  }

}

  

	

