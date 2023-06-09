using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wms3pl.WpfClient.Services
{
  public interface IFunctionService
  {
    IEnumerable<Function> LoadAllFunctions(string account);
    IEnumerable<Function> LoadPerfferedFunctions(string account);
    IEnumerable<Function> LoadAllFunctions();
    Function GetFunction(string id);

    IEnumerable<Function> MakeTree(IEnumerable<Function> functions);

		IEnumerable<Wms3pl.WpfClient.Common.FunctionShowInfo> GetFunctionShowInfos(string account);
  }
}
