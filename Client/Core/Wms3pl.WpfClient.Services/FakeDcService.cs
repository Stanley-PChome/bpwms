using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.Services
{
  public partial class FakeDcService : IDcService
  {
    public IQueryable<F1910> GetF1910sServicedByDc(string dc)
    {
      return new EnumerableQuery<F1910>(
        new List<F1910>{ new F1910(){}});
    }

    public string GetAccountName(string account)
    {
      return "wms";
    }
    
    public string GetF1901sDcName(string dc)
    {
      return "001";
    }

    public string GetCustName(string custcode)
    {
      return "金財通家庭";
    }

    public string GetUserGUP_CODE(string custCode)
    {
      return "01";
    }

    public string GetSchemaName(string dcCode, string custCode)
    {
      return "wms";
    }

	  public List<F192402Data> GetF192402Data(string account)
	  {
		  return null;
	  }

		public List<F190907Data> GetItemImagePathDatas()
		{
			return null;
		}

		public bool CheckIsCommon(string account)
		{
			return false;
		}

		public string GetFolderUser()
		{
			return string.Empty;
		}

		public string GetFolderPw()
		{			
			return string.Empty;
		}

		public string GetFolderDomain()
		{			
			return string.Empty;
		}
	}
}
