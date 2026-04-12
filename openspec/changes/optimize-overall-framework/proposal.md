# 提案：优化整体框架

## 背景

通过全面审查 BFramework-Ex 框架代码，发现若干影响运行时正确性的 bug 及潜在优化点。

## 涉及的管理器和基类

- `Framework/Base/GameBase.cs` — FSM 方法名拼写错误
- `Framework/Tools/AutoCleanup.cs` — 临时定时器跟踪 bug（被动影响）
- `Framework/Manager/UI/UIBase.cs` — AddTempTimer 调用顺序 bug
- `Framework/Manager/UI/UIManager.cs` — CloseUI 缺少预制体卸载
- `Framework/Manager/Timer/TimerManager.cs` — AddTimer 重复名称崩溃
- `Framework/Manager/Event/EventManager.cs` — SendEvent 异常隔离
- `Framework/GameGod.cs` — OnDestroy 全局 LoadHelper 未回收

## 需要修改的文件路径

1. `Assets/Framework/Base/GameBase.cs`
2. `Assets/Framework/Manager/UI/UIBase.cs`
3. `Assets/Framework/Manager/UI/UIManager.cs`
4. `Assets/Framework/Manager/Timer/TimerManager.cs`
5. `Assets/Framework/Manager/Event/EventManager.cs`
6. `Assets/Framework/GameGod.cs`
