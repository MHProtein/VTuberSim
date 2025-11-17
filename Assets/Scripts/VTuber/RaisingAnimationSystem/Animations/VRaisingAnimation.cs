using System;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.RaisingAnimationSystem;

namespace VTuber.ScheduleSystem.UI.RaisingAnimationSystem
{
    public class VRaisingAnimation : VUIBehaviour
    {
        [SerializeField] protected GameObject ui;
        protected bool debug;

        public virtual void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
        {
            if (!debug)
            {
                request.effectApply?.Invoke();
            }
        }

        public virtual void ResetAnimation()
        {
            
        }

        public void Hide()
        {
            ui.SetActive(false);
        }

        public void Show()
        {
            ui.SetActive(true);
        }

        public void SetDebug(bool debug)
        {
            this.debug = debug;
        }
    }
}