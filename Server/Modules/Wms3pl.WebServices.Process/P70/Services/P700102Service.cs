using Wms3pl.Datas.F70;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P70.Services
{
	public partial class P700102Service
	{
		private WmsTransaction _wmsTransaction;
		public P700102Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 刪除派車記錄
		/// </summary>
		/// <param name="serialNo">單據編號</param>
		/// <returns></returns>
		public bool DeleteDistrCarRecord(string serialNo, string dcCode, string gupCode, string custCode)
		{
			var f700101Repo = new F700101Repository(Schemas.CoreSchema);
			var f700102Repo = new F700102Repository(Schemas.CoreSchema);
			//先取出明細對應 DISTR_CAR_NO
			var f700102Data = f700102Repo.GetF700102CarNo(serialNo, dcCode, gupCode, custCode);
			foreach (var item in f700102Data)
			{
				f700101Repo.DeleteF700101(item.DISTR_CAR_NO, item.DC_CODE);
			}
			f700102Repo.DeleteF700102(serialNo, dcCode, gupCode, custCode);
			return true;
		}
	}
}
