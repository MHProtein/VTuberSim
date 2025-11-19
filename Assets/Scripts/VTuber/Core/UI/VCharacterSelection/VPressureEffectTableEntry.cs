using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.Core.UI.VCharacterSelection
{
    public class VPressureEffectTableEntry : VUIBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text pressureName;
        
        public void SetEffect(Sprite icon, string name, string description)
        {
            this.icon.sprite = icon;
            this.pressureName.text = name;
            this.description.text = description;
        }
    }
}