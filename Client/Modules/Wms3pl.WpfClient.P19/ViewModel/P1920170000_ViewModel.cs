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
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;
using System.Reflection;
using Wms3pl.WpfClient.P19.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using AutoMapper;


namespace Wms3pl.WpfClient.P19.ViewModel
{
    public partial class P1920170000_ViewModel : InputViewModelBase
    {
        #region 共用變數/資料連結/頁面參數     
        private bool isValid;
        private bool isInit=false;
        public Action AddAction = delegate { };
        public Action EditAction = delegate { };
        public Action SearchAction = delegate { };
        #endregion

        public P1920170000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                if (!isInit)
                {
                    InitControls();
                    isInit = true;
                }    
                
             
            }
        }

        private void InitControls()
        {
            GetDcCodes();
            SearchCommand.Execute(null);
        }

        private void GetDcCodes()
        {
            DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
            if (DcCodes.Any())
            {
                SelectDcCode = DcCodes.First().Value;
            }
        }

        private List<NameValuePair<string>> _dcCodes;
        public List<NameValuePair<string>> DcCodes
        {
            get { return _dcCodes; }
            set
            {
                _dcCodes = value;
                RaisePropertyChanged();
            }
        }

        private string _SelectDcCode;
        public string SelectDcCode
        {
            get { return _SelectDcCode; }
            set
            {
                _SelectDcCode = value;
                this.F0005_SelectedData = null;
                this.F0005List = null;
                SearchCommand.Execute(null);
                RaisePropertyChanged("SelectDcCode");
            }
        }

        private ObservableCollection<F0005> _F0005List = null;
        public ObservableCollection<F0005> F0005List
        {
            get
            {
                if (this._F0005List == null)
                    this._F0005List = new ObservableCollection<F0005>();
                return this._F0005List;
            }
            set
            {
                this._F0005List = value;
                RaisePropertyChanged("F0005List");
            }
        }

        private F0005 _F0005_SelectedData;
        public F0005 F0005_SelectedData
        {
            get { return _F0005_SelectedData; }
            set
            {
                if (_F0005_SelectedData != null && (UserOperateMode == OperateMode.Edit))
                    return;
                else
                {
                    _F0005_SelectedData = value;
                    RaisePropertyChanged("F0005_SelectedData");
                }
            }
        }

        protected string _Old_SET_VALUE = string.Empty;
        public string Old_SET_VALUE
        {
            get
            {
                return this._Old_SET_VALUE;
            }
            set
            {
                this._Old_SET_VALUE = value;
            }
        }

        protected string _Old_DESCRIPT = string.Empty;
        public string Old_DESCRIPT
        {
            get
            {
                return this._Old_DESCRIPT;
            }
            set
            {
                this._Old_DESCRIPT = value;
            }
        }

        #region Search
        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoSearch(), () => DoSearchCanExecute(),
                    o => DoSearchCompleted()
                    );
            }
        }

        private bool DoSearchCanExecute()
        {
            return UserOperateMode == OperateMode.Query;
        }

        private void DoSearch()
        {
            var F00Proxy = GetProxy<F00Entities>();
            this.F0005List = F00Proxy.F0005s.Where(x => x.DC_CODE == this.SelectDcCode).ToObservableCollection();
            if (this.F0005List.Count.Equals(0))
                ShowMessage(Messages.InfoNoData);
            F00Proxy = null;
        }

        private void DoSearchCompleted()
        {
            if (this.F0005List.Count > 0)
                this.F0005_SelectedData = this.F0005List.FirstOrDefault();
            SearchAction();
        }

        #endregion Search

        #region Add
        public ICommand AddCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoAdd(), () => DoAddCanExecute(), o => DoAddCompleted()
                    );
            }
        }

        private bool DoAddCanExecute()
        {
            return UserOperateMode == OperateMode.Query;
        }

        private void DoAdd()
        {
        }

        private void DoAddCompleted()
        {
            F0005 NewF0005 = new F0005();
            NewF0005.CRT_DATE = DateTime.Now;
            NewF0005.DC_CODE = SelectDcCode;
            this.F0005List.Add(NewF0005);
            RaisePropertyChanged("F0005List");
            this.F0005_SelectedData = NewF0005;
            AddAction();
            UserOperateMode = OperateMode.Add;
        }
        #endregion Add

        #region Edit
        public ICommand EditCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoEdit(),
                    () => DoEditCanExecute(),
                    o => DoEditCompleted()
                    );
            }
        }

        private bool DoEditCanExecute()
        {
            return (UserOperateMode == OperateMode.Query &&
                    (this.F0005_SelectedData != null && this.F0005List.Any()));
        }

        private void DoEdit()
        {
            this.Old_DESCRIPT = this.F0005_SelectedData.DESCRIPT;
            this.Old_SET_VALUE = this.F0005_SelectedData.SET_VALUE;
        }

        private void DoEditCompleted()
        {
            EditAction();
            UserOperateMode = OperateMode.Edit;
        }
        #endregion Edit

        #region Cancel
        public ICommand CancelCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                                    o => DoCancel(),
                                    () => DoCancelCanExecute(),
                                    p => DoCancelCompleted()
                                    );
            }
        }

        private bool DoCancelCanExecute()
        {
            return UserOperateMode != OperateMode.Query;
        }

        private void DoCancel()
        {
        }

        private void DoCancelCompleted()
        {
            if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
            {
                this.F0005_SelectedData.SET_VALUE = this.Old_SET_VALUE;
                this.F0005_SelectedData.DESCRIPT = this.Old_DESCRIPT;

                if (this.F0005List.Count > 0)
                    this.F0005_SelectedData = this.F0005List.First();

                SearchAction();
                UserOperateMode = OperateMode.Query;
                DoSearch();
                this.Old_SET_VALUE = string.Empty;
                this.Old_DESCRIPT = string.Empty;
            }
            else
            {
                if (UserOperateMode == OperateMode.Edit)
                    DoEditCompleted();               
            }
        }
        #endregion Cancel

        #region Delete
        public ICommand DeleteCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoDelete(), 
                    () => DoDeleteCanExecute(),
                    o => DoDeleteCompleted()
                    );
            }
        }

        private bool DoDeleteCanExecute()
        {
            return (UserOperateMode == OperateMode.Query && this.F0005_SelectedData != null);
        }

        private void DoDelete()
        {
            if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;

            var F00Proxy = GetProxy<F00Entities>();

            var F0005 = (from a in F00Proxy.F0005s
                         where a.DC_CODE.Equals(this.F0005_SelectedData.DC_CODE) 
                         && a.SET_NAME.Equals(this.F0005_SelectedData.SET_NAME)
                         select a).FirstOrDefault();
            if (F0005 == null)
            {
                DialogService.ShowMessage(Properties.Resources.P1920170000_SET_NAME_Null);
                return;
            }
            else
            {
                F00Proxy.DeleteObject(F0005);
            }
            F00Proxy.SaveChanges();
            ShowMessage(Messages.DeleteSuccess);
        }

        private void DoDeleteCompleted()
        {
            UserOperateMode = OperateMode.Query;
            SearchCommand.Execute(null);
        }
        #endregion Delete

        #region Save
        public ICommand SaveCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoSave(), () => DoSaveCanExecute(), p => DoSaveCompleted()
                    );
            }
        }

        private bool DoSaveCanExecute()
        {
            return UserOperateMode != OperateMode.Query;
        }

        private void DoSave()
        {
            isValid = true;

            if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
            {
                isValid = false;
                return;
            }

            if (string.IsNullOrEmpty(SelectDcCode) || 
                string.IsNullOrWhiteSpace(SelectDcCode))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1901160000_SelectedDC1);
                return;
            }

            if (string.IsNullOrEmpty(this.F0005_SelectedData.SET_NAME) ||
               string.IsNullOrWhiteSpace(F0005_SelectedData.SET_NAME))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920170000_InputSET_NAME);
                return;
            }

            if (string.IsNullOrEmpty(this.F0005_SelectedData.SET_VALUE) ||
                string.IsNullOrWhiteSpace(F0005_SelectedData.SET_VALUE))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920170000_InputSET_VALUE);
                return;
            }
                             
            if (UserOperateMode == OperateMode.Add)
                DoSaveAdd();
            else if (UserOperateMode == OperateMode.Edit)
            {
                DoSaveEdit();
                this.Old_DESCRIPT = string.Empty;
                this.Old_SET_VALUE = string.Empty;                
            }
        }

        private void DoSaveAdd()
        {
            var F00Proxy = GetProxy<F00Entities>();
            var F0005 = new F0005();
            F0005.DC_CODE = SelectDcCode;
            F0005.SET_VALUE = this.F0005_SelectedData.SET_VALUE;
            F0005.DESCRIPT = this.F0005_SelectedData.DESCRIPT;
            F00Proxy.AddToF0005s(F0005);
            F00Proxy.SaveChanges();
            ShowMessage(Messages.Success);
        }

        private void DoSaveEdit()
        {
            var F00Proxy = GetProxy<F00Entities>();

            var F0005 = F00Proxy.F0005s.Where(x => x.DC_CODE == SelectDcCode
                                           && x.SET_NAME == this.F0005_SelectedData.SET_NAME).SingleOrDefault();
           
            if (F0005 != null)
            {
                F0005.SET_VALUE = this.F0005_SelectedData.SET_VALUE;
                F0005.DESCRIPT = this.F0005_SelectedData.DESCRIPT;
                F0005.UPD_STAFF = Wms3plSession.Get<UserInfo>().Account;
                F0005.UPD_NAME = Wms3plSession.Get<UserInfo>().AccountName;
                F0005.UPD_DATE = DateTime.Now;
                F00Proxy.UpdateObject(F0005);
                F00Proxy.SaveChanges();
                ShowMessage(Messages.Success);
            }
            else
            {
                ShowMessage(Messages.Failed);
            }
        }

        private void DoSaveCompleted()
        {
            if (isValid == true)
            {
                UserOperateMode = OperateMode.Query;
                SearchCommand.Execute(null);
            }
        }
        #endregion Save

    }
}
