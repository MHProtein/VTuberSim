using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using PrimeTween;

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
        [Header("Card Resources")] [SerializeField]
        protected List<Sprite> backgrounds;

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
        [SerializeField] public Image costImage;

        [FormerlySerializedAs("Name")] [SerializeField]
        public TMP_Text name;

        [FormerlySerializedAs("Description")] [SerializeField]
        public TMP_Text description;

        [SerializeField] public TMP_Text cost;
        [SerializeField] public TMP_Text typeText;
        [SerializeField] public TMP_Text popularityText;
        [SerializeField] public TMP_Text shieldText;
        [SerializeField] private Sprite staminaSprite;

        public VCard Card { get; private set; }

        public void SetCard(VCard card)
        {
            background.sprite = backgrounds[(int)card.Rarity];
            descriptionImage.sprite = descriptionSprites[(int)card.Rarity];
            nameImage.sprite = nameSprites[(int)card.Rarity];
            typeImage.sprite = typeSprites[(int)card.Rarity];

            if (card.Icon)
                icon.sprite = card.Icon;

            name.text = card.CardName;
            description.text = card.GetDescription();
            if (card.IsExhaust)
                description.text += "\n消耗牌.";
            typeText.text = card.CardType;
            
            if (card.CostType == CostType.Stamina) costImage.color = Color.white;
            if (card.CostType == CostType.TrueStamina) costImage.color = Color.red;

            if (card.CostType == CostType.Buff)
            {
                costImage.color = Color.white;
                costImage.sprite = VDataManager.Instance.GetBuffConfigurationByID(card.CostBuffId).icon;
                cost.text = "-" + card.Cost;
                cost.transform.localPosition = new Vector3(cost.transform.localPosition.x, -40f, 0f);
            }
            else
            {
                cost.transform.localPosition = new Vector3(cost.transform.localPosition.x, 0f, 0f);
                costImage.sprite = staminaSprite;
                cost.text = "-" + card.Cost;
            }

            if (card.IsUpgraded)
            {
                name.text += "+";
                ColorUtility.TryParseHtmlString("#0ac736", out var color);
                name.color = color;
            }
            else
            {
                name.color = Color.black;
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

        public void SetImageAlpha(Image image, float alpha)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }
        
        public void SetAlpha(float alpha)
        {
            SetImageAlpha(background, alpha);
            SetImageAlpha(icon, alpha);
            SetImageAlpha(descriptionImage, alpha);
            SetImageAlpha(nameImage, alpha);
            SetImageAlpha(typeImage, alpha);
            SetImageAlpha(popularityImage, alpha);
            SetImageAlpha(shieldImage, alpha);
            SetImageAlpha(costImage, alpha);
            
            name.alpha = alpha;
            description.alpha = alpha;
            cost.alpha = alpha;
            typeText.alpha = alpha;
            popularityText.alpha = alpha;
            shieldText.alpha = alpha;
        }

        public Sequence TweenAlpha(float targetValue, float duration)
        {
            var alphaSequence = Sequence.Create();
            alphaSequence
                .Group(Tween.Alpha(background, targetValue, duration))
                .Group(Tween.Alpha(icon, targetValue, duration))
                .Group(Tween.Alpha(descriptionImage, targetValue, duration))
                .Group(Tween.Alpha(nameImage, targetValue, duration))
                .Group(Tween.Alpha(typeImage, targetValue, duration))
                .Group(Tween.Alpha(popularityImage, targetValue, duration))
                .Group(Tween.Alpha(shieldImage, targetValue, duration))
                .Group(Tween.Alpha(costImage, targetValue, duration))
                .Group(Tween.Alpha(name, targetValue, duration))
                .Group(Tween.Alpha(description, targetValue, duration))
                .Group(Tween.Alpha(cost, targetValue, duration))
                .Group(Tween.Alpha(typeText, targetValue, duration))
                .Group(Tween.Alpha(popularityText, targetValue, duration))
                .Group(Tween.Alpha(shieldText, targetValue, duration));
            return alphaSequence;
        }

        public void UpdateView()
        {
            SetCard(Card);
        }
    }
}