using System.Collections.Generic;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Core;

namespace VTuber.Core.UI
{
    public class VUIUtils : VSingletonMonobehaviour<VUIUtils>
    {
        [SerializeField] Dictionary<string, Sprite> attributeIcons;
        [SerializeField] List<Sprite> pressureIcons;
        [SerializeField] List<string> pressureNames;
        [SerializeField] Sprite coopIcon; 

        public Sprite GetAttributeIcon(string attributeName)
        {
            return attributeIcons[attributeName];
        }

        public Sprite GetCoopIcon()
        {
            return coopIcon;
        }

        public KeyValuePair<string, Sprite>  GetPressureIcon(int i)
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
    }
}