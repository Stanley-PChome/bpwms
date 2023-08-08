using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1952_HISTORYRepository : RepositoryBase<F1952_HISTORY, Wms3plDbContext, F1952_HISTORYRepository>
    {
        public F1952_HISTORYRepository(string connName, WmsTransaction wmsTransaction = null)
         : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F1952_HISTORY> GetLastFrequencyF1952History(string empId, int unrepeatableFrequency)
        {
            var result = _db.F1952_HISTORYs.Where(x => x.EMP_ID == empId).OrderByDescending(x=>x.CRT_DATE).Take(unrepeatableFrequency);
            return result; 
        }
    }
}
