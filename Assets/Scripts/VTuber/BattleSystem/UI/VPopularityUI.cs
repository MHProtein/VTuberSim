using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VPopularityUI : VStatUI
    {
        [SerializeField] private TMP_Text popularityText;

        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnPopularityChange;
            SetFontStyle(popularityText, FontStyles.Bold);
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            bool isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            bool shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            int delta = messagedict["Delta"] as int ? ?? 0;
            popularityText.text = $"热度: {messagedict["NewValue"] as int? ?? 0}";
            if(delta == 0)
                return;
            
            _animationQueue.Enqueue(AnimationType.Punch, transform, () =>
            {
                RaiseEvents(isFromCard, shouldPlayTwice);
                popularityText.faceColor = Color.white;
            });
            popularityText.faceColor = delta > 0 ? Color.green : Color.red;
        }
    }
}