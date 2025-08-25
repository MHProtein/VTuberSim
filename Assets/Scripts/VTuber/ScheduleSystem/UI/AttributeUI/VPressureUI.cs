using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.ScheduleSystem.UI
{
    public class VPressureUI : VAttributeUI
    {
        [SerializeField] private Image icon;
        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            base.OnValueChanged(messagedict);
            int delta = messagedict["Delta"] as int ? ?? 0;
            var info = VUIUtils.Instance.GetPressureIcon((int)messagedict["NewValue"]);
            icon.sprite = info.Value;
            text.text = info.Key;
            if(delta == 0)
                return;
            
            text.faceColor = delta > 0 ? Color.green : Color.red;
            _animationQueue.Enqueue(Tween.PunchScale(text.transform, Vector3.one * 1.3f, 0.4f).OnComplete((
                () =>
                {
                    text.faceColor = Color.white;
                })));
        }
    }
}