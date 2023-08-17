# Pokemon-Battle-Core
## Introduction

此框架专注于回合制游戏的快速实现，提供了游戏核心功能的接口，开发者只需要根据需求做少许修改即可适应大部分回合制游戏需求。

项目中还提供了两个可运行demo

* Demo1参考自宝可梦，经典回合制游戏。Demo支持单打，双打，除战斗结算外功能均已基本实现。
* Demo2参考自闪烁之光，放置类游戏。除战斗结算外功能均已基本实现。

## Framework Logic

1. 框架的核心，`BattleMgr`被初始化，需要持续被使用的对象被传入并保存。
2. `BattleMgr`初始化`BattleRound`，添加首回合需要调用的`BattlePlayable`，更新回合计数。
3. `BattlePlayable`依次被调用，若没有`BattlePlayable`等待被执行，`BattleMgr`的`EndOfCurRound`被调用（战斗结算也在这里判断），回合末的各种结算，更新函数被调用之后，`LoadNextBattleRound`被调用，新的`BattleRound`被初始化并添加下一回合需要的各种`BattlePlayable`。

## Framework Details

### Managers

#### BattleMgr

单例，负责战斗的流程调度，初始化每个回合，负责每个回合以及战斗的结算。每个`BattlePlayable`的处理（包括添加，删除，执行权限转让，结算等）都在这里处理。

开发者需要实现

* `StartFirstRound` - 游戏第一回合的初始化，额外需要初始化一个`BattleRound`，并在函数最后执行。
* `LoadNextBattleRound` - 除了第一回合，每个回合的初始化，额外需要初始化一个`BattleRound`，并在函数最后执行。
* `EndOfCurRound` - 每一回合的结尾，善后处理，战斗的结束也在这里判断，如果需要执行下一回合，则调用`LoadNextBattleRound`。

#### BuffMgr

单例，类似事件系统，此管理器负责buff的管理。

开发者需要实现

* `AddBuff` - 创建对应事件的buff列表，因为涉及某些特定buff事件的触发，没有放在抽象类里实现，此处给出对应代码和注释

  ```c#
          public override async UniTask<ABuffRecorder> AddBuff(IBattleEntity source, IBattleEntity target, int buffKey, bool isAttribute)
          {
              var template = ConfigManager.Instance.GetBuffTemplateByID(buffKey);
              Listeners.TryGetValue(template.BuffTriggerEvent, out var listener);
              if (listener == null)
              {
                  listener = new List<ABuffRecorder>();
                  Listeners.Add(template.BuffTriggerEvent, listener);
              }
  
              
              //以上是基础功能，从此以下是在最开始就要调用的buff
              BuffRecorder recorder = new BuffRecorder(source, target, template, isAttribute);
              listener.Add(recorder);
              
              if (recorder.Template.BuffTriggerEvent == Constant.Constant.BuffEventKey.AfterAddThisBuff)
              {
                  await ExecuteBuffByRecorder(recorder, new SkillResult(), target);
              }
  
              await ExecuteBuff(Constant.Constant.BuffEventKey.AfterAddBuff, new SkillResult(), target);
              
              return recorder;
          }
  ```

  
