using System.Collections.Generic;
using UnityEngine;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core.UI
{
    public class VUIUtils : VSingletonMonobehaviour<VUIUtils>
    {
        [SerializeField] Dictionary<string, Sprite> attributeIcons;

        public Sprite GetAttributeIcon(string attributeName)
        {
            return attributeIcons[attributeName];
        }
    }
}