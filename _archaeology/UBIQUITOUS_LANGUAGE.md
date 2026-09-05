# Ubiquitous Language — PPServerSeamless (GIT iGaming Solution)

> 从 PPServerSeamless 仓库考古过程中提取的领域语言。统一术语，供后续开发与沟通使用。

## 钱包与资金（Wallet Domain）

| 术语 | 定义 | 应避免的说法 |
|---|---|---|
| **Seamless Wallet（无缝钱包）** | 玩家余额由运营商侧托管、游戏方通过回调实时扣派款的钱包模式 | 单一钱包、integrated wallet |
| **Transfer Wallet（转账钱包）** | 玩家资金先划入游戏方账户、离场再转回运营商的钱包模式 | 转账模式、wallet transfer |
| **Seamless 回调五元组** | 聚合平台调用本服务的 5 个钱包接口：`GetBalance / Withdraw / Deposit / RollbackTransaction / BetWin` | 钱包 API、balance 接口 |
| **Withdraw** | 回调语义中的**扣款**（下注时调用），非玩家提现 | 提款、提现 |
| **Deposit** | 回调语义中的**入款**（派彩时调用），非玩家充值 | 存款、充值 |
| **RollbackTransaction** | 撤销一笔已发生的 Withdraw/Deposit，恢复余额 | 回滚、冲正 |
| **Agent（代理）** | 拥有下级玩家、可代充代提的运营商侧账户 | 代理商、operator |
| **GIT API** | 本系统对运营商/代理暴露的自有管理 API（`GITApiController`） | 后台 API、admin API |

## 游戏会话（Game Session Domain）

| 术语 | 定义 | 应避免的说法 |
|---|---|---|
| **mgckey** | 玩家游戏会话令牌，PP 客户端协议中所有请求的鉴权凭证 | token、session id |
| **doInit** | 客户端进入游戏的首个动作，触发进场（EnterGame）流程 | 初始化、login |
| **doSpin** | 一次付费旋转请求，服务端现算结果 | 旋转、下注 |
| **doCollect** | 玩家领取当前累计赢分的动作 | 收钱、cashout |
| **EnterGame（进场）** | 校验会话并为用户绑定游戏实例的准入流程 | 进入游戏、join |
| **index/counter** | 客户端请求序号对，服务端用于防重放校验 | 计数器、nonce |
| **symbol** | PP 协议中的游戏代号（如 `vs20doghouse`），非图标含义 | 游戏符号、game icon |
| **GAMEID** | 系统内部游戏数字编号（2001 起），与 symbol 一一映射 | 游戏 ID、game code |

## 游戏逻辑与赔付（Game Logic Domain）

| 术语 | 定义 | 应避免的说法 |
|---|---|---|
| **GameLogic** | 单款游戏的服务端规则实现类（转轮、赔付线、Bonus） | 游戏逻辑、slot engine |
| **BasePPSlotGame** | 全部 PP 游戏的抽象基类体系（5 个基类承载 411+ 款游戏） | 基类、slot base |
| **PayoutPool（奖池调节）** | 控制整体返奖率的赔付池机制，可干预单局结果 | 派彩池、pool |
| **Chipset（筹码配置）** | 按币种定义的可选投注档位集合（chipset(AUD).info 等） | 筹码、下注档位 |
| **PCG Random** | 系统内置的 PCG 族随机数实现（无第三方认证） | RNG、随机数 |
| **FRB（Free Round Bonus）** | 免费旋转奖励（促销接口 `frb/available`） | 免费转、freespin |
| **Race / Tournament** | PP 促销体系的两类竞速/锦标赛活动 | 比赛、活动 |

## 集群与节点（Cluster Domain）

| 术语 | 定义 | 应避免的说法 |
|---|---|---|
| **godgaming** | 全部方案共用的 Akka.NET ActorSystem 名称 | 集群名、system name |
| **Lighthouse** | Petabridge 开源种子节点服务，仅负责集群成员发现 | 发现服务、seed |
| **FrontNode** | 面向玩家客户端的 HTTP 协议网关节点 | 前置机、gateway |
| **UserNode** | 托管 UserActor（每在线用户一个）的会话/钱包节点 | 用户服务、session 节点 |
| **SlotNode** | 运行 GameLogic Actor 的游戏计算节点 | 游戏服、逻辑服 |
| **ApiNode** | 面向运营商的 GIT API 节点 | 管理节点、agent 节点 |
| **ApiIntegration** | 处理 Seamless 回调与上游官方 API 对接的集成节点 | 集成服、callback 节点 |
| **GITProtocol** | 全部节点共享的消息协议类库（GITMessage/GAMEID/BetweenServersMessage） | 协议库、common |

## 关系（Relationships）

- 一个 **Agent** 拥有多个**玩家**；玩家余额托管于 **Seamless Wallet** 或 **Transfer Wallet** 之一。
- 一次游戏会话以 **doInit** 开始、携带 **mgckey**，由 **index/counter** 防重放。
- **doSpin** 产生一次 **Withdraw**（扣款）+ 可选 **Deposit**（派彩）；异常时以 **RollbackTransaction** 冲正。
- **GAMEID** 与 **symbol** 一一对应；每个 GAMEID 路由到一个 **GameLogic**。
- **PayoutPool** 横切所有 **GameLogic**，调节全局返奖率。
- **FrontNode / UserNode / SlotNode / ApiNode / ApiIntegration** 均引用 **GITProtocol**，并通过 **Lighthouse** 加入 **godgaming** 集群。

## 示例对话（Example dialogue）

> **开发：** 玩家点了旋转但余额不足，链路怎么走？
> **领域专家：** 客户端发 **doSpin**，**UserActor** 先校验 **mgckey** 和 **index/counter**，再走 **Withdraw** 回调扣款——余额不足就在这一步拒掉，**GameLogic** 根本不会执行。
> **开发：** 那如果扣款成功了但算结果时节点挂了呢？
> **领域专家：** 聚合平台会发起 **RollbackTransaction** 冲正这笔 Withdraw；玩家重连后客户端的 counter 续传，不会重复扣款。
> **开发：** Transfer 版也一样吗？
> **领域专家：** 不一样。**Transfer Wallet** 模式钱进场时已划入，没有回调五元组，所以那个方案连 **ApiIntegration** 节点都没有。

## 已标记的歧义（Flagged ambiguities）

- **Withdraw/Deposit**：在 Seamless 回调语境是「扣款/派款」，在日常语境是「提现/充值」——两者方向相反，极易误读。**建议**：代码与文档中凡涉及回调一律写全称 `WithdrawCallback / DepositCallback`。
- **symbol**：PP 协议中是游戏代号（`vs20doghouse`），不是转轮图标。**建议**：内部统一用 **GAMEID**，仅协议边界处保留 symbol。
- **User**：UserNode 的 User 指**玩家**（终端赌客），QueenApi 的 `/api/user/create` 也是创建玩家；而「运营商/代理」是 **Agent**。两套 API 里 user 指向一致，但勿与系统操作员混淆。
- **GameProviders 枚举**：各方案取值不同（COUNT=1/6/9），同一名字 PP=1 虽稳定，但跨方案拷贝代码时 COUNT 语义已漂移。**建议**：以主力方案 Seamless2 的枚举为准。