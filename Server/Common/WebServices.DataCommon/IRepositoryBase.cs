using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.DataCommon
{
	public interface IRepositoryBase<T> where T : class
	{

		T Find(Expression<Func<T, bool>> keyCondition, bool isForUpdate = true, bool isByCache = true);

		void Update(T entity, bool isDefaultColumnModify = false,bool isModifyKeyColumn = false, List<string> exceptColumns = null);
	}
}
