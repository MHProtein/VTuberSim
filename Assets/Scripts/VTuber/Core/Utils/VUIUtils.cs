using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.UI;
using VTuber.Core.Foundation;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.Core;

namespace VTuber.Core.UI
{
    public class VUIUtils : VSingletonMonobehaviour<VUIUtils>
    {
        [SerializeField] private Dictionary<string, Sprite> attributeIcons;
        [SerializeField] private List<Sprite> pressureIcons;
        [SerializeField] private List<string> pressureNames;
        [SerializeField] private Sprite coopIcon;
        [SerializeField] private List<Sprite> haloSprites;
        [SerializeField] private Dictionary<VAnimationType, string> selectCardMenuTitles;
        [SerializeField] private Dictionary<VAnimationType, string> selectCardMenuPreviewCardTitles;

        public Sprite GetRandomAttributeIcon()
        {
            return attributeIcons.Values.ToList()[Random.Range(0, attributeIcons.Count)];
        }
        
        public Sprite GetAttributeIcon(string attributeName)
        {
            return attributeIcons[attributeName];
        }

        public Sprite GetCoopIcon()
        {
            return coopIcon;
        }
        
        public Sprite GetHaloSprite(int level)
        {
            return haloSprites[level];
        }

        public KeyValuePair<string, Sprite> GetPressureIcon(int i)
        {
            return new KeyValuePair<string, Sprite>(pressureNames[i - 1], pressureIcons[i - 1]);
        }

        public string GetEventName(VEventType eventType)
        {
            switch (eventType)
            {
                case VEventType.Stream:
                    return "直播";
                case VEventType.Practice:
                    return "练习";
                case VEventType.Coop:
                    return "协助";
                case VEventType.Outside:
                    return "外出";
                case VEventType.Work:
                    return "工作";
                case VEventType.Rest:
                    return "休息";
                case VEventType.Other:
                    return "其他";
            }

            return "";
        }

        public string GetAttributeName(string attributeName)
        {
            switch (attributeName)
            {
                case "CAStamina":
                    return "体力<sprite name=Icon_Stamina>";
                case "CAPressure":
                    return "压力<sprite name=Icon_Happy>";
                case "CASingingAbility":
                    return "歌力<sprite name=Icon_SingingAbility>";
                case "CASingingAbilityGainEfficiency":
                    return "歌力提升率<sprite name=Icon_SingingAbility>";
                case "CAGamingAbility":
                    return "游戏力<sprite name=Icon_GamingAbility>";
                case "CAGamingAbilityGainEfficiency":
                    return "游戏力提升率<sprite name=Icon_GamingAbility>";
                case "CAChattingAbility":
                    return "杂谈力<sprite name=Icon_ChattingAbility>";
                case "CAChattingAbilityGainEfficiency":
                    return "杂谈力提升率<sprite name=Icon_ChattingAbility>";
                case "CAFollowerCount":
                    return "粉丝<sprite name=Icon_Follower>";
                case "CAMembershipCount":
                    return "舰长<sprite name=Icon_Membership>";
                case "CAMoney":
                    return "沪币<sprite name=Icon_Money>";
            }

            return "";
        }

        public Sprite GetScoreLevelSprite(string level)
        {
            if (level.IsNullOrWhitespace()) return VResourcesManager.Instance.TryGetSprite("ScoreLevel_SSS");
            return VResourcesManager.Instance.TryGetSprite("ScoreLevel_" + level);
        }
        
        public static VCardUI SpawnCardUI(GameObject cardUIPrefab, VCard card, Transform parent)
        {
            if (card == null)
            {
                VDebug.LogError("SpawnCardUI: Card is null");
                return null;
            }

            var cardUI = Instantiate(cardUIPrefab, parent).GetComponent<VCardUI>();
            cardUI.SetCard(card);

            return cardUI;
        }

        public static GameObject SpawnPrefab(GameObject prefab, Transform parent)
        {
            return Instantiate(prefab, parent);
        }

        public static void SetImageAlpha(Image image, float alpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }

        public string GetSelectCardMenuTitle(VAnimationType cardSelectAnimationType)
        {
            return selectCardMenuTitles[cardSelectAnimationType];
        }

        public string GetSelectCardMenuPreviewCardTitle(VAnimationType cardSelectAnimationType)
        {
            return selectCardMenuPreviewCardTitles[cardSelectAnimationType];
        }
    }
}