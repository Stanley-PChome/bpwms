using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.WebServices.Shared.Services
{
	public partial class ReportService
	{
		private WmsTransaction _wmsTransaction;
		public ReportService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		

		private void InitDate(ref ReportParam param)
		{
			if (param.SDate== null && param.EDate == null) //系統上個月1號~系統日上個月月底
			{
				param.SDate = DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1);
				param.EDate = DateTime.Now.AddDays(-DateTime.Now.Day);
			}
			else if (param.SDate == null && param.EDate != null) //結束當月1號~結束日
				param.SDate = param.EDate.Value.AddDays(-param.EDate.Value.Day + 1);
			else if (param.SDate != null && param.EDate == null) //起始日~起始日當月月底
				param.EDate = param.SDate.Value.AddMonths(1).AddDays(-param.SDate.Value.Day);
			else //起始日~結束日
			{

			}
		}

		private Dictionary<string,string> GetTitieList<T>() 
		{
			var titleList = new Dictionary<string, string>();
			foreach( var attr in typeof(T).GetProperties())
			{
				switch(attr.Name)
				{
					case "ROWNUM":
						titleList.Add(attr.Name, "序號");
						break;
					case "ORDERMONTH":
					case "DELVMONTH":
					case "STOCKMONTH":
						titleList.Add(attr.Name, "年月");
						break;
					case "GUP_NAME":
						titleList.Add(attr.Name, "業主");
						break;
					case "CUST_NAME":
						titleList.Add(attr.Name, "貨主");
						break;
					case "ORDCOUNT":
						titleList.Add(attr.Name, "訂單數");
						break;
					case "ORDCOUNT_CANCEL":
						titleList.Add(attr.Name, "取消訂單數");
						break;
					case "DELVCOUNT":
						titleList.Add(attr.Name, "出貨單數");
						break;
					case "DELVCOUNT_NOCANCEL":
						titleList.Add(attr.Name, "有效出貨單數");
						break;
					case "WORK_DAY":
						titleList.Add(attr.Name, "工作天數");
						break;
					case "AVG_DELVCOUNT":
						titleList.Add(attr.Name, "平均有效出貨單數");
						break;
					case "DELV_TYPE":
						titleList.Add(attr.Name, "配送方式");
						break;
					case "COLLECT_AMT":
						titleList.Add(attr.Name, "代收總金額");
						break;
					case "STOCKCOUNT":
						titleList.Add(attr.Name, "進倉單數(結案)");
						break;
					case "RECV_ITEMCOUNT":
						titleList.Add(attr.Name, "進倉驗收品項數");
						break;
					case "RECV_ITEMQTY":
						titleList.Add(attr.Name, "進倉驗收商品數量");
						break;
				}
			}
			return titleList;
		}


	}
}
