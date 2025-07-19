using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace VTuber.BattleSystem.UI
{        
    public enum AnimationType { ScaleIn, Punch }
    public class BuffAnimationRequest
    {
        public Transform target;
        public AnimationType type;
        public Action onCompleteAction;
    }
    
    public class VAnimationQueue
    {
        private Queue<BuffAnimationRequest> _animationQueue = new();
        private bool _isAnimating = false;
        
        public void Enqueue(AnimationType type, Transform target, Action onCompleteAction = null)
        {
            _animationQueue.Enqueue(new BuffAnimationRequest {
                type = type,
                target = target,
                onCompleteAction = onCompleteAction
            });
            ProcessQueue();
        }

        public void ProcessQueue()
        {
            if (_isAnimating || _animationQueue.Count == 0)
                return;

            var req = _animationQueue.Dequeue();
            _isAnimating = true;

            Tween tween;
            if (req.type == AnimationType.ScaleIn)
            {
                tween = Tween.Scale(req.target, Vector3.one, 0.5f);
            }
            else
            {
                tween = Tween.PunchScale(req.target, Vector3.one * 1.3f, 0.5f);
            }

            tween.OnComplete(() =>
            {
                if (req.onCompleteAction != null)
                    req.onCompleteAction();
                _isAnimating = false;
                ProcessQueue();
            });
        }
        
    }
    
    
    
}