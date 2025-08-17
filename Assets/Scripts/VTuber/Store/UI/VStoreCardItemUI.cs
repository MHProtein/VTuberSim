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
    public class VStoreCardItemUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private VCardUI cardUI;
        [SerializeField] private GameObject buyPanel;
        [SerializeField] private Button buyButton;
        [SerializeField] private GameObject soldOutObject;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private GameObject discountObject;
        [SerializeField] private TMP_Text discountText;
        [SerializeField] private GameObject poorObject;
        private VStoreCardSlot _cardSlot;
        private VCharacter _character;
        private bool _canAfford = true;
        private bool _hasBought = false;

        protected override void Awake()
        {
            base.Awake();
            buyButton.onClick.AddListener(Buy);
        }

        public void SetCardSlot(VStoreCardSlot cardSlot, VCharacter character)
        {
            _cardSlot = cardSlot;
            _character = character;
            cardUI.SetCard(cardSlot.card);
            buyPanel.SetActive(false);
            buyButton.interactable = true;
            soldOutObject.SetActive(false);
            priceText.text = cardSlot.price.ToString();
            discountObject.SetActive(cardSlot.isDiscount);
            if (cardSlot.isDiscount)
            {
                priceText.color = Color.yellow;
                discountText.text = $"-{(int)(cardSlot.discount * 100)}%";
            }
            else
            {
                priceText.color = Color.white;
            }

            SetCanAfford();
        }

        public void Buy()
        {
            if (_canAfford)
            {
                _hasBought = true;
                buyPanel.SetActive(false);
                soldOutObject.SetActive(true);
                _cardSlot.Buy(_character);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_hasBought && _canAfford)
            {
                buyPanel.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_hasBought)
            {
                buyPanel.SetActive(false);
            }
        }

        public void SetCanAfford()
        {
            if(_hasBought)
                return;
            _canAfford = _cardSlot.Affordable(_character);
            poorObject.SetActive(!_canAfford);
            if (!_canAfford)
            {
                priceText.color = Color.red;
            }
        }
    }
}