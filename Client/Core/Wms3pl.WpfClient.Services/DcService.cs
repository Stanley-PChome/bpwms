using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Xml.Linq;
using System.Xml.XPath;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;

namespace Wms3pl.WpfClient.Services
{
  public partial class DcService : IDcService
  {
      F19Entities _proxy;
	  private List<F0005> FolderLoginInfo;

	  public DcService()
	  {
		  _proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "DcSetting");

		  var proxyF00 = ConfigurationHelper.GetProxy<F00Entities>(false, "DcSetting");
		  FolderLoginInfo = (from p in proxyF00.F0005s
			  where p.SET_NAME == "FOLDER_USER" || p.SET_NAME == "FOLDER_PW" || p.SET_NAME == "FOLDER_DOMAIN"
			  select p).ToList();
	  }

	  /// <summary>
    /// 取得登入帳號的名稱
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public string GetAccountName(string account)
    {
      var empName = (from a in _proxy.F1924s where a.EMP_ID.Equals(account) select a.EMP_NAME).FirstOrDefault();
      return empName == null ? "未具名" : empName;
    }

		/// <summary>
		/// 取得登入者可操作業主貨主物流中心
		/// </summary>
		/// <param name="account"></param>
		/// <returns></returns>
		public List<F192402Data> GetF192402Data(string account)
		{
			var allDc = (from p in _proxy.F1901s
				select p).ToList();

			var allGup = (from p in _proxy.F1929s
				select p).ToList();

			var allCust = (from a in _proxy.F1909s
						   where a.STATUS != "9"
						   select a).ToList();


			var userSet = (from i in _proxy.F192402s
							 where i.EMP_ID.Equals(account)
							 select new F192402Data()
							 {
								 EmpId = i.EMP_ID,
								 DcCode = i.DC_CODE,
								 GupCode = i.GUP_CODE,
								 CustCode = i.CUST_CODE
							 }).ToList();

			return (from user in userSet
				join dc in allDc on user.DcCode equals dc.DC_CODE
				join gup in allGup on user.GupCode equals gup.GUP_CODE
				join cust in allCust
					on new {user.GupCode, user.CustCode}
					equals new {GupCode = cust.GUP_CODE, CustCode = cust.CUST_CODE}
				select new F192402Data()
				{
					EmpId = user.EmpId,
					DcCode = user.DcCode,
					GupCode = user.GupCode,
					CustCode = user.CustCode,
					DcName = dc.DC_NAME,
					GupName = gup.GUP_NAME,
					CustName = cust.SHORT_NAME
				}).ToList();
			
		}

		public List<F190907Data> GetItemImagePathDatas()
		{
			return (from a in _proxy.F190907s
					where a.PATH_KEY == "ItemImagePath"
					select new F190907Data
					{
						DC_CODE = a.DC_CODE,
						GUP_CODE = a.GUP_CODE,
						CUST_CODE = a.CUST_CODE,
						PATH_KEY = a.PATH_KEY,
						PATH_NAME = a.PATH_NAME,
						PATH_ROOT = a.PATH_ROOT
					}).ToList();
		}


    public string GetSchemaName(string dcCode, string custCode)
    {
      return GetSchema(dcCode, custCode);
    }

    public string GetSchema(string dcCode, string custCode)
    {
      string filePath = @"Schema.xml";
      var doc = XDocument.Load(filePath);
      var selectPath = string.Format("/root/dc[@code={0}]/cust[@code={1}]", dcCode, custCode);
      var resultElement = doc.XPathSelectElement(selectPath);
      if (resultElement != null)
        return (string)resultElement.Attribute("Schema");
      else
        throw new Exception("未設定資料連線。請參考 Schema.xml");
    }

		public bool CheckIsCommon(string account)
		{
			var f1924 = (from a in _proxy.F1924s
										 where a.EMP_ID == account && a.ISCOMMON == "1"
										 select a).SingleOrDefault();
			return (f1924 != null);
		}

	  public string GetFolderUser()
	  {
			if (FolderLoginInfo != null && FolderLoginInfo.Any())
			{
				var f0005 = FolderLoginInfo.FirstOrDefault(n => n.SET_NAME == "FOLDER_USER");
				if (f0005 != null)
					return f0005.SET_VALUE;				
			}
			return "NoSetting";//若不模擬需給值 NoSetting
	  }

		public string GetFolderPw()
		{
			if (FolderLoginInfo != null && FolderLoginInfo.Any())
			{
				var f0005 = FolderLoginInfo.FirstOrDefault(n => n.SET_NAME == "FOLDER_PW");
				if (f0005 != null)
					return AesCryptor.Current.Decode(f0005.SET_VALUE);
			}
			return string.Empty;
		}

		public string GetFolderDomain()
		{
			if (FolderLoginInfo != null && FolderLoginInfo.Any())
			{
				var f0005 = FolderLoginInfo.FirstOrDefault(n => n.SET_NAME == "FOLDER_DOMAIN");
				if (f0005 != null)
					return f0005.SET_VALUE;
			}
			return string.Empty;
		}
  }
}
