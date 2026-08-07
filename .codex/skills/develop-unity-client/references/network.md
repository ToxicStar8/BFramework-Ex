# HTTP 与 WebSocket 参考

## 适用范围

用于发起或排查 HTTP 请求、建立 WebSocket 连接、发送与接收 JSON 消息，以及确认回调、事件分发和生命周期边界。

## 事实入口

- HTTP 管理与请求执行：`Assets/Framework/Manager/Net/HttpManager.cs`、`HttpRoutine.cs`
- WebSocket 管理与消息分发：`Assets/Framework/Manager/Net/SocketManager.cs`、`SocketRoutine.cs`
- 通用调用入口：`Assets/Framework/Base/GameBase.cs`、`GameBaseMono.cs`
- 管理器创建、更新与释放：`Assets/Framework/GameGod.cs`
- 现有业务调用示例：`Assets/GameData/Scripts/Module/PlayerModule.cs`
- WebSocket 底层实现：`Assets/Plugin/UnityWebSocket/`

继承 `GameBase` 或 `GameBaseMono` 的业务优先使用受保护的 `Http*`、`Socket*` 方法；未继承基类的框架位置才通过 `GameGod.Instance.HttpManager` 或 `GameGod.Instance.SocketManager` 访问，不再封装旁路管理器。

## HTTP API

| 用途 | 继承入口 | Manager 入口 |
| --- | --- | --- |
| GET 文本 | `HttpGet(url, success, error)` | `HttpManager.Get(...)` |
| GET 图片 | `HttpGetTexture(url, success, error)` | `HttpManager.GetTexture(...)` |
| GET 音频 | `GetAudioClip(url, audioType, success, error)` | `HttpManager.GetAudioClip(...)` |
| POST JSON | `HttpPost(url, json, success, error)` | `HttpManager.Post(...)` |
| 清空请求头 | `HttpClearHeader()` | `HttpManager.ClearHeader()` |
| 添加请求头 | `HttpAddHeader(key, value)` | `HttpManager.AddHeader(...)` |

## HTTP 行为与边界

- `HttpManager` 从类对象池获取 `HttpRoutine`；每个 Routine 同时只执行一个请求，请求完成后自动清理回调、重置状态并回池。调用方不得保存 Routine 后复用。
- GET 文本、图片和音频在 URL 为空白时记录错误并返回 `null`；POST 当前没有同样的前置检查，而且 `json` 最终会传给 `Encoding.UTF8.GetBytes`，调用方必须提供非空 URL 和非 `null` JSON 字符串。
- POST 使用 `application/json` 和 `UploadHandlerRaw`；请求字段、认证和响应结构由业务约定决定，框架不做 DTO、状态码或业务成功判断。
- `HttpHeaderDic` 应在请求前通过 `ClearHeader`、`AddHeader` 管理；`AddHeader` 使用字典 `Add`，重复 key 会抛异常，需要替换时先清理或避免重复添加。
- 当前请求超时固定为 10 秒，`Retry` 与 `RetryInterval` 初始化为 0 且没有公开设置入口；不要在业务规则中假定请求会自动重试。
- 图片和音频按 URL 缓存在 Manager 字典中；缓存命中会同步执行成功回调并返回 `null`，因此不能仅凭返回值是否为空判断请求成功或失败。
- 文本、图片和音频成功回调都在 Unity 协程继续执行时触发。调用方应提供错误回调并在其中处理失败，不以空数据或默认成功代替错误结果。
- `HttpRoutine.GetUwr()` 只暴露当前 `UnityWebRequest`；框架没有封装取消、进度或统一响应模型，需要这些能力时先确认是否扩展现有 Manager。
- 图片和音频的重试分支当前未保留原请求类型；在未修正实现前不要启用或宣传这两类请求的自动重试。

## WebSocket API

| 用途 | 继承入口 | Manager 入口 |
| --- | --- | --- |
| 连接 | `SocketConnect(wsUrl, open, close)` | `SocketManager.Connect(...)` |
| 发送文本 JSON | `SocketSendMsg(json, callback)` | `SocketManager.SendMsg(...)` |
| 发送 UTF-8 JSON 字节 | `SocketSendMsg(bytes, callback)` | `SocketManager.SendMsg(...)` |
| 清空连接请求头 | `SocketClearHeader()` | `SocketManager.ClearHeader()` |
| 添加连接请求头 | `SocketAddHeader(key, value)` | `SocketManager.AddHeader(...)` |
| 关闭 | `SocketClose()` | `SocketManager.CloseSocket()` |

## WebSocket 消息与分发

- `SocketManager` 当前只持有一个主连接；再次 `Connect` 会先关闭旧连接，再创建新的 `SocketRoutine`。
- 请求头在连接前配置并传给 `ConnectAsync`；`AddHeader` 同样使用字典 `Add`，重复 key 会抛异常。
- 发送与接收都按 UTF-8 JSON 处理，顶层对象必须能反序列化为 `JsonData`，并包含 `uint proto`。发送 `byte[]` 只是先转成 UTF-8 字符串读取 `proto`，不是任意二进制协议入口。
- 每次发送会用 `proto` 保存一次性回调；收到相同 `proto` 后调用并移除。相同 `proto` 同时存在多个未完成请求时，后一次会覆盖前一次，业务必须避免并发复用同一 `proto`。
- 收到的消息没有对应请求回调时，`SocketRoutine` 调用 `EventManager.SendEvent(proto, jsonData)`；监听方接收的 `object[]` 中第一个参数是该 `JsonData`。
- WebSocket 插件事件先进入加锁队列，再由 `SocketManager.OnUpdate()` 在 Unity 主线程分发；UI 与 Unity 对象更新放在该分发后的回调中。
- 关闭事件会清空全部待处理回调并执行关闭回调；当前没有自动重连、心跳或请求超时机制，不得假定框架会自动恢复连接或补发请求。
- `SendMsg`、`CloseSocket` 和 `GetSocket` 没有未连接保护；调用方必须先完成 `Connect`，并在打开回调后发送。

## Module 与 UI 边界

- 业务请求、连接状态和跨界面数据优先由 Module 持有；`PlayerModule` 现有 `HttpPost` 与 `SocketConnect` 只展示入口，不定义通用业务协议。
- Module 使用继承的 `AddEventListener` 消费主动推送时，监听会由 `ModuleBase` 的 `AutoCleanup` 在释放时移除；请求回调、连接回调和其他外部状态仍由实际所有者负责结束。
- UI 只调用 Module 的业务方法并消费打开参数、Event 参数或 Module 数据，不直接维护 HTTP Routine、Socket 连接或 `proto` 回调字典。
- 新消息格式、鉴权、重连、心跳、超时或并发策略属于业务契约；当前框架没有统一实现时先向用户确认，不在单个 UI 中临时补一套。

## 生命周期

- `GameGod.Awake()` 创建 `HttpManager` 与 `SocketManager`；`ManagerBase` 构造函数会立即调用各自的 `OnAwake()`。
- `GameGod.Update()` 每帧调用两个 Manager 的 `OnUpdate()`；HTTP 当前为空，WebSocket 在这里派发队列事件。
- `GameGod.OnDestroy()` 调用两个 Manager 的 `OnDispose()`；HTTP 当前没有额外释放逻辑，Socket 会关闭主连接并清空引用。
- 业务对象提前结束时，不要等待全局 `GameGod` 销毁来处理自己的回调状态；按实际所有者主动关闭连接或停止后续业务更新。

## 交叉引用

| 相关规范 | 处理内容 |
| --- | --- |
| [Module 与事件参考](module-event.md) | Module 数据所有权、事件监听与自动清理 |
| [C# UI 开发参考](../../develop-unity-ui/references/csharp-ui.md) | UI 通过 Module 消费网络结果 |

## 验证

- 聚焦检查调用方是否使用正确的 `GameBase`/`GameBaseMono` 包装入口、HTTP 是否同时处理成功和错误回调、WebSocket JSON 是否包含有效 `proto`。
- 检查相同 `proto` 没有并发覆盖回调，主动推送的事件监听会在所有者释放时移除，关闭或重连时没有继续使用旧请求状态。
- C# 修改运行 `dotnet build <项目解决方案>.sln`；HTTP 状态码、WebSocket 连接、断线、平台差异和真实消息格式仍需在 Unity 中连接实际目标验证。
