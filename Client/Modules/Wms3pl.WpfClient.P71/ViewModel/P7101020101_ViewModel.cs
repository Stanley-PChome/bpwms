using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
//using Wms3pl.Datas.F19;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.P71.Entities;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices.P71WcfService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public class P7101020101_ViewModel : InputViewModelBase
	{
		public Action ClosedSuccessClick = delegate { };
		public Action ClosedCancelClick = delegate { };
		public P7101020101_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
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

		private SelectionList<P710101DetailData> _locCodeList;
		public SelectionList<P710101DetailData> LocCodeList
		{
			get { return _locCodeList; }
			set { Set(ref _locCodeList, value); }
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
                        {
                            _tempData.AddRange(_dataDetails.Where(o => o.LOC_CODE.Substring(0, 1) == _selectedFloor.Item 
                            && !_tempData.Contains(o.LOC_CODE)
                            && o.STATUS == "0").Select(o => o.LOC_CODE).ToList());
                        }
						else
                        {
                            var removeDetails = _dataDetails.Where(o => o.LOC_CODE.Substring(0, 1) == _selectedFloor.Item
                            && _tempData.Contains(o.LOC_CODE)
                            && o.STATUS == "0");

                            _tempData = _tempData.Except(removeDetails.Select(x => x.LOC_CODE)).ToList();
                        }
							
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
                        {
                            _tempData.AddRange(_dataDetails.Where(o => o.LOC_CODE.Substring(0, 1) == _selectedFloor.Item 
                            && o.LOC_CODE.Substring(1, 2) == _selectedChannel.Item 
                            && !_tempData.Contains(o.LOC_CODE)
                            && o.STATUS == "0").Select(o => o.LOC_CODE).ToList());
                        }
                        else
                        {
                            var removeDetails = _dataDetails.Where(o => o.LOC_CODE.Substring(0, 1) == _selectedFloor.Item 
                            && o.LOC_CODE.Substring(1, 2) == _selectedChannel.Item 
                            && _tempData.Contains(o.LOC_CODE)
                            && o.STATUS == "0");

                            _tempData = _tempData.Except(removeDetails.Select(x => x.LOC_CODE)).ToList();
                        }
							
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
                        {
                            _tempData.AddRange(_dataDetails.Where(o => o.LOC_CODE.Substring(0, 1) == _selectedFloor.Item
                            && o.LOC_CODE.Substring(1, 2) == _selectedChannel.Item
                            && o.LOC_CODE.Substring(3, 2) == _selectedPlain.Item
                            && !_tempData.Contains(o.LOC_CODE)
                            && o.STATUS == "0").Select(o => o.LOC_CODE).ToList());
                        }
                        else
                        {
                            var removeDetails = _dataDetails.Where(o => o.LOC_CODE.Substring(0, 1) == _selectedFloor.Item
                            && o.LOC_CODE.Substring(1, 2) == _selectedChannel.Item
                            && o.LOC_CODE.Substring(3, 2) == _selectedPlain.Item
                            && _tempData.Contains(o.LOC_CODE)
                            && o.STATUS == "0");

                            _tempData = _tempData.Except(removeDetails.Select(x => x.LOC_CODE)).ToList();
                        }

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
				if (_selectedLocLevel != null)
				{
					_selectedLocLevel.IsSelectedPropertyChange = () =>
					{
                        if (_selectedLocLevel.IsSelected)
                        {
                            var filterDetails = _dataDetails.Where(o => o.LOC_CODE.Substring(0, 1) == _selectedFloor.Item
                            && o.LOC_CODE.Substring(1, 2) == _selectedChannel.Item
                            && o.LOC_CODE.Substring(3, 2) == _selectedPlain.Item
                            && o.LOC_CODE.Substring(5, 2) == _selectedLocLevel.Item
                            && !_tempData.Contains(o.LOC_CODE)
                            && o.STATUS == "0").Select(o => o.LOC_CODE).ToList();

                            _tempData.AddRange(filterDetails);

                        }
                        else
                        {
                            var removeDetails = _dataDetails.Where(o => o.LOC_CODE.Substring(0, 1) == _selectedFloor.Item
                            && o.LOC_CODE.Substring(1, 2) == _selectedChannel.Item
                            && o.LOC_CODE.Substring(3, 2) == _selectedPlain.Item
                            && o.LOC_CODE.Substring(5, 2) == _selectedLocLevel.Item
                            && o.STATUS == "0");

                            _tempData = _tempData.Except(removeDetails.Select(x => x.LOC_CODE)).ToList();
                        }
                        BindLocCodeGrid();
					};
					BindLocCodeGrid();
				}
			}
		}

		private ObservableCollection<P710102MasterData> _datas;
		private List<P710101DetailData> _dataDetails;
		private List<string> _tempData;
		public List<string> TempData
		{
			get { return _tempData; }
			set { Set(ref _tempData, value); }
		}

		private List<NameValuePair<string>> _locStatusList;
		public List<NameValuePair<string>> LocStatusList
		{
			get { return _locStatusList; }
			set { Set(ref _locStatusList, value); }
		}

		private ObservableCollection<NameValuePair<string>> _areaType;
		public ObservableCollection<NameValuePair<string>> AreaType
		{
			get { return _areaType; }
			set { Set(ref _areaType, value); }
		}

		private List<string> _selectedLocList;
		public List<string> SelectedLocList
		{
			get { return _selectedLocList; }
			set { Set(ref _selectedLocList, value); }
		}

		private SelectionItem<P710101DetailData> _selectedLocCode;
		public SelectionItem<P710101DetailData> SelectedLocCode
		{
			get { return _selectedLocCode; }
			set
			{
				Set(ref _selectedLocCode, value);
				if (_selectedLocCode != null)
				{
					_selectedLocCode.IsSelectedPropertyChange = () =>
					{
						if (_selectedLocCode.IsSelected)
							_tempData.Add(_selectedLocCode.Item.LOC_CODE);
						else
							_tempData.RemoveAll(o => o == _selectedLocCode.Item.LOC_CODE);
					};
				}
			}
		}

        private string _areaName = "";
        public string AreaName
        {
            get { return _areaName; }
            set { _areaName = value; }
        }

        private string _areaCode = "";
        public string AreaCode
        {
            get { return _areaCode; }
            set { _areaCode = value; }
        }

        #region Cancel
        public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => true,o => ClosedCancelClick()
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
					o => DoSave(), () => true,o => ClosedSuccessClick()
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作			
			UserOperateMode = OperateMode.Query;
		}
		#endregion Save

		public void BindData(ObservableCollection<P710102MasterData> datas, List<P710101DetailData> OldDetailDataList, List<string> selectedLoc, ObservableCollection<NameValuePair<string>> areaType)
		{
            var areaTypeData = areaType.Where(x => x.Name == _areaName).FirstOrDefault();
            _areaCode = areaTypeData?.Value;

            LocStatusList = GetBaseTableService.GetF000904List(FunctionCode, "P7101010000", "LocStatus");
			AreaType = areaType;
            var floors = datas.Select(o => o.Floor).ToList().Distinct().ToList();
			_datas = datas;
			SelectedLocList = selectedLoc;
			_tempData = Mapper.Map<List<string>, List<string>>(SelectedLocList);
			foreach (var floor in floors)
			{
				var floorData = new SelectionItem<string>(floor, false);
				if (FloorList == null || !FloorList.Any())
					FloorList = new SelectionList<string>(new List<string>());
				FloorList.Add(floorData);
			}
			_dataDetails = OldDetailDataList.OrderBy(o => o.LOC_CODE).ToList();
            SelectedFloor = FloorList.FirstOrDefault();
			LocCodeList = new SelectionList<P710101DetailData>(new ObservableCollection<P710101DetailData>());
			foreach (var locData in _dataDetails)
			{
				var gridData = new P710101DetailData();

				gridData = ExDataMapper.Map<P710101DetailData, P710101DetailData>(locData);
				gridData.IsEditData = gridData.STATUS != "1";
                var itemData = new SelectionItem<P710101DetailData>(gridData, selectedLoc.Contains(locData.LOC_CODE));
				LocCodeList.Add(itemData);
			}

		}

		private void BindChannelGrid()
		{
			ChannelList = new SelectionList<string>(new ObservableCollection<string>());
			var channels = _datas.Where(o => o.Floor == _selectedFloor.Item).Select(o => o.ChannelNo).ToList().Distinct().ToList();
			foreach (var channel in channels)
			{
				var LocCount = _tempData.Where(o => o.Substring(0, 1) == _selectedFloor.Item && o.Substring(1, 2) == channel).ToList().Count();
				var channelCount = _dataDetails.Where(o => (o.STATUS == "1" && o.AREA_CODE == _areaCode ? true : o.STATUS == "1" && o.AREA_CODE != _areaCode ? false : o.STATUS == "0") && o.LOC_CODE.Substring(0, 1) == _selectedFloor.Item && o.LOC_CODE.Substring(1, 2) == channel).ToList().Count();
				var channelData = new SelectionItem<string>(channel, LocCount == channelCount);
				ChannelList.Add(channelData);
			}
			SelectedChannel = ChannelList.FirstOrDefault();
		}

		private void BindPlainGrid()
		{
			PlainList = new SelectionList<string>(new ObservableCollection<string>());
			var plains = _datas.Where(o => o.Floor == SelectedFloor.Item && o.ChannelNo == SelectedChannel.Item).Select(o => o.PlainNo).Distinct().ToList();

			foreach (var plain in plains)
			{
				var LocCount = _tempData.Where(o => o.Substring(0, 1) == _selectedFloor.Item && o.Substring(1, 2) == SelectedChannel.Item && o.Substring(3, 2) == plain).ToList().Count();
				var plainCount = _dataDetails.Where(o => (o.STATUS == "1" && o.AREA_CODE == _areaCode ? true : o.STATUS == "1" && o.AREA_CODE != _areaCode ? false : o.STATUS == "0") && o.LOC_CODE.Substring(0, 1) == _selectedFloor.Item && o.LOC_CODE.Substring(1, 2) == SelectedChannel.Item && o.LOC_CODE.Substring(3, 2) == plain).ToList().Count();

				var plainData = new SelectionItem<string>(plain, LocCount == plainCount);
				PlainList.Add(plainData);
			}
			SelectedPlain = PlainList.FirstOrDefault();
			var selectCount = ChannelList.Where(o => o.IsSelected).ToList().Count();
			if (selectCount == ChannelList.Count())
				SelectedFloor.IsSelected = true;
			else if (selectCount == 0)
				SelectedFloor.IsSelected = false;
		}

		private void BindLocLevelGrid()
		{
			LocLevelList = new SelectionList<string>(new ObservableCollection<string>());
			var locLevels = _datas.Where(o => o.Floor == SelectedFloor.Item && o.ChannelNo == SelectedChannel.Item && o.PlainNo == SelectedPlain.Item).Select(o => o.LocLevelNo).Distinct().ToList();

			foreach (var locLevel in locLevels)
			{
				var LocCount = _tempData.Where(o => o.Substring(0, 1) == _selectedFloor.Item && o.Substring(1, 2) == SelectedChannel.Item && o.Substring(3, 2) == SelectedPlain.Item && o.Substring(5, 2) == locLevel).ToList().Count();
				var locLevelCount = _dataDetails.Where(o => (o.STATUS == "1" && o.AREA_CODE == _areaCode ? true : o.STATUS == "1" && o.AREA_CODE != _areaCode ? false : o.STATUS == "0") && o.LOC_CODE.Substring(0, 1) == _selectedFloor.Item && o.LOC_CODE.Substring(1, 2) == SelectedChannel.Item && o.LOC_CODE.Substring(3, 2) == SelectedPlain.Item && o.LOC_CODE.Substring(5, 2) == locLevel).ToList().Count();

				var locLevelData = new SelectionItem<string>(locLevel, LocCount == locLevelCount);
				LocLevelList.Add(locLevelData);
			}
			SelectedLocLevel = LocLevelList.FirstOrDefault();
			var selectCount = PlainList.Where(o => o.IsSelected).ToList().Count();
			if (selectCount == PlainList.Count())
				SelectedChannel.IsSelected = true;
			else if (selectCount == 0)
				SelectedChannel.IsSelected = false;
		}

		private void BindLocCodeGrid()
		{
            LocCodeList = new SelectionList<P710101DetailData>(new ObservableCollection<P710101DetailData>());
			foreach (var locData in _dataDetails)
			{
				var gridData = new P710101DetailData();

				gridData = ExDataMapper.Map<P710101DetailData, P710101DetailData>(locData);
				gridData.IsEditData = gridData.STATUS != "1";
                
				var itemData = new SelectionItem<P710101DetailData>(gridData, locData.STATUS == "1" && locData.AREA_CODE == _areaCode ? true : _tempData.Contains(locData.LOC_CODE) && locData.STATUS == "0");
				LocCodeList.Add(itemData);
			}
			var selectCount = LocLevelList.Where(o => o.IsSelected).ToList().Count();
			if (selectCount == LocLevelList.Count())
				SelectedPlain.IsSelected = true;
			else if (selectCount == 0)
				SelectedPlain.IsSelected = false;
		}

	}
}
