# 金財通WMS開發環境設定

---

Leo Shiang，2023/06/06

---

## 一、WMS

### 1.1 環境需求

* Windows 10/11
* 已安裝 Visual Studio 2017 任一版本

### 1.2 安裝 .NET 3.5.1

由於 Microsoft Enterprise Library 5.0 需要 .NET 3.5.1 才能安裝，因此請先按照以下步驟安裝 .NET 3.5：

* 進入`控制台` \ `解除應用程式` \ `開啟和關閉 Window 功能`

* 勾選 `.NET Framework 3.5 (包括 NET 2.0 與 3.0)`

* 重新開機

### 1.3 安裝 SAP Crystal Report For Visual Studio 2017(Service Pack 22 Version 13.0.22)

請用以下網址下載 Service Pack 22 Version 13.0.22 32 Bit

https://origin.softwaredownloads.sap.com/public/file/0020000000628072019

檔案名稱為 `CRVS2010CR3222_0-10010309.ZIP`，解壓縮之後安裝。

### 1.4 安裝 Microsoft Enterprise Library 5.0

請用以下網址下載 Microsoft Enterprise Library 5.0

https://www.microsoft.com/en-us/download/details.aspx?id=15104

> 勾選 Enterprise Library 5.0.msi 即可。

檔案名稱為 Enterprise Library 5.0.msi，Double-click 執行。

### 1.5 下載程式

XXX 是您在 github 所建立的 token

```powershell
git clone https://XXX@github.com/PChome-logistics/cs-bpwms.git
```

> 假設目錄在 C:\cs-bpwms

### 1.6 手動加入憑證

Double-click 此檔案 `C:\cs-bpwms\WMS_PC\Client\WpfClient\WpfClient_TemporaryKey.pfx`

啟動憑證匯入精靈，遇到輸入密碼時請輸入`bankpro`，接下來一直按 NEXT 到結束。

### 1.7 編譯程式

用 Visual Studio 2017 開啟 `C:\cs-bpwms\WMS_PC\iWms_mss_ph.sln`。

依序重新建置 Client 目錄下的專案：

* PhWpfClient
* PhTestWpfClient
* PhA7WpfClient

### 1.8 手動安裝憑證(如果需要)

#### 1.8.1 將 sn.exe 的位置加入 PATH

以`管理員身份`開啟命令提示字元，輸入以下指令：

```powershell
cd \
dir sn.exe /s /b
```

會出現 sn.xee 的位置

```
C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\sn.exe
C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\x64\sn.exe
```

第一個 sn.exe 是 32 位元，將路徑記錄下來，輸入以下指令將路徑加入 PATH：

```bash
setx /M PATH "%PATH%";"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools"
```

執行完畢之後，重新開啟命令提示字元。

## 二、PDA

### 2.1 安裝 JDK 8

> 下載必須要有 Oracle 帳號。

* 前往 JDK 下載網頁[Java Archive Downloads - Java SE 8 (oracle.com)](https://www.oracle.com/java/technologies/javase/javase8-archive-downloads.html)

* 下載 Java SE Development Kit 8u202 的 Windows x64 版本，檔案名稱為 `jdk-8u202-windows-x64.exe`

* 安裝 JDK
* 設定環境變數 `JAVA_HOME`  = C:\Program Files\Java\jdk1.8.0_202

### 2.2 安裝 Android Studio Flamingo

* 前往 Android 下載網頁 https://developer.android.com/studio
* 下載 Android Studio Flamingo
* 安裝 Android SDK，在選擇 JDK 時請選擇 JDK 8

