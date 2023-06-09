using System;
using System.Drawing;


namespace Wms3pl.WpfClient.Common
{
  public class ScreenUtility
  {
    /// <summary>
    /// 將指定區域的 screen 儲存成jpg 檔案
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="jpgPath"></param>
    public void SaveImage(int width, int height, string jpgPath)
    {
			//增加try catch,避免列印報表發生錯誤時,傳入參數值錯誤發生Error
	    try
	    {
		    var myImage = new Bitmap(width, height);
		    var g = Graphics.FromImage(myImage);
		    g.CopyFromScreen(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0),
			    new System.Drawing.Size(width, height));
		    myImage.Save(jpgPath);
	    }
	    catch (Exception)
	    {

	    }
    }
  }
}
