using PrimeTween;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Events.DialogueEvent;

namespace VTuber.EventSystem.UI
{
    public class VLoadingAnimation : VUIBehaviour
    {
        [SerializeField] private GameObject ui;

        [SerializeField] private Image backgroundImage;

        [SerializeField] private Image eventImage;

        [SerializeField] private Transform leftCorner;

        [SerializeField] private Transform rightCorner;

        [SerializeField] private Transform initialPosition;

        [SerializeField] private Transform middlePosition;

        [SerializeField] private Transform finalPosition;
        
        [FoldoutGroup("动画参数：背景")]
        [LabelText("背景填充时间")]
        [SerializeField] private float bgFillDuration = 0.5f;

        [FoldoutGroup("动画参数：背景")]
        [LabelText("背景淡出时间")]
        [SerializeField] private float bgFadeOutDuration = 0.5f;


        [FoldoutGroup("动画参数：角标移动")]
        [LabelText("从初始 → 中间 时间")]
        [SerializeField] private float moveToMiddleDuration = 0.25f;

        [FoldoutGroup("动画参数：角标移动")]
        [LabelText("从中间 → 结束 时间")]
        [SerializeField] private float moveToFinalDuration = 0.5f;

        [FoldoutGroup("动画参数：角标移动")]
        [LabelText("从结束 → 初始 时间")]
        [SerializeField] private float moveBackDuration = 0.5f;


        [FoldoutGroup("动画参数：事件图标")]
        [LabelText("事件图标缩放时间")]
        [SerializeField] private float iconScaleDuration = 0.5f;

        [FoldoutGroup("动画参数：事件图标")]
        [LabelText("停留时间")]
        [SerializeField] private float holdDuration = 1.0f;

        public Sequence PlayAnimation(VStreamEventConfiguration e)
        {
            eventImage.sprite = VResourcesManager.Instance.TryGetSprite(e.icon);
            backgroundImage.color = e.backgroundColor;

            var sequence = PlayAnimationInternal();

            sequence.Chain(Tween.Alpha(backgroundImage, 0.0f, bgFadeOutDuration));
            sequence.ChainCallback(() => ui.SetActive(false));

            return sequence;
        }


        public Sequence PlayAnimation(VDialogueEvent e)
        {
            eventImage.sprite = e.Icon;
            backgroundImage.color = e.BackgroundColor;

            var sequence = PlayAnimationInternal();

            if (!e.dialogueNode.IsNullOrWhitespace())
            {
                sequence.Chain(Tween.Alpha(backgroundImage, 0.0f, bgFadeOutDuration));
                sequence.ChainCallback(() => ui.SetActive(false));
            }

            return sequence;
        }


        private Sequence PlayAnimationInternal()
        {
            ui.SetActive(true);

            eventImage.transform.localScale = Vector3.zero;
            leftCorner.localScale = Vector3.one;
            rightCorner.localScale = Vector3.one;

            var sequence = Sequence.Create();

            sequence.Chain(Tween.UIFillAmount(backgroundImage, 1.0f, bgFillDuration, Ease.InExpo));

            sequence.Chain(Tween.LocalPosition(leftCorner, middlePosition.localPosition, moveToMiddleDuration, Ease.InOutSine));
            sequence.Group(Tween.LocalPosition(rightCorner, -middlePosition.localPosition, moveToMiddleDuration, Ease.InOutSine));

            sequence.Chain(Tween.LocalPosition(leftCorner, finalPosition.localPosition, moveToFinalDuration, Ease.InOutSine));
            sequence.Group(Tween.LocalPosition(rightCorner, -finalPosition.localPosition, moveToFinalDuration, Ease.InOutSine));
            sequence.Group(Tween.Scale(eventImage.transform, 1.0f, iconScaleDuration, Ease.InOutSine));

            sequence.ChainDelay(holdDuration);

            sequence.Chain(Tween.LocalPosition(leftCorner, initialPosition.localPosition, moveBackDuration, Ease.InOutSine));
            sequence.Group(Tween.LocalPosition(rightCorner, -initialPosition.localPosition, moveBackDuration, Ease.InOutSine));
            sequence.Group(Tween.Scale(eventImage.transform, Vector3.zero, iconScaleDuration));
            sequence.Group(Tween.Scale(leftCorner, 0f, 0.5f));
            sequence.Group(Tween.Scale(rightCorner, 0f, 0.5f));

            return sequence;
        }


        public void Close()
        {
            ui.SetActive(false);
        }
    }
}
