namespace Wms3pl.Datas.Shared.Entities
{
    /// <summary>
    /// 取得商品單位階層資料回傳物件
    /// </summary>
    public class GetItemLevelRes
    {
        public string GUP_CODE { get; set; }
        public string CUST_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public short UNIT_LEVEL { get; set; }
        public string UNIT_ID { get; set; }
        public string UNIT_NAME { get; set; }
        public int UNIT_QTY { get; set; }
        public decimal? LENGTH { get; set; }
        public decimal? WIDTH { get; set; }
        public decimal? HIGHT { get; set; }
        public decimal? WEIGHT { get; set; }
        public string SYS_UNIT { get; set; }
    }

    /// <summary>
    /// 計算商品包裝參考
    /// </summary>
    public class CountItemPackageRefRes
    {
        public string ItemCode { get; set; }
        public string PackageRef { get; set; }
    }

    /// <summary>
    /// 品號、數量物件
    /// </summary>
    public class ItemCodeQtyModel
    {
        public string ItemCode { get; set; }
        public int Qty { get; set; }
    }

    /// <summary>
    /// 計算商品整數箱與零散數
    /// </summary>
    public class CountItemFullAndBulkCaseQtyRes
    {
        public string ItemCode { get; set; }
        public int FullCaseQty { get; set; }
        public int BulkCaseQty { get; set; }
    }

    /// <summary>
    /// 允收天數、警示天數
    /// </summary>
    public class GetItemAllDlnAndAllShpRes
    {
        public int ALL_DLN { get; set; }
        public int ALL_SHP { get; set; }
    }
	public class ItemMakeNo
	{
		public string ItemCode { get; set; }
		public string MakeNo { get; set; }
	}
	
	public class ItemMakeNoAndSerialNo
	{
		public string ItemCode { get; set; }
		public string MakeNo { get; set; }
		public string SerialNo { get; set; }
	}
}
