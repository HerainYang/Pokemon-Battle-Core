# Pokemon-Battle-Core
## Introduction

此框架专注于回合制游戏的快速实现，提供了游戏核心功能的接口，开发者只需要根据需求做少许修改即可适应大部分回合制游戏需求。

项目中还提供了两个可运行demo

* Demo1参考自宝可梦，经典回合制游戏。Demo支持单打，双打，除战斗结算外功能均已基本实现。
* Demo2参考自闪烁之光，放置类游戏。除战斗结算外功能均已基本实现。


This framework focuses on the rapid implementation of turn-based games, providing interfaces for the core functionalities of the game. Developers only need to make a few modifications according to their requirements to adapt it for the majority of turn-based game needs.

The project also includes two runnable demos:

- Demo1 is inspired by Pokémon, a classic turn-based game. The demo supports single battles, double battles, and most functionalities apart from battle resolution have been implemented.
- Demo2 is inspired by Tales of Radiance, an idle game. Most functionalities except for battle resolution have been implemented.

## Framework Logic

1. 框架的核心，`BattleMgr`被初始化，需要持续被使用的对象被传入并保存。
2. `BattleMgr`初始化`BattleRound`，添加首回合需要调用的`BattlePlayable`，更新回合计数。
3. `BattlePlayable`依次被调用，若没有`BattlePlayable`等待被执行，`BattleMgr`的`EndOfCurRound`被调用（战斗结算也在这里判断），回合末的各种结算，更新函数被调用之后，`LoadNextBattleRound`被调用，新的`BattleRound`被初始化并添加下一回合需要的各种`BattlePlayable`。





1. The core of the framework, `BattleMgr`, is initialized and objects that continuously used are passed in and stored.

2. `BattleMgr` initializes `BattleRound`, adds the necessary `BattlePlayable` for the first round, and updates the round count.

3. `BattlePlayable` is called sequentially. If there are no `BattlePlayable` waiting to be executed, `EndOfCurRound` in `BattleMgr` is invoked (End of the battle is also determined here). After various end-of-round update functions are called, `LoadNextBattleRound` is triggered. A new `BattleRound` is initialized and various `BattlePlayable` required for the next round are added.

## Framework Details

### Managers

#### BattleMgr

单例，负责战斗的流程调度，初始化每个回合，负责每个回合以及战斗的结算。每个`BattlePlayable`的处理（包括添加，删除，执行权限转让，结算等）都在这里处理。

开发者需要实现

* `StartFirstRound` - 游戏第一回合的初始化，额外需要初始化一个`BattleRound`，并在函数最后执行。
* `LoadNextBattleRound` - 除了第一回合，每个回合的初始化，额外需要初始化一个`BattleRound`，并在函数最后执行。
* `EndOfCurRound` - 每一回合的结尾，善后处理，战斗的结束也在这里判断，如果需要执行下一回合，则调用`LoadNextBattleRound`。



Singleton responsible for the battle process scheduling, initializing each round, managing each round, and resolving the battle. Handling of each `BattlePlayable` (including addition, removal, permission transfer, and resolution) is managed here.

Developers need to implement:

- `StartFirstRound` - Initialization of the game's first round. It additionally requires initializing a `BattleRound` and executing it at the end of the function.
- `LoadNextBattleRound` - Initialization for each round except the first one. It also requires initializing a `BattleRound` and executing it at the end of the function.
- `EndOfCurRound` - Conclusion of each round, post-processing, and determining the end of the battle. If the next round needs to be executed, `LoadNextBattleRound` is called.

#### BuffMgr

单例，类似事件系统，此管理器负责buff的管理。此管理器不需要提前初始化，但需要在每回合结束时调用`Update`，该函数会按照效果剩余回合数清理buff，触发析构函数。

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




Singleton resembling an event system, this manager is responsible for managing buffs. This manager doesn't need to be initialized in advance but needs to be called during the `Update` at the end of each round. This function will clean up buffs based on the remaining effect duration and trigger the destructor.

Developers need to implement:

- `AddBuff` - Create a list of buffs corresponding to the event. Due to the involvement of triggering certain specific buff events, they are not implemented in the abstract class. 

Above is the corresponding code and comments.

### BattlePlayable

每个回合中，能抽象为一次行为的事件，都建议创建为Playable，比如一个技能的释放，一个角色的登场，一个角色的死亡等。`BattlePlayable`的执行顺序由`Priority`定义，在宝可梦中，则可以被宝可梦的速度，或者具有特别优先级的技能来定义。更多关于`BattlePlayable`的实现可以看本文后篇或者项目代码。

开发者需要实现

* `Execute` - 事件的逻辑
* `OnDestroy` - 事件结束的逻辑，以及需要调用`BattleMgr.Instance.BattlePlayableEnd`来告知战斗管理器事件以及结束。




In each round, any event that can be abstracted as a single action is recommended to be created as a "Playable". For instance, using a skill, introducing a character, or a character's death could all be represented as Playables. The execution order of `BattlePlayable` is defined by its `Priority`. In the context of Pokémon, this could be determined by a Pokémon's speed or by skills with special priorities. For more details on the implementation of `BattlePlayable`, you can refer to the latter part of this article or the project code.

Developers need to implement:

- `Execute` - The logic of the event.
- `OnDestroy` - The logic for when the event concludes, and it's important to call `BattleMgr.Instance.BattlePlayableEnd` to notify the battle manager about the event's completion.

### BattleRound

每个回合作为一个`BattleRound`，该类不需要开发者实现，所有的`BattlePlayable`由该类直接管理。



Each round is represented by a `BattleRound` class. This class doesn't need to be implemented by developers; it directly manages all the `BattlePlayable` instances.

### BattleComponents

#### BuffRecorder

记录了buff的基本信息，比较重要的是Template，其中记录了buff的执行逻辑。



It records the basic information of buffs, and the most important part is the "Template," where the execution logic of the buff is stored.

#### SkillTemplate

记录了技能或者buff的执行逻辑，由多个事件组成。举个例子《生命吸取》技能中，可以分为两个事情，扣血，加血。

* `ProcedureFunctions` - 技能的一系列逻辑，注意，此处的逻辑是对每个目标的逻辑，如果有多个目标，每个目标都会走一次这个逻辑。每个事件会会得到前一个事件的执行结果，比如伤害等数值。
* `OnLoadRequest` - 技能被加载的时候，额外需要做的逻辑，比如，需要选择释放目标，需要提前获取一些信息等。
* `OnProcedureFunctionsEndCallBacks` - 技能释放后的一系列逻辑，此处会获得所有目标的执行结果，简单地说，所有数据在这里都能获取。
* `OnDestroyCallBacks` - 逻辑结束之后的析构，可以用于处理buff取消的逻辑。
* `BuffCallBacks` - Buff的逻辑




It encapsulates the execution logic of skills or buffs, consisting of multiple events. For example, in the skill "Life Drain," it can be broken down into two actions: dealing damage and restoring health.

- `ProcedureFunctions` - A sequence of logic for the skill, specifically per target. If there are multiple targets, each target's logic will be executed separately. Each event will receive the execution results of the previous event, such as damage values.
- `OnLoadRequest` - Additional logic required when the skill is loaded, such as selecting target(s) or gathering preliminary information.
- `OnProcedureFunctionsEndCallBacks` - A sequence of logic after the skill is executed. Here, you can access the execution results of all targets. In simple terms, all relevant data can be obtained here.
- `OnDestroyCallBacks` - Destructive logic after the execution ends. This can be used to handle logic for canceling buffs.
- `BuffCallBacks` - Logic for buffs.

#### SkillResult

用于记录逻辑中每个小事件的执行结果，会被发往下一个事件。



Used to record the execution results of each small event within the logic, which will be passed on to the next event in the sequence.

### Playable Implementation Example

日后再加

See you later
