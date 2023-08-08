using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClients.SharedViews.ViewModel
{
  public partial class FunctionNavigator_ViewModel : ViewModelBase
  {
    private List<Function> _currentPolicyFunctions;

    public void LoadFunctions()
    {
      if (!this.IsInDesignMode)
      {
        var formService = new FormService();
        //所有目前使用者可用的功能
        var account = Wms3plSession.Get<UserInfo>().Account;
				var loginService = new LoginService();
				var user = loginService.GetUserData(account);
				var menuStyle = (user != null) ? user.MENUSTYLE : "0";
				var menuCode = (user != null) ? user.MENU_CODE : "000";
				_currentPolicyFunctions = FormService.LoadCurrentFormsByMenuStyle(account, menuStyle, menuCode);

				//所有目前使用者喜好的功能
				var currentPreferredFunctions = formService.LoadCurrentPreferredForms(account);

        foreach (var preferredFunction in currentPreferredFunctions)
        {
          var function = FindFunctionInTree(_currentPolicyFunctions, preferredFunction.Id);
          if (function != null && function.Functions.Count == 0) function.IsChecked = true;
        }
       
        Functions = new ObservableCollection<Function>(_currentPolicyFunctions);

        // 移除開頭為P80的功能(PDA功能)
        var filterP80 = Functions.Where(x=>x.Id.StartsWith("P80")).FirstOrDefault();
        Functions.Remove(filterP80);
        
        FilteredFunctions = Functions.ToList();
#if DEBUG
        CodedFunctions = formService.LoadCurrentCodedForms(account).MakeTree().ToList();
#endif
      }
    }

    private Function FindFunctionInTree(List<Function> functions, string targetId)
    {
      var firstFunction = functions.FirstOrDefault();
      if (firstFunction == null) return null;

      var findString = targetId.Substring(0, firstFunction.Level * 2 + 1);
      var targetFunction = functions.FirstOrDefault(f => f.Id.StartsWith(findString));
      if (targetFunction == null) return null;
      if (targetId == targetFunction.Id)
        return targetFunction;
      else
        return FindFunctionInTree(targetFunction.Functions, targetId);
    }

    #region IsBusy
    /// <summary>
    /// The <see cref="IsBusy" /> property's name.
    /// </summary>
    public const string IsBusyPropertyName = "IsBusy";

    private bool _isBusy = false;

    /// <summary>
    /// Gets the IsBusy property.
    /// </summary>
    public bool IsBusy
    {
      get
      {
        return _isBusy;
      }

      set
      {
        if (_isBusy == value)
        {
          return;
        }

        _isBusy = value;
        RaisePropertyChanged(IsBusyPropertyName);
      }
    }
    #endregion

    #region Functions
    /// <summary>
    /// The <see cref="Functions" /> property's name.
    /// </summary>
    public const string FunctionsPropertyName = "Functions";

    private ObservableCollection<Function> _functions = new ObservableCollection<Function>();

    /// <summary>
    /// Gets the Functions property.
    /// </summary>
    public ObservableCollection<Function> Functions
    {
      get
      {
        return _functions;
      }

      set
      {
        if (_functions == value)
        {
          return;
        }

        _functions = value;

        RaisePropertyChanged(FunctionsPropertyName);
      }
    }
    #endregion

    #region FilteredFunctions
    /// <summary>
    /// The <see cref="FilteredFunctions" /> property's name.
    /// </summary>
    public const string FilteredFunctionsPropertyName = "FilteredFunctions";

    private List<Function> _filteredFunctions = null;

    /// <summary>
    /// Gets the FilteredFunctions property.
    /// </summary>
    public List<Function> FilteredFunctions
    {
      get
      {
        return _filteredFunctions;
      }

      set
      {
        if (_filteredFunctions == value)
        {
          return;
        }
        _filteredFunctions = value;
        RaisePropertyChanged(FilteredFunctionsPropertyName);
      }
    }
    #endregion

    #region CodedFunctions
    /// <summary>
    /// The <see cref="CodedFunctions" /> property's name.
    /// </summary>
    public const string CodedFunctionsPropertyName = "CodedFunctions";

    private List<Function> _Codedfunctions = null;

    /// <summary>
    /// Gets the CodedFunctions property.
    /// </summary>
    public List<Function> CodedFunctions
    {
      get
      {
        return _Codedfunctions;
      }

      set
      {
        if (_Codedfunctions == value)
        {
          return;
        }

        _Codedfunctions = value;

        RaisePropertyChanged(CodedFunctionsPropertyName);
      }
    }
    #endregion

    internal void RemoveAllPreferredFunction()
    {
      foreach (var function in Functions)
        function.IsChecked = false;
    }
  }
}
