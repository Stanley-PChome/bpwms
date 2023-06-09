using System.ServiceModel;
using System.ServiceModel.Activation;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.Schedule.Export.Services;

namespace Wms3pl.WebServices.Schedule.Export
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

	public class ExportWcfService
	{
		[OperationContract]
		public ApiResult ExportWarehouseInResults(ExportResultReq req)
		{
			ExportServices epService = new ExportServices();
			return epService.ExportWarehouseInResults(req);
		}

		[OperationContract]
		public ApiResult ExportOrderResults(ExportResultReq req)
		{
			ExportServices epService = new ExportServices();
			return epService.ExportOrderResults(req);
		}

		[OperationContract]
		public ApiResult ExportVendorReturnResults(ExportResultReq req)
		{
			ExportServices epService = new ExportServices();
			return epService.ExportVendorReturnResults(req);
		}

		[OperationContract]
		public ApiResult ExpStockAlertResults(ExportResultReq req)
		{
			ExportServices epService = new ExportServices();
			return epService.ExpStockAlertResults(req);
		}

		[OperationContract]
		public ApiResult ExportHomeDeliveryResults(ExportResultReq req)
		{
			ExportServices epService = new ExportServices();
			return epService.ExportHomeDeliveryResults(req);
		}

		[OperationContract]
		public ApiResult ExportStockMovementResults(ExportResultReq req)
		{
			ExportServices epService = new ExportServices();
			return epService.ExportStockMovementResults(req);
		}

        [OperationContract]
        public ApiResult ExportSysErrorNotifyResults(ExportResultReq req)
        {
            ExportServices epService = new ExportServices();
            return epService.ExportSysErrorNotifyResults(req);
        }
    }
}
