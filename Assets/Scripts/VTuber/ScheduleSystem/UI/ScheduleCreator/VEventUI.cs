using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Events;

namespace VTuber.ScheduleSystem.UI
{
    public class VEventUI : VUIBehaviour, IPointerEnterHandler, IPointerDownHandler,
        IPointerExitHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
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
        
        protected override void Awake()
        {
            parentSlots = new List<VScheduleSlot>();
            parentBeforeDrag = new List<VScheduleSlot>();
            _interactable = true;
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

        public void Initialize(VScheduleEventConfiguration eventData, VScheduleSlot slot)
        {
            _event = eventData.CreateEvent();
            icon.sprite = VRaisingUI.Instance.GetIcon(eventData.icon);
            background.color = eventData.backgroundColor;
            _bgColor = eventData.backgroundColor;
            icon.transform.localScale = Vector3.zero;
            background.transform.localScale = Vector3.zero;
            Tween.Scale(icon.transform, new Vector3(1, 1, 1), 0.3f);
            Tween.Scale(background.transform, new Vector3(1, eventData.Duration, 1), 0.3f);

            if(slot.FindPosition(_event.Duration, initOffset.y, out var parents, out var transformParent, out var position))
            {
                SetNewParents(parents, transformParent, position, false);
            }
        }

        public void InitializeDrag(VScheduleEventConfiguration eventData,
            Vector2 initPosition)
        {
            _event = eventData.CreateEvent();
            icon.sprite = VRaisingUI.Instance.GetIcon(eventData.icon);
            background.color = eventData.backgroundColor;
            _bgColor = eventData.backgroundColor;
            _initPosition = initPosition;
            icon.raycastTarget = false;
            transform.SetParent(VSingletonMonobehaviour<VScheduleUIHelper>.Instance.ScheduleUIRect);
            _isSelected = true;
            icon.transform.localScale = Vector3.zero;
            background.transform.localScale = Vector3.zero;
            Tween.Scale(icon.transform, new Vector3(1, 1, 1), 0.3f);
            Tween.Scale(background.transform, new Vector3(1, eventData.Duration, 1), 0.3f);

            transform.SetAsLastSibling();
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
        }
        
        public void SetNewParents(List<VScheduleSlot> parents, Transform transformParent, Vector2 position, bool shouldTween)
        {
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
            transform.SetParent(VSingletonMonobehaviour<VScheduleUIHelper>.Instance.ScheduleUIRect);
            //transform.position = position;
        }
        
        public bool TryPlaceEvent(List<RaycastResult> results)
        {
            foreach (var result in results)
            {
                var slot = result.gameObject.GetComponent<VScheduleSlot>();
                if (slot is not null)
                {
                    if(slot.FindPosition(_event.Duration, initOffset.y, out var parents, out var transformParent, out var position))
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
                Vector3 mousePosition = Input.mousePosition  + (Vector3)initOffset;
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
                    Despawn();
                    return;
                }

                foreach (var result in results)
                {
                    var slot = result.gameObject.GetComponent<VScheduleSlot>();
                    if (slot is not null)
                    {
                        SetParentBeforeDrag();
                        return;
                    }
                }
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
                initOffset = transform.position - Input.mousePosition;
                _isSelected = true;
                
                foreach (var parent in parentSlots)
                {
                    parent.RemoveItem();
                }
                transform.SetAsLastSibling();
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