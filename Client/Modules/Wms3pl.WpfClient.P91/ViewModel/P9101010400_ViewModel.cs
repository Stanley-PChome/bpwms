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
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P91WcfService;

namespace Wms3pl.WpfClient.P91.ViewModel
{
	public partial class P9101010400_ViewModel : InputViewModelBase
	{
		public P9101010400_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}

		#region 資料連結
		private F910201 _baseData = null;
		public F910201 BaseData
		{
			get { return _baseData; }
			set { _baseData = value; RaisePropertyChanged("BaseData"); }
		}

		private int? _nowProcessQty = 0;
		public int? NowProcessQty
		{
			get { return _nowProcessQty; }
			set { _nowProcessQty = value; RaisePropertyChanged("NowProcessQty"); }
		}


		private int? _aProcessQty = 0;
		public int? AProcessQty
		{
			get { return _aProcessQty; }
			set { _aProcessQty = value; RaisePropertyChanged("AProcessQty"); }
		}

		private int? _breakQty = 0;
		public int? BreakQty
		{
			get { return _breakQty; }
			set { _breakQty = value; RaisePropertyChanged("BreakQty"); }
		}

		private short _unfinishedQty = 0;
		public short UnfinishedQty
		{
			get { return _unfinishedQty; }
			set { _unfinishedQty = value; RaisePropertyChanged("UnfinishedQty"); }
		}
		#endregion


		#region Function

		public void GetMinProcessQty()
		{
			//執行查詢動作

			var proxyEx = GetExProxy<P91ExDataSource>();
			// 3. 查詢已設定的回倉明細
			var tmpBackList = proxyEx.CreateQuery<int>("GetfinishedItems")
				.AddQueryOption("dcCode", string.Format("'{0}'", BaseData.DC_CODE))
				.AddQueryOption("gupCode", string.Format("'{0}'", BaseData.GUP_CODE))
				.AddQueryOption("custCode", string.Format("'{0}'", BaseData.CUST_CODE))
				.AddQueryOption("processNo", string.Format("'{0}'", BaseData.PROCESS_NO))
				.ToList();
			if (tmpBackList.Count > 0)
			{
				NowProcessQty = tmpBackList[0];
				AProcessQty = NowProcessQty;
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
					o => DoSearch()
					);
			}
		}

		private void DoSearch()
		{

		}
		#endregion Search

		#region Save
		private bool _isSaved = false;
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => BaseData != null && BaseData.STATUS.CompareTo("1") <= 0,
					o => DoSaveComplete()
				);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gtStatus">大於等於這個狀態就不能修改</param>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool CheckCanEdit(string gtStatus, string message)
		{
			var f910201 = FindByKey<F910201>(BaseData);
			if (f910201.STATUS.CompareTo(gtStatus) > 0)
			{
				ShowWarningMessage(message);
				BaseData = f910201;
				return false;
			}

			return true;
		}

		private void DoSave()
		{
			if (!CheckCanEdit("1", Properties.Resources.P9101010000xamlcs_Process_Status_CantEdit))
				return;

			_isSaved = false;
			if (!DoCheck())
				return;
			var proxyEx = GetExProxy<P91ExDataSource>();
			var results = proxyEx.CreateQuery<ExecuteResult>("FinishProcess")
				.AddQueryExOption("processNo", BaseData.PROCESS_NO)
				.AddQueryExOption("gupCode", BaseData.GUP_CODE)
				.AddQueryExOption("custCode", BaseData.CUST_CODE)
				.AddQueryExOption("dcCode", BaseData.DC_CODE)
				.AddQueryExOption("aProcessQty", AProcessQty ?? 0)
				.AddQueryExOption("breakQty", BreakQty ?? 0)
				.AddQueryExOption("memo", BaseData.MEMO).ToList();
			var result = results.Where(a => !a.IsSuccessed).FirstOrDefault();
			if (result != null)
			{
				ShowResultMessage(result);
			}
			else
			{
				ShowMessage(Messages.Success2);
				_isSaved = true;
			}
		}

		private void DoSaveComplete()
		{
			if (!_isSaved) return;
			var proxy = GetProxy<F91Entities>();
			var data = proxy.F910201s.Where(x => x.DC_CODE == BaseData.DC_CODE && x.GUP_CODE == BaseData.GUP_CODE && x.CUST_CODE == BaseData.CUST_CODE && x.PROCESS_NO == BaseData.PROCESS_NO).FirstOrDefault();
			BaseData = data;
			_isSaved = false;
		}

		private int GetF910204Count()
		{
			var f91Proxy = GetProxy<F91Entities>();
			var f910204Count = f91Proxy.F910204s.Where(x => x.DC_CODE == BaseData.DC_CODE
														&& x.GUP_CODE == BaseData.GUP_CODE
														&& x.CUST_CODE == BaseData.CUST_CODE
														&& x.PROCESS_NO == BaseData.PROCESS_NO).Count();
			return f910204Count;
		}

		private bool DoCheck()
		{
			if (AProcessQty == null || AProcessQty < 0 || BreakQty == null || BreakQty < 0)
			{
				DialogService.ShowMessage(Properties.Resources.P9101010400_ViewModel_InputCorrectQty);
				return false;
			}

			// 若沒有任何動作，則可以不用判斷實際加工數。
			var hasAction = GetF910204Count() > 0;
			if (hasAction)
			{
				if (AProcessQty + BreakQty > NowProcessQty)
				{
					DialogService.ShowMessage(string.Format(Properties.Resources.P9101010400_ViewModel_ProcessQtyGreaterThanRealProcessQty, AProcessQty + BreakQty, NowProcessQty));
					return false;
				}
			}

			if (AProcessQty + BreakQty + BaseData.A_PROCESS_QTY + BaseData.BREAK_QTY > BaseData.PROCESS_QTY)
			{
				DialogService.ShowMessage(string.Format(Properties.Resources.P9101010400_ViewModel_ProcessQtyAndAccumulateQtyGreaterThanRealProcessQty
														, AProcessQty + BreakQty
														, BaseData.A_PROCESS_QTY + BaseData.BREAK_QTY
														, BaseData.PROCESS_QTY));
				return false;
			}


			return true;
		}
		#endregion Save

		#endregion
	}
}
