using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;

namespace Wms3pl.WpfClient.P19.Services
{
	public class P190507TreeView : INotifyPropertyChanged
	{
		/// <summary>
		/// 原本 TreeView 沒有 DC層，現在一次顯示所有 DC 的儲位
		/// </summary>
		public string DcCode { get; set; }

		#region Id
		private string _id;
		public string Id
		{
			get { return _id; }
			set
			{
				_id = value;
				NotifyPropertyChanged("Id");
			}
		}
		#endregion

		#region Code - 上層ParentId
		private string _code;
		public string Code
		{
			get { return _code; }
			set
			{
				_code = value;
				NotifyPropertyChanged("Code");
			}
		}
		#endregion

		#region Name
		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				NotifyPropertyChanged("Name");
			}
		}
		#endregion

		private F1912 _src;
		public F1912 Src
		{
			get { return _src; }
			set { _src = value; }
		}

		//樹狀結構階層
		#region Level
		private int _level;
		public int Level
		{
			get { return _level; }
			set
			{
				_level = value;
				NotifyPropertyChanged("Level");
			}
		}
		#endregion

		public P190507TreeView Parent { get; set; }

		#region IsChecked

		private bool _isChecked;

		public bool IsChecked
		{
			get { return _isChecked; }
			set { SetIsChecked(value, true, true); }
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		private void SetIsChecked(bool value, bool updateChildren, bool updateParent)
		{
			//沒改
			if (value == _isChecked) return;

			_isChecked = value;

			//本項目改變後，子項目也全部要變
			if (updateChildren)
				TreeView.ForEach(c => c.SetIsChecked(value, true, true));


			if (Parent != null && updateParent)
			{
				if (!_isChecked)
				{
					// 客製化 - 當取消選取得, 上層也一併取消選取. 
					// 如此在ViewModel移除項目時, 只要抓到已選的項目就全部移除
					Parent.SetIsChecked(false, false, true);

					// 原程式 - 本項目是未checked。改變後，如果同一階層都未check，則父項目也要一起改成未 check
					//if (!Parent.TreeView.Any(i => i.IsChecked))
					//	Parent.SetIsChecked(false, false, true);
				}
				else
				{
					//本項目是 checked，改變後，所有的父項目也要改成 checked
					// 檢查同一階層是否都check
					if (!Parent.TreeView.Any(x => x.IsChecked == false))
					{
						// 設定父層為check狀態
						Parent.SetIsChecked(true, false, true);
					}
				}
			}
			NotifyPropertyChanged("IsChecked");
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", Id, Name);
		}

		private void NotifyPropertyChanged(string info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		#region TreeView清單
		private List<P190507TreeView> _TreeView = new List<P190507TreeView>();

		public List<P190507TreeView> TreeView
		{
			get { return _TreeView; }
			set { _TreeView = value; }
		}
		#endregion
	}
}
