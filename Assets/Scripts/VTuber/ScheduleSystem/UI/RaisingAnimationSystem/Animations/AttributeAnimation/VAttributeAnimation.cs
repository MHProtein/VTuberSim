using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;
using Sirenix.OdinInspector;

namespace VTuber.ScheduleSystem.UI.RaisingAnimationSystem
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

        [Header("初始缩放")]
        [LabelText("初始缩放值")]
        [SerializeField] private float initScale = 1.5f;


        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLast)
        {
            ResetAnimation();
            base.BeginAnimation(request, onComplete, isLast);

            // Display values
            icon.sprite = request.attributeIcon;
            valueText.color = request.value > 0 ? Color.green : Color.red;
            valueText.text = (request.value > 0 ? "+" : "") + request.value;

            // Build sequence
            var sequence = Sequence.Create();

            sequence
                // pop animation
                .Chain(Tween.Scale(infoTransform, popScale, fadeDuration, Ease.OutElastic))

                // fade in + move at the same time
                .Group(Tween.Alpha(icon, 1, fadeDuration))
                .Group(Tween.Alpha(valueText, 1, fadeDuration))
                .Group(Tween.Position(infoTransform, infoPosition.position, fadeDuration))

                // wait
                .ChainDelay(holdDuration)

                // fade out
                .Chain(Tween.Alpha(icon, 0, fadeOutDuration))
                .Group(Tween.Alpha(valueText, 0, fadeOutDuration))

                // final callback
                .ChainCallback(() => onComplete?.Invoke());

            // light rotation
            Tween.LocalEulerAngles(
                light,
                Vector3.zero,
                new Vector3(0, 0, 360f),
                lightSpinDuration,
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
