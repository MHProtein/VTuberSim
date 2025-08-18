using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.UI;
using VTuber.Character;
using VTuber.Core.Foundation;

namespace VTuber.Store.UI
{
    public class VStoreItemUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected GameObject buyPanel;
        [SerializeField] protected Button buyButton;
        [SerializeField] protected GameObject soldOutObject;
        [SerializeField] protected TMP_Text priceText;
        [SerializeField] protected GameObject discountObject;
        [SerializeField] protected TMP_Text discountText;
        [SerializeField] protected GameObject poorObject;
        protected VStoreSlot slot;
        protected VCharacter character;
        protected bool canBuy = true;
        protected bool hasBought = false;

        protected override void Awake()
        {
            base.Awake();
            buyButton.onClick.AddListener(Buy);
        }

        public virtual void SetSlot(VStoreSlot slot, VCharacter character)
        {
            hasBought = false;
            this.slot = slot;
            this.character = character;
            buyPanel.SetActive(false);
            buyButton.interactable = true;
            soldOutObject.SetActive(false);
            priceText.text = slot.Price.ToString();
            discountObject.SetActive(slot.IsDiscount);
            if (slot.IsDiscount)
            {
                priceText.color = Color.yellow;
                discountText.text = $"-{(int)(slot.Discount * 100)}%";
            }
            else
            {
                priceText.color = Color.white;
            }

            SetCanAfford();
        }

        public void Buy()
        {
            if (canBuy)
            {
                hasBought = true;
                buyPanel.SetActive(false);
                soldOutObject.SetActive(true);
                slot.Buy(character);
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!hasBought && canBuy)
            {
                buyPanel.SetActive(true);
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!hasBought)
            {
                buyPanel.SetActive(false);
            }
        }

        public void SetCanAfford()
        {
            if(hasBought)
                return;
            canBuy = slot.Affordable(character);
            poorObject.SetActive(!canBuy);
            if (!canBuy)
            {
                priceText.color = Color.red;
            }
        }
    }
}