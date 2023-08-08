using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;
using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.Datas.F05
{
    public partial class F051201Repository : RepositoryBase<F051201, Wms3plDbContext, F051201Repository>
    {
        public F051201Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }


       


        public IQueryable<F051201> GetDatas(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
        {
            var result = _db.F051201s.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 pickOrdNos.Contains(x.PICK_ORD_NO));

            return result;
        }

        public IQueryable<F051201> GetDatas(string dcCode, string gupCode, string custCode, string pickOrdNos)
        {
            var result = _db.F051201s.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 x.PICK_ORD_NO == pickOrdNos);

            return result;
        }


        public IQueryable<F051201> GetDatasNoTracking(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
        {
            var result = _db.F051201s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 pickOrdNos.Contains(x.PICK_ORD_NO));

            return result;
        }

		    public IQueryable<F051201> GetArtificialSinglePickOrders(string dcCode,string gupCode,string custCode,string wmsOrdNo)
				{
					return _db.F051201s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode &&
					x.SPLIT_TYPE =="03" && x.DISP_SYSTEM == "0" && x.SPLIT_CODE == wmsOrdNo);
				}

		public IQueryable<F051201> GetDatasByF0513s(string dcCode, string gupCode, string custCode, List<F0513> f0513s)
		{
			return _db.F051201s.Where(x => x.DISP_SYSTEM == "0" &&
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			f0513s.Any(z => x.DELV_DATE == z.DELV_DATE &&
			x.PICK_TIME == z.PICK_TIME));
		}

		public IQueryable<F051201> GetDatasByChangePickToolDatas(List<ChangePickTool> changePickToolList)
		{
			return _db.F051201s.Where(x => 
			changePickToolList.Any(z => x.DC_CODE == z.DC_CODE &&
			x.GUP_CODE == z.GUP_CODE &&
			x.CUST_CODE == z.CUST_CODE &&
			x.PICK_ORD_NO == z.PICK_ORD_NO &&
			x.PICK_TOOL != z.PICK_TOOL));
		}


  }
}
