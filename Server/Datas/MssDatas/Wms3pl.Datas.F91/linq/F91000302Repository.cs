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

namespace Wms3pl.Datas.F91
{
    public partial class F91000302Repository : RepositoryBase<F91000302, Wms3plDbContext, F91000302Repository>
    {
        public F91000302Repository(string connName, WmsTransaction wmsTransaction = null)
      : base(connName, wmsTransaction)
        {
        }
        public IQueryable<F91000302Data> GetAccUnitList(string itemTypeId)
        {
            var q = (from a in _db.F91000302s
                    where a.ITEM_TYPE_ID == itemTypeId
                    select new F91000302Data
                    {
                        ACC_UNIT = a.ACC_UNIT,
                        ACC_UNIT_NAME = a.ACC_UNIT_NAME
                    }).Distinct();
            

            return q;
        }

        public IQueryable<F91000302SearchData> GetF91000302Data(string itemTypeId, string accUnit, string accUnitName)
        {
            var q = from a in _db.F91000302s
                    join b in _db.F910003s on a.ITEM_TYPE_ID equals b.ITEM_TYPE_ID
                    select new { a, b.ITEM_TYPE };
            
            if (!string.IsNullOrWhiteSpace(itemTypeId))
            {
                q = q.Where(c => c.a.ITEM_TYPE_ID == itemTypeId);               
            }
            if (!string.IsNullOrWhiteSpace(accUnit))
            {
                q = q.Where(c => c.a.ACC_UNIT == accUnit);
            }
            if (!string.IsNullOrWhiteSpace(accUnitName))
            {
                q = q.Where(c => c.a.ACC_UNIT_NAME.Contains(accUnitName));
            }
            var result = q.AsEnumerable().Select((c, index) => new F91000302SearchData
            {
                ROWNUM = index + 1,
                ITEM_TYPE_ID = c.a.ITEM_TYPE_ID,
                ACC_UNIT = c.a.ACC_UNIT,
                ACC_UNIT_NAME = c.a.ACC_UNIT_NAME,
                CRT_STAFF = c.a.CRT_STAFF,
                CRT_NAME = c.a.CRT_NAME,
                CRT_DATE = c.a.CRT_DATE,
                UPD_STAFF = c.a.UPD_STAFF,
                UPD_NAME = c.a.UPD_NAME,
                UPD_DATE = c.a.UPD_DATE,
                ITEM_TYPE = c.ITEM_TYPE
            });
            return result.AsQueryable();
        }

        /// <summary>
        /// 取得商品單位名稱
        /// </summary>
        /// <param name="itemUnit"></param>
        /// <returns></returns>
        public string GetItemUnit(string itemUnit)
        {
            var result = _db.F91000302s.AsNoTracking().Where(x => x.ITEM_TYPE_ID == "001"
                                                             && x.ACC_UNIT == itemUnit)
                                                      .Select(x=>x.ACC_UNIT_NAME);

            return result.FirstOrDefault();
        }

        public IQueryable<F91000302> GetDatas(string itemTypeId, List<string> accUnits)
        {
            return _db.F91000302s.AsNoTracking().Where(x => x.ITEM_TYPE_ID == itemTypeId && 
                                                            accUnits.Contains(x.ACC_UNIT));
        }

        public IQueryable<F91000302> GetDatasByItemTypeId(string itemTypeId)
        {
            return _db.F91000302s.AsNoTracking().Where(x => x.ITEM_TYPE_ID == itemTypeId);
        }

        public IQueryable<F91000302> GetDatasByItemTypeIds(List<string> itemTypeIds, List<string> accUnitIds)
        {
            return _db.F91000302s.AsNoTracking().Where(x => itemTypeIds.Contains(x.ITEM_TYPE_ID) &&
																														accUnitIds.Contains(x.ACC_UNIT));
        }
    }
}
