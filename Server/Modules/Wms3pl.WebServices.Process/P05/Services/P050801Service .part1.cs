using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050801Service
	{
        #region P0503060000  出貨回傳檔下載
        public IQueryable<GetF050901CSV> GetF050901CSV(string dcCode, string gupCode, string custCode, DateTime? begCrtDate, DateTime? endCrtDate)
        {
            var repo = new F050901Repository(Schemas.CoreSchema);
            return repo.GetF050901CSV(dcCode, gupCode, custCode, begCrtDate, endCrtDate);

        }

        #endregion
    }
}