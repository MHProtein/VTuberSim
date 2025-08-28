using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VBattleStaminaUI : VBattleAttributeUI
    {
        [SerializeField] private TMP_Text staminaText;
        [SerializeField] private Image bar;
        
        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnStaminaChange;
            SetFontStyle(staminaText, FontStyles.Bold);
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            bool isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            bool shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            var value = messagedict["NewValue"] as int? ?? 0;
            var maxValue = messagedict["MaxValue"] as int? ?? 0;
            int delta = messagedict["Delta"] as int ? ?? 0;
            staminaText.text = $"{value}/{maxValue}";
            if(delta == 0)
                return;
            
            bar.fillAmount = (float)value / maxValue;
            staminaText.faceColor = delta > 0 ? Color.green : Color.red;
            _animationQueue.Enqueue(Tween.PunchScale(transform, Vector3.one * 1.3f, 0.4f).OnComplete((
                () =>
                {
                    RaiseEvents(isFromCard, shouldPlayTwice);
                    staminaText.faceColor = Color.white;
                })));
        }
    }
}