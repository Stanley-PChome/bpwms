using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.DataCommon
{
	public class CahceRepository<T> where T : class
	{
		private IRepositoryBase<T> _repository;
		private IList<T> _entityCaches = new List<T>();

		public CahceRepository(IRepositoryBase<T> repository)
		{
			this._repository = repository;
		}

		public T Find(Expression<Func<T, bool>> keyCondition, bool isForUpdate = true, bool isByCache = true)
		{
			var entity = _entityCaches.SingleOrDefault(keyCondition.Compile());

			if (entity == null)
			{
				entity = _repository.Find(keyCondition, isForUpdate);
				if (entity != null)
				{
					_entityCaches.Add(entity);
				}
			}

			return entity;
		}

		public void Update(T entity)
		{
			// 若是 Update 從快取取得的 Entity，則移除掉，避免下次 Find 又找到已經 Update 過的。
			_entityCaches.Remove(entity);
			_repository.Update(entity);
		}
	}
}
