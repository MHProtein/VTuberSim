using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.Foundation;

namespace VTuber.Store.UI
{
    [RequireComponent(typeof(Button))]
    public class VStoreButtonUI : VUIBehaviour
    {
        [SerializeField] protected GameObject soldOutObject;
        [SerializeField] protected TMP_Text priceText;
        [SerializeField] protected GameObject discountObject;
        [SerializeField] protected TMP_Text discountText;
        [SerializeField] protected GameObject poorObject;
        
        protected VCharacter character;
        protected bool canAfford = true;
        protected bool hasBought = false;
        protected VStoreButton storeButton;
        protected Button button;
        
        protected override void Awake()
        {
            base.Awake();
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        public virtual void SetButton(VStoreButton storeButton, VCharacter character) 
        {
            hasBought = false;
            button.interactable = true;
            this.storeButton = storeButton;
            this.character = character;
            
            this.character = character;
            soldOutObject.SetActive(false);
            priceText.text = storeButton.Price.ToString();
            discountObject.SetActive(storeButton.IsDiscount);
            if (storeButton.IsDiscount)
            {
                priceText.color = Color.yellow;
                discountText.text = $"-{(int)(this.storeButton.Discount * 100)}%";
            }
            else
            {
                priceText.color = Color.black;
            }

            SetCanAfford();
        }

        public virtual void OnClick()
        {
        }

        public void Buy()
        {
            hasBought = true;
            soldOutObject.SetActive(true);
            storeButton.Buy(character);
        }
        
        public void SetCanAfford()
        {
            if(hasBought)
                return;
            canAfford = storeButton.Affordable(character);
            poorObject.SetActive(!canAfford);
            if (!canAfford)
            {
                priceText.color = Color.red;
            }
        }
    }
}