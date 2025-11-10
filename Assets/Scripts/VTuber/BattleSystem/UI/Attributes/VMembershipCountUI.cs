using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.UI
{
    public class VBattleMembershipCountUI : VBattleAttributeUI
    {
        [SerializeField] private TMP_Text viewerCountText;

        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnMembershipCountChange;
            SetFontStyle(viewerCountText, FontStyles.Bold);
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            var isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            var shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            var delta = messagedict["Delta"] as int? ?? 0;
            viewerCountText.text = $"{messagedict["NewValue"] as int? ?? 0}";
            if (delta == 0)
                return;

            _animationQueue.Enqueue(Tween.PunchScale(transform, Vector3.one * 1.3f, 0.4f).OnComplete(() =>
            {
                RaiseEvents(isFromCard, shouldPlayTwice);
                viewerCountText.color = Color.white;
            }));
            viewerCountText.color = delta > 0 ? Color.green : Color.red;
        }
    }
}