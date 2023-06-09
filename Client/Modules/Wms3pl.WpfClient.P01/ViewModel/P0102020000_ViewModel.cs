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
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.ExDataServices.P01ExDataService;
using Wms3pl.WpfClient.UILib;
//using P010202SearchResult = Wms3pl.WpfClient.ExDataServices.P01ExDataService.P010202SearchResult;

namespace Wms3pl.WpfClient.P01.ViewModel
{
	public partial class P0102020000_ViewModel : InputViewModelBase
	{
		public P0102020000_ViewModel()
		{
			Init();
		}

		private void Init()
		{
			//物流中心
			DcCodes = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList);

			if (DcCodes.Count > 0) DcCode = DcCodes[0].Value;
			ChangeDateBegin = DateTime.Today;
			ChangeDateEnd = DateTime.Today;
		}

        #region 業主

        public string GupCode
        {
            get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
        }

        #endregion

        #region 貨主

        public string CustCode
        {
            get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
        }

        #endregion

        #region 查詢Command
        public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete()
					);
			}
		}

		private void DoSearch()
		{
			//檢查查詢條件
			if (!ChangeDateBegin.HasValue || !ChangeDateEnd.HasValue)
			{
				ShowWarningMessage(Properties.Resources.P010202_ChangeStartEndDateIsNull);
				return;
			}

			if (ChangeDateBegin.Value > ChangeDateEnd.Value)
			{
				DateTime temp1 = ChangeDateBegin.Value;
				ChangeDateBegin = ChangeDateEnd.Value;
				ChangeDateEnd = temp1;
			}

            if (new TimeSpan(ChangeDateEnd.Value.Ticks - ChangeDateBegin.Value.Ticks).Days > 90)
            {
                DialogService.ShowMessage(Properties.Resources.P010202_QueryIntervalDateOver);
                return;
            }

            SearchResults = GetF010202();

            if (SearchResults == null || !SearchResults.Any())
			{
				ShowMessage(Messages.InfoNoData);
			}
		}

		private List<P010202SearchResult> GetF010202()
		{
			var proxy = GetExProxy<P01ExDataSource>();
			return proxy.CreateQuery<P010202SearchResult>("GetF010202")
							.AddQueryOption("gupCode", string.Format("'{0}'", GupCode))
							.AddQueryOption("custCode", string.Format("'{0}'", CustCode))
							.AddQueryOption("dcCode", string.Format("'{0}'", DcCode))
							.AddQueryOption("changeDateBegin", string.Format("'{0}'", ChangeDateBegin.Value.ToString("yyyy/MM/dd")))
							.AddQueryOption("changeDateEnd", string.Format("'{0}'", ChangeDateEnd.Value.ToString("yyyy/MM/dd"))).ToList();
		}


		private void DoSearchComplete()
		{			
			UserOperateMode = OperateMode.Query;
		}
		#endregion

		#region 資料來源
		//物流中心
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

		private List<P010202SearchResult> _searchResults;

		public List<P010202SearchResult> SearchResults
		{
			get { return _searchResults; }
			set
			{
				_searchResults = value;
				RaisePropertyChanged("SearchResults");
			}
		}
		#endregion

		#region 查詢條件
		private string _dcCode = String.Empty;

		public string DcCode
		{
			get { return _dcCode; }
			set
			{
				_dcCode = value;
				RaisePropertyChanged("DcCode");				
			}
		}

		private DateTime? _changeDateBegin;

		public DateTime? ChangeDateBegin
		{
			get { return _changeDateBegin; }
			set
			{
				_changeDateBegin = value;
				RaisePropertyChanged("ChangeDateBegin");
			}
		}

		private DateTime? _changeDateEnd;

		public DateTime? ChangeDateEnd
		{
			get { return _changeDateEnd; }
			set
			{
				_changeDateEnd = value;
				RaisePropertyChanged("ChangeDateEnd");
			}
		}

		#endregion
	}
}
