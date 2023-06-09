using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Shared.Services
{
	/// <summary>
	/// HCT 新竹貨運
	/// </summary>
	public partial class ConsignService
	{
		/// <summary>
		/// 取得到著站簡稱(HTTPGET)
		/// </summary>
		private string _getStationGetUrlFormat = "http://is1fax.hct.com.tw/Webedi_Erstno/Addr_Compare.aspx?USER={0}&GROUP=1&ADDR={1}";
		/// <summary>
		/// 取得到著站簡稱方法名稱
		/// </summary>
		private string _getStationPostAction = "addrCompare";
		/// <summary>
		///  取得到著站簡稱 POST XML格式
		/// </summary>
		private string _getStationPostXmlFormat = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
											<soap:Envelope xmlns:xsi =""http://www.w3.org/2001/XMLSchema-instance"" 
                                     xmlns:xsd = ""http://www.w3.org/2001/XMLSchema""
                                     xmlns:soap =""http://schemas.xmlsoap.org/soap/envelope/"">
                      <soap:Body>
                      <addrCompare xmlns =""http://tempuri.org/"">
                      <sUser>{0}</sUser>
											<sAddr>{1}</sAddr>
                      <sGroup>1</sGroup>
                      </addrCompare>
                      </soap:Body>
			                </soap:Envelope>";

		/// <summary>
		/// 取得HCT到著站資訊
		/// </summary>
		/// <param name="custId">客戶代號</param>
		/// <param name="address">地址</param>
		/// <returns></returns>
		public HctStation GetHctStation(string custId, string address)
		{
			var hctStation = new HctStation();
			var doc = new XmlDocument();
			doc.LoadXml(string.Format(_getStationPostXmlFormat, custId, address));
			var returnData = GetData(_getStationPostAction, doc.OuterXml);
			var docReturn = new XmlDocument();
			docReturn.LoadXml(returnData);
			var manager = new XmlNamespaceManager(doc.NameTable);
			manager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
			var xmlNode = docReturn.DocumentElement.SelectSingleNode("/soap:Envelope/soap:Body", manager);
			if(xmlNode != null)
			{
				var xmlNode1 = xmlNode["addrCompareResponse"]?["addrCompareResult"]?["Info"];
				if(xmlNode1!=null)
				{
					hctStation.PutData_m = xmlNode1["PutData_m"]?.InnerText;
					hctStation.PutData_s = xmlNode1["PutData_s"]?.InnerText;
					hctStation.PutDataName = xmlNode1["PutDataName"]?.InnerText;
					hctStation.PutDataZip = xmlNode1["PutDataZip"]?.InnerText;
					hctStation.PutAo_Flag = xmlNode1["PutAo_Flag"]?.InnerText;
				}
			}
			return hctStation;
		}

		/// <summary>
		/// 取得資料
		/// </summary>
		/// <param name="method">服務方法</param>
		/// <param name="postData">傳送xml資料</param>
		/// <returns></returns>
		private string GetData(string method, string postData)
		{
			var Url = ConfigurationManager.AppSettings["HctUrl"];
			byte[] bs = Encoding.UTF8.GetBytes(postData);
			var request = HttpWebRequest.Create(string.Format("{0}?{1}", Url, method)) as HttpWebRequest;
			request.Method = "POST";
			request.ContentType = "text/xml; charset=utf-8";
			request.ContentLength = bs.Length;
			request.KeepAlive = false; //是否保持連線
																 //request.Timeout = 300000;

			using (var reqStream = request.GetRequestStream())
			{
				reqStream.Write(bs, 0, bs.Length);
			}

			var result = "";

			using (var response = request.GetResponse() as HttpWebResponse)
			{
				using (var sr = new StreamReader(response.GetResponseStream()))
				{
					result = sr.ReadToEnd();
					sr.Close();
				}
				response.Close();
			}
			return result;
		}
		/// <summary>
		/// 取得HCT託運單回檔
		/// </summary>
		/// <param name="hctShipReturnParam"></param>
		/// <returns></returns>
		public List<HctShipReturn> GetHctShipReturns(HctShipReturnParam hctShipReturnParam)
		{
			var repo = new F050901Repository(Schemas.CoreSchema);
			//有單派車 正物流(DISTR_USE = 01)
			var datas = new List<HctShipReturn>();
			if (string.IsNullOrWhiteSpace(hctShipReturnParam.DistrUse) || hctShipReturnParam.DistrUse == "01")
				datas.AddRange(repo.GetHctShipReturns(hctShipReturnParam).ToList());

			foreach(var item in datas)
			{
				var station = GetHctStation(item.CONTRACT_CUST_NO, item.RECEIVER_ADDRESS);
				item.ARRIVAL_CODE = station?.PutData_m;
				item.RECEIVER_ZIP_CODE = station?.PutDataZip;
			}
			return datas;
		}

		public ExecuteResult UpdateStatusForHCT(List<HctShipReturn> datas)
		{
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			f050901Repo.BulkUpdateDistrEdiStatus(datas);
			f700101Repo.BulkUpdatStatus(datas);
			return new ExecuteResult { IsSuccessed = true };
		}

		/// <summary>
		/// 取得客代主檔設定
		/// </summary>
		/// <returns></returns>
		public IQueryable<F194715> GetHctCustomerSetting(string customerId = null)
		{
			var f194715Repo = new F194715Repository(Schemas.CoreSchema);
			return f194715Repo.GetSettings("HCT",customerId);
		}
	}
}
