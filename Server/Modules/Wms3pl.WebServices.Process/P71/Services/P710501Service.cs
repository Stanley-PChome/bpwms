using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
    public partial class P710501Service
    {
        private WmsTransaction _wmsTransaction;

        public P710501Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        #region GetF199001Exs
        public IQueryable<F199001Ex> GetF199001Exs(string dcCode, string locTypeID, string tmprType, string status)
        {
            var f199001Repo = new F199001Repository(Schemas.CoreSchema);
            return f199001Repo.GetF199001Exs(dcCode, locTypeID, tmprType, status);
        }
        #endregion

        public ExecuteResult UpdateF199001(F199001 f199001)
        {
            var result = new ExecuteResult { IsSuccessed = false };

            var f199001Repo = new F199001Repository(Schemas.CoreSchema, _wmsTransaction);
						var f199001Data2 = f199001Repo.GetF199001SameAccItemName(f199001.DC_CODE, f199001.ACC_ITEM_NAME).FirstOrDefault();
						if (f199001Data2 != null &&
							!(f199001Data2.DC_CODE == f199001.DC_CODE && f199001Data2.LOC_TYPE_ID == f199001.LOC_TYPE_ID &&
								f199001Data2.TMPR_TYPE == f199001.TMPR_TYPE && f199001Data2.ACC_UNIT == f199001.ACC_UNIT &&
								f199001Data2.ACC_ITEM_KIND_ID == f199001.ACC_ITEM_KIND_ID))
						{
							result.IsSuccessed = false;
							result.Message = "更新失敗,同一物流中心不能有相同的計價設定名稱";
							return result;
						}

            F199001 f199001Data = f199001Repo.Find(x => x.DC_CODE == f199001.DC_CODE && x.LOC_TYPE_ID == f199001.LOC_TYPE_ID &&
             x.TMPR_TYPE == f199001.TMPR_TYPE && x.ACC_UNIT == f199001.ACC_UNIT && x.ACC_ITEM_KIND_ID == f199001.ACC_ITEM_KIND_ID);
           if (f199001Data != null)
           {
						 f199001.ACC_ITEM_NAME = f199001.ACC_ITEM_NAME;
	           f199001Data.ACC_NUM = f199001.ACC_NUM;
             f199001Data.UNIT_FEE = f199001.UNIT_FEE;
             f199001Data.IN_TAX = f199001.IN_TAX;
             f199001Repo.Update(f199001Data);
             result.IsSuccessed = true;
           }
           else
            {
                result.IsSuccessed = false;
            }
            return result;
        }

        public ExecuteResult InsertF199001(F199001 f199001)
        {
            var result = new ExecuteResult { IsSuccessed = false };

            var f199001Repo = new F199001Repository(Schemas.CoreSchema, _wmsTransaction);
            F199001 f199001Data = f199001Repo.Find(x => x.DC_CODE == f199001.DC_CODE && x.LOC_TYPE_ID == f199001.LOC_TYPE_ID &&
                x.TMPR_TYPE == f199001.TMPR_TYPE && x.ACC_UNIT == f199001.ACC_UNIT && x.ACC_ITEM_KIND_ID == f199001.ACC_ITEM_KIND_ID );
            if (f199001Data == null)
            {
                f199001Data = f199001Repo.GetF199001SameAccItemName(f199001.DC_CODE, f199001.ACC_ITEM_NAME).FirstOrDefault();
                if (f199001Data != null)
                {
                    result.IsSuccessed = false;
                    result.Message = "新增失敗,同一物流中心不能有相同的計價設定名稱";
                }
                else
                {
                    f199001Repo.Add(f199001);
                    result.IsSuccessed = true;
                }
            }
            else
            {
                    result.IsSuccessed = false;
                    result.Message = "新增失敗,資料(PK)重複";
            }

            return result;
            
        }

        public ExecuteResult DeleteF199001(F199001 f199001)
        {
            var result = new ExecuteResult { IsSuccessed = true };

            var f199001Repo = new F199001Repository(Schemas.CoreSchema, _wmsTransaction);
            F199001 f199001Data = f199001Repo.Find(x => x.DC_CODE == f199001.DC_CODE && x.LOC_TYPE_ID == f199001.LOC_TYPE_ID &&
              x.TMPR_TYPE == f199001.TMPR_TYPE && x.ACC_UNIT == f199001.ACC_UNIT && x.ACC_ITEM_KIND_ID == f199001.ACC_ITEM_KIND_ID );
           if (f199001Data != null)
            {
                f199001Data.STATUS = "9";
                f199001Data.UPD_STAFF = f199001.UPD_STAFF;
                f199001Data.UPD_DATE = f199001.UPD_DATE;
                f199001Data.UPD_NAME = f199001.UPD_NAME;
                f199001Repo.Update(f199001Data);
                result.IsSuccessed = true;
            }
           else
            {
                result.IsSuccessed = false;
            }
            return result;
        }

    }
}
