using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
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

        [SerializeField] private Sprite singingSprite;
        [SerializeField] private Sprite gamingSprite;
        [SerializeField] private Sprite chattingSprite;
        
        [SerializeField] private Color singingColor;
        [SerializeField] private Color gamingColor;
        [SerializeField] private Color chattingColor;
        
        [SerializeField] public Image background;
        [SerializeField] public Image icon;
        [SerializeField] public Image descriptionImage;
        [SerializeField] public Image nameImage;
        [SerializeField] public Image typeImage;
        [SerializeField] public Image popularityImage;
        
        [FormerlySerializedAs("Name")] [SerializeField] public TMP_Text name;
        [FormerlySerializedAs("Description")] [SerializeField] public TMP_Text description;
        [SerializeField] public TMP_Text cost;
        [SerializeField] public TMP_Text typeText;
        [SerializeField] public TMP_Text popularityText;
        
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

        public void SetPopularityImage(string multiplier)
        {
            if (multiplier.Equals("BASingingMultiplier"))
            {
                popularityImage.sprite = singingSprite;
                popularityText.color = singingColor;
            }
            else if (multiplier.Equals("BAGamingMultiplier"))
            {
                popularityImage.sprite = gamingSprite;
                popularityText.color = gamingColor;
            }
            else if (multiplier.Equals("BAChattingMultiplier"))
            {
                popularityImage.sprite = chattingSprite;
                popularityText.color = chattingColor;
            }
        }
    }
}