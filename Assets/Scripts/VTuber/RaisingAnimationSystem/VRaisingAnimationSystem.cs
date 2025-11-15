using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.RaisingAnimationSystem
{
    public enum VAnimationType
    {
        EffectCards,
        AttributeAnimation,
        CoopUpgrade,
        AddRelic,
        None,
        AddConsumable,
        SelectConsumableFrom3,
        AddCard,
        SelectCardFrom3,
        RemoveCard
    }
    
    public class VRaisingAnimationSystem : VSingletonMonobehaviour<VRaisingAnimationSystem>
    {
        [SerializeField] private bool debug;
        [SerializeField] private GameObject ui;
        [SerializeField] private Dictionary<VAnimationType, VRaisingAnimation> animations;
        private LinkedList<VAnimationRequest> _animationRequestQueue;
        private Action _onAnimationsExecuted;
        private bool _animationsExecuting;
        private VAnimationType _currentAnimationType;
        
        protected override void Awake()
        {
            base.Awake();
            _animationRequestQueue = new();
            _currentAnimationType = VAnimationType.None;
        }

        protected override void Start()
        {
            base.Start();        
            
            foreach (var anim in animations)
            {
                anim.Value.SetDebug(debug);
                anim.Value.Hide();
            }
        }
        
        public void DebugEnqueueAnimationRequest(VAnimationRequest request)
        {
            if (!debug)
                return;
            EnqueueAnimationRequest(request);
        }

        public void EnqueueAnimationRequest(VAnimationRequest request, bool insertAtFront = false)
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
            if(insertAtFront)
                _animationRequestQueue.AddFirst(request);
            else
                _animationRequestQueue.AddLast(request);
        }

        public void ExecuteAnimations(Action onAnimationsExecuted)
        {
            if (_animationRequestQueue.Count <= 0)
            {
                onAnimationsExecuted?.Invoke();
                return;
            }
            
            foreach (var anim in animations)
            {
                anim.Value.Hide();
            }
            ui.SetActive(true);
            _onAnimationsExecuted = onAnimationsExecuted;
            foreach (var animation in animations)
            {
                animation.Value.ResetAnimation();
            }
            ExecuteAnimationsImplement();
        }

        private void ExecuteAnimationsImplement()
        {
            _animationsExecuting = true;
            if (_animationRequestQueue.Count == 0)
            {
                _animationsExecuting = false;
                _currentAnimationType = VAnimationType.None; 
                
                ui.SetActive(false);
                _onAnimationsExecuted?.Invoke();
                
                return;
            }
            var request = _animationRequestQueue.First();
            _animationRequestQueue.RemoveFirst();
            
            var animation = animations[request.animationType];

            if (_currentAnimationType != request.animationType && _currentAnimationType != VAnimationType.None)
            {
                foreach (var anim in animations)
                {
                    anim.Value.Hide();
                }
            }
            
            animation.Show();
            _currentAnimationType = request.animationType;
            animation.BeginAnimation(request, ExecuteAnimationsImplement, _animationRequestQueue.Count == 0);
        }

        public bool HasAnimationRequests()
        {
            return _animationRequestQueue.Count > 0;
        }
    }
}