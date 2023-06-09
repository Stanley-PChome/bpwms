
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F50;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P91.Services
{
	public partial class P910303Service
	{
		private WmsTransaction _wmsTransaction;
		public P910303Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public string GetContractFee(string dcCode, string gupCode, string custCode, string quoteNo, string itemTye)
		{
			string ContractFee = string.Empty;
			if (string.IsNullOrEmpty(itemTye))
				return string.Empty;
			//002:儲位(倉租);005:派車(運費);003:作業;004:出貨;006:其他
			if (itemTye == "002")
			{
				#region 儲位(倉租)
				var repo = new F500101Repository(Schemas.CoreSchema);
				var result = repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.QUOTE_NO == quoteNo);
				if (result != null)
					ContractFee = string.Format(Properties.Resources.P910303Service_FEE,result.APPROV_UNIT_FEE);
				#endregion
			}
			else if (itemTye == "005")
			{
				#region 派車(運費)
				var repo = new F500102Repository(Schemas.CoreSchema);
				var result = repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.QUOTE_NO == quoteNo);
				if (result == null || string.IsNullOrEmpty(result.ACC_KIND))
					return string.Empty;

				//計價方式(A:均一價 B:實際尺寸 C:材積 D:重量)
				if (result.ACC_KIND == "A")
				{
					ContractFee = string.Format(Properties.Resources.P910303Service_FEE, result.APPROV_FEE);
				}
				else if (result.ACC_KIND == "B")
				{ 
					StringBuilder sb = new StringBuilder();
					sb.Append(string.Format(Properties.Resources.P910303Service_SIZE,result.ACC_NUM));
					sb.Append((result.MAX_WEIGHT != null) ? string.Format(Properties.Resources.P910303Service_WEIGHT,result.MAX_WEIGHT) : string.Empty);
					sb.Append(string.Format(Properties.Resources.P910303Service_FEE_DOLLAR,result.APPROV_FEE));
					sb.Append((result.OVER_VALUE != null) ? string.Format(Properties.Resources.P910303Service_OVERSIZE,result.OVER_VALUE) : string.Empty);
					sb.Append((result.OVER_UNIT_FEE != null) ? string.Format(Properties.Resources.P910303Service_FEE_DOLLAR, result.OVER_UNIT_FEE) : string.Empty);
					ContractFee = sb.ToString();
				}
				else if (result.ACC_KIND == "C")
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(string.Format(Properties.Resources.P910303Service_ACC_NUM, result.ACC_NUM));
					sb.Append((result.MAX_WEIGHT != null) ? string.Format(Properties.Resources.P910303Service_WEIGHT, result.MAX_WEIGHT) : string.Empty);
					sb.Append(string.Format(Properties.Resources.P910303Service_FEE_DOLLAR, result.APPROV_FEE));
					sb.Append((result.OVER_VALUE != null) ? string.Format(Properties.Resources.P910303Service_OVER_VALUE, result.OVER_VALUE) : string.Empty);
					sb.Append((result.OVER_UNIT_FEE != null) ? string.Format(Properties.Resources.P910303Service_FEE_DOLLAR, result.OVER_UNIT_FEE) : string.Empty);
					ContractFee = sb.ToString();
				}
				else if (result.ACC_KIND == "D")
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(string.Format(Properties.Resources.P910303Service_ACC_NUM_WEIGHT, result.ACC_NUM));
					sb.Append(string.Format(Properties.Resources.P910303Service_FEE_DOLLAR, result.APPROV_FEE));
					sb.Append((result.OVER_VALUE != null) ? string.Format(Properties.Resources.P910303Service_OVER_WEIGHT, result.OVER_VALUE) : string.Empty);
					sb.Append((result.OVER_UNIT_FEE != null) ? string.Format(Properties.Resources.P910303Service_FEE_DOLLAR, result.OVER_UNIT_FEE) : string.Empty);
					ContractFee = sb.ToString();
				}
				#endregion
			}
			else if (itemTye == "003")
			{
				#region 作業
				var repo = new F500104Repository(Schemas.CoreSchema);
				var result = repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.QUOTE_NO == quoteNo);
				if (result == null)
					return string.Empty;
				//計價方式(A:單一費用 B:條件計費)
				if (result.ACC_KIND == "A")
				{
					ContractFee = string.Format(Properties.Resources.P910303Service_APPROV_FEE, result.APPROV_FEE);
				}
				else if (result.ACC_KIND == "B")
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(string.Format(Properties.Resources.P910303Service_BASIC_FEE, result.BASIC_FEE));
					sb.Append(string.Format(Properties.Resources.P910303Service_OVER_FEE, result.OVER_FEE));
					ContractFee = sb.ToString();
				}
				#endregion
			}
			else if (itemTye == "004")
			{
				#region 出貨
				var repo = new F500103Repository(Schemas.CoreSchema);
				var result = repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.QUOTE_NO == quoteNo);
				if (result == null)
					return string.Empty;
				//計價方式(A:單一費用 B:條件計費)
				if (result.ACC_KIND == "A")
				{
					ContractFee = string.Format(Properties.Resources.P910303Service_APPROV_FEE, result.APPROV_FEE);
				}
				else if (result.ACC_KIND == "B")
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(string.Format(Properties.Resources.P910303Service_BASIC_FEE, result.BASIC_FEE));
					sb.Append(string.Format(Properties.Resources.P910303Service_OVER_FEE, result.OVER_FEE));
					ContractFee = sb.ToString();
				}
				#endregion
			}
			else if (itemTye == "006")
			{
				#region 其他
				var repo = new F500105Repository(Schemas.CoreSchema);
				var result = repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.QUOTE_NO == quoteNo);
				if (result != null)
					ContractFee = string.Format(Properties.Resources.P910303Service_FEE, result.APPROV_FEE);
				#endregion
			}
			else if (itemTye == "007")
			{
				#region 專案
				var repo = new F199007Repository(Schemas.CoreSchema);
				var result = repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ACC_PROJECT_NO == quoteNo);
				if (result != null)
					ContractFee = string.Format(Properties.Resources.P910303Service_FEE, result.FEE);
				#endregion
			}
			return ContractFee;
		}
	}
}

