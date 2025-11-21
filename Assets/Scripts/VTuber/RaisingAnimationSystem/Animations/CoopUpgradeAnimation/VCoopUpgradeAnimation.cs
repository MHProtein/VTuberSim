using System;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.SE;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.RaisingAnimationSystem.Animations.CoopUpgradeAnimation
{
    public class VCoopUpgradeAnimation : VRaisingAnimation, IPointerClickHandler
    {
        [SerializeField] private Transform coopInfo;
        [SerializeField] private Image coopIcon;
        [SerializeField] private TMP_Text coopName;
        [SerializeField] private Transform coopInfoInitPosition;
        [SerializeField] private Transform light;
 
        [LabelText("360度旋转时长")]
        [SerializeField] private float lightSpinDuration = 8f;     
        [Header("Phase 1")] 
        [SerializeField] private Transform coopInfoPhase1Position;
        [SerializeField] private TMP_Text upgradeText;

        [Header("Phase 1 动画时间（单位：秒）")]
        [LabelText("角色信息移动到第一阶段位置时间")][SerializeField] private float phase1MoveDuration = 0.5f;           
        [LabelText("关系加深淡入时间")][SerializeField] private float phase1TextFadeDuration = 0.5f;       
        [LabelText("到第二阶段等待时间")][SerializeField] private float phase1Delay = 1.0f;                  
        
        [Header("Phase 2")]
        [SerializeField] private Image arrow;
        [SerializeField] private Transform coopInfoPhase2Position;
        [SerializeField] private Transform currentLevelTextInitPosition;
        [SerializeField] private Transform currentLevelTextPosition;
        [SerializeField] private TMP_Text currentLevelText;
        [SerializeField] private TMP_Text upgradeLevelText;

        [Header("Phase 2 动画时间（单位：秒）")]
        [LabelText("角色信息移动到第二阶段位置时间")][SerializeField] private float phase2MoveDuration = 0.5f;          
        [LabelText("当前等级文字移动时间")][SerializeField] private float currentLevelMoveDuration = 0.25f;   
        [LabelText("箭头填充时间")][SerializeField] private float arrowFillDuration = 0.25f;          
        [LabelText("下一等级文字淡入时间")][SerializeField] private float upgradeLevelFadeDuration = 0.25f;   
        [LabelText("字间距动画时间")][SerializeField] private float spacingAnimDuration = 0.5f;   

        [FoldoutGroup("音效")] [LabelText("角色信息移动音效")] [SerializeField] private VAudioPlayInfo moveInfoAudio;
        [FoldoutGroup("音效")] [LabelText("关系加深文字淡入音效")] [SerializeField] private VAudioPlayInfo upgradeTextAudio;
        [FoldoutGroup("音效")] [LabelText("角色信息移动到第二阶段音效")] [SerializeField] private VAudioPlayInfo moveInfoPhase2Audio;
        [FoldoutGroup("音效")] [LabelText("当前等级文字移动音效")] [SerializeField] private VAudioPlayInfo currentLevelMoveAudio;
        [FoldoutGroup("音效")] [LabelText("箭头填充音效")] [SerializeField] private VAudioPlayInfo arrowFillAudio;
        [FoldoutGroup("音效")] [LabelText("下一等级文字淡入音效")] [SerializeField] private VAudioPlayInfo upgradeLevelFadeAudio;
        [FoldoutGroup("音效")] [LabelText("字间距动画音效")] [SerializeField] private VAudioPlayInfo spacingAnimAudio;
        [FoldoutGroup("音效")] [LabelText("光环旋转音效")] [SerializeField] private VAudioPlayInfo haloSpinAudio;

        [FoldoutGroup("动画速度")]
        [LabelText("动画速度倍数")]
        [SerializeField] private float speed = 1f;
        private float Interval(float baseDuration) => baseDuration / Mathf.Max(0.0001f, speed);

        private Action _onComplete;
        private bool _isAnimating;
        private bool _clicked;

        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
        {
            _clicked = false;
            _onComplete = onComplete;
            if (!debug)
            {
                var coop = request.coop;

                coopIcon.sprite = coop.Pfp;
                coopName.text = coop.configuration.Name;
                currentLevelText.text = coop.CurrentCoopLevel.levelName;
                upgradeLevelText.text = coop.GetNextLevel().levelName;
            }
            _isAnimating = true;
            var sequence = Sequence.Create();
            sequence
                // Phase 1: Move coopInfo
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(moveInfoAudio))
                .Chain(Tween.Position(coopInfo.transform, coopInfoPhase1Position.position, Interval(phase1MoveDuration), Ease.OutBounce))

                // Fade in upgradeText
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(upgradeTextAudio))
                .Chain(Tween.Alpha(upgradeText, 1, Interval(phase1TextFadeDuration)))

                // Delay
                .ChainDelay(Interval(phase1Delay))

                // Phase 2: Move coopInfo down
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(moveInfoPhase2Audio))
                .Chain(Tween.Position(coopInfo.transform, coopInfoPhase2Position.position, Interval(phase2MoveDuration)))

                // Show current level text
                .ChainCallback(() => currentLevelText.alpha = 1)

                // Animate current level text position
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(currentLevelMoveAudio))
                .Chain(Tween.Position(currentLevelText.transform, currentLevelTextPosition.position, Interval(currentLevelMoveDuration)))

                // Arrow fill
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(arrowFillAudio))
                .Chain(Tween.UIFillAmount(arrow, 1, Interval(arrowFillDuration)))

                // Fade in Next level text
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(upgradeLevelFadeAudio))
                .Chain(Tween.Alpha(upgradeLevelText, 1, Interval(upgradeLevelFadeDuration)))

                // Run base animation
                .ChainCallback(() => base.BeginAnimation(request, onComplete, isLastSameType))

                // Character spacing animation
                .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(spacingAnimAudio))
                .Chain(Tween.Custom(-100, 0, Interval(spacingAnimDuration), v => upgradeLevelText.characterSpacing = v))

                // Finalize
                .ChainCallback(() => _isAnimating = false);
            
            VAudioPlayer.Instance.PlaySFX(haloSpinAudio);
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
            currentLevelText.alpha = 0;
            currentLevelText.transform.position = currentLevelTextInitPosition.position;
            coopInfo.transform.position = coopInfoInitPosition.position;
            upgradeText.alpha = 0;
            arrow.fillAmount = 0;
            upgradeLevelText.alpha = 0;
            upgradeLevelText.characterSpacing = -100;
            _isAnimating = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isAnimating)
            {
                if (_clicked)
                    return;
                _clicked = true;
                var sequence = Sequence.Create();
                var position = coopInfoInitPosition.position;
                position.x = coopInfo.position.x;
                
                sequence
                    .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(moveInfoAudio))
                    .Chain(Tween.Position(coopInfo.transform, position, Interval(0.4f)))

                    .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(upgradeTextAudio))
                    .Chain(Tween.Alpha(upgradeText, 0, Interval(0.25f)))

                    .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(currentLevelMoveAudio))
                    .ChainCallback(() => currentLevelText.alpha = 0)

                    .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(currentLevelMoveAudio))
                    .Chain(Tween.Position(currentLevelText.transform, currentLevelTextPosition.position, Interval(0.25f)))

                    .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(arrowFillAudio))
                    .Chain(Tween.UIFillAmount(arrow, 0, Interval(0.25f)))

                    .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(upgradeLevelFadeAudio))
                    .Chain(Tween.Alpha(upgradeLevelText, 0, Interval(0.25f)))

                    .ChainCallback(() => VAudioPlayer.Instance.PlaySFX(upgradeLevelFadeAudio))
                    .Chain(Tween.Alpha(upgradeLevelText, 0, Interval(0.25f)))
                    
                    .ChainCallback(() => _onComplete?.Invoke());
            }
        }
    }
}
