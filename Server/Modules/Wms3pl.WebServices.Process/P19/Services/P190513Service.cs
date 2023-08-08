using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
    public partial class P190513Service
    {
        private WmsTransaction _wmsTransaction;
        public P190513Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        /// <summary>
        /// 刪除登入紀錄
        /// </summary>
        /// <param name="f0070LD"></param>
        /// <returns></returns>
        public ExecuteResult F0070LoginDatasDelete(List<F0070LoginData> f0070LD)
        {
            var repoF0070 = new F0070Repository(Schemas.CoreSchema, _wmsTransaction);
            int total = f0070LD.Count;
            int successCount = 0;
            int faleCount = 0;
            string deleteName = "";
            if (f0070LD.Any())
            {
                foreach (var item in f0070LD)
                {
                    try
                    {
                        deleteName = item.USERNAME;
                        repoF0070.Delete(o => o.CONNECTID == item.CONNECTID);
                        successCount++;
                    }
                    catch (Exception)
                    {
                        return new ExecuteResult() { IsSuccessed = false, Message = string.Format(Properties.Resources.P1905130000_DeleteErr, deleteName) };
                    }
                    
                }
                faleCount = total - successCount;
                return new ExecuteResult() { IsSuccessed = true, Message = string.Format(Properties.Resources.P1905130000_SucessDeleteData, total, successCount, faleCount) };
            }
            else
                return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P1905130000_DeleteNoData };
        }
    }
}
