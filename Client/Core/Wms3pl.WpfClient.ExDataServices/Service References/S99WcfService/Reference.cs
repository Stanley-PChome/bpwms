﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wms3pl.WpfClient.ExDataServices.S99WcfService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="WMSMessage", Namespace="http://schemas.datacontract.org/2004/07/Wms3pl.Datas.Shared.Entities")]
    [System.SerializableAttribute()]
    public partial class WMSMessage : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime CRT_DATEField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CUST_CODEField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<decimal> DAYSField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DC_CODEField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GUP_CODEField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsMailField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsSmsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MESSAGE_CONTENTField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal MESSAGE_IDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MSG_SUBJECTField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] ReceiverMailsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] ReceiverMobilesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string STATUSField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TARGET_CODEField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TARGET_TYPEField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime CRT_DATE {
            get {
                return this.CRT_DATEField;
            }
            set {
                if ((this.CRT_DATEField.Equals(value) != true)) {
                    this.CRT_DATEField = value;
                    this.RaisePropertyChanged("CRT_DATE");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CUST_CODE {
            get {
                return this.CUST_CODEField;
            }
            set {
                if ((object.ReferenceEquals(this.CUST_CODEField, value) != true)) {
                    this.CUST_CODEField = value;
                    this.RaisePropertyChanged("CUST_CODE");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<decimal> DAYS {
            get {
                return this.DAYSField;
            }
            set {
                if ((this.DAYSField.Equals(value) != true)) {
                    this.DAYSField = value;
                    this.RaisePropertyChanged("DAYS");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DC_CODE {
            get {
                return this.DC_CODEField;
            }
            set {
                if ((object.ReferenceEquals(this.DC_CODEField, value) != true)) {
                    this.DC_CODEField = value;
                    this.RaisePropertyChanged("DC_CODE");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string GUP_CODE {
            get {
                return this.GUP_CODEField;
            }
            set {
                if ((object.ReferenceEquals(this.GUP_CODEField, value) != true)) {
                    this.GUP_CODEField = value;
                    this.RaisePropertyChanged("GUP_CODE");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsMail {
            get {
                return this.IsMailField;
            }
            set {
                if ((this.IsMailField.Equals(value) != true)) {
                    this.IsMailField = value;
                    this.RaisePropertyChanged("IsMail");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsSms {
            get {
                return this.IsSmsField;
            }
            set {
                if ((this.IsSmsField.Equals(value) != true)) {
                    this.IsSmsField = value;
                    this.RaisePropertyChanged("IsSms");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MESSAGE_CONTENT {
            get {
                return this.MESSAGE_CONTENTField;
            }
            set {
                if ((object.ReferenceEquals(this.MESSAGE_CONTENTField, value) != true)) {
                    this.MESSAGE_CONTENTField = value;
                    this.RaisePropertyChanged("MESSAGE_CONTENT");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal MESSAGE_ID {
            get {
                return this.MESSAGE_IDField;
            }
            set {
                if ((this.MESSAGE_IDField.Equals(value) != true)) {
                    this.MESSAGE_IDField = value;
                    this.RaisePropertyChanged("MESSAGE_ID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MSG_SUBJECT {
            get {
                return this.MSG_SUBJECTField;
            }
            set {
                if ((object.ReferenceEquals(this.MSG_SUBJECTField, value) != true)) {
                    this.MSG_SUBJECTField = value;
                    this.RaisePropertyChanged("MSG_SUBJECT");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] ReceiverMails {
            get {
                return this.ReceiverMailsField;
            }
            set {
                if ((object.ReferenceEquals(this.ReceiverMailsField, value) != true)) {
                    this.ReceiverMailsField = value;
                    this.RaisePropertyChanged("ReceiverMails");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] ReceiverMobiles {
            get {
                return this.ReceiverMobilesField;
            }
            set {
                if ((object.ReferenceEquals(this.ReceiverMobilesField, value) != true)) {
                    this.ReceiverMobilesField = value;
                    this.RaisePropertyChanged("ReceiverMobiles");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string STATUS {
            get {
                return this.STATUSField;
            }
            set {
                if ((object.ReferenceEquals(this.STATUSField, value) != true)) {
                    this.STATUSField = value;
                    this.RaisePropertyChanged("STATUS");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TARGET_CODE {
            get {
                return this.TARGET_CODEField;
            }
            set {
                if ((object.ReferenceEquals(this.TARGET_CODEField, value) != true)) {
                    this.TARGET_CODEField = value;
                    this.RaisePropertyChanged("TARGET_CODE");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TARGET_TYPE {
            get {
                return this.TARGET_TYPEField;
            }
            set {
                if ((object.ReferenceEquals(this.TARGET_TYPEField, value) != true)) {
                    this.TARGET_TYPEField = value;
                    this.RaisePropertyChanged("TARGET_TYPE");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="S99WcfService.S99WcfService")]
    public interface S99WcfService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/S99WcfService/GetWmsMessages", ReplyAction="http://tempuri.org/S99WcfService/GetWmsMessagesResponse")]
        Wms3pl.WpfClient.ExDataServices.S99WcfService.WMSMessage[] GetWmsMessages();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/S99WcfService/GetWmsMessages", ReplyAction="http://tempuri.org/S99WcfService/GetWmsMessagesResponse")]
        System.Threading.Tasks.Task<Wms3pl.WpfClient.ExDataServices.S99WcfService.WMSMessage[]> GetWmsMessagesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/S99WcfService/SetMessageStatus", ReplyAction="http://tempuri.org/S99WcfService/SetMessageStatusResponse")]
        void SetMessageStatus(Wms3pl.WpfClient.ExDataServices.S99WcfService.WMSMessage[] wmsMessages);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/S99WcfService/SetMessageStatus", ReplyAction="http://tempuri.org/S99WcfService/SetMessageStatusResponse")]
        System.Threading.Tasks.Task SetMessageStatusAsync(Wms3pl.WpfClient.ExDataServices.S99WcfService.WMSMessage[] wmsMessages);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface S99WcfServiceChannel : Wms3pl.WpfClient.ExDataServices.S99WcfService.S99WcfService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class S99WcfServiceClient : System.ServiceModel.ClientBase<Wms3pl.WpfClient.ExDataServices.S99WcfService.S99WcfService>, Wms3pl.WpfClient.ExDataServices.S99WcfService.S99WcfService {
        
        public S99WcfServiceClient() {
        }
        
        public S99WcfServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public S99WcfServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public S99WcfServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public S99WcfServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Wms3pl.WpfClient.ExDataServices.S99WcfService.WMSMessage[] GetWmsMessages() {
            return base.Channel.GetWmsMessages();
        }
        
        public System.Threading.Tasks.Task<Wms3pl.WpfClient.ExDataServices.S99WcfService.WMSMessage[]> GetWmsMessagesAsync() {
            return base.Channel.GetWmsMessagesAsync();
        }
        
        public void SetMessageStatus(Wms3pl.WpfClient.ExDataServices.S99WcfService.WMSMessage[] wmsMessages) {
            base.Channel.SetMessageStatus(wmsMessages);
        }
        
        public System.Threading.Tasks.Task SetMessageStatusAsync(Wms3pl.WpfClient.ExDataServices.S99WcfService.WMSMessage[] wmsMessages) {
            return base.Channel.SetMessageStatusAsync(wmsMessages);
        }
    }
}
