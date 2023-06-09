using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System.Linq;
namespace Wms3pl.Datas.F19
{
	public partial class F195601Repository : RepositoryBase<F195601, Wms3plDbContext, F195601Repository>
	{
		public IQueryable<F195601> GetAll()
		{
			var sql = @" SELECT *
                    FROM F195601 
                   WHERE 1 = 1";
			return SqlQuery<F195601>(sql);
		}
	}
}
