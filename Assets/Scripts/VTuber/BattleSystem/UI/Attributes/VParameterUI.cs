using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.UI
{
    public class VBattleParameterUI : VBattleAttributeUI
    {
        [SerializeField] private TMP_Text ParameterText;

        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnParameterChange;
            SetFontStyle(ParameterText, FontStyles.Bold);
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            var isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            var shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            var delta = messagedict["Delta"] as int? ?? 0;
            ParameterText.text = $"{messagedict["NewValue"] as int? ?? 0}";
            if (delta == 0)
                return;

            _animationQueue.Enqueue(Tween.PunchScale(transform, Vector3.one * 1.3f, 0.4f).OnComplete(() =>
            {
                RaiseEvents(isFromCard, shouldPlayTwice);
                ParameterText.color = Color.white;
            }));

            ParameterText.color = delta > 0 ? Color.green : Color.red;
        }
    }
}