using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.Consumable
{
    public class VConsumableSlotUI : VUIBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject subMenu;
        [SerializeField] private Button useButton;
        [SerializeField] private Button discardButton;
        [SerializeField] private VConsumableUI consumableUI;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private RectTransform descriptionPanel;
        private VConsumableSlotsUI _slots;

        protected override void Awake()
        {
            base.Awake();
            descriptionPanel.anchoredPosition = Vector3.zero;
        }

        public void Init(VConsumableSlotsUI slots)
        {
            subMenu.SetActive(false);
            _slots = slots;
            consumableUI.gameObject.SetActive(false);
            useButton.onClick.AddListener(UseConsumable);
            discardButton.onClick.AddListener(DiscardConsumable);
        }

        private void UseConsumable()
        {
            consumableUI.gameObject.SetActive(false);
            consumableUI.UseConsumable();
            _slots.CloseSubMenu();
        }

        private void DiscardConsumable()
        {
            consumableUI.gameObject.SetActive(false);
            consumableUI.DiscardConsumable();
            _slots.CloseSubMenu();
        }

        public void SetConsumable(VConsumable consumable)
        {
            consumableUI.gameObject.SetActive(true);
            consumableUI.SetConsumable(consumable);
            _slots.CloseSubMenu();
            nameText.text = consumable.Name;
            descriptionText.text = consumable.Description;
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
                useButton.interactable = consumableUI.CanUse();
                eventData.Use();
                
                descriptionPanel.gameObject.SetActive(true);
                descriptionPanel.transform.SetParent(subMenu.transform);
                descriptionPanel.anchoredPosition = Vector3.zero;
                
                subMenu.SetActive(true);
                _slots.OnSubMenuOn();
            }
        }
        
        public void SetSubMenuInactive()
        {
            subMenu.SetActive(false);
            
            descriptionPanel.transform.SetParent(transform);
            descriptionPanel.anchoredPosition = Vector3.zero;
            descriptionPanel.gameObject.SetActive(false);
        }

        public bool HasConsumable()
        {
            return consumableUI.HasConsumable();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_slots.IsSubMenuActive)
                return;
            descriptionPanel.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_slots.IsSubMenuActive)
                return;
            descriptionPanel.gameObject.SetActive(false);
        }
    }
}