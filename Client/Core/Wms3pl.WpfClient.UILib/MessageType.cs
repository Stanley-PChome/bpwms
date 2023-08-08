using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.UILib
{
	public struct MessagesStruct
	{
		public string Message { get; set; }
		public string Title { get; set; }
		public DialogImage Image { get; set; }
		public DialogButton Button { get; set; }
	}

	public static class Messages
	{
		public static MessagesStruct Success = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.Success, Title = Properties.Resources.Message};
		public static MessagesStruct Success2 = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.Success2, Title = Properties.Resources.Message};
		public static MessagesStruct PrintSuccess = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.PrintSuccess, Title = Properties.Resources.Message};
		public static MessagesStruct InfoNoData = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.InfoNoData, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeAdd = new MessagesStruct() { Button = DialogButton.YesNoCancel, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforeAdd, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeUpdate = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforeUpdate, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeDelete = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforeDelete, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeCancel = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforeCancel, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforePrint = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforePrint, Title = Properties.Resources.Message};
		public static MessagesStruct Failed = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.Failed, Title = Properties.Resources.Message};
		public static MessagesStruct PrintFailed = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.PrintFailed, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNotModified = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.WarningNotModified, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoValue = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoValue, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoEmpName = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoEmpName, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoEmpID = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoEmpID, Title = Properties.Resources.Message};
		public static MessagesStruct WarningCannotDelete = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningCannotDelete, Title = Properties.Resources.Message};
		public static MessagesStruct WarningCannotDeleteGroup = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningCannotDeleteGroup, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeenDeleted = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeenDeleted, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeDisableFunction = new MessagesStruct() { Button = DialogButton.OKCancel, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforeDisableFunction, Title = Properties.Resources.Warning};
		public static MessagesStruct WarningBeforeEnableFunction = new MessagesStruct() { Button = DialogButton.OKCancel, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforeEnableFunction, Title = Properties.Resources.Warning};
		public static MessagesStruct WarningCannotDeleteFunction = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningCannotDeleteFunction, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoDcCode = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoDcCode, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoDcName = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoDcName, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoCustCode = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoCustCode, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoCustName = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoCustName, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoGupCode = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoGupCode, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoGupName = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoGupName, Title = Properties.Resources.Message};
		public static MessagesStruct WarningCannotDeleteGup = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningCannotDeleteGup, Title = Properties.Resources.Message};
		public static MessagesStruct WarningExist = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningExist, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoCheckNo = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoCheckNo, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoCheckName = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoCheckName, Title = Properties.Resources.Message};
		public static MessagesStruct WarningCannotDeleteCheckItem = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningCannotDeleteCheckItem, Title = Properties.Resources.Message};

		public static MessagesStruct WarningNoPackLength = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoPackLength, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoPackWidth = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoPackWidth, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoPackHight = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoPackHight, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoPackWeight = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoPackWeight, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoCaseLength = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoCaseLength, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoCaseWidth = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoCaseWidth, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoCaseHight = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoCaseHight, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoCaseWeight = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoCaseWeight, Title = Properties.Resources.Message};

		public static MessagesStruct PickTimeNoData = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.PickTimeNoData, Title = Properties.Resources.Message};
		public static MessagesStruct PickListNoData = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.PickListNoData, Title = Properties.Resources.Message};

		public static MessagesStruct InputPickNoNoData = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.InputPickNoNoData, Title = Properties.Resources.Message};
		public static MessagesStruct InputSerialNoDuplicate = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.InputSerialNoDuplicate, Title = Properties.Resources.Message};
		public static MessagesStruct WarningIsChickIn = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningIsChickIn+ Environment.NewLine + Properties.Resources.WarningIsChickIn2, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoMergeBoxNo = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoMergeBoxNo, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoItemCode = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoItemCode, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoItem = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoItem, Title = Properties.Resources.Message};
		public static MessagesStruct WarningCannotNext = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningCannotNext, Title = Properties.Resources.Message};
		public static MessagesStruct WarningCannotRevious = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningCannotRevious, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBoxBindingComplete = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningBoxBindingComplete, Title = Properties.Resources.Message};
		public static MessagesStruct WarningAlreadySowing = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningAlreadySowing, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeAddLack = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforeAddLack, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoBoxNo = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoBoxNo, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoLackQty = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoLackQty, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoWmsOrdNo = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoWmsOrdNo, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoWmsOrdItem = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoWmsOrdItem, Title = Properties.Resources.Message};

		public static MessagesStruct WarningNoReceiptDate = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoReceiptDate, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoTransport = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoTransport, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoCarNo = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoCarNo, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNoReturnNoOrPastNo = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoReturnNoOrPastNo, Title = Properties.Resources.Message};

		public static MessagesStruct WarningBeforePosting = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforePosting, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeForceClose = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforeForceClose, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeApproveF050101 = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforeApproveF050101, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeChangeContractObject = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforeChangeContractObject, Title = Properties.Resources.Message};
		/// <summary>
		/// 資料重複時的提示訊息. 自行在Message前加上重複的欄位名稱
		/// </summary>
		public static MessagesStruct WarningDuplicatedData = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningDuplicatedData, Title = Properties.Resources.Message};
		public static MessagesStruct WarningCannotDeleteWordgroup = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningCannotDeleteWordgroup, Title = Properties.Resources.Message};

		public static MessagesStruct InfoImportSuccess = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoImportSuccess, Title = Properties.Resources.Message};
		public static MessagesStruct ErrorImportFailed = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Error, Message = Properties.Resources.ErrorImportFailed, Title = Properties.Resources.Message};

		public static MessagesStruct InfoExportSuccess = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoExportSuccess, Title = Properties.Resources.Message};
		public static MessagesStruct ErrorExportFailed = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Error, Message = Properties.Resources.ErrorExportFailed, Title = Properties.Resources.Message};

		public static MessagesStruct WarningProgramSettingInvalid = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Error, Message = Properties.Resources.WarningProgramSettingInvalid, Title = Properties.Resources.Warning};

		public static MessagesStruct InfoAddSuccess = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoAddSuccess, Title = Properties.Resources.Message};
		public static MessagesStruct ErrorAddFailed = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Error, Message = Properties.Resources.ErrorAddFailed, Title = Properties.Resources.Message};
		public static MessagesStruct InfoUpdateSuccess = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoUpdateSuccess, Title = Properties.Resources.Message};
		public static MessagesStruct ErrorUpdateFailed = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Error, Message = Properties.Resources.ErrorUpdateFailed, Title = Properties.Resources.Message};

		public static MessagesStruct WarningInvalidLocCode = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Error, Message = Properties.Resources.WarningInvalidLocCode, Title = Properties.Resources.Message};
		public static MessagesStruct WarningInvalidOrderNo = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Error, Message = Properties.Resources.WarningInvalidOrderNo, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforePrintWarehouseInReport = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Information, Message = Properties.Resources.WarningBeforePrintWarehouseInReport, Title = Properties.Resources.Message};
		public static MessagesStruct InfoOrderNotInWarehouse = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoOrderNotInWarehouse, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforePrintInWarehouseTag = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Information, Message = Properties.Resources.WarningBeforePrintInWarehouseTag, Title = Properties.Resources.Message};
		public static MessagesStruct WarningInvalidRecvQty = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.WarningInvalidRecvQty, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeSaveP020203 = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Information, Message = Properties.Resources.WarningBeforeSaveP020203, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeGoP02020301 = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Information, Message = Properties.Resources.WarningBeforeGoP02020301, Title = Properties.Resources.Message};
		public static MessagesStruct InfoDeleteSuccess = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoDeleteSuccess, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeExit = new MessagesStruct() { Button = DialogButton.OKCancel, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforeExit, Title = Properties.Resources.Warning};
		public static MessagesStruct InfoSerialNotAllImported = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoSerialNotAllImported, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNotAllowedUCCTypeForSpecialPurchase = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.WarningNotAllowedUCCTypeForSpecialPurchase, Title = Properties.Resources.Message};
		public static MessagesStruct WarningFileSizeExceedLimits = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.WarningFileSizeExceedLimits+ (Wms3pl.WpfClient.Common.GlobalVariables.FileSizeLimit / 1024).ToString() + "KB", Title = Properties.Resources.Warning};
		public static MessagesStruct WarningBeforeUploadRtFiles = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Information, Message = Properties.Resources.WarningBeforeUploadRtFiles, Title = Properties.Resources.Message};
		public static MessagesStruct InfoFileUploaded = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoFileUploaded, Title = Properties.Resources.Message};
		public static MessagesStruct WarningFileUploadedFailure = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Error, Message = Properties.Resources.WarningFileUploadedFailure, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNotUccSelected = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.WarningNotUccSelected, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeComfirmEditItem = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Information, Message = Properties.Resources.WarningBeforeComfirmEditItem, Title = Properties.Resources.Message};
		public static MessagesStruct WarningNotValidFinishDate = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.WarningNotValidFinishDate, Title = Properties.Resources.Message};
		public static MessagesStruct WarningCannotUpdateProcessQty = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.WarningCannotUpdateProcessQty, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeUpdateProcessQty = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Information, Message = Properties.Resources.WarningBeforeUpdateProcessQty, Title = Properties.Resources.Message};
		public static MessagesStruct WarningCannotUpdateF910201 = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Information, Message = Properties.Resources.WarningCannotUpdateF910201, Title = Properties.Resources.Message};
		public static MessagesStruct InfoPickNoExists = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Information, Message = Properties.Resources.InfoPickNoExists, Title = Properties.Resources.Message};
		public static MessagesStruct DeleteSuccess = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.DeleteSuccess, Title = Properties.Resources.Message};
		public static MessagesStruct WarningInvalidSerialFile = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.WarningInvalidSerialFile, Title = Properties.Resources.Message};
		public static MessagesStruct WarningSerialNoHasExist = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningSerialNoHasExist, Title = Properties.Resources.Message};
		public static MessagesStruct WarningSmallTicketHasExist = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningSmallTicketHasExist, Title = Properties.Resources.Message};
		public static MessagesStruct WarningIsManualAssignCar = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningIsManualAssignCar, Title = Properties.Resources.Message};    		
		public static MessagesStruct WarningProcessFinished = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.WarningProcessFinished, Title = Properties.Resources.Message};
		public static MessagesStruct WarningProcessIp = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.WarningProcessIp, Title = Properties.Resources.Message};		

		#region 主管解鎖
		public static MessagesStruct InfoInvalidUserId = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoInvalidUserId, Title = Properties.Resources.Message};
		public static MessagesStruct InfoInvalidUnlockPermission = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoInvalidUnlockPermission, Title = Properties.Resources.Message};
		public static MessagesStruct InfoInvalidPassword = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoInvalidPassword, Title = Properties.Resources.Message};
		public static MessagesStruct InfoInvalidUnlockCode = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoInvalidUnlockCode, Title = Properties.Resources.Message};
		#endregion

		public static MessagesStruct InfoApprove = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoApprove, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeDebit = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningBeforeDebit, Title = Properties.Resources.Message};
		public static MessagesStruct InfoDebitSuccess = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoDebitSuccess, Title = Properties.Resources.Message};
		public static MessagesStruct InfoSerialCountOver = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Information, Message = Properties.Resources.InfoSerialCountOver, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeSave = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Information, Message = Properties.Resources.WarningBeforeSave, Title = Properties.Resources.Message};
		public static MessagesStruct WarningBeforeImportF110101 = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Information, Message = Properties.Resources.WarningBeforeImportF110101, Title = Properties.Resources.Message};
		public static MessagesStruct WarningFileLocked = new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.WarningFileLocked, Title = Properties.Resources.Warning};

		public static MessagesStruct WarningNoSelect = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = Properties.Resources.WarningNoSelect, Title = Properties.Resources.Message};

		//public static string ScanSerialOrBoxMessage = "此品項必須刷讀序號/盒號/箱號,請刷讀序號/盒號/箱號";
		public static string ScanSerialOrBoxMessage = Properties.Resources.ScanSerialOrBoxMessage;
		//public static string ScanSerialorBox = "請刷讀品號/序號/盒號/箱號";
		public static string ScanSerialorBox = Properties.Resources.ScanSerialorBox;
	}

}
