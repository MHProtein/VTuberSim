using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.RaisingEffect;
using VTuber.Core.SE;
using VTuber.Core.UI;
using VTuber.Core.UI.VCharacterSelection;

namespace VTuber.ScheduleSystem.UI
{
    public class VPressureUI : VAttributeUI, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private VPressureEffectTableEntry pressureEffectTableEntry;

        protected override void OnValueChanged(Dictionary<string, object> messagedict)
        {
            if ((bool)messagedict["shouldPlaySFX"])
                VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Raising_AttributeChange);
            base.OnValueChanged(messagedict);
            var delta = messagedict["Delta"] as int? ?? 0;
            var info = VUIUtils.Instance.GetPressureInfo((int)messagedict["NewValue"]);
            icon.sprite = info.Value;
            if ((int)messagedict["NewValue"] == 3)
            {
                pressureEffectTableEntry.SetEffect(info.Value,
                    info.Key,
                    "无效果");
            }
            else
            {
                pressureEffectTableEntry.SetEffect(info.Value,
                    info.Key,
                    "每天结束时, " + (messagedict["Effect"] as VRaisingEffect).Description);
            }
            
            text.text = info.Key;
            if (delta == 0)
                return;

            text.color = delta > 0 ? Color.green : Color.red;
            _animationQueue.Enqueue(Tween.PunchScale(text.transform, Vector3.one * 1.3f, 0.4f).OnComplete(() =>
            {
                text.color = Color.white;
            }));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            pressureEffectTableEntry.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pressureEffectTableEntry.gameObject.SetActive(false);
        }
    }
}