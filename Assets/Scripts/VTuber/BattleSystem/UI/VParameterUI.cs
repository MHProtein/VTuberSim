using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VParameterUI : VStatUI
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
            bool isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            bool shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            int delta = messagedict["Delta"] as int ? ?? 0;
            ParameterText.text = $"参数: {messagedict["NewValue"] as int? ?? 0}";
            if(delta == 0)
                return;
            
            _animationQueue.Enqueue(AnimationType.Punch, transform, () =>
            {
                RaiseEvents(isFromCard, shouldPlayTwice);
                ParameterText.faceColor = Color.white;
            });
            ParameterText.faceColor = delta > 0 ? Color.green : Color.red;
        }
        
    }
}