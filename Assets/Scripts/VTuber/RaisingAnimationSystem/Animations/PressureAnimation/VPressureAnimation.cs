using System;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.SE;
using VTuber.Core.UI;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.RaisingAnimationSystem.Animations.PressureAnimation
{
    public class VPressureAnimation : VRaisingAnimation
    {
        [SerializeField] private Image currentPressureImage;
        [SerializeField] private Image nextPressureImage;
        [SerializeField] private TMP_Text currentPressureName;
        [SerializeField] private TMP_Text nextPressureName;
        [SerializeField] private Image arrow;
        [SerializeField] private Transform pressureInitPosition;
        [SerializeField] private Transform pressurePosition;
        
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float arrowAppearDuration = 0.5f;
        [SerializeField] private float delay = 0.5f;
        [SerializeField]  float spacingAnimDuration = 0.5f;
        
        [FoldoutGroup("Animation Settings")]
        [LabelText("速度")]
        [SerializeField] private float speed = 1f;
        
        private float Interval(float baseDuration) => baseDuration / Mathf.Max(0.0001f, speed);
        
        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
        {
            var currentPressureLevel = VUIUtils.Instance.GetPressureInfo(request.currentPressureLevel);
            var nextPressureLevel = VUIUtils.Instance.GetPressureInfo(request.nextPressureLevel);
            
            currentPressureImage.sprite = currentPressureLevel.Value;
            nextPressureImage.sprite = nextPressureLevel.Value;
            currentPressureName.text = currentPressureLevel.Key;
            nextPressureName.text = nextPressureLevel.Key;
            
            var sequence = Sequence.Create();

            sequence
                .Chain(Tween.Position(currentPressureImage.transform, pressurePosition.position, Interval(fadeDuration), Ease.OutBack))
                .Chain(Tween.UIFillAmount(arrow, 1, Interval(arrowAppearDuration)))
                .ChainCallback(()=> nextPressureName.gameObject.SetActive(true))
                .Chain(Tween.Alpha(nextPressureImage, 1, Interval(arrowAppearDuration)))
                .Group(Tween.Custom(-100, 0, Interval(spacingAnimDuration), v => nextPressureName.characterSpacing = v))
                .ChainDelay(Interval(delay))
                .ChainCallback(() =>
                {
                    base.BeginAnimation(request, onComplete, isLastSameType);
                    onComplete?.Invoke();
                    nextPressureName.gameObject.SetActive(false);
                });
        }

        public override void ResetAnimation()
        {
            base.ResetAnimation();
            currentPressureImage.transform.position = pressureInitPosition.position;
            VUIUtils.SetImageAlpha(nextPressureImage, 0);
            arrow.fillAmount = 0;
            nextPressureName.characterSpacing = -100;
        }
    }
}