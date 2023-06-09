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

using System.Collections.ObjectModel;
using System.Reflection;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P16.ViewModel
{
	public partial class P1601030000_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		private string _userId;
		private string _userName;

		#region Form - 物流中心
		private List<NameValuePair<string>> _dcCodes;

		public List<NameValuePair<string>> DcCodes
		{
			get { return _dcCodes; }
			set
			{
				_dcCodes = value;
				RaisePropertyChanged("DcCodes");
			}
		}

		private string _selectDcCode;
		public string SelectDcCode
		{
			get { return _selectDcCode; }
			set
			{
				_selectDcCode = value;
				DgList = new List<DgDataClass>();
				RaisePropertyChanged("SelectDcCode");
			}
		}
		#endregion
		#region Data - 資料List
		private List<DgDataClass> _dgList;
		public List<DgDataClass> DgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("DgList");
			}
		}

		private DgDataClass _selectedData;

		public DgDataClass SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged("SelectedData");
			}
		}
		#endregion
		#region Data - 資料格式
		public class DgDataClass
		{
			public string DC_CODE { get; set; }
			public int HELP_NO { get; set; }
			public string HELP_TYPE { get; set; }
			public string ORD_NO { get; set; }
			public string STATUS { get; set; }
			public string STATUS_DESC { get; set; }
			public string CREATE { get; set; }
			public string HELP_NAME { get; set; }
			public DateTime CRT_DATE { get; set; }
		}
		#endregion
		#endregion

		#region 函式
		public P1601030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			_userId = Wms3plSession.Get<UserInfo>().Account;
			_userName = Wms3plSession.Get<UserInfo>().AccountName;
			GetDcCodeList();
		}

		private void GetDcCodeList()
		{
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcCodes.Any()) SelectDcCode = DcCodes.FirstOrDefault().Value;
		}
		#endregion

		#region Command
		#region Search

		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						if (!DgList.Any())
						{
							ShowMessage(Messages.InfoNoData);
						}
					});
			}
		}

		private void DoSearch()
		{
			//執行查詢動
			var proxy = GetProxy<F00Entities>();
			var qry = from a in proxy.F0010s.ToList()
					  join b in proxy.F001001s.ToList()
						  on a.HELP_TYPE equals b.HELP_TYPE into d
					  from c in d.DefaultIfEmpty()
					  where a.DC_CODE.Equals(SelectDcCode)
						   && a.STATUS == "0"
						   && (a.HELP_TYPE == "03" || a.HELP_TYPE == "04" || a.HELP_TYPE == "05")
					  orderby a.CRT_DATE
					  select new DgDataClass
								 {
									 DC_CODE = a.DC_CODE,
									 HELP_NO = a.HELP_NO,
									 HELP_TYPE = a.HELP_TYPE,
									 ORD_NO = a.ORD_NO,
									 STATUS = a.STATUS,
									 STATUS_DESC = a.STATUS == "0" ? Properties.Resources.P1601010000_Pending : Properties.Resources.P1601010100xamlcs_Processed,
									 CREATE = a.CRT_STAFF + " " + a.CRT_NAME,
									 HELP_NAME = c.HELP_NAME,
									 CRT_DATE = a.CRT_DATE
								 };

			DgList = qry.ToList();
		}

		#endregion Search

		#region Edit

		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query, o => EditComplate()
					);
			}
		}

		private void DoEdit()
		{
			if (SelectedData != null)
			{
				var proxy = GetModifyQueryProxy<F00Entities>();
				var f0010s = proxy.F0010s.Where(x => x.DC_CODE == SelectedData.DC_CODE
																							&& x.HELP_NO == SelectedData.HELP_NO).AsQueryable();


				var f0010 = f0010s.FirstOrDefault();
				f0010.STATUS = "1";
				f0010.UPD_DATE = DateTime.Now;
				f0010.UPD_STAFF = _userId;
				f0010.UPD_NAME = _userName;
				proxy.UpdateObject(f0010);
				proxy.SaveChanges();
			}
		}

		private void EditComplate()
		{
			DoSearch();
		}
		#endregion Edit
		#endregion
	}
}
