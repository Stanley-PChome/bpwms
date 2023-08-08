using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P25.ExDataSources;
using Wms3pl.WebServices.Process.P25.Services;


namespace Wms3pl.WebServices.Process.P25
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P25ExDataService : DataService<P25ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		#region P2501
		[WebGet]
		public F2501WcfData GetF2501Data(string gupCode, string custCode, string serialNo)
		{
			var repo = new F2501Repository(Schemas.CoreSchema);
			return repo.GetF2501WcfData(gupCode, custCode, serialNo);
		}

		#endregion


		/// <summary>
		/// 序號主檔查詢
		/// </summary>
		[WebGet]
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

		/// <summary>
		/// 序號異動查詢
		/// </summary>
		[WebGet]
		public IQueryable<P2502QueryData> GetP2502QueryDatas(string gupCode, string custCode,
		   string itemCode, string serialNo, string batchNo, string cellNum, string poNo, string wmsNo
		  , string status, string retailCode, Int16? combinNo, string crtName, string updSDate
		  , string updEDate, string boxSerial, string OpItemType)
		{

			var srv = new P250202Service();
			var result = srv.GetP2502QueryDatas(gupCode, custCode, itemCode, serialNo, batchNo, cellNum, poNo, wmsNo
			  , status, retailCode, combinNo, crtName, updSDate
			  , updEDate, boxSerial, OpItemType);

			return result;
		}


		

		[WebGet]
		public IQueryable<P250301QueryItem> GetP250301QueryData(string gupCode, string custCode,
		  string serialBegin, string serialEnd, DateTime? validDateBegin, DateTime? validDateEnd,
		  string clientIp, string userId, string userName, string reGetFromF2501)
		{
			var srv = new P250301Service();

			if (reGetFromF2501 == "1")
			{
				var wmsTransaction = new WmsTransaction();
				srv.InsertF2501DataToF250105(gupCode, custCode, serialBegin, serialEnd, validDateBegin, validDateEnd, clientIp,
				  userId, userName);
				wmsTransaction.Complete();
			}

			return srv.GetP250301QueryData(gupCode, custCode, clientIp);
		}

		[WebGet]
		public IQueryable<P250302QueryItem> GetP250302QueryData(string gupCode, string custCode, string clientIp, string onlyPass)
		{
			var srv = new P250302Service();
			return srv.GetP250302QueryData(gupCode, custCode, clientIp, onlyPass);
		}

		[WebGet]
		public IQueryable<ExecuteResult> CheckOldSerialNo(string gupCode, string custCode, string itemCode, string serialNo)
		{
			var srv = new P250302Service();
			var result = srv.CheckOldSerialNo(gupCode, custCode, itemCode, serialNo);

			var tmp = new List<ExecuteResult> { result };
			return tmp.AsQueryable();
		}

		//[WebGet]
		//public IQueryable<ExecuteResult> CheckNewSerialNo(string gupCode, string custCode, string itemCode, string oldSerialNo,
		//  string newSerialNo, string clientIp, string userId, string userName)
		//{
		//  var srv = new P250302Service();
		//  srv.CheckNewSerialNo(gupCode, custCode, itemCode, oldSerialNo, newSerialNo, clientIp, userId, userName);

		//  var tmp = new List<ExecuteResult> {new ExecuteResult(true)};
		//  return tmp.AsQueryable();
		//  //return srv.GetP250302QueryData(gupCode, custCode, clientIp);
		//}

	}
}
