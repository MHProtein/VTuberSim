using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using VTuber.Character;
using VTuber.Consumable;

namespace VTuber.Store.UI
{
    public class VStoreConsumableItemUI : VStoreItemUI
    {
        [SerializeField] private VConsumableUI consumableUI;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private RectTransform descriptionPanel;

        public override void SetSlot(VStoreSlot slot, VCharacter character)
        {
            var consumableSlot = slot as VStoreConsumableSlot;
            consumableUI.SetConsumable(consumableSlot.consumable);
            nameText.text = consumableSlot.consumable.Name;
            descriptionText.text = consumableSlot.consumable.Description;
            
            base.SetSlot(slot, character);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            descriptionPanel.gameObject.SetActive(true);
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            descriptionPanel.gameObject.SetActive(false);
        }
    }
}