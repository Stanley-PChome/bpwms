using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F0003Repository : RepositoryBase<F0003, Wms3plDbContext, F0003Repository>
    {
        public F0003Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F0003Ex> GetF0003(string dcCode, string gupCode, string custCode)
        {
            var query = _db.F0003s
                        .Where(x => x.DC_CODE == dcCode
                        && x.GUP_CODE == gupCode
                        && x.CUST_CODE == custCode)
                        .Select((x) => new F0003Ex() {
                            AP_NAME = x.AP_NAME,
                            CRT_DATE = x.CRT_DATE,
                            CRT_NAME = x.CRT_NAME,
                            CRT_STAFF = x.CRT_STAFF,
                            CUST_CODE = x.CUST_CODE,
                            DC_CODE = x.DC_CODE,
                            DESCRIPT = x.DESCRIPT,
                            FILENAME = x.FILENAME,
                            FILETYPE = x.FILETYPE,
                            GUP_CODE = x.GUP_CODE,
                            SYS_PATH = x.SYS_PATH,
                            UPD_DATE = x.UPD_DATE,
                            UPD_STAFF = x.UPD_STAFF,
                            UPD_NAME = x.UPD_NAME,
                        }).OrderBy(x => x.CRT_DATE).ThenBy(x => x.AP_NAME).ToList();

            foreach (var item in query.Select((val,idx)=>new { idx, val}))
            {
                item.val.ROWNUM = item.idx + 1;
                item.val.DisplayText = item.val.SYS_PATH;
                if (item.val.AP_NAME == "PackageLockPW" || item.val.AP_NAME == "DefaultPassword")
                {
                    item.val.DisplayText = "******";
                    item.val.IsPassword = true;
                }
            }

            return query.AsQueryable();
        }

        /// <summary>
        /// 取得APK版本號
        /// </summary>
        /// <returns></returns>
        public string GetVersionNo()
        {
            var result = _db.F0003s.AsNoTracking().Where(x => x.CUST_CODE == "00" 
                                                      && x.GUP_CODE == "00" 
                                                      && x.AP_NAME == "VERSIONNO")
                                                   .Select(x=>x.SYS_PATH).SingleOrDefault();
            return result;
        }

        /// <summary>
        /// 取得ApkUrl
        /// </summary>
        /// <returns></returns>
        public string GetApkUrl()
        {
            var result = _db.F0003s.AsNoTracking().Where(x => x.CUST_CODE == "00"
                                                      && x.GUP_CODE == "00"
                                                      && x.AP_NAME == "APKURL")
                                                      .Select(x=>x.SYS_PATH).SingleOrDefault();
            return result;
        }

	}
}

