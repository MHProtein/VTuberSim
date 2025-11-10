using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;

namespace VTuber.ScheduleSystem.UI.RaisingAnimationSystem
{
    public enum VAnimationType
    {
        EffectCards,
        AttributeAnimation
    }
    
    public class VRaisingAnimationSystem : VSingletonMonobehaviour<VRaisingAnimationSystem>
    {
        [SerializeField] private Dictionary<VAnimationType, VRaisingAnimation> animations;
        private Queue<VAnimationRequest> _animationRequestQueue;
        
        protected override void Awake()
        {
            base.Awake();
            _animationRequestQueue = new();
        }

        public void EnqueueAnimationRequest(VAnimationRequest request)
        {
            switch (request.instigatorType)
            {
                case VInstigatorType.Coop:
                case VInstigatorType.Event:
                case VInstigatorType.Relic:
                case VInstigatorType.Pressure:
                    request.animationType = VAnimationType.EffectCards;
                    break;
                case VInstigatorType.Consumable:
                case VInstigatorType.Dialog:
                    request.animationType = VAnimationType.AttributeAnimation;
                    break;
            }
            _animationRequestQueue.Enqueue(request);
        }

        public void ExecuteAnimations(Action onAnimationsExecuted)
        {
            foreach (var animation in animations)
            {
                animation.Value.ResetAnimation();
            }
            ExecuteAnimationsImplement(onAnimationsExecuted);
        }

        public void ExecuteAnimationsImplement(Action onAnimationsExecuted)
        {
            var request = _animationRequestQueue.Dequeue();
            if (request is null)
            {
                onAnimationsExecuted?.Invoke();
                return;
            }
            animations[request.animationType].BeginAnimation(request, () => ExecuteAnimationsImplement(onAnimationsExecuted));
        }
    }
}