using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.UI;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VAttributeUI : VUIBehaviour
    {
        [SerializeField] protected string name;
        [SerializeField] protected bool isPercentage;
        [SerializeField] protected TMP_Text text;
        [SerializeField] protected VRaisingEventKey key = VRaisingEventKey.Default;
        protected VAnimationQueue _animationQueue = new VAnimationQueue();
        
        protected override void OnEnable()
        {
            base.OnEnable();

            VDebug.Log(name + " " + key);
            VRaisingRootEventCenter.Instance.RegisterListener(key, OnValueChanged);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(key, OnValueChanged);
        }
        
        protected virtual void OnValueChanged(Dictionary<string, object> messagedict)
        {
            int delta = messagedict["Delta"] as int ? ?? 0;
            text.text = $"{messagedict["NewValue"] as int? ?? 0}";
            if (isPercentage)
                text.text += "%";
            if(delta == 0)
                return;
            
            text.faceColor = delta > 0 ? Color.green : Color.red;
            _animationQueue.Enqueue(Tween.PunchScale(transform, Vector3.one * 1.3f, 0.4f).OnComplete((
                () =>
                {
                    text.faceColor = Color.white;
                })));
        }

        protected virtual void RaiseEvents()
        {
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnPlayTheSecondTime, new Dictionary<string ,object>()
            {
                
            });
        }

        protected void SetFontStyle(TMP_Text text, FontStyles style)
        {
            text.fontStyle = style;
        }
    }
}