using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
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
        RemoveCard,
        SelectCard,
        UpgradeCard,
        SelectCardPreview,
        ReplaceCard
    }
    
    public class VRaisingAnimationSystem : VSingletonMonobehaviour<VRaisingAnimationSystem>
    {
        [SerializeField] private bool debug;
        [SerializeField] private GameObject ui;
        [SerializeField] private Image background;
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
            
            Tween.Alpha(background, 0.8f, 0.3f).OnComplete(ExecuteAnimationsImplement);
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
                Tween.Alpha(background, 0.0f, 0.3f);
                
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

            if (request.animationType != VAnimationType.EffectCards)
            {
                animation.ResetAnimation();
            }
            
            animation.Show();
            _currentAnimationType = request.animationType;
            
            animation.BeginAnimation(request,
                ExecuteAnimationsImplement, 
                _animationRequestQueue.Count == 0 || 
                (_animationRequestQueue.First is not null && _animationRequestQueue.First.Value.animationType != request.animationType));
        }

        public bool HasAnimationRequests()
        {
            return _animationRequestQueue.Count > 0;
        }
    }
}