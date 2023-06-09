using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F70
{
	public partial class F700101Repository : RepositoryBase<F700101, Wms3plDbContext, F700101Repository>
	{
		public F700101Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
        public IQueryable<F700101> GetF700101ByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            return _db.F700101s.Join(_db.F700102s, a => new { a.DC_CODE, a.DISTR_CAR_NO }, b => new { b.DC_CODE, b.DISTR_CAR_NO }, (a, b) => new { a, b })
                      .Where(x => x.a.DC_CODE == dcCode)
                      .Where(x => x.b.GUP_CODE == gupCode)
                      .Where(x => x.b.CUST_CODE == custCode)
                      .Where(x => x.b.WMS_NO == wmsOrdNo)
                      .Select(x => x.a);
            //string sql = @"SELECT A.*
			//								 FROM F700101 A
			//								 JOIN F700102 B ON A.DC_CODE = B.DC_CODE AND A.DISTR_CAR_NO = B.DISTR_CAR_NO
			//								WHERE A.DC_CODE = @p0 AND B.GUP_CODE = @p1 AND B.CUST_CODE = @p2  AND B.WMS_NO = @p3";
        }
        public IQueryable<F700101EX> GetF700101ByDistrCarNo(string distrCarNo, string dcCode)
        {
            return _db.F700101s
                        .Where(x => x.DISTR_CAR_NO == distrCarNo)
                        .Where(x => x.DC_CODE == dcCode)
                        .Select(a => new F700101EX
                        {
                            DISTR_CAR_NO = a.DISTR_CAR_NO,
                            TAKE_DATE = a.TAKE_DATE,
                            ALL_ID = a.ALL_ID,
                            CAR_KIND_ID = a.CAR_KIND_ID,
                            SP_CAR = a.SP_CAR,
                            CHARGE_CUST = a.CHARGE_CUST,
                            CHARGE_DC = a.CHARGE_DC,
                            FEE = a.FEE,
                            STATUS = a.STATUS,
                            DC_CODE = a.DC_CODE,
                            CRT_STAFF = a.CRT_STAFF,
                            CRT_DATE = a.CRT_DATE,
                            UPD_STAFF = a.UPD_STAFF,
                            UPD_DATE = a.UPD_DATE,
                            CRT_NAME = a.CRT_NAME,
                            UPD_NAME = a.UPD_NAME,
                            CHARGE_GUP_CODE = a.CHARGE_GUP_CODE,
                            CHARGE_CUST_CODE = a.CHARGE_CUST_CODE,
                            DISTR_SOURCE = a.DISTR_SOURCE,
                            HAVE_WMS_NO = a.HAVE_WMS_NO,
                        });
        }

        /// <summary>
        /// 從F700102的WMS_NO找F700101
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsNo"></param>
        /// <returns></returns>
        public F700101 FromF700102(string dcCode, string gupCode, string custCode, string wmsNo)
        {
            var q = _db.F700102s
                .Join(_db.F700101s, a => new { a.DISTR_CAR_NO, a.DC_CODE }, b => new { b.DISTR_CAR_NO, b.DC_CODE }, (a, b) => new { a, b })
                .Where(x => x.a.DC_CODE == dcCode)
                .Where(x => x.a.GUP_CODE == gupCode)
                .Where(x => x.a.CUST_CODE == custCode)
                .Where(x => x.a.WMS_NO == wmsNo);
            return q.Select(x=>x.b).SingleOrDefault();

        }

        public IQueryable<F700101> GetDatas(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
        {
            var q = _db.F700101s
                    .Join(_db.F700102s, a => new { a.DC_CODE, a.DISTR_CAR_NO }, b => new { b.DC_CODE, b.DISTR_CAR_NO }, (a, b) => new { a, b })
                    .Where(x => x.b.DC_CODE == dcCode)
                    .Where(x => x.b.GUP_CODE == gupCode)
                    .Where(x => x.b.CUST_CODE == custCode);
            q = q.Where(x=>wmsOrdNos.Contains(x.b.WMS_NO));
            return q.Select(x => x.a).Distinct();
        }
    }
}