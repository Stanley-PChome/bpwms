﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wms3pl.WpfClient.ExDataServices.P70WcfService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ExecuteResult", Namespace="http://schemas.datacontract.org/2004/07/Wms3pl.Datas.Shared.Entities")]
    [System.SerializableAttribute()]
    public partial class ExecuteResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsSuccessedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NoField;
        
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
        public bool IsSuccessed {
            get {
                return this.IsSuccessedField;
            }
            set {
                if ((this.IsSuccessedField.Equals(value) != true)) {
                    this.IsSuccessedField = value;
                    this.RaisePropertyChanged("IsSuccessed");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string No {
            get {
                return this.NoField;
            }
            set {
                if ((object.ReferenceEquals(this.NoField, value) != true)) {
                    this.NoField = value;
                    this.RaisePropertyChanged("No");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="P70WcfService.P70WcfService")]
    public interface P70WcfService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/P70WcfService/DeleteF700101ByDistrCarNo", ReplyAction="http://tempuri.org/P70WcfService/DeleteF700101ByDistrCarNoResponse")]
        Wms3pl.WpfClient.ExDataServices.P70WcfService.ExecuteResult DeleteF700101ByDistrCarNo(string distrCarNo, string dcCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/P70WcfService/DeleteF700101ByDistrCarNo", ReplyAction="http://tempuri.org/P70WcfService/DeleteF700101ByDistrCarNoResponse")]
        System.Threading.Tasks.Task<Wms3pl.WpfClient.ExDataServices.P70WcfService.ExecuteResult> DeleteF700101ByDistrCarNoAsync(string distrCarNo, string dcCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/P70WcfService/GetTableSeqId", ReplyAction="http://tempuri.org/P70WcfService/GetTableSeqIdResponse")]
        decimal GetTableSeqId(string tableSeqId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/P70WcfService/GetTableSeqId", ReplyAction="http://tempuri.org/P70WcfService/GetTableSeqIdResponse")]
        System.Threading.Tasks.Task<decimal> GetTableSeqIdAsync(string tableSeqId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface P70WcfServiceChannel : Wms3pl.WpfClient.ExDataServices.P70WcfService.P70WcfService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class P70WcfServiceClient : System.ServiceModel.ClientBase<Wms3pl.WpfClient.ExDataServices.P70WcfService.P70WcfService>, Wms3pl.WpfClient.ExDataServices.P70WcfService.P70WcfService {
        
        public P70WcfServiceClient() {
        }
        
        public P70WcfServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public P70WcfServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public P70WcfServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public P70WcfServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Wms3pl.WpfClient.ExDataServices.P70WcfService.ExecuteResult DeleteF700101ByDistrCarNo(string distrCarNo, string dcCode) {
            return base.Channel.DeleteF700101ByDistrCarNo(distrCarNo, dcCode);
        }
        
        public System.Threading.Tasks.Task<Wms3pl.WpfClient.ExDataServices.P70WcfService.ExecuteResult> DeleteF700101ByDistrCarNoAsync(string distrCarNo, string dcCode) {
            return base.Channel.DeleteF700101ByDistrCarNoAsync(distrCarNo, dcCode);
        }
        
        public decimal GetTableSeqId(string tableSeqId) {
            return base.Channel.GetTableSeqId(tableSeqId);
        }
        
        public System.Threading.Tasks.Task<decimal> GetTableSeqIdAsync(string tableSeqId) {
            return base.Channel.GetTableSeqIdAsync(tableSeqId);
        }
    }
}
