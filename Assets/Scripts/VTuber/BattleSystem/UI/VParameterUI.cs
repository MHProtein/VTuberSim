using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

            key = VRootEventKey.OnParameterChange;
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            base.OnValueChanged(messagedict);
            int delta = messagedict["Delta"] as int ? ?? 0;
            ParameterText.text = $"Parameter: {messagedict["NewValue"] as int? ?? 0}";
            if(delta == 0)
                return;
            
            Tween.PunchScale(transform, Vector3.one * 0.5f, 0.5f).OnComplete(OnAnimationFinished);
            ParameterText.faceColor = delta > 0 ? Color.green : Color.red;
        }

        protected override void OnAnimationFinished()
        {
            base.OnAnimationFinished();
            ParameterText.faceColor = Color.white;
        }
        
    }
}