using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.F00.Interfaces
{
	public interface IApiLogRepository<T>
	{
		IQueryable<T> GetData(string name);
    void InsertLog(string dcCode, string gupCode, string custCode, string apiName, string sendData,string returnData,string errMsg,string status,DateTime startTime);
		void UpdateLog(int id, string status, string errMsg, string retrunData);
  }
}
