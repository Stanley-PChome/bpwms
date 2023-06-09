using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.Shared.ServiceEntites
{
	public class EgsBase
	{
		/// <summary>
		/// 狀態:OK|ERROR
		/// </summary>
		public string status { get; set; }
		/// <summary>
		/// 錯誤訊息
		/// </summary>
		public string message { get; set; }
	}
	/// <summary>
	/// Egs資訊
	/// </summary>
	public class EgsInfo: EgsBase
	{
		/// <summary>
		/// EGS目前版本
		/// </summary>
		public string egs_version { get; set; }
		/// <summary>
		/// 速達五碼郵號地址庫目前版本
		/// </summary>
		public string address_db_version { get; set; }
		/// <summary>
		/// 0 or 1:處於Sandbox模式
		/// </summary>
		public string sandbox_mode { get; set; }
		/// <summary>
		/// 0 or 1:啟用網際網路連線
		/// </summary>
		public string internet_online { get; set; }
	}
	/// <summary>
	/// 地址對應的速達五碼郵遞區號
	/// </summary>
	public class EgsSuda5:EgsBase
	{
		public string address { get; set; }
		/// <summary>
		/// 速達五碼郵遞區號
		/// </summary>
		public string suda5_1 { get; set; }
	}
	/// <summary>
	/// 速達五碼郵遞區號對應的轉運站名稱
	/// </summary>
	public class EgsStation:EgsBase
	{
		public string suda5 { get; set; }
		public string base_1 { get; set; }
	}
	/// <summary>
	/// Egs客戶清單
	/// </summary>
	public class EgsCustomerInfo:EgsBase
	{
		public List<EgsCustomer> EgsCustomers { get; set; }
	}
	/// <summary>
	/// Egs客戶資料
	/// </summary>
	public class EgsCustomer
	{
		/// <summary>
		/// 契客編號
		/// </summary>
		public string customerId { get; set; }
		/// <summary>
		/// 契客名稱
		/// </summary>
		public string customerName { get; set; }
		/// <summary>
		/// 0 or 1:已驗證通過
		/// </summary>
		public int IsPass { get; set; }
		/// <summary>
		/// 0 or 1:屬於客樂得契約客戶
		/// </summary>
		public int IsCollect { get; set; }

	}
	/// <summary>
	/// 速達託運單號碼
	/// </summary>
	public class EgsRange:EgsBase
	{
		/// <summary>
		/// 託運單類別
		/// "A"=一般託運單|"B"=代收託運單
		/// </summary>
		public string waybill_type { get; set; }
		/// <summary>
		/// 託運單號(逗點分隔)
		/// </summary>
		public string waybill_id { get; set; }
	}
	/// <summary>
	/// 
	/// </summary>
	public class EgsStock : EgsBase
	{
		/// <summary>
		/// 託運單類別
		/// "A"=一般託運單|"B"=代收託運單
		/// </summary>
		public string waybill_type { get; set; }
		/// <summary>
		/// 速達託運單號剩餘存量
		/// </summary>
		public string waybill_id_remain { get; set; }
	}
	/// <summary>
	/// 五碼郵號是否可以使用指定的配達時段
	/// </summary>
	public class EgsDeliveryTimeZone : EgsBase
	{
		/// <summary>
		/// 配達時段 "1"=9~12時|"2"=12~17時|"3"=17~20時|"4"=不限時|"5"=20~21時
		/// </summary>
		public string	delivery_timezone_1 { get; set; }
	}
	public class EgsTransferConsign
	{
		/// <summary>
		/// 契客編號
		/// </summary>
		public string customer_id { get; set; }
		/// <summary>
		/// 託運單號碼
		/// </summary>
		public string tracking_number { get; set; }
		/// <summary>
		/// 訂單編號
		/// </summary>
		public string order_no { get; set; }
		/// <summary>
		/// 收件人姓名
		/// </summary>
		public string receiver_name { get; set; }
		/// <summary>
		/// 收件人地址
		/// </summary>
		public string receiver_address { get; set; }
		/// <summary>
		/// 收件人地址的速達五碼郵遞區號
		/// </summary>
		public string receiver_suda5 { get; set; }
		/// <summary>
		/// 收件者行動電話
		/// </summary>
		public string receiver_mobile { get; set; }
		/// <summary>
		/// 收件者電話
		/// </summary>
		public string receiver_phone { get; set; }
		/// <summary>
		/// 寄件者姓名
		/// </summary>
		public string sender_name { get; set; }
		/// <summary>
		/// 寄件者地址
		/// </summary>
		public string sender_address { get; set; }
		/// <summary>
		/// 寄件者地址的速達五碼郵遞區號
		/// </summary>
		public string sender_suda5 { get; set; }
		/// <summary>
		/// 寄件者電話
		/// </summary>
		public string sender_phone { get; set; }
		/// <summary>
		/// 代收款項金額
		/// </summary>
		public string product_price { get; set; }
		/// <summary>
		/// 品名
		/// </summary>
		public string product_name { get; set; }
		/// <summary>
		/// 備註
		/// </summary>
		public string comment { get; set; }
		/// <summary>
		/// 尺寸
		/// "0001"=60cm,"0002"=90cm,"0003"=120cm,"0004"=150cm
		/// </summary>
		public string package_size { get; set; }
		/// <summary>
		/// 溫層
		/// "0001"=常溫|"0002"=冷藏|"0003"=冷凍
		/// </summary>
		public string temperature { get; set; }
		/// <summary>
		/// 距離
		/// "00"=同縣市|"01"=外縣市|"02"=離島
		/// </summary>
		public string distance { get; set; }
		/// <summary>
		/// 指定配達日期
		/// </summary>
		public string delivery_date { get; set; }
		/// <summary>
		/// 指定配達時段
		/// "1"=9~12時|"2"=12~17時|"3"=17~20時|"4"=不限時|"5"=20~21時 
		/// </summary>
		public string delivery_timezone { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		public string create_time { get; set; }
		/// <summary>
		/// 列印日期
		/// </summary>
		public string print_time { get; set; }
		/// <summary>
		/// 託運單帳號
		/// </summary>
		public string account_id { get; set; }
		/// <summary>
		/// 會員編號
		/// </summary>
		public string member_no { get; set; }
	}
	public class EgsTransferConsignResult:EgsBase
	{
		/// <summary>
		/// 託運單資料
		/// </summary>
		public EgsTransferConsign EgsTransferConsign { get; set; }
	}
	/// <summary>
	/// 速達7碼條碼資料(+符號+轉運站 + 速達5碼)
	/// </summary>
	public class EgsSuda7:EgsBase
	{
		/// <summary>
		/// +符號+轉運站 + 速達5碼
		/// </summary>
		public string suda7_1 { get; set; }
		/// <summary>
		/// 地址
		/// </summary>
		public string address { get; set; }
	}
	/// <summary>
	/// 速達7碼資料(轉運站及速達5碼有dash分隔)
	/// </summary>
	public class EgsSuda7Dash : EgsBase
	{
		/// <summary>
		/// 轉運站xx+'-'+速達5碼(xxx-xx)
		/// </summary>
		public string suda7_1 { get; set; }
		/// <summary>
		/// 地址
		/// </summary>
		public string address { get; set; }

	}
	/// <summary>
	/// 速達收寄件人地址的距離
	/// </summary>
	public class EgsDistance:EgsBase
	{
		/// <summary>
		/// 00-同縣市 01-外縣市 02-離島
		/// </summary>
		public string distance_1 { get; set; }
	}
}
