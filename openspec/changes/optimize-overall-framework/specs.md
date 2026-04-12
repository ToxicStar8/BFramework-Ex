# 规格说明：优化整体框架

## 1. UIBase.AddTempTimer 调用顺序修正

**问题**：`UIBase.AddTempTimer` 在调用 `base.AddTempTimer(timerInfo)` 之前调用了 `Cleanup.TrackTimer(timerInfo.TimerName)`。此时 `TimerName` 为 null（TimerManager 在 `AddTimer` 内部才赋值），导致临时定时器无法被 AutoCleanup 正确跟踪，UI 关闭时无法取消相关定时器，造成野回调。

**修复**：先调用 `base.AddTempTimer(timerInfo)`（内部执行 `timerInfo.TimerName = timerName`），再调用 `Cleanup.TrackTimer(timerInfo.TimerName)`。

## 2. UIManager.CloseUI 预制体内存泄漏

**问题**：`UIManager.CloseUI` 调用 `uiBase.OnDispose()` 回收了 UI 自身的 LoadHelper，但 UI 预制体本身是通过 `GameGod.Instance.LoadHelper.CreateGameObjectSync` 加载的，其引用计数从未被释放。`CloseAll` 中有调用 `LoadManager.UnloadAsset(item.Key + ".prefab")`，行为不一致。

**修复**：在 `CloseUI` 的销毁逻辑末尾添加 `GameGod.Instance.LoadManager.UnloadAsset(uiName + ".prefab")`，与 `CloseAll` 保持一致。

## 3. TimerManager.AddTimer 重复名称崩溃

**问题**：`TimerInfoDic.Add(timerName, timerInfo)` 在名称重复时抛 `ArgumentException`，可能导致运行时崩溃。

**修复**：在 Add 之前检测是否存在同名定时器，若存在则记录错误日志并提前返回。

## 4. GameGod.OnDestroy 全局 LoadHelper 未回收

**问题**：`GameGod.Awake` 中通过 `LoadHelper.Create()` 创建了全局 LoadHelper，但 `OnDestroy` 中从未调用 `LoadHelper.Recycle(LoadHelper)` 回收，导致其内部资产引用不被释放。

**修复**：在 `OnDestroy` 的管理器销毁逻辑之前回收全局 LoadHelper。

## 5. GameBase.RelaseFsm 方法名拼写错误

**问题**：`GameBase` 中 FSM 释放方法名为 `RelaseFsm`（拼写错误），正确应为 `ReleaseFsm`。

**修复**：将 `RelaseFsm` 重命名为 `ReleaseFsm`，同时保留 `RelaseFsm` 作为废弃重定向，避免已有调用代码编译报错。

## 6. EventManager.SendEvent 异常隔离

**问题**：`SendEvent` 遍历监听者列表时，若某个监听者回调内部抛出异常，会中断遍历，导致后续监听者永远不会被调用。

**修复**：在遍历循环内为每个 `Invoke` 调用包裹 try-catch，捕获异常后记录错误日志并继续执行后续监听者。
