namespace Wms3pl.WebServices.DataCommon
{
	public static class Schemas
	{
		private static string _coreSchema = string.Empty;
		public static string CoreSchema
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_coreSchema))
				{
					return _coreSchema;
				}
				else
				{
					return DbSchemaHelper.GetSchema();
				}
			}
			set
			{
				_coreSchema = value;
			}
		}
		public const string LongTermSchema = "WMSLongTerm";

	}
}
