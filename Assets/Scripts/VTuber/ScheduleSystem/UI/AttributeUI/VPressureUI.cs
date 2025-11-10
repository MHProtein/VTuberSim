using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.SE;
using VTuber.Core.UI;

namespace VTuber.ScheduleSystem.UI
{
    public class VPressureUI : VAttributeUI
    {
        [SerializeField] private Image icon;

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            if ((bool)messagedict["shouldPlaySFX"])
                VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Raising_AttributeChange);
            base.OnValueChanged(messagedict);
            var delta = messagedict["Delta"] as int? ?? 0;
            var info = VUIUtils.Instance.GetPressureIcon((int)messagedict["NewValue"]);
            icon.sprite = info.Value;
            text.text = info.Key;
            if (delta == 0)
                return;

            text.color = delta > 0 ? Color.green : Color.red;
            _animationQueue.Enqueue(Tween.PunchScale(text.transform, Vector3.one * 1.3f, 0.4f).OnComplete(() =>
            {
                text.color = Color.white;
            }));
        }
    }
}