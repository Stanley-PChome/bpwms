using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;
namespace Wms3pl.WpfClient.DataServices.F70DataService
{

  [MetadataType(typeof(F700102.MetaData))]
  public partial class F700102 : IDataErrorInfo
  {
    public class MetaData
    {
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string DISTR_CAR_NO { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public Int16 DISTR_CAR_SEQ { get; set; }
      public string RETAIL_CODE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CUST_NAME { get; set; }
      public string ENTRUST_DEPT { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string ADDRESS { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CONTACT { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CONTACT_TEL { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string TAKE_TIME { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public Int32 ITEM_QTY { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string DISTR_USE { get; set; }
      public string ORD_TYPE { get; set; }
      public string WMS_NO { get; set; }
      public string MEMO { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string DC_CODE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string GUP_CODE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CUST_CODE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CRT_STAFF { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public DateTime CRT_DATE { get; set; }
      public string UPD_STAFF { get; set; }
      public DateTime? UPD_DATE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CRT_NAME { get; set; }
      public string UPD_NAME { get; set; }

    }

    [DoNotSerialize]
    public string Error
    {
      get
      {
        return string.Join<string>(",", InputValidator<F700102>.Validate(this));
      }
    }

    public string this[string columnName]
    {
      get
      {
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
        return InputValidator<F700102>.Validate(this, columnName);
      }
    }
  }

  [MetadataType(typeof(F700101.MetaData))]
  public partial class F700101 : IDataErrorInfo
  {
    public class MetaData
    {
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string DISTR_CAR_NO { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public DateTime TAKE_DATE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string ALL_ID { get; set; }
      public Decimal? CAR_KIND_ID { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string SP_CAR { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CHARGE_CUST { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CHARGE_DC { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public Decimal FEE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string STATUS { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string EDI_FLAG { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string DC_CODE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CRT_STAFF { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public DateTime CRT_DATE { get; set; }
      public string UPD_STAFF { get; set; }
      public DateTime? UPD_DATE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CRT_NAME { get; set; }
      public string UPD_NAME { get; set; }
      public string PIER_CODE { get; set; }
      public string CHARGE_GUP_CODE { get; set; }
      public string CHARGE_CUST_CODE { get; set; }

    }

    [DoNotSerialize]
    public string Error
    {
      get
      {
        return string.Join<string>(",", InputValidator<F700101>.Validate(this));
      }
    }

    public string this[string columnName]
    {
      get
      {
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
        return InputValidator<F700101>.Validate(this, columnName);
      }
    }
  }

  [MetadataType(typeof(F700201.MetaData))]
  public partial class F700201 : IDataErrorInfo
  {
    public class MetaData
    {
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string COMPLAINT_NO { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public DateTime COMPLAINT_DATE { get; set; }
      [MaxLength(20, ErrorMessage = "長度限制{1}碼")]
      public string RETAIL_CODE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      [MaxLength(80, ErrorMessage = "長度限制{1}碼")]
      public string CUST_NAME { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      [MaxLength(20, ErrorMessage = "長度限制{1}碼")]
      public string COMPLAINT_NAME { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string COMPLAINT_TYPE { get; set; }
      [MaxLength(100, ErrorMessage = "長度限制{1}碼")]
      public string COMPLAINT_DESC { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      [Range(0, 999999999)]
      public Int32 QTY { get; set; }
      public string DEP_ID { get; set; }
      [MaxLength(16, ErrorMessage = "長度限制{1}碼")]
      public string HANDLE_STAFF { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      [MaxLength(300, ErrorMessage = "長度限制{1}碼")]
      public string RESPOND_DESC { get; set; }
      [MaxLength(300, ErrorMessage = "長度限制{1}碼")]
      public string HANDLE_DESC { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string STATUS { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string DC_CODE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string GUP_CODE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CUST_CODE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CRT_STAFF { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public DateTime CRT_DATE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string CRT_NAME { get; set; }
      public string UPD_STAFF { get; set; }
      public DateTime? UPD_DATE { get; set; }
      public string UPD_NAME { get; set; }

    }

    [DoNotSerialize]
    public string Error
    {
      get
      {
        return string.Join<string>(",", InputValidator<F700201>.Validate(this));
      }
    }

    public string this[string columnName]
    {
      get
      {
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
        return InputValidator<F700201>.Validate(this, columnName);
      }
    }
  }

}


