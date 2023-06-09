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
using Wms3pl.WpfClient.UILib;
using System.Windows.Controls;
using Wms3pl.Common.Security;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0807010200_ViewModel : InputViewModelBase
	{
		public Action OnUnlockComplete = delegate { };
		public P0807010200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}

		#region 資料連結
		public string SelectedDc { get; set; }
		public string SelectedGup { get; set; }
		public string SelectedCust { get; set; }
		private string _adminId = string.Empty;
		public string AdminId
		{
			get { return _adminId; }
			set { _adminId = value; RaisePropertyChanged("AdminId"); }
		}
		public bool unlocked = false;
		#endregion

		#region Unlock
		public ICommand UnlockCommand
		{
			get
			{
				return new RelayCommand<object[]>(async p =>
					{
						IsBusy = true;
						var objPwd = (PasswordBox)p[0];
						var objUnlockCode = (PasswordBox)p[1];
						await Task.Run(() => DoUnlock(objPwd.Password, objUnlockCode.Password));
						IsBusy = false;
						DoUnlockComplete();
					},
						p => !IsBusy);
			}
		}

		private void DoUnlock(string adminPassword, string unlockCode)
		{
			var proxy = GetProxy<F19Entities>();
			var proxyF00 = GetProxy<F00Entities>();
			unlocked = false;
			// 找出USER
			var tmp = proxy.F1924s.Where(x => x.EMP_ID == AdminId && x.ISDELETED == "0").FirstOrDefault();
			if (tmp == null)
			{
				ShowMessage(Messages.InfoInvalidUserId);
				return;
			}
			// 判斷有無解鎖權限
			if (tmp.PACKAGE_UNLOCK == "0")
			{
				ShowMessage(Messages.InfoInvalidUnlockPermission);
				return;
			}
			// 比對PASSWORD
			var tmp2 = proxy.F1952s.Where(x => x.EMP_ID == AdminId).FirstOrDefault();
#if DEBUG
			if (tmp2 == null && AdminId.ToUpper() != "WMS")
			{
				ShowMessage(Messages.InfoInvalidPassword);
				return;
			}
#else
			if (tmp2 == null || !CryptoUtility.CompareHash(adminPassword, tmp2.PASSWORD))
			{
				ShowMessage(Messages.InfoInvalidPassword);
				return;
			}
#endif
			// 比對解鎖碼
			var tmp3 = proxyF00.F0003s.Where(x => x.DC_CODE == SelectedDc && x.GUP_CODE == SelectedGup && x.CUST_CODE == SelectedCust && x.AP_NAME == "PackageLockPW").FirstOrDefault();
			if (!CryptoUtility.CompareHash(unlockCode, tmp3.SYS_PATH))
			{
				ShowMessage(Messages.InfoInvalidUnlockCode);
				return;
			}
			unlocked = true;
			return;
		}
		private void DoUnlockComplete()
		{
			OnUnlockComplete();
		}
		#endregion Search
	}
}
