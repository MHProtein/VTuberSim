using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;

namespace VTuber.BattleSystem.Core.UI.VAccountSelection
{
    public class VAttributeEffectUI : VUIBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;

        public void Initialize(VRaisingEffect effect)
        {
            var attributeName = (effect as IAttributeEffect).AttributeName;
            icon.sprite = VUIUtils.Instance.GetAttributeIcon(attributeName);
            text.text = effect.GetParameter();
            if (attributeName.Contains("GainEfficiency"))
            {
                text.text += "%";
            }
        }

        public void SetEllipsis(Sprite ellipsisIcon)
        {
            icon.sprite = ellipsisIcon;
            text.text = "";
        }
    }
}