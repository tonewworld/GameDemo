# Roguelike战棋 Demo - 项目总文档

## 项目定位

**用途**：应聘展示用的可玩 Demo，非商业产品。

**核心目标**：在 2 分钟内让面试官看到：战棋走格子 + 肉鸽奖励循环 + 完整的代码架构。

**原则**：广度优先，深度次要。每个系统做"能跑"而不是"做全"。

---

## Demo 范围一览

| 模块 | Demo 要做 | Demo 不做 |
|------|-----------|-----------|
| 战斗 | 正方形网格、移动/攻击、我方/敌方回合交替、基础伤害公式 | Boss阶段转换、复杂AI、双击判定 |
| 英雄 | 3名可选初始英雄 + 2名可招募 | 转职、羁绊、8人满编 |
| 敌人 | 5种普通敌人 + 1个简易Boss | 精英敌人、部位破坏、复杂行为树 |
| 地图 | 10层塔、4种节点(战斗/商店/篝火/事件) | 章节主题、谜题棋盘、高度差 |
| 奖励 | 战后三选一（英雄/装备/消耗品） | 祝福诅咒、跳过机制 |
| 装备 | 武器+防具，各5件 | 饰品、圣物、Build协同 |
| UI | 主菜单、英雄选择、战斗HUD、奖励界面 | 图鉴、成就、完整设置 |
| Meta | 不做 | 灵魂残片成长树、难度阶梯、存档 |

### Demo 通关流程（约 15-20 分钟）
```
标题画面 → 选初始英雄(3选1) → 第1层战斗 → 三选一奖励 → 路线选择
→ 商店/篝火/事件 → 战斗 → ... → 第5层中Boss → ... → 第10层最终Boss → 通关画面
```

---

## 文档索引

### 0 — 总览
| 文档 | 描述 |
|------|------|
| [Design_Overview.md](0_Overview/Design_Overview.md) | 世界观概要、核心循环、Demo视觉风格 |

### 1 — 战斗
| 文档 | 描述 |
|------|------|
| [Module_Battle.md](1_Battle/Module_Battle.md) | 战棋战斗核心：回合制、六维属性、伤害公式 |
| [UI_Battle.md](1_Battle/UI_Battle.md) | 战斗HUD、行动队列、行动轮盘 |

### 2 — 地图
| 文档 | 描述 |
|------|------|
| [Module_Map.md](2_Map/Module_Map.md) | 塔层路线选择、节点类型、楼层生成 |
| [Module_Event.md](2_Map/Module_Event.md) | 随机事件（Demo含2-3个事件模板） |

### 3 — 实体
| 文档 | 描述 |
|------|------|
| [Module_Hero.md](3_Entities/Module_Hero.md) | 英雄属性、技能、升级 |
| [Module_Enemy.md](3_Entities/Module_Enemy.md) | 敌人种类、简易AI |
| [Module_Equipment.md](3_Entities/Module_Equipment.md) | 武器/防具/消耗品 |

### 4 — UI
| 文档 | 描述 |
|------|------|
| [UI_Main.md](4_UI/UI_Main.md) | 标题界面、简易设置 |
| [UI_HeroSelect.md](4_UI/UI_HeroSelect.md) | 初始英雄选择 |
| [UI_Reward.md](4_UI/UI_Reward.md) | 战后三选一奖励 |
| [UI_Camp.md](4_UI/UI_Camp.md) | 商店/篝火/事件对话 |

### 5 — 系统
| 文档 | 描述 |
|------|------|
| [Module_Backpack.md](5_Systems/Module_Backpack.md) | 背包（Demo简化版） |
| [Module_Meta.md](5_Systems/Module_Meta.md) | Meta成长（Demo不做，仅供后续参考） |

### 6 — 数据
| 文档 | 描述 |
|------|------|
| [Data_Tables.md](6_Data/Data_Tables.md) | 核心数据表结构 |

---

## 技术方案（Demo）

| 层面 | 方案 |
|------|------|
| 引擎 | Unity 2022.3 LTS |
| 渲染 | URP，2.5D斜俯视角 |
| 网格 | 正方形 Tilemap 或自定义 GridManager |
| 数据 | ScriptableObject 配置角色/敌人/装备 |
| 输入 | 新 Input System（键鼠为主） |

---

## Demo 开发步骤

### 第1步：获取模板（今天就做）
推荐 **Tactics Toolkit**（Unity Asset Store），它自带：
- 网格生成与移动范围显示
- 回合制（Player Phase / Enemy Phase）
- 单位属性/技能系统
- 战斗UI框架
- 敌人AI基础

### 第2步：改造成你的游戏（2-3周）
1. 替换美术资源 → 你的角色/场景
2. 修改属性系统 → 你的六维属性
3. 新增路线选择场景 → Slay the Spire 式地图
4. 新增奖励场景 → 三选一界面
5. 新增商店/篝火/事件场景
6. 串起完整流程：标题→选人→地图→战斗→奖励→循环

### 第3步：打磨（1周）
- 镜头动画
- 伤害数字弹出
- 转场效果
- 音效占位
- 录屏 + 简历链接

---

## 给面试官展示什么

1. **完整游戏循环**（30秒）：选人→战斗→奖励→继续，一气呵成
2. **战斗系统**（1分钟）：走格子、技能范围、伤害计算、击杀反馈
3. **肉鸽机制**（30秒）：路线选择、随机奖励、队伍构建
4. **代码架构**（面试时讲）：MVC分层、ScriptableObject数据驱动、状态机管理回合
