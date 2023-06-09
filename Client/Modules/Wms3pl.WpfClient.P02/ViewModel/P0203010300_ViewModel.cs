using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0203010300_ViewModel : InputViewModelBase
	{
		public P0203010300_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				IsSaveOk = false;
			}

		}

		#region Property

		public Action ExitClick = delegate { };
		public Action SetDefaultFocus = delegate { };
		public bool IsSaveOk;
		public string GupCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
		}

		public string CustCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
		}
		
		#region 品號
		private string _itemCode;

		public string ItemCode
		{
			get { return _itemCode; }
			set
			{
				if (_itemCode == value)
					return;
				Set(() => ItemCode, ref _itemCode, value);
			}
		}
		#endregion


		#region 品名
		private string _itemName;

		public string ItemName
		{
			get { return _itemName; }
			set
			{
				if (_itemName == value)
					return;
				Set(() => ItemName, ref _itemName, value);
			}
		}
		#endregion
		
		#endregion

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoSave()
		{
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Error,
				Message = "",
				Title = Properties.Resources.Message
			};
			if (string.IsNullOrEmpty(ItemCode))
				message.Message = Properties.Resources.P0203010300_ItemCodeIsNull;
			else
			{
				//執行確認儲存動作
				var proxy = GetProxy<F19Entities>();
				var item = proxy.F1903s.Where(o => o.GUP_CODE == GupCode && o.ITEM_CODE == ItemCode && o.CUST_CODE == CustCode).ToList().FirstOrDefault();
				if (item == null)
					message.Message = Properties.Resources.P0203010300_ItemCodeNotExist;
				else
				{
					var item1 =
						proxy.F1903s.Where(o => o.GUP_CODE == GupCode && o.CUST_CODE == CustCode && o.ITEM_CODE == ItemCode)
							.ToList()
							.FirstOrDefault();
					if (item1 == null)
						message.Message = Properties.Resources.P0203010300_ItemCodeNotExist;
					else if (item1.BUNDLE_SERIALLOC == "0")
						message.Message = Properties.Resources.P0203010300_BundleSerialloc;
					else
						ItemName = item.ITEM_NAME;
				}
			}
			if (!string.IsNullOrEmpty(message.Message))
			{
				SetDefaultFocus();
				ShowMessage(message);
			}
			else
			{

				IsSaveOk = true;
				ExitClick();
			}
				
			UserOperateMode = OperateMode.Query;
		}
		#endregion Save
	}
}
