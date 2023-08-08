using System.Collections.Generic;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;

namespace Wms3pl.Datas.F05
{
    public partial class F05000101Repository : RepositoryBase<F05000101, Wms3plDbContext, F05000101Repository>
    {
        public void BulkDelete(List<F05000101> data)
        {
            SqlBulkDeleteForAnyCondition(data, "F05000101", new List<string> { "ID" });
        }

        public void BulkInsertData(List<F05000101> data)
        {
            DateTime now = DateTime.Now;

            data.ForEach(x =>
            {
                x.ORDDATA = x.ORDDATA ?? "";
                x.ERRMSG = x.ERRMSG ?? "";
                x.CRT_DATE = now;
                x.CRT_STAFF = Current.Staff;
                x.CRT_NAME = Current.StaffName;
            });

            BulkInsert(data);
        }

        public void BulkUpdateData(List<F05000101> data)
        {
            DateTime now = DateTime.Now;

            data.ForEach(x =>
            {
                x.ERRMSG = x.ERRMSG ?? "";
                x.UPD_DATE = now;
                x.UPD_STAFF = Current.Staff;
                x.UPD_NAME = Current.StaffName;
            });

            BulkUpdate(data);
        }
    }
}
