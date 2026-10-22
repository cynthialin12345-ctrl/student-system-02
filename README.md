# 時間分類管理系統

ASP.NET Core MVC 專案，提供專注計時、歷史紀錄查詢與統計分析圖表。

## 環境需求

- .NET SDK 8.0 或更新版本
- SQL Server LocalDB 或 SQL Server

## 執行方式

1. 確認 `Final/appsettings.json` 的連線字串。

   預設連線：

   ```json
   "CmsContext": "Server=(localdb)\\MSSQLLocalDB;Database=TimeManager;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;"
   ```

2. 還原與建置專案。

   ```bash
   dotnet restore Final.sln
   dotnet build Final.sln
   ```

3. 啟動網站。

   ```bash
   dotnet run --project Final/Final.csproj
   ```

啟動時程式會套用 EF Core migrations，並在資料庫沒有初始資料時建立預設分類與使用者。需要先登入才能使用專注計時、歷史紀錄與統計分析。

## 資料庫

主要以 EF Core migrations 建立資料庫。`create_timemanager_database.sql` 是手動建庫備用腳本，schema 與 migrations 保持一致。

若需要展示用資料，可在建庫後另外執行 `insert_sample_data.sql`。這個腳本只插入範例細分類與活動紀錄，不屬於正式 schema 或啟動初始化流程。

預設使用者：

- Email: `developer@yzu.edu.tw`
- Password: `password123`

## 功能頁面

- 首頁：`/`
- 登入：`/Account/Login`
- 專注計時：`/Focus`
- 歷史紀錄：`/Activity/History`
- 統計分析：`/Statistics`
