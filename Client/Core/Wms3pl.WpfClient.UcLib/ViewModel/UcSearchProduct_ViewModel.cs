using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.SharedWcfService;

namespace Wms3pl.WpfClient.UcLib.ViewModel
{
	public partial class UcSearchProduct_ViewModel : InputViewModelBase
	{
		public UcSearchProduct_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}

		}
		private void InitControls()
		{
			//_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		}


		//private string _gupCode;

		//public string GupCode
		//{
		//	get { return _gupCode; }
		//	set { Set(() => CustCode, ref _gupCode, value); }
		//}

		//private string _custCode;

		//public string CustCode
		//{
		//	get { return _custCode; }
		//	set { Set(() => CustCode, ref _custCode, value);}
		//}

		private string _searchItemCode;

		public string SearchItemCode
		{
			get { return _searchItemCode; }
			set { Set(() => SearchItemCode, ref _searchItemCode, value); }
		}
		

		public List<string> FindItems(string gupCode, string custCode, string itemCode)
		{
			var proxy = new wcf.SharedWcfServiceClient();
			var f2501 = new wcf.F2501();
			return RunWcfMethod(proxy.InnerChannel,
				() => proxy.FindItems(gupCode, custCode, itemCode, ref f2501)).ToList();

		}
	}
}
