using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Wms3pl.WpfClient.Common
{
  public class GlobalInfo
  {
		/// <summary>
		/// 目前選擇的 GupCode
		/// </summary>
		public string GupCode { get; set; }

		/// <summary>
		/// 目前選擇的 GupName
		/// </summary>
		public string GupName { get; set; }

		/// <summary>
		/// 目前選擇的 CustCode
		/// </summary>
		public string CustCode { get; set; }

		/// <summary>
		/// 目前選擇的 Cust Name
		/// </summary>
		public string CustName { get; set; }

    /// <summary>
    /// 系統所屬公司
    /// </summary>
    public string Owner { get; set; }

    /// <summary>
    /// 應用的 schema name
    /// </summary>
    public string SchemaName { get; set; }

		/// <summary>
		/// 目前選擇的 Lang
		/// </summary>
		public string Lang { get; set; }

		public bool IsSecretePersonalData { get; set; }

		public bool IsSecretePersonalDataVm { get; set; }

		public string FunctionCode { get; set; }

		public bool IsNeedGCCollect { get; set; }

		public string ImpersonationDomain { get; set; }

		public string ImpersonationAccount { get; set; }

		public string ImpersonationPassword { get; set; }

	  public string ClientIp { get; set; }

		public string ItemImagePath { get; set; }

		/// <summary>
		/// 登入者可操作的物流中心,業主,貨主
		/// </summary>
		public List<F192402Data> DcGupCustDatas { get; set; }

		public List<F190907Data> ItemPathDatas { get; set; }

		/// <summary>
		/// 登入者可操作的物流中心(for 程式內部下拉選單用)
		/// </summary>
	  public List<NameValuePair<string>> DcCodeList { get; set; }

		public int ModifyingFormCount { get; set; }

		/// <summary>
		/// 登入者可操作的業主
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		public List<NameValuePair<string>> GetGupDataList(string dcCode = null)
	  {
			return (from o in
											 (from o in DcGupCustDatas
												where (string.IsNullOrEmpty(dcCode) || o.DcCode == dcCode)
												select new { o.GupCode, o.GupName }).Distinct()
							orderby o.GupCode
							select new NameValuePair<string>()
							{
								Name = o.GupName,
								Value = o.GupCode
							}).ToList();
	  }
		/// <summary>
		/// 登入者可操作的貨主
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主(null或空白代表全部)</param>
	  public List<NameValuePair<string>> GetCustDataList(string dcCode = null,string gupCode = null)
	  {
			return (from o in
										(from o in DcGupCustDatas
										 where (string.IsNullOrEmpty(dcCode) || o.DcCode == dcCode) &&
													(string.IsNullOrEmpty(gupCode) || o.GupCode == gupCode)
										 select new { o.CustCode, o.CustName }).Distinct()
							orderby o.CustCode
							select new NameValuePair<string>()
							{
								Name = o.CustName,
								Value = o.CustCode
							}).Distinct().ToList();
	  }

		/// <summary>
		/// 登入者可操作的物流中心,業主,貨主
		/// </summary>
		public List<FunctionShowInfo> FunctionShowInfos { get; set; }
  }

	public class F192402Data
	{
		public string EmpId { get; set; }
		public string DcCode { get; set; }
		public string DcName { get; set; }
		public string GupCode { get; set; }
		public string GupName { get; set; }
		public string CustCode { get; set; }
		public string CustName { get; set; }
	}

	public class F190907Data
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string PATH_KEY { get; set; }
		public string PATH_NAME { get; set; }
		public string PATH_ROOT { get; set; }
	}
	public class FunctionShowInfo
	{
		public System.Decimal GRP_ID { get; set; }
		public string SHOWINFO { get; set; }
		public string FUN_CODE { get; set; }
	}
}
