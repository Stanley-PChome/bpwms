using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P50.ExDataSources;
using Wms3pl.WebServices.Process.P50.Services;


namespace Wms3pl.WebServices.Process.P50
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P50ExDataService : DataService<P50ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		#region P5001010000 專案計價設定
		[WebGet]
		public IQueryable<F199007Data> GetProjectValuation(string dcCode, string gupCode, string custCode,
			string crtDateStare, string crtDateEnd, string accProjectNo, string enableDate, string disableDate,
			string quoteNo, string status, string accProjectName)
		{
			var P500101Service = new P500101Service();
			DateTime? creDateS = (string.IsNullOrEmpty(crtDateStare)) ? null : DateTime.Parse(crtDateStare).Date as DateTime?;
			DateTime? creDateE = (string.IsNullOrEmpty(crtDateEnd)) ? null : DateTime.Parse(crtDateEnd).Date as DateTime?;
			DateTime? enableD = (string.IsNullOrEmpty(enableDate)) ? null : DateTime.Parse(enableDate).Date as DateTime?;
			DateTime? disableD = (string.IsNullOrEmpty(disableDate)) ? null : DateTime.Parse(disableDate).Date as DateTime?;

			return P500101Service.GetProjectValuation(dcCode, gupCode, custCode, creDateS, creDateE,
				accProjectNo, enableD, disableD, quoteNo, status, accProjectName);
		}

		[WebGet]
		public ExecuteResult DeleteP5001010000(string dcCode, string gupCode, string custCode, string accProjectNo)
		{
			var _wmsTransaction = new WmsTransaction();
			var srv = new P500101Service(_wmsTransaction);
			var result = srv.DeleteP5001010000(dcCode, gupCode, custCode, accProjectNo);
			if (result.IsSuccessed)
				_wmsTransaction.Complete();
			return result;
		}
		#endregion


		#region 儲位報價主檔查詢
		[WebGet]
		public IQueryable<F500101QueryData> GetF500101QueryData(
			string dcCode,string gupCode,string custCode, string enableSDate, string enableEDate, string quoteNo, string status)
		{
			var srv = new P500102Service();
			var result = srv.GetF500101QueryData(dcCode,gupCode,custCode, enableSDate, enableEDate, quoteNo, status);

			return result;
		}
		#endregion

		#region 作業報價主檔查詢
		[WebGet]
		public IQueryable<F500104QueryData> GetF500104QueryData(
			string dcCode, string enableSDate, string enableEDate, string quoteNo, string status)
		{
			var srv = new P500103Service();
			var result = srv.GetF500104QueryData(dcCode, enableSDate, enableEDate, quoteNo, status);

			return result;
		}
		#endregion

		#region 出貨報價主檔查詢
		[WebGet]
		public IQueryable<F500103QueryData> GetF500103QueryData(
			string dcCode, string enableSDate, string enableEDate, string quoteNo, string status)
		{
			var srv = new P500104Service();
			var result = srv.GetF500103QueryData(dcCode, enableSDate, enableEDate, quoteNo, status);

			return result;
		}
		#endregion

		#region 運費報價主檔查詢
		[WebGet]
		public IQueryable<F500102QueryData> GetF500102QueryData(
			string dcCode, string enableSDate, string enableEDate, string quoteNo, string status)
		{
			var srv = new P500105Service();
			var result = srv.GetF500102QueryData(dcCode, enableSDate, enableEDate, quoteNo, status);

			return result;
		}
		#endregion

		#region 其他報價主檔查詢
		[WebGet]
		public IQueryable<F500105QueryData> GetF500105QueryData(
			string dcCode, string enableSDate, string enableEDate, string quoteNo, string status)
		{
			var srv = new P500106Service();
			var result = srv.GetF500105QueryData(dcCode, enableSDate, enableEDate, quoteNo, status);

			return result;
		}
		#endregion

		#region 結算作業
		[WebGet]
		public IQueryable<F500201ClearingData> GetF500201ClearingData(string gupCode, string custCode, string outSourceId, string clearingYearMonth)
		{
			var p500201Service = new P500201Service();
			return p500201Service.GetF500201ClearingData(gupCode, custCode, outSourceId, clearingYearMonth);

		}

		[WebGet]
		public IQueryable<RP7105100001> GetRp7105100001Data(string gupCode, string custCode, string outSourceId, DateTime cntDate)
		{
			var p500201Service = new P500201Service();
			var result = p500201Service.GetRp7105100001Data(gupCode, custCode, outSourceId, cntDate);			
			return result;
		}

		[WebGet]
		public IQueryable<RP7105100002> GetRp7105100002Data(DateTime cntDate, string contractNo)
		{			
			var p500201Service = new P500201Service();			
			return p500201Service.GetRp7105100002Data(cntDate, contractNo);
		}

		[WebGet]
		public IQueryable<RP7105100003> GetRp7105100003Data(DateTime cntDate, string contractNo)
		{			
			var p500201Service = new P500201Service();			
			return p500201Service.GetRp7105100003Data(cntDate, contractNo);
		}

		[WebGet]
		public IQueryable<RP7105100004> GetRp7105100004Data(DateTime cntDate, string contractNo)
		{			
			var p500201Service = new P500201Service();			
			return p500201Service.GetRp7105100004Data(cntDate, contractNo);
		}

		[WebGet]
		public IQueryable<RP7105100005> GetRp7105100005Data(DateTime cntDate, string contractNo)
		{			
			var p500201Service = new P500201Service();			
			return p500201Service.GetRp7105100005Data(cntDate, contractNo);
		}

		[WebGet]
		public ExecuteResult SettlementClosing(DateTime cntDate, string gupCode, string custCode)
		{
			var _wmsTransaction = new WmsTransaction();
			var p500201Service = new P500201Service(_wmsTransaction);
			var result = p500201Service.SettlementClosing(Current.Staff, Current.StaffName, cntDate, gupCode, custCode);
			if (result.IsSuccessed)
				_wmsTransaction.Complete();
			return result;
		}



		#endregion

	}
}
