# 红点系统参考

## 适用范围

用于给 UI Prefab 配置红点，新增或修改红点计算、事件刷新和显示逻辑。

## 相关实现

- 红点 Prefab 与组件：`RedPoint_1`
- 显示与监听：`RedPointBase`
- 业务计算与刷新：`RedPointHelper`
- 数量与回调保存：`RedPointManager`
- 刷新事件：`RedPointEvent`

## Prefab 配置流程

1. 在目标 UI Prefab 下放入 `RedPoint_1`，或在已有红点节点挂 `RedPointBase` 派生组件。
2. 在 Inspector 的 `RedPointIds` 填写红点 Id；可配置多个 Id，任意一个数量大于零都会显示。
3. 不区分具体对象时保持空 `Key`；固定对象可直接在 Prefab 填写 `Key`。
4. 列表项等动态对象在代码中设置 `Key`，其值必须与刷新回调发送的 Key 完全一致。
5. 需要业务代码访问组件时沿用 UI 生成绑定；绑定不存在时先取得用户确认，不手写 Design 字段代替生成结果。

`RedPoint_1` 的 `RedPointIds` 和 `Key` 默认都为空，实际值由宿主 Prefab 配置。

## 客户端红点流程

1. 启动流程调用一次 `RedPointHelper.Instance.OnAwake()`，由 `RegisterUpdateCallback` 注册刷新事件。
2. `RedPointBase.Start` 遍历 Prefab 上的 `RedPointIds`，向 `RedPointManager` 注册回调，并通过 `GetNumById(id, Key)` 读取初始数量。
3. 数据变化后发送对应 `RedPointEvent`。
4. `RedPointHelper` 的 `Update_XXX` 重新计算数量，再通过 `CommoneSetNumAndInvoke` 保存数量并通知监听组件。
5. `RedPointBase` 只处理 Key 与自身 `Key` 相同的回调，再控制自身显隐。
6. `RedPointBase.OnDestroy` 负责移除回调，业务 UI 不重复注册和清理。

`RedPointEvent` 是刷新事件，`RedPointIds` 是红点业务 Id，两者不是同一个值。数量缓存只按红点 Id 保存，Key 只参与初始计算和回调分发，不得把它当成按 Key 独立保存的数量。

## 新增客户端红点

1. 先确认能否复用已有红点 Id 和刷新事件；业务含义不同才新增。
2. 在 `RedPointEvent` 新增事件，并在 `RegisterUpdateCallback` 注册对应 `Update_XXX`。
3. 在 `InitRedPoint` 增加首次显示计算，在 `Update_XXX` 增加数据变化后的计算；两处判断保持一致。
4. `Update_XXX` 使用同一个红点 Id 调用 `CommoneSetNumAndInvoke`，并按业务需要传入 Key。
5. 在目标 Prefab 配置相同的 `RedPointIds`，再决定使用空 Key、固定 Key 或动态 Key。
6. 在实际数据所有者处发送刷新事件。

新增前确认红点 Id、事件、处理方法和 Prefab 配置没有重复或遗漏。

## 生命周期注意

- `Awake` 创建组件自己的数量字典，`Start` 注册回调并计算初始数量，两者都只执行一次。
- `OnDisable` 会清空当前数量和字典，但不会移除回调，也不会在重新启用时再次计算初始数量。
- 会被隐藏后复用的 Prefab 必须验证再次显示后是否会收到刷新事件；不能假设重新启用会自动重算。
- `OnDestroy` 移除回调；`RedPointManager` 的数量和回调由 `ClearRedPointData` 或管理器销毁流程统一清理。

## 交叉引用

| 相关规范 | 处理内容 |
| --- | --- |
| [C# UI 开发参考](../../develop-unity-ui/references/csharp-ui.md) | Prefab 绑定、组件生成和 C# UI 生命周期 |

## 验证

- 检查 Prefab 的组件类型、`RedPointIds` 和 `Key`。
- 检查 `RedPointHelper` 已初始化，红点 Id、刷新事件、`InitRedPoint` 和 `Update_XXX` 对应一致。
- 验证初始显示、`0 → 1 → 0` 切换、不同 Key、隐藏后复用和销毁清理。
- C# 修改运行 `dotnet build <项目解决方案>.sln`；Prefab 配置和界面刷新仍需在 Unity 中验证。
