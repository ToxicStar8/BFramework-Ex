# BFEX（BFramework）API 速查

只记录当前项目已暴露且有实际调用基础的 BFEX API。未列出的入口先按根 `AGENT.md` 的搜索顺序确认现有用法。

## 基础入口

- 非 Mono 对象基类：`Assets/Framework/Base/GameBase.cs`
- Mono 对象基类：`Assets/Framework/Base/GameBaseMono.cs`
- 框架总入口：`Assets/Framework/GameGod.cs`
- Manager 实现：`Assets/Framework/Manager/`

继承 `GameBase` 或 `GameBaseMono` 时使用基类入口；静态方法、未继承基类的对象或已有特殊路径沿用 `GameGod.Instance.<Manager>`。

## 定时器

| 用途 | API |
| --- | --- |
| 创建 | `TimerInfo.Create(allCount, intervalTime, isExecImmed, execCallback, endCallback)` |
| 添加命名定时器 | `AddTimer(timerName, timerInfo)` |
| 添加自动命名定时器 | `AddTempTimer(timerInfo)` |
| 获取命名定时器 | `GetTimerInfo(timerName)` |
| 移除 | `RemoveTimer(timerName)` / `RemoveTimer(timerInfo)` |

- `AddTempTimer` 只是由管理器自动生成名称，不代表只执行一次；执行次数由 `allCount` 决定。
- `intervalTime` 单位为毫秒，`isExecImmed` 决定是否立即执行第一次回调；`endCallback` 的 `bool` 参数表示是否因取消结束。
- 定时器结束或取消后会自动回收，原 `TimerInfo` 不再复用。

## 日志

| 用途 | API |
| --- | --- |
| 普通日志 | `Log(...)` |
| 错误 | `LogError(...)` |
| 警告 | `LogWarning(...)` |
| 异常 | `LogException(...)` |
| 静态位置 | `GameGod.Instance.Log(E_Log.Xxx, title, content)` |

`GameGod.Log` 最终调用的日志入口标记为 `UNITY_EDITOR` 条件调用；需要非编辑器环境保留时先确认现有诊断入口。

## 交叉引用

| 相关规范 | 处理内容 |
| --- | --- |
| [Unity UI 开发 Skill](../../develop-unity-ui/SKILL.md) | UI 显隐、组件绑定与生命周期 |
| [HTTP 与 WebSocket 参考](network.md) | HTTP 请求、WebSocket 连接、消息回调与分发 |
| [资源加载参考](resource-loading.md) | UI、Sprite、Prefab 与普通 Asset 加载 |
| [红点系统参考](red-point.md) | 红点计算、刷新、组件与 Key |

## 验证

C# 修改运行 `dotnet build <项目解决方案>.sln`；需要运行时回调或显示结果时继续在 Unity 中验证。
