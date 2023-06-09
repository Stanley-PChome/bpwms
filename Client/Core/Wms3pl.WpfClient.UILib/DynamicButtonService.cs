
using System.Windows.Input;

namespace Wms3pl.WpfClient.UILib
{
	/// <summary>
	/// 動態按鈕資料
	/// </summary>
	public class DynamicButtonData
	{
		public string Content { get; set; }

		public ICommand Command { get; set; }
		public string FunctionId { get; set; }

		public DynamicButtonData(string content, ICommand command, string functionId)
		{
			Content = content;
			Command = command;
			FunctionId = functionId;
		}
	}
}
