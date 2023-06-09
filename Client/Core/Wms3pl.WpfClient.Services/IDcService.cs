using System;
using System.Collections.Generic;
using System.Windows.Documents;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.Services
{
  public interface IDcService
  {
    string GetAccountName(string account);
    
    /// <summary>
    /// Get SchemaName
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="custCode"></param>
    /// <returns></returns>
    string GetSchemaName(string dcCode, string custCode);

		List<F192402Data> GetF192402Data(string account);

		bool CheckIsCommon(string account);

	  string GetFolderUser();
		string GetFolderPw();
		string GetFolderDomain();
		List<F190907Data> GetItemImagePathDatas();
  }
}
