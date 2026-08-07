# 资源加载参考

## 事实入口

- 通用资源加载：`LoadHelper`
- UI 和 Unit 使用所属界面持有的 `LoadHelper`
- YooAsset 包名：`GlobalDefine.PackageName`

## 对外 API

| 资源用途 | 使用入口 |
| --- | --- |
| UI 图集中的命名 Sprite | `loadHelper.LoadSpriteSync(atlasName, spriteName)` / `GetSpriteAsync(...)` |
| 指定类型的普通 Asset | `loadHelper.LoadSync<T>(objectName)` / `LoadAsync<T>(...)` |
| 不指定类型的普通 Asset | `loadHelper.LoadSync(objectName)` / `LoadAsync(...)` |
| 创建 Prefab 实例 | `loadHelper.CreateGameObjectSync(objectName, parent)` / `CreateGameObjectAsync(...)` |

- 新的资源所有者使用 `LoadHelper.Create()` 创建加载器，并在生命周期结束时调用 `LoadHelper.Recycle(loadHelper)`。
- UI 和 Unit 复用已有 `LoadHelper`，不自行创建第二个加载器；UI Prefab 继续通过现有 UI 打开和关闭入口管理。
- 已有语义化入口时优先使用，不重复拼接资源名或建立旁路加载方法。
- 普通业务代码不直接调用 `YooAssets.GetPackage`；只有修改底层加载器、启动或资源更新流程时才沿用对应框架实现。

## 同步、异步与缓存

- 沿用目标调用链当前的同步或异步方式，不主动把同步业务改成 `UniTask`，也不为预留优化增加异步包装。
- 只有现有接口已经异步、资源确实需要等待，或用户明确要求时使用 `LoadAsync`、`GetSpriteAsync` 或 `CreateGameObjectAsync`。
- 每个 `LoadHelper` 按资源名缓存 AssetHandle，并缓存已经取得的 Sprite；业务层不再建立相同缓存。
- `LoadHelper` 不提供单资源卸载；同一加载器管理的资源按所有者生命周期统一释放。
- `LoadHelper.Recycle` 会销毁该加载器创建的 GameObject、清空 Sprite 缓存并释放全部 AssetHandle；返回对象仍在使用时不得提前回收。
- UI 隐藏不会回收加载器，关闭 UI 才会释放。

## YooAsset 边界

- `LoadHelper` 封装 YooAsset 加载与 Handle 释放，外部业务不自行保存或释放内部 Handle。
- 修改 `LoadHelper` 时先核对 Handle、缓存键和回收时机，避免返回对象仍被使用时提前卸载。
- YooAsset 下载、版本检查和资源更新不属于本 reference；涉及这些流程时读取项目实际更新代码后单独处理。

## 验证

- 聚焦确认普通业务没有新增直接 `YooAssets` 调用，也没有绕过已有 `LoadHelper`；搜索工具顺序遵守根 `AGENT.md`。
- 检查加载键、图集名、Sprite 名和加载器所有权，并确认回收时没有仍在使用的资源或实例。
- C# 修改运行 `dotnet build <项目解决方案>.sln`；资源地址、图集内容、Prefab 实例化和清理效果仍需 Unity 运行时验证。
