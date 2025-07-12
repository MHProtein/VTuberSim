using System.Collections.Generic;
using TMPro;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VStatUI : VUIBehaviour
    {
        protected VRootEventKey key = VRootEventKey.Default;
        private bool isFromCard = false;
        private bool shouldPlayTwice = false;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            VBattleRootEventCenter.Instance.RegisterListener(key, OnValueChanged);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(key, OnValueChanged);
        }
        
        protected virtual void OnValueChanged(Dictionary<string, object> messagedict)
        {
            isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
        }

        protected virtual void OnAnimationFinished()
        {
            if (shouldPlayTwice)
            {
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnPlayTheSecondTime, new Dictionary<string ,object>()
                {
                    
                });
                shouldPlayTwice = false;
                return;
            }
            
            if (isFromCard)
            {
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnNotifyBeginDisposeCard, new Dictionary<string ,object>()
                {
                    
                });
                isFromCard = false;
            }
        }

        protected void SetFontStyle(TMP_Text text, FontStyles style)
        {
        }

    }
}