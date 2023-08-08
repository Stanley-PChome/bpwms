using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F25
{
    public partial class F250101Repository : RepositoryBase<F250101, Wms3plDbContext, F250101Repository>
    {
        public F250101Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<P2502QueryData> GetP2502QueryDatas(string gupCode, string custCode,
            string[] itemCode, string[] serialNo, string batchNo, string cellNum, string poNo, string[] wmsNo
            , string status, string retailCode, Int16? combinNo, string crtName, DateTime? updSDate
            , DateTime? updEDate, string boxSerial, string OpItemType)
        {
            var q = from a in _db.F250101s
                    join b in _db.F1903s on new { a.ITEM_CODE, a.GUP_CODE, a.CUST_CODE }
                    equals new { b.ITEM_CODE, b.GUP_CODE, b.CUST_CODE } into bb
                    from b in bb.DefaultIfEmpty()
                    join d in _db.F1929s on a.GUP_CODE equals d.GUP_CODE
                    join e in _db.F1909s on new { a.GUP_CODE, a.CUST_CODE } equals new { e.GUP_CODE, e.CUST_CODE }
                    join h in _db.F000903s on a.ORD_PROP equals h.ORD_PROP into hh
                    from h in hh.DefaultIfEmpty()
                    join i in _db.F1908s on new { a.VNR_CODE, a.GUP_CODE, a.CUST_CODE } equals new { i.VNR_CODE, i.GUP_CODE, i.CUST_CODE } into ii
                    from i in ii.DefaultIfEmpty()
                    join j in _db.F1908s on new { p1 = a.SYS_VNR, p2 = a.GUP_CODE, p3 = a.CUST_CODE } equals new { p1 = j.VNR_CODE, p2 = j.GUP_CODE, p3 = j.CUST_CODE } into jj
                    from j in jj.DefaultIfEmpty()
                    where a.GUP_CODE == gupCode && a.CUST_CODE == custCode
                    select new P2502QueryData()
                    {
                        SERIAL_NO = a.SERIAL_NO,
                        GUP_CODE = a.GUP_CODE,
                        CUST_CODE = a.CUST_CODE,
                        ITEM_CODE = string.IsNullOrEmpty(a.BOUNDLE_ITEM_CODE) ? a.ITEM_CODE : a.BOUNDLE_ITEM_CODE,
                        ITEM_SPEC = b.ITEM_SPEC,
                        STATUS = a.STATUS,
                        TYPE = b.TYPE,
                        BOX_SERIAL = a.BOX_SERIAL,
                        BATCH_NO = a.BATCH_NO,
                        TAG3G = a.TAG3G,
                        PUK = a.PUK,
                        VALID_DATE = a.VALID_DATE,
                        CASE_NO = a.CASE_NO,
                        PO_NO = a.PO_NO,
                        WMS_NO = a.WMS_NO,
                        IN_DATE = a.IN_DATE,
                        ORD_PROP = h.ORD_PROP,
                        ORD_PROP_NAME = h.ORD_PROP_NAME,
                        RETAIL_CODE = a.RETAIL_CODE,
                        ACTIVATED = a.ACTIVATED == "1" ? "是" : "否",
                        PROCESS_NO = a.PROCESS_NO,
                        COMBIN_NO = Convert.ToInt32(a.COMBIN_NO),
                        CELL_NUM = a.CELL_NUM,
                        VNR_NAME = i.VNR_NAME,
                      SYS_NAME = j.VNR_NAME,
                   
                        CAMERA_NO = a.CAMERA_NO,
                        CLIENT_IP = a.CLIENT_IP,
                        ITEM_UNIT = b.ITEM_UNIT,
                        SEND_CUST = a.SEND_CUST == "1" ? "是" : "否",
                        ITEM_NAME = b.ITEM_NAME,
                        GUP_NAME = d.GUP_NAME,
                        CUST_NAME = e.CUST_NAME,
                        CRT_DATE = a.CRT_DATE,
                        CRT_NAME = a.CRT_NAME,
                        UPD_DATE = a.UPD_DATE,
                        UPD_NAME = a.UPD_NAME
                    };
            
            var filterSql = string.Empty;

            //ITEM_CODE

            if (itemCode.Any())
            {
                q = q.Where(c => itemCode.Contains(c.ITEM_CODE));                
            }

            //SERIAL_NO		
            if (serialNo.Any())
            {
                q = q.Where(c => serialNo.Contains(c.SERIAL_NO));                
            }
            //WMS_NO
            if (wmsNo.Any())
            {
                q = q.Where(c => wmsNo.Contains(c.WMS_NO));                
            }

            //BATCH_NO
            if (!string.IsNullOrEmpty(batchNo))
            {
                q = q.Where(c => c.BATCH_NO == batchNo);                
            }

            //CELL_NUM
            if (!string.IsNullOrEmpty(cellNum))
            {
                q = q.Where(c => c.CELL_NUM == cellNum);                
            }

            if (!string.IsNullOrEmpty(poNo))
            {
                q = q.Where(c => c.PO_NO == poNo);
            }

            //STATUS
            if (!string.IsNullOrEmpty(status))
            {
                q = q.Where(c => c.STATUS == status);
            }

            //RETAIL_CODE 客戶代號
            if (!string.IsNullOrEmpty(retailCode))
            {
                q = q.Where(c => c.RETAIL_CODE == retailCode);
            }
            //COMBIN_NO BOUNDEL_ID組合商品編號
            if (combinNo.HasValue && combinNo != 0)
            {
                q = q.Where(c => c.COMBIN_NO == combinNo);
            }

            //CRT_NAME
            if (!string.IsNullOrEmpty(crtName))
            {
                q = q.Where(c => c.CRT_NAME == crtName);
            }

            //修改日期-起
            if (updSDate.HasValue)
            {
                q = q.Where(c => c.UPD_DATE.Value  >= updSDate.Value);
            }
            //修改日期-迄
            if (updEDate.HasValue)
            {
                q = q.Where(c => c.UPD_DATE <= updEDate.Value.AddDays(1));                
            }
            if (!string.IsNullOrEmpty(boxSerial))
            {
                q = q.Where(c => c.BOX_SERIAL == boxSerial);
            }
            if (!string.IsNullOrEmpty(OpItemType))
            {
                q = q.Where(c => c.ORD_PROP == OpItemType);
            }
            q = q.OrderByDescending(c => c.SERIAL_NO).ThenByDescending(c => c.UPD_DATE);
            var result = q.ToList();
            var rI = 1;
            foreach (var item in result)
            {
                item.ROWNUM = rI;
                rI++;
            }
            return result.AsQueryable();

        }
    }
}
