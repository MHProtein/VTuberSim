using System;
using UnityEngine;
using UnityEngine.EventSystems;
using VTuber.Consumable;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VSelectConsumableUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private VConsumableUI _consumableUI;
        private bool _selectable = true;
        private Action<VSelectConsumableUI> _selectAction;
        public VConsumable Consumable => _consumableUI.consumable;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.dragging)
                return;
            if (!_selectable)
                return;
            _consumableUI.background.color = Color.grey;
            _selectAction?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        public void Initialize(VConsumableUI consumableUI, bool selectable, Action<VSelectConsumableUI> selectAction)
        {
            _consumableUI = consumableUI;
            _selectable = selectable;
            _selectAction = selectAction;
            consumableUI.descriptionObject.SetActive(true);
        }

        public void SetSelectable(bool selectable)
        {
            _selectable = selectable;
        }

        public void UnSelect()
        {
            _consumableUI.background.color = Color.white;
        }
    }
}