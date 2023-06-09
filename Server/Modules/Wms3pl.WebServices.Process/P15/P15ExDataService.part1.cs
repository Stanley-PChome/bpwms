using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F15; 
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P15.ExDataSources;
using Wms3pl.WebServices.Process.P15.Services;
 
namespace Wms3pl.WebServices.Process.P15
{
	public partial class P15ExDataService
	{
        //調撥匯出 
        [WebGet]
        public IQueryable<GetF150201CSV> GetF150201CSVData(string gupCode, string custCode, string SourceDcCode, string TargetDcCode, DateTime CRTDateS, DateTime CRTDateE, string TxtSearchAllocationNo, DateTime? PostingDateS, DateTime? PostingDateE, string SourceWarehouseList, string TargetWarehouseList, string StatusList, string TxtSearchSourceNo)
        {
            var p150201Service = new P150201Service();
            return p150201Service.GetF150201CSV(gupCode, custCode, SourceDcCode, TargetDcCode, CRTDateS, CRTDateE, TxtSearchAllocationNo, PostingDateS, PostingDateE, SourceWarehouseList, TargetWarehouseList, StatusList, TxtSearchSourceNo);
        }
    }
}
