using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P08.Services
{
	public partial class PurchaseService
	{
		/// <summary>
		/// 是否可以特殊採購
		/// </summary>
		/// <param name="p020203Data"></param>
		/// <returns></returns>
		public bool CanExecuteSpecial(P020203Data p020203Data)
		{
			if (p020203Data == null || p020203Data.STATUS != "0")
				return false;

			if (p020203Data.BUNDLE_SERIALNO == "0")
			{
				// 只有未檢驗的一般商品才能做特殊採購
				return (p020203Data.CHECK_ITEM == "0");
			}
			else
			{
				// 序號商品若尚未檢驗且尚未已刷讀序號，也不能做特殊採購
				if (p020203Data.CHECK_ITEM == "0" && p020203Data.CHECK_SERIAL == "0")
					return false;

				// 序號商品若尚未檢驗或尚未已刷讀序號只有其中一個符合才能做特殊採購
				return (p020203Data.CHECK_ITEM == "0" || p020203Data.CHECK_SERIAL == "0");
			}
		}

		/// <summary>
		/// 當進倉單沒有填採購單號時，詢問是否繼續作業，是的話，採購單號自動填入進倉單號
		/// </summary>
		/// <returns></returns>
		public bool CheckShopNo(InputViewModelBase viewModel, string dcCode, string gupCode, string custCode, string purchaseNo)
		{
			var proxy = viewModel.GetExProxy<P02ExDataSource>();
			var result = proxy.CheckShopNo(dcCode, gupCode, custCode, purchaseNo);
			if (result.IsSuccessed)
				return true;

			// "該進倉單無採購單號，請確定是否繼續作業?"
			if (viewModel.ShowConfirmMessage(result.Message) == DialogResponse.No)
				return false;

			result = proxy.UpdateShopNoBePurchaseNo(dcCode, gupCode, custCode, purchaseNo);
			if (!result.IsSuccessed)
				viewModel.ShowResultMessage(result);

			return result.IsSuccessed;
		}

	}
}
