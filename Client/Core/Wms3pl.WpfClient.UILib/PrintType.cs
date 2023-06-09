namespace Wms3pl.WpfClient.UILib
{
  public enum PrintType
  {
    Preview,
    ToPrinter
  }

	public enum PrinterType
	{
		/// <summary>
		/// 一般印表機(裝置設定第一台)
		/// </summary>
		A4,
		/// <summary>
		/// 標籤機(裝置設定第三台)
		/// </summary>
		Label,
		/// <summary>
		/// 點陣式印表機(裝置設定第二台)
		/// </summary>
		Matrix
	}
}