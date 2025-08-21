using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.Foundation;
using VTuber.EventSystem.UI;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.UI;
using VTuber.ScheduleSystem.UI.ConsumableViewUI;

namespace VTuber.Dialogue.UI
{
    public class VEventSystemUI : VSingletonMonobehaviour<VEventSystemUI>
    {
        [SerializeField] private VPhaseEndingSelectionMenu phaseEndingSelectionMenu;
        [SerializeField] private GameObject endingOptionPrefab;
        [SerializeField] private VCardViewSelectionUI selectCardUI;
        [SerializeField] private VCardViewSelectionUI upgradeCardUI;
        [FormerlySerializedAs("selectFrom3Menu")] [SerializeField] private VSelectFrom3CardsMenu selectFrom3CardsMenu;
        [SerializeField] private VSelectFrom3ConsumablesMenu selectFrom3ConsumablesMenu;
        
        private Action _closePhaseEndingSelectionMenuAction;
        private Action _closeCardLibrary;
        private Action _CloseSelectFrom3Menu;
        private Action _CloseSelectFrom3ConsumablesMenu;

        protected override void Awake()
        {
            base.Awake();
            selectCardUI.confirmButton.onClick.AddListener(CloseCardLibrary);
            upgradeCardUI.confirmButton.onClick.AddListener(CloseUpgradeCard);
        }

        public void InitializePhaseEndingSelectionMenu(List<VStreamEvent> endings, Action confirmAction)
        {
            phaseEndingSelectionMenu.gameObject.SetActive(true);
            phaseEndingSelectionMenu.Initialize(endingOptionPrefab, endings);
            _closePhaseEndingSelectionMenuAction = confirmAction;
        }

        public void ClosePhaseEndingSelectionMenu()
        {
            phaseEndingSelectionMenu.gameObject.SetActive(false);
            _closePhaseEndingSelectionMenuAction?.Invoke();
        }
        
        public void OpenSelectFrom3ConsumablesMenu(VCharacter character, List<VConsumable> consumables, Action<VConsumable> confirmAction, Action closeAction)
        {
            selectFrom3ConsumablesMenu.gameObject.SetActive(true);
            selectFrom3ConsumablesMenu.Initialize(character, consumables, confirmAction);
            _CloseSelectFrom3ConsumablesMenu = closeAction;
            selectFrom3ConsumablesMenu.confirmButton.onClick.AddListener(CloseSelectFrom3ConsumablesMenu);
        }
        
        public void OpenSelectCard(List<VCard> cards, bool select, Action<VCard> confirmAction, Action closeAction)
        {
            selectCardUI.gameObject.SetActive(true);
            selectCardUI.Initialize(cards, false, select, false, confirmAction);
            _closeCardLibrary = closeAction;
        }
        
        public void OpenUpgradeCard(List<VCard> cards, Action closeAction)
        {
            upgradeCardUI.gameObject.SetActive(true);
            upgradeCardUI.Initialize(cards, false, true, true, (card) => card.Upgrade(false), null, (card) => card.Upgrade(false));
            _closeCardLibrary = closeAction;
        }
        
        public void OpenSelectFrom3Menu(List<VCard> cards, Action<VCard> confirmAction, Action closeAction)
        {
            selectFrom3CardsMenu.gameObject.SetActive(true);
            selectFrom3CardsMenu.Initialize(cards, confirmAction);
            _CloseSelectFrom3Menu = closeAction;
        }
        
        public void CloseCardLibrary()
        {
            selectCardUI.gameObject.SetActive(false);
            _closeCardLibrary?.Invoke();
        }
        
        public void CloseUpgradeCard()
        {
            upgradeCardUI.gameObject.SetActive(false);
            _closeCardLibrary?.Invoke();
        }
        
        public void CloseSelectFrom3ConsumablesMenu()
        {
            selectFrom3ConsumablesMenu.gameObject.SetActive(false);
            _CloseSelectFrom3ConsumablesMenu?.Invoke();
        }
        
        public void CloseSelectFrom3Menu()
        {
            selectFrom3CardsMenu.gameObject.SetActive(false);
            _CloseSelectFrom3Menu?.Invoke();
        }
    }
}