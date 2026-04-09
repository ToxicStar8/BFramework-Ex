# BFramework-Ex 主规范

> **项目**：BFramework-Ex — 基于 Unity 的轻量级游戏框架  
> **仓库**：https://github.com/ToxicStar8/BFramework-Ex  
> **分支**：main  
> **维护时间**：2022/12 起

---

## 1. 项目概览

BFramework-Ex 是一个面向 Unity（C# 9.0 / .NET Framework 4.7.1）的轻量级游戏框架，支持 **HybridCLR 热更新**。框架采用「总控制器 + 多 Manager」的单例驱动架构，所有管理器通过 `GameGod`（MonoBehaviour 单例）统一创建、驱动和销毁。

### 1.1 技术栈

| 类别 | 选择 |
|------|------|
| 引擎 | Unity（版本不固定，C# 9.0） |
| 目标框架 | .NET Framework 4.7.1 |
| 热更新 | HybridCLR（DLL 名：`Assembly-CSharp.dll.bytes`） |
| 异步 | UniTask（`Cysharp.Threading.Tasks`）
| 序列化 | Newtonsoft.Json |
| 动画 | DOTween / DOTween Pro |
| 网络 | UnityWebRequest (HTTP) + System.Net.WebSockets (WebSocket) |
| 包管理 | AssetBundle 自建管理 |
| UI 绑定 | 子对象命名约定（`TypePrefix_FieldName`）+ Editor 代码生成（`UIBind` 组件已弃用注释） |

### 1.2 程序集划分

| 程序集 | 用途 |
|--------|------|
| `Assembly-CSharp` | 框架核心（`Framework`）+ 游戏逻辑（`GameData`）+ 热更入口（`Hotfix`），**可热更** |
| `Assembly-CSharp-Editor` | 编辑器工具（代码生成、AB 打包、Excel 工具等） |
| `Main` (`MainPackage`) | 启动包 — `GameEntry`、下载器、日志，**不可热更** |
| `HybridCLR.*` | HybridCLR 运行时与编辑器 |
| `DOTween*` / `UnityWebSocket*` | 第三方插件 |

---

## 2. 目录结构

```
Assets/
├── Framework/                  # 框架核心（namespace: Framework）
│   ├── Base/                   # 基类
│   │   ├── GameBase.cs         # 非 Mono 对象基类（事件/池/表/UI/网络快捷方法）
│   │   ├── GameBaseMono.cs     # Mono 对象基类（同步 GameBase 方法）
│   │   ├── InstanceBase<T>.cs  # 泛型单例基类（继承 GameBase）
│   │   └── ManagerBase.cs      # 管理器抽象基类（OnAwake / OnUpdate / OnDispose）
│   ├── Manager/                # 各管理器
│   │   ├── Audio/              # AudioManager — 背景音乐 + 音效
│   │   ├── Event/              # EventManager — uint 键事件系统
│   │   ├── FSM/                # FsmManager + Fsm<T> + FsmState<T>
│   │   ├── Load/               # LoadManager（资源加载）+ ABManager（AB 包索引）
│   │   ├── Module/             # ModuleManager + ModuleBase（数据/存档层）
│   │   ├── Net/                # HttpManager + SocketManager
│   │   ├── Pool/               # PoolManager（ClassObjectPool<T> + GameObjectPool）
│   │   ├── RedPointSpecial/    # RedPointManager — 字典式红点（当前启用）
│   │   ├── RedPointTree/       # RedPointManager — 前缀树式红点（已注释，备选）
│   │   ├── Table/              # TableManager + TableCtrlBase<T> + TableBase
│   │   ├── Task/               # TaskManager — UniTask 串行任务队列
│   │   ├── Timer/              # TimerManager + TimerInfo — 异步定时器
│   │   └── UI/                 # UIManager + UIBase + UnitBase + UnitPool<T>
│   ├── Component/              # 自定义 UI 组件（ButtonEx, LoopScrollRect, UIBind 等）
│   ├── Define/                 # 框架常量（服务器地址、AB 路径等）
│   ├── Extensions/             # 扩展方法（Transform, GameObject, Button, Image 等）
│   └── Tools/                  # 工具类（LoadHelper, AutoCleanup, TimeUtil 等）
├── GameData/                   # 游戏业务逻辑（namespace: GameData）
│   ├── Scripts/
│   │   ├── Define/             # 游戏枚举、常量、事件定义、图集/资源名
│   │   ├── Extensions/         # 游戏层扩展方法
│   │   ├── Fsm/                # 具体状态机实现
│   │   ├── Module/             # 具体 Module 实现（PlayerModule 等）+ ModuleTypes 注册
│   │   ├── RedPoint/           # 具体红点实现
│   │   ├── Table/              # 具体配表控制器 + TableTypes 注册
│   │   └── UI/                 # 具体 UI 实现（Design 自动生成 + 逻辑手写）
├── Editor/                     # 编辑器工具（namespace: Framework）
│   ├── Build/                  # AB 打包、音频打包、图集打包、AssetName 生成
│   ├── Component/              # 编辑器窗口（GameManager 监视、RedPoint 树、UIBind）
│   ├── UI/                     # UI 代码生成器（GenerateUIScript）
│   ├── ABConfig/               # AB 包配置
│   └── ExcelTools/             # Excel → 配表工具
├── Hotfix/
│   └── HotUpdateMain.cs       # 热更入口（初始化 GameGod → 注册表格/Module → 启动）
├── MainPackage/                # 启动包（namespace: MainPackage）
│   ├── GameEntry.cs            # 游戏入口 MonoBehaviour（UI Root、下载管理、热更加载）
│   ├── Dowload/                # AB 包下载 + MD5 校验
│   ├── Tools/                  # MD5、日志、跳过 Logo
│   └── Component/              # 输入框适配
└── HybridCLRData/              # HybridCLR 生成数据
```

---

## 3. 核心架构

### 3.1 启动流程

```
GameEntry.Awake()         → 初始化入口单例、UI Root、DontDestroyOnLoad
GameEntry.Start()         → 创建 DowloadManager → 下载 AB 包
                          → 热更完成后加载 HotUpdateMain
HotUpdateMain.Start()     → AddComponent<GameGod>()
GameGod.Awake()           → 创建全部 14 个 Manager + LoadHelper
HotUpdateMain             → 注册 TableTypes / ModuleTypes → 打开首个 UI → 销毁自身
```

### 3.2 GameGod — 总控制器

`GameGod : MonoBehaviour`（`Assets/Framework/GameGod.cs`）

- **唯一单例**：`GameGod.Instance`，`DontDestroyOnLoad`
- **拥有 14 个管理器**：ABManager, LoadManager, PoolManager, HttpManager, SocketManager, UIManager, ModuleManager, EventManager, TableManager, AudioManager, TimerManager, FsmManager, RedPointManager, TaskManager
- **Update 驱动**：按顺序调用每个 Manager 的 `OnUpdate()`，最后触发 `UpdateCallback` 事件
- **销毁顺序**：`OnApplicationQuit` 触发 `DisposeCallback`，`OnDestroy` 按序调用各 Manager 的 `OnDispose()`
- **全局 LoadHelper**：`GameGod.Instance.LoadHelper`，基本不释放

### 3.3 ManagerBase — 管理器基类

```csharp
public abstract class ManagerBase
{
    public abstract void OnAwake();   // 构造时自动调用
    public abstract void OnUpdate();  // GameGod.Update 中调用
    public abstract void OnDispose(); // GameGod.OnDestroy 中调用
}
```

所有 Manager 继承此类。构造函数中自动 Log 并调用 `OnAwake()`。

### 3.4 GameBase / GameBaseMono — 对象基类

| 基类 | 继承 | 用于 |
|------|------|------|
| `GameBase` | `object` | ModuleBase, TableBase, InstanceBase 等非 Mono 对象 |
| `GameBaseMono` | `MonoBehaviour` | UIBase, UnitBase 等 Mono 对象 |

两者提供完全相同的快捷方法：
- **事件**：`SendEvent`, `AddEventListener`, `RemoveEventListener`
- **对象池**：`CreateClassPool<T>`, `CreateClassObj<T>`, `RecycleClassObj<T>`, `CreateGameObjectPool`, `CreateGameObject`, `RecycleGameObject`
- **配表**：`GetTableCtrl<T>`
- **图片**：`GetSpriteSync`, `GetSpriteAsync`
- **UI**：`OpenUI<T>`, `GetUI<T>`, `HideUI<T>`, `CloseUI<T>`
- **网络**：`HttpGet`, `HttpPost`, `SocketConnect`, `SocketSend`
- **定时器**：`AddTimer`, `AddTempTimer`, `RemoveTimer`
- **日志**：`Log`

### 3.5 AutoCleanup — 自动清理器

`AutoCleanup`（`Assets/Framework/Tools/AutoCleanup.cs`）

UIBase 和 ModuleBase 各自持有一个 `AutoCleanup` 实例，用于跟踪：
- **事件监听**：`TrackEvent(eventNo, callback)`
- **Update 回调**：`RegisterUpdate(ownerName, callback)`（注册到 `GameGod.UpdateCallback`）
- **定时器**：`TrackTimer(timeName)`

在 `Dispose()` 时统一移除所有跟踪的注册，防止泄漏。

---

## 4. 管理器详解

### 4.1 UIManager — UI 管理器

**文件**：`Assets/Framework/Manager/UI/UIManager.cs`

- 维护 `Dictionary<string, UIBase> _uiBaseDic`
- `OpenUI<T>(uiLevel, args)`：缓存命中则 Show，否则同步加载 prefab、挂组件、调 OnAwake + OnShow
- `HideUI` / `CloseUI` / `CloseAll`
- UI 层级通过 `E_UILevel` 枚举 → `GameEntry.GetUILevelTrans()` 获取对应 RectTransform

#### UIBase — UI 基类

**文件**：`Assets/Framework/Manager/UI/UIBase.cs`，继承 `GameBaseMono`

- 生命周期：`OnAwake()` → `OnShow(args)` → `OnBeforeDestroy()` → `OnDispose()`
- 拥有独立 `LoadHelper`（关闭时回收）
- 拥有 `AutoCleanup` — 重写的 `AddEventListener`、`AddTimer` 自动 Track
- `CloseSelf()` / `HideSelf()` 快捷关闭
- `RegisterUpdate(callback)` 注册帧回调

#### UnitBase — 子组件基类

**文件**：`Assets/Framework/Manager/UI/UnitBase.cs`，继承 `GameBaseMono`

- 用于 UI 中可复用的子控件
- 生命周期：`OnAwake()` → `OnRecycle()` → `OnBeforeDestroy()`

#### UnitPool\<T\> — Unit 对象池

**文件**：`Assets/Framework/Manager/UI/UnitPool.cs`

- UI 内部的 Unit 对象池，基于模板实例化 + 回收队列
- `CreateUnit(parent)` / `RecycleUnit(unit)` / `RecycleAll()`
- 为 `LoopScrollRect` 提供 `CreateUnitToRect()` 适配

#### UI 代码生成

**文件**：`Assets/Editor/UI/GenerateUIScript.cs`

- 右键 GameObject → "生成UI代码"
- 自动生成 `partial class` 的 `.Design.cs`（声明绑定字段）+ 逻辑 `.cs`（如果不存在）
- 基于子对象**命名约定**自动识别绑定：子对象名格式为 `TypePrefix_FieldName`（如 `Btn_Submit`、`Txt_Title`、`Img_Avatar`），前缀映射到对应组件类型（`ButtonEx`、`TmpEx`、`Image` 等）；数组字段格式为 `TypePrefix_FieldName_Index`（如 `Btn_Items_0`）
- `UIBind` 组件（`Assets/Framework/Component/UIBind.cs`）及其 Inspector 扩展均已**全部注释弃用**
- 支持同时生成关联的 Unit 代码

**UI 文件命名约定**：
- `UIXxx.cs` — 手写逻辑（partial class）
- `UIXxx.Design.cs` — 自动生成的字段绑定（partial class）
- `Unit_Xxx.cs` / `Unit_Xxx.Design.cs` — 同上，用于 Unit

### 4.2 ModuleManager — 数据/存档管理器

**文件**：`Assets/Framework/Manager/Module/ModuleManager.cs`

- 通过 `InitModuleType(Type[])` 注册所有 Module 类型（在 `ModuleTypes.cs` 中定义）
- `NewAllModule()` / `LoadAllModule()` — 新建或从存档加载
- `GetModule<T>()` — 获取指定 Module
- 支持 JSON 序列化存档到 `Application.persistentDataPath`

#### ModuleBase — 数据操作基类

**文件**：`Assets/Framework/Manager/Module/ModuleBase.cs`，继承 `GameBase`

- 生命周期：构造 → `OnNew()`（新建时）或 `OnLoad()`（加载时） → `OnBeforeDispose()` → `OnDispose()`
- 拥有 `AutoCleanup` — 自动跟踪事件和 Update
- 内部通过 `RegisterUpdate(callback)` 注册帧回调

### 4.3 TableManager — 配表管理器

**文件**：`Assets/Framework/Manager/Table/TableManager.cs`

- 通过 `Init(Type[])` 注册所有配表控制器类型（在 `TableTypes.cs` 中定义，自动生成）
- `GetTableCtrl<T>()` — 懒加载，首次访问时读取数据
- `HotReload()` — 热重载已加载的表格

#### 配表体系

| 类 | 职责 |
|----|------|
| `ITableCtrlBase` | 接口，解决泛型字典值类型限制 |
| `TableCtrlBase<T>` | 控制器基类，持有 `List<T>` + `Dictionary<int, T>`，负责解析文本 |
| `TableBase` | 数据行基类，`OnAwake(nameGroupArr, dataStr)` 解析单行 |

**数据格式**：TextAsset，行分隔符 `` ` ``，列分隔符 `^`，第一行为变量名。

### 4.4 EventManager — 事件管理器

**文件**：`Assets/Framework/Manager/Event/EventManager.cs`

- `Dictionary<uint, List<Action<object[]>>>` — uint 键 + 可变参数回调
- `AddEventListener(eventNo, callback)` / `RemoveEventListener` / `SendEvent`
- 事件编号在 `GameData.EventDefine.cs` 中以枚举定义，每 100 一个模块区间

### 4.5 PoolManager — 对象池管理器

**文件**：`Assets/Framework/Manager/Pool/PoolManager.cs`

#### ClassObjectPool\<T\>
- `Queue<T>` 回收队列 + `LinkedList<T>` 已用跟踪
- `CreateClassObj()` / `Recycle(obj)`

#### GameObjectPool
- `Queue<GameObject>` 回收队列 + `LinkedList<GameObject>` 已用跟踪
- `CreateObj(trans)` — 池空则 `LoadManager.LoadSync` + `Instantiate`，否则 Dequeue
- `Recycle(go)` — 挂到 `GameEntry.ObjPool` 节点下
- `OnDispose()` — 销毁所有并卸载 AB 资源

### 4.6 LoadManager — 资源加载管理器

**文件**：`Assets/Framework/Manager/Load/LoadManager.cs`

- **编辑器模式**：扫描 `Assets/GameData/` 建立 `文件名→路径` 字典，使用 `AssetDatabase` 加载
- **AB 包模式**：通过 `ABManager.ABInfo` 查找 AB 包名 → 加载依赖 → 加载资源
- `LoadSync<T>(objName)` / `LoadAsync<T>(objName)`（当前 Async 实为 Sync 包装）
- `GetSprite(atlasName, spriteName)` — SpriteAtlas 加载
- `UnloadAsset(objName)` — 引用计数卸载
- 资源名全部**带后缀**（如 `"UIMainMenu.prefab"`, `"RetroComedy.ogg"`）

#### LoadHelper — 加载助手

**文件**：`Assets/Framework/Tools/LoadHelper.cs`

- 每个 UIBase / UnitBase 持有独立 LoadHelper
- 包装 `LoadManager` 的加载方法 + 记录已加载资源名（`HashSet<string>`）
- `Create()` / `Recycle()` 基于 ClassObjectPool
- `UnloadAll()` — 卸载所有记录的资源
- 提供 `CreateGameObjectSync` / `CreateGameObjectAsync` 等便捷方法

### 4.7 ABManager — AB 包管理器

**文件**：`Assets/Framework/Manager/Load/ABManager.cs`

- 加载 AB 包索引文件（`abInformation`），反序列化为 `ABInfo`
- 维护 `ABRelyInfoDic` — AB 包依赖关系缓存
- 仅在非编辑器模式或 AB 包运行模式下工作

### 4.8 TimerManager — 定时器管理器

**文件**：`Assets/Framework/Manager/Timer/TimerManager.cs`

- `AddTimer(name, timerInfo)` / `AddTempTimer(timerInfo)` / `RemoveTimer(name)`
- `TimerInfo` 支持：执行次数、间隔（毫秒）、立即执行、执行回调、结束回调
- 基于 `CancellationTokenSource` + UniTask 实现异步等待
- `TimerInfo` 通过 ClassObjectPool 池化

### 4.9 FsmManager — 状态机管理器

**文件**：`Assets/Framework/Manager/FSM/FsmManager.cs`

- `CreateFsm<T>(owner, states)` → `Fsm<T>` — 泛型状态机，拥有者 + 状态数组
- `Fsm<T>` 维护 `Dictionary<sbyte, FsmState<T>>`，`ChangeState(sbyte)` 切换
- `FsmState<T>` 抽象类：`OnAwake`, `OnEnter`, `OnUpdate`, `OnLeave`, `OnDestroy`
- FsmManager.OnUpdate 驱动所有活跃状态机

### 4.10 AudioManager — 声音管理器

**文件**：`Assets/Framework/Manager/Audio/AudioManager.cs`

- 双 AudioSource：背景音乐（循环）+ 音效（OneShot）
- 音量持久化到 `PlayerPrefs`
- `PlayBackground(audioName)` / `PlaySound(audioName)`

### 4.11 HttpManager — HTTP 管理器

**文件**：`Assets/Framework/Manager/Net/HttpManager.cs`

- 基于 `HttpRoutine`（ClassObjectPool 池化）
- 支持 `Get` / `Post` / `GetTexture` / `GetAudioClip`
- 支持自定义 Header（Token 等）
- 可配重试次数和间隔

### 4.12 SocketManager — WebSocket 管理器

**文件**：`Assets/Framework/Manager/Net/SocketManager.cs`

- 基于 `SocketRoutine`（封装 `ClientWebSocket`）
- `Connect(wsUrl, openCallback, closeCallback)` / `SendMsg(msg, callback)` / `CloseSocket()`
- 支持 string 和 byte[] 消息
- 回调使用 `JsonData` 类型

### 4.13 RedPointManager — 红点管理器

**文件**：`Assets/Framework/Manager/RedPointSpecial/RedPointManager.cs`（当前启用）

- 字典式：`Dictionary<int, int>` 记录红点类型→数量
- `AddRedCallBack(id, callback)` / `RemoveRedCallBack` / `SetRedPointCount`
- 回调签名：`Action<int, string, int>`（id, key, count）

> 备选方案（已注释）：`Assets/Framework/Manager/RedPointTree/RedPointManager.cs` — 前缀树实现

### 4.14 TaskManager — 任务队列管理器

**文件**：`Assets/Framework/Manager/Task/TaskManager.cs`

- 串行任务队列（`Queue<TaskInfo>`）
- `AddTask(Action<TaskInfo>)` — 入队并尝试执行
- `TaskInfo.OnComplete()` 标记完成后自动执行下一个
- 基于 `UniTaskCompletionSource` 支持 `await WaitComplete()`

---

## 5. 编辑器工具

| 工具 | 文件 | 功能 |
|------|------|------|
| UI 代码生成 | `Editor/UI/GenerateUIScript.cs` | 右键 → 生成 UI/Unit 代码（partial class） |
| AB 打包 | `Editor/Build/BuildAssetBundles.cs` | 打 AB 包 |
| 音频打包 | `Editor/Build/BuildAudio.cs` | 音频资源打包 |
| 图集打包 | `Editor/Build/BuildSpriteAtlas.cs` | 精灵图集打包 |
| AssetName 生成 | `Editor/Build/BuildAssetName.cs` | 生成资源名常量 |
| Excel 工具 | `Editor/ExcelTools/ExcelToolsWindow.cs` | Excel → 配表数据 |
| AB 配置 | `Editor/ABConfig/ABConfig.cs` | AB 包配置 ScriptableObject |
| GameManager 监视 | `Editor/Component/GameManagerEditorWindow.cs` | 运行时对象池/管理器监视面板 |
| RedPoint 树 | `Editor/Component/RedPointTreeWindow.cs` | 红点树可视化（配合前缀树方案） |
| ~~UIBind 面板~~ | `Editor/Component/UIBindInspectorEx.cs` | UIBind 组件 Inspector 扩展（**已全部注释弃用**） |

---

## 6. 命名约定与编码规范

### 6.1 命名规则

| 元素 | 规则 | 示例 |
|------|------|------|
| 命名空间 | PascalCase，框架/游戏/启动包分离 | `Framework`, `GameData`, `MainPackage` |
| 类 | PascalCase | `UIManager`, `PlayerModule` |
| 接口 | I 前缀 + PascalCase | `ITableCtrlBase` |
| 公开属性 | PascalCase | `LoadHelper`, `Instance` |
| 私有字段 | `_` 前缀 + camelCase | `_uiBaseDic`, `_allModuleDic` |
| 方法 | PascalCase | `OpenUI`, `OnAwake` |
| 枚举 | `E_` 前缀 + PascalCase | `E_UILevel`, `E_Log` |
| 事件枚举 | 模块名 + Event | `GameEvent`, `UIEvent`, `ModuleEvent` |
| 常量 | PascalCase | `ServerHttp`, `ABInfoName` |
| 扩展方法类 | `Ex_` 前缀 + 目标类型 | `Ex_Transform`, `Ex_Button` |
| 自定义组件 | 类型 + `Ex` 后缀 | `ButtonEx`, `TmpEx`, `ImageEx` |
| UI 文件 | `UI` 前缀 | `UIMainMenu.cs`, `UITips.cs` |
| Unit 文件 | `Unit_` 前缀 | `Unit_Tips.cs`, `Unit_AddItemTips.cs` |
| Design 文件 | `.Design.cs` 后缀（自动生成） | `UIMainMenu.Design.cs` |

### 6.2 文件头注释

每个文件顶部需包含标准注释块：
```csharp
/*********************************************
 * BFramework
 * 描述
 * 创建时间：YYYY/MM/DD HH:mm:ss
 *********************************************/
```

### 6.3 编码风格

- 使用 `var` 进行类型推断
- 使用 `?.` 空条件运算符
- 使用 `??=` 空合并赋值
- 使用 `new()` 目标类型推断的 new
- 使用 `=>` 表达式体成员（简单属性/方法）
- 属性 getter/setter 使用 `{ private set; get; }` 顺序
- for 循环中缓存长度：`for (int i = 0, length = arr.Length; i < length; i++)`
- 使用 `#region` 分组代码块
- 日志统一通过 `GameGod.Instance.Log(E_Log, title, content)` 或基类 `Log()` 方法

---

## 7. 关键设计模式

### 7.1 Manager 模式
所有系统功能封装为 Manager（继承 `ManagerBase`），由 `GameGod` 统一持有和驱动。Manager 不使用 MonoBehaviour，生命周期完全由 GameGod 控制。

### 7.2 双基类体系
`GameBase`（非 Mono）和 `GameBaseMono`（Mono）提供一致的快捷方法接口，业务代码根据是否需要 MonoBehaviour 选择继承。

### 7.3 AutoCleanup 自动清理
UIBase 和 ModuleBase 通过 `AutoCleanup` 跟踪注册的事件、Update 回调、定时器，在 Dispose 时统一移除，避免手动管理遗漏导致的泄漏。

### 7.4 LoadHelper 资源追踪
每个 UI/Unit 持有独立的 `LoadHelper`，记录所有加载过的资源名，关闭时批量卸载，实现按 UI 粒度的资源生命周期管理。

### 7.5 类型注册表
`TableTypes.cs` 和 `ModuleTypes.cs` 以 `Type[]` 数组集中注册所有配表控制器和 Module 类型，供管理器在运行时通过反射创建实例。

### 7.6 Partial Class + 代码生成
UI 采用 `partial class` 拆分：`.Design.cs`（编辑器自动生成组件绑定字段）+ `.cs`（手写业务逻辑），避免手动绑定和代码覆盖。

---

## 8. 新增功能指南

### 8.1 添加新 UI

1. 在 Unity 中创建 Prefab，挂 `UIBind` 标记需要绑定的组件
2. 右键 → "生成UI代码" → 自动生成 `UIXxx.Design.cs` + `UIXxx.cs`
3. 在 `UIXxx.cs` 中实现 `OnAwake()`、`OnShow()`、`OnBeforeDestroy()`
4. 通过 `OpenUI<UIXxx>(E_UILevel.Common)` 打开

### 8.2 添加新 Module

1. 创建类继承 `ModuleBase`，实现 `OnNew()`、`OnLoad()`、`OnBeforeDispose()`
2. 在 `ModuleTypes.cs` 的 `ModuleTypeArr` 中注册类型
3. 通过 `GameGod.Instance.ModuleManager.GetModule<XxxModule>()` 访问

### 8.3 添加新配表

1. 使用 Excel 工具生成数据文件
2. 创建 `TableXxx : TableBase` 和 `TableXxxCtrl : TableCtrlBase<TableXxx>`
3. 在 `TableTypes.cs` 的 `TableCtrlTypeArr` 中注册类型
4. 通过 `GetTableCtrl<TableXxxCtrl>()` 访问

### 8.4 添加新 Manager

1. 创建类继承 `ManagerBase`，实现 `OnAwake()`、`OnUpdate()`、`OnDispose()`
2. 在 `GameGod.cs` 中添加属性声明 + Awake 中 new + Update 中 OnUpdate + OnDestroy 中 OnDispose

### 8.5 添加新事件

1. 在 `EventDefine.cs` 对应枚举中添加事件编号
2. 发送方：`SendEvent((uint)GameEvent.XxxEvent, args)`
3. 接收方：`AddEventListener((uint)GameEvent.XxxEvent, callback)`（UIBase/ModuleBase 中自动跟踪清理）

---

## 9. 关键文件索引

> 快速定位常用文件

| 用途 | 路径 |
|------|------|
| 总控制器 | `Assets/Framework/GameGod.cs` |
| 游戏入口 | `Assets/MainPackage/GameEntry.cs` |
| 热更入口 | `Assets/Hotfix/HotUpdateMain.cs` |
| Manager 基类 | `Assets/Framework/Base/ManagerBase.cs` |
| 对象基类（非Mono） | `Assets/Framework/Base/GameBase.cs` |
| 对象基类（Mono） | `Assets/Framework/Base/GameBaseMono.cs` |
| 单例基类 | `Assets/Framework/Base/InstanceBase.cs` |
| UI 管理器 | `Assets/Framework/Manager/UI/UIManager.cs` |
| UI 基类 | `Assets/Framework/Manager/UI/UIBase.cs` |
| Unit 基类 | `Assets/Framework/Manager/UI/UnitBase.cs` |
| Unit 对象池 | `Assets/Framework/Manager/UI/UnitPool.cs` |
| Module 管理器 | `Assets/Framework/Manager/Module/ModuleManager.cs` |
| Module 基类 | `Assets/Framework/Manager/Module/ModuleBase.cs` |
| 配表管理器 | `Assets/Framework/Manager/Table/TableManager.cs` |
| 配表控制器基类 | `Assets/Framework/Manager/Table/TableCtrlBase.cs` |
| 事件管理器 | `Assets/Framework/Manager/Event/EventManager.cs` |
| 对象池管理器 | `Assets/Framework/Manager/Pool/PoolManager.cs` |
| 资源加载管理器 | `Assets/Framework/Manager/Load/LoadManager.cs` |
| AB 包管理器 | `Assets/Framework/Manager/Load/ABManager.cs` |
| 加载助手 | `Assets/Framework/Tools/LoadHelper.cs` |
| 自动清理器 | `Assets/Framework/Tools/AutoCleanup.cs` |
| 定时器管理器 | `Assets/Framework/Manager/Timer/TimerManager.cs` |
| 状态机管理器 | `Assets/Framework/Manager/FSM/FsmManager.cs` |
| 声音管理器 | `Assets/Framework/Manager/Audio/AudioManager.cs` |
| HTTP 管理器 | `Assets/Framework/Manager/Net/HttpManager.cs` |
| Socket 管理器 | `Assets/Framework/Manager/Net/SocketManager.cs` |
| 红点管理器 | `Assets/Framework/Manager/RedPointSpecial/RedPointManager.cs` |
| 任务队列管理器 | `Assets/Framework/Manager/Task/TaskManager.cs` |
| 框架常量 | `Assets/Framework/Define/ConstDefine.cs` |
| 游戏常量 | `Assets/GameData/Scripts/Define/ConstDefine.cs` |
| 事件定义 | `Assets/GameData/Scripts/Define/EventDefine.cs` |
| 枚举定义 | `Assets/GameData/Scripts/Define/EnumDefine.cs` |
| Module 类型注册 | `Assets/GameData/Scripts/Module/ModuleTypes.cs` |
| 配表类型注册 | `Assets/GameData/Scripts/Table/TableTypes.cs` |
| UI 代码生成 | `Assets/Editor/UI/GenerateUIScript.cs` |
| GameManager 监视 | `Assets/Editor/Component/GameManagerEditorWindow.cs` |
