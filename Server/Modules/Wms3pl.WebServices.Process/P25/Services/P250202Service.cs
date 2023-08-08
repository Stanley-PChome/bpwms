
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.WebServices.Process.P25.Services
{
	public partial class P250202Service
	{
		private WmsTransaction _wmsTransaction;
		public P250202Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}
		/// <summary>
		/// 異動查詢
		/// </summary>	
		public IQueryable<P2502QueryData> GetP2502QueryDatas(string gupCode, string custCode,
			 string itemCode, string serialNo, string batchNo, string cellNum, string poNo, string wmsNo
			, string status, string retailCode, Int16? combinNo, string crtName, string updSDate
			, string updEDate, string boxSerial, string OpItemType)
		{

			var coverUpdSDate = (string.IsNullOrEmpty(updSDate)) ? ((DateTime?)null) : Convert.ToDateTime(updSDate);
			var coverUpdEDate = (string.IsNullOrEmpty(updEDate)) ? ((DateTime?)null) : Convert.ToDateTime(updEDate);

			var itemCodeArray = string.IsNullOrEmpty(itemCode) ? new string[] { } : itemCode.Split(',').ToArray();
			var wmsNoArray = string.IsNullOrEmpty(wmsNo) ? new string[] { } : wmsNo.Split(',').ToArray();
			var serialNoArray = string.IsNullOrEmpty(serialNo) ? new string[] { } : serialNo.Split(',').ToArray();



			var repF250101 = new F250101Repository(Schemas.CoreSchema);
			var f2502QueryData = repF250101.GetP2502QueryDatas(gupCode, custCode, itemCodeArray, serialNoArray, batchNo, cellNum, poNo, wmsNoArray
																, status, retailCode, combinNo, crtName, coverUpdSDate
																, coverUpdEDate, boxSerial, OpItemType);
			return f2502QueryData; 
		}



	}
}
