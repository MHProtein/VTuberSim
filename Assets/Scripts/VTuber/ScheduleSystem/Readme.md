

7.18 update
事件的代码存在Assets/Resources/Event 中
按钮通过prefab自动生成，计划表的可视化还在研究中。目前按钮的反馈见log。ui之前接触的不多花了不少时间还请原谅ORZ


---

# 周表系统代码说明文档

## 概述

该系统用于管理和执行玩家一周的游戏事件安排，分为两个主要状态：

* **规划阶段（Planning）**：玩家从可选事件列表中选择事件填入周表的事件槽，规划未来一周的活动安排。
* **执行阶段（Executing）**：系统按顺序执行周表中的事件，根据玩家状态判断是否能执行，若不能执行则进入失败事件处理。

---

## 核心概念和结构

### 1. 时间段（TimeOfDay）

一天被分为三个时间段：

* Morning（早上）
* Afternoon（下午）
* Evening（晚上）

在一天内，玩家可为每个时间段安排一个事件。

### 2. 事件类型（ScheduleEventType）

事件分为四类：

* Live（直播类）
* Practice（练习类）
* Assist（协助类）
* Recovery（回复类）

### 3. 执行失败原因（FailureReason）

* InsufficientStamina（体力不足）
* Other（其他原因）

### 4. 阶段状态（PhaseState）

* Planning（规划阶段）
* Executing（执行阶段）

---

## 代码模块说明

### VTuber.ScheduleSystem.Core

#### ScheduleEnums.cs

定义了系统中所有枚举类型，包括时间段、事件类型、失败原因和当前阶段状态。

#### PlayerStatus.cs

玩家的当前状态，包含体力（Stamina）、经验（Experience）等简化字段。

---

### VTuber.ScheduleSystem.Event

#### ScheduleEventConfiguration.cs

事件的配置文件（ScriptableObject），定义事件的名称、描述、图标、类型、体力消耗、情绪和技能经验奖励等信息。
配置用于创建运行时事件（ScheduleEvent）。

#### ScheduleEvent.cs

事件的运行时类，包含执行判断和执行逻辑。
主要方法：

* `CanExecute(PlayerStatus player)` 判断玩家是否满足执行条件（如体力是否足够）。
* `Execute(PlayerStatus player)` 执行事件，消耗体力等。

---

### VTuber.ScheduleSystem.Schedule

#### WeeklySchedule.cs

管理一周7天的事件安排，内含7个DaySchedule实例。
主要接口：

* `SetEvent(int dayIndex, TimeOfDay timeOfDay, ScheduleEvent evt)` 为某天某时间段设置事件。
* `GetEvent(int dayIndex, TimeOfDay timeOfDay)` 获取对应事件。

#### DaySchedule.cs

管理一天3个时间段的事件，使用字典保存TimeOfDay与ScheduleEvent的映射。

---

### VTuber.ScheduleSystem.Phase

#### PhaseConfiguration.cs

时间段阶段的配置（ScriptableObject），可以定义自动执行标记、描述等（可扩展）。

#### PhaseData.cs

某天某时间段的事件快照，保存执行时使用。

---

### VTuber.ScheduleSystem.Runtime

#### ScheduleExecutor.cs

调度器，负责依次执行一周内每天所有时间段的事件。

关键逻辑：

* `ExecuteAll()` 执行整周排程。
* `ExecuteDay(int dayIndex)` 执行一天的所有时间段。
* `ExecutePhase(PhaseData phase)` 执行单个时间段的事件，判断体力是否足够，执行成功或失败。

失败时，系统会打印日志，并可在后续实现中扩展失败事件处理逻辑。

---

### 测试示例（ScheduleTestRunner.cs）

一个简单的MonoBehaviour测试脚本，模拟一周的事件执行，展示体力和经验变化。

---

## 实现说明（针对策划案）

1. **规划阶段**

    * 玩家通过UI从“可选事件表”选择事件填充到周表（WeeklySchedule）中对应的天和时间段。
    * 系统内部使用`WeeklySchedule`和`DaySchedule`存储玩家安排。
    * 可实现事件拖拽和替换功能。

2. **执行阶段**

    * 进入执行阶段时，调用`ScheduleExecutor.ExecuteAll()`开始事件执行。
    * 对每个事件执行前调用`ScheduleEvent.CanExecute()`判断条件。
    * 如果条件不满足（如体力不足），跳转到失败事件逻辑（当前只打印警告，可扩展失败事件表）。
    * 执行事件时调用`ScheduleEvent.Execute()`，进行体力消耗、经验奖励等处理。

3. **事件失败处理**

    * 当某事件执行失败时，系统应触发对应失败事件（如体力不足事件）。
    * 失败事件根据玩家选择，可以决定回到规划阶段重新安排，或继续执行剩余事件。

4. **周结束处理**

    * 执行完7天所有事件后，重置周表数据，回到规划阶段准备下一周。

5. **事件扩展**

    * 事件可拓展属性，如经验、情绪加成、资源奖励、概率失败等。
    * 通过`ScheduleEventConfiguration`和`ScheduleEvent`支持。


