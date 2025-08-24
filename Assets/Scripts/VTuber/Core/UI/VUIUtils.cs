using System.Collections.Generic;
using UnityEngine;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core.UI
{
    public class VUIUtils : VSingletonMonobehaviour<VUIUtils>
    {
        [SerializeField] Dictionary<string, Sprite> attributeIcons;
        [SerializeField] List<Sprite> pressureIcons;

        public Sprite GetAttributeIcon(string attributeName)
        {
            return attributeIcons[attributeName];
        }

        public Sprite GetPressureIcon(int i)
        {
            return pressureIcons[i - 1];
        }
    }
}