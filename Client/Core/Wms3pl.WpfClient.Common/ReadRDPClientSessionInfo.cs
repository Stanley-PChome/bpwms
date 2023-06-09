using System;
using System.Runtime.InteropServices;

namespace Wms3pl.WpfClient.Common
{
  public static class ReadRdpClientSessionInfo
  {
    #region Constants

    private const int WtsCurrentSession = -1;

    #endregion

    #region Dll Imports

    [DllImport("Wtsapi32.dll")]
    private static extern bool WTSQuerySessionInformation(
      IntPtr pServer,
      int iSessionId,
      WtsInfoClass oInfoClass,
      out IntPtr pBuffer,
      out uint iBytesReturned);

    [DllImport("wtsapi32.dll")]
    private static extern void WTSFreeMemory(
      IntPtr pMemory);

    #endregion

    #region Structures

    //Structure for Terminal Service Client IP Address

    #region Nested type: WtsClientAddress

    [StructLayout(LayoutKind.Sequential)]
    private struct WtsClientAddress
    {
      public readonly int iAddressFamily;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
      public readonly byte[] bAddress;
    }

    #endregion

    #endregion

    #region Enumurations

    #region WtsConnectstateClass enum

    private enum WtsConnectstateClass
    {
      WtsActive,
      WtsConnected,
      WtsConnectQuery,
      WtsShadow,
      WtsDisconnected,
      WtsIdle,
      WtsListen,
      WtsReset,
      WtsDown,
      WtsInit
    }

    #endregion

    #region WtsInfoClass enum

    private enum WtsInfoClass
    {
      WtsInitialProgram,
      WtsApplicationName,
      WtsWorkingDirectory,
      WtsoemId,
      WtsSessionId,
      WtsUserName,
      WtsWinStationName,
      WtsDomainName,
      WtsConnectState,
      WtsClientBuildNumber,
      WtsClientName,
      WtsClientDirectory,
      WtsClientProductId,
      WtsClientHardwareId,
      WtsClientAddress,
      WtsClientDisplay,
      WtsClientProtocolType,
      WtsIdleTime,
      WtsLogonTime,
      WtsIncomingBytes,
      WtsOutgoingBytes,
      WtsIncomingFrames,
      WtsOutgoingFrames,
      WtsClientInfo,
      WtsSessionInfo,
      WtsConfigInfo,
      WtsValidationInfo,
      WtsSessionAddressV4,
      WtsIsRemoteSession
    }

    #endregion

    #endregion


    private static bool GetRdpIsRemote()
    {
      var pServer = IntPtr.Zero;
      IntPtr pAddress;
      uint iReturned;
      if (WTSQuerySessionInformation(pServer, WtsCurrentSession, WtsInfoClass.WtsClientName,
                                     out pAddress, out iReturned))
      {
        var clientName = Marshal.PtrToStringAnsi(pAddress);
        WTSFreeMemory(pAddress);
        return clientName.Trim() != string.Empty;
      }
      return false;
    }

    public static string GetRdpClientIpAddress()
    {
      //判斷如果是Rdp Session 才回傳資訊
      if (GetRdpIsRemote())
      {
        var pServer = IntPtr.Zero;
        IntPtr pAddress;
        uint iReturned;
        if (WTSQuerySessionInformation(pServer, WtsCurrentSession, WtsInfoClass.WtsClientAddress,
                                       out pAddress, out iReturned))
        {
          var oClientAddres = (WtsClientAddress)Marshal.PtrToStructure(pAddress, typeof(WtsClientAddress));
          var sIpAddress = string.Format("{0}.{1}.{2}.{3}",
                                         oClientAddres.bAddress[2], oClientAddres.bAddress[3], oClientAddres.bAddress[4], oClientAddres.bAddress[5]);
          WTSFreeMemory(pAddress);
          return sIpAddress;
        }
      }
      return "0.0.0.0";
    }

    public static string GetRdpClientName()
    {
      //判斷如果是Rdp Session 才回傳資訊
      if (GetRdpIsRemote())
      {
        var pServer = IntPtr.Zero;
        IntPtr pAddress;
        uint iReturned;
        if (WTSQuerySessionInformation(pServer, WtsCurrentSession, WtsInfoClass.WtsClientName,
                                       out pAddress, out iReturned))
        {
          var clientName = Marshal.PtrToStringAnsi(pAddress);
          WTSFreeMemory(pAddress);
          return clientName;
        }
      }
			else
			{
				return System.Net.Dns.GetHostName();
			}
      return "localPC";
    }
  }
}

