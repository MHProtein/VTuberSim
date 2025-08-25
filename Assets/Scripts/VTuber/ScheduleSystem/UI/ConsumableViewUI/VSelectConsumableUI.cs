using System;
using UnityEngine;
using UnityEngine.EventSystems;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.UI;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VSelectcConsumableUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public VConsumable Card => _consumableUI.consumable;
        private VConsumableUI _consumableUI;
        private bool _selectable = true;
        private Action<VSelectcConsumableUI> _selectAction;
        
        public void Initialize(VConsumableUI consumableUI, bool selectable, Action<VSelectcConsumableUI> selectAction)
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
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.dragging)
                return;
            if (!_selectable)
                return;
            _consumableUI.background.color = Color.grey;
            _selectAction?.Invoke(this);
        }
    }
}