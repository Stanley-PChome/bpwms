using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wms3pl.WpfClient.Common
{
  public interface IRepository<T>
  {
    void All(Action<IEnumerable<T>> action); 
    void FindAll(Func<T, bool> filter, Action<IEnumerable<T>> action);
  }
}
