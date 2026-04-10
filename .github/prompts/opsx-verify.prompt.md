---
description: 归档前验证实现与变更工件一致
---

验证实现是否与变更工件（specs、tasks、design）一致。

**输入**：可在 `/opsx:verify` 后指定变更名称（例如 `/opsx:verify add-auth`）。若省略，先从对话上下文判断能否推断；若模糊或有歧义，必须提示用户选择可用变更。

**步骤**

1. **未提供变更名称时，提示选择**

   运行 `openspec list --json` 获取可用变更。使用 **AskUserQuestion 工具**让用户选择。

   仅展示存在实现任务的变更（tasks 工件存在）。
   若可用，包含每个变更的 schema。
   对任务未完成的变更标记 “(In Progress)”。

   **重要**：不要猜测或自动选择，必须让用户做选择。

2. **检查状态以了解 schema**
   ```bash
   openspec status --change "<name>" --json
   ```
   解析 JSON，了解：
   - `schemaName`：正在使用的工作流（例如 “spec-driven”）
   - 该变更已有哪些工件

3. **获取变更目录并加载工件**

   ```bash
   openspec instructions apply --change "<name>" --json
   ```

   返回变更目录和上下文文件。读取 `contextFiles` 中的所有可用工件。

4. **初始化验证报告结构**

   创建三维度报告结构：
   - **Completeness**：任务与规范覆盖
   - **Correctness**：需求实现与场景覆盖
   - **Coherence**：设计一致性与模式一致性

5. **验证完整性**

   参照上文（同技能说明）。

6. **验证正确性**

   参照上文（同技能说明）。

7. **验证一致性**

   参照上文（同技能说明）。

8. **生成验证报告**

   按上文格式输出汇总表、问题列表与结论。

**护栏**
- 使用明确的 Markdown 结构
- 每个问题都给出可执行建议
- 若不确定，降低严重级别
