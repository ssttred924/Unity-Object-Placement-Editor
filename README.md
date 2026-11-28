# Unity Object Placement Editor

Unity 3D 道具擺放編輯器

## Project Overview

本專案使用 Unity 與 C# 開發，實作一套 3D 場景道具擺放系統。

使用者可以透過右側道具清單，拖曳不同形狀的 3D 道具，並將其放置於具有高低起伏的有限範圍曲面場景中。

系統支援 Desktop 與 XR 兩種操作模式，提供物件預覽、放置位置判斷、表面對齊、場景物件選取及刪除等功能。

## Development Environment

- Unity 6
- C#
- Universal Render Pipeline (URP)
- XR Interaction Toolkit 3.3.2
- Input System

## Features

- **3D Object Placement**：透過拖曳方式將不同形狀的 3D 道具放置於具有高低起伏的曲面。
- **Placement Preview**：拖曳過程中即時顯示預覽物件，並跟隨滑鼠或 XR 控制器移動。
- **Surface Alignment**：依據地形表面法線調整物件旋轉與高度，使道具貼合曲面。
- **Placement Validation**：使用 Raycast 判斷有效放置位置，並透過紅綠引導線提示放置狀態。
- **Placement Guide Line**：拖曳過程中顯示引導線，呈現道具預計放置於曲面上的位置。
- **Object Selection**：選取場景物件並顯示 Highlight。
- **Object Interaction Menu**：在選取物件附近顯示互動選單，提供物件刪除功能。
- **Desktop / XR Mode Detection**：程式啟動時自動偵測 XR 裝置是否啟用，並依據偵測結果進入對應的操作模式。
- **Responsive UI**：支援不同視窗尺寸與解析度的 UI 佈局。

## Device Mode Detection

程式啟動時透過 `XRSettings.isDeviceActive` 判斷 XR 裝置是否處於啟用狀態，並依據偵測結果初始化對應的操作模式。

- **XR Device Detected**：啟用 XR Origin、XR 控制器及 XR UI 互動系統，進入 XR 模式。
- **No XR Device Detected**：啟用 Desktop Camera 與桌面 UI 互動系統，進入 Desktop 模式。

模式判斷於程式啟動時執行，不支援執行期間動態切換裝置模式。

## Controls

### Desktop Mode（桌面模式）

#### Camera Controls

相機操作方式與 Unity Scene View 類似。

| 操作 | 說明 |
|---|---|
| 滑鼠右鍵按住並移動 | 旋轉相機視角 |
| WASD | 前後左右移動相機 |
| Q / E | 向下／向上移動相機 |
| 滑鼠中鍵按住並拖曳 | 平移相機視角 |

#### Object Interaction

| 操作 | 說明 |
|---|---|
| 滑鼠左鍵 | 選取 UI 道具或場景物件 |
| 按住滑鼠左鍵拖曳道具 | 開始拖曳並顯示預覽物件 |
| 移動滑鼠 | 更新預覽物件位置 |
| 放開滑鼠左鍵 | 將道具放置於有效地形 |
| 點擊空白區域 | 取消目前物件選取 |
| 點擊刪除按鈕 | 刪除已選取物件 |
| 點擊道具清單按鈕 | 展開或收合道具清單 |

**Placement Indicators**

- 綠色引導線：目前位置可以放置道具。
- 紅色引導線：目前位置不可放置道具。
- 引導線消失：目前位置不可放置道具。

### XR Mode（XR 模式）

#### Movement Controls

| 操作 | 說明 |
|---|---|
| 頭部移動 | 改變觀察方向與視角 |
| 左手控制器搖桿 | 控制角色移動 |
| 右手控制器 | 控制射線方向與 UI 互動 |

#### Object Interaction

| 操作 | 說明 |
|---|---|
| 右手控制器射線 | 指向 UI 道具或場景物件 |
| 按住右手 Trigger 並指向道具 | 開始拖曳道具 |
| 移動右手控制器 | 更新預覽物件與放置位置 |
| 放開右手 Trigger | 將道具放置於有效地形 |
| 指向已放置物件並按下 Trigger | 選取物件並顯示互動選單 |
| 使用互動選單 | 操作或刪除已選取物件 |

## Demo Videos

### Play Mode Demo

[Watch Play Mode Demo](https://youtu.be/gZcAkOBMMnI)

影片展示內容：

- Desktop 模式的相機操作及道具擺放功能。
- XR Device Simulator 模擬 XR 控制器操作。
- XR 模式下的 UI 互動、道具拖曳放置及物件選取。
- 物件互動選單與刪除功能。
- 刪除物件時，可同步觀察 Unity Hierarchy，確認場景中的物件已被移除。

### Windows Build Demo

[Watch Windows Build Demo](https://youtu.be/fvw0AZDosvM)

展示正式建置後的 Windows 執行檔及實際操作結果。

## Windows Build

- Platform：Windows
- Architecture：x86_64
- Build：Release Build

**Download Windows Build:**

[Download Object Placement Editor](https://drive.google.com/file/d/1A-nIg_dZRXwxVZ-n-L4fpsgtnx3qbPv4/view?usp=sharing)

下載後請解壓縮完整 Build 資料夾，並執行 `ObjectPlacementEditor.exe`。

請保留 Unity 建置產生的 `_Data` 資料夾及其他必要檔案，不可只單獨複製或執行 `.exe`。

## Testing Environment

### Desktop Mode

已透過 Unity Play Mode 與 Windows Build 執行測試，確認桌面模式的道具拖曳、放置、物件選取及刪除等功能正常。

### XR Mode

程式會在啟動時自動偵測 XR 裝置，若偵測到已啟用的 XR 裝置，便進入 XR 模式；否則進入 Desktop 模式。

由於目前沒有實體 VR 頭戴裝置，因此 XR 功能僅能透過 Unity XR Device Simulator 進行模擬測試。

已使用 XR Device Simulator 測試控制器射線、UI 互動、道具拖曳放置、物件選取及刪除等功能，相關操作過程已於 Play Mode Demo 影片中展示。

**Note:** XR 功能尚未經實體 VR 裝置測試。