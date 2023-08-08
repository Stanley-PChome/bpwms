using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F19
{
    public partial class F197001Repository : RepositoryBase<F197001, Wms3plDbContext, F197001Repository>
    {
        public F197001Repository(string connName, WmsTransaction wmsTransaction = null)
      : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F197001Data> GetF197001Seq(string gupCode, string custCode)
        {
            var result = _db.F197001s.Where(x => x.GUP_CODE == gupCode
                                            && x.CUST_CODE == custCode)
                                            .OrderByDescending(x => x.LABEL_SEQ).Skip(2)
                                            .Select(x=> new F197001Data {
                                                LABEL_SEQ = x.LABEL_SEQ
                                            });
            return result;
        }

        /// <summary>
		/// 檢查是否重複新增 條件 LabelCode / ItemCode / VNRCode
		/// </summary>
		public IQueryable<F197001Data> GetLabelData(F197001 f197001Data)
        {
            var result = _db.F197001s.AsNoTracking().Where(x => x.GUP_CODE == f197001Data.GUP_CODE
                                                        && x.LABEL_CODE == f197001Data.LABEL_CODE
                                                        && x.ITEM_CODE == f197001Data.ITEM_CODE
                                                        && x.VNR_CODE == f197001Data.VNR_CODE)
                                                        .Select(x=>new F197001Data
                                                        {
                                                            LABEL_SEQ = x.LABEL_SEQ,
                                                            LABEL_CODE = x.LABEL_CODE,
                                                            VNR_CODE = x.VNR_CODE,
                                                            ITEM_CODE = x.ITEM_CODE,
                                                            WARRANTY = x.WARRANTY,
                                                            WARRANTY_S_Y = x.WARRANTY_S_Y,
                                                            WARRANTY_S_M = x.WARRANTY_S_M,
                                                            WARRANTY_Y = x.WARRANTY_Y,
                                                            WARRANTY_M = x.WARRANTY_M,
                                                            WARRANTY_D = x.WARRANTY_D,
                                                            OUTSOURCE = x.OUTSOURCE,
                                                            CHECK_STAFF = x.CHECK_STAFF,
                                                            ITEM_DESC_A = x.ITEM_DESC_A,
                                                            ITEM_DESC_B = x.ITEM_DESC_B,
                                                            ITEM_DESC_C = x.ITEM_DESC_C,
                                                            GUP_CODE = x.GUP_CODE,
                                                            CUST_CODE = x.CUST_CODE,
                                                            CRT_DATE = x.CRT_DATE,
                                                            CRT_NAME = x.CRT_NAME,
                                                            UPD_DATE = x.UPD_DATE,
                                                            UPD_NAME = x.UPD_NAME,
                                                        });

        

            return result;
        }
    }
}
