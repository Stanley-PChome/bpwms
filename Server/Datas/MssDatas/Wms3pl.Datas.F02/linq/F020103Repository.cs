using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F02
{
    public partial class F020103Repository : RepositoryBase<F020103, Wms3plDbContext, F020103Repository>
    {
        public F020103Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public F020103Detail Find(string date, int serialNo, string purchaseNo, string dcCode, string gupCode, string custCode)
        {
            return _db.F020103s.Where(x => x.ARRIVE_DATE.Date == DateTime.Parse(date).Date)
                .Where(x => x.SERIAL_NO == serialNo)
                .Where(x => x.PURCHASE_NO == purchaseNo)
                .Where(x => x.DC_CODE == dcCode)
                .Where(x => x.GUP_CODE == gupCode)
                .Where(x => x.CUST_CODE == custCode)
                .Select(x=>new F020103Detail
                {
                    //缺少欄位]如下\：
                    //VNR_NAME = x.VNR_NAME,
                    //CUST_NAME = x.CUST_NAME,
                    //ARRIVE_TIME_DESC = x.ARRIVE_TIME_DESC,
                    //TOTALTIME = x.TOTALTIME,

                    ARRIVE_DATE = x.ARRIVE_DATE,
                    ARRIVE_TIME = x.ARRIVE_TIME,
                    PURCHASE_NO = x.PURCHASE_NO,
                    PIER_CODE = x.PIER_CODE,
                    VNR_CODE = x.VNR_CODE,
                    CAR_NUMBER = x.CAR_NUMBER,
                    BOOK_INTIME = x.BOOK_INTIME,
                    INTIME = x.INTIME,
                    OUTTIME = x.OUTTIME,
                    DC_CODE = x.DC_CODE,
                    GUP_CODE = x.GUP_CODE,
                    CUST_CODE = x.CUST_CODE,
                    CRT_DATE = x.CRT_DATE,
                    CRT_STAFF = x.CRT_STAFF,
                    UPD_DATE = x.UPD_DATE,
                    UPD_STAFF = x.UPD_STAFF,
                    ITEM_QTY = x.ITEM_QTY,
                    ORDER_QTY = x.ORDER_QTY,
                    ORDER_VOLUME = x.ORDER_VOLUME,
                    SERIAL_NO = x.SERIAL_NO,
                    CRT_NAME = x.CRT_NAME,
                    UPD_NAME = x.UPD_NAME,
                })
                .FirstOrDefault();
        }

        /// <summary>
        /// 取得新ID
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public Int16 GetNewId(string dcCode, string gupCode, string custCode, string purchaseNo, DateTime date)
        {

            var maxSerialNo = this.Filter(x => x.DC_CODE.Equals(EntityFunctions.AsNonUnicode(dcCode))
                                            && x.GUP_CODE.Equals(EntityFunctions.AsNonUnicode(gupCode))
                                            && x.CUST_CODE.Equals(EntityFunctions.AsNonUnicode(custCode))
                                            && x.PURCHASE_NO.Equals(EntityFunctions.AsNonUnicode(purchaseNo))
                                            && x.ARRIVE_DATE.Equals(date))
                                  .Max(x => (short?)x.SERIAL_NO);
            return (short)(maxSerialNo.HasValue ? maxSerialNo.Value + 1 : 1);
        }
    }
}
