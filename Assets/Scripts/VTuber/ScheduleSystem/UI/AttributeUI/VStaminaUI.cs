using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.EventCenter;

namespace VTuber.ScheduleSystem.UI
{
    public class VStaminaUI : VAttributeUI
    {
        [SerializeField] private Image bar;
        
        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            int delta = messagedict["Delta"] as int ? ?? 0;
            var value = messagedict["NewValue"] as int? ?? 0;
            var maxValue = messagedict["MaxValue"] as int? ?? 0;
            text.text = $"{value}/{maxValue}";
            if(delta == 0)
                return;
            Tween.UIFillAmount(bar, (float)value / maxValue, 0.3f);
            text.color = delta > 0 ? Color.green : Color.red;
            _animationQueue.Enqueue(Tween.PunchScale(text.transform, Vector3.one * 1.3f, 0.4f).OnComplete((
                () =>
                {
                    text.color = Color.white;
                })));
        }
    }
}