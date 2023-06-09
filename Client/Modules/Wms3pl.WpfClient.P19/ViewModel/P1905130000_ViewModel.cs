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
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
    public class P1905130000_ViewModel : InputViewModelBase
    {
        #region 建構子

        public P1905130000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                //初始化執行時所需的值及資料
                DoSearch();
            }
        }

        #endregion

        #region 屬性

        /// <summary>
        /// 查詢回來存取用的屬性
        /// </summary>
        private List<F0070LoginData> _f0070Datas;

        public List<F0070LoginData> F0070Datas
        {
            get { return _f0070Datas; }
            set
            {
                _f0070Datas = value;
                RaisePropertyChanged("F0070Datas");
            }
        }

        private F0070LoginData _selectedF0070Data;

        public F0070LoginData SelectedF0070Data
        {
            get { return _selectedF0070Data; }
            set
            {
                _selectedF0070Data = value;
                RaisePropertyChanged("SelectedF0070Data");
            }
        }

        /// <summary>
        /// 全選屬性
        /// </summary>
        private bool _isSelectedAll = false;
        public bool IsSelectedAll
        {
            get { return _isSelectedAll; }
            set { _isSelectedAll = value; RaisePropertyChanged("IsSelectedAll"); }
        }

        #endregion

        #region Command

        #region CheckAll
        /// <summary>
        /// 全選事件
        /// </summary>
        public ICommand CheckAllCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoCheckAllItem()
                );
            }
        }

        public void DoCheckAllItem()
        {
            if (F0070Datas != null)
            {
                foreach (var p in F0070Datas)
                    p.IsSelected = IsSelectedAll;                
            }
        }
        #endregion

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
            F0070Datas = new List<F0070LoginData>();
            var proxyEx = GetExProxy<P19ExDataSource>();
            F0070Datas = proxyEx.CreateQuery<F0070LoginData>("GetLoninData").ToList();
        }
        #endregion Search

        #region Delete
        public ICommand DeleteCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoDelete(), () => UserOperateMode == OperateMode.Query
                    );
            }
        }

        private void DoDelete()
        {
            //執行刪除動作
            var deleteData = F0070Datas.Where(o => o.IsSelected == true).ToList();

            if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
            {
                return;
            }
            if (deleteData.Any())
            {
                var proxy = new wcf.P19WcfServiceClient(); ;
                var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
                    () => proxy.F0070LoginDatasDelete(ExDataMapper.MapCollection<F0070LoginData, wcf.F0070LoginData>(deleteData).ToArray()));
                if (result.IsSuccessed)
                    ShowWarningMessage(Properties.Resources.P1905130000_DeleteSucess);
                else
                    ShowWarningMessage(result.Message);
            }
            else
                ShowWarningMessage(Properties.Resources.P1905130000_ErrSelected);
            DoSearch();
        }
        #endregion Delete

        #endregion

    }
}
