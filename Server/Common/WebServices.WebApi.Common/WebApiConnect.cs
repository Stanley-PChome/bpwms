using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;

namespace WebServices.WebApi.Common
{
  public class WebApiConnect
  {
    private string _accessToken;
    private string _tokenAddress;
    private string _tokenClientId;
    private string _tokenSecret;
    private string _tokenscope;
    private int _retryConnectCount = 3;
    /// <summary>
    /// 是否連線成功
    /// </summary>
    public bool IsConnectSuccess { get; set; }
    /// <summary>
    /// 連線錯誤訊息
    /// </summary>
    public string ErrorMessage { get; set; }
    /// <summary>
    /// 是否使用OAuth2.0
    /// </summary>
    public bool IsUseOAuth2 { get; set; }

    /// <summary>
    /// 是否使用SSL
    /// </summary>
    public bool IsDisabledSSLValidate { get; set; }

    public bool IsUseSSL { get; set; }

    /// <summary>
    /// 最小間隔參數
    /// </summary>
    public int MillisecondsTimeout { get; set; } = 0;
    /// <summary>
    /// Time out(超時)設定
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(15);// 預設15分鐘
                                                                     /// <summary>
                                                                     /// 是否逾時
                                                                     /// </summary>
    public bool IsTimeout { get; set; } = false;

    /// <summary>
    /// Log 資料夾路徑
    /// </summary>
    public string LogFolderPath { get; set; }

    public string CurrentStaff { get; set; } //紀錄登入者，防止Current.Staff被更換掉

    /// <summary>
    /// API回傳原始訊息
    /// </summary>
    public string OriReturnValue { get; private set; }

		/// <summary>
		/// API 回傳Http狀態代碼
		/// </summary>
		public string HttpCode { get; private set; }

		/// <summary>
		/// API 回傳 Http 內容
		/// </summary>
		public string HttpContentValue { get; private set; }

		/// <summary>
		/// WebApi連線
		/// </summary>
		public WebApiConnect()
    {
      LogFolderPath = ConfigurationManager.AppSettings["HttpClientLogFolder"].ToString();
    }
    /// <summary>
    /// WebApi連線
    /// </summary>
    /// <param name="tokenAddress">token地址</param>
    /// <param name="tokenClientId">token連線Id</param>
    /// <param name="tokenSecret">token安全密碼</param>
    /// <param name="tokenscope">token Scope</param>
    public WebApiConnect(string tokenAddress, string tokenClientId, string tokenSecret, string tokenscope)
    {
      _tokenAddress = tokenAddress;
      _tokenClientId = tokenClientId;
      _tokenSecret = tokenSecret;
      _tokenscope = tokenscope;
      LogFolderPath = ConfigurationManager.AppSettings["HttpClientLogFolder"].ToString();
    }
    /// <summary>
    /// 連線認證取得token
    /// </summary>
    private void ConnectToken()
    {
      var count = 0;
      do
      {
        var tokenClient = new TokenClient(_tokenAddress, _tokenClientId, _tokenSecret);
        var result = tokenClient.RequestClientCredentialsAsync(_tokenscope).GetAwaiter().GetResult();
        if (result.IsError)
        {
          count++;
          IsConnectSuccess = false;
          ErrorMessage = result.Error;
        }
        else if (result.IsHttpError)
        {
          count++;
          IsConnectSuccess = false;
          ErrorMessage = result.HttpErrorStatusCode + result.HttpErrorReason;
        }
        else
        {
          _accessToken = result.AccessToken;
          count = _retryConnectCount;
          IsConnectSuccess = true;
        }
        tokenClient = null;
      } while (_retryConnectCount > count);
    }

    /// <summary>
    /// 使用HttpGet 取得Api資料
    /// T:回傳物件
    /// </summary>
    /// <param name="requestUrl">api路徑</param>
    /// <param name="httpHeaderList">傳送時增加httpHeader設定</param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string requestUrl, List<KeyValuePair<string, string>> httpHeaderList = null)
    {
			OriReturnValue = string.Empty;
			IsTimeout = false;
			IsConnectSuccess = false;
			HttpCode = string.Empty;
			HttpContentValue = string.Empty;
			T data = default(T);
      if (IsUseOAuth2)
      {
        if (string.IsNullOrWhiteSpace(_accessToken))
          ConnectToken();
        if (!IsConnectSuccess)
          return data;
      }

      var count = 0;
      do
      {
        if (IsUseSSL)
        {
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
        if (IsDisabledSSLValidate)
        {
          ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }
        using (var httpClient = new HttpClient())
        {
          httpClient.Timeout = Timeout;
          if (httpHeaderList != null && httpHeaderList.Any())
          {
            foreach (var header in httpHeaderList)
              httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
          }
          if (IsUseOAuth2) { httpClient.SetBearerToken(_accessToken); }

          HttpResponseMessage response = new HttpResponseMessage();
          var responseStr = String.Empty;
          try
          {
            response = await httpClient.GetAsync(requestUrl);
						responseStr = await response.Content.ReadAsStringAsync();
						OriReturnValue = responseStr;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = responseStr;
						if (response.IsSuccessStatusCode)
            {
              data = JsonConvert.DeserializeObject<T>(responseStr);
              count = _retryConnectCount;
              IsConnectSuccess = true;
            }
            else
            {
              count++;
              IsConnectSuccess = false;
              ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, responseStr);
            }
            IsTimeout = false;
            //WriteLog(requestUrl, "HttpGet", "", response != null ? ((int)response.StatusCode).ToString() : "", "", "", responseStr);
          }
          catch (TaskCanceledException ex)
          {
            IsTimeout = true;
            count++;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.Message;
						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.Message);
            //WriteLog(requestUrl, "HttpGet", "", response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr);
          }
          catch (Exception ex)
          {
            count = _retryConnectCount;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.Message;
						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.Message);
            //WriteLog(requestUrl, "HttpGet", "", response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr);
          }
          //                    finally
          //                    {
          //#if (DEBUG)
          //                        WriteLog(requestUrl, "HttpGet", "", response != null ? ((int)response.StatusCode).ToString() : "", "", "", ResponseStr);
          //#endif
          //                    }

        }
        if (MillisecondsTimeout > 0)
          Thread.Sleep(MillisecondsTimeout);
      } while (_retryConnectCount > count);
      return data;
    }

    /// <summary>
    /// 使用HttpGet 取得Api資料
    /// T:回傳物件
    /// </summary>
    /// <param name="requestUrl">api路徑</param>
    /// <param name="paramList">傳送參數</param>
    /// <param name="httpHeaderList">傳送時增加httpHeader設定</param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string requestUrl, List<KeyValuePair<string, string>> paramList, List<KeyValuePair<string, string>> httpHeaderList = null)
    {
      var paramUrl = string.Empty;
      if (paramList != null && paramList.Any())
        foreach (var item in paramList)
          paramUrl += ((!string.IsNullOrWhiteSpace(paramUrl)) ? "&" : "") + string.Format("{0}={1}", item.Key, item.Value);
      if (!string.IsNullOrWhiteSpace(paramUrl))
        requestUrl += string.Format("?{0}", paramUrl);
      return await GetAsync<T>(requestUrl, httpHeaderList);
    }

    /// <summary>
    /// 使用HttpPut 送出資料 並回傳結果
    /// T:回傳物件
    /// </summary>
    /// <param name="requestUrl">api路徑</param>
    /// <param name="paramList">傳送參數</param>
    /// <param name="httpHeaderList">傳送時增加httpHeader設定</param>
    /// <returns>回傳結果</returns>
    public async Task<T> PutAsync<T>(string requestUrl, List<KeyValuePair<string, string>> paramList, List<KeyValuePair<string, string>> httpHeaderList = null)
    {
			OriReturnValue = string.Empty;
			IsTimeout = false;
			IsConnectSuccess = false;
			HttpCode = string.Empty;
			HttpContentValue = string.Empty;

			T data = default(T);
      if (IsUseOAuth2)
      {
        if (string.IsNullOrWhiteSpace(_accessToken))
          ConnectToken();
        if (!IsConnectSuccess)
          return data;
      }

      var count = 0;
      do
      {
        if (IsUseSSL)
        {
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
        if (IsDisabledSSLValidate)
        {
          ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }
        using (var httpClient = new HttpClient())
        {
          httpClient.Timeout = Timeout;
          if (httpHeaderList != null && httpHeaderList.Any())
          {
            foreach (var header in httpHeaderList)
              httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
          }
          if (IsUseOAuth2) { httpClient.SetBearerToken(_accessToken); }
          var content = new FormUrlEncodedContent(paramList);

          HttpResponseMessage response = new HttpResponseMessage();
          var responseStr = String.Empty;
          try
          {
            response = await httpClient.PutAsync(requestUrl, content);
						responseStr = await response.Content.ReadAsStringAsync();
						OriReturnValue = responseStr;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = responseStr;

						if (response.IsSuccessStatusCode)
            {
              data = JsonConvert.DeserializeObject<T>(responseStr);
              count = _retryConnectCount;
              IsConnectSuccess = true;
            }
            else
            {
              count++;
              IsConnectSuccess = false;
              ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, responseStr);
            }
            IsTimeout = false;
            //WriteLog(requestUrl, "HttpPut", paramList, response != null ? ((int)response.StatusCode).ToString() : "", "", "", responseStr);
          }
          catch (TaskCanceledException ex)
          {
            IsTimeout = true;
            count++;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.Message;
						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.Message);
            //WriteLog(requestUrl, "HttpPut", paramList, response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr);
          }
          catch (Exception ex)
          {
            count = _retryConnectCount;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.Message;
						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.Message);
            //WriteLog(requestUrl, "HttpPut", paramList, response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr);
          }
          //                    finally
          //                    {
          //#if (DEBUG)
          //                        WriteLog(requestUrl, "HttpPut", paramList, response != null ? ((int)response.StatusCode).ToString() : "", "", "", ResponseStr);
          //#endif
          //                    }

        }
        if (MillisecondsTimeout > 0)
          Thread.Sleep(MillisecondsTimeout);
      } while (_retryConnectCount > count);
      return data;
    }

    /// <summary>
    /// 使用HttpPut 送出資料 並回傳結果
    /// T1:傳輸物件
    /// T2:回傳物件
    /// </summary>
    /// <param name="requestUrl">api路徑</param>
    /// <param name="paramData">傳送物件(需為序列化物件)</param>
    /// <param name="httpHeaderList">傳送時增加httpHeader設定</param>
    /// <returns>回傳結果</returns>
    public async Task<T2> PutAsync<T1, T2>(string requestUrl, T1 paramData, List<KeyValuePair<string, string>> httpHeaderList = null)
    {
			OriReturnValue = string.Empty;
			IsTimeout = false;
			IsConnectSuccess = false;
			HttpCode = string.Empty;
			HttpContentValue = string.Empty;

			T2 data = default(T2);
			if (IsUseOAuth2)
      {
        if (string.IsNullOrWhiteSpace(_accessToken))
          ConnectToken();
        if (!IsConnectSuccess)
          return data;
      }

      var count = 0;
      do
      {
        if (IsUseSSL)
        {
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
        if (IsDisabledSSLValidate)
        {
          ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }
        using (var httpClient = new HttpClient())
        {
          httpClient.Timeout = Timeout;
          if (httpHeaderList != null && httpHeaderList.Any())
          {
            foreach (var header in httpHeaderList)
              httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
          }
          if (IsUseOAuth2) { httpClient.SetBearerToken(_accessToken); }

          HttpResponseMessage response = new HttpResponseMessage();
          var responseStr = String.Empty;

          try
          {
            response = await httpClient.PutAsJsonAsync(requestUrl, paramData).ConfigureAwait(false);
						responseStr = await response.Content.ReadAsStringAsync();
						OriReturnValue = responseStr;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = responseStr;
						if (response.IsSuccessStatusCode)
            {
              data = JsonConvert.DeserializeObject<T2>(responseStr);
              count = _retryConnectCount;
              IsConnectSuccess = true;
            }
            else
            {
              count++;
              IsConnectSuccess = false;
              ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, responseStr);
            }
            IsTimeout = false;
            //WriteLog(requestUrl, "HttpPut", paramData, response != null ? ((int)response.StatusCode).ToString() : "", "", "", responseStr);
          }
          catch (TaskCanceledException ex)
          {
            IsTimeout = true;
            count++;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.Message;

						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.Message);
            //WriteLog(requestUrl, "HttpPut", paramData, response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr);
          }
          catch (Exception ex)
          {
            count = _retryConnectCount;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.Message;
						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.Message);
            //WriteLog(requestUrl, "HttpPut", paramData, response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr);
          }
          //                    finally
          //                    {
          //#if (DEBUG)
          //                        WriteLog(requestUrl, "HttpPut", paramData, response != null ? ((int)response.StatusCode).ToString() : "", "", "", ResponseStr);
          //#endif
          //                    }

        }
        if (MillisecondsTimeout > 0)
          Thread.Sleep(MillisecondsTimeout);
      } while (_retryConnectCount > count);
      return data;
    }

    /// <summary>
    /// 使用HttpPost 送出資料 並回傳結果
    /// T:回傳物件
    /// </summary>
    /// <param name="requestUrl">api路徑</param>
    /// <param name="paramList">傳送參數</param>
    /// <param name="httpHeaderList">傳送時增加httpHeader設定</param>
    /// <returns>回傳結果</returns>
    public async Task<T> PostAsync<T>(string requestUrl, List<KeyValuePair<string, string>> paramList, List<KeyValuePair<string, string>> httpHeaderList = null)
    {
      OriReturnValue = "";
			IsTimeout = false;
			IsConnectSuccess = false;
			HttpCode = string.Empty;
			HttpContentValue = string.Empty;
			T data = default(T);
      if (IsUseOAuth2)
      {
        if (string.IsNullOrWhiteSpace(_accessToken))
          ConnectToken();
        if (!IsConnectSuccess)
          return data;
      }

      var count = 0;
      do
      {
        if (IsUseSSL)
        {
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
        if (IsDisabledSSLValidate)
        {
          ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }
        using (var httpClient = new HttpClient())
        {
          httpClient.Timeout = Timeout;
          if (httpHeaderList != null && httpHeaderList.Any())
          {
            foreach (var header in httpHeaderList)
              httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
          }
          if (IsUseOAuth2) { httpClient.SetBearerToken(_accessToken); }
          var content = new FormUrlEncodedContent(paramList);

          HttpResponseMessage response = new HttpResponseMessage();
          var responseStr = String.Empty;
          try
          {
            response = await httpClient.PostAsync(requestUrl, content);
						responseStr = await response.Content.ReadAsStringAsync();
						OriReturnValue = responseStr;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = responseStr;
						if (response.IsSuccessStatusCode)
            {
             
              data = JsonConvert.DeserializeObject<T>(responseStr);
              count = _retryConnectCount;
              IsConnectSuccess = true;
            }
            else
            {
              count++;
              IsConnectSuccess = false;
              ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, responseStr);
            }
            IsTimeout = false;
            //WriteLog(requestUrl, "HttpPost", paramList, response != null ? ((int)response.StatusCode).ToString() : "", "", "", responseStr);
          }
          catch (TaskCanceledException ex)
          {
            IsTimeout = true;
            count++;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.Message;
						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.Message);
            //WriteLog(requestUrl, "HttpPost", paramList, response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr);
          }
          catch (Exception ex)
          {
            count = _retryConnectCount;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.Message;
						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.Message);
            //WriteLog(requestUrl, "HttpPost", paramList, response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr);
          }
          //                    finally
          //                    {
          //#if (DEBUG)
          //                        WriteLog(requestUrl, "HttpPost", paramList, response != null ? ((int)response.StatusCode).ToString() : "", "", "", ResponseStr);
          //#endif
          //                    }

        }
        if (MillisecondsTimeout > 0)
          Thread.Sleep(MillisecondsTimeout);
      } while (_retryConnectCount > count);
      return data;
    }

    /// <summary>
    /// 使用HttpPost 送出資料 並回傳結果
    /// T1:傳輸物件
    /// T2:回傳物件
    /// </summary>
    /// <param name="requestUrl">api路徑</param>
    /// <param name="paramData">傳送物件(需為序列化物件)</param>
    /// <param name="httpHeaderList">傳送時增加httpHeader設定</param>
    /// <returns>回傳結果</returns>
    public async Task<T2> PostAsync<T1, T2>(string requestUrl, T1 paramData, List<KeyValuePair<string, string>> httpHeaderList = null)
    {
      OriReturnValue = "";
			IsTimeout = false;
			IsConnectSuccess = false;
			HttpCode = string.Empty;
			HttpContentValue = string.Empty;
			T2 data = default(T2);

      if (IsUseOAuth2)
      {
        if (string.IsNullOrWhiteSpace(_accessToken))
          ConnectToken();
        if (!IsConnectSuccess)
          return data;
      }

      var count = 0;
      do
      {
        if (IsUseSSL)
        {
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
        if (IsDisabledSSLValidate)
        {
          ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }
        using (var httpClient = new HttpClient())
        {
          httpClient.Timeout = Timeout;
          if (httpHeaderList != null && httpHeaderList.Any())
          {
            foreach (var header in httpHeaderList)
              httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
          }
          if (IsUseOAuth2) { httpClient.SetBearerToken(_accessToken); }

          HttpResponseMessage response = new HttpResponseMessage();
          var responseStr = String.Empty;

          try
          {
            response = await httpClient.PostAsJsonAsync(requestUrl, paramData).ConfigureAwait(false);
						responseStr = await response.Content.ReadAsStringAsync();
						OriReturnValue = responseStr;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = responseStr;

						if (response.IsSuccessStatusCode)
            {
              data = JsonConvert.DeserializeObject<T2>(responseStr);
              count = _retryConnectCount;
              IsConnectSuccess = true;
            }
            else
            {
              count++;
              IsConnectSuccess = false;
              ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, responseStr);
            }
            IsTimeout = false;
            //WriteLog(requestUrl, "HttpPost", paramData, response != null ? ((int)response.StatusCode).ToString() : "", "", "", responseStr);
          }
          catch (TaskCanceledException ex)
          {
            IsTimeout = true;
            count++;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.Message;
						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.Message);
            //WriteLog(requestUrl, "HttpPost", paramData, response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr);
          }
          catch (Exception ex)
          {
            count = _retryConnectCount;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.Message;
						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.Message);
            //WriteLog(requestUrl, "HttpPost", paramData, response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr);
          }
          //                    finally
          //                    {
          //#if (DEBUG)
          //                        WriteLog(requestUrl, "HttpPost", paramData, response != null ? ((int)response.StatusCode).ToString() : "", "", "", ResponseStr);
          //#endif
          //                    }

        }
        if (MillisecondsTimeout > 0)
          Thread.Sleep(MillisecondsTimeout);
      } while (_retryConnectCount > count);
      return data;
    }

    /// <summary>
    /// 使用HttpPost 送出資料 並回傳結果
    /// T1:傳輸物件
    /// T2:回傳物件
    /// </summary>
    /// <param name="requestUrl">api路徑</param>
    /// <param name="paramData">傳送物件(需為序列化物件)</param>
    /// <param name="httpHeaderList">傳送時增加httpHeader設定</param>
    /// <returns>回傳結果</returns>
    public async Task PostAsync<T>(string requestUrl, T paramData, List<KeyValuePair<string, string>> httpHeaderList = null)
    {
      OriReturnValue = "";
			IsTimeout = false;
			IsConnectSuccess = false;
			HttpCode = string.Empty;
			HttpContentValue = string.Empty;

			if (IsUseOAuth2)
      {
        if (string.IsNullOrWhiteSpace(_accessToken))
          ConnectToken();
      }

      var count = 0;
      do
      {
        if (IsUseSSL)
        {
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
        if (IsDisabledSSLValidate)
        {
          ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }
        using (var httpClient = new HttpClient())
        {
          httpClient.Timeout = Timeout;
          if (httpHeaderList != null && httpHeaderList.Any())
          {
            foreach (var header in httpHeaderList)
              httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
          }
          if (IsUseOAuth2) { httpClient.SetBearerToken(_accessToken); }

          HttpResponseMessage response = new HttpResponseMessage();
          var responseStr = String.Empty;
          try
          {
            response = await httpClient.PostAsJsonAsync(requestUrl, paramData);
						responseStr = await response.Content.ReadAsStringAsync();
						OriReturnValue = responseStr;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = responseStr;

						if (response.IsSuccessStatusCode)
            {
              count = _retryConnectCount;
              IsConnectSuccess = true;
            }
            else
            {
              count++;
              IsConnectSuccess = false;
              ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, responseStr);
            }
            IsTimeout = false;
            //WriteLog(requestUrl, "HttpPost", paramData, response != null ? ((int)response.StatusCode).ToString() : "", "", "", responseStr);
          }
          catch (TaskCanceledException ex)
          {
            IsTimeout = true;
            count++;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.Message;

						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.Message);
            //WriteLog(requestUrl, "HttpPost", paramData, response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr, true);
          }
          catch (Exception ex)
          {
            count = _retryConnectCount;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.Message;

						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.Message);
            //WriteLog(requestUrl, "HttpPost", paramData, response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr, true);
          }
          //					finally
          //					{
          //#if (DEBUG)
          //						WriteLog(requestUrl, "HttpPost", paramData, response != null ? ((int)response.StatusCode).ToString() : "", "", "", ResponseStr);
          //#endif
          //					}

        }
        if (MillisecondsTimeout > 0)
          Thread.Sleep(MillisecondsTimeout);
      } while (_retryConnectCount > count);
    }

    public async Task<T2> PostFileAsync<T1, T2>(string requestUrl, T1 paramData, List<KeyValuePair<string, string>> httpHeaderList = null) where T2 : LmsApiResult
    {
      OriReturnValue = "";
			IsTimeout = false;
			IsConnectSuccess = false;
			HttpCode = string.Empty;
			HttpContentValue = string.Empty;

			T2 data = Activator.CreateInstance<T2>();

      if (IsUseOAuth2)
      {
        if (string.IsNullOrWhiteSpace(_accessToken))
          ConnectToken();
        if (!IsConnectSuccess)
          return data;
      }
      var count = 0;
      do
      {
        if (IsUseSSL)
        {
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
        if (IsDisabledSSLValidate)
        {
          ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        using (var httpClient = new HttpClient())
        {

          httpClient.Timeout = Timeout;
          if (httpHeaderList != null && httpHeaderList.Any())
          {
            foreach (var header in httpHeaderList)
              httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
          }
          if (IsUseOAuth2) { httpClient.SetBearerToken(_accessToken); }
          HttpResponseMessage response = new HttpResponseMessage();
          var responseStr = String.Empty;
          CurrentStaff = Current.Staff;
          try
          {
            response = await httpClient.PostAsJsonAsync(requestUrl, paramData);
						
						switch (response.StatusCode)
            {
              case HttpStatusCode.Created: //201
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                data.Code = "201";
                data.Msg = "取得檔案";
                data.Data = fileBytes;
                if (response.Content.Headers.Contains("Content-Type"))
                {
                  data.ContentType = response.Content.Headers.GetValues("Content-Type").FirstOrDefault();
                  OriReturnValue = "ContentType:"+ data.ContentType;
                }
                break;
              case HttpStatusCode.OK: // 200
                responseStr = await response.Content.ReadAsStringAsync();
                OriReturnValue = responseStr;
                data = JsonConvert.DeserializeObject<T2>(responseStr);
                break;
							default:
								responseStr = await response.Content.ReadAsStringAsync();
								OriReturnValue = responseStr;
								break;
						}
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = responseStr;
						count = _retryConnectCount;
            IsConnectSuccess = true;
            //WriteLog(requestUrl, "HttpPostFile", paramData, response != null ? ((int)response.StatusCode).ToString() : "", "", "", responseStr);
          }
          catch (TaskCanceledException tc)
          {
            IsTimeout = true;
            count++;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = tc.ToString();
						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, tc.ToString());
            //WriteLog(requestUrl, "HttpPostFile", paramData, response != null ? ((int)response.StatusCode).ToString() : "", tc.Message, tc.ToString(), responseStr, true);
          }
          catch (Exception ex)
          {
            // 產生檔案失敗重新產生新的GUID並建立檔案，再失敗就不產檔
            IsTimeout = false;
            IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.ToString();
						count = _retryConnectCount;
            ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.ToString());
            //WriteLog(requestUrl, "HttpPostFile", paramData, response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr, true);
          }
          //                    finally
          //                    {
          //#if (DEBUG)
          //                        WriteLog(requestUrl, "HttpPostFile", paramData, response != null ? ((int)response.StatusCode).ToString() : "", "", "", ResponseStr);
          //#endif
          //                    }

        }
      } while (_retryConnectCount > count);

      return data;
    }

		public async Task<T2> GetFileAsync<T1, T2>(string requestUrl, T1 paramData, List<KeyValuePair<string, string>> httpHeaderList = null) where T2 : LmsApiResult
		{
			OriReturnValue = "";
			IsTimeout = false;
			IsConnectSuccess = false;
			HttpCode = string.Empty;
			HttpContentValue = string.Empty;

			T2 data = Activator.CreateInstance<T2>();

			if (IsUseOAuth2)
			{
				if (string.IsNullOrWhiteSpace(_accessToken))
					ConnectToken();
				if (!IsConnectSuccess)
					return data;
			}
			var count = 0;
			do
			{
				if (IsUseSSL)
				{
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				}
				if (IsDisabledSSLValidate)
				{
					ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
				}

				using (var httpClient = new HttpClient())
				{

					httpClient.Timeout = Timeout;
					if (httpHeaderList != null && httpHeaderList.Any())
					{
						foreach (var header in httpHeaderList)
							httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
					}
					if (IsUseOAuth2) { httpClient.SetBearerToken(_accessToken); }
					HttpResponseMessage response = new HttpResponseMessage();
					var responseStr = String.Empty;
					CurrentStaff = Current.Staff;
					try
					{
						response = await httpClient.GetAsync(requestUrl);

						switch (response.StatusCode)
						{
							case HttpStatusCode.Created: //201
								var fileBytes = await response.Content.ReadAsByteArrayAsync();
								data.Code = "201";
								data.Msg = "取得檔案";
								data.Data = fileBytes;
								if (response.Content.Headers.Contains("Content-Type"))
								{
									data.ContentType = response.Content.Headers.GetValues("Content-Type").FirstOrDefault();
									OriReturnValue = "ContentType:" + data.ContentType;
								}
								break;
							case HttpStatusCode.OK: // 200
								responseStr = await response.Content.ReadAsStringAsync();
								OriReturnValue = responseStr;
								data = JsonConvert.DeserializeObject<T2>(responseStr);
								break;
							default:
								responseStr = await response.Content.ReadAsStringAsync();
								OriReturnValue = responseStr;
								break;
						}
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = responseStr;
						count = _retryConnectCount;
						IsConnectSuccess = true;
						//WriteLog(requestUrl, "HttpGetFile", paramData, response != null ? ((int)response.StatusCode).ToString() : "", "", "", responseStr);
					}
					catch (TaskCanceledException tc)
					{
						IsTimeout = true;
						count++;
						IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = tc.ToString();
						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, tc.ToString());
						//WriteLog(requestUrl, "HttpGetFile", paramData, response != null ? ((int)response.StatusCode).ToString() : "", tc.Message, tc.ToString(), responseStr, true);
					}
					catch (Exception ex)
					{
						// 產生檔案失敗重新產生新的GUID並建立檔案，再失敗就不產檔
						IsTimeout = false;
						IsConnectSuccess = false;
						HttpCode = ((int)response.StatusCode).ToString();
						HttpContentValue = ex.ToString();
						count = _retryConnectCount;
						ErrorMessage = string.Format("錯誤代碼:{0}錯誤訊息:{1}", (int)response.StatusCode, ex.ToString());
						//WriteLog(requestUrl, "HttpGetFile", paramData, response != null ? ((int)response.StatusCode).ToString() : "", ex.Message, ex.ToString(), responseStr, true);
					}
					//                    finally
					//                    {
					//#if (DEBUG)
					//                        WriteLog(requestUrl, "HttpPostFile", paramData, response != null ? ((int)response.StatusCode).ToString() : "", "", "", ResponseStr);
					//#endif
					//                    }

				}
			} while (_retryConnectCount > count);

			return data;
		}

		/*/// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"></param>
    /// <param name="method"></param>
    /// <param name="paramData"></param>
    /// <param name="httpStatusCode"></param>
    /// <param name="exceptionMsg"></param>
    /// <param name="exceptionFull"></param>
    /// <param name="returnOriData">回傳的原始內容</param>
    private void WriteLog<T>(string url, string method, T paramData, string httpStatusCode, string exceptionMsg, string exceptionFull, string returnOriData, bool isException = false)
    {
      if (isException ? isException : ConfigurationManager.AppSettings["HttpClientLogMode"].ToString() == "1")
      {
        var sb = new List<string>();
        sb.Add("============================================================");
        sb.Add(string.Format("DateTime:{0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
        sb.Add(string.Format("URL:{0}", url));
        sb.Add(string.Format("Http Method:{0}", method));
        sb.Add(string.Format("Param:{0}", JsonConvert.SerializeObject(paramData)));
        sb.Add(string.Format("Http StatusCode:{0}", httpStatusCode));
        sb.Add(string.Format("Exception Message:{0}", exceptionMsg));
        sb.Add(string.Format("Exception Full:{0}", exceptionFull));
        sb.Add(string.Format("Return Data:{0}", returnOriData));
        sb.Add("============================================================");
        var dir = new DirectoryInfo(Path.Combine(LogFolderPath, "HttpClientLog", DateTime.Now.ToString("yyyyMMdd")));
        if (IsConnectSuccess)
        {
          dir = new DirectoryInfo(Path.Combine(LogFolderPath, "HttpClientNormalLog", DateTime.Now.ToString("yyyyMMdd")));
        }

        try
        {
          if (!dir.Exists)
            dir.Create();
          var fileFullName = Path.Combine(dir.FullName, $"HttpClient_{CurrentStaff}_{Guid.NewGuid().ToString()}.log");

          using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
            sw.WriteLine(string.Join(Environment.NewLine, sb));
        }
        catch (Exception)
        {
          if (!dir.Exists)
            dir.Create();
          var fileFullName = Path.Combine(dir.FullName, $"HttpClient_{CurrentStaff}_{Guid.NewGuid().ToString()}.log");

          using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
            sw.WriteLine(string.Join(Environment.NewLine, sb));
        }
      }
    }*/

  }
}
