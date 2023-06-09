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
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.P19.ViewModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P19.ViewModel
{
    public partial class P1920100000_ViewModel : InputViewModelBase
    {
        #region 共用變數/資料連結/頁面參數
        private bool isValid;
        public Action AddAction = delegate { };
        public Action EditAction = delegate { };
        public Action SearchAction = delegate { };

        #endregion

        public P1920100000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                var _F00proxy = GetProxy<F00Entities>();
				this.TMPR_TYPE_List = GetBaseTableService.GetF000904List(FunctionCode, "F194702").Select(o=>new F000904 { VALUE = o.Value,NAME = o.Name}).ToObservableCollection(); 
                this.TMPR_TYPE_List.Insert(0, new F000904());

                //InitControls();
            }
        }
        //private void InitControls()
        //{
        //}

        private string _txtCAR_KIND_ID = string.Empty;
        public string txtCAR_KIND_ID
        {
            get
            {
                return this._txtCAR_KIND_ID;
            }
            set
            {
                this._txtCAR_KIND_ID = value;
                RaisePropertyChanged("txtCAR_KIND_ID");
            }
        }


        private string _txtCAR_KIND_NAME = string.Empty;
        public string txtCAR_KIND_NAME
        {
            get
            {
                return this._txtCAR_KIND_NAME;
            }
            set
            {
                this._txtCAR_KIND_NAME = value;
                RaisePropertyChanged("txtCAR_KIND_NAME");
            }
        }

        private string _CAR_SIZE = string.Empty;
        public string CAR_SIZE
        {
            get
            {
                return this._CAR_SIZE;
            }
            set
            {
                this._CAR_SIZE = value;
                RaisePropertyChanged("CAR_SIZE");
            }
        }

        private string _TMPR_TYPE = string.Empty;
        public string TMPR_TYPE
        {
            get
            {
                return this._TMPR_TYPE;
            }
            set
            {
                this._TMPR_TYPE = value;
                RaisePropertyChanged("TMPR_TYPE");
            }
        }

        private ObservableCollection<F194702> _F194702List;
        public ObservableCollection<F194702> F194702List
        {
            get
            {
                if (_F194702List == null) _F194702List = new ObservableCollection<F194702>();
                return _F194702List;
            }
            set
            {
                _F194702List = value;
                RaisePropertyChanged("F194702List");
            }
        }

        private F194702 _F194702_SelectedData;
        public F194702 F194702_SelectedData
        {
            get { return _F194702_SelectedData; }
            set
            {
                if (_F194702_SelectedData != null && (UserOperateMode == OperateMode.Edit || UserOperateMode == OperateMode.Add))
                {
                    return;
                }
                else
                {
                    _F194702_SelectedData = value;
                    RaisePropertyChanged("F194702_SelectedData");
                }
            }
        }

        private ObservableCollection<F000904> _TMPR_TYPE_List = null;
        public ObservableCollection<F000904> TMPR_TYPE_List
        {
            get
            {
                return this._TMPR_TYPE_List;
            }
            set
            {
                this._TMPR_TYPE_List = value;
                RaisePropertyChanged("TMPR_TYPE_List");
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
            
            Regex NumberPattern=new Regex("^\\d+$");            
            int intCAR_KIND_ID = 0;
            if ((!string.IsNullOrWhiteSpace(txtCAR_KIND_ID)))
            {
                if (NumberPattern.IsMatch(txtCAR_KIND_ID) == false)
                {
                    DialogService.ShowMessage(Properties.Resources.P1920100000_txtCAR_KIND_ID_FormatError);
                    return;
                }
                intCAR_KIND_ID = Convert.ToInt32(txtCAR_KIND_ID);
            }               
         
            var F19Proxy = GetProxy<F19Entities>();

            var f194702s = F19Proxy.F194702s.AsQueryable();
            if (!string.IsNullOrEmpty(txtCAR_KIND_ID))
            {
                f194702s = f194702s.Where(x => x.CAR_KIND_ID.Equals(intCAR_KIND_ID));
            }
            if (!string.IsNullOrEmpty(txtCAR_KIND_NAME))
            {
                f194702s = f194702s.Where(x => x.CAR_KIND_NAME.Contains(txtCAR_KIND_NAME));
            }
            if (!string.IsNullOrEmpty(CAR_SIZE))
            {
                f194702s = f194702s.Where(x => x.CAR_SIZE.Equals(CAR_SIZE));
            }
            if (!string.IsNullOrEmpty(TMPR_TYPE))
            {
                f194702s = f194702s.Where(x => x.TMPR_TYPE.Equals(TMPR_TYPE));
            }
            this.F194702List = f194702s.OrderBy(x => x.CAR_KIND_ID).ToObservableCollection();

            if (this.F194702List == null || this.F194702List.Count.Equals(0))
            {
                ShowMessage(Messages.InfoNoData);
                return;
            }
        }

        private void DoSearchCompleted()
        {
            if (this.F194702List == null || !this.F194702List.Any()) return;
            this.F194702_SelectedData = this.F194702List.FirstOrDefault();
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
            F194702 NewF194702 = new F194702();
            NewF194702.CRT_DATE = DateTime.Now;
            this.F194702List.Add(NewF194702);
            RaisePropertyChanged("F194702List");
            this.F194702_SelectedData = NewF194702;
            UserOperateMode = OperateMode.Add;
            AddAction();
          

         

        }
        #endregion Add

        #region Edit
        public ICommand EditCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoEdit(), () => UserOperateMode == OperateMode.Query
                                        && (this.F194702_SelectedData != null
                                        && this.F194702List.Any()), o => DoEditCompleted()
                    );
            }
        }

        private void DoEdit()
        {
            //執行編輯動作
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
                var resetSelectedData = false;
                if (UserOperateMode == OperateMode.Add && this.F194702List.Any())
                {
                    resetSelectedData = true;
                }

                UserOperateMode = OperateMode.Query;
                if (resetSelectedData && this.F194702List.Any())
                    this.F194702_SelectedData = this.F194702List.First();
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
                    o => DoDelete(), () => UserOperateMode == OperateMode.Query
                        && this.F194702_SelectedData != null,
                    o => DoDeleteCompleted()
                    );
            }
        }

        private void DoDelete()
        {
            if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;


            var F19Proxy = GetProxy<F19Entities>();
            var F194702 = (from EachF194702 in F19Proxy.F194702s
                           where EachF194702.CAR_KIND_ID.Equals(this.F194702_SelectedData.CAR_KIND_ID)
                           select EachF194702).FirstOrDefault();

            if (F194702 == null)
            {
                DialogService.ShowMessage(Properties.Resources.P1920100000_txtCAR_KIND_ID_Null);
                return;
            }
            else
            {

                F19Proxy.DeleteObject(F194702);
                F19Proxy.SaveChanges();
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
            Regex NumberPattern = new Regex("^\\d+$");    
            if (string.IsNullOrEmpty(this.F194702_SelectedData.CAR_KIND_ID.ToString()) ||
                    string.IsNullOrWhiteSpace(F194702_SelectedData.CAR_KIND_ID.ToString()) ||
                        this.F194702_SelectedData.CAR_KIND_ID.Equals(0) ||
                            NumberPattern.IsMatch(this.F194702_SelectedData.CAR_KIND_ID.ToString())==false)
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920100000_InputCAR_KIND_IDAndCheckFormatCorrect);
                return;
            }

            if (UserOperateMode == OperateMode.Add)
            {
                var F19Proxy = GetProxy<F19Entities>();
                var F194702 = F19Proxy.F194702s.Where(x => x.CAR_KIND_ID.Equals(this.F194702_SelectedData.CAR_KIND_ID)).SingleOrDefault();
                if (F194702 != null)
                {
                    isValid = false;
                    DialogService.ShowMessage(Properties.Resources.P1920100000_CAR_KIND_ID_Duplicate);
                    return;
                }
            }

            if (string.IsNullOrEmpty(this.F194702_SelectedData.CAR_KIND_NAME) ||
                    string.IsNullOrWhiteSpace(this.F194702_SelectedData.CAR_KIND_NAME))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920100000_InputCAR_KIND_Name);
                return;
            }

            if (UserOperateMode == OperateMode.Add)
                DoSaveAdd();
            else if (UserOperateMode == OperateMode.Edit)
                DoSaveEdit();
        }

        private void DoSaveAdd()
        {
            F194702 F194702 = new F194702();
            
            F194702.CAR_KIND_ID = this.F194702_SelectedData.CAR_KIND_ID;
            F194702.CAR_KIND_NAME = this.F194702_SelectedData.CAR_KIND_NAME;
            F194702.CAR_SIZE = this.F194702_SelectedData.CAR_SIZE;
            F194702.TMPR_TYPE = this.F194702_SelectedData.TMPR_TYPE;
           
            var F19Proxy = GetProxy<F19Entities>();
            F19Proxy.AddToF194702s(F194702);
            F19Proxy.SaveChanges();
            ShowMessage(Messages.Success);
        }

        private void DoSaveEdit()
        {
            var F19Proxy = GetProxy<F19Entities>();
            var F194702 = F19Proxy.F194702s.Where(x => x.CAR_KIND_ID == this.F194702_SelectedData.CAR_KIND_ID).SingleOrDefault();

            if (F194702 != null)
            {
                F194702.CAR_KIND_NAME = this.F194702_SelectedData.CAR_KIND_NAME;
                F194702.CAR_SIZE = this.F194702_SelectedData.CAR_SIZE;
                F194702.TMPR_TYPE = this.F194702_SelectedData.TMPR_TYPE;

                F194702.UPD_STAFF = Wms3plSession.Get<UserInfo>().Account;
                F194702.UPD_NAME = Wms3plSession.Get<UserInfo>().AccountName;
                F194702.UPD_DATE = DateTime.Now;
                F19Proxy.UpdateObject(F194702);
                F19Proxy.SaveChanges();
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

        //public bool IsValidPireCode(String pireCode)
        //{
        //    System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
        //    return reg1.IsMatch(pireCode);
        //}
        //public bool IsValidTempArea(String tempArea)
        //{
        //    System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\+?[1-9][0-9]*$");
        //    return reg1.IsMatch(tempArea);
        //}

    }
}
