using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.UI
{
    public class VBattleTurnLeftUI : VBattleAttributeUI
    {
        [SerializeField] private TMP_Text turnLeftText;

        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnTurnChange;
            SetFontStyle(turnLeftText, FontStyles.Bold);
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            var isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            var shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            var delta = messagedict["Delta"] as int? ?? 0;
            turnLeftText.text = $"剩余回合：{messagedict["NewValue"] as int? ?? 0}";
            if (delta == 0)
                return;

            _animationQueue.Enqueue(Tween.PunchScale(transform, Vector3.one * 1.3f, 0.4f).OnComplete(() =>
            {
                RaiseEvents(isFromCard, shouldPlayTwice);
                turnLeftText.color = Color.white;
            }));
            turnLeftText.color = delta > 0 ? Color.green : Color.red;
        }
    }
}