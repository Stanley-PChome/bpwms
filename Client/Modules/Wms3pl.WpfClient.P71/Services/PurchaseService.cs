﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P71.Services
{
	public partial class PurchaseService
	{

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
