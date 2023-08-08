using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P15.ExDataSources;
using Wms3pl.WebServices.Process.P15.Services;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P15
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P15ExDataService : DataService<P15ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		#region 調撥相關
		[WebGet]
		public IQueryable<F151001Data> GetF151001Datas(string dcCode, string gupCode, string custCode, string sourceNo)
		{
			var p151001Service = new P150201Service();
			return p151001Service.GetF151001Datas(dcCode, gupCode, custCode, sourceNo);
		}

        /// <summary>
        /// 調撥單明細查詢
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="allocationNo"></param>
        /// <param name="isExpendDate"></param>
        /// <param name="action">動作來源 01:查詢 02:編輯 03:過帳</param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F151001DetailDatas> GetF151001DetailDatas(string dcCode, string gupCode, string custCode, string allocationNo, string isExpendDate = "0", string action = "01")
        {
            var p151001Service = new P150201Service();
            return p151001Service.GetF151001DetailDatas(dcCode, gupCode, custCode, allocationNo, isExpendDate, action);
        }

        [WebGet]
		public IQueryable<F151001ReportData> GetF151001ReportData(string dcCode, string gupCode, string custCode, string allocationNo)
		{
			var p151001Service = new P150201Service();
			return p151001Service.GetF151001ReportData(dcCode, gupCode, custCode, allocationNo);
		}
		[WebGet]
		public IQueryable<F151001ReportDataByExpendDate> GetF151001ReportDataByExpendDate(string dcCode, string gupCode, string custCode, string allocationNo)
		{
			var p151001Service = new P150201Service();
			return p151001Service.GetF151001ReportDataByExpendDate(dcCode, gupCode, custCode, allocationNo);
		}
		[WebGet]
		public IQueryable<F1913WithF1912Moved> GetF1913WithF1912Moveds(string dcCode, string gupCode, string custCode, string srcLocCodeS, string srcLocCodeE, string itemCode, string itemName, string srcWarehouseId,string isExpendDate,string makeNoList)
		{
			var p151001Service = new P150201Service();
			var result = p151001Service.GetF1913WithF1912Moveds(dcCode, gupCode, custCode, srcLocCodeS, srcLocCodeE, itemCode, itemName, srcWarehouseId, isExpendDate, string.IsNullOrWhiteSpace(makeNoList) ? null : makeNoList.Split(',').ToList());
			return result;
		}

		[WebGet]
		public IQueryable<ExecuteResult> CheckLocCodeInWarehouseId(string dcCode, string warehouseId, string locCode)
		{
			var p151001Service = new P150201Service();
			var result = p151001Service.CheckLocCodeInWarehouseId(dcCode, warehouseId, locCode);
			return new List<ExecuteResult> { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<F151002ItemData> GetF151002ItemQty(string dcCode, string gupCode, string custCode, string allocationNo, string itemCode, string locCodeS,string isExpendDate)
		{
			var p151001Service = new P150201Service();
			var result = p151001Service.GetF151002ItemQty(dcCode, gupCode, custCode, allocationNo, itemCode, locCodeS, isExpendDate);
			return result;
		}

        //[WebGet]
        //public IQueryable<F1510Data> GetF1510Data(string dcCode, string gupCode, string custCode, string allocationNo, string allocationDate,string status, string userId)
        //{
        //	var p020301Service = new P151001Service();
        //	return p020301Service.GetF1510Data(dcCode, gupCode, custCode, allocationNo, allocationDate,status, userId);
        //}

        [WebGet]
        public IQueryable<P150201ExportSerial> GetExportSerial(string dcCode, string gupCode, string custCode, string allocationNo)
        {
            var p151201Service = new P150201Service();
            return p151201Service.GetExportSerial(dcCode, gupCode, custCode, allocationNo);
        }
        #endregion

		[WebGet]
		public IQueryable<P1502010000Data> GetAllocationData(string srcDcCode,string tarDcCode,string gupCode,string custCode,
			DateTime crtDateS, DateTime crtDateE, DateTime? postingDateS, DateTime? PostingDateE,
			string allocationNo,string status,string sourceNo,string userName, string containerCode, string allocationType)
		{
			var p151201Service = new P150201Service();
			return p151201Service.GetAllocationData(srcDcCode, tarDcCode, gupCode, custCode, crtDateS, crtDateE,
				postingDateS, PostingDateE, allocationNo, status, sourceNo, userName, containerCode, allocationType);
		}

        [WebGet]
        public IQueryable<P1502010500Data> GetP1502010500Data(string dcCode , string gupCode, string custCode, string allocationNo)
        {
            var p151201Service = new P150201Service();
            return p151201Service.GetP1502010500Data(dcCode, gupCode, custCode, allocationNo);
        }

    }
}