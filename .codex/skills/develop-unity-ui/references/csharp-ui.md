# C# UI 开发参考

> **交叉引用**：[Module 与事件参考](../../develop-unity-client/references/module-event.md)（Module 创建、数据保存与 Event 转发）。仅标识规范关系，实际加载仍按根 `AGENT.md` 的 Skill 调用路由执行。

## 适用范围

- 用于扩展现有 C# UI 模块，或修改依附现有 C# 生命周期与数据的 UI 组件。
- 选择 C# 只确定实现语言，不代表立即生成代码或修改 Prefab。

## 选择 C# 后检查

1. 定位目标 Prefab、`<UIName>.cs`、`<UIName>.Design.cs`、打开调用方及对应 Module/Event。
2. 读取目标 UI 和一个同类型现有 UI，确认层级、参数、生命周期、Unit 复用和数据所有权。
3. 现有 Design 字段足够时只改业务 `.cs`，不要运行生成器，也不要手改 `.Design.cs`。
4. 需要新增、删除或重命名绑定时，先说明将更新 Prefab 绑定和 Design；业务脚本不存在时生成器才会创建，再取得用户许可。
5. 不主动新增 UI 层级、管理器或旁路调用方式；缺少配置时先报告事实。

## 文件与生成边界

- 根据需求使用 Unity 菜单 `GameObject/生成UI代码`、`GameObject/生成UI代码(不生成组件)` 或 `GameObject/生成UI代码(不生成Unit)`。
- 生成代码按 Prefab 的子目录写入 `Assets/GameData/Scripts/UI/` 下的对应子目录。
- `<UIName>.cs` 和 `<UnitName>.cs` 仅在文件不存在时生成，不覆盖已有主代码；后续业务逻辑写在对应 `partial class` 中。
- UI 和 Unit 的 `.Design.cs` 由生成器更新，文件头明确禁止手改。
- Prefab 节点前缀决定绑定类型；不得手写 Design 字段代替生成器。
- 修改前读取目标 `.cs`、对应 `.Design.cs`、Prefab、相关 Module 和一个同模块参考 UI。

## 业务代码风格

### 代码顺序

1. 类内枚举、常量和显示状态字段；新增私有字段使用 `_camelCase`，不顺手重命名无关旧字段。
2. `OnAwake`：初始化 UnitPool、注册事件并绑定固定按钮。
3. Module Event 回调。
4. `OnShow`、重置与刷新方法，以及 Unit 创建和业务辅助方法。
5. `OnClick_Btn_*`、`OnToggle_Tg_*` 等交互处理。
6. `OnBeforeDestroy`，完成额外解绑和资源清理。

### Lambda 表达式

- 不需要参数或捕获变量时直接使用方法组：`Btn_Close.AddListener(OnClick_Btn_Close);`。
- 需要携带索引、Unit 或临时业务数据时使用 Lambda：`unit.Btn_Click.RemoveAllAndAddListener(() => OnClickMember(index, unit));`。
- 单行转发方法可使用表达式体；包含判断、数据修改或多个调用时使用完整方法体。
- 动态 Unit 重新绑定按钮时先移除旧监听，避免复用后重复触发。

### UnitPool

- 在面板的 `OnAwake` 中按现有 Design 字段初始化 `UnitPool`；完整刷新列表前调用 `RecycleAll()`，再通过 `CreateUnit(parent)` 获取 Unit 并调用其数据显示方法。
- Unit 实现 `OnAwake()`、`OnRecycle()` 和 `OnBeforeDestroy()`；数据显示使用具体业务需要的强类型方法，不额外增加通用 `object[]` 入口。
- `CreateUnit()` 只在首次实例化时调用 `OnAwake()`，复用对象时不会自动刷新数据。
- `OnRecycle` 只清理会污染下次复用的状态、数据、回调或动态监听，不为形式完整添加重置逻辑。
- `OnBeforeDestroy` 只处理通过 UnitPool 最终释放时持有的额外资源。

### 异步调用

- 默认沿用现有同步流程，不主动引入 `Task`、`UniTask`、`async` 或 `await`。
- 只有现有接口或所在模块已经使用异步调用，或用户明确要求异步时，才沿用当前调用链。
- 确实需要异步时使用 `UniTask`；存在实际等待才使用 `async`/`await`，明确不等待时才使用 `.Forget()`，禁止 `async void` 和多余包装。

## Module 与 Event 边界

- UI 只消费打开参数、Event 参数或 Module 已保存的数据并刷新显示；业务 HTTP/WebSocket 由 Module 处理。

## 生命周期

- `OnAwake` 只执行一次，用于固定按钮监听、UnitPool 和 Event 注册。
- `OnShow` 每次打开都会执行，按当前入口消费参数或刷新显示。
- 隐藏只关闭 GameObject，不触发生命周期回调；只在显示期间运行的行为需要由业务显式停止。
- `OnBeforeDestroy` 处理额外解绑和资源清理；通过 UI 自身接口注册的 Event、Update 和 Timer 会在关闭时统一移除。

## UI 调用与数据边界

- C# 面板沿用 `OpenUI<T>()`、`HideUI<T>()`、`CloseUI<T>()` 及现有字符串重载，不新建旁路管理器。
- UI 只消费打开参数、Event 参数或 Module 已保存的数据，不复制 Module 的数据所有权和网络处理。
- 同一行为不要在 UI 与 Module 各维护一份数据；明确所有者、刷新入口和清理职责。
- 不新增未经验证的反射、动态类型或绑定名称依赖。

## 验证

- 检查业务文件与 `.Design.cs` 的职责没有混淆；执行过生成器时，确认预期脚本和组件绑定已正确更新。
- 使用聚焦搜索确认数据来源、Module 数据保存、打开 UI 或发送 Event 的入口、UI 数据消费和 Unit 生命周期。
- C# 修改必须运行 `dotnet build <项目解决方案>.sln`；编译失败时不得声称验证通过。
- Prefab 绑定、显示层级和运行时交互仍需 Unity 验证；无法运行时明确说明人工验证步骤。
