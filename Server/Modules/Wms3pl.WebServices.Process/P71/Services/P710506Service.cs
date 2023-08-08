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
    public partial class P710506Service 
    {
        private WmsTransaction _wmsTransaction;

        public P710506Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        public ExecuteResult UpdateF199006(F199006 f199006)
        {
            var result = new ExecuteResult { IsSuccessed = false };

            var f199006Repo = new F199006Repository(Schemas.CoreSchema, _wmsTransaction);
					  F199006 f199006Data = f199006Repo.Find(x => x.DC_CODE == f199006.DC_CODE && x.ACC_ITEM_NAME == f199006.ACC_ITEM_NAME &&
             x.ACC_UNIT == f199006.ACC_UNIT && x.DELV_ACC_TYPE == f199006.DELV_ACC_TYPE );
            if (f199006Data != null)
            {
	              f199006.ACC_NUM = f199006.ACC_NUM;
                f199006Data.FEE = f199006.FEE;
                f199006Data.IN_TAX = f199006.IN_TAX;
                f199006Repo.Update(f199006Data);
                result.IsSuccessed = true;
           }
           else
            {
                result.IsSuccessed = false;
            }
            return result;
        }

        public ExecuteResult InsertF199006(F199006 f199006)
        {
            var result = new ExecuteResult { IsSuccessed = false };

            var f199006Repo = new F199006Repository(Schemas.CoreSchema, _wmsTransaction);
						F199006 f199006Data = f199006Repo.Find(x => x.DC_CODE == f199006.DC_CODE && x.ACC_ITEM_NAME == f199006.ACC_ITEM_NAME  && 
             x.ACC_UNIT == f199006.ACC_UNIT && x.DELV_ACC_TYPE == f199006.DELV_ACC_TYPE );
            if (f199006Data == null)
            {
		            f199006Repo.Add(f199006);
		            result.IsSuccessed = true;
            }
						else
						{
							result.IsSuccessed = false;
							result.Message = "新增失敗,資料(PK)重複";
						}

            return result;
            
        }

        public ExecuteResult DeleteF199006(F199006 f199006)
        {
            var result = new ExecuteResult { IsSuccessed = true };

            var f199006Repo = new F199006Repository(Schemas.CoreSchema, _wmsTransaction);
            F199006 f199006Data = f199006Repo.Find(x => x.DC_CODE == f199006.DC_CODE && x.ACC_ITEM_NAME == f199006.ACC_ITEM_NAME &&
             x.ACC_UNIT == f199006.ACC_UNIT && x.DELV_ACC_TYPE == f199006.DELV_ACC_TYPE);
            if (f199006Data != null)
            {
                f199006Data.STATUS = "9";
                f199006Repo.Update(f199006Data);
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
