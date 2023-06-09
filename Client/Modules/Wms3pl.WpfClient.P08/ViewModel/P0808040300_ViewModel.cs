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
	public enum BindBoxType
	{
		NormalShipBox,
		CanelOrderBox
	}
	public class P0808040300_ViewModel : InputViewModelBase
	{
		private string _gupCode;
		private string _custCode;
		public Action Close = delegate { };
		public Action BoxFocus = delegate { };

		public P0808040300_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				BoxNo = string.Empty;
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

			}

		}


		#region 綁定箱號類型
		private BindBoxType _currentbindBoxType;

		public BindBoxType CurrentBindBoxType
		{
			get { return _currentbindBoxType; }
			set
			{
				Set(() => CurrentBindBoxType, ref _currentbindBoxType, value);
				switch(value)
				{
					case BindBoxType.NormalShipBox:
						Title = Properties.Resources.P0808040300_PleaseScanCrossWarehouseContainerCode;
						BoxTypeName = Properties.Resources.P0808040300_NormalShip;
						BoxTypeColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
						break;
					case BindBoxType.CanelOrderBox:
						Title = Properties.Resources.P0808040300_PleaseScanInWarehouseContainerCode;
						BoxTypeName = Properties.Resources.P0808040300_CancelOrder;
						BoxTypeColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
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


		#region 綁定箱號類型名稱
		private string _boxTypeName;

		public string BoxTypeName
		{
			get { return _boxTypeName; }
			set
			{
				Set(() => BoxTypeName, ref _boxTypeName, value);
			}
		}
		#endregion


		#region 箱號類型文字顏色
		private SolidColorBrush _boxTypeColor;

		public SolidColorBrush BoxTypeColor
		{
			get { return _boxTypeColor; }
			set
			{
				Set(() => BoxTypeColor, ref _boxTypeColor, value);
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

		#region 容器揀貨資訊
		private ContainerPickInfo currentContainerPickInfo;

		public ContainerPickInfo CurrentContainerPickInfo
		{
			get { return currentContainerPickInfo; }
			set
			{
				Set(() => CurrentContainerPickInfo, ref currentContainerPickInfo, value);
			}
		}
		#endregion

		#region 原箱號
		private string _orgBoxNo;

		public string OrgBoxNo
		{
			get { return _orgBoxNo; }
			set
			{
				Set(() => OrgBoxNo, ref _orgBoxNo, value);
			}
		}
		#endregion


		#region 箱號
		private string _boxNo;

		public string BoxNo
		{
			get { return _boxNo; }
			set
			{
				Set(() => BoxNo, ref _boxNo, value);
			}
		}
		#endregion

		#region 是否加箱
		private bool _isAddBox;

		public bool IsAddBox
		{
			get { return _isAddBox; }
			set
			{
				Set(() => IsAddBox, ref _isAddBox, value);
			}
		}
		#endregion


		#region 箱資訊
		private BoxInfo boxInfo;

		public BoxInfo BoxInfo
		{
			get { return boxInfo; }
			set
			{
				Set(() => BoxInfo, ref boxInfo, value);
			}
		}
		#endregion


		#region ExecBindBox 綁定箱號
		/// <summary>
		/// Gets the BindBox.
		/// </summary>
		public ICommand ExecBindBoxCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoExecBindBox(), () => UserOperateMode == OperateMode.Query
);
			}
		}

		public void DoExecBindBox()
		{
			BoxNo = BoxNo.ToUpper();
			var sowType = string.Empty;
			switch (CurrentBindBoxType)
			{
				case BindBoxType.NormalShipBox:
					sowType = "01";
					break;
				case BindBoxType.CanelOrderBox:
					sowType = "02";
					break;
			}
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.BindBox(DcCode, _gupCode, _custCode, CurrentContainerPickInfo.DelvDate, CurrentContainerPickInfo.PickTime, CurrentContainerPickInfo.MoveOutTarget, sowType, BoxNo, OrgBoxNo, IsAddBox));
			DispatcherAction(() =>
			{
				if (!result.IsSuccessed)
				{
					ShowWarningMessage(result.Message);
					BoxFocus();
					return;
				}
				IsOk = true;
				BoxInfo = result.BoxInfo;
				Close();
			});
		
		}
		#endregion BindBox
	}
}
