using System.Collections.Generic;
using TMPro;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VStatUI : VUIBehaviour
    {
        protected VBattleEventKey key = VBattleEventKey.Default;
        protected VAnimationQueue _animationQueue = new VAnimationQueue();
        
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
        }

        protected virtual void RaiseEvents( bool isFromCard, bool shouldPlayTwice)
        {
            if (shouldPlayTwice)
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnPlayTheSecondTime, new Dictionary<string ,object>()
                {
                    
                });
                shouldPlayTwice = false;
                return;
            }
            
            if (isFromCard)
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard, new Dictionary<string ,object>()
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