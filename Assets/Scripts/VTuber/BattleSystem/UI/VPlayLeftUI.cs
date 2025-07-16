using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VPlayLeftUI : VStatUI
    {
        [SerializeField] private TMP_Text PlayLeftText;
        
        
        protected override void Awake()
        {
            base.Awake();

            key = VBattleEventKey.OnPlayLeftChange;
            SetFontStyle(PlayLeftText, FontStyles.Bold);
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            base.OnValueChanged(messagedict);
            int delta = messagedict["Delta"] as int ? ?? 0;
            PlayLeftText.text = $"出牌数: {messagedict["NewValue"] as int? ?? 0}";
            if(delta == 0)
                return;
            
            Tween.PunchScale(transform, Vector3.one * 0.5f, 0.5f).OnComplete(OnAnimationFinished);
            PlayLeftText.faceColor = delta > 0 ? Color.green : Color.red;
        }

        protected override void OnAnimationFinished()
        {
            base.OnAnimationFinished();
            PlayLeftText.faceColor = Color.white;
        }
    }
}