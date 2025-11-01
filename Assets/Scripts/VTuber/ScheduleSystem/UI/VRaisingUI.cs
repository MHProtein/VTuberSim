using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using TMPro;
using Tutorial.Script;
using Tutorial.UI;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.UI;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.UI;
using VTuber.Reincarnation;

namespace VTuber.ScheduleSystem.UI
{
    public class VRaisingUI : VSingletonMonobehaviour<VRaisingUI>
    {
        [SerializeField] private TMP_Text weekCountText;

        [Header("Schedule")] [SerializeField] private Transform _scheduleUI;
        [SerializeField] private Transform initialScheduleUIPosition;
        [SerializeField] private GameObject eventUIPrefab;

        [Space(3)] [Header("ScheduleCreation")] [SerializeField]
        private GameObject scheduleCreationUI;

        [SerializeField] private TMP_Text eventNameUI;
        [SerializeField] private TMP_Text eventDescriptionUI;
        [SerializeField] private Transform eventDescriptionUITransform;
        [SerializeField] private Transform eventDescriptionUITransformPosition;
        [SerializeField] private Transform eventDescriptionUITransformTutorialPosition;
        [SerializeField] private VTips tipUI;

        [SerializeField] private Transform creationSchedulePosition;

        [SerializeField] private VScheduleCreator scheduleCreatorUI;

        [Space(3)] [Header("ExecutionUI")] [SerializeField]
        private GameObject executionUI;

        [SerializeField] private TMP_Text pauseText;

        [SerializeField] private Transform executionSchedulePosition;


        [Space(3)] [Header("PauseUI")] [SerializeField]
        private GameObject pauseUI;

        [SerializeField] private Transform pauseSchedulePosition;

        [Space(3)] [Header("CardViewUI")] [SerializeField]
        private GameObject cardLibraryUIObject;

        [SerializeField] private VCardViewSelectionUI cardLibraryUI;

        [Space(3)] [Header("ConsumableUI")] [SerializeField]
        private GameObject consumableUIParent;

        [SerializeField] private GameObject consumableUIBattleParent;
        [SerializeField] private GameObject consumableUI;
        [SerializeField] private Transform uiWrapper;

        [Space(3)] [Header("EndingUI")] [SerializeField]
        private VEndingUI endingUI;

        [Space(3)] [Header("Attributes")] [SerializeField]
        private GameObject staminaUI;

        [SerializeField] private GameObject membershipUI;

        [Space(3)] [Header("TutorialRestartWeekPanel")] [SerializeField]
        private GameObject tutorialRestartWeekPanel;

        [SerializeField] private Button tutorialRestartWeekButton;

        public List<Color> abilityColors = new();

        [Space(3)] [Header("AddCard")]
        [SerializeField] private GameObject pickCardMenuScroll;
        [SerializeField] private Transform pickCardMenuScrollContent;
        [SerializeField] private VPickCardMenu pickCardMenu;
        [SerializeField] private Button addCardButton;
        [SerializeField] private GameObject cardUIPrefab;
        
        protected override void Awake()
        {
            base.Awake();
            tutorialRestartWeekButton.onClick.AddListener(RestartWeek);
            addCardButton.onClick.AddListener(OnAddCardButtonClicked);
        }

        protected override void OnEnable()
        {
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnNotifyEventDescriptionChange,
                OnNotifyEventDescriptionChange);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnNotifyEventDescriptionChange,
                OnNotifyEventDescriptionChange);
        }
        
        public void OnAddCardButtonClicked()
        {
            var cardUIs = new List<VCardUI>();
            var cards = VDataManager.Instance.GetAllCardConfigurations().Select(card => card.CreateCard());
            foreach (var card in cards) cardUIs.Add(VUIUtils.SpawnCardUI(cardUIPrefab, card, pickCardMenuScrollContent));
            pickCardMenu.BeginPickCard(cardUIs, 1000, VCardPileType.ALL, false, false, OnCardPicked);
            pickCardMenuScroll.SetActive(true);
        }

        private void OnCardPicked(List<VCard> cards)
        {
            VGameManager.Instance.AddCardsToCharacter(cards);
            pickCardMenuScroll.SetActive(false);
        }

        public void Initialize(bool isTutorial)
        {
            if (isTutorial)
            {
                eventDescriptionUITransform.position = eventDescriptionUITransformTutorialPosition.position;
                tipUI.gameObject.SetActive(true);
            }
            else
            {
                eventDescriptionUITransform.position = eventDescriptionUITransformPosition.position;
                tipUI.gameObject.SetActive(false);
            }
        }

        public void SetTips(VTipConfig tipConfig)
        {
            tipUI.SetTips(tipConfig.title, tipConfig.description, tipConfig.image);
        }

        public void ShowRestartWeekUI()
        {
            tutorialRestartWeekPanel.SetActive(true);
        }

        private void RestartWeek()
        {
            VGameManager.Instance.TutorialRestartWeek();
            tutorialRestartWeekPanel.SetActive(false);
        }

        public void SwitchAttributesUIBattle(bool active)
        {
            staminaUI.SetActive(active);
            membershipUI.SetActive(active);
        }

        public void InitializeEndingUI(string characterName, string ratingLevel, int score, VAccount account)
        {
            endingUI.Initialize(characterName, ratingLevel, score, account);
        }

        public void ShowEndingUI()
        {
            endingUI.Show();
        }

        public void InitializeCardLibraryUI(List<VCard> cards)
        {
            cardLibraryUIObject.SetActive(true);
            cardLibraryUI.Initialize(cards, false, false, false, null);
        }

        public void CloseCardLibraryUI()
        {
            cardLibraryUI.Close();
            cardLibraryUIObject.SetActive(false);
        }

        public void SetPauseText(bool shouldPause)
        {
            pauseText.text = shouldPause ? "此事件后暂停" : "暂停周表";
        }

        private void OnNotifyEventDescriptionChange(Dictionary<string, object> messagedict)
        {
            eventNameUI.text = messagedict["Name"] as string;
            eventDescriptionUI.text = messagedict["Description"] as string;
        }

        public void SetExecutionUIActive(bool active)
        {
            executionUI.SetActive(active);
        }

        public void SetCreationUIActive(bool active)
        {
            scheduleCreationUI.SetActive(active);
        }

        public void SetPauseUIActive(bool active)
        {
            pauseUI.SetActive(active);
        }

        public Tween SetScheduleUIPositionToCreation()
        {
            return Tween.Position(_scheduleUI, creationSchedulePosition.position, 0.3f);
        }

        public Tween SetScheduleUIPositionToExecution()
        {
            return Tween.Position(_scheduleUI, executionSchedulePosition.position, 0.3f);
        }

        public Tween SetScheduleUIPositionToPause()
        {
            return Tween.Position(_scheduleUI, pauseSchedulePosition.position, 0.3f);
        }

        public void SetScheduleUIPositionToInitial()
        {
            _scheduleUI.transform.position = initialScheduleUIPosition.position;
        }

        public Tween UpdateWeekCount(int weekCount)
        {
            weekCountText.text = $"周数：{weekCount}";
            return Tween.PunchScale(weekCountText.transform, Vector3.one * 1.3f, 0.3f);
        }

        public void SwitchToScheduleCreation(Action onComplete = null)
        {
            executionUI.SetActive(false);
            scheduleCreationUI.SetActive(true);
            Tween.Position(_scheduleUI, creationSchedulePosition.position, 0.3f).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public void SwitchToExecution(Action onComplete = null)
        {
            executionUI.SetActive(true);
            Tween.Position(_scheduleUI, executionSchedulePosition.position, 0.3f).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public VEventUI CreateEventUI(Transform parent)
        {
            var eventUI = Instantiate(eventUIPrefab, parent);
            var eventUIComponent = eventUI.GetComponent<VEventUI>();
            return eventUIComponent;
        }

        public void SetConsumableToBattle()
        {
            consumableUI.transform.SetParent(uiWrapper.transform);
            consumableUI.transform.SetParent(consumableUIBattleParent.transform);
            consumableUI.transform.localPosition = Vector3.zero;
            consumableUI.transform.localScale = Vector3.one;
        }

        public void SetConsumableToRaising()
        {
            consumableUI.transform.SetParent(consumableUIParent.transform);
            consumableUI.transform.localPosition = Vector3.zero;
            consumableUI.transform.localScale = Vector3.one;
        }
    }
}