// Wms3pl.WpfClients.SharedViews
// Author: Charles@BankPro 
// 
// Final Update: 2012/04/25
// Copyright@2012

using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.Utility;

namespace Wms3pl.WpfClients.SharedViews.ViewModel
{
  public class SettingsWindowViewModel : ViewModelBase
  {
    private readonly Wms3plSettings _settings;

    public SettingsWindowViewModel()
    {
      var sl = new ServiceLocator();
      var settingsService = sl.GetService<ISettingStorage>("");
      _settings = settingsService.Load(Wms3plSession.CurrentUserInfo.Account);

      OkCommand = new RelayCommand(() =>
                                     {
                                       SetSetting();
                                       RequestClose();
                                     });
    }

    public RelayCommand OkCommand { get; private set; }

    public Wms3plSettings Settings
    {
      get { return _settings; }
    }

    public List<int> AvailableFontSizes
    {
      get { return new List<int> {14, 16, 18}; }
    }

    public event Action RequestClose = delegate { };

    private void SetSetting()
    {
      Settings.ApplySettings();
      //save settings
      var sl = new ServiceLocator();
      var settingsService = sl.GetService<ISettingStorage>("");
      settingsService.Save(Settings, Wms3plSession.CurrentUserInfo.Account);
      Wms3plSession.Set(Settings);
    }
  }
}