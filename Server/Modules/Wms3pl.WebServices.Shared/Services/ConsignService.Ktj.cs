using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Shared.Services
{
	public partial class ConsignService
	{
		public KtjStation GetKtjStation(string address)
		{
			var ktjStation = new KtjStation();
			var ktjPostData = new KtjPostData { ID = "1", Address = address};

			var returnData = GetKtjData(new List<KtjPostData> { ktjPostData });
			return returnData.FirstOrDefault();
		}

		public List<KtjStation> GetKtjData(List<KtjPostData> postData)
		{
			var Url = ConfigurationManager.AppSettings["KtjUrl"];
			var request = HttpWebRequest.Create(string.Format("{0}", Url)) as HttpWebRequest;
			request.Method = "POST";
			request.ContentType = "application/json";
			request.KeepAlive = false; //是否保持連線
									   //request.Timeout = 300000;
			string serialisedData = JsonConvert.SerializeObject(postData);
			using (var reqStream = new StreamWriter(request.GetRequestStream()))
			{
				reqStream.Write(serialisedData);
			}

			var result = "";

			var httpResponse = (HttpWebResponse)request.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				result = streamReader.ReadToEnd();
				streamReader.Close();
			}
			return JsonConvert.DeserializeObject<List<KtjStation>>(result);
		}

		public List<KTJShipReturn> GetKTJShipReturns(HctShipReturnParam hctShipReturnParam)
		{
			var repo = new F050901Repository(Schemas.CoreSchema);
			var datas = repo.GetKTJShipReturns(hctShipReturnParam).ToList();
			var posList = datas.Select(o => new KtjPostData { ID = o.CONSIGN_NO, Address = o.RECEIVER_ADDRESS }).ToList();

			var stations = GetKtjData(posList);
			foreach (var d in datas)
			{
				var station = stations.FirstOrDefault(o=>o.ID == d.CONSIGN_NO);
				d.RECEIVER_ZIP_CODE = station?.Zip;
			}
			return datas;
		}


		/// <summary>
		/// 取得客代主檔設定
		/// </summary>
		/// <returns></returns>
		public IQueryable<F194715> GetKtjCustomerSetting(string customerId = null)
		{
			var f194715Repo = new F194715Repository(Schemas.CoreSchema);
			return f194715Repo.GetSettings("KTJ", customerId);
		}

		public ExecuteResult UpdateStatusForKTJ(List<KTJShipReturn> datas)
		{
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			f050901Repo.BulkUpdateDistrEdiStatus(datas);
			f700101Repo.BulkUpdatStatus(datas);
			return new ExecuteResult { IsSuccessed = true };
		}
	}
}
