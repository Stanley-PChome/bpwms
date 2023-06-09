using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.ViewModel
{
    public partial class P0814010300_ViewModel : InputViewModelBase
    {
        public Action OnSetFocuseContainerCode = delegate { };
        public Action<Boolean> OnWindowsClose = delegate { };
        public P0814010300_ViewModel()
        {
            if (!IsInDesignMode)
            {
            }
        }

        /// <summary>
        /// 資料初始化
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="WmsOrdNo">出貨單號</param>
        /// <param name="ContainerCode">容器條碼</param>
        public void DataInit(String dcCode, String gupCode, String custCode, String WmsOrdNo, String ContainerCode)
        {
            var req = new GetShipLogisticBoxReq()
            {
                DcCode = dcCode,
                GupCode = gupCode,
                CustCode = custCode,
                WmsOrdNo = WmsOrdNo
            };
            var wcfproxy = GetWcfProxy<P08WcfServiceClient>();
            var result = wcfproxy.RunWcfMethod(w => w.GetShipLogisticBox(req));
            if (!result.IsSuccessed)
            {
                ShowWarningMessage("找不到出貨單容器資料");
                OnWindowsClose(false);
                return;
            }

            ContainerList = result.Datas.ToObservableCollection();
            foreach (var item in ContainerList)
                item.IsScan = item.ContainerCode == ContainerCode;
            ContainerList = ContainerList;
        }


        #region UI屬性

        #region 容器箱數
        /// <summary>
        /// 容器箱數
        /// </summary>
        public int ContainerQty
        {
            get { return ContainerList.Count; }
        }
        #endregion 容器箱數

        #region 容器條碼
        private string _ContainerCode;
        /// <summary>
        /// 容器條碼
        /// </summary>
        public string ContainerCode
        {
            get { return _ContainerCode; }
            set { _ContainerCode = value; RaisePropertyChanged(); }
        }
        #endregion 容器條碼
        private ObservableCollection<GetShipLogisticBoxData> _ContainerList;
        public ObservableCollection<GetShipLogisticBoxData> ContainerList
        {
            get { return _ContainerList; }
            set { _ContainerList = value; RaisePropertyChanged(); }
        }
        #region 容器條碼清單

        #endregion 容器條碼清單

        #endregion UI屬性

        #region ICommand

        #region 確認Command
        public ICommand CheckCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { },
                    () => true,
                    o => { DoCheck(); }
                    );
            }
        }

        private Boolean DoCheck()
        {
            /*
            刷讀容器條碼按下Enter/輸入容器條碼按下確認:
            1. 檢查容器條碼是有值，若無值顯示[請刷讀容器條碼]
            2. 將容器條碼值轉大寫後往下執行
            3. 檢查刷入的容器條碼是否在[A]資料中，如果是，更新IsScan=是
            4. 若[A]還有一筆容器條碼IsScan=否，容器條碼Focus
            5. 若[A]所有容器條碼IsScan=是，關閉視窗，並回傳[true]
            */
            if (String.IsNullOrWhiteSpace(ContainerCode))
            {
                ShowWarningMessage("請刷讀容器條碼");
                return false;
            }
            ContainerCode = ContainerCode.ToUpper();
            var finder = ContainerList.Where(x => x.ContainerCode == ContainerCode);
            foreach (var item in finder)
                item.IsScan = true;
            ContainerList = ContainerList;
            if (ContainerList.Any(x => !x.IsScan))
                OnSetFocuseContainerCode();
            else
                OnWindowsClose(true);

            return true;
        }
        #endregion 確認Command

        #region 離開Command
        public ICommand ExitCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { },
                    () => true,
                    o => { OnWindowsClose(!ContainerList.Any(x => !x.IsScan)); })
                    ;
            }
        }
        #endregion 離開Command

        #endregion ICommand
    }

    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                var b = (bool)value;
                return b ? "是" : "否";
            }
            throw new Exception("Value is not Type of Boolean !");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibility = value.ToString();
            if (visibility == "是")
                return true;
            else if (visibility == "否")
                return false;
            throw new Exception("Value is not Type of string !");
        }
    }

}
