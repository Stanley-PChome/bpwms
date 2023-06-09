using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F70DataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using DeliveryData = Wms3pl.WpfClient.ExDataServices.P08ExDataService.DeliveryData;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;

namespace Wms3pl.WpfClient.P08.Services
{
	public class ReportData : DeliveryData
	{
		public int PackageBoxNo { get; set; }
		public string BoxNo { get; set; }
		public string ItemCodeBarcode { get; set; }
	}

	public class SerialReadingStatus
	{
		public string SerialNo { get; set; }
		public string ItemCode { get; set; }
		public string IsPass { get; set; }
		public string Message { get; set; }
	}

	public static class PackingService
	{
		/// <summary>
		/// 刷讀後更新統計數
		/// </summary>
		public static List<DeliveryData> RefreshReadCount(List<DeliveryData> dlvData, string dcCode, string gupCode, string custCode, string wmsOrdNo, short packageBoxNo, string functionCode)
		{
			// 每讀一次就從資料庫讀一次實數總數
			var proxy = ConfigurationExHelper.GetExProxy<P08ExDataSource>(false, functionCode);
			var deliveryDataList = proxy.CreateQuery<DeliveryData>("GetQuantityOfDeliveryInfo")
										.AddQueryExOption("dcCode", dcCode)
										.AddQueryExOption("gupCode", gupCode)
										.AddQueryExOption("custCode", custCode)
										.AddQueryExOption("wmsOrdNo", wmsOrdNo)
										.AddQueryExOption("packageBoxNo", packageBoxNo)
										.ToList();

			return deliveryDataList;
		}

		/// <summary>
		/// 是否為同DC的內部交易單
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		public static bool IsSameDCInternalTrading(F050801 f050801)
		{
			return f050801.SOURCE_TYPE == "09" && f050801.PRINT_PASS == "0";
		}

		public static List<NameValuePair<string>> GetStatusList(string functionCode)
		{
			return GetBaseTableService.GetF000904List(functionCode, "F050801", "STATUS", true);
		}

		public static List<NameValuePair<string>> GetLackStatusList(string functionCode)
		{
			return GetBaseTableService.GetF000904List(functionCode, "F051206", "STATUS", true);
		}

		public static List<NameValuePair<string>> GetLackRtnStatusList(string functionCode)
		{
			return GetBaseTableService.GetF000904List(functionCode, "F051206", "RETURN_FLAG", true);
		}

		public static List<LableItem> PassDataToLabel(List<F055001Data> consignData)
		{
			var labelItems = new List<LableItem>();
			foreach (var pass in consignData)
			{
				var label = new LableItem();
				label.input = new ObservableCollection<string>();
                //HCT (input1 ~ input20) 
                label.input.Add(pass.DELV_DATE);
				label.input.Add(pass.CUST_ORD_NO);
				label.input.Add(pass.ARRIVAL_DATE);
				label.input.Add(pass.COLLECT_AMT.ToString());
				label.input.Add(pass.ERST_NO);
				label.input.Add(pass.TOTAL_AMOUNT.ToString("##,###"));
				label.input.Add(pass.COLLECT);
				label.input.Add(pass.CONSIGNEE);
				label.input.Add(pass.ADDRESS);
				label.input.Add("");
				label.input.Add("");
				label.input.Add(pass.TEL);
				label.input.Add(Properties.Resources.PackingService_SATotal + pass.SA_QTY + Properties.Resources.PackingService_Copies);
				label.input.Add(pass.SHORT_NAME);
				label.input.Add(pass.PAST_NO);
				label.input.Add(pass.CUST_TEL);
				label.input.Add(pass.CUST_ADDRESS);
				label.input.Add(Properties.Resources.PackingService_MEMO + pass.MEMO);
				label.input.Add("");
				label.input.Add("");

                //HLSC (input21 ~ input30) 
                //label.input.Add(string.IsNullOrEmpty(pass.RETAIL_CODE) ? string.Empty.PadLeft(10, '_') : pass.RETAIL_CODE);
                label.input.Add(pass.RETAIL_CODE);
				label.input.Add(pass.CUST_NAME);
				label.input.Add("");
				label.input.Add(pass.ADDRESS_TYPE);
				label.input.Add(pass.ROUTE);
				label.input.Add(pass.FIXED_CODE);
				label.input.Add(pass.PRINT_TIME);
				label.input.Add(pass.PAST_NO);
				label.input.Add(pass.ADDRESS);
                label.input.Add("");

                //目前未有標籤使用,若有10個欄位內的可以拿來用 (input31 ~ input40) 
                label.input.Add("");
                label.input.Add("");
                label.input.Add("");
                label.input.Add("");
                label.input.Add("");
                label.input.Add("");
                label.input.Add("");
                label.input.Add("");
                label.input.Add("");
                label.input.Add("");


                //PELICAN (input41 ~ input60) 
                label.input.Add(pass.DELV_DATE);
                label.input.Add(pass.CUST_ORD_NO);
                label.input.Add(pass.ARRIVAL_DATE);
                label.input.Add(pass.COLLECT_AMT.ToString().Equals("0") ? Properties.Resources.PackingService_No : pass.COLLECT_AMT.ToString());
                //宅配通 ZIP
                string[] PELICAN_DATA = null;
                if (pass.ERST_NO != null)
                    PELICAN_DATA = pass.ERST_NO.Split('/');
                if(PELICAN_DATA != null)
                    label.input.Add((PELICAN_DATA.Length >= 1 ? PELICAN_DATA[0] : "").Length > 3 ? PELICAN_DATA[0].Insert(3, "-") : PELICAN_DATA[0]);
                else
                    label.input.Add("");

                label.input.Add(pass.CONSIGN_ID);
                label.input.Add(pass.CONSIGN_NAME);
                label.input.Add(pass.CONSIGNEE);
                label.input.Add(pass.ADDRESS);
                label.input.Add(pass.TEL);
                label.input.Add(pass.MEMO);
                //宅配通 配送區域
                if(PELICAN_DATA != null)
                    label.input.Add(PELICAN_DATA.Length >= 2 ? PELICAN_DATA[1] : "");
                else
                    label.input.Add("");

                label.input.Add(pass.PAST_NO);
                label.input.Add(pass.CUST_TEL);
                label.input.Add(pass.CUST_ADDRESS);
                label.input.Add(pass.PAST_NO);
                label.input.Add("1");
                label.input.Add(" ");
                label.input.Add("1");
                label.input.Add("");

				       
                //黑貓宅急便 (input61 ~ input84) 
                label.input.Add(pass.PAST_NO);//包裹查詢號碼 input61
                label.input.Add(pass.DELV_DATE);//收貨日 input62
                label.input.Add(pass.ARRIVAL_DATE);//預定配達日 input63
                label.input.Add(Properties.Resources.PackingService_NotSpecify);//配達時段 input64
                label.input.Add(Properties.Resources.PackingService_NorthTwoSpecial);//發貨所 input65 call api
                label.input.Add(pass.EGS_VOLUME + "cm");//尺寸 input66
                label.input.Add(pass.CONSIGNEE);//收件人 input67
                label.input.Add(pass.TEL);//電話 input68
                label.input.Add(pass.ADDRESS);//地址 input69
                label.input.Add(pass.EGS_SUDA5);//郵遞區號 input70  call api
                label.input.Add(Wms3plSession.Get<GlobalInfo>().CustName);//寄件人 input71
                label.input.Add(pass.CUST_TEL);//寄件人電話 input72
                label.input.Add("");//寄件人手機 input73
                label.input.Add(pass.CUST_ADDRESS);//寄件人地址 input74
                label.input.Add(pass.CUST_ZIP);//寄件人郵遞區號 input75
                label.input.Add(pass.COLLECT_AMT.ToString() + Properties.Resources.PackingService_Yuan);//代收款 input76
                label.input.Add(pass.CUST_ORD_NO);//訂單編號 input77
                label.input.Add("");//品名 input78
                label.input.Add(pass.MEMO);//備註 input79
                label.input.Add(pass.EGS_SUDA7_DASH);//Egs input80 call api
                label.input.Add(pass.EGS_SUDA7);//EgsBarCode input81 call api
                label.input.Add(pass.VERSION_DATA);//VersionDate input82 call api
                label.input.Add(pass.VERSION_NUMBER);//VersionNumber input83 call api
                label.input.Add(pass.CONSIGN_ID);//客戶代號 input84

                labelItems.Add(label);
			}
			return labelItems;
		}
	}
}
