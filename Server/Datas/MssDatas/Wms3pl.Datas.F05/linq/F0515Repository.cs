using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F0515Repository : RepositoryBase<F0515, Wms3plDbContext, F0515Repository>
    {
        public F0515Repository(string connName, WmsTransaction wmsTransaction = null)
               : base(connName, wmsTransaction)
        {
        }

        public F0515 GetData(string dcCode, string gupCode, string custCode, string batchNo)
        {
            var result = _db.F0515s.Where(x => x.DC_CODE == dcCode &&
                                               x.GUP_CODE == gupCode &&
                                               x.CUST_CODE == custCode &&
                                               x.BATCH_NO == batchNo).FirstOrDefault();

            return result;
        }  

        public F0515 GetAGVHasWorkBatchByNotInBatchNo(string dcCode, string gupCode, string custCode, string batchNo)
        {
            var result = _db.F0515s.Where(x => x.DC_CODE == dcCode &&
                                               x.GUP_CODE == gupCode &&
                                               x.CUST_CODE == custCode &&
                                               x.BATCH_NO != batchNo &&
                                               x.PICK_TOOL == "4" &&
                                               x.PICK_STATUS == "1").FirstOrDefault();

            return result;
        }
    }
}
