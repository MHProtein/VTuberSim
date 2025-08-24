using System.Collections.Generic;
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
        [SerializeField] private Image levelBG;
        [SerializeField] private Image arrow;
        [SerializeField] private List<Sprite> levelSprite;
        [SerializeField] private TMP_Text text;

        public void Initialize(VRaisingEffect effect, int level)
        {
            var attributeName = (effect as IAttributeEffect).AttributeName;
            icon.sprite = VUIUtils.Instance.GetAttributeIcon(attributeName);
            text.text = effect.GetParameter();
            levelBG.sprite = levelSprite[level];
            if (attributeName.Contains("GainEfficiency"))
            {
                text.text += "%";
                arrow.gameObject.SetActive(true);
            }
        }

        public void SetEllipsis(Sprite ellipsisIcon)
        {
            icon.sprite = ellipsisIcon;
            text.text = "";
        }
    }
}