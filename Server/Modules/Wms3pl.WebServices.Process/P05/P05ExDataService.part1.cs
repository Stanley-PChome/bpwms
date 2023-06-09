using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P05.ExDataSources;
using Wms3pl.WebServices.Process.P05.Services;

namespace Wms3pl.WebServices.Process.P05
{
	public partial class P05ExDataService
	{
        //出貨回傳檔下載
        [WebGet]
        public IQueryable<GetF050901CSV> GetF050901CSVData(string dcCode, string gupCode, string custCode, DateTime? begCrtDate, DateTime? endCrtDate) 
        {    
            var p050801Service = new P050801Service();   
            return p050801Service.GetF050901CSV(dcCode, gupCode, custCode, begCrtDate, endCrtDate);
        }
    }
}
