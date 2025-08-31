using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Character;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VResultScreenUI : VUIBehaviour
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private Transform attributes;
        [SerializeField] private Transform attributesOriginalPosition;
        [SerializeField] private Transform attributesFinalPosition;
        [SerializeField] private Transform abilities;
        [SerializeField] private Transform abilitiesOriginalPosition;
        [SerializeField] private Transform abilitiesFinalPosition;
        [SerializeField] private GameObject failureText;
        [SerializeField] private GameObject successText;
        [SerializeField] private GameObject hugeSuccessText;
        [SerializeField] private TMP_Text popularityText;
        [SerializeField] private TMP_Text finalViewerCountText;
        [SerializeField] private TMP_Text highestViewerCountText;
        [SerializeField] private TMP_Text revenueText;
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text membershipText;
        [SerializeField] private TMP_Text singingAbilityText;
        [SerializeField] private TMP_Text gamingAbilityText;
        [SerializeField] private TMP_Text chattingAbilityText;
        [SerializeField] private Button continueButton;

        private int _moneyDelta;
        private int _membershipDelta;
        private int _singingAbilityDelta;
        private int _gamingAbilityDelta;
        private int _chattingAbilityDelta;
        private int _popularity;
        private bool _isBattleSuccess;

        protected override void Awake()
        {
            base.Awake();
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleBegin, OnBattleBegin);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleBegin, OnBattleBegin);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
        }
        
        private void OnBattleBegin(Dictionary<string, object> messagedict)
        {
            var characterAttributesManager = messagedict["CharacterAttributeManager"] as VCharacterAttributeManager;
            
            _singingAbilityDelta = characterAttributesManager.Attributes["CASingingAbility"].Value;
            _gamingAbilityDelta = characterAttributesManager.Attributes["CAGamingAbility"].Value;
            _chattingAbilityDelta = characterAttributesManager.Attributes["CAChattingAbility"].Value;
            _membershipDelta = characterAttributesManager.Attributes["CAMembershipCount"].Value;
            _moneyDelta = characterAttributesManager.Attributes["CAMoney"].Value;
        }
        
        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            var characterAttributesManager = messagedict["CharacterAttributeManager"] as VCharacterAttributeManager;
            var battleAttributeManager = messagedict["BattleAttributeManager"] as VBattleAttributeManager;
            
            _singingAbilityDelta = characterAttributesManager.Attributes["CASingingAbility"].Value - _singingAbilityDelta;
            _gamingAbilityDelta = characterAttributesManager.Attributes["CAGamingAbility"].Value - _gamingAbilityDelta;
            _chattingAbilityDelta = characterAttributesManager.Attributes["CAChattingAbility"].Value - _chattingAbilityDelta;
            _membershipDelta = characterAttributesManager.Attributes["CAMembershipCount"].Value - _membershipDelta;
            _moneyDelta = characterAttributesManager.Attributes["CAMoney"].Value - _moneyDelta;

            singingAbilityText.text = $"+{_singingAbilityDelta}";
            gamingAbilityText.text = $"+{_gamingAbilityDelta}";
            chattingAbilityText.text = $"+{_chattingAbilityDelta}";
            membershipText.text = $"+{_membershipDelta}";
            moneyText.text = $"+{_moneyDelta}";
            
            var viewerCount = battleAttributeManager.BattleAttributes["BAViewerCount"];
            finalViewerCountText.text = viewerCount.Value.ToString();
            highestViewerCountText.text = viewerCount.HighestValue.ToString();

            _popularity = battleAttributeManager.BattleAttributes["BAPopularity"].Value;
            popularityText.text = _popularity.ToString();
            revenueText.text = battleAttributeManager.BattleAttributes["BARevenue"].Value.ToString();
        
            if ((bool)messagedict["ReachedExtraTarget"])
            {
                hugeSuccessText.SetActive(true);
                _isBattleSuccess = true;
            }
            else if ((bool)messagedict["ReachedTarget"])
            {
                successText.SetActive(true);
                _isBattleSuccess = true;
            }
            else
            {
                failureText.SetActive(true);
                _isBattleSuccess = false;
            }
            Show();
        }
        
        public void Show()
        {
            ui.SetActive(true);
            continueButton.interactable = false;
            Tween.Position(attributes, attributesFinalPosition.position, 0.5f);
            Tween.Position(abilities, abilitiesFinalPosition.position, 0.5f).OnComplete((() => continueButton.interactable = true));
        }
        
        public Tween Hide()
        {
            Tween.Position(attributes, attributesOriginalPosition.position, 0.5f).OnComplete((() =>
            {
                ui.SetActive(false);
            }));
            return Tween.Position(abilities, abilitiesOriginalPosition.position, 0.5f);
        }

        public void OnContinueButtonClicked()
        {
            Hide().OnComplete((() =>
            {
                hugeSuccessText.SetActive(false);
                successText.SetActive(false);
                failureText.SetActive(false);
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleEndNotify, new Dictionary<string, object>
                {
                    { "IsTargetMet", _isBattleSuccess },
                    { "Popularity", _popularity },
                });
            }));
        }
    }
}