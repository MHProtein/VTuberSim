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
        [SerializeField] protected List<Sprite> backgrounds;
        [SerializeField] protected List<Sprite> descriptionSprites;
        [SerializeField] protected List<Sprite> nameSprites;
        [SerializeField] protected List<Sprite> typeSprites;

        [SerializeField] protected Sprite singingSprite;
        [SerializeField] protected Sprite gamingSprite;
        [SerializeField] protected Sprite chattingSprite;
        
        [SerializeField] protected Color singingColor;
        [SerializeField] protected Color gamingColor;
        [SerializeField] protected Color chattingColor;
        
        [SerializeField] public Image background;
        [SerializeField] public Image icon;
        [SerializeField] public Image descriptionImage;
        [SerializeField] public Image nameImage;
        [SerializeField] public Image typeImage;
        [SerializeField] public Image popularityImage;
        [SerializeField] public Image shieldImage;
        
        [FormerlySerializedAs("Name")] [SerializeField] public TMP_Text name;
        [FormerlySerializedAs("Description")] [SerializeField] public TMP_Text description;
        [SerializeField] public TMP_Text cost;
        [SerializeField] public TMP_Text typeText;
        [SerializeField] public TMP_Text popularityText;
        [SerializeField] public TMP_Text shieldText;
        
        public VCard Card { get; private set; }

        public void SetCard(VCard card)
        {
            background.sprite = backgrounds[(int)card.Rarity];
            descriptionImage.sprite = descriptionSprites[(int)card.Rarity];
            nameImage.sprite = nameSprites[(int)card.Rarity];
            typeImage.sprite = typeSprites[(int)card.Rarity];
            
            if(card.Icon)
                icon.sprite = card.Icon;

            name.text = card.CardName;
            description.text = card.GetDescription();
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
                var buff = VDataManager.Instance.GetBuffConfigurationByID(card.CostBuffId);
                cost.text = buff.buffName + " x " + card.Cost.ToString();
            }
            else
            {
                cost.text = card.Cost.ToString();
            }
            
            
            if (card.IsUpgraded)
            {
                name.text += "+"; 
                ColorUtility.TryParseHtmlString("#0ac736", out var color);
                name.color = color;
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