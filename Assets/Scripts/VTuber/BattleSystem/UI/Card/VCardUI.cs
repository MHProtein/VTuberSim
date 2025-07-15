using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VCardUI : VUIBehaviour
    {
        [FormerlySerializedAs("Background")] [SerializeField] public Image background;
        [FormerlySerializedAs("Facade")] [SerializeField] public Image facade;
        
        [FormerlySerializedAs("Name")] [SerializeField] public TMP_Text name;
        [FormerlySerializedAs("Description")] [SerializeField] public TMP_Text description;
        [SerializeField] public TMP_Text cost;
        
        public VCard Card { get; private set; }
        
        public void SetCard(VCard card)
        {
            if(card.Background)
                background.sprite = card.Background;
            
            if(card.Facade)
                facade.sprite = card.Facade;
            
            name.text = card.CardName;
            description.text = card.Description;
            cost.text = card.Cost.ToString();
            Card = card;
        }

        public void SetBackgroundColor(Color color)
        {
            background.color = color;
        }
    }
}