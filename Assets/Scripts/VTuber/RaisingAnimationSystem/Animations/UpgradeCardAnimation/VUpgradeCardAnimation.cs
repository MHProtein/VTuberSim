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

namespace VTuber.RaisingAnimationSystem.Animations.UpgradeCardAnimation
{
    public class VUpgradeCardAnimation : VRaisingAnimation
    {
        [SerializeField] private VCardUI cardUI;

        [SerializeField] private Transform halo;

        [SerializeField] private Image haloImage;

        [SerializeField] private Button confirmButton;
        
        [FoldoutGroup("出现阶段")]
        [LabelText("首次放大倍率")]
        [SerializeField] private float appearScale = 1.3f;

        [FoldoutGroup("出现阶段")]
        [LabelText("首次放大时长")]
        [SerializeField] private float appearDuration = 0.25f;

        [FoldoutGroup("出现阶段")]
        [LabelText("出现后延迟时长")]
        [SerializeField] private float afterAppearDelay = 0.75f;

        
        [FoldoutGroup("升级动画")]
        [LabelText("缩小到 0 的时长")]
        [SerializeField] private float shrinkDuration = 0.5f;

        [FoldoutGroup("升级动画")]
        [LabelText("升级后放大倍率")]
        [SerializeField] private float upgradeScale = 1.6f;

        [FoldoutGroup("升级动画")]
        [LabelText("升级后放大时长")]
        [SerializeField] private float upgradeScaleDuration = 0.5f;

        [FoldoutGroup("升级动画")]
        [LabelText("光环展开时长")]
        [SerializeField] private float haloExpandDuration = 0.5f;

        [FoldoutGroup("升级动画")]
        [LabelText("升级展示后延迟")]
        [SerializeField] private float afterUpgradeDelay = 0.75f;
        
        
        [FoldoutGroup("呼吸动画")]
        [LabelText("呼吸动画往返时长")]
        [SerializeField] private float cardPulseDuration = 3.0f;

        [FoldoutGroup("呼吸动画")]
        [LabelText("呼吸放大倍数")]
        [SerializeField] private float cardPulseScale = 1.8f;
        
        [FoldoutGroup("光环旋转")]
        [LabelText("光环旋转一圈时长")]
        [SerializeField] private float haloSpinDuration = 8f;

        [FoldoutGroup("音效")] [LabelText("卡牌出现音效")] [SerializeField] private VAudioPlayInfo appearAudio;
        [FoldoutGroup("音效")] [LabelText("卡牌升级音效")] [SerializeField] private VAudioPlayInfo upgradeAudio;
        [FoldoutGroup("音效")] [LabelText("光环旋转音效")] [SerializeField] private VAudioPlayInfo haloSpinAudio;
        [FoldoutGroup("音效")] [LabelText("确认音效")] [SerializeField] private VAudioPlayInfo confirmAudio;
        
        private Action _onComplete;
        private Sequence _sequence;

        protected override void Awake()
        {
            base.Awake();
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        private void OnConfirmButtonClicked()
        {
            _sequence.Stop();
            _sequence = Sequence.Create();
            _sequence
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(confirmAudio))
                .Chain(cardUI.TweenAlpha(1.0f, 0.25f))
                .Group(Tween.Alpha(haloImage, 0.0f, 0.25f))
                .ChainCallback(() => { _onComplete?.Invoke(); });
        }

        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
        {
            _onComplete = onComplete;
            _sequence = Sequence.Create();
            if (!debug)
            {
                cardUI.SetCard(request.cards.First());
                haloImage.sprite = VUIUtils.Instance.GetHaloSprite((int)cardUI.Card.Rarity);
            }

            confirmButton.interactable = false;
            _sequence
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(appearAudio))
                .Chain(Tween.Scale(cardUI.transform, Vector3.one * appearScale, appearDuration, Ease.OutBack))
                .Group(cardUI.TweenAlpha(1.0f, appearDuration))
                .ChainDelay(afterAppearDelay)
                .Chain(Tween.Scale(cardUI.transform, Vector3.zero, shrinkDuration, Ease.InQuart))
                .ChainCallback(() =>
                {
                    VAudioPlayer.Instance.PlaySFX(upgradeAudio);
                    if (!debug)
                    {
                        cardUI.Card.Upgrade(false);
                        cardUI.UpdateView();
                    }
                })
                .Chain(Tween.Scale(cardUI.transform, Vector3.one * upgradeScale, upgradeScaleDuration, Ease.OutBack))
                .Chain(Tween.Scale(halo, Vector3.one, haloExpandDuration, Ease.OutBack))
                .ChainCallback(() => confirmButton.interactable = true)
                .ChainDelay(afterUpgradeDelay)
                .Chain(
                    Tween.Scale(
                        cardUI.transform,
                        cardPulseScale,
                        cardPulseDuration,
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
                haloSpinDuration,
                Ease.Linear,
                -1,
                CycleMode.Incremental
            );
        }

        public override void ResetAnimation()
        {
            base.ResetAnimation();
            cardUI.transform.localScale = Vector3.one * 5.0f;
            halo.localScale = Vector3.zero;
            VUIUtils.SetImageAlpha(haloImage, 1);
            cardUI.SetAlpha(0.0f);
        }
    }
}
