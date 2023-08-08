
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710403Service
	{
		private WmsTransaction _wmsTransaction;
		public P710403Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<DcWmsNoStatusItem> GetReceProcessOver30MinDatasByDc(string dcCode, DateTime receDate)
		{
			var f02020101Repo = new F02020101Repository(Schemas.CoreSchema);
			return f02020101Repo.GetReceProcessOver30MinDatasByDc(dcCode, receDate);
		}

		public IQueryable<DcWmsNoStatusItem> GetReceUnUpLocOver30MinDatasByDc(string dcCode, DateTime receDate)
		{
			var f020201Repo = new F020201Repository(Schemas.CoreSchema);
			return f020201Repo.GetReceUnUpLocOver30MinDatasByDc(dcCode, receDate);
		}

		public IQueryable<DcWmsNoStatusItem> GetReturnProcessOver30MinByDc(string dcCode, DateTime returnDate)
		{
			var f161401Repo = new F161401Repository(Schemas.CoreSchema);
			return f161401Repo.GetReturnProcessOver30MinByDc(dcCode, returnDate);
		}

		public IQueryable<DcWmsNoStatusItem> GetReturnWaitUpLocOver30MinByDc(string dcCode, DateTime rtnApplyDate)
		{
			var f161601Repo = new F161601Repository(Schemas.CoreSchema);
			return f161601Repo.GetReturnWaitUpLocOver30MinByDc(dcCode, rtnApplyDate);

		}

		public IQueryable<DcWmsNoStatusItem> GetReturnNoHelpByDc(string dcCode, DateTime returnDate)
		{
			var f0010Repo = new F0010Repository(Schemas.CoreSchema);
			return f0010Repo.GetReturnNoHelpByDc(dcCode, returnDate);
		}

		public IQueryable<ProduceLineStatusItem> GetProduceLineStatusItems(string dcCode, DateTime finishDate)
		{
			var f910201Repo = new F910201Repository(Schemas.CoreSchema);
			return f910201Repo.GetProduceLineStatusItems(dcCode, finishDate);
		}

		public IQueryable<DcWmsNoStatusItem> GetWorkProcessOverFinishTimeByDc(string dcCode, DateTime finishDate)
		{
			var f910201Repo = new F910201Repository(Schemas.CoreSchema);
			return f910201Repo.GetWorkProcessOverFinishTimeByDc(dcCode, finishDate);
		}

        public IQueryable<F020201Data> GetDatasByWaitOrUpLoc(string dcCode, string receDate)
        {
            var f020201Repo = new F020201Repository(Schemas.CoreSchema);
            return f020201Repo.GetDatasByWaitOrUpLoc(dcCode, DateTime.Parse(receDate));
        }
    }
}

