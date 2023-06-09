using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F20;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P20.Services
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

	public partial class P20WcfService
	{
		#region 範例用，以後移除

		[OperationContract]
		public IQueryable<ExecuteResult> GetF1929WithF1909Tests(string gupCode)
		{
			return new List<ExecuteResult>().AsQueryable();
		}
		#endregion 範例用，以後移除

		#region P2001010000 異動調整作業(訂單,商品,盤點庫存)

		[OperationContract]
		public ExecuteResult InsertP200101ByAdjustType0(F050301Data[] f050301Datas,
			string workType,
			string allId, string allTime, string address, string newDcCode,
			string cause, string causeMemo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P200101Service(wmsTransaction);
			var result = srv.BeforeInsertP200101ByAdjustType0Check(f050301Datas.ToList(), workType);
			if (result.IsSuccessed)
			{
				result = srv.InsertP200101ByAdjustType0(f050301Datas.ToList(), workType, allId, allTime, address,
					newDcCode, cause, causeMemo);
				if(result.IsSuccessed)
					wmsTransaction.Complete();
			}
			return result;
		}

		[OperationContract]
		public ExecuteResult InsertP200101ByAdjustType1(F1913Data[] f1913Datas, KeyValuePair<int,SerialNoResult[]>[] serialNoResults)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P200101Service(wmsTransaction);
			var result = srv.InsertP200101ByAdjustType1(f1913Datas.ToList(), serialNoResults);
			if (result.IsSuccessed)
					wmsTransaction.Complete();
			return result;
		}

        [OperationContract]
        public ImportF1913DataResult ImportF1913DataItems(string dcCode, string gupCode, string custCode,List<F1913DataImport> importF1913Datas)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P200101Service(wmsTransaction);
            return srv.ImportF1913DataItems(dcCode, gupCode, custCode, importF1913Datas);
        }
        #endregion
    }
}
