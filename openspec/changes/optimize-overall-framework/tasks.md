# 任务列表：优化整体框架

## Assets/Framework/Manager/UI/UIBase.cs

- [x] 修复 `AddTempTimer`：先调用 `base.AddTempTimer(timerInfo)`，再调用 `Cleanup.TrackTimer(timerInfo.TimerName)`

## Assets/Framework/Manager/UI/UIManager.cs

- [x] 修复 `CloseUI`：在 `_uiBaseDic.Remove(uiName)` 之前添加 `GameGod.Instance.LoadManager.UnloadAsset(uiName + ".prefab")`

## Assets/Framework/Manager/Timer/TimerManager.cs

- [x] 修复 `AddTimer`：在 `TimerInfoDic.Add` 前检测重复名称，重复时 Log Error 并 return

## Assets/Framework/GameGod.cs

- [x] 修复 `OnDestroy`：在首行添加 `LoadHelper.Recycle(LoadHelper)`

## Assets/Framework/Base/GameBase.cs

- [x] 修复 `RelaseFsm`：将方法重命名为 `ReleaseFsm`，同时保留 `RelaseFsm` 作为废弃转发方法

## Assets/Framework/Manager/Event/EventManager.cs

- [x] 优化 `SendEvent`：为每个监听者 `Invoke` 包裹 try-catch，捕获异常后 Log Error 并继续遍历
