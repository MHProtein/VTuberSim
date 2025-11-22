using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.SE;

namespace VTuber.ScheduleSystem.UI
{
    public class VStaminaUI : VAttributeUI
    {
        [SerializeField] private Image bar;
        private float _barWidth;
        protected override void Awake()
        {
            base.Awake();
            _barWidth = bar.rectTransform.rect.width;
        }

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            if ((bool)messagedict["shouldPlaySFX"])
                VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Raising_AttributeChange);
            var delta = messagedict["Delta"] as int? ?? 0;
            var value = messagedict["NewValue"] as int? ?? 0;
            var maxValue = messagedict["MaxValue"] as int? ?? 0;
            text.text = $"{value}/{maxValue}";
            if (delta == 0)
                return;
            var x = -(1.0f - (value / (float)maxValue)) * _barWidth;
            Tween.LocalPositionX(bar.transform, x, 0.3f);
            //Tween.UIFillAmount(bar, (float)value / maxValue, 0.3f);
            text.color = delta > 0 ? Color.green : Color.red;
            _animationQueue.Enqueue(Tween.PunchScale(text.transform, Vector3.one * 1.3f, 0.4f).OnComplete(() =>
            {
                text.color = Color.white;
            }));
        }
    }
}