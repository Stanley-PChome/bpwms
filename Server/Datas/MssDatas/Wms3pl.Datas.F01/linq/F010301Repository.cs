using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
    public partial class F010301Repository : RepositoryBase<F010301, Wms3plDbContext, F010301Repository>
    {
        public F010301Repository(string connName, WmsTransaction wmsTransaction = null)
                : base(connName, wmsTransaction)
        {
        }

        public ExecuteResult DeleteF010301ScanCargoDatas(F010301[] front_f010301s)
        {
            foreach (var item in front_f010301s)
            {
                Delete(d => d.ID == item.ID);
            }
            _wmsTransaction.Complete();
            return new ExecuteResult(true);
        }

        public ExecuteResult UpdateF010301ScanCargoMemo(F010301 f010301Data)
        {
            UpdateFields(new { f010301Data.MEMO }, x => x.ID == f010301Data.ID);
            //Update(f010301Data);
            _wmsTransaction.Complete();
            return new ExecuteResult(true);
        }

    }
}
