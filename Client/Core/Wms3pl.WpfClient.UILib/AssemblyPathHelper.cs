using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.UILib
{
    class AssemblyHelper
    {
        public static Type GetUserControlType(Function function)
        {
            var currentAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
            var type = GetUserControlType(function, currentAssemblyName);
            if (type != null) return type;

            type = GetUserControlType(function, "Wms3pl.WpfClient");
            return type;
        }

        private static Type GetUserControlType(Function function, string currentAssemblyName)
        {
            string moduleId = function.Id.Substring(0, 3);
            string targetNameSpace = string.IsNullOrEmpty(function.DllPath)
                                       ? string.Format("{0}.{1}", currentAssemblyName, moduleId)
                                       : Path.GetFileNameWithoutExtension(function.DllPath);

            string assemblyName = string.IsNullOrEmpty(function.DllPath)
                                    ? string.Format("{0}.dll", targetNameSpace)
                                    : function.DllPath;

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName);

            if (!File.Exists(path)) return null;
            var assembly = Assembly.LoadFrom(assemblyName);
            var fundId = string.Format("{0}.Views.{1}", targetNameSpace, function.Id);
            var type = assembly.GetType(fundId);
            return type;
        }
    }
}
