
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Transaction.T05.Services;

namespace Wms3pl.WebServices.Transaction.T05
{
	/// <summary>
	/// 系統功能
	/// </summary>
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class T05WcfService
	{


		#region 配庫

		[OperationContract]
		public IQueryable<ExecuteResult> AllotStocks(List<string> ordNos,string priorityCode)
		{
			var wmsTransaction = new WmsTransaction();
			var stockService = new StockService();
			if (stockService.GetAllotStockMode() == "0")
			{
				var srv = new T050101Service(wmsTransaction);
				var results = srv.AllotStocks(ordNos);
				wmsTransaction.Complete();
				return results;
			}
			else
			{
				var srv = new T050102Service(wmsTransaction);
				var results =  srv.AllotStocks(ordNos, priorityCode);
        wmsTransaction.Complete();
        return results;
			}
		}


		[OperationContract]
		public void AutoAllotStocks()
		{
			Current.DefaultStaff = "Trans";
			Current.DefaultStaffName = "Trans";
			var wmsTransaction = new WmsTransaction();
			var stockService = new StockService();
			if(stockService.GetAllotStockMode() == "0")
			{
				var srv = new T050101Service(wmsTransaction);
				srv.AllotStocks();
				wmsTransaction.Complete();
			}
			else
			{
				var srv = new T050102Service(wmsTransaction);
				srv.AllotStocks();
        wmsTransaction.Complete();
      }

		}

		#endregion 配庫
	}
}

