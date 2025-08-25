using System.Collections.Generic;
using UnityEngine;
using VTuber.Core.Foundation;

namespace VTuber.Core.UI
{
    public class VUIUtils : VSingletonMonobehaviour<VUIUtils>
    {
        [SerializeField] Dictionary<string, Sprite> attributeIcons;
        [SerializeField] List<Sprite> pressureIcons;
        [SerializeField] List<string> pressureNames;

        public Sprite GetAttributeIcon(string attributeName)
        {
            return attributeIcons[attributeName];
        }

        public KeyValuePair<string, Sprite>  GetPressureIcon(int i)
        {
            return new KeyValuePair<string, Sprite>(pressureNames[i - 1], pressureIcons[i - 1]);
        }
    }
}