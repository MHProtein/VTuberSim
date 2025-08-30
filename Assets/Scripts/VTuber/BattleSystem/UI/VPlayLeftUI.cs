using System.Collections.Generic;
using DG.Tweening;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VBattlePlayLeftUI : VBattleAttributeUI
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
            bool isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            bool shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
            int delta = messagedict["Delta"] as int ? ?? 0;
            PlayLeftText.text = $"{messagedict["NewValue"] as int? ?? 0}";
            if(delta == 0)
                return;
            
            transform.DOKill();

            PlayLeftText.color = delta > 0 ? Color.green : Color.red;

            transform.DOPunchScale(Vector3.one * 0.3f, 0.4f, vibrato: 1)
                .OnComplete(() =>
                {
                    RaiseEvents(isFromCard, shouldPlayTwice);
                    PlayLeftText.color = Color.white;
                });
        }
        
    }
}