using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F192402Repository : RepositoryBase<F192402, Wms3plDbContext, F192402Repository>
    {
        public F192402Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        public IQueryable<DcList> GetUserDcList(string AccNo)
        {

            var f192402s = _db.F192402s.AsNoTracking().Where(x => x.EMP_ID == AccNo);
            var f1901s = _db.F1901s.AsNoTracking();
            var result = (from A in f192402s
                          join B in f1901s on A.DC_CODE equals B.DC_CODE
                          select new DcList
                          {
                              DcNo = B.DC_CODE,
                              DcName = B.DC_NAME
                          }).Distinct();

            return result;
        }

        public IQueryable<GupList> GetUserGupList(string AccNo)
        {
            var f192402s = _db.F192402s.AsNoTracking().Where(x => x.EMP_ID == AccNo);
            var f1929s = _db.F1929s.AsNoTracking();
            var result = (from A in f192402s
                          join B in f1929s on A.GUP_CODE equals B.GUP_CODE
                          select new GupList
                          {
                              GupNo = B.GUP_CODE,
                              GupName = B.GUP_NAME
                          }).Distinct();
            return result;
        }

        /// <summary>
        /// 檢核人員貨主權限
        /// </summary>
        /// <param name="custCode">貨主編號</param>
        /// <param name="empId">帳號</param>
        /// <returns></returns>
        public int CheckAccCustCode(string custCode, string empId)
        {
            var result = _db.F192402s.AsNoTracking().Where(x => x.CUST_CODE == custCode
                                                          && x.EMP_ID == empId).Count();
            return result;
        }

        /// <summary>
        /// 檢核人員物流中心權限
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="empId">帳號</param>
        /// <returns></returns>
        public int CheckAccDc(string dcCode, string empId)
        {
            var result = _db.F192402s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                                        && x.EMP_ID == empId).Count();

            return result;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="AccNo"></param>
        /// <returns></returns>
        public IQueryable<CustList> GetUserCustList(string AccNo)
        {
            var f192402s = _db.F192402s.AsNoTracking().Where(x => x.EMP_ID == AccNo);
            var f1909s = _db.F1909s.AsNoTracking();
            var result = (from A in f192402s
                          join B in f1909s on A.CUST_CODE equals B.CUST_CODE
                          select new CustList
                          {
                              CustNo = B.CUST_CODE,
                              CustName = B.CUST_NAME,
                              GupNo = B.GUP_CODE,
                              AllowEditBoxQty = B.ALLOW_EDIT_BOX_QTY,
                              ShowMessage = B.SHOW_MESSAGE,
                              ShowQty = B.SHOW_QTY =="1" ? "2" : B.SHOW_QTY,
                          }).Distinct();
            return result;

        }
    }
}
