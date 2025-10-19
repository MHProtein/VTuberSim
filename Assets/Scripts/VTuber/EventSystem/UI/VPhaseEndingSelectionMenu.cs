using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.SE;
using VTuber.Dialogue.UI;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Events.DialogueEvent;
using VTuber.Relic;
// Make sure you have the correct using statements for your project's resource/game managers
// using VTuber.Core.Data; // Example
// using VTuber.Relics; // Example

namespace VTuber.EventSystem.UI
{
    public class VPhaseEndingSelectionMenu : VUIBehaviour
    {
        private List<VPhaseEndingOption> _options;
        private VPhaseEndingOption _selectedOption;

        [Header("Original References")] [SerializeField]
        private Button confirmButton;

        [SerializeField] private Transform grid;

        // --- NEW CODE START ---
        [Header("Relic Display")] [SerializeField]
        private GameObject relicUiPrefab; // Drag your Relic UI Prefab here in the Inspector

        [SerializeField] private Transform characterRelicsContainer; // Drag the container you created
        [SerializeField] private Transform inheritableRelicsContainer; // Drag the other container
        // --- NEW CODE END ---


        public void Initialize(GameObject endingPrefab, List<VStreamEvent> endings)
        {
            confirmButton.interactable = false;
            _options = new List<VPhaseEndingOption>();
            foreach (var ending in endings)
            {
                GameObject endingGo = Instantiate(endingPrefab, grid);
                VPhaseEndingOption option = endingGo.GetComponent<VPhaseEndingOption>();
                option.Initialize(ending, this);
                _options.Add(option);
            }

            // --- NEW CODE START ---
            // Ensure relic displays are empty when the menu first opens
            ClearRelicDisplays();
            // --- NEW CODE END ---
        }

        public void SelectOption(VPhaseEndingOption option)
        {
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
            confirmButton.interactable = true;
            if (_selectedOption is not null)
                _selectedOption.Unselect();
            _selectedOption = option;

            // --- NEW CODE START ---
            // When a new option is selected, clear the old relics and show the new ones
            ClearRelicDisplays();
            DisplayRelicsForOption(option);
            // --- NEW CODE END ---
        }

        public void Confirm()
        {
            confirmButton.interactable = false;
            _selectedOption.E.Phase.SetEndingEventID(_selectedOption.E.EventID);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnPhaseEndingSelected,
                new Dictionary<string, object>()
                {
                    { "KPIs", _selectedOption.E.Kpis }
                });
            foreach (var option in _options)
            {
                Destroy(option.gameObject);
            }

            _options.Clear();
            _selectedOption = null;
            VEventSystemUI.Instance.ClosePhaseEndingSelectionMenu();
        }

        // --- NEW METHODS START ---

        /// <summary>
        /// Clears all instantiated relic icons from the display containers.
        /// </summary>
        private void ClearRelicDisplays()
        {
            foreach (Transform child in characterRelicsContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in inheritableRelicsContainer)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Fetches and displays the relics associated with the selected event option.
        /// </summary>
        private void DisplayRelicsForOption(VPhaseEndingOption option)
        {
            if (relicUiPrefab == null) return;

            ClearRelicDisplays();
            VStreamEvent streamEvent = option.E;

            // 1. Get the Dialog object
            Dialog dialog = VResourcesManager.Instance.TryGetDialog(streamEvent.dialogueNode);

            if (dialog != null)
            {
                // 2. Get the list of relic configurations
                List<VRelicConfiguration> relicConfigs = dialog.GetRelics();

                if (relicConfigs != null)
                {
                    foreach (var config in relicConfigs)
                    {
                        // 3. Create the VRelic directly from the configuration object
                        VRelic relicData = config.CreateRelic(); // <-- THE CORRECTED LINE

                        if (relicData != null)
                        {
                            GameObject relicGo = Instantiate(relicUiPrefab, characterRelicsContainer);
                            relicGo.GetComponent<VRelicUI>().Initialize(relicData);
                        }
                    }
                }
            }
            // 2. Display inheritable relics (This part was likely already correct)
            var inheritableRelicReward =
                VGameManager.Instance.ReincarnationConfiguration.relicRewards.Find(x =>
                    streamEvent.EventID == x.eventID);
            if (inheritableRelicReward != null)
            {
                foreach (var relicId in inheritableRelicReward.relicIDs)
                {
                    // As before, you'll need a manager to get relic data from an ID.
                    VRelic relicData =
                        VDataManager.Instance.CreateRelicByID(relicId); // This is an assumption, but a common pattern.

                    if (relicData != null)
                    {
                        GameObject relicGo = Instantiate(relicUiPrefab, inheritableRelicsContainer);
                        relicGo.GetComponent<VRelicUI>().Initialize(relicData);
                    }
                }
            }
        }
    }
}