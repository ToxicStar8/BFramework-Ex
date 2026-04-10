---
description: 实现 OpenSpec 变更中的任务（实验性）
---

实现 OpenSpec 变更中的任务。

**输入**：可在 `/opsx:apply` 后指定变更名称（例如 `/opsx:apply add-auth`）。若省略，先从对话上下文判断能否推断；若模糊或有歧义，必须提示用户选择可用变更。

**步骤**

1. **选择变更**

   若提供名称，直接使用。否则：
   - 若对话中提到变更，则从上下文推断
   - 若仅有一个活跃变更，可自动选择
   - 若存在歧义，运行 `openspec list --json` 获取可用变更，并使用 **AskUserQuestion 工具**让用户选择

   始终声明：“使用变更：<name>”，并说明如何切换（例如 `/opsx:apply <other>`）。

2. **检查状态以了解 schema**
   ```bash
   openspec status --change "<name>" --json
   ```
   解析 JSON，了解：
   - `schemaName`：正在使用的工作流（例如 “spec-driven”）
   - 哪个工件包含任务（spec-driven 通常为 “tasks”，其他需查看状态）

3. **获取 apply 指令**

   ```bash
   openspec instructions apply --change "<name>" --json
   ```

   返回内容包括：
   - 上下文文件路径（取决于 schema）
   - 进度（总数/已完成/剩余）
   - 任务列表及状态
   - 基于当前状态的动态指令

   **处理不同状态：**
   - 若 `state: "blocked"`（缺少工件）：提示并建议使用 `/opsx:continue`
   - 若 `state: "all_done"`：祝贺并建议归档
   - 否则：进入实现流程

4. **读取上下文文件**

   读取 apply 指令输出中的 `contextFiles`。
   文件依赖 schema：
   - **spec-driven**：proposal、specs、design、tasks
   - 其他 schema：以 CLI 输出的 contextFiles 为准

5. **展示当前进度**

   展示：
   - 正在使用的 schema
   - 进度：“N/M tasks complete”
   - 剩余任务概览
   - CLI 返回的动态指令

6. **实现任务（循环直到完成或阻塞）**

   对每个未完成任务：
   - 显示正在处理的任务
   - 完成所需代码变更
   - 变更保持最小且聚焦
   - 在 tasks 文件中标记完成：`- [ ]` → `- [x]`
   - 继续下一个任务

   **暂停条件：**
   - 任务不清晰 → 先询问
   - 实现暴露设计问题 → 建议更新工件
   - 遇到错误或阻塞 → 报告并等待指示
   - 用户中断

7. **完成或暂停时展示状态**

   展示：
   - 本次会话完成的任务
   - 总体进度：“N/M tasks complete”
   - 若全部完成：建议归档
   - 若暂停：说明原因并等待指示

**护栏**
- 按任务推进，直到完成或阻塞
- 开始前务必读取上下文文件（来自 apply 指令输出）
- 任务不明确时暂停并询问
- 实现暴露问题时暂停并建议更新工件
- 每个任务的代码变更要最小且聚焦
- 完成任务后立即勾选
- 遇到错误/阻塞/不清晰需求时暂停，不要猜
- 以 CLI 输出的 contextFiles 为准，不要假设文件名

