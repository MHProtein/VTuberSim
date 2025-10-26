using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.SE;
using VTuber.ScheduleSystem.Events;

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
        [Tooltip("上方向指示器")]
        [SerializeField] private GameObject upIndicator;
        [Tooltip("下方向指示器")]
        [SerializeField] private GameObject downIndicator;
        [Tooltip("左方向指示器")]
        [SerializeField] private GameObject leftIndicator;
        [Tooltip("右方向指示器")]
        [SerializeField] private GameObject rightIndicator;
        
        
        [HideInInspector] public Vector2 initOffset;
        private bool _isSelected;
        private bool _interactable;
        
        public VScheduleEvent Event => _event;
        private VScheduleEvent _event;
        
        private List<VScheduleSlot> parentBeforeDrag;
        private Vector2 _lastPosition;

        private List<VScheduleSlot> parentSlots;

        private Vector2 _initPosition;
        
        private Color _bgColor;

        public bool IsFixed => _isFixed;
        private bool _isFixed = false;
        private Camera _camera;
        private List<VScheduleSlot> _disposeSlots;
        private bool _disposable = true;
        private Transform _disposePosition;
        
        private RectTransform _rectTransform;
        private bool _hasInSchedule = false;
        
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
            icon.sprite = VResourcesManager.Instance.TryGetSprite(e.Icon);
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
            var rectTransform = (background.transform as RectTransform);
            Tween.UISizeDelta(rectTransform, new Vector2(rectTransform.rect.width, rectTransform.rect.height * e.Duration), 0.3f);

            if (slot.FindPosition((int)e.EventID, _event.Duration, initOffset.y, out var parents, out var transformParent,
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
            icon.sprite = VResourcesManager.Instance.TryGetSprite(e.Icon);
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
            var rectTransform = (background.transform as RectTransform);
            Tween.UISizeDelta(rectTransform, new Vector2(rectTransform.rect.width, rectTransform.rect.height * e.Duration), 0.3f);

            transform.SetAsLastSibling();

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventUISelected, new Dictionary<string, object>()
            {
                { "Event", _event }
            });
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
        }
        
        public void SetParentBeforeDrag()
        {
            parentSlots = parentBeforeDrag;

            Tween.Position(transform, _lastPosition, 0.2f);
            //transform.position = _lastPosition;
            foreach (var parent in parentSlots)
            {
                parent.SetItem(this);
            }
            
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventUIPlaced, new Dictionary<string, object>()
            {
                { "Event", _event }
            });
        }
        
        public void SetParentDisposeSlot()
        {
            parentSlots = _disposeSlots;

            Tween.Position(transform, _disposePosition.position, 0.2f);
            //transform.position = _lastPosition;
            foreach (var parent in parentSlots)
            {
                parent.SetItem(this);
            }
            transform.SetParent(_disposePosition);
            
        }

        public void SetNewParents(List<VScheduleSlot> parents, Transform transformParent, Vector2 position, bool shouldTween)
        {
            _hasInSchedule = true;
            parentBeforeDrag = parentSlots;
            parentSlots = parents;
            _lastPosition = position;
            
            foreach (var parent in parentSlots)
            {
                parent.SetItem(this);
            }
            if(shouldTween)
                Tween.Position(transform, position, 0.2f);
            else
            {
                transform.position = position;
            }
            transform.SetParent(transformParent);
            //transform.position = position;
            if(!_event.IsSpecialEvent)
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventUIPlaced, new Dictionary<string, object>()
                {
                    { "Event", _event }
                });
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Raising_PlaceEvent);
        }
        
        public bool TryPlaceEvent(List<RaycastResult> results)
        {
            foreach (var result in results)
            {
                var slot = result.gameObject.GetComponent<VScheduleSlot>();
                if (slot is not null)
                {
                    if(slot.FindPosition((int)_event.EventID, _event.Duration, initOffset.y, out var parents, out var transformParent, out var position))
                    {
                        SetNewParents(parents, transformParent, position, true);
                        return true;
                    }
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
                Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition) + (Vector3)initOffset;
                mousePosition.z = 0;
                transform.position = mousePosition;
                
                var results = VSingletonMonobehaviour<VScheduleUIHelper>.Instance.RaycastFromMouse();
                
                foreach (var result in results)
                {
                    var slot = result.gameObject.GetComponent<VScheduleSlot>();
                    if (slot is not null)
                    {
                        slot.SetIndicator(_event.Duration, initOffset.y);
                        break;
                    }
                }
            }

            if (_isSelected && Input.GetMouseButtonUp(0))
            {
                OnEndDragging();
            }
        }

        public void OnEndDragging()
        {
            _isSelected = false;
            icon.raycastTarget = true;
            var results = VSingletonMonobehaviour<VScheduleUIHelper>.Instance.RaycastFromMouse();
            if (TryPlaceEvent(results))
            {
                icon.raycastTarget = true;
                _isSelected = false;
                return;
            }
      
            if(parentSlots is null || parentSlots.Count == 0)
            {
                if (_disposable)
                {
                    Despawn();
                    return;
                }
                else
                {
                    SetParentBeforeDrag();
                    return;
                }
            }

            if ((!_disposable && _hasInSchedule) || _disposable)
            {
                foreach (var result in results)
                {
                    var slot = result.gameObject.GetComponent<VScheduleSlot>();
                    if (slot is not null)
                    {
                        SetParentBeforeDrag();
                        return;
                    }
                }
            }

            if (!_disposable && _disposeSlots is not null)
            {
                SetParentDisposeSlot();
            }
            else
            {
                Despawn();
            }
        }

        public void Despawn()
        {
            foreach (var slot in parentSlots)
            {
                slot.RemoveItem();
            }
            Tween.Scale(transform, Vector3.one * 0.2f, 0.28f);
            Tween.Position(transform, _initPosition, 0.3f)
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
            
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventUIPlaced, new Dictionary<string, object>()
            {
                { "Event", _event }
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

            // --- MODIFIED LOGIC START ---
            if (_event?.SchedulingCondition == null || conditionIndicatorsContainer == null) return;


            var pattern = _event.SchedulingCondition.PositionPattern;
            if (pattern == VSchedulingConditionPositionPatterns.None) return;

            // Activate the main container
            conditionIndicatorsContainer.SetActive(true);
            
            // Deactivate all indicators first to reset state
            upIndicator?.SetActive(false);
            downIndicator?.SetActive(false);
            leftIndicator?.SetActive(false);
            rightIndicator?.SetActive(false);
            
            // Activate indicators based on the pattern
            switch (pattern)
            {
                case VSchedulingConditionPositionPatterns.UD:
                    upIndicator?.SetActive(true);
                    downIndicator?.SetActive(true);
                    break;
                case VSchedulingConditionPositionPatterns.LR:
                    leftIndicator?.SetActive(true);
                    rightIndicator?.SetActive(true);
                    break;
                case VSchedulingConditionPositionPatterns.UDLR:
                case VSchedulingConditionPositionPatterns.All: // Treat 'All' as the 4 cardinal directions
                    upIndicator?.SetActive(true);
                    downIndicator?.SetActive(true);
                    leftIndicator?.SetActive(true);
                    rightIndicator?.SetActive(true);
                    break;
            }
            // --- MODIFIED LOGIC END ---
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
        
    }
}