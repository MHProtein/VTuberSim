using System;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Managers;
using VTuber.Core.SE;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.RaisingAnimationSystem.Animations.AddRelicAnimation
{
    public class VAddRelicAnimation : VRaisingAnimation
    {
        [SerializeField] private Image relicImage;
        [SerializeField] private Transform relicInitPosition;
        
        [SerializeField] private Transform descriptionObject;
        [SerializeField] private TMP_Text relicDescriptionText;
        [SerializeField] private Transform descriptionInitPosition;
        [SerializeField] private Transform descriptionPosition;
        
        [SerializeField] private TMP_Text relicNameText;
        [SerializeField] private Transform relicLibraryPosition;
        [SerializeField] private Button addRelicButton;
        [SerializeField] private Transform light;

        [Header("Animation Settings")]

        [LabelText("遗物入场位置移动时长")]
        [SerializeField] private float relicEnterDuration = 0.5f;

        [LabelText("遗物入场缩放时长")]
        [SerializeField] private float relicEnterScaleDuration = 0.5f;

        [LabelText("遗物入场透明度渐变时长")]
        [SerializeField] private float relicEnterAlphaDuration = 0.5f;

        [LabelText("遗物名称显示时长")]
        [SerializeField] private float relicNameFadeDuration = 0.5f;

        [LabelText("文字间距恢复时长")]
        [SerializeField] private float nameSpacingDuration = 0.5f;

        [LabelText("描述面板移动时长")]
        [SerializeField] private float descriptionMoveDuration = 0.5f;

        [LabelText("描述面板缓动")]
        [SerializeField] private Ease descriptionMoveEase = Ease.OutCubic;

        [LabelText("允许点击按钮的延迟")]
        [SerializeField] private float enableButtonDelay = 0.0f;

        [LabelText("遗物呼吸动画时长（往返）")]
        [SerializeField] private float relicPulseDuration = 3.0f;

        [LabelText("遗物呼吸放大倍数")]
        [SerializeField] private float relicPulseScale = 1.3f;

        [LabelText("呼吸动画循环次数（-1无限）")]
        [SerializeField] private int relicPulseCycles = 100;

        [LabelText("光环旋转一圈时间")]
        [SerializeField] private float lightSpinDuration = 8f;

        [LabelText("光环旋转循环次数（-1无限）")]
        [SerializeField] private int lightSpinCycles = -1;

        [LabelText("描述面板退场时长")]
        [SerializeField] private float descriptionExitDuration = 0.5f;

        [LabelText("遗物缩小至图鉴时长")]
        [SerializeField] private float relicShrinkDuration = 0.5f;

        [LabelText("遗物移动到图鉴位置时长")]
        [SerializeField] private float relicMoveToLibraryDuration = 0.5f;

        [LabelText("名称透明度隐藏时长")]
        [SerializeField] private float relicNameHideDuration = 0.5f;

        [FoldoutGroup("音效")] [LabelText("遗物出现音效")] [SerializeField] private VAudioPlayInfo appearAudio;
        [FoldoutGroup("音效")] [LabelText("描述框出现音效")] [SerializeField] private VAudioPlayInfo descriptionAppearAudio;
        [FoldoutGroup("音效")] [LabelText("光环旋转音效")] [SerializeField] private VAudioPlayInfo haloSpinAudio;
        [FoldoutGroup("音效")] [LabelText("移动到库音效")] [SerializeField] private VAudioPlayInfo moveShrinkAudio;

        private Action _onComplete;
        private Action _applyEffect;
        private Sequence _sequence;

        protected override void Awake()
        {
            base.Awake();
            addRelicButton.onClick.AddListener(OnAddRelicButtonClicked);
        }


        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
        {
            _onComplete = onComplete;

            if (!debug)
            {
                var relic = VDataManager.Instance.Relics[request.relicId];
                relicImage.sprite = relic.icon;
                relicNameText.text = relic.relicName;
                relicDescriptionText.text = relic.description;
                _applyEffect = request.effectApply;
            }

            addRelicButton.interactable = false;

            _sequence = Sequence.Create();

            _sequence
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(appearAudio))
                .Chain(Tween.LocalPosition(relicImage.transform, Vector3.zero, relicEnterDuration, Ease.OutBounce))
                .Group(Tween.Alpha(relicImage, 1, relicEnterAlphaDuration))
                .Group(Tween.Scale(relicImage.transform, Vector3.one, relicEnterScaleDuration, Ease.OutBounce))
                .Group(Tween.Alpha(relicNameText, 1, relicNameFadeDuration))
                .Group(Tween.Custom(relicNameText.characterSpacing, 0, nameSpacingDuration,
                    v => relicNameText.characterSpacing = v))
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(descriptionAppearAudio))
                .Chain(Tween.LocalPosition(descriptionObject, descriptionPosition.localPosition, descriptionMoveDuration, descriptionMoveEase))
                .ChainCallback(() => { addRelicButton.interactable = true; });

            // breathing pulse animation
            _sequence.Group(
                Tween.Scale(
                    relicImage.transform,
                    Vector3.one * relicPulseScale,
                    relicPulseDuration,
                    Ease.InCubic,
                    relicPulseCycles,
                    CycleMode.Rewind
                )
            );

            VAudioPlayer.Instance.PlaySFX(haloSpinAudio);
            // rotating light
            Tween.LocalEulerAngles(
                light,
                Vector3.zero,
                new Vector3(0, 0, 360f),
                lightSpinDuration,
                Ease.Linear,
                lightSpinCycles,
                CycleMode.Incremental
            );
        }
        
        public void OnAddRelicButtonClicked()
        {
            _sequence.Stop();
            addRelicButton.interactable = false;

            var sequence = Sequence.Create();

            relicImage.transform.SetParent(relicLibraryPosition);

            sequence
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(moveShrinkAudio))
                .Chain(Tween.LocalPosition(descriptionObject, descriptionInitPosition.localPosition, descriptionExitDuration, Ease.OutCubic))
                .Group(Tween.Scale(relicImage.transform, Vector3.zero, relicShrinkDuration, Ease.InCubic))
                .Group(Tween.LocalPosition(relicImage.transform, Vector3.zero, relicMoveToLibraryDuration, Ease.InCubic))
                .Group(Tween.Custom(relicNameText.characterSpacing, 0, relicNameHideDuration, v => relicNameText.characterSpacing = v))
                .Group(Tween.Alpha(relicNameText, 0, relicNameHideDuration))
                .ChainCallback(() =>
                {
                    relicImage.transform.SetParent(ui.transform);
                    if (!debug)
                        _applyEffect?.Invoke();
                    _onComplete?.Invoke();
                });
        }


        public override void ResetAnimation()
        {
            base.ResetAnimation();

            relicImage.color = new Color(1, 1, 1, 0);
            relicImage.transform.localPosition = relicInitPosition.localPosition;
            relicImage.transform.localScale = Vector3.one * 10.0f;

            relicNameText.alpha = 0;
            relicNameText.characterSpacing = -100;

            descriptionObject.localPosition = descriptionInitPosition.localPosition;
        }
    }
}
