using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.Consumable
{
    public class VConsumableSlotUI : VUIBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject subMenu;
        [SerializeField] private Button useButton;
        [SerializeField] private Button discardButton;
        [SerializeField] private VConsumableUI consumableUI;
        private VConsumableSlotsUI _slots;
        
        public void Init(VConsumableSlotsUI slots)
        {
            subMenu.SetActive(false);
            _slots = slots;
        }

        public void SetConsumable(VConsumable consumable)
        {
            consumableUI.gameObject.SetActive(true);
            consumableUI.SetConsumable(consumable);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_slots.IsSubMenuActive)
            {
                _slots.CloseSubMenu();
                return;
            }

            if (consumableUI.HasConsumable())
            {
                eventData.Use();
                subMenu.SetActive(true);
                _slots.OnSubMenuOn();
            }
        }
        
        public void SetSubMenuActive(bool active)
        {
            subMenu.SetActive(active);
        }
    }
}