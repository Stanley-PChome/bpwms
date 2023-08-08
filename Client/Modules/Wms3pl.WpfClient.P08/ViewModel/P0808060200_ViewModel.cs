using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public class P0808060200_ViewModel : InputViewModelBase
	{
		private string _gupCode;
		private string _custCode;
		public Action DoClose = delegate { };
		public Action DoContainerCodeFocus = delegate { };

		public P0808060200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				ContainerCode = string.Empty;
			}
		}

		#region 物流中心編號
		private string _dcCode;

		public string DcCode
		{
			get { return _dcCode; }
			set
			{
				Set(() => DcCode, ref _dcCode, value);
			}
		}
		#endregion

		#region 綁定箱號類型
		private BindBoxType _currentbindBoxType;

		public BindBoxType CurrentBindBoxType
		{
			get { return _currentbindBoxType; }
			set
			{
				Set(() => CurrentBindBoxType, ref _currentbindBoxType, value);
				switch (value)
				{
					case BindBoxType.NormalShipBox:
						Title = "跨庫箱號";
						break;
					case BindBoxType.CanelOrderBox:
						Title = "容器條碼";
						break;
				}
			}
		}
		#endregion

		#region 標題
		private string _title;

		public string Title
		{
			get { return _title; }
			set
			{
				Set(() => Title, ref _title, value);
			}
		}
		#endregion

		#region 箱號
		private string _containerCode;

		public string ContainerCode
		{
			get { return _containerCode; }
			set
			{
				Set(() => ContainerCode, ref _containerCode, value);
			}
		}
		#endregion

		#region 是否確定
		private bool _isOk;

		public bool IsOk
		{
			get { return _isOk; }
			set
			{
				Set(() => IsOk, ref _isOk, value);
			}
		}
		#endregion

		#region 原箱資訊
		public OutContainerInfo OriContainerInfo { get; set; }
		public BindingPickContainerInfo CurrentPickContainterInfo { get; set; }
		#endregion

		#region ExecRebindContainer 綁定箱號
		/// <summary>
		/// Gets the RebindContainer.
		/// </summary>
		public ICommand ExecRebindContainerCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoExecRebindContainer(),
						() => UserOperateMode == OperateMode.Query
				);
			}
		}

		public void DoExecRebindContainer()
		{
			if (string.IsNullOrWhiteSpace(ContainerCode))
			{
				ShowWarningMessage($"請刷入{ Title }");
				DoContainerCodeFocus();
				return;
			}
			ContainerCode = ContainerCode.ToUpper();
			var sowType = string.Empty;
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			ExecuteResult result = null;
			switch (CurrentBindBoxType)
			{
				case BindBoxType.NormalShipBox:
					result = proxy.RunWcfMethod(w => w.RebindNormalContainer(DcCode, _gupCode, _custCode, OriContainerInfo, ContainerCode, CurrentPickContainterInfo));
					break;
				case BindBoxType.CanelOrderBox:
					result = proxy.RunWcfMethod(w => w.RebindCancelContainer(DcCode, _gupCode, _custCode, OriContainerInfo, ContainerCode));
					break;
			}
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				DoContainerCodeFocus();
				return;
			}
			IsOk = true;
			DispatcherAction(() =>
			{
				DoClose();
			});
		}
		#endregion BindBox
	}
}
