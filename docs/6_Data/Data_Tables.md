# Data_Tables - 核心数据表结构

> [Demo] 先实现英雄表、技能表、装备表、敌人表。其余表（事件/祝福/章节/羁绊/Meta）后续补充。

## 概述

本文档定义游戏中核心数据的JSON/数据结构参考，供程序与策划对齐使用。

---

## 一、英雄表 (heroes.json)

```json
{
  "heroes": [
    {
      "id": "hero_ella",
      "name": "艾拉",
      "title": "银骑士",
      "rarity": "common",
      "class": "knight",
      "unlock_condition": {"type": "default"},
      "base_stats": {
        "hp": 120, "mp": 40,
        "str": 14, "mag": 6, "spd": 10, "def": 16, "res": 10, "mov": 4
      },
      "growth_rates": {
        "hp": 15, "mp": 8,
        "str": 0.6, "mag": 0.2, "spd": 0.4, "def": 0.7, "res": 0.4
      },
      "initial_skills": ["hero_ella_holy_slash", "hero_ella_shield_bash"],
      "initial_passive": "hero_ella_knight_oath",
      "bond_targets": ["hero_sera"],
      "bond_skill": "hero_ella_sera_bond",
      "class_options": ["paladin", "valkyrie"],
      "portrait_path": "sprites/heroes/ella_portrait",
      "model_path": "models/heroes/ella"
    }
  ]
}
```

---

## 二、技能表 (skills.json)

```json
{
  "skills": [
    {
      "id": "hero_ella_holy_slash",
      "name": "圣光斩",
      "owner": "hero_ella",
      "type": "active",
      "category": "physical",
      "element": "holy",
      "cost": {"mp": 12, "ap": 2, "hp": 0},
      "range": {"shape": "line", "length": 3, "width": 1},
      "target": "enemy",
      "damage_formula": "str * 1.5 + 10",
      "effects": [
        {"type": "bonus_damage", "target_category": "demon", "multiplier": 1.3}
      ],
      "cooldown": 2,
      "animation": "fx_holy_slash",
      "sfx": "sfx_holy_slash",
      "upgrades": ["hero_ella_holy_slash_plus", "hero_ella_holy_slash_wide"]
    }
  ]
}
```

### 技能范围定义
```json
{
  "range_shapes": {
    "single": {"type": "point", "range": 1},
    "line": {"type": "line", "length": 3, "width": 1, "pierce": true},
    "cone": {"type": "cone", "length": 3, "angle": 90},
    "cross": {"type": "cross", "range": 1},
    "diamond": {"type": "diamond", "range": 2},
    "square": {"type": "square", "width": 3, "height": 3},
    "self": {"type": "self"},
    "global": {"type": "global"}
  }
}
```

---

## 三、装备表 (equipment.json)

```json
{
  "equipment": [
    {
      "id": "equip_eagle_eye_amulet",
      "name": "鹰眼护符",
      "slot": "accessory",
      "rarity": "rare",
      "stats": {"hit_rate": 15, "crit_rate": 5},
      "effects": [
        {
          "trigger": "on_attack_high_ground",
          "effect": "damage_multiplier",
          "value": 1.2
        }
      ],
      "price": 120,
      "sell_price": 60,
      "description": "高地攻击时伤害+20%，命中+15%，暴击+5%",
      "flavor": "鹰眼佣兵团团长的遗物",
      "icon_path": "sprites/equipment/eagle_eye_amulet",
      "model_path": null
    }
  ]
}
```

---

## 四、敌人表 (enemies.json)

```json
{
  "enemies": [
    {
      "id": "enemy_demon_soldier",
      "name": "恶魔士兵",
      "type": "grunt",
      "category": "demon",
      "tier": 1,
      "stats": {
        "hp": 80, "mp": 20,
        "str": 12, "mag": 4, "spd": 8, "def": 8, "res": 4, "mov": 4
      },
      "skills": ["enemy_slash", "enemy_shield_bash"],
      "ai_preset": "grunt_aggressive",
      "loot_table": "tier1_demon",
      "exp": 30,
      "soul_shards": 15,
      "model_path": "models/enemies/demon_soldier",
      "portrait_path": "sprites/enemies/demon_soldier"
    }
  ]
}
```

---

## 五、消耗品表 (consumables.json)

```json
{
  "consumables": [
    {
      "id": "item_health_potion",
      "name": "治疗药水",
      "type": "consumable",
      "rarity": "common",
      "effect": {"type": "heal", "value": 40, "target": "single_ally"},
      "stack_max": 3,
      "price": 30,
      "sell_price": 15,
      "battle_usable": true,
      "icon_path": "sprites/items/health_potion"
    }
  ]
}
```

---

## 六、事件表 (events.json)

```json
{
  "events": [
    {
      "id": "event_rescue_civilian",
      "name": "废墟中的哭声",
      "type": "rescue",
      "rarity": "common",
      "chapter_filter": [1, 2, 3, 4, 5],
      "min_floor": 3,
      "max_floor": 45,
      "cooldown_per_run": 1,
      "illustration": "illustrations/event_rescue",
      "narrative": "前方的走廊传来微弱的哭声...",
      "options": [
        {
          "id": "opt_1",
          "text": "[力量] 搬开石柱",
          "check": {"stat": "str", "value": 12, "scope": "highest_party"},
          "on_success": {
            "narrative": "石柱被你移开...",
            "rewards": [
              {"type": "equipment", "rarity": "rare", "count": 1},
              {"type": "soul_shards", "amount": 80}
            ]
          },
          "on_failure": {
            "narrative": "石柱纹丝不动...",
            "penalties": [{"type": "hp_damage", "amount": 20, "target": "highest_str"}]
          }
        },
        {
          "id": "opt_2",
          "text": "[离开] 无视哭声",
          "consequence": "none"
        }
      ]
    }
  ]
}
```

---

## 七、祝福/诅咒表 (blessings.json)

```json
{
  "blessings": [
    {
      "id": "blessing_war_god",
      "name": "战神祝福",
      "type": "blessing",
      "rarity": "rare",
      "effect": {"stat": "str", "modifier": "multiply", "value": 1.15, "scope": "all_allies"},
      "duration_floors": 5,
      "icon_path": "sprites/blessings/war_god"
    }
  ],
  "curses": [
    {
      "id": "curse_weakness",
      "name": "虚弱诅咒",
      "type": "curse",
      "rarity": "common",
      "effect": {"stat": "str", "modifier": "multiply", "value": 0.8, "scope": "all_allies"},
      "duration_floors": -1,
      "soul_shard_bonus": 80,
      "icon_path": "sprites/curses/weakness"
    }
  ]
}
```

---

## 八、章节配置 (chapters.json)

```json
{
  "chapters": [
    {
      "id": "chapter_1",
      "name": "迷失回廊",
      "floors": [1, 10],
      "theme": "dungeon",
      "terrain_pool": ["stone_floor", "pillar", "rubble"],
      "enemy_pool": ["tier1"],
      "boss": "boss_gatekeeper_garrum",
      "boss_floor": 10,
      "bgm": "bgm_chapter1",
      "ambient": "ambient_dungeon",
      "lighting": "dark_torchlight",
      "color_palette": "stone_gray"
    }
  ]
}
```

---

## 九、Meta成长表 (meta_progression.json)

```json
{
  "meta_levels": [
    {
      "level": 1,
      "required_shards": 0,
      "unlocks": [
        {"type": "starting_gold", "value": 30}
      ]
    },
    {
      "level": 2,
      "required_shards": 200,
      "unlocks": [
        {"type": "backpack_slots", "value": 1}
      ]
    },
    {
      "level": 4,
      "required_shards": 700,
      "unlocks": [
        {"type": "hero_unlock", "hero_id": "hero_vera"}
      ]
    }
  ]
}
```

---

## 十、羁绊表 (bonds.json)

```json
{
  "bonds": [
    {
      "id": "bond_ella_sera",
      "hero_a": "hero_ella",
      "hero_b": "hero_sera",
      "name": "远近距离",
      "levels": [
        {
          "level": 1,
          "name": "相识",
          "condition": "both_in_party",
          "effect": {"stat": "spd", "modifier": "add_percent", "value": 10, "when": "adjacent"}
        },
        {
          "level": 2,
          "name": "信任",
          "condition": "battled_together_3",
          "effect": {"stat": "spd", "modifier": "add_percent", "value": 15, "when": "adjacent"},
          "unlocks_skill": "bond_ella_sera_cover_shot"
        },
        {
          "level": 3,
          "name": "灵魂羁绊",
          "condition": "battled_together_10",
          "effect": {
            "stat_bonus": {"spd": 20, "crit": 10},
            "revive_protection": true
          }
        }
      ]
    }
  ]
}
```
