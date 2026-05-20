# GameDemo# Roguelike战棋 Demo

基于 [EmblemForge](https://github.com/DallenLarson/EmblemForge) 开源战棋框架开发的肉鸽战棋Demo项目。
开发过程由AI驱动

## 模板来源

- **EmblemForge** by DallenLarson (MIT License)
- 原始仓库：https://github.com/DallenLarson/EmblemForge
- 提供了网格系统、回合制战斗（Player Phase / Enemy Phase）、单位ScriptableObject架构
- 飞书链接https://rcni4yr62dhz.feishu.cn/wiki/LiHVwYD7ziQUWVkKPt7cJGUrnDg?table=tblLdRmRqFBja2lN&view=vewXxBNTOK
## 改动方向

在 EmblemForge 基础上增加：
- 肉鸽路线选择（Slay the Spire 式爬塔地图）
- 战后三选一奖励系统
- 六维属性 + 自定义伤害公式
- 技能系统 + 行动轮盘UI
- 商店/篝火/随机事件
- 简易敌人AI

## 技术栈

- Unity 2022.3 LTS
- C#
- URP

## 项目结构

```
Assets/
├── Scripts/          # 核心C#脚本
├── Scenes/           # Unity场景
├── Prefabs/          # 预制体
├── Units/            # 单位ScriptableObject数据
└── ...
docs/                 # 项目设计文档
```

## License

本项目代码基于 EmblemForge (MIT License) 修改。
