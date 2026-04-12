# BFramework-Ex

<p align="center">
  <b>一个基于 Unity 的功能完整的热更新游戏框架扩展</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Unity-2021.3%2B-blue" />
  <img src="https://img.shields.io/badge/C%23-9.0-green" />
  <img src="https://img.shields.io/badge/HybridCLR-热更新-orange" />
  <img src="https://img.shields.io/badge/license-MIT-lightgrey" />
</p>

---

## 简介

`BFramework-Ex`（简称 `BFEX`）是基于 [BFramework](https://github.com/ToxicStar8/BFramework) 的扩展框架，在保留原框架轻量核心的基础上，集成了热更新、AB包管理、UI框架、红点系统、网络通信、定时器、状态机等一系列生产级模块，适用于中小型 Unity 项目的快速开发。

**核心特性：**
- 🔥 基于 [HybridCLR](https://github.com/focus-creative-games/hybridclr) 的原生 C# 热更新
- 📦 完整的 AssetBundle 打包与增量热更新流程
- 🎮 十余个开箱即用的 Manager 模块
- 🛠️ 丰富的 Editor 工具链，大幅提升开发效率
- 🧩 十几种通用 UI 组件与扩展方法

---

## 目录结构

```
Assets/
├── Editor/          # 编辑器工具（打包、图集、UI代码生成等）
├── Framework/       # 核心框架（Base、Manager、Component、Extensions）
├── GameData/        # 游戏数据（策划表、美术资源、预制体、脚本）
├── Hotfix/          # 热更新代码 DLL
├── HybridCLRData/   # HybridCLR 生成数据
├── MainPackage/     # 主包（入口、下载器、启动流程）
├── Plugin/          # 第三方插件
└── Scenes/          # 场景文件
```

---

## 热更流程

本框架采用 HybridCLR + AssetBundle 的方案，热更步骤如下：

1. **生成主包体依赖文件**：`HybridCLR/Generate/All`
2. **打包并生成 AotDll**：`File / Build Settings / Build`
3. **打图集**：`BFramework / Build SpriteAtlas`
4. **生成资源名称**：`BFramework / Build AssetName`
5. **编译热更 DLL**：`HybridCLR / CompileDll / ActiveBuildTarget`
6. **打 AB 包**：`BFramework / Build AssetBundles`
7. **上传 AssetBundle 文件夹至 CDN 服务器**
8. **发布包体**

> 编辑器模式下跳过下载流程，直接读取本地 `AssetBundle/` 目录。

---

## 模块说明

### MainPackage — 主包

| 类 | 说明 |
|---|---|
| `GameEntry` | 游戏总入口，负责启动流程控制 |
| `DowloadManager` | 原生实现 AB 包增量下载，支持 MD5 比对、断点重试（最多3次）、进度回调 |
| `InputFieldMobileSupport` | 移动端输入框适配支持组件 |

**下载状态机：**
- `0` 未开始 → `1` 检查更新 → `2` 下载中 → `3` 下载完毕

---

### Framework — 核心框架

#### Base — 基类

| 类 | 说明 |
|---|---|
| `GameBase` | 所有游戏对象的基类，封装了对所有 Manager 的便捷调用（事件、UI、音频、定时器、网络、状态机等） |
| `GameBaseMono` | MonoBehaviour 版本的游戏对象基类 |
| `ManagerBase` | 所有 Manager 的基类，规范 `OnUpdate` / `OnDispose` 生命周期 |
| `InstanceBase` | 单例基类 |

#### Manager — 管理器

| Manager | 功能描述 |
|---|---|
| **AudioManager** | 支持播放`背景音乐`（唯一）和多个`音效` |
| **EventManager** | 事件中心，支持添加/移除/发送事件监听；`SendEvent` 内置异常隔离，单个回调异常不影响其他监听器 |
| **FsmManager** | 有限状态机管理器，支持创建多个独立 FSM 实例 |
| **LoadManager** | 可寻址同步/异步加载资源与精灵，通过 `LoadHelper` 管理对象持有的资源引用，支持资源释放 |
| **ModuleManager** | 数据层管理器，网络游戏中作为请求器和数据缓存，单机游戏中作为存档管理 |
| **HttpManager** | 原生实现可自定义 Header 的 HTTP GET/POST 请求 |
| **SocketManager** | 基于 [UnityWebSocket](https://github.com/psygames/UnityWebSocket) 实现可自定义 Header 的 WebSocket 通信 |
| **PoolManager** | 提供`游戏对象池`和`类对象池`两种池化方案，降低 GC 压力 |
| **RedPointManager** | 使用 Dictionary 实现红点系统，支持按 Id 注册/移除回调、设置/获取红点数量 |
| **TableManager** | 基于 EPPlus 将 Excel 表格数据转换为运行时可用的配置表 |
| **TimerManager** | 基于 UniTask 的定时器，支持命名定时器（防重复注册）和一次性匿名定时器 |
| **UIManager** | 基于 UGUI 的 UI 框架，支持多层级管理、打开/隐藏/关闭 UI，内置 `UnitPool` 管理 UI 内部列表元素 |
| **TaskManager** | 基于 UniTask 实现的任务队列，支持异步任务按序逐步执行 |
| **ABManager** | AB 包管理器，负责加载和管理 AssetBundle 及其依赖 |

#### Component — 通用组件

| 组件 | 说明 |
|---|---|
| `ButtonEx` | 按钮扩展，支持多点触控防穿透 |
| `ImageEx` | Image 扩展 |
| `ImageNotAlpha` | 不响应透明区域点击的 Image |
| `ImageCustomClick` | 自定义点击区域的 Image |
| `CornerRadiusImage` | 圆角 Image |
| `CycleImage` | 循环滚动 Image |
| `LoopScrollRect` | 循环滚动列表 |
| `ContentSizeFitterEx` | ContentSizeFitter 扩展 |
| `TmpEx` | TextMeshPro 扩展 |
| `DOTweenSequence` | DOTween 序列组件化封装 |
| `UpdateMono` | 轻量级 Update 回调注册组件 |
| `UIBind` | UI 自动绑定组件（配合代码生成工具使用） |
| `IntBind` | 整数数据绑定组件 |
| `Collision2DMono` / `Trigger2DMono` | 2D 碰撞/触发事件委托组件 |
| `EmptyMono` | 空 MonoBehaviour，用于挂载事件 |

#### Extensions — 扩展方法

为以下类型提供扩展方法：

`Array` / `List<T>` / `Dictionary<K,V>` / `string` / `GameObject` / `Transform` / `RectTransform` / `Image` / `Text` / `Button` / `Component` / `Collider2D`

#### Tools — 工具类

| 工具 | 说明 |
|---|---|
| `LoadHelper` | 资源加载器，管理对象持有的资源生命周期，支持 `Recycle` 回收 |
| `AutoCleanup` | 自动清理器，统一管理事件/Update/定时器的注销，防止内存泄漏 |
| `TimeUtil` | 时间工具 |
| `Base64Util` | Base64 编解码工具 |
| `FFmpegUtil` | FFmpeg 调用工具 |

---

### Editor — 编辑器工具

通过菜单栏 `BFramework` 快速执行以下操作：

| 功能 | 说明 |
|---|---|
| **Build AssetName** | 一键生成所有资源的名称索引 |
| **Build SpriteAtlas** | 一键打包图集 |
| **Build AssetBundles** | 一键打包 AB 包（支持自定义 `ABConfig` 配置） |
| **ExcelTools** | Excel 策划表可视化编辑与导出工具窗口 |
| **可视化红点树** | 在 Inspector 面板实时查看红点树结构 |
| **UI 代码生成** | 一键生成 UI 绑定代码，减少手动查找节点 |
| **GameManager 监视窗口** | 独立 EditorWindow，运行时实时查看对象池、计时器等管理器状态 |

---

### GameData — 游戏数据

| 目录 | 说明 |
|---|---|
| `Art/` | 美术资源 |
| `Prefabs/` | 预制体 |
| `Scripts/` | 业务逻辑代码 |
| `Table/` | Excel 策划表 |

---

## 依赖

| 依赖 | 用途 |
|---|---|
| [HybridCLR](https://github.com/focus-creative-games/hybridclr) | C# 热更新运行时 |
| [UniTask](https://github.com/Cysharp/UniTask) | 异步任务（定时器、任务队列） |
| [UnityWebSocket](https://github.com/psygames/UnityWebSocket) | WebSocket 通信 |
| [DOTween](http://dotween.demigiant.com/) | 动画缓动 |
| [EPPlus](https://www.epplussoftware.com/) | Excel 表格解析 |
| [Newtonsoft.Json](https://www.newtonsoft.com/json) | JSON 序列化 |
| TextMesh Pro | 高质量文本渲染 |

---

## 快速开始

1. **克隆仓库**并在 Unity 2021.3 LTS（或更高版本）中打开项目
2. 安装 HybridCLR：执行 `HybridCLR / Installer`
3. 在 `GameEntry` 挂载的 GameObject 上配置 `IsEditorMode = true` 以跳过热更下载，直接进行本地开发；若需在编辑器内使用 AB 包运行，额外勾选 `IsRunABPackage = true`
4. 继承 `GameBase` 或 `GameBaseMono` 编写业务逻辑，通过内置方法调用各 Manager
5. 参考 `Editor/热更说明.txt` 完成完整的热更打包流程

---

## 相关链接

- [BFramework（基础版）](https://github.com/ToxicStar8/BFramework)
- [HybridCLR 官方文档](https://www.hybridclr.cn/docs/intro)
- [UniTask](https://github.com/Cysharp/UniTask)
- [UnityWebSocket](https://github.com/psygames/UnityWebSocket)

---

## License

MIT License
