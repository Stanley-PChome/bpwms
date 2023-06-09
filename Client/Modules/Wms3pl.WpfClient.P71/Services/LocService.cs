using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P71.Services
{
	public partial class LocService
	{
		/// <summary>
		/// 將數字轉對應文字
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public string ChangeNumberToWord(int number)
		{
			string str = string.Empty;
			for (int i = 0; i < number.ToString().Length; i++)
			{
				int a = 1;
				for (int x = 1; x < (number.ToString().Length - i); x++)
					a *= 36;

				a = a == 1 ? 36 : a;
				int q = 0;
				if (number.ToString().Length - i >= 2)
				{
					q = number / a;
					if (number.ToString().Length - i > 2 && q == 0)
						continue;
				}
				else
					q = number % a;

				if (q > 9)
					q += 55;
				else
					q += 48;
				str += Convert.ToChar(q).ToString();
			}

			return str;
		}

		/// <summary>
		/// 將文字儲位轉數字
		/// </summary>
		/// <param name="english"></param>
		/// <returns></returns>
		public int ChangeWordToNumber(string english)
		{
			var upString = english.ToUpper();
			int result = 0;
			for (int i = 0; i < english.Length; i++)
			{
				int letter = (int)upString[i];
				int a = 1;

				for (int x = 1; x < (english.Length - i); x++)
					a *= 36;
				if (i + 1 != english.Length)
				{
					if (letter >= 65 && letter <= 90)
						result += (letter - 55) * a;
					else if (letter >= 48 && letter <= 57)
						result += (letter - 48) * a;
				}
				else
				{
					if (letter >= 65 && letter <= 90)
						result += (letter - 55);
					else if (letter >= 48 && letter <= 57)
						result += (letter - 48);
				}
			}

			return result;
		}
	}
}
