# Module 与事件参考

> **交叉引用**：[C# UI 开发参考](../../develop-unity-ui/references/csharp-ui.md)（C# UI 对 Module/Event 数据的消费与生命周期）。仅标识规范关系，实际加载仍按根 `AGENT.md` 的 Skill 调用路由执行。

## 事实入口

- Module 基类：`Assets/Framework/Manager/Module/ModuleBase.cs`
- Module 管理：`Assets/Framework/Manager/Module/ModuleManager.cs`
- Module 注册表：`Assets/GameData/Scripts/Module/ModuleTypes.cs`
- 通用调用入口：`Assets/Framework/Base/GameBase.cs`、`GameBaseMono.cs`
- 事件实现与定义：`Assets/Framework/Manager/Event/EventManager.cs`、`Assets/GameData/Scripts/Define/EventDefine.cs`

## 新建 Module

1. 在 `Assets/GameData/Scripts/Module/` 创建 `<业务名>Module` 并继承 `ModuleBase`。
2. 将 `typeof(<业务名>Module)` 加入 `ModuleTypes.ModuleTypeArr`；未注册的 Module 不会被初始化，`GetModule<T>()` 只会返回错误和 `null`。
3. 新建存档需要的初始化放在 `OnNew()`，读取存档后的恢复处理放在 `OnLoad()`；两者只会按 `NewAllModule()` 或 `LoadAllModule()` 的当前入口执行其中之一。
4. 新建与读档都需要的事件监听沿用现有 Module 的公共初始化位置，并通过继承入口 `AddEventListener` 注册，使 `ModuleBase` 能统一跟踪和清理。
5. `OnBeforeDispose()` 只清理 Module 自己持有的额外资源、静态状态或持久化数据；没有清理内容时保持为空。

`ModuleManager.NewAllModule()` 会实例化注册表中的全部 Module 并调用 `OnNew()`；业务代码通过继承自 `GameBase` 或 `GameBaseMono` 的 `GetModule<T>()` 获取已初始化实例，不自行 `new Module`。

## Module 职责

- Module 负责业务操作入口、需要跨界面使用的数据缓存或存档，以及通过事件通知消费者；涉及 HTTP 或 WebSocket 时同时读取网络 reference。
- 有现成 Module 时优先扩展；没有 Module 时先追踪当前调用链，确认数据确实需要共享或跨生命周期保存后再新建。
- 数据先保存到 Module，再 `SendEvent(...)`；Event 只通知变化并携带本次更新需要的参数，不代替 Module 保存长期数据。
- UI 和其他消费者调用 Module 业务方法或读取缓存，不绕过 Module 重复维护同一份数据。
- 依赖实例缓存、存档或生命周期的数据操作使用 Module 实例，不为调用方便增加静态旁路。

## C# UI 数据流

- C# UI 能使用 Module 时必须使用 Module；UI 不直接发起 Module 已承担的 HTTP 请求，也不直接操作业务 WebSocket。
- 数据到达后，Module 按现有入口通过 `OpenUI<T>(..., data)` 打开界面，或先保存数据再 `SendEvent(...)` 通知已打开界面；UI 只消费打开参数、Event 参数或 Module 数据并刷新显示。
- UI 在 `OnAwake` 通过 `AddEventListener` 注册一次；数据入口优先沿用打开 UI 时传入的 `args`、Event 参数或 `GetModule<T>()` 中已保存的数据，只有该 UI 本身就是查询入口时，才在 `OnShow` 或点击处理中调用 Module 请求方法。
- 同一结果需要多个 UI 使用时，由 Module 更新一次数据并发送 Event，各 UI 独立监听；不要让 UI 互相调用，也不要为每个 UI 重复维护相同数据。
- 仅显示期间才应响应的 UI 回调按目标界面的实际激活状态处理，但 Module 仍应先保存数据，不能因 UI 隐藏而丢弃更新。
- 优先复用已有 Module、业务方法和 `EventDefine`；确实没有对应语义时才新增。

## 网络协作边界

- Module 继承 `GameBase`，可直接使用其 HTTP 与 WebSocket 保护方法；具体 API、回调分发和限制读取 [HTTP 与 WebSocket 参考](network.md)。
- `PlayerModule` 当前展示了 `HttpPost(...)` 与 `SocketConnect(...)` 的最小调用方式，只作为入口示例；新增业务仍应先确认数据所有者和回调后的保存或通知位置。
- WebSocket 未命中请求回调的消息会进入 `EventManager`；Module 需要消费时使用继承的 `AddEventListener`，由 `ModuleBase` 在释放时统一移除。
- Module 持有的额外请求状态或外部委托仍在 `OnBeforeDispose()` 主动清理，不能把 Event 的自动清理扩大解释为所有网络状态都会自动取消。

## 事件 API 与生命周期

| 用途 | API |
| --- | --- |
| 发送事件 | `SendEvent((uint)ModuleEvent.Xxx, params object[] args)` |
| 注册事件 | `AddEventListener((uint)ModuleEvent.Xxx, Action<object[]> callback)` |
| 移除事件 | `RemoveEventListener((uint)ModuleEvent.Xxx, callback)` |
| 获取 Module | `GetModule<T>()` |

- 新事件添加到 `EventDefine.cs` 的对应枚举并使用语义明确的名称；按根 `AGENT.md` 规定的搜索工具顺序，确认枚举名和值没有重复。
- ModuleBase 会记录通过其受保护入口注册的 `EventDefine` 监听，并在 `OnDispose()` 统一移除。
- `UIBase` 同样会在销毁时移除其事件监听；普通 `GameBase`、`GameBaseMono` 派生对象仍要在自己的实际销毁点成对移除。
- `ModuleBase` 和 `UIBase` 的自动清理按事件 Id 保存回调，同一对象对同一个事件 Id 只注册一个回调；`EventManager` 也会把重复监听视为错误。
- 回调参数是 `object[]`；发送方和监听方必须核对数量、顺序和类型，不通过默认值或空判断掩盖签名不一致。

## 验证

- 聚焦确认 Module 已加入 `ModuleTypes.ModuleTypeArr`，`OnNew` 与 `OnLoad` 职责没有混淆，事件发送方和监听方参数一致；搜索工具顺序遵守根 `AGENT.md`。
- 检查数据先保存再发事件，非自动清理对象具备对应移除入口。
- C# 修改运行 `dotnet build <项目解决方案>.sln`；涉及 HTTP 或 WebSocket 时，响应、断线和生命周期仍需在 Unity 中连接实际目标验证。
