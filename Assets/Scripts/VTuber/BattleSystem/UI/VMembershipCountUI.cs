using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Core;
using PrimeTween;

namespace VTuber.BattleSystem.UI
{
    public class VMembershipCountUI : VStatUI
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
            bool isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            bool shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            int delta = messagedict["Delta"] as int ? ?? 0;
            viewerCountText.text = $"{messagedict["NewValue"] as int? ?? 0}";
            if(delta == 0)
                return;
            
            _animationQueue.Enqueue(Tween.PunchScale(transform, Vector3.one * 1.3f, 0.4f).OnComplete((
                () =>
                {
                    RaiseEvents(isFromCard, shouldPlayTwice);
                    viewerCountText.faceColor = Color.white;
                })));
            viewerCountText.faceColor = delta > 0 ? Color.green : Color.red;
        }
    }
}