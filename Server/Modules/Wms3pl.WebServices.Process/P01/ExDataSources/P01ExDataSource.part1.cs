using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P01.ExDataSources
{
	public partial class P01ExDataSource
	{
        #region 進倉明細查詢
        public IQueryable<P010202SearchResult> P010202SearchResults
        {
            get { return new List<P010202SearchResult>().AsQueryable(); }
        }
        #endregion

    }
}
