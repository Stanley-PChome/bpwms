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
        #region test


        public IQueryable<F010301Main> F010301Mains
        {
            get { return new List<F010301Main>().AsQueryable(); }
        }

        public IQueryable<F010302Detail> F010302Details
        {
            get { return new List<F010302Detail>().AsQueryable(); }
        }


        public IQueryable<F010302CollectDetail> F010302CollectDetails
        {
            get { return new List<F010302CollectDetail>().AsQueryable(); }
        }

        #endregion
    }
}
