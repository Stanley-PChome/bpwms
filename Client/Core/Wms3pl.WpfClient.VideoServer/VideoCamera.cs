using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeamMark;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;

namespace Wms3pl.WpfClient.VideoServer
{
	public class VideoCamera
	{
		private RemotePort _remoteVideoServer;
		public VideoCamera(RemotePort remoteVideoServer, bool isIgnoreVideoError, string cameraNo, string account, string accountName)
		{
			_remoteVideoServer = remoteVideoServer;

			IsIgnoreVideoError = isIgnoreVideoError;
			CameraNo = cameraNo;
			Account = account;
			AccountName = accountName;
		}

		#region Public Property
		public string CameraNo { get; private set; }
		public string Account { get; private set; }
		public string AccountName { get; private set; }

		/// <summary>
		/// 是否忽略攝影機主機錯誤(不啟用攝影機)
		/// </summary>
		public bool IsIgnoreVideoError { get; private set; }
		/// <summary>
		/// 攝影機已經準備好了? 連接或關閉會改變這個狀態
		/// </summary>
		public bool IsVideoReady { get; private set; }
		/// <summary>
		/// 攝影機Session準備好了?
		/// </summary>
		public bool IsVideoSessionReady { get; private set; }
		#endregion

		#region Method

		public void SetIsVideoReady(bool isVideoReady)
		{
			IsVideoReady = isVideoReady;
		}

		public void SetIsVideoSessionReady(bool isVideoSessionReady)
		{
			IsVideoSessionReady = isVideoSessionReady;
		}

		public ExecuteResult VideoStartSessionByCustOrderNo(string custOrderNo, string sdNo)
		{
			IsVideoSessionReady = false;

			if (string.IsNullOrWhiteSpace(custOrderNo) || string.IsNullOrWhiteSpace(sdNo))
				return Failure($"{CameraNo}機台: 未設定單號");

			//主項Table
			var command = string.Format("S,{0},{1},{2},{3}", CameraNo, custOrderNo, sdNo, Account);

			var result = SendCommand(command);
			if (!result.IsSuccessed)
				return result;

			IsVideoSessionReady = true;
			return Success();
		}

		public ExecuteResult VideoStartSessionByCustOrderNo(string date, string custOrderNo, string custOrderName)
		{
			IsVideoSessionReady = false;

			if (string.IsNullOrWhiteSpace(custOrderNo))
				return Failure($"{CameraNo}機台: 未設定單號");

			//主項Table
			var command = string.Format("S,{0},{1},{2},{3},{4}", CameraNo, date, custOrderNo, custOrderName, Account);

			var result = SendCommand(command);
			if (!result.IsSuccessed)
				return result;

			IsVideoSessionReady = true;
			return Success();
		}

		// "出貨單號,店名,店號,路線,司機,出貨日,掃描人員"
		public ExecuteResult VideoStartSessionByCustOrderNo(string custOrderNo, string retailName, string retailCode, string delvDate)
		{
			IsVideoSessionReady = false;

			// oVC.VideoStartSession lblOrdNo.Caption & "," & lblRetailName.Caption & "," & lblRetailCode.Caption & "," & "-" & "," & "-" & "," & Format(CDate(labDELV_DATE.Caption), "YYYYMMDD") & "," & objRef.CurUserID & " " & objRef.CurUserName
			//主項Table
			var command = string.Format("S,{0},{1},{2},{3},-,-,{4},{5} {6}", CameraNo, custOrderNo, retailName, retailCode, delvDate, Account, AccountName);
			if (string.IsNullOrWhiteSpace(custOrderNo))
				return Failure($"{CameraNo}機台: 未設定單號");

			var result = SendCommand(command);
			if (!result.IsSuccessed)
				return result;

			IsVideoSessionReady = true;
			return new ExecuteResult { IsSuccessed = true };
		}

		// "退貨單號,店名,店號,路線,司機,退貨日,退貨掃描人員""
		public ExecuteResult VideoStartSessionByRsiOrdNo(string rsiOrdNo, string retailName, string retailCode, string delvNo, string dirverName, string delvDate)
		{
			IsVideoSessionReady = false;

			if (string.IsNullOrWhiteSpace(rsiOrdNo))
				return Failure($"{CameraNo}機台: 未設定單號");

			//主項Table
			var command = string.Format("S,{0},{1},{2},{3},{4},{5},{6},{7} {8}", CameraNo, rsiOrdNo, retailName, retailCode, delvNo, dirverName, delvDate, Account, AccountName);

			var result = SendCommand(command);
			if (!result.IsSuccessed)
				return result;

			IsVideoSessionReady = true;
			return new ExecuteResult { IsSuccessed = true };
		}

		private string FixItemName(string itemName)
		{
			itemName = itemName ?? "";
			var fixItemName = itemName.Length >= 20 ? itemName.Substring(0, 20) : itemName;

			fixItemName = fixItemName.Replace("：", " ")
									.Replace("，", " ")
									.Replace("[", " ")
									.Replace("]", " ")
									.Replace("#", " ")
									.Replace(":", " ");
			return fixItemName;
		}


		public ExecuteResult VideoShowItemByScan(string custOrderNo, string sdNo, string itemName,
											string itemBarCode, decimal orderQty, decimal scanQty, string rsiOrdNo, string rsiOrdSeq)
		{
			var fixItemName = FixItemName(itemName);

			var command = string.Format("M,{0},,,{1},{2},{3},{4},{5},{6},0",
									CameraNo, fixItemName, itemBarCode, orderQty, scanQty, rsiOrdNo, rsiOrdSeq);

			return SendCommand(command);
		}

		public ExecuteResult VideoShowItemByScan(string itemName,
											string itemBarCode, string locCode, string info, string rsiOrdNo, string rsiOrdSeq)
		{
			var fixItemName = FixItemName(itemName);

			var command = string.Format("M,{0},,,{1},{2},{3},{4},{5},{6},0",
									CameraNo, itemBarCode, fixItemName, locCode, info, rsiOrdNo, rsiOrdSeq);

			return SendCommand(command);
		}


		public ExecuteResult VideoShowItemByPastNo(string itemCode, string itemName, decimal orderQty, string pastNo)
		{
			var fixItemName = FixItemName(itemName);

			var command = string.Format("M,{0},,,{1},{2},{3},{4},0",
								CameraNo, itemCode, fixItemName, orderQty, pastNo);

			return SendCommand(command);
		}

		public ExecuteResult VideoShowItemByBoxNo(string itemCode, string itemName, decimal orderQty, string boxNo, string remark)
		{
			// "品號,品名,累計數量,備註"
			var command = string.Format("M,{0},{1},{2},{3},{4}", CameraNo, itemCode, itemName, orderQty, boxNo);

			return SendCommand(command);
		}

		public ExecuteResult VideoShowWeight(string custOrderNo, string sdNo, int scanBoxQty, decimal weight)
		{
			if (string.IsNullOrWhiteSpace(custOrderNo) || string.IsNullOrWhiteSpace(sdNo))
			{
				IsVideoSessionReady = false;
				return Failure($"{CameraNo}機台: 未設定單號");
			}

			var command = string.Format("M,{0},,,件數/重量,,{1},{2},,,",
									CameraNo, scanBoxQty, weight);

			return SendCommand(command);
		}

		public ExecuteResult VideoChangeCaption(string custOrderNo, string sdNo, string msg)
		{
			if (string.IsNullOrWhiteSpace(custOrderNo) || string.IsNullOrWhiteSpace(sdNo))
			{
				IsVideoSessionReady = false;
				return Failure($"{CameraNo}機台: 未設定單號");
			}

			var command = string.Format("H,{0},{1},{2},{3}{4},{5}",
							CameraNo, custOrderNo, sdNo, Account, AccountName, msg);

			return SendCommand(command);
		}

		public ExecuteResult VideoCancelSession()
		{
			var command = string.Format("C,{0}", CameraNo);

			var result = SendCommand(command);
			if (!result.IsSuccessed)
				return result;

			IsVideoReady = false;
			return Success();
		}

		public ExecuteResult VideoEndSession(string custOrderNo, string sdNo, string msg)
		{
			IsVideoSessionReady = false;

			if (string.IsNullOrWhiteSpace(custOrderNo))
			{
				return Failure($"{CameraNo}機台: 未設定單號");
			}

			var command = string.Format("E,{0}", CameraNo);

			var result = SendCommand(command);
			if (!result.IsSuccessed)
				return result;

			IsVideoReady = false;
			return Success();
		}

		#endregion

		ExecuteResult SendCommand(string cmd)
		{
			//加入如果忽略攝影主機錯誤的話，預設就不啟用攝影主機
			if (IsIgnoreVideoError)
				return Success();

			// 需要先開始記錄過單號，才能發送其他資訊
			if (!IsVideoReady)
				return Failure($"{CameraNo}機台:錯誤未連線，請先連線攝影主機");

			//Thread.Sleep(100);

			var result = _remoteVideoServer.SendCommand(cmd);
			if (!result)
				return Failure($"{CameraNo}機台:{_remoteVideoServer.LastError}");

			return Success();
		}

		ExecuteResult Success() => new ExecuteResult { IsSuccessed = true };
		ExecuteResult Failure(string message, string no = null) => new ExecuteResult { IsSuccessed = false, No = no, Message = message };
	}
}
