using System;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.UI;
using VTuber.Core.SE;
using VTuber.Core.UI;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.RaisingAnimationSystem.Animations.ReplaceCardAnimation
{
    public class VReplaceCardAnimation : VRaisingAnimation
    {
        [SerializeField] private VCardUI cardUI;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Transform halo;
        [SerializeField] private Image haloImage;
        [SerializeField] private Transform cardLibrary;

        [FoldoutGroup("卡牌翻转动画")]
        [LabelText("翻转总时长")]
        [SerializeField] private float flipDuration = 3f;

        [FoldoutGroup("卡牌翻转动画")]
        [LabelText("翻转角度")]
        [SerializeField] private float flipAngle = 36000f;

        [FoldoutGroup("卡牌翻转动画")]
        [LabelText("翻转缓动类型")]
        [SerializeField] private Ease flipEase = Ease.InOutCirc;


        [FoldoutGroup("光环旋转")]
        [LabelText("光环旋转时长")]
        [SerializeField] private float haloSpinDuration = 8f;

        [FoldoutGroup("确认动画")]
        [LabelText("回收时间（移动）")]
        [SerializeField] private float collectMoveDuration = 0.5f;

        [FoldoutGroup("确认动画")]
        [LabelText("回收时间（缩放）")]
        [SerializeField] private float collectScaleDuration = 0.5f;

        [FoldoutGroup("音效")] [LabelText("卡牌出现音效")] [SerializeField] private VAudioPlayInfo appearAudio;
        [FoldoutGroup("音效")] [LabelText("卡牌翻转音效")] [SerializeField] private VAudioPlayInfo flipAudio;
        [FoldoutGroup("音效")] [LabelText("光环旋转音效")] [SerializeField] private VAudioPlayInfo haloSpinAudio;
        [FoldoutGroup("音效")] [LabelText("卡牌回收音效")] [SerializeField] private VAudioPlayInfo collectAudio;

        private VCard _cardToReplace;
        private VCard _cardToBeReplaced;
        private Action _onComplete;


        protected override void Awake()
        {
            base.Awake();
            confirmButton.onClick.AddListener(Confirm);
        }

        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
        {
            _onComplete = onComplete;
            if (!debug)
            {
                _cardToReplace = request.cards[0];
                _cardToBeReplaced = request.cards[1];
                cardUI.SetCard(_cardToBeReplaced);
                haloImage.sprite = VUIUtils.Instance.GetHaloSprite((int)_cardToReplace.Rarity);
            }
            cardUI.SetAlpha(0);
            VUIUtils.SetImageAlpha(haloImage, 0);
            Sequence sequence = Sequence.Create();
            Sequence replaceSequence = Sequence.Create();
            confirmButton.interactable = false;
            replaceSequence.ChainDelay(flipDuration / 2);
            replaceSequence.ChainCallback(() => {
                VAudioPlayer.Instance.PlaySFX(flipAudio);
                if (debug)
                    cardUI.SetBackgroundColor(Color.cyan);
                else
                    cardUI.SetCard(_cardToReplace);
            });
            replaceSequence.Group(Tween.Alpha(haloImage, 1, flipDuration / 2));
            sequence.ChainCallback(() => VAudioPlayer.Instance.PlaySFX(appearAudio));
            sequence.Chain(cardUI.TweenAlpha(1, 0.5f));
            sequence.Group(replaceSequence);
            sequence.Group(Tween.LocalEulerAngles(
                cardUI.transform,
                Vector3.zero,
                new Vector3(0, flipAngle, 0),
                flipDuration,
                flipEase
            ));
            sequence.ChainCallback(() => confirmButton.interactable = true);
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

        private void Confirm()
        {
            cardUI.transform.SetParent(cardLibrary);
            var sequence = Sequence.Create();
            sequence
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(collectAudio))
                .Chain(Tween.LocalPosition(cardUI.transform, Vector3.zero, collectMoveDuration, Ease.InSine))
                .Group(Tween.Scale(cardUI.transform, Vector3.zero, collectScaleDuration, Ease.InBack))
                .ChainCallback(() =>
                {
                    _onComplete?.Invoke();
                    if (!debug)
                        VGameManager.Instance.Character.CardLibrary.ReplaceCard(_cardToReplace, _cardToBeReplaced);
                    cardUI.transform.SetParent(ui.transform);
                    cardUI.transform.localScale = Vector3.zero;
                });
        }

        public override void ResetAnimation()
        {
            base.ResetAnimation();
            cardUI.transform.localRotation = Quaternion.identity;
        }
    }
}
