using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;

namespace Wms3pl.WebServices.Schedule.WmsSchedule
{
    public partial class DockReceivingService
    {
        private WmsTransaction _wmsTransaction;

        public DockReceivingService(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = new WmsTransaction();
        }

        /// <summary>
        /// 將碼頭收貨作業移轉到歷史紀錄區F010301,F010302=>F010301_HISTORY,F010302_HISTORY
        /// </summary>
        /// <returns></returns>
        public ApiResult DockDataTransferToHistory()
        {
            return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSSH_DPR, "0", "0", "0", "DockDataTransferToHistory", new object { }, () =>
             {
                 var result = DoTransfer();
                 return new ApiResult { IsSuccessed = result.IsSuccessed, MsgCode = "", MsgContent = result.Message };
             }, true);
        }

        private ExecuteResult DoTransfer()
        {
            var F010301s = new F010301Repository(Schemas.CoreSchema, _wmsTransaction);
            var F010301HISTORYs = new F010301_HISTORYRepository(Schemas.CoreSchema, _wmsTransaction);
            var F010302s = new F010302Repository(Schemas.CoreSchema, _wmsTransaction);
            var F010302HISTORYs = new F010302_HISTORYRepository(Schemas.CoreSchema, _wmsTransaction);

            var OldF010301s = F010301s.GetOldF010301Datas().ToList();
            var insertf010301_HISTORY = ConvertEntity<F010301, F010301_HISTORY>(OldF010301s).ToList();
            insertf010301_HISTORY.ForEach(x => F010301HISTORYs.Add(x));
            OldF010301s.ForEach(x => F010301s.Delete(a => a.ID == x.ID));

            var OldF010302s = F010302s.GetOldF010302Datas().ToList();
            var insertf010302_HISTORY = ConvertEntity<F010302, F010302_HISTORY>(OldF010302s).ToList();
            insertf010302_HISTORY.ForEach(x => F010302HISTORYs.Add(x));
            OldF010302s.ForEach(x => F010302s.Delete(a => a.ID == x.ID));

            _wmsTransaction.Complete();
            return new ExecuteResult(true);
        }

        private IEnumerable<TDestination> ConvertEntity<TSource, TDestination>(IEnumerable<TSource> sourceCollection)
        {
            foreach (var source in sourceCollection)
            {
                var destinationInstance = Activator.CreateInstance<TDestination>();
                foreach (var p in typeof(TSource).GetProperties())
                {
                    if (!p.CanWrite)
                        continue;
                    string propertyName = p.Name;
                    var destinationProperty = typeof(TDestination).GetProperty(propertyName);
                    if (destinationProperty != null)
                    {
                        var sourceValue = p.GetValue(source, null);
                        destinationProperty.SetValue(destinationInstance, sourceValue, null);
                    }
                }
                yield return destinationInstance;
            }
        }

    }
}
