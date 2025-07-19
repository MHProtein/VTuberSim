using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.UI
{
    public class VViewerCountUI : VStatUI
    {
        [SerializeField] private TMP_Text viewerCountText;

        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnViewerCountChange;
            SetFontStyle(viewerCountText, FontStyles.Bold);
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            bool isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            bool shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            int delta = messagedict["Delta"] as int ? ?? 0;
            viewerCountText.text = $"同接: {messagedict["NewValue"] as int? ?? 0}";
            if(delta == 0)
                return;
            
            _animationQueue.Enqueue(AnimationType.Punch, transform, () =>
            {
                RaiseEvents(isFromCard, shouldPlayTwice);
                viewerCountText.faceColor = Color.white;
            });
            viewerCountText.faceColor = delta > 0 ? Color.green : Color.red;
        }
    }
}