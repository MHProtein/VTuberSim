using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.SE;
using VTuber.Dialogue.UI;
using VTuber.ScheduleSystem.Events;
// Add any other necessary using statements for your project managers
using VTuber.Core.Managers; 
using VTuber.Relic;

namespace VTuber.EventSystem.UI
{
    public class VPhaseEndingSelectionMenu : VUIBehaviour
    {
        private List<VPhaseEndingOption> _options;
        private VPhaseEndingOption _selectedOption;
        
        [Header("Original References")]
        [SerializeField] private Button confirmButton;
        [SerializeField] private Transform grid;

        [Header("Relic Display")]
        [SerializeField] private GameObject relicUiPrefab;
        [SerializeField] private Transform characterRelicsContainer;
        [SerializeField] private Transform inheritableRelicsContainer;
        
        // ... (Initialize, Confirm methods remain the same) ...

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
            ClearRelicDisplays();
        }
        
        public void SelectOption(VPhaseEndingOption option)
        {
            // === DEBUG STEP 1: Is the selection registered? ===
            Debug.Log($"[DEBUG] SelectOption called for event: {option.E.EventID}");

            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
            confirmButton.interactable = true;
            if(_selectedOption is not null)
                _selectedOption.Unselect();
            _selectedOption = option;
            
            DisplayRelicsForOption(option);
        }

        private void DisplayRelicsForOption(VPhaseEndingOption option)
        {
            ClearRelicDisplays();
            
            // === DEBUG STEP 2: Are the critical prefabs and containers assigned? ===
            if (relicUiPrefab == null)
            {
                Debug.LogError("[DEBUG] FATAL: relicUiPrefab is NOT ASSIGNED in the Inspector!");
                return;
            }
            if (characterRelicsContainer == null || inheritableRelicsContainer == null)
            {
                Debug.LogError("[DEBUG] FATAL: One or more relic containers are NOT ASSIGNED in the Inspector!");
                return;
            }

            VStreamEvent streamEvent = option.E;
            Debug.Log($"--- [DEBUG] Starting to display relics for event: {streamEvent.EventID} ---");

            // --- Character Relics ---
            Dialog dialog = VResourcesManager.Instance.TryGetDialog(streamEvent.dialogueNode);
            if (dialog == null)
            {
                Debug.LogWarning($"[DEBUG] No Dialog object found for dialogueNode key: '{streamEvent.dialogueNode}'");
            }
            else
            {
                List<VRelicConfiguration> relicConfigs = dialog.GetRelics();
                // === DEBUG STEP 3: Do we have relic data? ===
                Debug.Log($"[DEBUG] Found {relicConfigs.Count} character relic configurations.");
                
                foreach (var config in relicConfigs)
                {
                    VRelic relicData = config.CreateRelic();
                    if (relicData != null)
                    {
                        // === DEBUG STEP 4: Are we instantiating the UI? ===
                        Debug.Log($"[DEBUG] Instantiating UI for Character Relic: '{relicData.GetRelicName()}'");
                        GameObject relicGo = Instantiate(relicUiPrefab, characterRelicsContainer);
                        relicGo.GetComponent<VRelicUI>().Initialize(relicData);
                    }
                }
            }

            // --- Inheritable Relics ---
            var inheritableRelicReward = VGameManager.Instance.ReincarnationConfiguration.relicRewards.Find(x => streamEvent.EventID == x.eventID);
            if (inheritableRelicReward != null)
            {
                // === DEBUG STEP 3 (Part 2): Do we have inheritable relic data? ===
                Debug.Log($"[DEBUG] Found {inheritableRelicReward.relicIDs.Count} inheritable relic IDs.");

                foreach (var relicId in inheritableRelicReward.relicIDs)
                {
                    VRelic relicData = VDataManager.Instance.CreateRelicByID(relicId);
                    if (relicData != null)
                    {
                        // === DEBUG STEP 4 (Part 2): Are we instantiating the UI? ===
                        Debug.Log($"[DEBUG] Instantiating UI for Inheritable Relic: '{relicData.GetRelicName()}'");
                        GameObject relicGo = Instantiate(relicUiPrefab, inheritableRelicsContainer);
                        relicGo.GetComponent<VRelicUI>().Initialize(relicData);
                    }
                    else
                    {
                        Debug.LogWarning($"[DEBUG] Failed to create inheritable relic with ID: {relicId}");
                    }
                }
            }
            else
            {
                Debug.Log("[DEBUG] No inheritable relic rewards found for this event.");
            }
        }

        private void ClearRelicDisplays()
        {
            foreach (Transform child in characterRelicsContainer) Destroy(child.gameObject);
            foreach (Transform child in inheritableRelicsContainer) Destroy(child.gameObject);
        }

        public void Confirm()
        {
            // ... (Confirm method remains the same) ...
        }
    }
}