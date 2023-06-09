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
	public partial class F700102Repository : RepositoryBase<F700102, Wms3plDbContext, F700102Repository>
	{
		public F700102Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
        }

        /// <summary>
        /// 是否該出貨單存在於待處理的派車單明細
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsOrdNo"></param>
        /// <returns></returns>
        public bool ExistsF700102ByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo, string sourceNo)
        {
            var q = _db.F700101s
                    .Join(_db.F700102s, a => new { a.DC_CODE, a.DISTR_CAR_NO }, b => new { b.DC_CODE, b.DISTR_CAR_NO }, (a, b) => new { a, b })
                    .Where(x => x.a.STATUS == "0")
                    .Where(x => x.b.DC_CODE == dcCode)
                    .Where(x => x.b.GUP_CODE == gupCode)
                    .Where(x => x.b.CUST_CODE == custCode)
                    .Where(x => (x.b.WMS_NO == wmsOrdNo || x.b.WMS_NO == sourceNo)).Any();
            return q;
        }


        /// <summary>
        /// 以出貨單號取得尚未取消的派車單明細
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsNos"></param>
        /// <returns></returns>
        public IQueryable<F700102> GetNotCancelF700102ByWmsNos(IEnumerable<string> wmsNos)
        {
            return _db.F700101s
                        .Join(_db.F700102s, a => new { a.DC_CODE, a.DISTR_CAR_NO }, b => new { b.DC_CODE, b.DISTR_CAR_NO }, (a, b) => new { a, b })
                        .Where(x => x.a.STATUS != "9")
                        .Where(x => wmsNos.Contains(x.b.WMS_NO))

                        .Select(x => x.b);
        }
        public IQueryable<F700102> GetF700102ByWmsNos(string dcCode, string gupCode, string custCode, string status, IEnumerable<string> wmsNos)
        {
            return _db.F700101s
                .Join(_db.F700102s, a => new { a.DC_CODE, a.DISTR_CAR_NO }, b => new { b.DC_CODE, b.DISTR_CAR_NO }, (a, b) => new { a, b })
                .Where(x => x.b.DC_CODE == dcCode)
                .Where(x => x.b.GUP_CODE == gupCode)
                .Where(x => x.b.CUST_CODE == custCode)
                .Where(x => x.a.STATUS == status)
                .Where(x=> wmsNos.Contains(x.b.WMS_NO))
                .Select(x => x.b);
        }
        public IQueryable<F700102> GetDatas(string dcCode, string distrCarNo)
        {
            return _db.F700102s
                .Where(x => x.DC_CODE == dcCode)
                .Where(x => x.DISTR_CAR_NO == distrCarNo)
                .Select(x => x);
        }
        public F700102 GetDataByWmsNo(string dcCode, string gupCode, string custCode, string wmsNo)
        {
            var q = _db.F700101s
                    .Join(_db.F700102s, a => new { a.DC_CODE, a.DISTR_CAR_NO }, b => new { b.DC_CODE, b.DISTR_CAR_NO }, (a, b) => new { a, b })
                    .Where(x => x.a.STATUS != "9")
                    .Where(x => x.b.DC_CODE == dcCode)
                    .Where(x => x.b.GUP_CODE == gupCode)
                    .Where(x => x.b.CUST_CODE == custCode)
                    .Where(x => x.b.WMS_NO == wmsNo)
                    .Select(x => x.b)
                    .SingleOrDefault();
            return q;
        }

        public List<F700102> GetDataByWmsNos(string dcCode, string gupCode, string custCode, List<string> wmsNos)
        {
            return _db.F700101s
                    .Join(_db.F700102s, a => new { a.DC_CODE, a.DISTR_CAR_NO }, b => new { b.DC_CODE, b.DISTR_CAR_NO }, (a, b) => new { a, b })
                    .Where(x => x.a.STATUS != "9")
                    .Where(x => x.b.DC_CODE == dcCode)
                    .Where(x => x.b.GUP_CODE == gupCode)
                    .Where(x => x.b.CUST_CODE == custCode)
                    .Where(x => wmsNos.Contains(x.b.WMS_NO))
                    .Select(x => x.b).ToList();
        }
        public string GetNearestTakeTime(string dcCode, string gupCode, string custCode)
        {
            return _db.F700101s
                .Join(_db.F700102s, a => new { a.DC_CODE, a.DISTR_CAR_NO }, b => new { b.DC_CODE, b.DISTR_CAR_NO }, (a, b) => new { a, b })
                .Where(x => x.a.STATUS   != "9")
                .Where(x => x.b.ORD_TYPE == "O")
                .Where(x => x.b.GUP_CODE == gupCode)
                .Where(x => x.b.CUST_CODE== custCode)
                .Where(x => x.b.DC_CODE  == dcCode)
                .Where(x => (x.a.TAKE_DATE >= DateTime.Today && x.a.TAKE_DATE < DateTime.Today.AddDays(1)))
                .Where(x => string.Compare(x.b.TAKE_TIME,DateTime.Now.ToString("HHmm"))>0)
                .Select(x=>x.b.TAKE_TIME)
                .SingleOrDefault();
        }
        public IQueryable<F700102DirstCarNo> GetF700102CarNo(string serialNo, string dcCode, string gupCode, string custCode)
        {
            return _db.F700102s
                .Where(x => x.WMS_NO == serialNo)
                .Select(x => new F700102DirstCarNo
                {
                    DISTR_CAR_NO = x.DISTR_CAR_NO,
                    DC_CODE = x.DC_CODE,
                })
                .Distinct();
        }

        public IQueryable<F700102> GetF700102(string dcCode, string distrCarNo)
        {
            return _db.F700102s
                .Where(x => x.DC_CODE == dcCode)
                .Where(x => x.DISTR_CAR_NO == distrCarNo)
                .Select(x => x);
        }
    }
}