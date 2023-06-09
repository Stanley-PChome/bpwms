using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common
{
	public static class ProcessHelper
	{
		private static int timeOutLimit = 30 * 1000;
		public static KeyValuePair<bool, string> RunProcess(string command, string arguments = "", string processPath = @"C:\Windows\System32\cmd.exe")
		{

			var executeResult = string.Empty;
			var errorResult = string.Empty;
			var check = true;
			//command = command.Trim().TrimEnd('&') + "&exit";
			//註記：不管命令是否成功均執行exit命令，否則當調用ReadToEnd()方法時，會處於假死狀態
			var processStartInfo = new ProcessStartInfo(processPath)
			{
				Arguments = string.IsNullOrWhiteSpace(arguments) ? string.Empty : arguments,
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			using (var process = new System.Diagnostics.Process())
			{

				process.StartInfo = processStartInfo;
				process.Start();

				if (!string.IsNullOrWhiteSpace(command))
				{

					process.StandardInput.AutoFlush = true;
					process.StandardInput.WriteLine(command);
					process.StandardInput.WriteLine("exit");
					process.StandardInput.Close();
				}

				process.WaitForExit(timeOutLimit);
				//獲取cmd視窗的輸出信息
				executeResult = process.StandardOutput.ReadToEnd();

				//using (var fs = File.OpenWrite(Path.Combine(folder, "consoleOutput.log")))
				//proc.StandardOutput.BaseStream.CopyTo(fs);
				//using (var fs = File.OpenWrite(Path.Combine(folder, "consoleError.log")))
				//proc.StandardError.BaseStream.CopyTo(fs);

				var exitCode = process.ExitCode;
				if (exitCode != 0)	// 沒成功
				{
					check = false;
					errorResult = process.StandardError.ReadToEnd();
				}

				if (!process.HasExited)//檢查是否已經終止
				{
					if (process.Responding)//是否還有回應
					{
						process.CloseMainWindow();//關閉使用者介面的處理序
					}
					else
					{
						process.Kill();//強制關閉相關處理序
					}
				}
				process.Close();
			}
			/*
			 * 有時候錯誤的指令不會回傳Error訊息，而是在執行程序上顯示Help提示訊息
			 * 所以判斷執行失敗，還要加上判斷errorResult不能為空，否則回傳執行視窗
			 */
			return new KeyValuePair<bool, string>(check,
				(check == false ? (string.IsNullOrWhiteSpace(errorResult) ? executeResult : errorResult) : executeResult));
		}


		public static bool ExecuteCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return false;
			}

			// 執行程式
			var result = RunProcess(command);
			if (!result.Key)
			{
				return false;
			}

			return true;
		}
	}
}
