
using System.Collections.Generic;

namespace Wms3pl.WebServices.DataCommon
{
  public class CommandResult
  {
    public bool IsSuccessed { get; set; }
    public string Message { get; set; }
  }

	public class CommandFunctionResult
	{
		public CommandFunctionResult()
		{
			IsSuccessed = true;
			OutValuePair = new List<KeyValuePair<string, string>>();
		}

		public bool IsSuccessed { get; set; }
		public string Message { get; set; }
		public string ReturnValue { get; set; }
		public List<KeyValuePair<string, string>> OutValuePair { get; set; }
	}
}
