# _archaeology — 项目考古产出

对仓库 `Bismarck777666/PPServerSeamless`（commit `7a1d6a7`，2025-09-01）的自动化考古分析产物。

## 目录

| 文件 | 内容 |
|---|---|
| `PPServerSeamless深度分析.md` | 总体深度分析：业务定位、10 套方案对比、Akka 集群架构、风险清单 |
| `项目考古报告.md` | Git 历史考古（热点/归属/密钥扫描）+ API 面还原 + 术语提取的汇总报告 |
| `UBIQUITOUS_LANGUAGE.md` | DDD 领域术语表：4 大领域 30 个术语 + 4 处已标记歧义 |
| `PPServerSeamless-API.openapi.json` | 全部 HTTP 接口的 OpenAPI 3.0.3 规范（298 个端点实现 → 131 条唯一路径），可直接导入 Swagger UI / Redoc / Apifox |
| `diagrams/*.mmd` | 4 张架构图的 Mermaid 源码：全局架构 / 单方案集群解剖 / Spin 请求流程 / 仓库组织树 |

## 关于图表渲染

`diagrams/` 收录的是 Mermaid **源文件**。GitHub 会在 Markdown 中直接渲染 ` ```mermaid ` 代码块；如需 SVG 图片，本地一条命令重新生成：

```bash
npm i -g @mermaid-js/mermaid-cli
mmdc -i diagrams/arch_overview.mmd -o diagrams/arch_overview.svg
```

## 安全提醒

本仓库历史中含明文生产凭据（MSSQL sa / Redis 密码 / 公网 IP），详见 `项目考古报告.md` 第 1.3 节。在采取轮换与历史清洗前，请谨慎控制本仓库的可见性。
