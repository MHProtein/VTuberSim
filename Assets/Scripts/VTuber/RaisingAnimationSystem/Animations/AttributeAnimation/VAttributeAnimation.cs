using System;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.SE;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.RaisingAnimationSystem.Animations.AttributeAnimation
{
    public class VAttributeAnimationView : VRaisingAnimation
    {
        [Header("References")]
        [SerializeField] private Transform infoTransform;
        [SerializeField] private Transform infoInitPosition;
        [SerializeField] private Transform infoPosition;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private Transform light;

        [Header("Animation Settings")]

        [LabelText("弹入缩放")]
        [SerializeField] private float popScale = 1.0f;


        [LabelText("淡入时长")]
        [SerializeField] private float fadeDuration = 0.25f;

        [LabelText("移动时长")]
        [SerializeField] private float moveDuration = 0.25f;

        [LabelText("停留时长")]
        [SerializeField] private float holdDuration = 2.0f;

        [LabelText("淡出时长")]
        [SerializeField] private float fadeOutDuration = 0.1f;

        [LabelText("360度旋转时长")]
        [SerializeField] private float lightSpinDuration = 8f;

        // 新增：动画速度（乘算到所有时间间隔）
        [FoldoutGroup("Animation Settings")]
        [LabelText("速度")]
        [SerializeField] private float speed = 1f;

        // 辅助：将基础时长乘以速度，避免非正值
        private float Interval(float baseDuration) => baseDuration / Mathf.Max(0.0001f, speed);

        [Header("初始缩放")]
        [LabelText("初始缩放值")]
        [SerializeField] private float initScale = 1.5f;

        [FoldoutGroup("音效")] [LabelText("出现音效")] [SerializeField] private VAudioPlayInfo appearAudio;
        [FoldoutGroup("音效")] [LabelText("光环旋转音效")] [SerializeField] private VAudioPlayInfo haloSpinAudio;
        [FoldoutGroup("音效")] [LabelText("消失音效")] [SerializeField] private VAudioPlayInfo disappearAudio;

        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
        {
            ResetAnimation();
            base.BeginAnimation(request, onComplete, isLastSameType);

            // Display values
            icon.sprite = request.attributeIcon;
            valueText.color = request.value > 0 ? Color.green : Color.red;
            valueText.text = (request.value > 0 ? "+" : "") + request.value;

            // Build sequence
            var sequence = Sequence.Create();

            sequence
                // pop animation
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(appearAudio))
                .Chain(Tween.Scale(infoTransform, popScale, Interval(fadeDuration), Ease.OutElastic))

                // fade in + move at the same time
                .Group(Tween.Alpha(icon, 1, Interval(fadeDuration)))
                .Group(Tween.Alpha(valueText, 1, Interval(fadeDuration)))
                .Group(Tween.Position(infoTransform, infoPosition.position, Interval(fadeDuration)))

                // wait
                .ChainDelay(Interval(holdDuration))

                // fade out
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(disappearAudio))
                .Chain(Tween.Alpha(icon, 0, Interval(fadeOutDuration)))
                .Group(Tween.Alpha(valueText, 0, Interval(fadeOutDuration)))

                // final callback
                .ChainCallback(() => onComplete?.Invoke());


            VAudioPlayer.Instance.PlaySFX(haloSpinAudio);
            // light rotation
            Tween.LocalEulerAngles(
                light,
                Vector3.zero,
                new Vector3(0, 0, 360f),
                Interval(lightSpinDuration),
                Ease.Linear,
                100,
                CycleMode.Incremental
            );
        }

        public override void ResetAnimation()
        {
            base.ResetAnimation();

            icon.color = new Color(1, 1, 1, 0);
            valueText.alpha = 0;
            infoTransform.localScale = Vector3.one * initScale;
            infoTransform.position = infoInitPosition.position;
        }
    }
}
