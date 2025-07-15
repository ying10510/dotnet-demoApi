## 系統架構環境
- **ASP.NET Core**：版本 `.NET 9.0.203`
- **C# 語言版本**：v13
- **資料庫**：Microsoft SQL Server（透過 Docker 建立）
- **快取/黑名單儲存**：Redis（透過 Docker 建立）
- **日誌紀錄**：ELK Stack（Elasticsearch + Logstash + Kibana，透過 Docker 建立）

## 系統流程關係圖
```mermaid
flowchart TD
    %% 模組分區
    subgraph Server[𝗔𝗣]
        A1[API Request]
        A2(EF Core 資料存取)
        A3(Redis 黑名單驗證)
        A4(Serilog 日誌輸出)
    end

    subgraph DB[𝗗𝗕]
        B[MSSQL]
    end

    subgraph Cache[𝗖𝗮𝗰𝗵𝗲]
        C[Redis]
    end

    subgraph LogPipeline[𝗘𝗟𝗞]
        D[Logstash]
        E[Elasticsearch]
        F[Kibana]
    end

    %% 連線流程
    A1 --> A2
    A2 --> | CRUD | B
    B --> | 查詢結果 | A2

    A1 --> A3
    A3 --> | JWT驗證 / 寫入 Token Blacklist | C
    C --> | 驗證結果 | A3

    A1 --> A4
    A4 --> | JSON over TCP | D
    D --> | Log儲存至 | E
    E --> | 查詢與視覺化 | F

    %% 虛線樣式設定（針對 subgraph）
    classDef dashedBox stroke-dasharray: 5 5, stroke:#999, stroke-width:2px, fill:none;
    class Server,DB,Cache,LogPipeline dashedBox;

    %% 樣式強化
    style A1 stroke:#2196F3,stroke-width:2px
    style B stroke:#9C27B0,stroke-width:2px
    style C stroke:#FBC02D,stroke-width:2px
    style D stroke:#26A69A,stroke-width:2px
    style E stroke:#607D8B,stroke-width:2px
    style F stroke:#8BC34A,stroke-width:2px

```


## 系統功能總覽

### 🔐 JWT Token 驗證機制
- 採用標準 JWT 格式實作登入與授權流程
- 支援解析與驗證過期時間

### 📚 Swagger / OpenAPI 文件
- 自動產生 API 說明文件
- 整合 JWT Bearer 驗證支援測試
- 可透過瀏覽器存取並測試 API

### 🌐 全域例外處理（GlobalExceptionMiddleware）
- 捕捉所有未處理例外，統一格式回應
- 區分：
  - 預期型錯誤（商業邏輯例外）
  - 非預期錯誤（系統例外、未處理例外）

### ⏱️ RequestTimingMiddleware
- 自動記錄每一筆 API 請求處理時間
- 提供日誌分析依據（整合 Serilog）

### 🧊 Redis 黑名單機制
- 登出後將 JWT 加入 Redis 黑名單
- 每次請求驗證時自動檢查是否為黑名單 Token


### 📊 Serilog 結構化日誌
- 使用 Serilog 記錄結構化日誌
- 支援輸出至 Console 與 ELK (透過 TCP + JSON 格式)

### 📈 ELK 整合
- Logstash 透過 TCP 接收 Serilog 日誌
- Elasticsearch 儲存日誌
- Kibana 顯示可視化介面
- 支援 JSON codec 與結構化格式過濾
