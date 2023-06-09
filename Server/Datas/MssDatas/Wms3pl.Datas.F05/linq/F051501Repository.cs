using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
    public partial class F051501Repository : RepositoryBase<F051501, Wms3plDbContext, F051501Repository>
    {
        public F051501Repository(string connName, WmsTransaction wmsTransaction = null)
                  : base(connName, wmsTransaction)
        {
        }


        public IQueryable<BatchPickStation> GetBatchPickStations(string dcCode, string gupCode, string custCode, string batchNo)
        {
            var f051501Data = _db.F051501s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     x.BATCH_NO == batchNo);

            var vwF000904LangData = _db.VW_F000904_LANGs.AsNoTracking().Where(x => x.TOPIC == "F051501" &&
                                                                                   x.SUBTOPIC == "STATUS" &&
                                                                                   x.LANG == Current.Lang);

            var result = (from A in f051501Data
                          join B in vwF000904LangData
                          on A.STATUS equals B.VALUE
                          select new BatchPickStation
                          {
                              DC_CODE = A.DC_CODE,
                              GUP_CODE = A.GUP_CODE,
                              CUST_CODE = A.CUST_CODE,
                              BATCH_PICK_NO = A.BATCH_PICK_NO,
                              STATION_NO = A.STATION_NO,
                              STATUS = A.STATUS,
                              STATUS_NAME = B.NAME,
                              ITEM_CNT = Convert.ToInt32(A.ITEM_CNT),
                              TOTAL_QTY = Convert.ToInt32(A.TOTAL_QTY) 
                          }).ToList();

            // RowNum
            for (int i = 0; i < result.Count; i++){ result[i].ROWNUM = i + 1; }

            return result.AsQueryable();
        }
    }
}
