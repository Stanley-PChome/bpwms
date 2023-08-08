using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F15
{
	public partial class F151002Repository : RepositoryBase<F151002, Wms3plDbContext, F151002Repository>
	{
		public F151002Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
       
        public IQueryable<F151002> GetDatasByValidDate(string dcCode, string gupCode, string custCode, string allocationNo, string itemCode, DateTime validDate)
        {
            var result = _db.F151002s.Where(x => x.DC_CODE == dcCode
                                         && x.GUP_CODE == gupCode
                                         && x.CUST_CODE == custCode
                                         && x.ALLOCATION_NO == allocationNo
                                         && x.ITEM_CODE == itemCode
                                         && x.VALID_DATE == validDate);
            return result;
        }

        public IQueryable<F151002> GetDatas(string dcCode, string gupCode, string custCode, string allocationNo,
            string itemCode)
        {
            var result = _db.F151002s.Where(x => x.DC_CODE == dcCode
                                        && x.GUP_CODE == gupCode
                                        && x.CUST_CODE == custCode
                                        && x.ALLOCATION_NO == allocationNo
                                        && x.ITEM_CODE == itemCode);
            return result;
        }


        public IQueryable<F151002> GetDatas(string dcCode, string gupCode, string custCode, List<string> allocationNo)
        {
            var result = _db.F151002s.Where(x => x.DC_CODE == dcCode
                                         && x.GUP_CODE == gupCode
                                         && x.CUST_CODE == custCode
                                         && allocationNo.Contains(x.ALLOCATION_NO));

            return result;
        }

        public IQueryable<F151002> GetDatasNoTracking(string dcCode, string gupCode, string custCode, List<string> allocationNo)
        {
            var result = _db.F151002s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                         && x.GUP_CODE == gupCode
                                         && x.CUST_CODE == custCode
                                         && allocationNo.Contains(x.ALLOCATION_NO));

            return result;
        }

        public int GetNextSeq(string dcCode, string gupCode, string custCode, string allocNo)
        {
            var result = _db.F151002s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                                           && x.GUP_CODE == gupCode
                                                           && x.CUST_CODE == custCode
                                                           && x.ALLOCATION_NO == allocNo)
                                                        .Max(x => x.ALLOCATION_SEQ);

            return result + 1;
        }

        public IQueryable<F151002> GetDatasForSeqs(string dcCode, string gupCode, string custCode, string allocationNo, List<short> seqs)
        {
            var result = _db.F151002s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                        && x.GUP_CODE == gupCode
                                        && x.CUST_CODE == custCode
                                        && x.ALLOCATION_NO == allocationNo
                                        && seqs.Contains(x.ALLOCATION_SEQ));
            return result;
        }

        public IQueryable<F151002> GetDatasForOrgSeq(string dcCode, string gupCode, string custCode, string allocationNo, List<short> orgSeq)
        {
            var result = _db.F151002s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                        && x.GUP_CODE == gupCode
                                        && x.CUST_CODE == custCode
                                        && x.ALLOCATION_NO == allocationNo
                                        && orgSeq.Contains(x.ORG_SEQ));
            return result;
        }

        public IQueryable<F151002> GetDataByF051206LackList_Allots(List<F051206LackList_Allot> data)
        {
            var result = _db.F151002s.Where(x => data.Any(z=>z.DC_CODE == x.DC_CODE &&
            z.GUP_CODE == x.GUP_CODE &&
            z.CUST_CODE == x.CUST_CODE &&
            z.ALLOCATION_NO == x.ALLOCATION_NO &&
            z.ALLOCATION_SEQ == x.ALLOCATION_SEQ));
            return result;
        }

        public IQueryable<F151002> GetAllocNoDataByLackSeqs(List<decimal> lackSeqs)
        {
            var f151003s = _db.F151003s.Where(x => lackSeqs.Contains(x.LACK_SEQ));
            var result = _db.F151002s.Where(x => f151003s.Any(z => z.DC_CODE == x.DC_CODE &&
            z.GUP_CODE == x.GUP_CODE &&
            z.CUST_CODE == x.CUST_CODE &&
            z.ALLOCATION_NO == x.ALLOCATION_NO));
            return result;
        }

        public IQueryable<F151002> GetDataByRefNosWithSnLoc(string dcCode, string gupCode, string custCode, List<string> refNos)
        {
            var f151002s = _db.F151002s.AsNoTracking().Where(x =>
            x.DC_CODE == dcCode &&
            x.GUP_CODE == gupCode &&
            x.CUST_CODE == custCode &&
            refNos.Contains(x.REFENCE_NO) &&
            !string.IsNullOrWhiteSpace(x.SERIAL_NO));
            
            var f151001 = _db.F151001s.AsNoTracking().Where(x =>
            x.DC_CODE == dcCode &&
            x.GUP_CODE == gupCode &&
            x.CUST_CODE == custCode &&
            x.ALLOCATION_TYPE == "4" &&
            f151002s.Select(z => z.ALLOCATION_NO).Contains(x.ALLOCATION_NO));

            var result = from A in f151002s
                         join B in f151001
                         on A.ALLOCATION_NO equals B.ALLOCATION_NO
                         select A;

            return result;
        }
    }

}