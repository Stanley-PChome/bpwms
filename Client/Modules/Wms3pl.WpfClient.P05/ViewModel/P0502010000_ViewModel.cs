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
using Wms3pl.WpfClient.DataServices.F70DataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

using System.Collections.ObjectModel;
using System.Reflection;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using P05Ex = Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public partial class P0502010000_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		private string _userId;
		private string _userName;
		private string _gupCode;
		private string _custCode;
		private bool _isInit = true;

		#region 查詢
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
				RaisePropertyChanged("SelectDcCode");

			}
		}
		#endregion
		#region Form - 批次日期
		private DateTime _delvDate;
		public DateTime DelvDate
		{
			get { return _delvDate; }
			set
			{
				_delvDate = value;
				RaisePropertyChanged("DelvDate");
				DelvTimeList = GetDelvTimeList();
				if (DelvTimeList.Any())
					SelectDelvTime = DelvTimeList.First().Value;
			}
		}
		#endregion
		#region Form - 批次時段
		private List<NameValuePair<string>> _delvTimeList;
		public List<NameValuePair<string>> DelvTimeList
		{
			get { return _delvTimeList; }
			set { _delvTimeList = value; RaisePropertyChanged("DelvTimeList"); }
		}
		private string _selectDelvTime;
		public string SelectDelvTime
		{
			get { return _selectDelvTime; }
			set
			{
				_selectDelvTime = value;
				RaisePropertyChanged("SelectDelvTime");
			}
		}
		#endregion
		#region Form - 指派狀態清單
		private List<NameValuePair<string>> _pierStatusList;
		public List<NameValuePair<string>> PierStatusList
		{
			get { return _pierStatusList.OrderBy(x => x.Value).ToList(); }
			set { _pierStatusList = value; RaisePropertyChanged("PierStatusList"); }
		}

		private string _pierStatus = string.Empty;
		public string PierStatus
		{
			get { return _pierStatus; }
			set { _pierStatus = value; RaisePropertyChanged("PierStatus"); }
		}
		#endregion
		#endregion

		#region Form - 碼頭清單
		private List<NameValuePair<string>> _pierList;

		public List<NameValuePair<string>> PierList
		{
			get { return _pierList; }
			set
			{
				_pierList = value;
				RaisePropertyChanged("PierList");
			}
		}

		private string _selectPier;
		public string SelectPier
		{
			get { return _selectPier; }
			set
			{
				_selectPier = value;
				RaisePropertyChanged("SelectPier");
			}
		}
		#endregion
		#region Form - 指定碼頭Radio
		private bool _rapier = false;
		public bool RaPier
		{
			get { return _rapier; }
			set { _rapier = value; RaisePropertyChanged("RaPier"); PierTypeChange(); }
		}

		private bool _radelv = false;
		public bool RaDelv
		{
			get { return _radelv; }
			set { _radelv = value; RaisePropertyChanged("RaDelv"); PierTypeChange(); }
		}

		private int _pierType;
		public int PierType
		{
			get { return _pierType; }
			set { _pierType = value; RaisePropertyChanged("PierType"); }
		}
		#endregion
		#region Data - 資料List
		private List<P05Ex.F0513WithF1909> _dgList;
		public List<P05Ex.F0513WithF1909> DgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("DgList");
			}
		}

		private P05Ex.F0513WithF1909 _selectedData;

		public P05Ex.F0513WithF1909 SelectedData
		{
			get { return _selectedData; }
			set { _selectedData = value; RaisePropertyChanged("SelectedData");  }
		}
		#endregion
		#region Data - 配送商指定碼頭List
		private List<F1947WithF194701> _deliveryList;
		public List<F1947WithF194701> DeliveryList
		{
			get { return _deliveryList; }
			set
			{
				_deliveryList = value;
				RaisePropertyChanged("DeliveryList");
			}
		}

		private F1947WithF194701 _selectedItem;

		public F1947WithF194701 SelectedItem
		{
			get { return _selectedItem; }
			set { _selectedItem = value; RaisePropertyChanged("SelectedItem"); }
		}
		#endregion
		#endregion

		#region 函式
		public P0502010000_ViewModel()
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
			_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			GetDcCodes();
			GetPierStatusList();
			PierStatus = PierStatusList.First().Value;
			DelvDate = DateTime.Today;
			DelvTimeList = GetDelvTimeList();
			if (DelvTimeList.Any())
				SelectDelvTime = DelvTimeList.First().Value;
		}

		/// <summary>
		/// 取得物流中心DeCodeList
		/// </summary>
		private void GetDcCodes()
		{
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcCodes.Any())
			{
				SelectDcCode = DcCodes.First().Value;
			}
		}

		private void GetPierStatusList()
		{
			var list = GetBaseTableService.GetF000904List(FunctionCode, "P0502010000", "STATUS").ToList();
			list.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
			PierStatusList = list.ToList();
		}

		private void GetPierList()
		{
			if (SelectedData != null && RaPier)
			{
				// 取出F1981主檔
				var proxyF19 = GetProxy<F19Entities>();
				var result = proxyF19.F1981s.Where(x => x.DC_CODE.Equals(SelectedData.DC_CODE))
						.OrderBy(x => x.PIER_CODE)
						.Select(x => new NameValuePair<string>()
						{
							Name = x.PIER_CODE,
							Value = x.PIER_CODE
						}).ToList();

				PierList = result.ToList();
				if (PierList.Any())
					SelectPier = PierList.First().Value;

			}
		}
	
		public List<NameValuePair<string>> GetDelvTimeList()
		{
			var procFlagList = new List<string> { "1", "2" };
			var proxy = GetProxy<F05Entities>();
			var query = from o in proxy.F0513s
									where o.DC_CODE == SelectDcCode && o.GUP_CODE == _gupCode && o.CUST_CODE == _custCode
									where o.DELV_DATE == DelvDate
									orderby o.PICK_TIME
									select o;

			var list = query.Select(o => new { o.PICK_TIME, o.PROC_FLAG })
											.ToList()
											.Where(o => procFlagList.Contains(o.PROC_FLAG))
											.Select(o => new NameValuePair<string>(o.PICK_TIME, o.PICK_TIME))
											.GroupBy(o => o.Value)
											.Select(g => g.First())
											.ToList();

			list.Insert(0, new NameValuePair<string>(Resources.Resources.All, ""));
			return list;
		}

		private void PierTypeChange()
		{
			if (RaPier)
			{
				PierType = 1;
				GetPierList();
			}
		}
		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query, o => SearchComplate()
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動作
			var proxyEx = GetExProxy<P05Ex.P05ExDataSource>();
			var qry = proxyEx.CreateQuery<P05Ex.F0513WithF1909>("GetF0513WithF1909Datas")
																	.AddQueryOption("dcCode", string.Format("'{0}'", SelectDcCode))
																	.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
																	.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
																	.AddQueryExOption("delvDate", DelvDate.ToString("yyyy/MM/dd"))
																	.AddQueryOption("delvTime", string.Format("'{0}'", SelectDelvTime))
																	.AddQueryExOption("status", PierStatus).ToList();
			if (qry.Any())
			{
				DgList = qry.ToList();
				SelectedData = DgList.FirstOrDefault();
			}
			else
			{
				DgList = new List<P05Ex.F0513WithF1909>();
				ShowMessage(Messages.InfoNoData);
				return;
			}
		}

		private void SearchComplate()
		{
		}
		#endregion Search

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel()
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
		}
		#endregion Cancel

		#region Save

		private bool _isSaveOk;
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => SelectedData != null, o => SaveComplate()
					);
			}
		}

		private void DoSave()
		{
			_isSaveOk = false;
			string selPier = string.Empty;
			var Success = new MessagesStruct
				{
					Button = DialogButton.OK,
					Image = DialogImage.Warning,
					Message = Properties.Resources.P0502010000_NotSelectPier,
					Title = Resources.Resources.Information
				};
			if (ShowMessage(Messages.WarningBeforeUpdate) == DialogResponse.Yes)
			{
				//執行確認儲存動作
				if (PierType == 1)
				{
					if (!string.IsNullOrWhiteSpace(SelectPier))
						selPier = SelectPier;
					else
					{
						ShowMessage(Success);
						return;
					}
				}
				else
				{
					ShowMessage(Success);
					return;
				}
				var proxy = GetExProxy<P05Ex.P05ExDataSource>();
				var result = proxy.CreateQuery<P05Ex.ExecuteResult>("UpdatePierCode")
					.AddQueryExOption("dcCode", SelectedData.DC_CODE)
					.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
					.AddQueryExOption("custCode", SelectedData.CUST_CODE)
					.AddQueryExOption("delvDate", SelectedData.DELV_DATE.ToString("yyyy/MM/dd"))
					.AddQueryExOption("pickTime", SelectedData.PICK_TIME)
					.AddQueryExOption("allId", SelectedData.ALL_ID)
					.AddQueryExOption("takeTime", SelectedData.TAKE_TIME)
					.AddQueryExOption("pierCode", selPier).ToList();

				if (result.First().IsSuccessed)
				{
					_isSaveOk = true;
					ShowMessage(Messages.InfoUpdateSuccess);
					UserOperateMode = OperateMode.Query;
				}
				else
					ShowWarningMessage(result.First().Message);
			}
		}
		private void SaveComplate()
		{
			if (_isSaveOk)
			{
				if (SelectedData != null)
				{
					SelectDcCode = SelectedData.DC_CODE;
					DelvDate = SelectedData.DELV_DATE;
					PierStatus = "";
				}
				DoSearch();
			}
		}
		#endregion Save
		#endregion
	}
}
