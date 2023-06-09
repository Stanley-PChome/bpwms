using System.Collections.Generic;
using System.Windows;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.UILib
{
  public interface IFormService
  {
    /// <summary>
    ///   載入目前使用者喜好的 Functions
    /// </summary>
    /// <param name = "dc"></param>
    /// <param name = "account"></param>
    /// <returns></returns>
    List<Function> LoadCurrentPreferredForms(string account);

    /// <summary>
    ///   載入目前已開發的 Function
    /// </summary>
    /// <param name = "dc"></param>
    /// <returns></returns>
    List<Function> LoadCurrentCodedForms(string account);

    object AddFunctionForm(string functionId, Window owner = null, params object[] parameters);
    object GetUserControl(Function function, params object[] parameters);

    /// <summary>
    ///   增加新視窗
    /// </summary>
    /// <param name = "function"></param>
    /// <param name="owner"> </param>
    /// <param name="parameters"></param>
    /// <param name="style"> </param>
    object AddFunctionForm(Function function,
                                           Style style = null, Window owner = null, params object[] parameters);
  }
}