

using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.SignalRWcfService;
using Wms3pl.WpfClient.Services;
using System;
using System.Windows;
using Wms3pl.WpfClient.Common;

namespace PublishVersionNotice.ViewModel
{
	public class SchemaElement
	{
		public string Name { get; set; }
		public string Value { get; set; }
		public string DcCode { get; set; }
		public string GupCode { get; set; }
	}
	public class MainViewModel : ViewModelBase
	{
		#region Property

		#region 是否處理中
		private bool _isBusy;

		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				Set(() => IsBusy, ref _isBusy, value);
			}
		}
		#endregion

		#region 選取的資料庫Schema
		private SchemaElement _selectedDBSchema;

		public SchemaElement SelectedDBSchema
		{
			get { return _selectedDBSchema; }
			set
			{
				Set(() => SelectedDBSchema, ref _selectedDBSchema, value);
			}
		}
		#endregion

		#region 資料庫清單
		private List<SchemaElement> _dbNameList;

		public List<SchemaElement> DBNameList
		{
			get { return _dbNameList; }
			set
			{
				Set(() => DBNameList, ref _dbNameList, value);
			}
		}
		#endregion


		#region 訊息通知清單
		private List<NameValuePair<string>> _noticeList;

		public List<NameValuePair<string>> NoticeList
		{
			get { return _noticeList; }
			set
			{
				Set(() => NoticeList, ref _noticeList, value);
			}
		}
		#endregion


		#region 選取的訊息通知
		private string _selectedNotice;

		public string SelectedNotice
		{
			get { return _selectedNotice; }
			set
			{
				Set(() => SelectedNotice, ref _selectedNotice, value);
				MessageContent = value;
			}
		}
		#endregion


		#region 訊息內容
		private string _messageContent;

		public string MessageContent
		{
			get { return _messageContent; }
			set
			{
				Set(() => MessageContent, ref _messageContent, value);
			}
		}
		#endregion


		#endregion

		#region Constructor

		public MainViewModel()
		{
			SetDBNameList();
			SetNoticeList();
		}

		#endregion

		#region Method

		/// <summary>
		/// 取得並設定資料庫清單
		/// </summary>
		public void SetDBNameList()
		{
			string filePath = @"Schema.xml";
			var doc = XDocument.Load(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, filePath));
			var dcs = doc.Root.Elements("dc");
			var list = new List<SchemaElement>();
			foreach(XElement dc in dcs)
			{
				var gups = dc.Elements("gup");
				foreach(var gup in gups)
				{
					list.Add(new SchemaElement
					{
						Name = gup.Attribute("name").Value,
						Value = gup.Attribute("Schema").Value,
						DcCode = dc.Attribute("code").Value,
						GupCode = gup.Attribute("code").Value
					});
				}
			}
			DBNameList = list;
			SelectedDBSchema = DBNameList.First();
		}

		/// <summary>
		/// 取得並設定資料庫清單
		/// </summary>
		public void SetNoticeList()
		{
			string filePath = @"Notice.xml";
			var doc = XDocument.Load(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, filePath));
			var notices = doc.Root.Elements("notice");
			var list = new List<NameValuePair<string>>();
			foreach (XElement notice in notices)
			{
				list.Add(new NameValuePair<string>
				{
					Name = notice.Attribute("name").Value,
					Value = notice.Attribute("value").Value.Replace("\\r\\n", Environment.NewLine)
				});
			}
			list.Insert(0,new NameValuePair<string> { Name="自訂",Value=""});
			NoticeList = list;
			SelectedNotice = NoticeList.First().Value;
		}
		#endregion

		#region Event


		#region Send
		/// <summary>
		/// Gets the Send.
		/// </summary>
		public ICommand SendCommand
		{
			get
			{
				return new AsyncDelegateCommand(
						o => DoSend(), () => true,o => { IsBusy = false; }, ex => { IsBusy = false; }, () => { IsBusy = true; }
				);
			}
		}

		public void DoSend()
		{
			var proxy = new wcf.SignalRWcfServiceClient();
			var result = WcfServiceHelper.ExecuteForConsole(proxy.InnerChannel
																	, () => proxy.SendMessage( MessageContent),false,"", SelectedDBSchema.DcCode, SelectedDBSchema.GupCode);
		  if(result.IsSuccessed)
			{
				MessageBox.Show("發送通知成功!","訊息");
			}
			else
			{
				MessageBox.Show(string.Format("發送通知成功!{0}{1}", Environment.NewLine, result.Message), "訊息");
			}

		}
		#endregion Send


		#endregion
	}
}