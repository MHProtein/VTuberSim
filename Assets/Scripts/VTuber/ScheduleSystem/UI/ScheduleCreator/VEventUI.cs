﻿using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.SE;
using VTuber.ScheduleSystem.Events;
using VTuber.Core.Managers; // 引入 VDataManager
using VTuber.ScheduleSystem.Core; // 引入 VScheduleEventConfiguration

namespace VTuber.ScheduleSystem.UI
{
    public class VEventUI : VUIBehaviour, IPointerEnterHandler, IPointerDownHandler,
        IPointerExitHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
        [SerializeField] private Image nameIcon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Image costIcon;
        
        //  Position Pattern for UI purposes
        [Header("日程规划条件UI")]
        [Tooltip("用于容纳所有位置指示器的父对象")]
        [SerializeField] private GameObject conditionIndicatorsContainer; 
        [Tooltip("上方向指示器的 Image 组件")]
        [SerializeField] private Image upIndicatorImage;
        [Tooltip("下方向指示器的 Image 组件")]
        [SerializeField] private Image downIndicatorImage;
        [Tooltip("左方向指示器的 Image 组件")]
        [SerializeField] private Image leftIndicatorImage;
        [Tooltip("右方向指示器的 Image 组件")]
        [SerializeField] private Image rightIndicatorImage;
        
        [Header("指示器文本")]
        [Tooltip("请确保这些数组/字段对应 Up, Down, Left, Right 的 Text 组件")]
        [SerializeField] private TMP_Text upText;
        [SerializeField] private TMP_Text downText;
        [SerializeField] private TMP_Text leftText;
        [SerializeField] private TMP_Text rightText;
        
        
        [HideInInspector] public Vector2 initOffset;
        
        
        [Header("高亮显示")]
        [Tooltip("用于“作为邻居”满足条件时的高亮")]
        [SerializeField] private GameObject neighborHighlightVisual; // 拖入你新创建的UI

        private Color _bgColor;

        private Vector2 _initPosition;
        private bool _interactable;
        private bool _isSelected;
        private Vector2 _lastPosition;


        private List<VScheduleSlot> parentBeforeDrag;

        private List<VScheduleSlot> parentSlots;

        public VScheduleEvent Event => _event;
        private VScheduleEvent _event;


        public bool IsFixed => _isFixed;
        private bool _isFixed = false;
        private Camera _camera;
        private List<VScheduleSlot> _disposeSlots;
        private bool _disposable = true;
        private Transform _disposePosition;
        
        private RectTransform _rectTransform;
        private bool _hasInSchedule = false;
        
        // Add this field to your VEventUI class to keep track of the last slot hovered over.
        private VScheduleSlot _lastHoveredSlot = null;
        
        // 用于缓存当前高亮的所有邻居，以便拖拽结束时清除
        private static List<VEventUI> _highlightedNeighbors = new List<VEventUI>();
        
        protected override void Awake()
        {
            parentSlots = new List<VScheduleSlot>();
            parentBeforeDrag = new List<VScheduleSlot>();
            _interactable = true;
            _camera = Camera.main;
            _rectTransform = GetComponent<RectTransform>();

            if (conditionIndicatorsContainer != null)
            {
                conditionIndicatorsContainer.SetActive(false);
            }
            neighborHighlightVisual?.SetActive(false); // 确保默认隐藏
        }
        //用于打开/关闭条件高亮
        public void SetNeighborHighlight(bool value)
        {
            neighborHighlightVisual?.SetActive(value);
        }
        // public void InitializeMove(EventData eventData, Vector2 initPosition)
        // {
        //     _eventData = eventData;
        //     icon.sprite = eventData.icon;
        //     background.color = eventData.backgroundColor;
        //     _initPosition = initPosition;
        //     icon.raycastTarget = false;
        //     transform.SetParent(VSingletonMonobehaviour<VScheduleUIHelper>.Instance.CanvasRect);
        //     isSelected = true;
        //     canSetParent = true;
        //     icon.transform.localScale = Vector3.zero;
        //     background.transform.localScale = Vector3.zero;
        //     Tween.Scale(icon.transform, new Vector3(1, 1, 1), 0.3f);
        //     Tween.Scale(background.transform, new Vector3(1, eventData.height, 1), 0.3f);
        // }

        public void SetColorGrey()
        {
            background.color = Color.grey;
        }

        public void SetColorOriginal()
        {
            background.color = _bgColor;
            icon.color = Color.white;
        }

        public void SetFixed(bool isFixed)
        {
            _isFixed = isFixed;
            _interactable = isFixed;
        }

        public void SetInteractive(bool interactable)
        {
            _interactable = interactable;
        }

        public void Initialize(VScheduleEvent e, VScheduleSlot slot, bool disposable, Transform parent = null)
        {
            _event = e;
            icon.sprite = e.Icon;
            background.color = e.BackgroundColor;
            _bgColor = background.color;
            icon.transform.localScale = Vector3.zero;
            background.transform.localScale = Vector3.zero;
            nameText.text = e.EventName;
            nameIcon.color = e.BackgroundColor;

            switch (e.CostType)
            {
                case VEventCostType.Stamina:
                    costIcon.sprite = VResourcesManager.Instance.TryGetSprite("Icon_Stamina");
                    break;
                case VEventCostType.Money:
                    costIcon.sprite = VResourcesManager.Instance.TryGetSprite("Icon_Money");
                    break;
            }

            costText.text = e.Cost.ToString();

            Tween.Scale(icon.transform, Vector3.one, 0.3f);
            Tween.Scale(background.transform, Vector3.one, 0.3f);
            var rectTransform = background.transform as RectTransform;
            Tween.UISizeDelta(rectTransform,
                new Vector2(rectTransform.rect.width, rectTransform.rect.height * e.Duration), 0.3f);

            if (slot.FindPosition((int)e.EventID, Event.Duration, initOffset.y, out var parents,
                    out var transformParent,
                    out var position))
            {
                SetNewParents(parents, transformParent, position,
                    false);

                if (parent is not null)
                {
                    transform.SetParent(parent);
                    transform.SetAsLastSibling();
                }

                parentBeforeDrag.Clear();
                _disposeSlots = parentSlots;
                _disposePosition = parent;
            }

            _disposable = disposable;
            _hasInSchedule = false;
        }

        public void InitializeDrag(VScheduleEvent e, Vector2 initPosition)
        {
            _event = e;
            icon.sprite = e.Icon;
            background.color = e.BackgroundColor;
            _bgColor = background.color;
            _initPosition = initPosition;
            icon.raycastTarget = false;
            transform.SetParent(VSingletonMonobehaviour<VScheduleUIHelper>.Instance.EventParent);
            _isSelected = true;
            icon.transform.localScale = Vector3.zero;
            background.transform.localScale = Vector3.zero;
            nameText.text = e.EventName;
            nameIcon.color = e.BackgroundColor;

            switch (e.CostType)
            {
                case VEventCostType.Stamina:
                    costIcon.sprite = VResourcesManager.Instance.TryGetSprite("Icon_Stamina");
                    break;
                case VEventCostType.Money:
                    costIcon.sprite = VResourcesManager.Instance.TryGetSprite("Icon_Money");
                    break;
            }

            costText.text = e.Cost.ToString();

            Tween.Scale(icon.transform, new Vector3(1, 1, 1), 0.3f);
            Tween.Scale(background.transform, Vector3.one, 0.3f);
            var rectTransform = background.transform as RectTransform;
            Tween.UISizeDelta(rectTransform,
                new Vector2(rectTransform.rect.width, rectTransform.rect.height * e.Duration), 0.3f);

            transform.SetAsLastSibling();

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventUISelected, new Dictionary<string, object>
            {
                { "Event", Event }
            });
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
            
            
            // 调用显示指示器
            ShowConditionIndicators();
            // --- 新增逻辑：高亮所有满足条件的事件 ---
            HighlightAllSatisfyingEvents(_event);
            // --- 逻辑结束 ---
        }

        public void SetParentBeforeDrag()
        {
            parentSlots = parentBeforeDrag;

            Tween.Position(transform, _lastPosition, 0.2f);
            //transform.position = _lastPosition;
            foreach (var parent in parentSlots) parent.SetItem(this);

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventUIPlaced, new Dictionary<string, object>
            {
                { "Event", Event }
            });
        }

        public void SetParentDisposeSlot()
        {
            parentSlots = _disposeSlots;

            Tween.Position(transform, _disposePosition.position, 0.2f);
            //transform.position = _lastPosition;
            foreach (var parent in parentSlots) parent.SetItem(this);
            transform.SetParent(_disposePosition);
        }

        public void SetNewParents(List<VScheduleSlot> parents, Transform transformParent, Vector2 position,
            bool shouldTween)
        {
            _hasInSchedule = true;
            parentBeforeDrag = parentSlots;
            parentSlots = parents;
            _lastPosition = position;

            foreach (var parent in parentSlots) parent.SetItem(this);
            if (shouldTween)
                Tween.Position(transform, position, 0.2f);
            else
                transform.position = position;
            transform.SetParent(transformParent);
            //transform.position = position;
            if (!Event.IsSpecialEvent)
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventUIPlaced, new Dictionary<string, object>
                {
                    { "Event", Event }
                });
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Raising_PlaceEvent);
        }

        public bool TryPlaceEvent(List<RaycastResult> results)
        {
            foreach (var result in results)
            {
                var slot = result.gameObject.GetComponent<VScheduleSlot>();
                if (slot is not null)
                    if (slot.FindPosition((int)Event.EventID, Event.Duration, initOffset.y, out var parents,
                            out var transformParent, out var position))
                    {
                        SetNewParents(parents, transformParent, position, true);
                        return true;
                    }
            }

            return false;
        }

        protected override void UpdateImpl()
        {
            if (!_interactable)
                return;
            base.UpdateImpl();
            if (_isSelected)
            {
                // This part remains the same: move the UI with the mouse
                Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition) + (Vector3)initOffset;
                mousePosition.z = 0;
                transform.position = mousePosition;

                // --- NEW AND IMPROVED LOGIC FOR REAL-TIME HIGHLIGHTING ---
                var results = VSingletonMonobehaviour<VScheduleUIHelper>.Instance.RaycastFromMouse();
                VScheduleSlot currentHoveredSlot = null;

                // Find the slot currently under the mouse
                foreach (var result in results)
                {
                    var slot = result.gameObject.GetComponent<VScheduleSlot>();
                    if (slot != null)
                    {
                        currentHoveredSlot = slot;
                        // We also call the existing SetIndicator logic here
                        slot.SetIndicator(_event.Duration, initOffset.y);
                        break;
                    }
                }

                // Check if we have moved to a new slot
                if (currentHoveredSlot != _lastHoveredSlot)
                {
                    // Turn off the highlight on the slot we just left
                    _lastHoveredSlot?.HideHighlight();

                    // Ask the new slot to check the condition and highlight itself if needed
                    currentHoveredSlot?.CheckAndHighlight(_event);

                    // Update the last hovered slot
                    _lastHoveredSlot = currentHoveredSlot;
                }
                // --- END OF NEW LOGIC ---
            }

            if (_isSelected && Input.GetMouseButtonUp(0))
            {
                // When we stop dragging, make sure to clear any active highlight
                _lastHoveredSlot?.HideHighlight();
                _lastHoveredSlot = null;
        
                OnEndDragging();
            }
        }
        
        

        public void OnEndDragging()
        {
            // --- 新增逻辑：在拖拽最开始时，清除所有高亮 ---
            ClearAllSatisfyingEventHighlights();
            // --- 逻辑结束 ---
            // 新增：在拖拽结束时，也隐藏指示器
            if (conditionIndicatorsContainer != null)
            {
                conditionIndicatorsContainer.SetActive(false);
            }
            _isSelected = false;
            icon.raycastTarget = true;
            _lastHoveredSlot?.HideHighlight();
            _lastHoveredSlot = null;
            var results = VSingletonMonobehaviour<VScheduleUIHelper>.Instance.RaycastFromMouse();
            if (TryPlaceEvent(results))
            {
                icon.raycastTarget = true;
                _isSelected = false;
                return;
            }

            if (parentSlots is null || parentSlots.Count == 0)
            {
                if (_disposable)
                {
                    Despawn();
                    return;
                }

                SetParentBeforeDrag();
                return;
            }

            if ((!_disposable && _hasInSchedule) || _disposable)
                foreach (var result in results)
                {
                    var slot = result.gameObject.GetComponent<VScheduleSlot>();
                    if (slot is not null)
                    {
                        SetParentBeforeDrag();
                        return;
                    }
                }

            if (!_disposable && _disposeSlots is not null)
                SetParentDisposeSlot();
            else
                Despawn();
        }

        public void Despawn()
        {
            foreach (var slot in parentSlots) slot.RemoveItem();
            Tween.Scale(transform, Vector3.one * 0.2f, 0.28f);
            Tween.Position(transform, _initPosition, 0.3f)
                .OnComplete(() => { Destroy(gameObject); });

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventUIPlaced, new Dictionary<string, object>
            {
                { "Event", Event }
            });
        }
        
public void OnPointerEnter(PointerEventData eventData)
        {            
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnNotifyEventDescriptionChange,
                new Dictionary<string, object>()
                {
                    {"Name", _event.EventName},
                    {"Description", _event.Description}
                });


        }



        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_interactable || _isFixed)
                return;
            if (!_isSelected && eventData.button
                == PointerEventData.InputButton.Left)
            {
                icon.raycastTarget = false;
                parentBeforeDrag = parentSlots;
                transform.SetParent(VSingletonMonobehaviour<VScheduleUIHelper>.Instance.CanvasRect);
                initOffset = transform.position - _camera.ScreenToWorldPoint(Input.mousePosition);
                _isSelected = true;

                // --- 在这里添加新逻辑 ---
                // 调用显示指示器的方法
                ShowConditionIndicators(); 
                // --- 新逻辑结束 ---

                // --- 新增逻辑：高亮所有满足条件的事件 ---
                HighlightAllSatisfyingEvents(_event);
                // --- 逻辑结束 ---
                
                foreach (var parent in parentSlots)
                {
                    parent.RemoveItem();
                }
                transform.SetAsLastSibling();

                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventUISelected, new Dictionary<string, object>()
                {
                    { "Event", _event }
                });
                VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
            }
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {   
            
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            // // 如果指示器容器存在，就将其设为非激活状态
            // if (conditionIndicatorsContainer != null)
            // {
            //     conditionIndicatorsContainer.SetActive(false);
            // }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnBeginDrag");
        }

        public void OnDrag(PointerEventData eventData)
        {
            
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("EndDrag");
        }
        // VEventUI.cs (在类的任何地方添加这个新方法)
        // 新增：一个私有结构体，用于在方法间传递条件信息
        private struct ConditionTargetInfo
        {
            public string HintText;
            public Color BackgroundColor;
            public bool IsValid;
        }
private void ShowConditionIndicators()
        {
            if (_event?.SchedulingCondition == null || conditionIndicatorsContainer == null) return;

            var condition = _event.SchedulingCondition;
            var pattern = condition.PositionPattern;
            
            if (pattern == VSchedulingConditionPositionPatterns.None) return;

            // 1. 获取包含文本和颜色的信息
            ConditionTargetInfo info = GetConditionTargetInfo(condition);
            if (!info.IsValid) return; // 如果条件无效（如 SameType 但无事件），则不显示

            // 2. 激活容器
            conditionIndicatorsContainer.SetActive(true);
            
            // 重置状态
            upIndicatorImage?.gameObject.SetActive(false);
            downIndicatorImage?.gameObject.SetActive(false);
            leftIndicatorImage?.gameObject.SetActive(false);
            rightIndicatorImage?.gameObject.SetActive(false);
            
            // 3. 根据模式显示指示器，并传入文本和颜色
            switch (pattern)
            {
                case VSchedulingConditionPositionPatterns.UD:
                    ActivateIndicator(upIndicatorImage, upText, info.HintText, info.BackgroundColor);
                    ActivateIndicator(downIndicatorImage, downText, info.HintText, info.BackgroundColor);
                    break;
                case VSchedulingConditionPositionPatterns.LR:
                    ActivateIndicator(leftIndicatorImage, leftText, info.HintText, info.BackgroundColor);
                    ActivateIndicator(rightIndicatorImage, rightText, info.HintText, info.BackgroundColor);
                    break;
                case VSchedulingConditionPositionPatterns.UDLR:
                case VSchedulingConditionPositionPatterns.All:
                    ActivateIndicator(upIndicatorImage, upText, info.HintText, info.BackgroundColor);
                    ActivateIndicator(downIndicatorImage, downText, info.HintText, info.BackgroundColor);
                    ActivateIndicator(leftIndicatorImage, leftText, info.HintText, info.BackgroundColor);
                    ActivateIndicator(rightIndicatorImage, rightText, info.HintText, info.BackgroundColor);
                    break;
            }
        }
        
        // 新增：激活指示器并设置文字的辅助方法
        private void ActivateIndicator(Image indicatorImage, TMP_Text textComponent, string text, Color color)
        {
            if (indicatorImage != null)
            {
                indicatorImage.gameObject.SetActive(true);
                indicatorImage.color = color; 
                
                if (textComponent != null)
                {
                    textComponent.text = text;
                }
            }
        }
        
        // 新增：核心逻辑 - 获取条件描述文字
// 重构 GetConditionHintText 为 GetConditionTargetInfo
        private ConditionTargetInfo GetConditionTargetInfo(VSchedulingCondition condition)
        {
            // 默认值
            string text = "";
            Color color = Color.grey; // 默认灰色
            bool isValid = true;
            
            VScheduleEventConfiguration config = null;

            switch (condition.Type)
            {
                case VSchedulingConditionType.ID:
                    // 使用你提供的 API 逻辑
                    if (condition.IsTargetStream) // (假设 IsTargetStream 存在于 VSchedulingCondition)
                    {
                        config = VDataManager.Instance.GetStreamEventConfigurationByID(condition.TargetID);
                    }
                    else
                    {
                        config = VDataManager.Instance.GetDialogueEventConfigurationByID(condition.TargetID);
                    }
                    
                    if (config != null)
                    {
                        text = config.eventName;
                        color = config.backgroundColor; // 获取配置的颜色
                    }
                    else
                    {
                        text = "Unknown Event";
                    }
                    break;

                case VSchedulingConditionType.SameType:
                    text = "Same Type";
                    color = _event.BackgroundColor; // 直接使用当前拖拽事件的颜色
                    break;
                    
                case VSchedulingConditionType.Type:
                case VSchedulingConditionType.ExcludeType:
                    text = condition.TargetType.ToString();
                    color = Color.grey; // 类型条件太泛，使用中性色
                    break;
                    
                case VSchedulingConditionType.ExcludeID:
                    text = "NOT ID: " + condition.TargetID;
                    color = Color.grey; // 排除条件也用中性色
                    break;
                    
                default:
                    isValid = false;
                    break;
            }

            // 为“排除”条件添加前缀
            if (condition.Type == VSchedulingConditionType.ExcludeType || condition.Type == VSchedulingConditionType.ExcludeID)
            {
                text = "NOT " + text;
            }

            return new ConditionTargetInfo { HintText = text, BackgroundColor = color, IsValid = isValid };
        }
        
        
        
        // 高亮所有满足条件的事件
        private void HighlightAllSatisfyingEvents(VScheduleEvent eventBeingDragged)
        {
            // 清除任何可能残留的高亮
            ClearAllSatisfyingEventHighlights();

            var condition = eventBeingDragged?.SchedulingCondition;
            if (condition == null) return;

            // 我们只关心需要“特定事件”的条件
            if (condition.Type != VSchedulingConditionType.ID && 
                condition.Type != VSchedulingConditionType.Type && 
                condition.Type != VSchedulingConditionType.SameType)
            {
                return;
            }

            // 获取所有事件所在的父节点 (CanvasRect 似乎是最安全的根)
            var eventParent = VSingletonMonobehaviour<VScheduleUIHelper>.Instance.CanvasRect;
            if (eventParent == null) return;

            // 查找所有当前在日程表中的 VEventUI 实例
            VEventUI[] allPlacedEvents = eventParent.GetComponentsInChildren<VEventUI>();

            foreach (var placedEventUI in allPlacedEvents)
            {
                // 排除自己
                if (placedEventUI == this) continue;
                // 排除其他正在被拖拽的（理论上不应该）
                if (placedEventUI._isSelected) continue; 

                var placedEvent = placedEventUI.Event;
                if (placedEvent == null) continue;

                bool conditionMet = false;
                switch (condition.Type)
                {
                    case VSchedulingConditionType.ID:
                        conditionMet = (placedEvent.EventID == condition.TargetID);
                        break;
                    case VSchedulingConditionType.Type:
                        conditionMet = (placedEvent.Type == condition.TargetType);
                        break;
                    case VSchedulingConditionType.SameType:
                        conditionMet = (placedEvent.Type == eventBeingDragged.Type);
                        break;
                }

                if (conditionMet)
                {
                    placedEventUI.SetNeighborHighlight(true);
                    _highlightedNeighbors.Add(placedEventUI); // 存入缓存，以便清除
                }
            }
        }

        // 清除所有高亮
        private void ClearAllSatisfyingEventHighlights()
        {
            foreach (var eventUI in _highlightedNeighbors)
            {
                if(eventUI != null) // 增加安全检查
                    eventUI.SetNeighborHighlight(false);
            }
            _highlightedNeighbors.Clear();
        }
        
    }
}