# 设计说明：优化整体框架

## 数据流与调用关系

### 临时定时器生命周期（修复后）

```
UIBase.AddTempTimer(timerInfo)
  → base.AddTempTimer(timerInfo)
      → TimerManager.AddTempTimer(timerInfo)
          → timerInfo.TimerName = _tempIndex.ToString()  ← 此时才赋值
          → TimerManager.AddTimer(timerName, timerInfo)
  → Cleanup.TrackTimer(timerInfo.TimerName)  ← 修复后：名称已赋值，可正确跟踪
```

### UI 关闭流程（修复后）

```
UIManager.CloseUI(uiName)
  → uiBase.OnDispose()
      → LoadHelper.Recycle(LoadHelper)   ← 卸载 UI 内部加载的资源
      → AutoCleanup.Dispose()            ← 清理事件/Update/定时器
  → Object.Destroy(uiBase.gameObject)
  → LoadManager.UnloadAsset(uiName + ".prefab")   ← 新增：释放预制体引用
  → _uiBaseDic.Remove(uiName)
```

### TimerManager.AddTimer 防重（修复后）

```
AddTimer(timerName, timerInfo)
  → if TimerInfoDic.ContainsKey(timerName) → Log Error + return  ← 新增防护
  → timerInfo.TimerName = timerName
  → TimerInfoDic.Add(timerName, timerInfo)
  → ExecTimer(timerInfo).Forget()
```

### GameGod 销毁流程（修复后）

```
OnDestroy()
  → LoadHelper.Recycle(LoadHelper)   ← 新增：回收全局加载器
  → ModuleManager.OnDispose()
  → ... (其余管理器顺序不变)
```

### EventManager.SendEvent 异常隔离（修复后）

```
SendEvent(eventNo, args)
  → for each listener (倒序遍历)
      → try { listener.Invoke(args) }
         catch (Exception e) { Log Error }   ← 新增：异常隔离
```

## 类结构变更

### GameBase.cs

- 将 `protected void RelaseFsm(int fsmId)` 重命名为 `protected void ReleaseFsm(int fsmId)`
- 保留 `protected void RelaseFsm(int fsmId)` 作为废弃调用，内部调用 `ReleaseFsm`

### UIBase.cs

- `AddTempTimer`：调换 base 调用与 Cleanup.TrackTimer 的顺序

### UIManager.cs

- `CloseUI`：在销毁 GameObject 后添加 `GameGod.Instance.LoadManager.UnloadAsset(uiName + ".prefab")`

### TimerManager.cs

- `AddTimer`：在 `TimerInfoDic.Add` 前添加重复名称检测

### EventManager.cs

- `SendEvent`：在回调 Invoke 外层添加 try-catch

### GameGod.cs

- `OnDestroy`：在首行添加 `LoadHelper.Recycle(LoadHelper)`
