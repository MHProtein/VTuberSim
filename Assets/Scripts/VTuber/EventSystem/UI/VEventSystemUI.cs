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
    }
}