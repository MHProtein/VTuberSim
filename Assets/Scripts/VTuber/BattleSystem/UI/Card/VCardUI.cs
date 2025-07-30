using System.Collections.Generic;
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
        /*
         * 0 - Basic
         * 1 - Common
         * 3 - Rare
         * 4 - Epic
         * 5 - Special
         */
        [Header("Card Resources")]
        [SerializeField] private List<Sprite> backgrounds;
        [SerializeField] private List<Sprite> descriptionSprites;
        [SerializeField] private List<Sprite> nameSprites;
        [SerializeField] private List<Sprite> typeSprites;
        
        
        [SerializeField] public Image background;
        [SerializeField] public Image icon;
        [SerializeField] public Image descriptionImage;
        [SerializeField] public Image nameImage;
        [SerializeField] public Image typeImage;
        
        [FormerlySerializedAs("Name")] [SerializeField] public TMP_Text name;
        [FormerlySerializedAs("Description")] [SerializeField] public TMP_Text description;
        [SerializeField] public TMP_Text cost;
        [SerializeField] public TMP_Text typeText;
        
        public VCard Card { get; private set; }
        
        public void SetCard(VCard card)
        {
            background.sprite = backgrounds[(int)card.Rarity];
            descriptionImage.sprite = descriptionSprites[(int)card.Rarity];
            nameImage.sprite = nameSprites[(int)card.Rarity];
            typeImage.sprite = typeSprites[(int)card.Rarity];
            
            
            if(card.Facade)
                icon.sprite = card.Facade;
            
            name.text = card.CardName;
            description.text = card.Description;
            if(card.IsExhaust)
                description.text += "\nExhaust.";
            typeText.text = card.CardType;
            
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