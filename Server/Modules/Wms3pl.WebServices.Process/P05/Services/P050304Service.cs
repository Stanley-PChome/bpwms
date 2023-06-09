
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Common.Collections;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Transaction.T05.Services;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050304Service
	{
		private WmsTransaction _wmsTransaction;
		public P050304Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

    public IQueryable<F050001Data> GetF050001Datas(string dcCode, string gupCode, string custCode, string ordType,
      string ordSDate, string ordEDate, string arrivalSDate, string arrivalEDate, string ordNo, string custOrdNo,
      string consignee, string itemCode, string itemName, string sourceType, string retailCode, string carPeriod,
      string delvNo, string custCost, string fastDealType, string crossCode, string channel, string subChannel)
    {
      var f050001Repo = new F050001Repository(Schemas.CoreSchema);
      return f050001Repo.GetF050001Datas(dcCode, gupCode, custCode, ordType, ordSDate, ordEDate, arrivalSDate,
        arrivalEDate, ordNo, custOrdNo, consignee, itemCode, itemName, sourceType, retailCode, carPeriod, delvNo,
        custCost, fastDealType, crossCode, channel, subChannel);
    }

    #region 配庫試算
    /// <summary>
    /// 配庫試算
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="ordNos"></param>
    /// <returns></returns>
    public ExecuteResult AllotStockTrialCalculation(string dcCode,string gupCode,string custCode,List<string> ordNos)
		{
			var stockService = new StockService();
			if (stockService.GetAllotStockMode() == "0")
			{
				var service = new T050101Service(_wmsTransaction);
				return service.AllotStockTrialCalculation(dcCode, gupCode, custCode, ordNos);
			}
			else
			{
				var service = new T050102Service(_wmsTransaction);
				return service.AllotStockTrialCalculation(dcCode, gupCode, custCode, ordNos);
			}
			
		}

		public List<F050805Data> GetF050805Datas(string dcCode, string gupCode, string custCode, string calNo)
		{
			var f050805Repo = new F050805Repository(Schemas.CoreSchema);
			var data = f050805Repo.GetF050805Datas(dcCode, gupCode, custCode, calNo).ToList();
			return data;
		}
		public List<F05080501Data> GetF05080501Datas(string dcCode, string gupCode, string custCode, string calNo)
		{
			var f05080501Repo = new F05080501Repository(Schemas.CoreSchema);
			var data = f05080501Repo.GetF05080501Datas(dcCode, gupCode, custCode, calNo).ToList();
			return data;
		}

		public List<F05080502Data> GetF05080502Datas(string dcCode, string gupCode, string custCode, string calNo)
		{
			var f05080502Repo = new F05080502Repository(Schemas.CoreSchema);
			var data = f05080502Repo.GetF05080502Datas(dcCode, gupCode, custCode, calNo).ToList();
			return data;
		}

		public List<F05080504Data> GetF05080504Datas(string dcCode,string gupCode,string custCode,string calNo)
		{
			var f05080504Repo = new F05080504Repository(Schemas.CoreSchema);
			var data = f05080504Repo.GetF05080504Datas(dcCode, gupCode, custCode, calNo).ToList();
			return data;
		}
    public List<F05080505Data> GetF05080505Datas(string dcCode, string gupCode, string custCode, string calNo, string flag)
    {
      var f05080505Repo = new F05080505Repository(Schemas.CoreSchema);
      var data = f05080505Repo.GetF05080505Datas(dcCode, gupCode, custCode, calNo, flag).ToList();
      return data;
    }

    public List<F05080506Data> GetF05080506Datas(string dcCode, string gupCode, string custCode, string calNo, string flag)
    {
      var f05080506Repo = new F05080506Repository(Schemas.CoreSchema);
      var data = f05080506Repo.GetF05080506Datas(dcCode, gupCode, custCode, calNo, flag).ToList();
      return data;

    }

    /// <summary>
    /// 建立補貨調撥單
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="calNo"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    public ExecuteResult CreateAllocation(string dcCode, string gupCode, string custCode,string calNo, List<decimal> ids)
		{
            var _sharedService = new SharedService(_wmsTransaction);
			      var returnStocks = new List<F1913>();
			      var allocationList = new List<ReturnNewAllocation>();

            var erplemishService = new ReplenishService(_wmsTransaction);
            var result = erplemishService.ManualReplenish(dcCode, gupCode, custCode, ids);

            return result;

            /*
            var erplemishService = new ReplenishService(_wmsTransaction);
            var res = erplemishService.CreateReplenishStock(ref returnStocks, ref allocationList, dcCode, gupCode, custCode, ids: ids);
            if (!allocationList.Any())
                return new ExecuteResult(false, Properties.Resources.P050304Service_SelectedItemNoReSupplyStock);

            var result1 = _sharedService.BulkInsertAllocation(allocationList, returnStocks);
            if (!result1.IsSuccessed)
                return result1;
            else
                return new ExecuteResult(true, string.Format(Properties.Resources.P050304Service_CreateReSupplyAllot, Environment.NewLine,
                    string.Join(Environment.NewLine, allocationList.Select(x => x.Master.ALLOCATION_NO).ToList())));
            */
        }


        #endregion

    }
}

