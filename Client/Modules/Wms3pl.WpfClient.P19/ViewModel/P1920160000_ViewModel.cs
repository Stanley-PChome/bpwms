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
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.P19.ViewModel;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P19.ViewModel
{
    public partial class P1920160000_ViewModel : InputViewModelBase
    {
        #region 共用變數/資料連結/頁面參數
        //private readonly F19Entities _proxy;
        //private string _userId;
        //private string _userName;
        private bool isValid;
        public Action AddAction = delegate { };
        public Action EditAction = delegate { };
        public Action SearchAction = delegate { };

        #endregion

        public P1920160000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                //初始化執行時所需的值及資料
                //_proxy = GetProxy<F19Entities>();
                //_userId = Wms3plSession.Get<UserInfo>().Account;
                //_userName = Wms3plSession.Get<UserInfo>().AccountName;
                InitControls();
            }
        }

        private void InitControls()
        {
          
        }

        private string _PROCESS_ID = string.Empty;
        public string PROCESS_ID
        {
            get { return _PROCESS_ID; }
            set
            {
                _PROCESS_ID = value;
                RaisePropertyChanged("PROCESS_ID");
            }
        }

        private string _PROCESS_ACT = string.Empty;
        public string PROCESS_ACT
        {
            get { return _PROCESS_ACT; }
            set
            {
                _PROCESS_ACT = value;
                RaisePropertyChanged("PROCESS_ACT");
            }
        }

        private ObservableCollection <F910001> _F910001List;
        public ObservableCollection<F910001> F910001List
        {
            get
            {
                if (_F910001List == null) _F910001List = new ObservableCollection<F910001>();
                return _F910001List;
            }
            set
            {
                _F910001List = value;
                RaisePropertyChanged("F910001List");
            }
        }

        private F910001 _F910001_SelectedData;

        public F910001 F910001_SelectedData
        {
            get { return _F910001_SelectedData; }
            set
            {
                //if (_F910001_SelectedData != null && (UserOperateMode == OperateMode.Edit || UserOperateMode == OperateMode.Add))               
                if (_F910001_SelectedData != null && (UserOperateMode == OperateMode.Edit))
                {                
                    return;
                }
                else
                {
                    _F910001_SelectedData = value;
                    RaisePropertyChanged("F910001_SelectedData");
                }
            }
        }

        #region Search
        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoSearch(), () => UserOperateMode == OperateMode.Query,
                    o => DoSearchCompleted()
                    );
            }
        }

        private void DoSearch()
        {
            var F91Proxy = GetProxy<F91Entities>();
            this.F910001List = F91Proxy.F910001s.ToObservableCollection();
            if (!string.IsNullOrWhiteSpace(this.PROCESS_ID))
            {
                this.F910001List = this.F910001List.Where(x => x.PROCESS_ID == this.PROCESS_ID).ToObservableCollection();
            }
            if (!string.IsNullOrWhiteSpace(this.PROCESS_ACT))
            {
                this.F910001List = this.F910001List.Where(x => x.PROCESS_ACT.Contains(this.PROCESS_ACT)).ToObservableCollection();
            }
            //this.F910001List = F91Proxy.F910001s.Where(x => (x.PROCESS_ID == this.PROCESS_ID || string.IsNullOrWhiteSpace(this.PROCESS_ID))
            //&& (x.PROCESS_ACT.Contains(this.PROCESS_ACT) || string.IsNullOrWhiteSpace(this.PROCESS_ACT))).ToObservableCollection();
            if (this.F910001List == null || !this.F910001List.Any())
            {
                ShowMessage(Messages.InfoNoData);
                return;
            }                      
        }

        private void DoSearchCompleted()
        {
            if (this.F910001List == null || !this.F910001List.Any()) return;
            this.F910001_SelectedData = this.F910001List.FirstOrDefault();
            SearchAction();
        }

        #endregion Search

        #region Add
        public ICommand AddCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoAdd(), () => UserOperateMode == OperateMode.Query, o => DoAddCompleted()
                    );
            }
        }

        private void DoAdd()
        {

        }

        private void DoAddCompleted()
        {
            F910001 NewF910001 = new F910001();
            NewF910001.CRT_DATE = DateTime.Now;
            this.F910001List.Add(NewF910001);
            RaisePropertyChanged("F910001List");
            this.F910001_SelectedData = NewF910001;
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
                    o => DoEdit(), () => UserOperateMode == OperateMode.Query 
                        && (this.F910001_SelectedData != null && this.F910001List.Any()), o => DoEditCompleted()
                    );
            }
        }

        private void DoEdit()
        {         
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
                                    o => DoCancel(), () => UserOperateMode != OperateMode.Query, p => DoCancelCompleted()
                                    );
            }
        }

        private void DoCancel()
        {

        }

        private void DoCancelCompleted()
        {
            if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
            {
               
                UserOperateMode = OperateMode.Query;
               
                SearchAction();

                DoSearch();
            }
            else
            {
                if (UserOperateMode == OperateMode.Edit)
                    DoEditCompleted();
                else
                {
                    //UserOperateMode = OperateMode.Query;
                    //DoAddCompleted();
                }

            }
        }
        #endregion Cancel

        #region Delete
        public ICommand DeleteCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoDelete(), () => UserOperateMode == OperateMode.Query && this.F910001_SelectedData != null,
                    o => DoDeleteCompleted()
                    );
            }
        }

        private void DoDelete()
        {
            if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;

            var F91Proxy = GetProxy<F91Entities>();

            var F910001 = (from EachF910001 in F91Proxy.F910001s
                           where EachF910001.PROCESS_ID.Equals(this.F910001_SelectedData.PROCESS_ID)
                           select EachF910001).FirstOrDefault();
            if (F910001 == null)
            {
                DialogService.ShowMessage(Properties.Resources.P1920160000_PROCESS_ID_Null);
                return;
            }
            else
            {
                F91Proxy.DeleteObject(F910001);
                F91Proxy.SaveChanges();
                ShowMessage(Messages.DeleteSuccess);
            }
           
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
                    o => DoSave(), () => UserOperateMode != OperateMode.Query, p => DoSaveCompleted()
                    );
            }
        }

        private void DoSave()
        {
            isValid = true;
            
            if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
            {
                isValid = false;
                return;
            }

            if (string.IsNullOrEmpty(this.F910001_SelectedData.PROCESS_ID) ||
                string.IsNullOrWhiteSpace(this.F910001_SelectedData.PROCESS_ID))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920160000_InputPROCESS_ID);
                return;
            }

            var F91Proxy = GetProxy<F91Entities>();

            if (UserOperateMode == OperateMode.Add)
            {
                var F910001 = F91Proxy.F910001s.Where(x => x.PROCESS_ID.ToLower().Equals(this.F910001_SelectedData.PROCESS_ID.ToLower())).SingleOrDefault();
                if (F910001 !=null)
                {
                    isValid = false;
                    DialogService.ShowMessage(Properties.Resources.P1920160000_PROCESS_ID_Duplicate);
                    return;
                }
            }

            if (string.IsNullOrEmpty(F910001_SelectedData.PROCESS_ACT) || string.IsNullOrWhiteSpace(F910001_SelectedData.PROCESS_ACT))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920160000_InputPROCESS_ACT);
                return;
            }

            for (int i = 0; i <= F910001_SelectedData.PROCESS_ID.Length - 1;i++ )
            {
                if(Encoding.Default.GetByteCount(F910001_SelectedData.PROCESS_ID[i].ToString())==2)
                {
                    isValid = false;
                    DialogService.ShowMessage(Properties.Resources.P1920160000_InputPROCESS_ACT_WithByte);
                    return;
                }
            }
                     
            int intGetByteCount = Encoding.Default.GetByteCount(F910001_SelectedData.PROCESS_ID);
            if (intGetByteCount>5)
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920160000_PROCESS_ACT_ByteCountError);
                return;
            }

            if (UserOperateMode == OperateMode.Add)
                DoSaveAdd();
            else if (UserOperateMode == OperateMode.Edit)
                DoSaveEdit();
        }

        private void DoSaveAdd()
        {           
            F910001 F910001 = new F910001();
            F910001.PROCESS_ID = this.F910001_SelectedData.PROCESS_ID;
            F910001.PROCESS_ACT = this.F910001_SelectedData.PROCESS_ACT;
            var F91Proxy = GetProxy<F91Entities>();
            F91Proxy.AddToF910001s(F910001);
            F91Proxy.SaveChanges();
            ShowMessage(Messages.Success);
        }

        private void DoSaveEdit()
        {
            var F91Proxy = GetProxy<F91Entities>();
            var F910001 = F91Proxy.F910001s.Where(x => x.PROCESS_ID == this.F910001_SelectedData.PROCESS_ID).SingleOrDefault();

            if (F910001 != null)
            {
                F910001.PROCESS_ACT = this.F910001_SelectedData.PROCESS_ACT;              
                F910001.UPD_STAFF = Wms3plSession.Get<UserInfo>().Account;
                F910001.UPD_NAME = Wms3plSession.Get<UserInfo>().AccountName;
                F910001.UPD_DATE = DateTime.Now;
                F91Proxy.UpdateObject(F910001);
                F91Proxy.SaveChanges();
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
