using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Windows;

namespace Wms3pl.WpfClientStarter
{
    class Program
    {
        [System.STAThreadAttribute()]
        static void Main(string[] args)
        {
            string appName = "Wms3pl.WpfClient";
            if (args.Length > 0)
                appName = args[0];

            int result;
            try
            {
                do
                {
                    RunAppInDomain(appName, args);
                    var tempPath = GetTempPath();
                    if (File.Exists(tempPath))
                    {
                        var text = File.ReadAllText(tempPath);
                        result = int.Parse(text);
                    }
                    else
                    {
                        result = 0;
                    }
                } while (result == 999);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "錯誤");
            }
        }

        private static void ShowUsage()
        {
            MessageBox.Show("Wms3pl 主程式: Wms3pl.WpfClientStarter Wms3pl.WpfClient\nWms3pl RF程式: Wms3pl.WpfClientStarter Wms3pl.RFClient");
        }

        private static int RunAppInDomain(string appName, string[] args)
        {
            var strShadowCopyFiles = "false";
            var settings = ConfigurationManager.AppSettings["ShadowCopyFiles"];
            if (!string.IsNullOrWhiteSpace(settings))
                strShadowCopyFiles = settings;
            var ads = new AppDomainSetup
                        {
                            ApplicationBase = System.Environment.CurrentDirectory,
                            DisallowBindingRedirects = false,
                            DisallowCodeDownload = true,
                            //ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
                            //ConfigurationFile = "Wms3pl.WpfClient.exe.config",
                            ConfigurationFile = string.Format("{0}.exe.config", appName),
                            ShadowCopyFiles = strShadowCopyFiles,
                        };
            ads.LoaderOptimization = LoaderOptimization.SingleDomain;
            //ads.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var baseEvidence = AppDomain.CurrentDomain.Evidence;
            var evidence = new Evidence(baseEvidence);
            var domain = AppDomain.CreateDomain(appName, evidence, ads);
            var newArgs = new List<string>() { appName };
            newArgs.AddRange(args);
            int exitCode = domain.ExecuteAssembly(string.Format("{0}.exe", newArgs.ToArray()));
            return exitCode;
        }

        private static string GetTempPath()
        {
            var id = Process.GetCurrentProcess().Id;
            string path = Path.Combine(Path.GetTempPath(), id.ToString());
            return path;
        }
    }
}

