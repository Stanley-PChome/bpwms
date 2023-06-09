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
using Wms3pl.WebServices.Process.P20.ExDataSources;
using Wms3pl.WebServices.Process.P20.Services;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P20
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P20ExDataService : DataService<P20ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		#region P2001010000 異動調整作業(訂單,商品,盤點庫存)

		[WebGet]
		public IQueryable<F200101Data> GetF200101Datas(string dcCode, string gupCode, string custCode, string adjustNo,
			string adjustType, string workType, string begAdjustDate, string endAdjustDate)
		{
			var p200101Service = new P200101Service();
			return p200101Service.GetF200101Datas(dcCode, gupCode, custCode, adjustNo, adjustType, workType, begAdjustDate,
				endAdjustDate);
		}

		[WebGet]
		public IQueryable<F200101Data> GetF200101DatasByAdjustType1Or2(string dcCode, string gupCode, string custCode, string adjustNo,
			string adjustType, string workType, string begAdjustDate, string endAdjustDate)
		{
			var p200101Service = new P200101Service();
			return p200101Service.GetF200101DatasByAdjustType1Or2(dcCode, gupCode, custCode, adjustNo, adjustType, workType, begAdjustDate,
				endAdjustDate);
		}

		[WebGet]
		public IQueryable<F200102Data> GetF200102Datas(string dcCode, string gupCode, string custCode, string adjustNo,string workType)
		{
			var p200101Service = new P200101Service();
			return p200101Service.GetF200102Datas(dcCode, gupCode, custCode, adjustNo, workType);
		}


		[WebGet]
		public IQueryable<F050301Data> GetF050301Datas(string dcCode, string gupCode, string custCode, string delvDate,
			string pickTime, string custOrdNo, string itemCode, string consignee, string ordNo,string workType)
		{
			var p200101Service = new P200101Service();
			return p200101Service.GetF050301Datas(dcCode, gupCode, custCode, delvDate, pickTime, custOrdNo, itemCode, consignee,ordNo, workType);
		}

		[WebGet]
		public IQueryable<F0513Data> GetF0513Datas(string dcCode, string gupCode, string custCode, string delvDate)
		{
			var p200101Service = new P200101Service();
			return p200101Service.GetF0513Datas(dcCode, gupCode, custCode, delvDate);
		}

		[WebGet]
		public IQueryable<ExecuteResult> UpdateP200101ByAdjustTye0(string dcCode, string gupCode, string custCode, string adjustNo,
			int adjustSeq, string workType, string address, string newDcCode,
			string cause, string causeMemo)
		{
			var wmsTransaction = new WmsTransaction();
			var p200101Service = new P200101Service(wmsTransaction);
			var result = p200101Service.UpdateP200101ByAdjustTye0(dcCode,gupCode,custCode,adjustNo,adjustSeq,workType,address,newDcCode,cause,causeMemo);
		  if(result.IsSuccessed)
				 wmsTransaction.Complete();
			return new List<ExecuteResult>() {result}.AsQueryable();
		}

		[WebGet]
		public IQueryable<ExecuteResult> DeleteP200101ByAdjustType0(string dcCode, string gupCode, string custCode, string adjustNo)
		{
			var wmsTransaction = new WmsTransaction();
			var p200101Service = new P200101Service(wmsTransaction);
			var result = p200101Service.DeleteP200101ByAdjustType0(dcCode, gupCode, custCode, adjustNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return new List<ExecuteResult>() { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<ExecuteResult> DeleteP200101DetailByAdjustType0(string dcCode, string gupCode, string custCode, string adjustNo,
			int adjustSeq)
		{
		
			var wmsTransaction = new WmsTransaction();
			var p200101Service = new P200101Service(wmsTransaction);
			var result = p200101Service.DeleteP200101DetailByAdjustType0Selected(dcCode, gupCode, custCode, adjustNo, adjustSeq);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return new List<ExecuteResult>() { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<F200103Data> GetF200103Datas(string dcCode, string gupCode, string custCode, string adjustNo)
		{
			var p200101Service = new P200101Service();
			return p200101Service.GetF200103Datas(dcCode, gupCode, custCode, adjustNo);
		}

		[WebGet]
		public IQueryable<F1913Data> GetF1913Datas(string dcCode, string gupCode, string custCode, string warehouseId,
			string itemCode, string itemName)
		{
			var p200101Service = new P200101Service();
			return p200101Service.GetF1913Datas(dcCode, gupCode, custCode, warehouseId,itemCode,itemName);
		}
		[WebGet]
		public IQueryable<ExecuteResult> UpdateP200101ByAdjustType1(string dcCode, string gupCode, string custCode, string adjustNo,
			int adjustSeq, string cause, string causeMemo)
		{
			var wmsTransaction = new WmsTransaction();
			var p200101Service = new P200101Service(wmsTransaction);
			var result = p200101Service.UpdateP200101ByAdjustType1(dcCode, gupCode, custCode, adjustNo,adjustSeq,cause,causeMemo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return new List<ExecuteResult>() { result }.AsQueryable();

		}

		[WebGet]
		public IQueryable<ExecuteResult> CheckItemLocTmpr(string dcCode, string gupCode, string itemCode, string custCode, string locCode)
		{
			var sharedService = new SharedService();
			var message = sharedService.CheckItemLocTmpr(dcCode, gupCode, itemCode, custCode, locCode);
			return
				new List<ExecuteResult> {new ExecuteResult {IsSuccessed = string.IsNullOrEmpty(message), Message = message}}
					.AsQueryable();
		}
		#endregion
	}
}
