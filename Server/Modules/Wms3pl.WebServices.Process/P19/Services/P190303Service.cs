using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	public partial class P190303Service
	{
		private WmsTransaction _wmsTransaction;
		public P190303Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult AddF190303Data(F190303 newData)
		{
			var f190303Rep = new F190303Repository(Schemas.CoreSchema, _wmsTransaction);
			var data = f190303Rep.Find(
				x => x.GUP_CODE == newData.GUP_CODE && x.CUST_CODE == newData.CUST_CODE && x.ITEM_CODE == newData.ITEM_CODE && x.VNR_CODE == newData.VNR_CODE);

			if (data == null)
			{
				data = newData;
				f190303Rep.Add(data);
			}
			else
			{
				data.SOURCE_NO = newData.SOURCE_NO;
				f190303Rep.Update(data);
			}
			return new ExecuteResult(true);
		}

		Func<F190303, string, string, string, string, bool> F190303Func = FindF190303;
		private static bool FindF190303(F190303 f190303,string gupCode,string custCode,string itemCode,string vnrCode)
		{
			return f190303.GUP_CODE == gupCode && f190303.CUST_CODE == custCode && f190303.ITEM_CODE == itemCode && f190303.VNR_CODE == vnrCode ;
		}
		public ExecuteResult AddorUpdateF190303Data(List<F190303> datas)
		{
			var addF190303List = new List<F190303>();
			var updF190303List = new List<F190303>();
			var f190303Repo = new F190303Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach(var item in datas)
			{
				bool isDbUpdate = false;
				var f190303 = addF190303List.Find(x => F190303Func(x, item.GUP_CODE, item.CUST_CODE, item.ITEM_CODE, item.VNR_CODE));
				if(f190303 == null)
			     f190303 = updF190303List.Find(x => F190303Func(x, item.GUP_CODE, item.CUST_CODE, item.ITEM_CODE, item.VNR_CODE));
				if (f190303 == null)
				{
					f190303 = f190303Repo.Find(x =>x.GUP_CODE == item.GUP_CODE && x.CUST_CODE == item.CUST_CODE && x.ITEM_CODE == item.ITEM_CODE && x.VNR_CODE == item.VNR_CODE);
					isDbUpdate = true;
				}
				if (f190303 == null)
					addF190303List.Add(item);
				else
				{
					f190303.SOURCE_NO = item.SOURCE_NO;
					if(isDbUpdate)
						updF190303List.Add(f190303);
				}
			}
			f190303Repo.BulkInsert(addF190303List);
			f190303Repo.BulkUpdate(updF190303List);
			return new ExecuteResult(true);
		}


	}
}
