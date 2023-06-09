using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P01.ExDataSources;
using Wms3pl.WebServices.Process.P01.Services;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P01
{
	public partial class P01ExDataService : DataService<P01ExDataSource>
	{
        #region 進倉明細查詢
        [WebGet]
        public IQueryable<P010202SearchResult> GetF010202(string gupCode, string custCode, string dcCode,
          string changeDateBegin, string changeDateEnd, string itemCode, string itemName, string receiptType)
        {
            var repo = new F010202Repository(Schemas.CoreSchema);

            return repo.GetF010202(gupCode, custCode, dcCode,Convert.ToDateTime(changeDateBegin), Convert.ToDateTime(changeDateEnd));
        }
        #endregion

    }
}
