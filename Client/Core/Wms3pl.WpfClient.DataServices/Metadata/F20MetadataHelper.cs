using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.DataServices.F20DataService
{
	
	[MetadataType(typeof(F200101.MetaData))]
	public partial class F200101 : IDataErrorInfo
  {
    public class MetaData
    {
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string ADJUST_NO {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string ADJUST_TYPE {get; set;}
	public string WORK_TYPE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public DateTime ADJUST_DATE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string STATUS {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string DC_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string GUP_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CUST_CODE {get; set;}
	public string UPD_STAFF {get; set;}
	public DateTime? UPD_DATE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CRT_STAFF {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public DateTime CRT_DATE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CRT_NAME {get; set;}
	public string UPD_NAME {get; set;}
	
}

    [DoNotSerialize]
    public string Error
    {
      get {
        return string.Join<string>(",", InputValidator<F200101>.Validate(this));
      }
    }

    public string this[string columnName]
    {
      get {
        //switch (columnName)
        //{
        //  case "VNR_NAME":
        //    if (string.IsNullOrWhiteSpace(VNR_NAME))
        //    {
        //      return "不可為空白";
        //    }
        //    break;

        //  default:
        //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
        //}

        //return "";
        return InputValidator<F200101>.Validate(this, columnName);
      }
    }
  }
	
	[MetadataType(typeof(F200102.MetaData))]
	public partial class F200102 : IDataErrorInfo
  {
    public class MetaData
    {
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string ADJUST_NO {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Int16 ADJUST_SEQ {get; set;}
	public string WORK_TYPE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string ORD_NO {get; set;}
	public string ADDRESS {get; set;}
	public string ALL_ID {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CAUSE {get; set;}
	public string CAUSE_MEMO {get; set;}
	public string NEW_DC_CODE {get; set;}
	public string ORG_STATUS {get; set;}
	public string ORG_PICK_TIME {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string STATUS {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string GUP_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CUST_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string DC_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CRT_STAFF {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public DateTime CRT_DATE {get; set;}
	public string UPD_STAFF {get; set;}
	public DateTime? UPD_DATE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CRT_NAME {get; set;}
	public string UPD_NAME {get; set;}
	public string NEW_ORD_NO {get; set;}
	public string ORG_ADDRESS {get; set;}
	
}

    [DoNotSerialize]
    public string Error
    {
      get {
        return string.Join<string>(",", InputValidator<F200102>.Validate(this));
      }
    }

    public string this[string columnName]
    {
      get {
        //switch (columnName)
        //{
        //  case "VNR_NAME":
        //    if (string.IsNullOrWhiteSpace(VNR_NAME))
        //    {
        //      return "不可為空白";
        //    }
        //    break;

        //  default:
        //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
        //}

        //return "";
        return InputValidator<F200102>.Validate(this, columnName);
      }
    }
  }
	
	[MetadataType(typeof(F20010201.MetaData))]
	public partial class F20010201 : IDataErrorInfo
  {
    public class MetaData
    {
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string ADJUST_NO {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Int32 LOG_SEQ {get; set;}
	public string SERIAL_NO {get; set;}
	public string SERIAL_STATUS {get; set;}
	public string CELL_NUM {get; set;}
	public string PUK {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string ISPASS {get; set;}
	public string MESSAGE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string DC_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string GUP_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CUST_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CRT_STAFF {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CRT_NAME {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public DateTime CRT_DATE {get; set;}
	public string UPD_STAFF {get; set;}
	public string UPD_NAME {get; set;}
	public DateTime? UPD_DATE {get; set;}
	
}

    [DoNotSerialize]
    public string Error
    {
      get {
        return string.Join<string>(",", InputValidator<F20010201>.Validate(this));
      }
    }

    public string this[string columnName]
    {
      get {
        //switch (columnName)
        //{
        //  case "VNR_NAME":
        //    if (string.IsNullOrWhiteSpace(VNR_NAME))
        //    {
        //      return "不可為空白";
        //    }
        //    break;

        //  default:
        //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
        //}

        //return "";
        return InputValidator<F20010201>.Validate(this, columnName);
      }
    }
  }
	
	[MetadataType(typeof(F200103.MetaData))]
	public partial class F200103 : IDataErrorInfo
  {
    public class MetaData
    {
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string ADJUST_NO {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Int16 ADJUST_SEQ {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string WORK_TYPE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string WAREHOUSE_ID {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string LOC_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string ITEM_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string VNR_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public DateTime VALID_DATE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public DateTime ENTER_DATE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Int32 ADJ_QTY {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CAUSE {get; set;}
	public string CAUSE_MEMO {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string STATUS {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string GUP_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CUST_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string DC_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CRT_STAFF {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public DateTime CRT_DATE {get; set;}
	public string UPD_STAFF {get; set;}
	public DateTime? UPD_DATE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CRT_NAME {get; set;}
	public string UPD_NAME {get; set;}
	
}

    [DoNotSerialize]
    public string Error
    {
      get {
        return string.Join<string>(",", InputValidator<F200103>.Validate(this));
      }
    }

    public string this[string columnName]
    {
      get {
        //switch (columnName)
        //{
        //  case "VNR_NAME":
        //    if (string.IsNullOrWhiteSpace(VNR_NAME))
        //    {
        //      return "不可為空白";
        //    }
        //    break;

        //  default:
        //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
        //}

        //return "";
        return InputValidator<F200103>.Validate(this, columnName);
      }
    }
  }
	
	[MetadataType(typeof(F20010301.MetaData))]
	public partial class F20010301 : IDataErrorInfo
  {
    public class MetaData
    {
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string ADJUST_NO {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Int16 ADJUST_SEQ {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Int32 LOG_SEQ {get; set;}
	public string SERIAL_NO {get; set;}
	public string SERIAL_STATUS {get; set;}
	[MaxLength(50, ErrorMessage = "長度限制{1}碼")]
	public string CELL_NUM {get; set;}
	[MaxLength(50, ErrorMessage = "長度限制{1}碼")]
	public string PUK {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string ISPASS {get; set;}
	public string MESSAGE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string DC_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string GUP_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CUST_CODE {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CRT_STAFF {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public string CRT_NAME {get; set;}
	  [Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public DateTime CRT_DATE {get; set;}
	public string UPD_STAFF {get; set;}
	public string UPD_NAME {get; set;}
	public DateTime? UPD_DATE {get; set;}
	
}

    [DoNotSerialize]
    public string Error
    {
      get {
        return string.Join<string>(",", InputValidator<F20010301>.Validate(this));
      }
    }

    public string this[string columnName]
    {
      get {
        //switch (columnName)
        //{
        //  case "VNR_NAME":
        //    if (string.IsNullOrWhiteSpace(VNR_NAME))
        //    {
        //      return "不可為空白";
        //    }
        //    break;

        //  default:
        //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
        //}

        //return "";
        return InputValidator<F20010301>.Validate(this, columnName);
      }
    }
  }
}
