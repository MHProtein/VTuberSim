using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VBattleStaminaUI : VBattleAttributeUI
    {
        [SerializeField] private TMP_Text staminaText;
        [SerializeField] private Image bar;
        private float _barWidth;

        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnStaminaChange;
            SetFontStyle(staminaText, FontStyles.Bold);
            _barWidth = bar.rectTransform.rect.width;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VDebug.Log("");
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            var isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            var shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            var value = messagedict["NewValue"] as int? ?? 0;
            var maxValue = messagedict["MaxValue"] as int? ?? 0;
            var delta = messagedict["Delta"] as int? ?? 0;
            staminaText.text = $"{value}/{maxValue}";
            if (delta == 0)
                return;

            var x = -(1.0f - (value / (float)maxValue)) * _barWidth;
            Tween.LocalPositionX(bar.transform, x, 0.3f);
            _animationQueue.Enqueue(Tween.PunchScale(staminaText.transform, Vector3.one * 1.3f, 0.4f).OnComplete(() =>
            {
                RaiseEvents(isFromCard, shouldPlayTwice);
                staminaText.color = Color.white;
            }));
            staminaText.color = delta > 0 ? Color.green : Color.red;
        }
    }
}