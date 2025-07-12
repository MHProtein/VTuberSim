using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
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

            key = VRootEventKey.OnTurnChange;
            SetFontStyle(turnLeftText, FontStyles.Bold);
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            base.OnValueChanged(messagedict);
            int delta = messagedict["Delta"] as int? ?? 0;
            turnLeftText.text = $"回合: {messagedict["NewValue"] as int? ?? 0}";
            if(delta == 0)
                return;
            
            Tween.PunchScale(transform, Vector3.one * 0.5f, 0.5f).OnComplete(OnAnimationFinished);
            turnLeftText.faceColor = delta > 0 ? Color.green : Color.red;
        }

        protected override void OnAnimationFinished()
        {
            base.OnAnimationFinished();
            turnLeftText.faceColor = Color.white;
        }
    }
}