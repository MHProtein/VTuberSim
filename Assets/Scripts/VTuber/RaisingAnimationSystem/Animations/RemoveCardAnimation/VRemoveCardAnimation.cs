using System;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.BattleSystem.UI;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.RaisingAnimationSystem.Animations.RemoveCardAnimation
{
    public class VRemoveCardAnimation : VRaisingAnimation
    {
        [FoldoutGroup("基础引用")]
        [LabelText("卡牌 UI")]
        [SerializeField] private VCardUI cardUI;

        [FoldoutGroup("基础引用")]
        [LabelText("烟雾特效")]
        [SerializeField] private Transform smoke;
        
        
        [FoldoutGroup("初始设置")]
        [LabelText("卡牌初始放大倍数")]
        [SerializeField] private float cardStartScale = 3.0f;


        [FoldoutGroup("出现动画")]
        [LabelText("卡牌缩放到正常大小时长")]
        [SerializeField] private float cardScaleInDuration = 0.5f;

        [FoldoutGroup("出现动画")]
        [LabelText("卡牌淡入时长")]
        [SerializeField] private float cardFadeInDuration = 0.5f;

        [FoldoutGroup("出现动画")]
        [LabelText("出现后等待时间")]
        [SerializeField] private float cardPauseBeforeRemoval = 0.5f;


        [FoldoutGroup("移除动画")]
        [LabelText("卡牌缩小到 0 的时长")]
        [SerializeField] private float cardShrinkDuration = 0.5f;

        [FoldoutGroup("移除动画")]
        [LabelText("烟雾放大时长")]
        [SerializeField] private float smokeExpandDuration = 1.0f;

        [FoldoutGroup("移除动画")]
        [LabelText("烟雾消失时长")]
        [SerializeField] private float smokeFadeOutDuration = 0.15f;

        [FoldoutGroup("移除动画")]
        [LabelText("烟雾消失后等待")]
        [SerializeField] private float smokeFinishDelay = 0.25f;


        private Vector3 _initScale;

        protected override void Awake()
        {
            base.Awake();
            _initScale = cardUI.transform.localScale;
        }

        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
        {
            base.BeginAnimation(request, onComplete, isLastSameType);

            cardUI.transform.localScale = Vector3.one * cardStartScale;

            if (!debug)
            {
                cardUI.SetCard(request.cards[0]);
            }

            var sequence = Sequence.Create();
            sequence
                .Chain(Tween.Scale(cardUI.transform, _initScale, cardScaleInDuration, Ease.OutBack))
                .Group(cardUI.TweenAlpha(1.0f, cardFadeInDuration))
                .ChainDelay(cardPauseBeforeRemoval)
                .Chain(Tween.Scale(cardUI.transform, Vector3.zero, cardShrinkDuration, Ease.InQuart))
                .Group(Tween.Scale(smoke, Vector3.one, smokeExpandDuration, Ease.OutCubic))
                .Chain(Tween.Scale(smoke, Vector3.zero, smokeFadeOutDuration))
                .ChainDelay(smokeFinishDelay)
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
