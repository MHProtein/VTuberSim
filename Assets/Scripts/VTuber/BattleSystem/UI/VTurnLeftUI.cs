using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VTurnLeftUI : VStatUI
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
            bool isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            bool shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            int delta = messagedict["Delta"] as int? ?? 0;
            turnLeftText.text = $"回合: {messagedict["NewValue"] as int? ?? 0}";
            if(delta == 0)
                return;
            
            _animationQueue.Enqueue(AnimationType.Punch, transform, () =>
            {
                RaiseEvents(isFromCard, shouldPlayTwice);
                turnLeftText.faceColor = Color.white;
            });
            turnLeftText.faceColor = delta > 0 ? Color.green : Color.red;
        }
    }
}