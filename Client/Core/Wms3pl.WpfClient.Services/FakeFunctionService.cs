using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;

namespace Wms3pl.WpfClient.Services
{
  public partial class FakeFunctionService : IFunctionService
  {
    public FakeFunctionService()
    {
    }
    private List<Function> LoadFunctions(string xml)
    {
      XElement root = XElement.Parse(xml);

      var q = from i in root.Elements("DATA_RECORD")
              select new Function()
              {
                Id = (string)i.Element("FUN_CODE"),
                Name = (string)i.Element("FUN_NAME"),
              };

      //MakeTree

      List<Function> q2 = MakeTree(q.ToList());

      return q2.ToList();
    }



    private List<Function> MakeTree(List<Function> list)
    {
      var level1Functions = new List<Function>();

      //對於每個 Function，找到 Parent
      foreach (var function in list)
      {
        var curentId = function.Id.TrimEnd('0');
        if ((curentId.Length % 2) == 1) curentId += '0';
        if (curentId.Length != 2)
        {
          var parentId = curentId.Substring(0, curentId.Length - 2).PadRight(10, '0');
          var parentFunction = list.SingleOrDefault(x => x.Id == parentId);
          if (parentFunction != null)
          {
            function.Parent = parentFunction;
            function.Parent.Functions.Add(function);
          }
        }
        else
        {
          level1Functions.Add(function);
        }
      }

      return level1Functions;
    }

    #region IFunctionService Members

    public List<Function> LoadAllFunctions(string account, bool makeTree)
    {
      return LoadFunctions("");
    }

    public List<Function> LoadPerfferedFunctions(string account, bool makeTree)
    {
      return LoadFunctions("");
    }

    #endregion

    public IEnumerable<Function> LoadAllFunctions(string account)
    {
      return new List<Function>() {};
    }

    public IEnumerable<Function> LoadPerfferedFunctions(string account)
    {
      return new List<Function>() { };
    }

    IEnumerable<Function> IFunctionService.LoadAllFunctions()
    {
      return LoadAllFunctions("");
    }

    public Function GetFunction(string id)
    {
      return new Function() { Id = id};
    }

   


    public List<Function> LoadAllFunctions(bool makeTree)
    {
      throw new NotImplementedException();
    }


    public IEnumerable<Function> MakeTree(IEnumerable<Function> functions)
    {
      FunctionService service = new FunctionService();
      return service.MakeTree(functions);
    }


		public IEnumerable<Common.FunctionShowInfo> GetFunctionShowInfos(string account)
		{
			var exProxy = ConfigurationExHelper.GetExProxy<P19ExDataSource>(false, "FunctionService");
			var q = exProxy.CreateQuery<Wms3pl.WpfClient.ExDataServices.P19ExDataService.FunctionShowInfo>("GetFunctionShowInfos")
				.AddQueryOption("account", account);

			return (from a in q
							select new Wms3pl.WpfClient.Common.FunctionShowInfo
							{
								GRP_ID = a.GRP_ID,
								SHOWINFO = a.SHOWINFO,
								FUN_CODE = a.FUN_CODE
							}
							).ToList();
		}
	}
}
