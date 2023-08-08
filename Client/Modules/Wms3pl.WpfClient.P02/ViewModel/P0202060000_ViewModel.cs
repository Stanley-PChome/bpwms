using System;
using System.Windows.Input;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202060000_ViewModel : P0202030000_ViewModel
	{
		public Action ShowP0202060100 = delegate { };

		public P0202060000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				//InitControls();
			}

			RT_MODE = "1";
		}

		#region Command
		#region 綁容器
		public ICommand BindContainerCommand
		{
			get
			{
				var isOk = false;
				return CreateBusyAsyncCommand(
						o =>
							 isOk = BindContainer(),
						() => SelectedData?.STATUS == "3",
						o =>
						{
							if (isOk)
								ShowP0202060100();
						}
						);
			}
		}

		public bool BindContainer()
		{
			var proxy = new wcf.P02WcfServiceClient();
			var res = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.CheckCanBindContainer(SelectedData.DC_CODE, SelectedData.GUP_CODE, SelectedData.CUST_CODE, SelectedData.RT_NO));
			if (!res.IsSuccessed)
				ShowWarningMessage(res.Message);
			return res.IsSuccessed;
		}


		#endregion

		#region 容器明細
		public ICommand ContainerDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				o => { },
				//() => SelectedData?.HasRecvData == "1",
				() => true,
				o => { }
				);
			}
		}
		#endregion

		#endregion
	}
}
