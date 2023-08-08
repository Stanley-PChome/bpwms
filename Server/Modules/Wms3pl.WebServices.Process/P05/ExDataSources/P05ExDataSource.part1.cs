using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F05;

namespace Wms3pl.WebServices.Process.P05.ExDataSources
{
  public partial class P05ExDataSource
  {
        //出貨回傳檔下載
        public IQueryable<GetF050901CSV> GetF050901CSV
        {
            get { return new List<GetF050901CSV>().AsQueryable(); }
        }
    }
}
