namespace Wms3pl.ScheduleModule.Consoles.WmsSchedule
{
	public class AppConfig
	{
		/// <summary>
		/// 產生檔案預設暫存路徑
		/// </summary>
		public string FilePath { get; set; }
    /// <summary>
    /// 連線資料庫名稱
    /// </summary>
    public string SchemaName { get; set; }
		/// <summary>
		/// 排程編號
		/// </summary>
		public string ScheduleNo { get; set; }
	}
}
