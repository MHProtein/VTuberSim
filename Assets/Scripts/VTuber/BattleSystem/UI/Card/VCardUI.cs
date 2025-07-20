using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

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
            description.text += "\n" + card.CardType;
            // foreach (var effect in card.Effects)
            // {
            //     string effectDescription = effect.Name + ": " + effect.Description;
            //     description.text += "\n" + effectDescription;
            // }

            if (card.CostType == CostType.Buff)
            {
                var buff = VBattleDataManager.Instance.GetBuffConfigurationByID(card.CostBuffId);
                cost.text = buff.buffName + " x " + card.Cost.ToString();
            }
            else
            {
                cost.text = card.Cost.ToString();
            }
            Card = card;
        }

        public void SetBackgroundColor(Color color)
        {
            background.color = color;
        }
    }
}