using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Schedule;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.Common.Extensions;
namespace Wms3pl.WebServices.Shared.Services
{
	/// <summary>
	/// EGS 統一速達
	/// </summary>
	public partial class ConsignService
	{
		private string _egsInfoCommand = @"cmd=query_egs_info";
		private string _egsSuda5Command = @"cmd=query_suda5&address_1={0}";
		private string _egsStationCommand = @"cmd=query_base&suda5_1={0}";
		private string _egsCustomerInfoCommand = @"cmd=query_customers";
		private string _egsRangeCommand = @"cmd=query_waybill_id_range&customer_id={0}&waybill_type={1}&count={2}";
		private string _egsStockCommand = @"cmd=query_waybill_id_remain&customer_id={0}&waybill_type={1}";
		private string _egsDeliveryTimeZoneCommand = @"cmd=test_delivery_timezone&service_type={0}&suda5_1={1}";
		private string _egsTransferCommand = @"cmd=transfer_waybill{0}";
		private string _egsSuda7Command = @"cmd=query_suda7&address_1={0}";
		private string _egsSuda7DashCommand = @"cmd=query_suda7_dash&address_1={0}";
		private string _egsDistanceCommand = @"cmd=query_distance&suda5_senderpostcode_1={0}&suda5_customerpostcode_1={1}";

		private List<string> GetData(string param)
		{
			var Url = ConfigurationManager.AppSettings["EgsUrl"];

			HttpWebRequest request = HttpWebRequest.Create(Url) as HttpWebRequest;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.KeepAlive = false; //是否保持連線
									   //request.Timeout = 300000;

			byte[] bs = Encoding.UTF8.GetBytes(param);

			using (Stream reqStream = request.GetRequestStream())
			{
				reqStream.Write(bs, 0, bs.Length);
			}

			var result = "";

			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				using (StreamReader sr = new StreamReader(response.GetResponseStream()))
				{
					result = sr.ReadToEnd();

					sr.Close();
				}
				response.Close();
			}
			return result.Split('&').ToList();
		}
		private static void SetValue<TSource>(TSource destination, List<string> values)
		{
			foreach (var property in typeof(TSource).GetProperties().Where(p => p.CanRead))
			{
				var destinationProperty = typeof(TSource).GetProperty(property.Name);
				foreach (var item in values)
				{
					if (item.Split('=')[0].ToUpper() == property.Name.ToUpper())
					{
						destinationProperty.SetValue(destination, HttpUtility.UrlDecode(item.Split('=')[1]));
						break;
					}
				}
			}
		}
		/// <summary>
		/// 將物件轉成字串用&串聯
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		private static string ObjectToString<TSource>(TSource source)
		{
			var sb = new StringBuilder();
			foreach (var property in typeof(TSource).GetProperties().Where(p => p.CanRead))
			{
				var value = property.GetValue(source);
				if (value != null)
					value = HttpUtility.UrlEncode(value.ToString());
				sb.Append(string.Format("&{0}={1}", property.Name, value ?? ""));
			}
			return sb.ToString();
		}
		/// <summary>
		/// 查詢EGS資訊
		/// </summary>
		/// <returns></returns>
		public EgsInfo GetEgsInfo()
		{
			var data = GetData(_egsInfoCommand);
			var item = new EgsInfo();
			SetValue<EgsInfo>(item, data);
			return item;
		}
		/// <summary>
		/// 查詢地址對應的速達五碼郵遞區號
		/// </summary>
		/// <param name="addressList">地址清單</param>
		/// <returns></returns>
		public List<EgsSuda5> GetEgsSuda5List(List<string> addressList)
		{
			var list = new List<EgsSuda5>();
			foreach (var address in addressList)
			{
				var data = GetData(string.Format(_egsSuda5Command, HttpUtility.UrlEncode((string.IsNullOrWhiteSpace(address)) ? "noaddress" : address)));
				var item = new EgsSuda5();
				SetValue<EgsSuda5>(item, data);
				item.address = address;
				list.Add(item);
			}
			return list;
		}
		/// <summary>
		/// 查詢速達五碼郵遞區號對應的轉運站名稱
		/// </summary>
		/// <param name="suda5List">速達五碼郵遞區號清單</param>
		/// <returns></returns>
		public List<EgsStation> GetEgsStationList(List<string> suda5List)
		{
			var list = new List<EgsStation>();
			foreach (var suda5 in suda5List)
			{
				var data = GetData(string.Format(_egsStationCommand, (string.IsNullOrWhiteSpace(suda5)) ? "99999" : suda5));
				var item = new EgsStation();
				SetValue<EgsStation>(item, data);
				item.suda5 = suda5;
				list.Add(item);
			}
			return list;
		}

		/// <summary>
		/// 查詢契客資料
		/// </summary>
		/// <returns></returns>
		public EgsCustomerInfo GetEsgCustomerInfo()
		{
			var item = new EgsCustomerInfo();
			item.EgsCustomers = new List<EgsCustomer>();
			var data = GetData(_egsCustomerInfoCommand);
			item.status = data.Find(o => o.Contains("status")).Split('=')[1];
			var messageItem = data.Find(o => o.Contains("message"));
			item.message = messageItem == null ? "" : messageItem.Split('=')[1];
			for (var i = 0; i < data.Count - ((messageItem == null) ? 1 : 2); i++)
			{
				var customer = new EgsCustomer();
				var dItem = data.Find(o => o.Contains(string.Format("customer_{0}", i)));
				if (dItem != null)
				{
					customer.customerId = dItem.Split(',')[0].Split('=')[1];
					customer.customerName = dItem.Split(',')[1];
					customer.IsPass = int.Parse(dItem.Split(',')[2]);
					customer.IsCollect = int.Parse(dItem.Split(',')[3]);
					item.EgsCustomers.Add(customer);
				}
			}
			return item;
		}
		/// <summary>
		/// 取得申請速達託運單號碼
		/// </summary>
		/// <param name="customerId">契客代號</param>
		/// <param name="waybilltype">託運單類別"A"=一般託運單|"B"=代收託運單</param>
		/// <param name="count">要產生託運單號碼數量</param>
		/// <returns></returns>
		public EgsRange GetEgsRange(string customerId, string waybilltype, int count)
		{
			var data = GetData(string.Format(_egsRangeCommand, customerId, waybilltype, count));
			var item = new EgsRange();
			SetValue<EgsRange>(item, data);
			return item;
		}
		/// <summary>
		/// 查詢速達託運單號碼存量
		/// </summary>
		/// <param name="customerId">契客代號</param>
		/// <param name="waybilltype">託運單類別"A"=一般託運單|"B"=代收託運單</param>
		/// <returns></returns>
		public EgsStock GetEgsStock(string customerId, string waybilltype)
		{
			var data = GetData(string.Format(_egsStockCommand, customerId, waybilltype));
			var item = new EgsStock();
			SetValue<EgsStock>(item, data);
			return item;
		}

		/// <summary>
		/// 檢查五碼郵號是否可以使用指定的配達時段
		/// </summary>
		/// <param name="serviceType">配達時段 "1"=9~12時|"2"=12~17時|"3"=17~20時|"4"=不限時|"5"=20~21時(夜配)</param>
		/// <param name="suda5">速達5碼郵遞區號</param>
		/// <returns></returns>
		public EgsDeliveryTimeZone GetEgsDeliveryTimeZone(int serviceType, string suda5)
		{
			var data = GetData(string.Format(_egsDeliveryTimeZoneCommand, serviceType, suda5));
			var item = new EgsDeliveryTimeZone();
			SetValue<EgsDeliveryTimeZone>(item, data);
			return item;
		}

		/// <summary>
		/// 傳送託運單資料
		/// </summary>
		/// <param name="datas">要傳送的託運單資料</param>
		/// <returns></returns>
		public List<EgsTransferConsignResult> ConsignToEgs(List<EgsTransferConsign> datas)
		{
			var list = new List<EgsTransferConsignResult>();
			foreach (var transferItem in datas)
			{
				var result = GetData(string.Format(_egsTransferCommand, ObjectToString<EgsTransferConsign>(transferItem)));
				var item = new EgsTransferConsignResult();
				item.EgsTransferConsign = transferItem;
				SetValue<EgsTransferConsignResult>(item, result);
				list.Add(item);
			}
			return list;
		}

		/// <summary>
		/// 查詢地址對應的速達七碼條碼資料(+符號+轉運站 + 速達5碼)
		/// </summary>
		/// <param name="addressList">地址清單</param>
		/// <returns></returns>
		public List<EgsSuda7> GetEgsSuda7List(List<string> addressList)
		{
			var list = new List<EgsSuda7>();
			foreach (var address in addressList)
			{
				var data = GetData(string.Format(_egsSuda7Command, HttpUtility.UrlEncode((string.IsNullOrWhiteSpace(address)) ? "noaddress" : address)));
				var item = new EgsSuda7();
				SetValue<EgsSuda7>(item, data);
				item.address = address;
				list.Add(item);
			}
			return list;
		}

		/// <summary>
		/// 查詢地址對應的速達七碼資料(轉運站及速達5碼有dash分隔)
		/// </summary>
		/// <param name="addressList">地址清單</param>
		/// <returns></returns>
		public List<EgsSuda7Dash> GetEgsSuda7DashList(List<string> addressList)
		{
			var list = new List<EgsSuda7Dash>();
			foreach (var address in addressList)
			{
				var data = GetData(string.Format(_egsSuda7DashCommand, HttpUtility.UrlEncode((string.IsNullOrWhiteSpace(address)) ? "noaddress" : address)));
				var item = new EgsSuda7Dash();
				SetValue<EgsSuda7Dash>(item, data);
				item.address = address;
				list.Add(item);
			}
			return list;
		}
		/// <summary>
		///查詢速達收寄件人地址的距離
		/// </summary>
		/// <param name="senderPostCode">寄件者速達5碼郵遞區號</param>
		/// <param name="customerPostCode">收件者速達5碼郵遞區號</param>
		/// <returns></returns>
		public EgsDistance GetEgsDistance(string senderPostCode, string customerPostCode)
		{
			var data = GetData(string.Format(_egsDistanceCommand, senderPostCode, customerPostCode));
			var item = new EgsDistance();
			SetValue<EgsDistance>(item, data);
			return item;
		}
		/// <summary>
		/// 產生託運單號
		/// </summary>
		/// <returns></returns>
		public ExecuteResult GenerateEgsConsign(AutoGenConsignParam param)
		{
			var schedultJobResultRepo = new SCHEDULE_JOB_RESULTRepository(Schemas.CoreSchema);
			var f194712Repo = new F194712Repository(Schemas.CoreSchema, _wmsTransaction);
			var f19471201Repo = new F19471201Repository(Schemas.CoreSchema, _wmsTransaction);
			var settings = f194712Repo.GetSettings(param).ToList();
			var list = new List<F19471201>();
			var sb = new StringBuilder();
			var isOk = false;
			foreach (var item in settings)
			{
				var id = schedultJobResultRepo.InsertLog(item.DC_CODE, item.GUP_CODE, "0", "GenerateEgsConsign" + item.CONSIGN_TYPE, "0", "");

				//當未使用託運單號數量小於等於安全庫存 則產生新的託運單號
				if (item.UNUSEDQTY <= item.SAVE_QTY)
				{
					var datas = GetEgsRange(item.CUSTOMER_ID, item.CONSIGN_TYPE, item.PATCH_QTY);
					if (datas.status == "OK")
					{
						var consignNoList = datas.waybill_id.Split(',');
						foreach (var consignNo in consignNoList)
						{
							list.Add(new F19471201
							{
								DC_CODE = item.DC_CODE,
								GUP_CODE = item.GUP_CODE,
								CUST_CODE = item.CUST_CODE,
								CHANNEL = item.CHANNEL,
								ALL_ID = item.ALL_ID,
								CONSIGN_TYPE = item.CONSIGN_TYPE,
								CUSTOMER_ID = item.CUSTOMER_ID,
								ISUSED = "0",
								CONSIGN_NO = consignNo,
								CRT_DATE = DateTime.Now,
								CRT_STAFF = Current.Staff,
								CRT_NAME = Current.StaffName,
								ISTEST = item.ISTEST
							});
						}
						sb.Append(string.Format("物流中心:{0} 業主:{1} 貨主:{2} 通路:{3} 配送商:{4} 託運單類別:{5} 契客代號:{6} 產生{7}筆新託運單號成功{8}", item.DC_NAME, item.GUP_NAME, item.CUST_NAME, item.CHANNEL_NAME, item.ALL_COMP, item.CONSIGN_TYPE_NAME, item.CUSTOMER_ID, consignNoList.Count(), Environment.NewLine));
						isOk = true;
						schedultJobResultRepo.UpdateIsSuccess(id ?? 0, "1");
					}
					else
					{
						var message = string.Format("物流中心:{0} 業主:{1} 貨主:{2} 通路:{3} 配送商:{4} 託運單類別:{5} 契客代號{6} 產生新託運單號失敗，EGS失敗原因:{7}{8}", item.DC_NAME, item.GUP_NAME, item.CUST_NAME, item.CHANNEL_NAME, item.ALL_COMP, item.CONSIGN_TYPE_NAME, item.CUSTOMER_ID, datas.message, Environment.NewLine);
						sb.Append(message);
						schedultJobResultRepo.UpdateIsSuccess(id ?? 0, "0", message);
					}
				}
				else
				{
					sb.Append(string.Format("物流中心:{0} 業主:{1}  貨主:{2} 通路:{3} 配送商:{4} 託運單類別:{5} 契客代號{6} 目前託運單號數大於安全存量不須產生{7}", item.DC_NAME, item.GUP_NAME, item.CUST_NAME, item.CHANNEL_NAME, item.ALL_COMP, item.CONSIGN_TYPE_NAME, item.CUSTOMER_ID, Environment.NewLine));
					schedultJobResultRepo.UpdateIsSuccess(id ?? 0, "1");
				}
			}
			if (list.Any())
				f19471201Repo.BulkInsert(list);
			return new ExecuteResult(isOk, sb.ToString());
		}

		/// <summary>
		/// 取得客代主檔設定
		/// </summary>
		/// <returns></returns>
		public IQueryable<F194715> GetEgsCustomerSetting(string customerId = null)
		{
			var f194715Repo = new F194715Repository(Schemas.CoreSchema);
			return f194715Repo.GetSettings("TCAT", customerId);
		}

		public List<EgsReturnConsign> GetEgsReturnConsigns(EgsReturnConsignParam param)
		{
			var repo = new F050301Repository(Schemas.CoreSchema);
			//有單派車 正物流(DISTR_USE = 01)
			var datas = new List<EgsReturnConsign>();
			if (string.IsNullOrWhiteSpace(param.DISTR_USE) || param.DISTR_USE == "01")
				datas.AddRange(repo.GetEgsReturnConsigns(param).ToList());

			var f700101Repo = new F700101Repository(Schemas.CoreSchema);
			//有單派車 逆物流(DISTR_USE = 02)
			if (string.IsNullOrWhiteSpace(param.DISTR_USE) || param.DISTR_USE == "02")
				f700101Repo.GetEgsReturnConsignsByHaveWmsNoBack(param).ToList().ForEach(x =>
				{
					var item = new EgsReturnConsign();
					x.CloneProperties(item);
					datas.Add(item);
				});

			//無單派車 正/逆物流
			var noWmsDistrDatas = f700101Repo.GetEgsReturnConsignsByNoWmsNo(param);
			noWmsDistrDatas.ToList().ForEach(x =>
			{
				var item = new EgsReturnConsign();
				x.CloneProperties(item);
				datas.Add(item);
			});

			foreach (var item in datas)
			{
				if (item.DISTR_USE == "01")
					item.RECEIVER_NAME = item.RECEIVER_NAME.Substring(0, 1) + "**";

				if (string.IsNullOrWhiteSpace(item.SENDER_SUDA5))
				{
					var suda5 = GetEgsSuda5List(new List<string> { item.RECEIVER_ADDRESS, item.SENDER_ADDRESS });
					item.RECEIVER_SUDA5 = suda5[0].suda5_1;
					item.SENDER_SUDA5 = suda5[1].suda5_1;
				}
				else
				{
					var suda5 = GetEgsSuda5List(new List<string> { item.RECEIVER_ADDRESS });
					item.RECEIVER_SUDA5 = suda5[0].suda5_1;
				}
				var distance = GetEgsDistance(item.SENDER_SUDA5, item.RECEIVER_SUDA5);
				item.DISTANCE = distance == null ? "00" : (distance.distance_1 ?? "00");
			}
			return datas;
		}
		public ExecuteResult UpdateStatusForTCAT(List<EgsReturnConsign> datas)
		{
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			f050901Repo.BulkUpdateDistrEdiStatus(datas);
			f700101Repo.BulkUpdatStatus(datas);
			return new ExecuteResult { IsSuccessed = true };
		}

		public ExecuteResult UpdateStatusForTCATSOD(List<F050901> datas)
		{
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var d3 = datas.Where(o => o.STATUS == "3").ToList();
			if (d3 != null && d3.Any())
				f050901Repo.BulkUpdateDistrEdiStatusSod(d3, "3");
			var d = datas.Where(o => o.STATUS != "3" && o.STATUS != "4").ToList();
			if (d != null && d.Any())
				f050901Repo.BulkUpdateDistrEdiStatusSod(d, "2");
			var d4 = datas.Where(o => o.STATUS == "4").ToList();
			if (d4 != null && d4.Any())
				f050901Repo.BulkUpdateDistrEdiStatusSod(d4, "4");
			return new ExecuteResult { IsSuccessed = true };
		}

		public List<F194714> GetStatusList(string allId = "")
		{
			var f194714repos = new F194714Repository(Schemas.CoreSchema, _wmsTransaction);
			if (string.IsNullOrWhiteSpace(allId))
				return f194714repos.GetDatasByAllId("TCAT");
			else
				return f194714repos.GetDatasByAllId(allId);
		}

		public List<F050901> GetUpdateF050901DataForSOD(string customerId, List<string> consignNos)
		{
			var f050901repos = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			return f050901repos.GetUpDataForSOD(customerId, consignNos);
		}
	}
}
