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
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.ExDataServices;
using Wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;
namespace Wms3pl.WpfClient.P08.ViewModel
{
	public class P0802010300_ViewModel : InputViewModelBase
	{
		#region Property
		private ReturnDetailSummary _oldReturnDetailSummary;
		public Action ReDataGridFocus = delegate { };

		#region 物流中心
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


		#region 業主
		private string _gupCode;

		public string GupCode
		{
			get { return _gupCode; }
			set
			{
				Set(() => GupCode, ref _gupCode, value);
			}
		}
		#endregion


		#region 貨主
		private string _custCode;

		public string CustCode
		{
			get { return _custCode; }
			set
			{
				Set(() => CustCode, ref _custCode, value);
			}
		}
		#endregion


		#region 退貨單號
		private string _returnNo;

		public string ReturnNo
		{
			get { return _returnNo; }
			set
			{
				Set(() => ReturnNo, ref _returnNo, value);
			}
		}
		#endregion




		#region 退貨身檔明細資料
		private List<ReturnDetailSummary> _returnDetailList;

		public List<ReturnDetailSummary> ReturnDetailList
		{
			get { return _returnDetailList; }
			set
			{
				Set(() => ReturnDetailList, ref _returnDetailList, value);
			}
		}
		#endregion


		#region 選取的退貨身檔明細
		private ReturnDetailSummary _selectedReturnDetail;

		public ReturnDetailSummary SelectedReturnDetail
		{
			get { return _selectedReturnDetail; }
			set
			{
				_oldReturnDetailSummary = value.Clone();
				if (_selectedReturnDetail != null)
					_selectedReturnDetail.PropertyChanged -= SelectedReturnDetail_PropertyChanged; //先移除PropertyChanged的Event Handler，避免重複呼叫
				Set(() => SelectedReturnDetail, ref _selectedReturnDetail, value);
				if (_selectedReturnDetail != null)
					_selectedReturnDetail.PropertyChanged += SelectedReturnDetail_PropertyChanged; //增加PropertyChanged 的Event Handler
			}
		}
		#endregion


		#endregion

		#region Contructor
		public P0802010300_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}
		#endregion

		#region Method

		public void Init(string dcCode, string gupCode, string custCode, string returnNo)
		{
			DcCode = dcCode;
			GupCode = gupCode;
			CustCode = custCode;
			ReturnNo = returnNo;
			SearchCommand.Execute(null);
		}

		#endregion

		#region Event


		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => true
					);
			}
		}

		private void DoSearch()
		{
			var proxy = GetExProxy<P08ExDataSource>();
			ReturnDetailList = proxy.CreateQuery<ReturnDetailSummary>("GetReturnDetailSummary")
				.AddQueryExOption("dcCode", DcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("returnNo", ReturnNo).ToList();
			if (ReturnDetailList.Any())
				SelectedReturnDetail = ReturnDetailList.First();
		}
		#endregion Search

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
			var data = ExDataMapper.MapCollection<ReturnDetailSummary, Wcf.ReturnDetailSummary>(ReturnDetailList).ToArray();
			var wcfProxy = GetWcfProxy<Wcf.P08WcfServiceClient>();
			var result = wcfProxy.RunWcfMethod(w => w.UpdateF16140201(DcCode, GupCode, CustCode, ReturnNo, data));
			ShowResultMessage(result);
			SearchCommand.Execute(null);
		}
		#endregion Save


		private void SelectedReturnDetail_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_selectedReturnDetail.PropertyChanged -= SelectedReturnDetail_PropertyChanged;
			switch (e.PropertyName)
			{
				case nameof(SelectedReturnDetail.BAD_QTY):
					if (SelectedReturnDetail.BAD_QTY > SelectedReturnDetail.AUDIT_QTY)
					{
						SelectedReturnDetail.BAD_QTY = _oldReturnDetailSummary.BAD_QTY;
						ShowWarningMessage(string.Format(Properties.Resources.P0802010300_BadQtyOverError, SelectedReturnDetail.AUDIT_QTY));
						ReDataGridFocus();
					}
					else if (SelectedReturnDetail.BAD_QTY < 0)
					{
						SelectedReturnDetail.BAD_QTY = _oldReturnDetailSummary.BAD_QTY;
						ShowWarningMessage(Properties.Resources.P0802010300_BadQtyZeroError);
						ReDataGridFocus();
					}
					else
						SelectedReturnDetail.GOOD_QTY = SelectedReturnDetail.AUDIT_QTY - SelectedReturnDetail.BAD_QTY;
					break;
			}
			_selectedReturnDetail.PropertyChanged += SelectedReturnDetail_PropertyChanged;
		}
		#endregion
	}
}
