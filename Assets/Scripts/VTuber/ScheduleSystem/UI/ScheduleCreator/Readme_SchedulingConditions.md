# **日程规划条件 (Scheduling Conditions) UI 实现文档**

## **1\. 简介**

本文档旨在详细说明“日程规划条件”相关UI功能的实现逻辑，作为技术参考以便于后期维护和功能迭代。  
本文档涵盖以下两个核心功能：

* **拖拽显示提示**：**当玩家开始拖拽一个事件UI时**，显示其规划条件所需的位置提示，并附带呼吸灯动画。  
* **拖拽实时高亮**：当拖拽事件UI时，实时检测鼠标下的日程表槽位是否满足放置条件，并显示高亮反馈。

## **2\. 功能一：拖拽事件显示位置提示**

### **2.1 目标**

**当玩家开始拖拽一个 VEventUI（事件UI）时**，如果该事件包含日程规划条件 (SchedulingCondition)，则在UI周围显示其所需检测的位置（如上、下、左、右），并播放呼吸灯（透明度渐变）动画。

### **2.2 相关脚本**

* VEventUI.cs: 核心交互脚本，负责处理鼠标的 **OnPointerDown (在日程表上拾起), InitializeDrag (从列表拖出) 和 OnEndDragging (放下) 事件**。  
* VSchedulingCondition.cs: 数据脚本，提供事件所需的 PositionPattern（位置模式）。  
* IndicatorAnimationManager.cs: 动画管理脚本，负责统一驱动呼吸灯效果。

### **2.3 Unity 预制件设置 (VEventUI Prefab)**

* **层级结构**：在 VEventUI 预制件的根目录下，创建一个空的 GameObject，命名为 ConditionIndicatorsContainer。  
* **指示器**：在 ConditionIndicatorsContainer 下，创建四个 Image 类型的UI元素，分别命名为 UpIndicator, DownIndicator, LeftIndicator, RightIndicator，并调整到事件UI的相应外侧位置。  
* **默认状态**：将 ConditionIndicatorsContainer 默认设置为不激活（禁用）。  
* **组件添加**：  
  * 为 UpIndicator, DownIndicator, LeftIndicator, RightIndicator 这四个 GameObject 分别添加 Canvas Group 组件。  
  * 为父对象 ConditionIndicatorsContainer 添加 IndicatorAnimationManager.cs 脚本。  
* **脚本引用**：选中 VEventUI 根对象，在 VEventUI.cs 脚本的 Inspector 面板中，将 ConditionIndicatorsContainer 及其四个子指示器 GameObject 拖拽到对应的 \[SerializeField\] 字段上。

### **2.4 实现逻辑**

#### **2.4.1 数据层 (VSchedulingCondition.cs)**

为 VSchedulingCondition 类添加一个公开属性 PositionPattern，以暴露私有的 \_positionPattern 字段，使UI脚本能读取到位置模式（如 UD, LR, UDLR 等）。

// In VSchedulingCondition.cs  
public VSchedulingConditionPositionPatterns PositionPattern \=\> \_positionPattern;  
private VSchedulingConditionPositionPatterns \_positionPattern;

#### **2.4.2 动画层 (IndicatorAnimationManager.cs)**

此脚本挂载在 ConditionIndicatorsContainer 上，统一管理所有子指示器的动画，以提高性能和可维护性。

* Awake(): GetComponentsInChildren\<CanvasGroup\>(true, \_indicatorCanvasGroups)，获取所有子指示器的 CanvasGroup 组件并缓存到列表中。  
* OnEnable(): SetAlpha(minAlpha)，在每次容器被激活时，重置所有指示器的透明度，以同步动画。  
* Update():  
  * 使用 Mathf.PingPong 计算一个在 minAlpha 和 maxAlpha 之间来回变化的当前透明度值。  
  * SetAlpha(currentAlpha)，将计算出的透明度值应用到列表中的所有 CanvasGroup 组件上。  
* **参数可调**：minAlpha, maxAlpha 和 speed 均为公开 \[SerializeField\] 字段，可在 Inspector 窗口中实时调整呼吸灯效果。

#### **2.4.3 UI交互层 (VEventUI.cs)**

**VEventUI 负责在正确的时间（开始拖拽）显示或（结束拖拽）隐藏指示器容器，动画则由 IndicatorAnimationManager 自动处理。**

* Awake(): 确保 conditionIndicatorsContainer.SetActive(false)，使其默认隐藏。  
* **OnPointerDown(PointerEventData eventData) (在日程表上拾起)**:  
  * 当检测到是鼠标左键按下，并且事件可交互时。  
  * 在设置 \_isSelected \= true 后，**立即调用辅助方法 ShowConditionIndicators() 来显示指示器**。  
* **InitializeDrag(VScheduleEvent e, Vector2 initPosition) (从列表拖出)**:  
  * 当事件从创建列表被拖出时（这是拖拽的另一种起点）。  
  * 在此方法的末尾，**同样调用辅助方法 ShowConditionIndicators()**，以确保两种拖拽来源的行为一致。  
* **OnEndDragging() (拖拽结束)**:  
  * 此方法在鼠标松开、拖拽操作结束时被调用。  
  * **在方法的最开头，立即调用 conditionIndicatorsContainer.SetActive(false)**。  
  * 这能确保无论事件是否成功放置，指示器都会被正确隐藏。  
* **ShowConditionIndicators() (私有辅助方法)**:  
  * **这是一个新封装的方法，包含了所有显示逻辑**。  
  * 检查事件数据 \_event?.SchedulingCondition 是否存在。  
  * 获取 pattern \= \_event.SchedulingCondition.PositionPattern。  
  * 如果 pattern \== VSchedulingConditionPositionPatterns.None，则不执行任何操作。  
  * 如果 pattern 有效，则 conditionIndicatorsContainer.SetActive(true)。  
  * 使用 switch (pattern) 语句，根据 UD, LR, UDLR, All 等不同情况，分别调用 upIndicator.SetActive(true), leftIndicator.SetActive(true) 等，来激活对应的方向指示器。

## **3\. 功能二：拖拽事件实时高亮槽位**

### **3.1 目标**

当玩家拖拽一个 VEventUI 在日程表（由 VScheduleSlot 组成）上移动时，鼠标下方的 VScheduleSlot 必须实时检查自己是否满足该事件的 SchedulingCondition。如果满足，该槽位显示一个高亮效果；如果不满足或鼠标离开，则高亮消失。

### **3.2 相关脚本**

* VScheduleSlot.cs: 核心被检测对象，负责实现高亮逻辑。  
* VEventUI.cs: 核心拖拽对象，负责在 Update 中触发检测。  
* VSchedulingCondition.cs: 提供 IsTrue(VCharacter character, VScheduleSlot slot) 方法用于逻辑判断。

### **3.3 Unity 预制件设置 (VScheduleSlot Prefab)**

* **层级结构**：在 VScheduleSlot 预制件下，创建一个新的 Image 元素，命名为 ConditionHighlight。  
* **样式设置**：将其设置为一个半透明的彩色边框或叠加背景，用于表示“条件满足”。  
* **默认状态**：将 ConditionHighlight 默认设置为不激活（禁用）。  
* **脚本引用**：选中 VScheduleSlot 根对象，在 VScheduleSlot.cs 脚本的 Inspector 面板中，新增一个 \[SerializeField\] 字段，并将 ConditionHighlight 游戏对象拖拽到该字段上。

// In VScheduleSlot.cs  
\[Header("Scheduling Condition UI")\]  
\[Tooltip("The UI element to show when a scheduling condition is met during drag")\]  
\[SerializeField\] private GameObject conditionHighlight;

### **3.4 实现逻辑**

#### **3.4.1 槽位层 (VScheduleSlot.cs)**

VScheduleSlot 负责“显示”或“隐藏”自身的高亮状态。

* Awake(): 确保 conditionHighlight.SetActive(false)。  
* **新增 CheckAndHighlight(VScheduleEvent eventBeingDragged) 方法**:  
  * 检查 conditionHighlight 是否为空，或 eventBeingDragged 及其 SchedulingCondition 是否为空，是则返回。  
  * 调用 eventBeingDragged.SchedulingCondition.IsTrue(\_scheduleUI.Character, this) 执行核心逻辑判断。  
  * 根据返回的 bool 值（isConditionMet），调用 conditionHighlight.SetActive(isConditionMet) 来显示或隐藏高亮。  
* **新增 HideHighlight() 方法**:  
  * conditionHighlight.SetActive(false)，提供一个外部调用的接口来强制隐藏高亮。

#### **3.4.2 事件UI层 (VEventUI.cs)**

VEventUI 在拖拽时，负责“触发”槽位的检测。

* **新增字段**: private VScheduleSlot \_lastHoveredSlot \= null; 用于跟踪上一个鼠标悬停的槽位，以避免重复调用和处理鼠标移开的逻辑。  
* **修改 UpdateImpl() 中的 if (\_isSelected) 拖拽逻辑**:  
  * Vector3 mousePosition \= ... (保持原有的UI跟随鼠标逻辑)。  
  * var results \= VSingletonMonobehaviour\<VScheduleUIHelper\>.Instance.RaycastFromMouse() (射线检测)。  
  * 遍历 results 找到第一个 VScheduleSlot，存入 currentHoveredSlot。  
  * **核心逻辑**：检查 if (currentHoveredSlot \!= \_lastHoveredSlot)。  
  * 如果 currentHoveredSlot 发生了变化（意味着鼠标移动到了新的槽位或移出了槽位）：  
    * \_lastHoveredSlot?.HideHighlight()：命令**上一个**槽位隐藏其高亮。  
    * currentHoveredSlot?.CheckAndHighlight(\_event)：命令**新**槽位进行检测并（如果满足条件）显示高亮。  
    * \_lastHoveredSlot \= currentHoveredSlot：更新跟踪变量。  
* **修改 OnEndDragging() 和 UpdateImpl 中的 GetMouseButtonUp 逻辑**:  
  * 在停止拖拽的逻辑（如 OnEndDragging() 方法的开头）中，必须调用 \_lastHoveredSlot?.HideHighlight() 并将 \_lastHoveredSlot \= null。  
  * 这确保了在玩家松开鼠标时，所有槽位的高亮都会被正确清除。

## **4\. 总结与改进方向**

* **当前状态**：已实现需求文档中的两个核心视觉反馈功能。  
* **调试**：**功能一（拖拽提示）** 可通过在 VEventUI 预制件中强制激活 ConditionIndicatorsContainer 来常驻显示，以便调试呼吸灯效果。  
* **可改进点**：  
  * **性能**：VEventUI 在 Update 中进行射线检测。如果日程表非常大，可考虑使用 IPointerEnterHandler 等接口（在 VScheduleSlot 上实现）来替代 VEventUI 的主动检测，但这会增加 VScheduleSlot 的逻辑复杂度。（*注：当前实现对于功能二是必须的，因为IPointer事件在被拖拽物遮挡时可能失效。*）  
  * **效果**：高亮和提示动画可以替换为更复杂的 Shader 或 DOTween 动画，以增强视觉表现力。