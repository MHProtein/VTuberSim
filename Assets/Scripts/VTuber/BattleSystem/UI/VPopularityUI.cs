using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

            key = VRootEventKey.OnPopularityChange;
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            base.OnValueChanged(messagedict);
            int delta = messagedict["Delta"] as int ? ?? 0;
            popularityText.text = $"Popularity: {messagedict["NewValue"] as int? ?? 0}";
            if(delta == 0)
                return;
            
            Tween.PunchScale(transform, Vector3.one * 0.5f, 0.5f).OnComplete(OnAnimationFinished);
            popularityText.faceColor = delta > 0 ? Color.green : Color.red;
        }

        protected override void OnAnimationFinished()
        {
            base.OnAnimationFinished();
            popularityText.faceColor = Color.white;
        }
        
        
    }
}