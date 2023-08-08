
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050201Service
	{
		private WmsTransaction _wmsTransaction;
		public P050201Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}
		public IQueryable<F0513WithF1909> GetF0513WithF1909Datas(string dcCode, string gupCode, string custCode, DateTime delvDate, string delvTime, string status)
		{
			var f0513Repo = new F0513Repository(Schemas.CoreSchema);
			return f0513Repo.GetF0513WithF1909Datas(dcCode, gupCode, custCode, delvDate, delvTime, status);
		}

		public ExecuteResult UpdatePierCode(string dcCode, string gupCode, string custCode, string delvDate,
			string pickTime, string allId, string takeTime, string pierCode)
		{
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var items = f700101Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, DateTime.Parse(delvDate), pickTime, allId, takeTime);
			foreach (var f700101 in items)
			{
				f700101.PIER_CODE = pierCode;
				f700101Repo.Update(f700101);
			}
			return new ExecuteResult{IsSuccessed = true,Message = ""};
		}
		
	}
}

