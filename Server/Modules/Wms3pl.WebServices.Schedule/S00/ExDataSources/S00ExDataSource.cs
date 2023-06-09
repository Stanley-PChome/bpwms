using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Schedule.S00.ExDataSources
{
	public partial class S00ExDataSource
	{
		public IQueryable<ContractSettleData> ContractSettleDatas
		{
			get { return new List<ContractSettleData>().AsQueryable(); }
		}
	}
}
