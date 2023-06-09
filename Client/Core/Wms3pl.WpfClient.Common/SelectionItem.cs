using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Wms3pl.WpfClient.Common
{
	public class SelectionItem<T> : INotifyPropertyChanged
	{
		/// <summary>
		/// 初始化是否選擇，包含記憶第一次的選擇
		/// </summary>
		/// <param name="item"></param>
		/// <param name="isSelected"></param>
		public SelectionItem(T item, bool isSelected)
		{
			_item = item;
			_IsSelected = isSelected;
			IsSelectedOld = isSelected;
		}

		public SelectionItem(T item)
		{
			this._item = item;
		}


		/// <summary>
		/// The <see cref="IsSelected" /> property's name.
		/// </summary>
		public const string IsSelectedPropertyName = "IsSelected";

		private bool _IsSelected = false;

		/// <summary>
		/// Gets the IsSelected property.
		/// TO DO Update documentation:
		/// Changes to that property's value raise the PropertyChanged event. 
		/// This property's value is broadcasted by the Messenger's default instance when it changes.
		/// </summary>
		public bool IsSelected
		{
			get
			{
				return _IsSelected;
			}

			set
			{
				if (_IsSelected == value)
					return;

				_IsSelected = value;
				RaisePropertyChanged(IsSelectedPropertyName);
				IsSelectedPropertyChange();

			}
		}
		/// <summary>
		/// The <see cref="IsSelectedOld" /> property's name.
		/// </summary>

		public bool IsSelectedOld
		{
			get;
			set;
		}

		public Action IsSelectedPropertyChange = delegate { };

		private T _item;

		public T Item
		{
			get { return _item; }
			set { _item = value; }
		}



		public event PropertyChangedEventHandler PropertyChanged = delegate { };


		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
