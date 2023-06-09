// Wms3pl.WpfClient.Services
// Author: Charles@BankPro 
// 
// Final Update: 2012/04/25
// Copyright@2012

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace Wms3pl.WpfClient.Services
{
  [Serializable]
  public class Wms3plSettings : INotifyPropertyChanged
  {
    public Wms3plSettings()
    {
      FunctionIds = new List<string>();
      PreferredFunctionIds = new List<string>();
      NeedLeavingConfirmation = true;
    }


    /// <summary>
    ///   上次離開程式時，未關閉的功能 Id
    /// </summary>
    public List<string> FunctionIds { get; set; }

    public List<string> PreferredFunctionIds { get; set; }

    public bool AutoHideMenu { get; set; }

    #region NeedLeavingConfirmation

    /// <summary>
    ///   The <see cref="NeedLeavingConfirmation" /> property's name.
    /// </summary>
    public const string NeedLeavingConfirmationPropertyName = "NeedLeavingConfirmation";

    private bool _NeedLeavingConfirmation = true;

    /// <summary>
    ///   離開主程式時，是否要詢問
    /// </summary>
    public bool NeedLeavingConfirmation
    {
      get { return _NeedLeavingConfirmation; }

      set
      {
        if (_NeedLeavingConfirmation == value)
        {
          return;
        }

        var oldValue = _NeedLeavingConfirmation;
        _NeedLeavingConfirmation = value;

        // Update bindings, no broadcast
        RaisePropertyChanged(NeedLeavingConfirmationPropertyName);
      }
    }

    #endregion

    #region NeedContinueUnClosedFunctions

    /// <summary>
    ///   The <see cref="NeedContinueUnClosedFunctions" /> property's name.
    /// </summary>
    public const string NeedContinueUnClosedFunctionsPropertyName = "NeedContinueUnClosedFunctions";

    private bool _NeedContinueUnClosedFunctions = false;

    /// <summary>
    ///   要繼續執行上次離開程式時，未關閉的功能
    /// </summary>
    public bool NeedContinueUnClosedFunctions
    {
      get { return _NeedContinueUnClosedFunctions; }

      set
      {
        if (_NeedContinueUnClosedFunctions == value)
        {
          return;
        }

        var oldValue = _NeedContinueUnClosedFunctions;
        _NeedContinueUnClosedFunctions = value;

        // Update bindings, no broadcast
        RaisePropertyChanged(NeedContinueUnClosedFunctionsPropertyName);
      }
    }

    #endregion

    #region CurrentCulture

    /// <summary>
    /// The <see cref="CurrentCulture" /> property's name.
    /// </summary>
    public const string CurrentCulturePropertyName = "CurrentCulture";

    private string _currentCulture = "zh-Hant";

    /// <summary>
    /// Gets the CurrentCulture property.
    /// </summary>
    public string CurrentCulture
    {
      get
      {
        return _currentCulture;
      }

      set
      {
        if (_currentCulture == value) return;
        _currentCulture = value;
        RaisePropertyChanged(CurrentCulturePropertyName);
      }
    }
    #endregion CurrentCulture	

    #region INotifyPropertyChanged Members

    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    #endregion

    public void RaisePropertyChanged(string propertyName)
    {
      PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}