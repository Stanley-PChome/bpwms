using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.IO;

namespace Wms3pl.WpfClient.Common
{
	/// <summary>
	/// 音效播放
	/// </summary>
	public static class PlaySoundHelper
	{
		private static string SoundDirectoryPath
		{
			get { return System.Configuration.ConfigurationManager.AppSettings["SoundDirectoryPath"]; }
		}

		private static void PlaySound(string fileName)
		{
			string soundFilePath = Path.Combine(SoundDirectoryPath, fileName);
			if (File.Exists(soundFilePath))
			{
				var player = new SoundPlayer { SoundLocation = soundFilePath };
				player.Play();
			}
		}

		/// <summary>
		/// 登了燈
		/// </summary>
		public static void Scan()
		{
			PlaySound(@"SCAN.WAV");
		}

		/// <summary>
		/// 唉呦
		/// </summary>
		public static void Au()
		{
			PlaySound(@"AU.WAV");
		}

		/// <summary>
		/// 歐歐
		/// </summary>
		public static void Oo()
		{
			PlaySound(@"OO.WAV");
		}

		/// <summary>
		/// 下一個
		/// </summary>
		public static void Next()
		{
			PlaySound(@"NEXT.WAV");
		}

		/// <summary>
		/// 播放文字內容
		/// </summary>
		/// <param name="word"></param>
		public static void Word(string word)
		{
			foreach (var chr in word)
			{
				PlaySound(chr + @".WAV");
				Thread.Sleep(360);
			}
		}

		/// <summary>
		/// 個
		/// </summary>
		public static void Unit()
		{
			PlaySound(@"UNIT.WAV");
		}

		/// <summary>
		/// 次
		/// </summary>
		public static void Once()
		{
			PlaySound(@"ONCE.WAV");
		}

		/// <summary>
		/// DU~DU~DU~
		/// </summary>
		public static void See()
		{
			PlaySound(@"SEE.WAV");
		}

		/// <summary>
		/// ERROR
		/// </summary>
		public static void Error()
		{
			PlaySound(@"ERROR.WAV");
		}

		public static void Stop(int millisecond)
		{
			Thread.Sleep(millisecond);
		}
	}
}
