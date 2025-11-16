using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Consumable;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;
using VTuber.Core.UI;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.RaisingAnimationSystem.Animations
{
    public class VAddConsumableAnimation : VRaisingAnimation
    {     
        [SerializeField] private Image consumableImage;
        [SerializeField] private Transform consumableInitPosition;
        
        [SerializeField] private Transform descriptionObject;
        [SerializeField] private TMP_Text relicDescriptionText;
        [SerializeField] private Transform descriptionInitPosition;
        [SerializeField] private Transform descriptionPosition;
        
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Button addConsumableButton;
        [SerializeField] private Button returnButton;
        [SerializeField] private Transform light;
        [SerializeField] private Image haloImage;
        [SerializeField] private Transform debugSlot;
        [SerializeField] private VConsumableSlotsUI consumableSlotsUI;
        
        [SerializeField] private Transform addConsumableButtonRightPosition;
        [SerializeField] private Transform addConsumableButtonCenterPosition;
        
        
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


        private Action _onComplete;
        private Action _applyEffect;
        private Sequence _sequence;

        protected override void Awake()
        {
            base.Awake();
            addConsumableButton.onClick.AddListener(OnAddConsumableButtonClicked);
            returnButton.onClick.AddListener(OnReturnButtonClicked);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (debug)
                return;
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnAddConsumable, OnAddConsumable);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRemoveConsumable, OnRemoveConsumable);
        }
        
        protected override void OnDisable()
        {
            base.OnEnable();
            if (debug)
                return;
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnAddConsumable, OnAddConsumable);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRemoveConsumable, OnRemoveConsumable);
        }

        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
        {
            _onComplete = onComplete;

            if (!debug)
            {
                var consumable = VDataManager.Instance.ConsumableConfigurationss[request.consumableIDs.First()];
                consumableImage.sprite = consumable.icon;
                nameText.text = consumable.consumableName;
                relicDescriptionText.text = consumable.description;
                _applyEffect = request.effectApply;
                haloImage.sprite = VUIUtils.Instance.GetHaloSprite((int)consumable.rarity + 1);
            }

            if (request.returnable)
            {
                returnButton.gameObject.SetActive(true);
                addConsumableButton.transform.position = addConsumableButtonRightPosition.position;
            }
            else
            {
                returnButton.gameObject.SetActive(false);
                addConsumableButton.transform.position = addConsumableButtonCenterPosition.position;
            }

            addConsumableButton.interactable = false;
            returnButton.interactable = false;

            _sequence = Sequence.Create();

            _sequence
                .Chain(Tween.LocalPosition(consumableImage.transform, Vector3.zero, relicEnterDuration, Ease.OutBounce))
                .Group(Tween.Alpha(consumableImage, 1, relicEnterAlphaDuration))
                .Group(Tween.Scale(consumableImage.transform, Vector3.one, relicEnterScaleDuration, Ease.OutBounce))
                .Group(Tween.Alpha(nameText, 1, relicNameFadeDuration))
                .Group(Tween.Custom(nameText.characterSpacing, 0, nameSpacingDuration,
                    v => nameText.characterSpacing = v))
                .Chain(Tween.LocalPosition(descriptionObject, descriptionPosition.localPosition, descriptionMoveDuration, descriptionMoveEase))
                .ChainCallback(() =>
                {
                    if(debug)
                        addConsumableButton.interactable = true;
                    else
                        addConsumableButton.interactable = consumableSlotsUI.GetEmptySlot() is not null;
                    returnButton.interactable = true;
                    
                    returnButton.interactable = true;
                });

            _sequence.Group(
                Tween.Scale(
                    consumableImage.transform,
                    Vector3.one * relicPulseScale,
                    relicPulseDuration,
                    Ease.InCubic,
                    relicPulseCycles,
                    CycleMode.Rewind
                )
            );

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

        public void OnReturnButtonClicked()
        {
            _sequence.Stop();
            addConsumableButton.interactable = false;
            returnButton.interactable = false;
            var sequence = Sequence.Create();
            sequence
                .Chain(Tween.LocalPosition(descriptionObject, descriptionInitPosition.localPosition, descriptionExitDuration, Ease.OutCubic))
                .Group(Tween.Scale(consumableImage.transform, Vector3.zero, relicShrinkDuration, Ease.InCubic))
                .Group(Tween.Custom(nameText.characterSpacing, 0, relicNameHideDuration, v => nameText.characterSpacing = v))
                .Group(Tween.Alpha(nameText, 0, relicNameHideDuration))
                .ChainCallback(() =>
                {
                    _onComplete?.Invoke();
                });
        }
        
        public void OnAddConsumableButtonClicked()
        {
            _sequence.Stop();
            addConsumableButton.interactable = false;
            returnButton.interactable = false;

            var sequence = Sequence.Create();

            if (debug)
            {
                consumableImage.transform.SetParent(debugSlot);
            }
            else
            {
                
                var slot = consumableSlotsUI.GetEmptySlot();
                consumableImage.transform.SetParent(slot.transform);
            }

            sequence
                .Chain(Tween.LocalPosition(descriptionObject, descriptionInitPosition.localPosition, descriptionExitDuration, Ease.OutCubic))
                .Group(Tween.Scale(consumableImage.transform, Vector3.zero, relicShrinkDuration, Ease.InCubic))
                .Group(Tween.LocalPosition(consumableImage.transform, Vector3.zero, relicMoveToLibraryDuration, Ease.InCubic))
                .Group(Tween.Custom(nameText.characterSpacing, 0, relicNameHideDuration, v => nameText.characterSpacing = v))
                .Group(Tween.Alpha(nameText, 0, relicNameHideDuration))
                .ChainCallback(() =>
                {
                    consumableImage.transform.SetParent(ui.transform);
                    if (!debug)
                        _applyEffect?.Invoke();
                    _onComplete?.Invoke();
                });
        }
        
        public override void ResetAnimation()
        {
            base.ResetAnimation();

            consumableImage.color = new Color(1, 1, 1, 0);
            consumableImage.transform.localPosition = consumableInitPosition.localPosition;
            consumableImage.transform.localScale = Vector3.one * 10.0f;

            nameText.alpha = 0;
            nameText.characterSpacing = -100;

            descriptionObject.localPosition = descriptionInitPosition.localPosition;
        }

        private void OnRemoveConsumable(Dictionary<string, object> messagedict)
        {
            var areSlotsFull = (bool)messagedict["AreSlotsFull"];

            addConsumableButton.interactable = !areSlotsFull;
        }

        private void OnAddConsumable(Dictionary<string, object> messagedict)
        {
            var areSlotsFull = (bool)messagedict["AreSlotsFull"];

            addConsumableButton.interactable = !areSlotsFull;
        }
    }
}