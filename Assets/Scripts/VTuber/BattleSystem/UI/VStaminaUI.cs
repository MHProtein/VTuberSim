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
    public class VStaminaUI : VStatUI
    {
        [SerializeField] private TMP_Text staminaText;
        
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
            int delta = messagedict["Delta"] as int ? ?? 0;
            staminaText.text = $"体力: {messagedict["NewValue"] as int? ?? 0}";
            if(delta == 0)
                return;
            
            _animationQueue.Enqueue(AnimationType.Punch, transform, () =>
            {
                RaiseEvents(isFromCard, shouldPlayTwice);
                staminaText.faceColor = Color.white;
            });
            staminaText.faceColor = delta > 0 ? Color.green : Color.red;
        }
    }
}