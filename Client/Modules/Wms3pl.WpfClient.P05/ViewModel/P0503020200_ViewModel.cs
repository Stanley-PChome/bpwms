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
using Wms3pl.WpfClient.DataServices.F05DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.ExDataServices;
using wcf = Wms3pl.WpfClient.ExDataServices.P05WcfService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Text.RegularExpressions;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.DataServices.F00DataService;
using System.IO;
using Wms3pl.WpfClient.Services;
using System.Data;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public partial class P0503020200_ViewModel : InputViewModelBase
	{
		private string _userId;
		private string _userName;
		private string _gupCode;
		private string _custCode;
		public Action AddAction = delegate { };
		public Action EditAction = delegate { };
		public Action SearchAction = delegate { };
		public Action DeleteAction = delegate { };
		public Action ExitClick = delegate { };
		public P0503020200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
				InitControls();
			}
		}

		private void InitControls()
		{

			
		}

		private string _datacount;
		public string DATA_COUNT
		{
			get { return _datacount; }
			set { _datacount = value; RaisePropertyChanged("DATA_COUNT"); }
		}

		private string _gupcode;
		public string GUP_CODE
		{
			get { return _gupcode; }
			set { _gupcode = value; RaisePropertyChanged("GUP_CODE"); }
		}

		private string _dccode;
		public string DC_CODE
		{
			get { return _dccode; }
			set { _dccode = value; RaisePropertyChanged("DC_CODE"); }
		}

		private string _custcode;
		public string CUST_CODE
		{
			get { return _custcode; }
			set { _custcode = value; RaisePropertyChanged("CUST_CODE"); }
		}

		private string _ordno;
		public string ORD_NO
		{
			get { return _ordno; }
			set { _ordno = value; RaisePropertyChanged("ORD_NO"); }
		}

		private List<F05010101> _xlsData;
		public List<F05010101> XLS_DATA
		{
			get { return _xlsData; }
			set { _xlsData = value; RaisePropertyChanged("XLS_DATA"); }
		}

		#region 小單
		private List<F05010101> _dgsmallticketlist;
		public List<F05010101> dgSmallTicketList
		{
			get { return _dgsmallticketlist; }
			set
			{
				_dgsmallticketlist = value;
				RaisePropertyChanged("dgSmallTicketList");
			}
		}
		#endregion



		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => DoSearch(),
				  () => UserOperateMode == OperateMode.Query,
				  o => DoSearchCompleted()
				  );
			}
		}


		private void DoSearch()
		{
			if (XLS_DATA==null )
			{
				var proxy = GetProxy<F05Entities>();
				dgSmallTicketList = proxy.F05010101s.Where(x => x.GUP_CODE == GUP_CODE && x.CUST_CODE == CUST_CODE && x.DC_CODE == DC_CODE && x.ORD_NO == ORD_NO).ToList();
				DATA_COUNT = dgSmallTicketList.Count.ToString();
			}
			else 
			{
				int i = 1;
				foreach (var d in XLS_DATA)
				{
					d.SMALL_ORD_SEQ = i;
					i++;
				}
				dgSmallTicketList = XLS_DATA;
				DATA_COUNT = dgSmallTicketList.Count.ToString();
			}
		}

		private void DoSearchCompleted()
		{
		
		}
		#endregion Search

		#region Exit
		public ICommand ExitCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoExit(),
					() => UserOperateMode == OperateMode.Query,
					o => DoExitCompleted()
					);
			}
		}

		private void DoExit()
		{
			//執行刪除動作
		}

		private void DoExitCompleted()
		{
			ExitClick();
		}
		#endregion











	}
}
