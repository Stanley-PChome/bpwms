using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Wms3pl.WpfClient.Common
{
	[Guid("C90E96C1-8534-4243-9530-960D9AF982CC")]
	[ComVisible(true)]
	public class WeighingCom
	{
		//初始化連線原本寫在建構子；但發現HTML <Object>載入時，就會實體化。將佔走通訊

		public bool IsTerminated = false;
		public bool IsCommunicationThreadStop = false;
		string _correctWeightValue = "";
		public string CorrectWeightValue { get { return _correctWeightValue; } }
		public SerialPort SPort;
		public String ErrorString = "NOT SET";
		public String ComDetail = "";
		public string Weight = "0";
		public bool HasInit = false;
		private bool _skipError;//是否模擬秤重機
		protected void WriteLog(string log)
		{
			try
			{
				//return;
				var sw = new StreamWriter(@"C:\WeighingCom.log", true);
				sw.WriteLine(log);
				sw.Close();
			}
			catch (Exception)
			{

			}
		}
		public void Init(bool skipError = false)
		{
			WriteLog("init");

			if (skipError)
			{
				_skipError = true;
				HasInit = true;
				return;
			}

			if (HasInit)
				return;			

			SPort = null;
			try
			{
				SpinWait.SpinUntil(() => false, 100);
				string[] names = SerialPort.GetPortNames();
				if (names.Length > 0)
				{
					SPort = new SerialPort();
					if (SPort.IsOpen)
					{
						SPort.Close();
					}
					SPort.PortName = names[0];
					//sPort.PortName = "COM6";
					SPort.BaudRate = 9600;
					SPort.DataBits = 8;
					SPort.Parity = Parity.None;
					SPort.StopBits = StopBits.One;
					SPort.ReadBufferSize = 1024;

					HasInit = true;
				}
				else
				{
					throw new Exception("找不到可用的序列裝置");
				}
			}
			catch (Exception ue)
			{
				Weight = "0";
				ErrorString = ue.Message;
				if (SPort != null && SPort.IsOpen)
				{
					SPort.Close();
				}
				throw ue;
			}
		}

		public string StartRead()
		{
			try
			{
				
				if (_skipError)
				{
					Weight = "0";
#if DEBUG
					var rnd = new Random();
					Weight = rnd.Next(100).ToString();					
#endif
					_correctWeightValue = Weight;
					return _correctWeightValue;

				}

				if (HasInit)
				{
					Weight = "0";

					IsTerminated = false;
					IsCommunicationThreadStop = false;

					SPort.DataReceived += new SerialDataReceivedEventHandler(sPort_DataReceived);
					SPort.Open();
					int errorCheckCount = 0;
					
					SpinWait.SpinUntil(() => false, 10);
					_correctWeightValue = "";
					while (true)
					{
						if (_correctWeightValue.Length > 0)
						{
							break;
						}
						else
						{
							errorCheckCount += 1;
						}
						if (errorCheckCount > 600000)
						{
							IsTerminated = true; //Original to use, but will get error 
							break;
						}
					}

					SpinWait.SpinUntil(() => false, 50);//讓次Thread能夠把結束工作處理完


					//檢查通訊是否已正確斷開

					while (IsCommunicationThreadStop == false)
					{						
						SpinWait.SpinUntil(() => false, 100);

						if (IsCommunicationThreadStop)
						{
							break;
						}
						if (_isEventTrigger == false)
						{
							break;
						}
					}
					if (SPort.IsOpen)
					{
						SPort.Close();
					}
					_isEventTrigger = false;
					if (_correctWeightValue.Length > 0)
					{
						return _correctWeightValue;
					}
					else
					{
						throw new Exception("重量讀取失敗；請再試一次");
					}
				}
				else
				{
					throw new Exception("Please execute init method first");
				}

			}
			catch (Exception ue)
			{
				ErrorString = ue.Message;
				throw;
			}
		}

		bool _isEventTrigger = false;
		void sPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			_isEventTrigger = true;
			var sPort = (SerialPort)sender;
			try
			{
				if (sPort.IsOpen)
				{
					sPort.ReadTimeout = 100;
					string s = sPort.ReadLine();
					if (_correctWeightValue.Length == 0)
					{
						if (s.IndexOf("ST") > -1 && s.IndexOf("kg") > -1 && s.IndexOf("?") == -1)
						{
							int idx = s.IndexOf("kg");
							s = s.Substring(0, idx + 2);
							_correctWeightValue = s;
							IsTerminated = true;
							return;
						}
					}
					SpinWait.SpinUntil(() => false, 100);
					Update(s);
					SpinWait.SpinUntil(() => false, 100);
					sPort.DiscardInBuffer();
					if (IsTerminated)
					{
						SpinWait.SpinUntil(() => false, 100);
						sPort.DataReceived -= sPort_DataReceived;
						SpinWait.SpinUntil(() => false, 100);
						sPort.DiscardInBuffer();
						SpinWait.SpinUntil(() => false, 100);
						sPort.Close();
						SpinWait.SpinUntil(() => false, 100);
						WriteLog("sending job Terminated");
						IsCommunicationThreadStop = true;
					}
				}
			}
			catch (Exception ue)
			{
				ErrorString = ue.Message;
			}
		}
		private void Update(string w)
		{
			Weight = w;
		}
	}
}