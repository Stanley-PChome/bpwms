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
using Wms3pl.WebServices.Process.P01.ExDataSources;
using Wms3pl.WebServices.Process.P01.Services;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P01
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P01ExDataService : DataService<P01ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		[WebGet]
		public IQueryable<F010201Data> GetF010201Datas(string dcCode, string gupCode, string custCode, string begStockDate,
			string endStockDate, string stockNo, string vnrCode, string vnrName, string custOrdNo, string sourceNo, string status, string userClosed)
		{
			var p010201Service = new P010201Service();
			return p010201Service.GetF010201Datas(dcCode, gupCode, custCode, begStockDate, endStockDate, stockNo, vnrCode,
				vnrName, custOrdNo, sourceNo, status, userClosed);
		}

		[WebGet]
		public IQueryable<F010202Data> GetF010202Datas(string dcCode, string gupCode, string custCode, string stockNo)
		{
			var p010201Service = new P010201Service();
			return p010201Service.GetF010202Datas(dcCode, gupCode, custCode, stockNo);
		}

        [WebGet]
		public IQueryable<ExecuteResult> DeleteP010201(string stockNo, string gupCode, string custCode, string dcCode)
		{
			var wmsTransaction = new WmsTransaction();
			var p010201Srv = new P010201Service(wmsTransaction);
			var result = p010201Srv.DeleteP010201(stockNo, gupCode, custCode, dcCode);
			if (result.IsSuccessed) wmsTransaction.Complete();

			return (new List<ExecuteResult> { result }).AsQueryable();
		}

		#region 代採購作業
		[WebGet]
		public IQueryable<F010101ShopNoList> GetF010101ShopNoList(string dcCode, string gupCode, string custCode,
			string shopDateS, string shopDateE, string shopNo, string vnrCode, string vnrName, string itemCode, string custOrdNo,
			string status)
		{
			var p010101Service = new P010101Service();
			return p010101Service.GetF010101ShopNoList(dcCode, gupCode, custCode,
				shopDateS, shopDateE, shopNo, vnrCode, vnrName, itemCode, custOrdNo, status);
		}

		[WebGet]
		public IQueryable<F010101Data> GetF010101Datas(string dcCode, string gupCode, string custCode, string shopNo)
		{
			var p010101Service = new P010101Service();
			return p010101Service.GetF010101Datas(dcCode, gupCode, custCode, shopNo);
		}

		[WebGet]
		public IQueryable<F010102Data> GetF010102Datas(string dcCode, string gupCode, string custCode, string shopNo)
		{
			var p010101Service = new P010101Service();
			return p010101Service.GetF010102Datas(dcCode, gupCode, custCode, shopNo);
		}

		[WebGet]
		public IQueryable<F010101ReportData> GetF010101Reports(string dcCode, string gupCode, string custCode, string shopNo)
		{
			var p010101Service = new P010101Service();
			return p010101Service.GetF010101Reports(dcCode, gupCode, custCode, shopNo);
		}
		#endregion

		#region 
		[WebGet]
		public IQueryable<F1913WithF1912Qty> GetF1913WithF1912Qty(string dcCode, string gupCode, string custCode,
			 string itemCode, string dataTable)
		{
			var p010201Service = new P010201Service();
			return p010201Service.GetF1913WithF1912Qty(dcCode, gupCode, custCode, itemCode, dataTable);
		}
		#endregion


		#region 列印棧板標籤資料
		[WebGet]
		public IQueryable<P010201PalletData> GetP010201PalletDatas(string dcCode, string gupCode, string custCode, string stockNo)
		{
			var service = new P010201Service();
			return service.GetP010201PalletDatas(dcCode, gupCode, custCode, stockNo);
		}

        #endregion

        #region 廠商報到Grid
        [WebGet]
        public IQueryable<F010202Data> GetF010202DatasMargeValidate(string dcCode, string gupCode, string custCode, string stockNo)
        {
            var p010201Service = new P010201Service();
            return p010201Service.GetF010202DatasMargeValidate(dcCode, gupCode, custCode, stockNo);
        }
				[WebGet]
				public IQueryable<F010201MainData> GetF010202DatasMargeValidateChange(string dcCode, string gupCode, string custCode, string stockNo)
				{
					var p010201Service = new P010201Service();
					return p010201Service.GetF010202DatasMargeValidateChange(dcCode, gupCode, custCode, stockNo);
				}
		#endregion

	}
}
