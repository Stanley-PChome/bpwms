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
using Wms3pl.WpfClient.P71.Entities;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.ExDataServices;
using AutoMapper;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public class P7101010101_ViewModel : InputViewModelBase
	{
		public Action ClosedSuccessClick = delegate { };
		public Action ClosedCancelClick = delegate { };
		public P7101010101_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				FloorIsSelect = true;
				ChannelIsSelect = true;
				PlainIsSelect = true;
			}

		}

		private SelectionList<string> _floorList;
		public SelectionList<string> FloorList
		{
			get { return _floorList; }
			set { Set(ref _floorList, value); }
		}

		private SelectionList<string> _channelList;
		public SelectionList<string> ChannelList
		{
			get { return _channelList; }
			set { Set(ref _channelList, value); }
		}

		private SelectionList<string> _plainList;
		public SelectionList<string> PlainList
		{
			get { return _plainList; }
			set { Set(ref _plainList, value); }
		}

		private SelectionList<string> _locLevelList;
		public SelectionList<string> LocLevelList
		{
			get { return _locLevelList; }
			set { Set(ref _locLevelList, value); }
		}

		private SelectionList<string> _locTypeList;
		public SelectionList<string> LocTypeList
		{
			get { return _locTypeList; }
			set { Set(ref _locTypeList, value); }
		}

		private SelectionItem<string> _selectedFloor;
		public SelectionItem<string> SelectedFloor
		{
			get { return _selectedFloor; }
			set
			{
				Set(ref _selectedFloor, value);
				if (_selectedFloor != null)
				{
					_selectedFloor.IsSelectedPropertyChange = () =>
					{
						if (_selectedFloor.IsSelected)
							_tempData.AddRange(_datas.Where(o => o.FloorNo == _selectedFloor.Item).ToList());
						else
							_tempData.RemoveAll(o => o.FloorNo == _selectedFloor.Item);
						BindChannelGrid();
					};
					BindChannelGrid();
				}
			}
		}

		private SelectionItem<string> _selectedChannel;
		public SelectionItem<string> SelectedChannel
		{
			get { return _selectedChannel; }
			set
			{
				Set(ref _selectedChannel, value);
				if (_selectedChannel != null)
				{
					_selectedChannel.IsSelectedPropertyChange = () =>
					{
						if (_selectedChannel.IsSelected)
							_tempData.AddRange(_datas.Where(o => o.FloorNo == _selectedFloor.Item && o.ChannelNo == _selectedChannel.Item).ToList());
						else
							_tempData.RemoveAll(o => o.FloorNo == _selectedFloor.Item && o.ChannelNo == _selectedChannel.Item);
						BindPlainGrid();
					};
					BindPlainGrid();
				}
			}
		}

		private SelectionItem<string> _selectedPlain;
		public SelectionItem<string> SelectedPlain
		{
			get { return _selectedPlain; }
			set
			{
				Set(ref _selectedPlain, value);
				if (_selectedPlain != null)
				{
					_selectedPlain.IsSelectedPropertyChange = () =>
					{
						if (_selectedPlain.IsSelected)
							_tempData.AddRange(_datas.Where(o => o.FloorNo == _selectedFloor.Item && o.ChannelNo == _selectedChannel.Item && o.PlainNo == _selectedPlain.Item).ToList());
						else
							_tempData.RemoveAll(o => o.FloorNo == _selectedFloor.Item && o.ChannelNo == _selectedChannel.Item && o.PlainNo == _selectedPlain.Item);
						
						BindLocLevelGrid();
					};
					BindLocLevelGrid(); 
				}
			}
		}

		private SelectionItem<string> _selectedLocLevel;
		public SelectionItem<string> SelectedLocLevel
		{
			get { return _selectedLocLevel; }
			set
			{
				Set(ref _selectedLocLevel, value);
				LocTypeList = new SelectionList<string>(new ObservableCollection<string>());
				if (_selectedLocLevel != null)
				{
					_selectedLocLevel.IsSelectedPropertyChange = () =>
					{
						if (_selectedLocLevel.IsSelected)
							_tempData.AddRange(_datas.Where(o => o.FloorNo == _selectedFloor.Item && o.ChannelNo == _selectedChannel.Item && o.PlainNo == _selectedPlain.Item && o.LocLevelNo == _selectedLocLevel.Item).ToList());
						else
							_tempData.RemoveAll(o => o.FloorNo == _selectedFloor.Item && o.ChannelNo == _selectedChannel.Item && o.PlainNo == _selectedPlain.Item && o.LocLevelNo == _selectedLocLevel.Item);

					};
					var n = _datas.Except(_tempData);
					var locTypes = _datas.Where(o => o.FloorNo == SelectedFloor.Item && o.ChannelNo == SelectedChannel.Item && o.PlainNo == SelectedPlain.Item && o.LocLevelNo == SelectedLocLevel.Item).Select(o => o.LocTypeNo).ToList();
					foreach (var locType in locTypes)
					{
						var locTypeData = new SelectionItem<string>(locType, !n.Where(o => o.FloorNo == SelectedFloor.Item && o.ChannelNo == SelectedChannel.Item && o.PlainNo == SelectedPlain.Item && o.LocLevelNo == SelectedLocLevel.Item && o.LocLevelNo == locType).Any());
						LocTypeList.Add(locTypeData);
					}
				}
			}
		}

		private bool _floorIsSelect;
		public bool FloorIsSelect
		{
			get { return _floorIsSelect; }
			set { Set(ref _floorIsSelect, value); }
		}

		private bool _channelIsSelect;
		public bool ChannelIsSelect
		{
			get { return _channelIsSelect; }
			set { Set(ref _channelIsSelect, value); }
		}

		private bool _plainIsSelect;
		public bool PlainIsSelect
		{
			get { return _plainIsSelect; }
			set { Set(ref _plainIsSelect, value); }
		}

		private ObservableCollection<P710101MasterData> _datas;
		private List<P710101MasterData> _tempData;
		public List<P710101MasterData> TempData
		{
			get { return _tempData; }
			set { Set(ref _tempData, value); }
		}

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => true, o =>
					{
						ClosedCancelClick();
					}
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => true, o =>
					{
						ClosedSuccessClick();
					}
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Save

		public void BindData(ObservableCollection<P710101MasterData> datas)
		{
			var floors = datas.Select(o => o.FloorNo).ToList().Distinct().ToList();
			_datas = datas;
			_tempData = Mapper.Map<List<P710101MasterData>, List<P710101MasterData>>(_datas.ToList());
			foreach (var floor in floors)
			{
				var floorData = new SelectionItem<string>(floor, true);
				if (FloorList == null || !FloorList.Any())
					FloorList = new SelectionList<string>(new List<string>());
				FloorList.Add(floorData);
			}
			SelectedFloor = FloorList.FirstOrDefault();
		}

		private void BindChannelGrid()
		{
			FloorIsSelect = _selectedFloor.IsSelected;
			ChannelList = new SelectionList<string>(new ObservableCollection<string>());
			var n = _datas.Except(_tempData);
			var channels = _datas.Where(o => o.FloorNo == _selectedFloor.Item).Select(o => o.ChannelNo).ToList().Distinct().ToList();
			foreach (var channel in channels)
			{
				var nowLoc = _datas.Where(o => o.FloorNo == SelectedFloor.Item && o.ChannelNo == channel).Count();
				var channelData = new SelectionItem<string>(channel, n.Where(o => o.FloorNo == SelectedFloor.Item && o.ChannelNo == channel).Count() != nowLoc);
				ChannelList.Add(channelData);
			}
			SelectedChannel = ChannelList.FirstOrDefault();
		}

		private void BindPlainGrid()
		{
			ChannelIsSelect = _selectedChannel.IsSelected;
			PlainList = new SelectionList<string>(new ObservableCollection<string>());
			var plains = _datas.Where(o => o.FloorNo == SelectedFloor.Item && o.ChannelNo == SelectedChannel.Item).Select(o => o.PlainNo).Distinct().ToList();
			var n = _datas.Except(_tempData);
			foreach (var plain in plains)
			{
				var nowLoc = _datas.Where(o => o.FloorNo == SelectedFloor.Item && o.ChannelNo == SelectedChannel.Item && o.PlainNo == plain).Count();
				var plainData = new SelectionItem<string>(plain, n.Where(o => o.FloorNo == SelectedFloor.Item && o.ChannelNo == SelectedChannel.Item && o.PlainNo == plain).Count() != nowLoc);
				PlainList.Add(plainData);
			}
			SelectedPlain = PlainList.FirstOrDefault();
		}

		private void BindLocLevelGrid()
		{
			PlainIsSelect = _selectedPlain.IsSelected;
			LocLevelList = new SelectionList<string>(new ObservableCollection<string>());
			var locLevels = _datas.Where(o => o.FloorNo == SelectedFloor.Item && o.ChannelNo == SelectedChannel.Item && o.PlainNo == SelectedPlain.Item).Select(o => o.LocLevelNo).Distinct().ToList();
			var n = _datas.Except(_tempData);
			foreach (var locLevel in locLevels)
			{
				var nowLoc = _datas.Where(o => o.FloorNo == SelectedFloor.Item && o.ChannelNo == SelectedChannel.Item && o.PlainNo == SelectedPlain.Item && o.LocLevelNo == locLevel).Count();
				var locLevelData = new SelectionItem<string>(locLevel, n.Where(o => o.FloorNo == SelectedFloor.Item && o.ChannelNo == SelectedChannel.Item && o.PlainNo == SelectedPlain.Item && o.LocLevelNo == locLevel).Count() != nowLoc);
				LocLevelList.Add(locLevelData);
			}
			SelectedLocLevel = LocLevelList.FirstOrDefault();
		}
	}
}
