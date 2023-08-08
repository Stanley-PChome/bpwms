using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F191302Repository : RepositoryBase<F191302, Wms3plDbContext, F191302Repository>
	{
		public F191302Repository(string connName, WmsTransaction wmsTransaction = null)
						 : base(connName, wmsTransaction)
		{
		}

		public IQueryable<StockAbnormalData> GetStockAbnormalData(string dcCode, string gupCode, string custCode,
			DateTime? begCrtDate, DateTime? endCrtDate, string srcType, string srcWmsNo, string procFlag, string allocationNo, string itemCode)
		{
			var f191302s = _db.F191302s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode);

			if (begCrtDate != null)
				f191302s = f191302s.Where(x => x.CRT_DATE >= begCrtDate);
			if (endCrtDate != null)
			{
				var endCrtDateTmp = endCrtDate.Value.AddDays(1).AddSeconds(-1);
				f191302s = f191302s.Where(x => x.CRT_DATE <= endCrtDateTmp);
			}
				
			if (!string.IsNullOrWhiteSpace(srcType))
				f191302s = f191302s.Where(x => x.SRC_TYPE == srcType);
			if (!string.IsNullOrWhiteSpace(srcWmsNo))
				f191302s = f191302s.Where(x => x.SRC_WMS_NO == srcWmsNo);
			if (!string.IsNullOrWhiteSpace(procFlag))
				f191302s = f191302s.Where(x => x.PROC_FLAG == procFlag);
			if (!string.IsNullOrWhiteSpace(allocationNo))
				f191302s = f191302s.Where(x => x.ALLOCATION_NO == allocationNo);
			if (!string.IsNullOrWhiteSpace(itemCode))
				f191302s = f191302s.Where(x => x.ITEM_CODE == itemCode);

			var f1903s = _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			f191302s.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE));

			var f1980s = _db.F1980s.AsNoTracking().Where(x => x.DC_CODE == dcCode && f191302s.Select(z => z.SRC_WAREHOUSE_ID).Contains(x.WAREHOUSE_ID));

			var srcTypeList = _db.VW_F000904_LANGs.AsNoTracking().Where(x => x.TOPIC == "F191302" && x.SUBTOPIC == "SRC_TYPE");

			var procFlagList = _db.VW_F000904_LANGs.AsNoTracking().Where(x => x.TOPIC == "F191302" && x.SUBTOPIC == "PROC_FLAG");

			var datas = (from A in f191302s
									 join B in f1903s
									 on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE } into subB
									 from B in subB.DefaultIfEmpty()
									 join C in srcTypeList
									 on A.SRC_TYPE equals C.VALUE into subC
									 from C in subC.DefaultIfEmpty()
									 join D in procFlagList
									 on A.PROC_FLAG equals D.VALUE into subD
									 from D in subD.DefaultIfEmpty()
									 join E in f1980s
									 on new { WAREHOUSE_ID = A.SRC_WAREHOUSE_ID,A.DC_CODE } equals new { E.WAREHOUSE_ID,E.DC_CODE } into subE
									 from E in subE.DefaultIfEmpty()
									 orderby A.ID
									 select new StockAbnormalData
									 {
										 IsSelected = false,
										 ID = A.ID,
										 DC_CODE = A.DC_CODE,
										 GUP_CODE = A.GUP_CODE,
										 CUST_CODE = A.CUST_CODE,
										 SRC_WMS_NO = A.SRC_WMS_NO,
										 SRC_TYPE = A.SRC_TYPE,
										 SRC_TYPE_NAME = C.NAME ?? null,
										 ALLOCATION_NO = A.ALLOCATION_NO,
										 ALLOCATION_SEQ = A.ALLOCATION_SEQ,
										 SRC_WAREHOUSE_ID = A.SRC_WAREHOUSE_ID,
										 SRC_WAREHOUSE_NAME = E.WAREHOUSE_NAME ?? null,
										 SRC_LOC_CODE = A.SRC_LOC_CODE,
										 ITEM_CODE = A.ITEM_CODE,
										 ITEM_NAME = B.ITEM_NAME ?? null,
										 VALID_DATE = A.VALID_DATE,
										 MAKE_NO = A.MAKE_NO,
										 QTY = A.QTY,
										 PROC_FLAG = A.PROC_FLAG,
										 PROC_FLAG_NAME = D.NAME ?? null,
										 MEMO = A.MEMO,
										 CRT_NAME = A.CRT_NAME,
										 CRT_DATE = A.CRT_DATE,
										 UPD_NAME = A.UPD_NAME,
										 UPD_DATE = A.UPD_DATE,
                     PROC_WMS_NO = A.PROC_WMS_NO,
                     SERIAL_NO = A.SERIAL_NO
                   }).ToList();

			for (int i = 0; i < datas.Count; i++) { datas[i].ROWNUM = i + 1; }

			return datas.AsQueryable();
		}
	}
}
