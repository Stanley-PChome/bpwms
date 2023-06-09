using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Text;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;

namespace Wms3pl.WpfClient.Services
{
    public partial class FunctionService : IFunctionService
    {
        public FunctionService()
        {
            _proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "FunctionSetting");
        }

        F19Entities _proxy;
        /// <summary>
        /// 全部的功能。
        /// </summary>
        /// <remarks>
        /// 很慢，故要做 cache in memory
        /// </remarks>
        private IEnumerable<Function> _allFunctions;
        public IEnumerable<Function> LoadAllFunctions(string account)
        {
            var settings = Wms3plSession.Get<Wms3plSettings>();
            var gi = Wms3plSession.Get<GlobalInfo>();
            var useHans = ((settings != null) && (settings.CurrentCulture == "zh-Hans"));
            if (string.IsNullOrWhiteSpace(account)) throw new ArgumentException("parameter account cannot be null or empty", "account");
            var exProxy = ConfigurationExHelper.GetExProxy<P19ExDataSource>(false, "FunctionService");

            var f1954s = exProxy.CreateQuery<F1954>("GetUserFunctions")
          .AddQueryOption("account", string.Format("'{0}'", account));
            var q = from i in f1954s.ToList()
                    select new Function
                    {
                        Name = useHans ? ChineseConverter.Convert(i.FUN_NAME, ChineseConversionDirection.TraditionalToSimplified) : i.FUN_NAME,
                        Id = i.FUN_CODE.Trim()
                    };

            return q.ToList();
        }



        public IEnumerable<Function> LoadAllFunctions()
        {
            if (_allFunctions == null)
            {
                var exProxy = ConfigurationExHelper.GetExProxy<P19ExDataSource>(false, "FunctionService");

                var f1954s = exProxy.CreateQuery<FunctionCodeName>("GetAllFunctions");

                var q = from i in f1954s
                        orderby i.FUN_CODE
                        select new Function
                        {
                            Name = i.FUN_NAME,
                            Id = i.FUN_CODE.Trim()
                        };
                _allFunctions = q.ToList();
            }

            return _allFunctions;
        }

        public IEnumerable<Function> AllFunctions
        {
            get
            {
                if (_allFunctions == null)
                {
                    var exProxy = ConfigurationExHelper.GetExProxy<P19ExDataSource>(false, "FunctionService");

                    var f1954s = exProxy.CreateQuery<FunctionCodeName>("GetAllFunctions");

                    var q = from i in f1954s
                                //where i.CUST_CODE == custCode
                            orderby i.FUN_CODE
                            select new Function
                            {
                                Name = i.FUN_NAME,
                                Id = i.FUN_CODE.Trim()
                            };
                    _allFunctions = q.ToList();
                }

                return _allFunctions;
            }
        }

        public Function GetFunction(string id)
        {
            return AllFunctions.Where(i => i.Id == id).SingleOrDefault();
        }

        public IEnumerable<Function> LoadPerfferedFunctions(string account)
        {
            return new List<Function>();
        }


   

        private List<string> GetFunctionCodes(List<Function> functions)
        {
            var results = (from i in functions
                           where i.IsChecked
                           select i.Id).ToList();

            foreach (var f in functions)
                results.AddRange(GetFunctionCodes(f.Functions));

            return results;
        }

        public IEnumerable<Function> MakeTree(IEnumerable<Function> functions)
        {

            var level1Functions = new List<Function>();

            //對於每個 Function，找到 Parent
            foreach (var function in functions)
            {
                var curentId = function.Id.TrimEnd('0', ' ');
                if (((curentId.Length - 1) % 2) == 1) curentId += '0';
                if ((curentId.Length - 1) != 2)
                {
                    var parentId = curentId.Substring(0, curentId.Length - 2).PadRight(11, '0');
                    var parentFunction = functions.SingleOrDefault(x => x.Id.Trim() == parentId);
                    if (parentFunction != null)
                    {
                        function.Parent = parentFunction;
                        if (!function.Parent.Functions.Contains(function))
                            function.Parent.Functions.Add(function);
                    }
                }
                else
                {
                    level1Functions.Add(function);
                }
            }

            return level1Functions.AsEnumerable();
        }

        public IEnumerable<Wms3pl.WpfClient.Common.FunctionShowInfo> GetFunctionShowInfos(string account)
        {
            var exProxy = ConfigurationExHelper.GetExProxy<P19ExDataSource>(false, "FunctionService");
            var q = exProxy.CreateQuery<Wms3pl.WpfClient.ExDataServices.P19ExDataService.FunctionShowInfo>("GetFunctionShowInfos")
                .AddQueryOption("account", string.Format("'{0}'", account)).ToList();

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
