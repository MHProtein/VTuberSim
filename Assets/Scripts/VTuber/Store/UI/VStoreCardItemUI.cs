using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.UI;
using VTuber.Core.Foundation;

namespace VTuber.Store.UI
{
    public class VStoreCardItemUI : VUIBehaviour
    {
        [SerializeField] private VCardUI cardUI;
        [SerializeField] private GameObject buyPanel;
        [SerializeField] private Button buyButton;
        [SerializeField] private GameObject soldOutObject;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private GameObject discountObject;
        [SerializeField] private TMP_Text discountText;
        private VStoreCardSlot _cardSlot;
        
    }
}