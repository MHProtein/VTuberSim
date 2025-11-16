using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.UI;
using VTuber.Core.Managers;
using VTuber.RaisingAnimationSystem;

namespace VTuber.ScheduleSystem.UI.RaisingAnimationSystem.RemoveCardAnimation
{
    public class VRemoveCardAnimation : VRaisingAnimation
    {
        [SerializeField] private VCardUI cardUI;
        [SerializeField] private Transform smoke;

        private Vector3 _initScale;
        protected override void Awake()
        {
            base.Awake();
            _initScale = cardUI.transform.localScale;
        }

        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
        {
            base.BeginAnimation(request, onComplete, isLastSameType);

            cardUI.transform.localScale = Vector3.one * 3.0f;
            if (!debug)
            {
                cardUI.SetCard(request.cards[0]);
            }

            var sequence = Sequence.Create();
            sequence
                .Chain(Tween.Scale(cardUI.transform, _initScale, 0.5f, Ease.OutBack))
                .Group(cardUI.TweenAlpha(1.0f, 0.5f))
                .ChainDelay(0.5f)
                .Chain(Tween.Scale(cardUI.transform, Vector3.zero, 0.5f, Ease.InQuart))
                .Group(Tween.Scale(smoke, Vector3.one, 1.0f, Ease.OutCubic))
                .Chain(Tween.Scale(smoke, Vector3.zero, 0.15f))
                .ChainDelay(0.25f)
                .ChainCallback(() =>
                {
                    request.effectApply?.Invoke();

                    onComplete?.Invoke();
                });

        }

        public override void ResetAnimation()
        {
            base.ResetAnimation();
            smoke.localScale = Vector3.zero;
            cardUI.SetAlpha(0.0f);
        }
    }
}