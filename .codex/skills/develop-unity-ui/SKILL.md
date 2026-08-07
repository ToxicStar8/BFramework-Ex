---
name: develop-unity-ui
description: 按项目现有 C# 实现开发、修改或排查 Unity UI。用于新建界面、扩展现有界面，以及处理 UI 生命周期、组件绑定、业务数据、资源显示和 Module 数据边界。
---

# Unity UI 开发

## 核心规则

1. 新界面、可独立拆分的新 UI 功能和现有 UI 修改均使用 C#，并沿用当前生命周期和调用方式。
2. 开发前定位目标业务现有的 C# Module 与 Event；已有 Module 时优先复用，涉及共享数据所有者时按根 `AGENT.md` 的 Skill 调用路由加载客户端 Skill。
3. HTTP 与 WebSocket 调用由业务 Module 承接，UI 只消费打开参数、Event 参数或 Module 保存的数据。
4. 不确定选择会影响模块边界、Prefab 结构或数据传递时，列出可行方案并等待用户确认。
5. 只读取当前任务需要的 C# UI reference。

## Reference 路由

| 任务类型 | 必读文档 | 补充文档 | 优先级 |
| --- | --- | --- | --- |
| 新建、修改或排查 C# UI | [csharp-ui.md](references/csharp-ui.md) | — | P0 |

## 执行顺序

1. 检查目标 Prefab、挂载组件、现有脚本、直接调用方、数据格式和可复用 Module，确认当前 UI 归属。
2. 按上表读取 reference，并读取目标文件和一个同类型参考实现。
3. 明确初始化、显示、刷新、隐藏、销毁时机，以及数据来源和 UI/Module 边界。
4. C# 业务文件与所需绑定已存在时直接开发；文件缺失或必须更新生成绑定时，按 reference 先询问用户，获得确认后再使用对应的 UI 生成入口。
5. 按 reference 执行聚焦验证，并说明需要 Unity Editor、运行时或服务端配合的未验证项。

## 通用提示与选择

- 短暂的非阻塞提示使用 `UITips`，打开参数依次为提示文本和物品 Id。
- 普通确认使用 `UIDialog` 与 `UIDialogData`，按需求配置确认、取消按钮和回调。
- 多项选择使用 `UISelect` 与 `UISelectData`。
- 现有通用界面不支持复选框、验证码或文字确认；需求确实存在时按新 UI 流程处理，不拼接不存在的接口。
- 确认回调只执行用户确认后的业务；取消与关闭行为有需求时分别传入对应回调，不用默认成功或自动继续代替用户选择。
