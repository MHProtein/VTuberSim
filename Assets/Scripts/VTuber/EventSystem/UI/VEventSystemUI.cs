using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Core.Foundation;
using VTuber.EventSystem.UI;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Dialogue.UI
{
    public class VEventSystemUI : VSingletonMonobehaviour<VEventSystemUI>
    {
        [SerializeField] private VPhaseEndingSelectionMenu phaseEndingSelectionMenu;
        [SerializeField] private GameObject endingOptionPrefab;
        [SerializeField] private VCardLibraryUI cardLibraryUI;
        [SerializeField] private VSelectFrom3Menu selectFrom3Menu;
        
        
        public void InitializePhaseEndingSelectionMenu(List<KeyValuePair<VStreamEvent, List<bool>>> endings)
        {
            phaseEndingSelectionMenu.gameObject.SetActive(true);
            phaseEndingSelectionMenu.Initialize(endingOptionPrefab, endings);
        }

        public void ClosePhaseEndingSelectionMenu()
        {
            phaseEndingSelectionMenu.gameObject.SetActive(false);
        }
        
        public void OpenCardLibrary(List<VCard> cards, bool select, Action<VCard> confirmAction)
        {
            cardLibraryUI.gameObject.SetActive(true);
            cardLibraryUI.Initialize(cards, select, confirmAction);
        }
        
        public void CloseCardLibrary()
        {
            cardLibraryUI.gameObject.SetActive(false);
        }
        
        public void OpenSelectFrom3Menu(List<VCard> cards, Action<VCard> confirmAction)
        {
            selectFrom3Menu.gameObject.SetActive(true);
            selectFrom3Menu.Initialize(cards, confirmAction);
        }
        
        public void CloseSelectFrom3Menu()
        {
            selectFrom3Menu.gameObject.SetActive(false);
        }
    }
}