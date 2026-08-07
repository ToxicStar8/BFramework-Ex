---
name: develop-unity-client
description: 按 Unity 项目现有实现使用 BFEX 框架 API，开发或修改客户端 Module、事件、HTTP、WebSocket、资源加载和红点流程。用于查询或调用项目已有框架入口、新建或扩展业务 Module、处理跨模块事件、发起 HTTP 请求、建立 WebSocket 连接与分发消息、注册与清理监听、选择资源加载 API、处理缓存或卸载边界、新增或修改红点；按任务只读取对应项目 reference。
---

# Unity 客户端开发

## 核心规则

1. 先读取目标代码、直接调用方、对应基类和一个同类型实现，再决定新增或沿用入口。
2. 优先复用项目已有 Module、事件和资源管理器，不为单个业务建立旁路或重复封装。
3. 只读取当前任务需要的 reference；同时涉及多个领域时再组合读取。
4. reference 与实际代码不一致时，以代码确认当前事实并指出冲突，不按过期说明直接修改。

## Reference 路由

| 任务类型 | 必读文档 |
| --- | --- |
| 查询或调用 BFEX/BFramework 基类、定时器和日志 API | [bfex-api.md](references/bfex-api.md) |
| 新建或扩展 Module、处理共享业务数据、EventDefine、事件注册与清理 | [module-event.md](references/module-event.md) |
| 发起 HTTP 请求，建立 WebSocket 连接，处理请求头、回调、消息分发与生命周期 | [network.md](references/network.md) |
| 加载 Sprite、Prefab 或其他 Asset，处理缓存、卸载、YooAsset 调用边界 | [resource-loading.md](references/resource-loading.md) |
| 配置或修改红点、扩展 RedPointHelper、处理 Key 与刷新 | [red-point.md](references/red-point.md) |

## 执行要求

1. 确认数据或资源的所有者、创建入口、使用生命周期和清理位置。
2. 沿用目标模块现有同步/异步方式和命名，不顺手重构旧实现。
3. 做最小完整修改，并运行与改动范围匹配的聚焦搜索、编译或运行时验证。
