using System;
using System.Linq;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.UI;
using VTuber.Core.SE;
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
        
        [FoldoutGroup("光环旋转")]
        [LabelText("光环旋转时长")]
        [SerializeField] private float haloSpinDuration = 8f;
        
        [FoldoutGroup("移入卡库动画")]
        [LabelText("缩小时长")]
        [SerializeField] private float moveShrinkDuration = 0.5f;

        [FoldoutGroup("移入卡库动画")]
        [LabelText("位置动画时长")]
        [SerializeField] private float movePositionDuration = 0.5f;

        [FoldoutGroup("移入卡库动画")]
        [LabelText("光环淡出时长")]
        [SerializeField] private float haloFadeDuration = 0.5f;

        // 新增：动画速度（乘算到所有时间间隔）
        [FoldoutGroup("动画设置")]
        [LabelText("速度")]
        [SerializeField] private float speed = 1f;

        // 辅助：把基础时长乘以速度，避免非正值
        private float Interval(float baseDuration) => baseDuration / Mathf.Max(0.0001f, speed);

        [FoldoutGroup("音效")] [LabelText("卡牌出现音效")] [SerializeField] private VAudioPlayInfo appearAudio;
        [FoldoutGroup("音效")] [LabelText("光环旋转音效")] [SerializeField] private VAudioPlayInfo haloSpinAudio;
        [FoldoutGroup("音效")] [LabelText("移动到卡牌库音效")] [SerializeField] private VAudioPlayInfo moveShrinkAudio;

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
        
        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
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
            confirmButton.interactable = false;
            
            _sequence
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(appearAudio))
                .Chain(Tween.Scale(cardUI.transform, _initScale, Interval(appearDuration), appearEase))
                .Group(Tween.Scale(halo, Vector3.one, Interval(appearDuration), Ease.OutCubic))
                .ChainCallback(() => confirmButton.interactable = true)
                .Chain(
                    Tween.Scale(
                        cardUI.transform,
                        cardPulseScale,
                        Interval(cardPulseDuration),
                        Ease.InOutCubic,
                        1000,
                        CycleMode.Rewind
                    )
                );
            
            VAudioPlayer.Instance.PlaySFX(haloSpinAudio);

            Tween.LocalEulerAngles(
                halo,
                Vector3.zero,
                new Vector3(0, 0, 360f),
                Interval(haloSpinDuration),
                Ease.Linear,
                -1,
                CycleMode.Incremental
            );
        }

        private void OnConfirmButtonClicked()
        {
            _sequence.Stop();

            confirmButton.interactable = false;
            var moveToLibrarySeq = Sequence.Create();

            cardUI.transform.SetParent(cardLibraryPosition);

            moveToLibrarySeq
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(moveShrinkAudio))
                .Group(Tween.Alpha(haloImage, 0f, Interval(haloFadeDuration)))
                .Group(Tween.Scale(cardUI.transform, Vector3.zero, Interval(moveShrinkDuration), Ease.InCubic))
                .Group(Tween.LocalPosition(cardUI.transform, Vector3.zero, Interval(movePositionDuration), Ease.InOutCubic))
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
