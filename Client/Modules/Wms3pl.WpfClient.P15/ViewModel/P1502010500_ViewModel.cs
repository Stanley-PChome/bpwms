using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P15ExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P15WcfService;

namespace Wms3pl.WpfClient.P15.ViewModel
{
    public class P1502010500_ViewModel : InputViewModelBase
    {
    /// <summary>
    /// 是否有輸入缺貨再關閉視窗
    /// </summary>
    public Action<Boolean> OnClose = delegate { };

        public P1502010500_ViewModel()
        {
            if (!IsInDesignMode)
            {

            }
        }

        public void DoSearchInitData(string DcCode, string GupCode, string CustCode, string AllocationNo)
        {
            var proxy = GetExProxy<P15ExDataSource>();
            var result = proxy.CreateQuery<P1502010500Data>("GetP1502010500Data")
                                              .AddQueryExOption("dcCode", DcCode)
                                              .AddQueryExOption("gupCode", GupCode)
                                              .AddQueryExOption("custCode", CustCode)
                                              .AddQueryExOption("allocationNo", AllocationNo)
                                              .ToObservableCollection();
            if (!result?.Any() ?? true)
            {
                //ShowWarningMessage("查無資料");
                //OnClose(false);
                return;
            }

            P1502010500Datas = result;
            this.SrcDcCodeName = result.First().SRC_DC_CODE_NAME;
            this.SrcWarehouseName = result.First().SRC_WAREHOUSE_NAME;
            this.AllocationNo = result.First().ALLOCATION_NO;
        }

        private ObservableCollection<P1502010500Data> _P1502010500Datas;
        public ObservableCollection<P1502010500Data> P1502010500Datas
        {
            get { return _P1502010500Datas; }
            set { _P1502010500Datas = value; RaisePropertyChanged(); }
        }


        private P1502010500Data _SelectedP1502010500Datas;
        public P1502010500Data SelectedP1502010500Datas
        {
            get { return _SelectedP1502010500Datas; }
            set { _SelectedP1502010500Datas = value; RaisePropertyChanged(); }
        }

        private string _AllocationNo;
        public string AllocationNo
        {
            get { return _AllocationNo; }
            set
            {
                _AllocationNo = value;
                RaisePropertyChanged();
            }
        }

        private string _SrcWarehouseName;
        public string SrcWarehouseName
        {
            get { return _SrcWarehouseName; }
            set
            {
                _SrcWarehouseName = value;
                RaisePropertyChanged();
            }
        }

        private string _SrcDcCodeName;
        public string SrcDcCodeName
        {
            get { return _SrcDcCodeName; }
            set
            {
                _SrcDcCodeName = value;
                RaisePropertyChanged();
            }
        }

    /// <summary>
    /// 是否已過帳
    /// </summary>
    public Boolean IsPosted;

    public ICommand SaveCommand
    {
      get
      {
        wcf.ExecuteResult result = new wcf.ExecuteResult();
        return CreateBusyAsyncCommand(
          o =>
          {
            if (CheckLackQty())
            {
              result.IsSuccessed = false;
              return;
            }
            result.IsSuccessed = true;
            var proxy = GetWcfProxy<wcf.P15WcfServiceClient>();
            var wcfReq = P1502010500Datas.MapCollection<P1502010500Data, wcf.P1502010500Data>().ToArray();
            result = proxy.RunWcfMethod(w => w.FinishedOffShelfWithLack(wcfReq.First().DC_CODE, wcfReq.First().GUP_CODE, wcfReq.First().CUST_CODE, wcfReq.First().ALLOCATION_NO, wcfReq));
            ShowResultMessage(result);
            IsPosted = result.No == "TRUE";
          },
          () => true,
          o =>
          {
            if (result.IsSuccessed)
              OnClose(true);
          });
      }
    }

        public ICommand CancelCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                       o => { },
                       () => true,
                       o =>
                       {
                           if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
                               OnClose(false);
                       });
            }
        }

        /// <summary>
        /// 檢查輸入的缺貨數是否有異常，給null就檢查datagrid全部內容
        /// </summary>
        /// <returns></returns>
        public Boolean CheckLackQty(int? LACK_QTY = null)
        {
            if (LACK_QTY == null)
            {
                foreach (var item in P1502010500Datas)
                {
                    if (item.SRC_QTY < item.LACK_QTY)
                    {
                        ShowWarningMessage("缺貨數不可大於預計下架數");
                        return true;
                    }
                }
                if(P1502010500Datas.All(x=>x.LACK_QTY == 0))
                {
                    ShowWarningMessage("未輸入缺貨數");
                    return true;
                }
            }
            else
            {
                if (SelectedP1502010500Datas.SRC_QTY < LACK_QTY)
                {
                    ShowWarningMessage("缺貨數不可大於預計下架數");
                    return true;
                }
            }
            return false;
        }
    }
}
