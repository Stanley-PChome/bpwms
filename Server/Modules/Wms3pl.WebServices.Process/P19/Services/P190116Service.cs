
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
    public partial class P190116Service
    {
        private WmsTransaction _wmsTransaction;
        public P190116Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        public ExecuteResult DeleteP190116(string gupCode, string custCode, string dcCode,string delvNo)
        {
            var f194716Repo = new F194716Repository(Schemas.CoreSchema, _wmsTransaction);
            var f19471601Repo = new F19471601Repository(Schemas.CoreSchema, _wmsTransaction);

            var f194716 = f194716Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.DC_CODE==dcCode && x.DELV_NO==delvNo);

            // 先判斷存不存在該項目, 不存在就回傳該資料已糟刪除
            if (f194716 == null )
            {
                return new ExecuteResult(false, Properties.Resources.P190116Service_DELV_No_Deleted);
            }

            //刪除車次明細
            f19471601Repo.Delete(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.DC_CODE == dcCode && x.DELV_NO == delvNo);
            //刪除車次主檔
            f194716Repo.Delete(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.DC_CODE == dcCode && x.DELV_NO == delvNo);


            return new ExecuteResult(true);
        }

        public ExecuteResult InsertOrUpdateF194716(F194716 data, List<F19471601Data> dtlItems, bool isAdd)
        {
            var f194716Repo = new F194716Repository(Schemas.CoreSchema, _wmsTransaction);
            var f19471601Repo = new F19471601Repository(Schemas.CoreSchema, _wmsTransaction);
            var f000904Rep = new F000904Repository(Schemas.CoreSchema);

            var f194716 = f194716Repo.Find(x => x.GUP_CODE == data.GUP_CODE && x.CUST_CODE == data.CUST_CODE && x.DC_CODE == data.DC_CODE && x.DELV_NO == data.DELV_NO);

            //判斷車次主檔F194716 
            if (isAdd && f194716 != null)
                return new ExecuteResult(false, Properties.Resources.P190116Service_DELV_No_Duplicate);
            if (!isAdd && f194716 == null)
                return new ExecuteResult(false, Properties.Resources.DataDelete);

            //insert/update車次主檔F194716 
            if (isAdd)
                f194716Repo.Add(data);
            else
                f194716Repo.UpdateHasKey(data);

            //車次明細資資料,先刪再新增
            if (dtlItems != null)
            {
                //先刪
                f19471601Repo.Delete(x => x.GUP_CODE == data.GUP_CODE && x.CUST_CODE == data.CUST_CODE && x.DC_CODE == data.DC_CODE && x.DELV_NO == data.DELV_NO);
             
                var f000904s = f000904Rep.GetDatas("P190116", "DELV_WAY").ToList();

                //後新增
                foreach (var item in dtlItems)
                {
                    var newItem = new F19471601();
                    newItem.GUP_CODE = item.GUP_CODE;
                    newItem.CUST_CODE = item.CUST_CODE;
                    newItem.DC_CODE = item.DC_CODE;
                    newItem.DELV_NO = item.DELV_NO;
                    newItem.RETAIL_CODE = item.RETAIL_CODE;
                    newItem.DELV_WAY = f000904s.Where(o => o.NAME == item.DELV_WAY).FirstOrDefault().VALUE;
                    newItem.ARRIVAL_TIME_S = item.ARRIVAL_TIME_S;
                    newItem.ARRIVAL_TIME_E = item.ARRIVAL_TIME_E;
                    newItem.CRT_DATE = DateTime.Now;
                    newItem.CRT_NAME = item.CRT_NAME;
                    newItem.CRT_STAFF = item.CRT_STAFF;
                    f19471601Repo.Add(newItem);
                }
            }

            return new ExecuteResult(true);
        }
    }
}

