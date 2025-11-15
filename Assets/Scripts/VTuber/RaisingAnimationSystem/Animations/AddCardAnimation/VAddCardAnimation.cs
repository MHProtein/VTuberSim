using System;
using System.Linq;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.UI;
using VTuber.Core.Managers;
using VTuber.Core.UI;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.RaisingAnimationSystem.Animations.AddCardAnimation
{
    public class VAddCardAnimation : VRaisingAnimation
    {
        [SerializeField] private VCardUI cardUI;

        [SerializeField] private Transform halo;

        [SerializeField] private Image haloImage;

        [SerializeField] private Button confirmButton;

        [SerializeField] private Transform cardLibraryPosition;

        [FoldoutGroup("卡牌出现动画")]
        [LabelText("出现动画时长")]
        [SerializeField] private float appearDuration = 0.5f;

        [FoldoutGroup("卡牌出现动画")]
        [LabelText("出现缓动类型")]
        [SerializeField] private Ease appearEase = Ease.OutBack;
        
        [FoldoutGroup("呼吸动画")]
        [LabelText("呼吸动画时长（往返）")]
        [SerializeField] private float cardPulseDuration = 3.0f;

        [FoldoutGroup("呼吸动画")]
        [LabelText("呼吸放大倍数")]
        [SerializeField] private float cardPulseScale = 1.8f;

        [FoldoutGroup("呼吸动画")]
        [LabelText("呼吸循环次数（-1 无限）")]
        [SerializeField] private int cardPulseLoops = 1000;

        [FoldoutGroup("呼吸动画")]
        [LabelText("呼吸循环模式")]
        [SerializeField] private CycleMode cardPulseCycle = CycleMode.Rewind;

        [FoldoutGroup("光环旋转")]
        [LabelText("光环旋转时长")]
        [SerializeField] private float haloSpinDuration = 8f;

        [FoldoutGroup("光环旋转")]
        [LabelText("旋转循环模式")]
        [SerializeField] private CycleMode haloSpinCycle = CycleMode.Incremental;
        
        [FoldoutGroup("移入卡库动画")]
        [LabelText("缩小时长")]
        [SerializeField] private float moveShrinkDuration = 0.5f;

        [FoldoutGroup("移入卡库动画")]
        [LabelText("缩小缓动")]
        [SerializeField] private Ease moveShrinkEase = Ease.InCubic;

        [FoldoutGroup("移入卡库动画")]
        [LabelText("位置动画时长")]
        [SerializeField] private float movePositionDuration = 0.5f;

        [FoldoutGroup("移入卡库动画")]
        [LabelText("位置缓动")]
        [SerializeField] private Ease movePositionEase = Ease.InCubic;

        [FoldoutGroup("移入卡库动画")]
        [LabelText("光环淡出时长")]
        [SerializeField] private float haloFadeDuration = 0.5f;


        private Action _onComplete;
        private Action _applyEffect;
        private Sequence _sequence;
        private Vector3 _initScale;


        protected override void Awake()
        {
            base.Awake();
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            _initScale = cardUI.transform.localScale;
        }


        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLast)
        {
            if (!debug)
            {
                _applyEffect = request.effectApply;
                cardUI.SetCard(request.cards.First());
                haloImage.sprite = VUIUtils.Instance.GetHaloSprite((int)cardUI.Card.Rarity);
            }

            _onComplete = onComplete;
            _applyEffect = request.effectApply;
            _sequence = Sequence.Create();


            _sequence
                .Chain(Tween.Scale(cardUI.transform, _initScale, appearDuration, appearEase))
                .Group(Tween.Scale(halo, Vector3.one, appearDuration, Ease.OutCubic))

                .Chain(
                    Tween.Scale(
                        cardUI.transform,
                        cardPulseScale,
                        cardPulseDuration,
                        Ease.InOutCubic,
                        cardPulseLoops,
                        cardPulseCycle
                    )
                );
            
            Tween.LocalEulerAngles(
                halo,
                Vector3.zero,
                new Vector3(0, 0, 360f),
                haloSpinDuration,
                Ease.Linear,
                -1,
                CycleMode.Incremental
            );
        }


        private void OnConfirmButtonClicked()
        {
            _sequence.Stop();

            var moveToLibrarySeq = Sequence.Create();

            cardUI.transform.SetParent(cardLibraryPosition);

            moveToLibrarySeq
                .Chain(Tween.Scale(cardUI.transform, Vector3.zero, moveShrinkDuration, moveShrinkEase))
                .Group(Tween.LocalPosition(cardUI.transform, Vector3.zero, movePositionDuration, movePositionEase))
                .Group(Tween.Alpha(haloImage, 0f, haloFadeDuration))
                .ChainCallback(() =>
                {
                    cardUI.transform.SetParent(ui.transform);
                    cardUI.transform.localPosition = Vector3.zero;

                    if (!debug)
                        _applyEffect?.Invoke();

                    _onComplete?.Invoke();
                });
        }


        public override void ResetAnimation()
        {
            base.ResetAnimation();
            haloImage.color = Color.white;
            cardUI.transform.localScale = Vector3.zero;
            halo.localScale = Vector3.zero;
        }
    }
}
