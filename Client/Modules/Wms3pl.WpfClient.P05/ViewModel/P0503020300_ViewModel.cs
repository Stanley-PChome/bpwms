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
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public class P0503020300_ViewModel : InputViewModelBase
	{
		public P0503020300_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}
		public Action ExitClick = delegate { };

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

		private ObservableCollection<F05010102OSerialNo> _dgSerialNoData;
		public ObservableCollection<F05010102OSerialNo> dgSerialNoData
		{
			get { return _dgSerialNoData; }
			set { Set(ref _dgSerialNoData, value); }
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query
					);
			}
		}


		private void DoSearch()
		{
			//執行查詢動作
			P05ExDataSource proxyEx = GetExProxy<P05ExDataSource>();
			dgSerialNoData = proxyEx.CreateQuery<F05010102OSerialNo>("GetF05010102SerialNoData")
														.AddQueryOption("dcCode", string.Format("'{0}'", DC_CODE))
														.AddQueryOption("gupCode", string.Format("'{0}'", GUP_CODE))
														.AddQueryOption("custCode", string.Format("'{0}'", CUST_CODE))
														.AddQueryOption("ordNo",string.Format("'{0}'",ORD_NO)).ToObservableCollection();
			
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
