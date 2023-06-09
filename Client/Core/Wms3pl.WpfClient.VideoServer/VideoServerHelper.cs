using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TeamMark;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
namespace Wms3pl.WpfClient.VideoServer
{
	public partial class VideoServerHelper
	{
		#region Private Property
		private RemotePort _remoteVideoServer;

		#endregion
		#region Public Property
		private List<VideoCamera> _videoCameras = null;
		public IEnumerable<VideoCamera> VideoCameras
		{
			get
			{
				if (_videoCameras == null)
					return Enumerable.Empty<VideoCamera>();

				return _videoCameras.OrderByDescending(x => x.IsVideoReady).ThenByDescending(x => x.IsVideoSessionReady);
			}
		}

		/// <summary>
		/// 攝影機連線Url
		/// </summary>
		public string VideoServerUrl { get; private set; }

		/// <summary>
		/// 是否忽略攝影機主機錯誤(不啟用攝影機)
		/// </summary>
		public bool IsIgnoreVideoError { get; private set; }
		#endregion

		#region Construtor
		public VideoServerHelper()
		{
			_remoteVideoServer = new RemotePort();
		}
		#endregion

		#region Method

		/// <summary>
		/// 連線攝影幾主機
		/// </summary>
		/// <returns></returns>
		public ExecuteResult ConnectVideoServer(string videoServerUrl, bool isIgnoreVideoError, string cameraNos, string account, string accountName)
		{
			cameraNos = cameraNos ?? string.Empty;
			var cameras = cameraNos.Split(',').Select(cameraNo => cameraNo.Trim()).ToArray();
			if (!cameras.Any())
				return Failure("必須設定攝影機台編號");

			return ConnectVideoServer(videoServerUrl, isIgnoreVideoError, cameras, account, accountName);
		}

		/// <summary>
		/// 連線攝影幾主機
		/// </summary>
		/// <returns></returns>
		public ExecuteResult ConnectVideoServer(string videoServerUrl, bool isIgnoreVideoError, string[] cameraNos, string account, string accountName)
		{
			if (cameraNos == null || !cameraNos.Any())
				return Failure("必須設定攝影機台編號");

			if (string.IsNullOrWhiteSpace(account))
				return Failure("必須設定人員帳號");

			VideoServerUrl = videoServerUrl;
			IsIgnoreVideoError = isIgnoreVideoError;

			_videoCameras = cameraNos.Where(cameraNo => !string.IsNullOrWhiteSpace(cameraNo))
								.Select(cameraNo => cameraNo.Trim())
								.Select(cameraNo => new VideoCamera(_remoteVideoServer, isIgnoreVideoError, cameraNo, account, accountName))
								.ToList();

			//加入如果忽略攝影主機錯誤的話，預設就不啟用攝影主機
			if (IsIgnoreVideoError)
				return Success();

			return ConnectVideoServer();
		}

		private ExecuteResult ConnectVideoServer()
		{
			_videoCameras.ForEach(x => x.SetIsVideoReady(false));

			// 設定 WebService Url
			var result = _remoteVideoServer.Connect(VideoServerUrl);
			if (!result)
				return Failure($"連線網址：{VideoServerUrl} 失敗");

			// 開啟 Port 試試看有沒有連接
			result = _remoteVideoServer.OpenPort();
			if (!result)
				return Failure($"開啟{VideoServerUrl}通訊阜Port：失敗{Environment.NewLine}{_remoteVideoServer.LastError}");

			_videoCameras.ForEach(x => x.SetIsVideoReady(true));

			return Success();
		}


		public ExecuteResult CloseVideoPort()
		{
			//加入如果忽略攝影主機錯誤的話，預設就不啟用攝影主機
			if (IsIgnoreVideoError)
				return Success();

			// 關閉 Port
			var result = _remoteVideoServer.ClosePort();
			if (!result)
				return Failure($"關閉通訊阜Port：{VideoServerUrl} 失敗{Environment.NewLine}{_remoteVideoServer.LastError}");

			_videoCameras.ForEach(x => x.SetIsVideoReady(false));
			_videoCameras.ForEach(x => x.SetIsVideoSessionReady(false));
			return Success();
		}

		/// <summary>
		/// 某筆單錄影開始
		/// </summary>
		/// <param name="custOrderNo"></param>
		/// <param name="sdNo"></param>
		/// <returns></returns>
		public ExecuteResult VideoStartSessionByCustOrderNo(string custOrderNo, string sdNo)
		{
			ConnectVideoServer();

			foreach (var camera in VideoCameras)
			{
				var result = camera.VideoStartSessionByCustOrderNo(custOrderNo, sdNo);
				if (!result.IsSuccessed)
					return result;
			}
			return Success();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="date"></param>
		/// <param name="custOrderNo"></param>
		/// <param name="custOrderName"></param>
		/// <returns></returns>
		public ExecuteResult VideoStartSessionByCustOrderNo(string date, string custOrderNo, string custOrderName)
		{
			ConnectVideoServer();

			foreach (var camera in VideoCameras)
			{
				var result = camera.VideoStartSessionByCustOrderNo(date, custOrderNo, custOrderName);
				if (!result.IsSuccessed)
					return result;
			}
			return Success();
		}

		/// <summary>
		/// 單號, 店名, 店號, 出貨日, 掃描人員
		/// </summary>
		/// <param name="custOrderNo"></param>
		/// <param name="retailName"></param>
		/// <param name="retailCode"></param>
		/// <param name="delvDate"></param>
		/// <returns></returns>
		public ExecuteResult VideoStartSessionByCustOrderNo(string custOrderNo, string retailName, string retailCode, DateTime delvDate)
		{
			ConnectVideoServer();

			foreach (var camera in VideoCameras)
			{
				var result = camera.VideoStartSessionByCustOrderNo(custOrderNo, retailName, retailCode, delvDate.ToString("yyyy/MM/dd"));
				if (!result.IsSuccessed)
					return result;
			}
			return Success();
		}

		/// <summary>
		/// 退貨單號,店名,店號,路線,司機,退貨日,退貨掃描人員
		/// </summary>
		/// <param name="rsiOrdNo"></param>
		/// <param name="retailName"></param>
		/// <param name="retailCode"></param>
		/// <param name="delvNo"></param>
		/// <param name="dirverName"></param>
		/// <param name="delvDate"></param>
		/// <returns></returns>
		public ExecuteResult VideoStartSessionByRsiOrdNo(string rsiOrdNo, string retailName, string retailCode, string delvNo, string dirverName, DateTime delvDate)
		{
			ConnectVideoServer();

			foreach (var camera in VideoCameras)
			{
				var result = camera.VideoStartSessionByRsiOrdNo(rsiOrdNo, retailName, retailCode, delvNo, dirverName, delvDate.ToString("yyyy/MM/dd"));
				if (!result.IsSuccessed)
					return result;
			}
			return Success();
		}

		/// <summary>
		/// 單號, 託運單號, 商品名稱, barcode, 總數, 刷讀數, ??單號, ??順序
		/// </summary>
		/// <param name="custOrderNo"></param>
		/// <param name="sdNo"></param>
		/// <param name="itemName"></param>
		/// <param name="itemBarCode"></param>
		/// <param name="orderQty"></param>
		/// <param name="scanQty"></param>
		/// <param name="rsiOrdNo"></param>
		/// <param name="rsiOrdSeq"></param>
		/// <returns></returns>
		public ExecuteResult VideoShowItemByScan(string custOrderNo, string sdNo, string itemName,
											string itemBarCode, decimal orderQty, decimal scanQty, string rsiOrdNo, string rsiOrdSeq)
		{
			ConnectVideoServer();

			foreach (var camera in VideoCameras)
			{
				var result = camera.VideoShowItemByScan(custOrderNo, sdNo, itemName, itemBarCode, orderQty, scanQty, rsiOrdNo, rsiOrdSeq);
				if (!result.IsSuccessed)
					return result;
			}
			return Success();
		}

		public ExecuteResult VideoShowItemByScan(string itemName,
											string itemBarCode, string locCode, string info, string rsiOrdNo, string rsiOrdSeq)
		{
			ConnectVideoServer();

			foreach (var camera in VideoCameras)
			{
				var result = camera.VideoShowItemByScan(itemName, itemBarCode, locCode, info, rsiOrdNo, rsiOrdSeq);
				if (!result.IsSuccessed)
					return result;
			}
			return Success();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemCode">出貨單號</param>
		/// <param name="itemName">箱號</param>
		/// <param name="orderQty"></param>
		/// <param name="pastNo">託運單</param>
		/// <returns></returns>
		public ExecuteResult VideoShowItemByPastNo(string itemCode, string itemName, decimal orderQty, string pastNo)
		{
			ConnectVideoServer();

			foreach (var camera in VideoCameras)
			{
				var result = camera.VideoShowItemByPastNo(itemCode, itemName, orderQty, pastNo);
				if (!result.IsSuccessed)
					return result;
			}
			return Success();
		}

		public ExecuteResult VideoShowItemByBoxNo(string itemCode, string itemName, decimal orderQty, string boxNo, string remark)
		{
			ConnectVideoServer();

			foreach (var camera in VideoCameras)
			{
				var result = camera.VideoShowItemByBoxNo(itemCode, itemName, orderQty, boxNo, remark);
				if (!result.IsSuccessed)
					return result;
			}
			return Success();
		}

		public ExecuteResult VideoShowWeight(string custOrderNo, string sdNo, int scanBoxQty, decimal weight)
		{
			ConnectVideoServer();

			foreach (var camera in VideoCameras)
			{
				var result = camera.VideoShowWeight(custOrderNo, sdNo, scanBoxQty, weight);
				if (!result.IsSuccessed)
					return result;
			}
			return Success();
		}

		public ExecuteResult VideoChangeCaption(string custOrderNo, string sdNo, string account, string msg)
		{
			ConnectVideoServer();

			foreach (var camera in VideoCameras)
			{
				var result = camera.VideoChangeCaption(custOrderNo, sdNo, msg);
				if (!result.IsSuccessed)
					return result;
			}
			return Success();
		}

		public ExecuteResult VideoCancelSession()
		{
			ConnectVideoServer();

			foreach (var camera in VideoCameras)
			{
				var result = camera.VideoCancelSession();
				if (!result.IsSuccessed)
					return result;
			}
			return Success();
		}

		public ExecuteResult VideoEndSession(string custOrderNo, string sdNo, string msg)
		{
			ConnectVideoServer();

			foreach (var camera in VideoCameras)
			{
				var result = camera.VideoEndSession(custOrderNo, sdNo, msg);
				if (!result.IsSuccessed)
					return result;
			}
			return Success();
		}

		#endregion

		ExecuteResult Success() => new ExecuteResult { IsSuccessed = true };
		ExecuteResult Failure(string message, string no = null) => new ExecuteResult { IsSuccessed = false, No = no, Message = message };
	}
}
