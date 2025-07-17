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
    public class VShieldUI : VStatUI
    {
        [SerializeField] private TMP_Text shieldText;


        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnShieldChange;
            SetFontStyle(shieldText, FontStyles.Bold);
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            base.OnValueChanged(messagedict);
            int delta = messagedict["Delta"] as int ? ?? 0;
            shieldText.text = $"护盾: {messagedict["NewValue"] as int? ?? 0}";
            if(delta == 0)
                return;
            
            Tween.PunchScale(transform, Vector3.one * 0.5f, 0.5f).OnComplete(OnAnimationFinished);
            shieldText.faceColor = delta > 0 ? Color.green : Color.red;
        }

        protected override void OnAnimationFinished()
        {
            base.OnAnimationFinished();
            shieldText.faceColor = Color.white;
        }
    }
}