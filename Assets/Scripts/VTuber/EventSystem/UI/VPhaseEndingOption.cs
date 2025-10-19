using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.EventSystem.UI;
using VTuber.ScheduleSystem.Events;
// --- Add these new using statements ---
using VTuber.Core.Managers;
using VTuber.Relic;

namespace VTuber.Dialogue.UI
{
    public class VPhaseEndingOption : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public VStreamEvent E => e;
        private VStreamEvent e;
        VPhaseEndingSelectionMenu _menu;
        // --- NEW FIELDS: Add these for relic display ---
        [Header("Relic Display")]
        [SerializeField] private GameObject relicUiPrefab; // The prefab for a single relic icon
        [SerializeField] private Transform relicsGrid;     // The grid inside this option to hold the relics
        
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private GameObject conditionPrefab;
        [SerializeField] private Transform grids;
        private List<TMP_Text> conditionDescriptions;
        
        public void Initialize(VStreamEvent streamEvent, VPhaseEndingSelectionMenu menu)
        {;
            e = streamEvent;
            _menu = menu;
            conditionDescriptions = new List<TMP_Text>();
            titleText.text = streamEvent.EventName;
            var kpis = streamEvent.Kpis;
            foreach (var kpi in kpis)
            {
                GameObject conditionGo = Instantiate(conditionPrefab, grids);
                var text = conditionGo.GetComponent<TMP_Text>();
                text.text = $"每周至少需完成 {kpi.RequiredAmount} 次 <color=red>{kpi.AbilityName}{kpi.EventName}</color>";
            }
            // --- NEW LOGIC: Display relics immediately upon initialization ---
            DisplayRelics();
        }
        
        private void DisplayRelics()
        {
            if (relicUiPrefab == null || e == null) return;

            // 1. Character Relics
            Dialog dialog = VResourcesManager.Instance.TryGetDialog(e.dialogueNode);
            if (dialog != null)
            {
                List<VRelicConfiguration> relicConfigs = dialog.GetRelics();
                if (relicConfigs != null)
                {
                    foreach (var config in relicConfigs)
                    {
                        VRelic relicData = config.CreateRelic();
                        if (relicData != null)
                        {
                            Instantiate(relicUiPrefab, relicsGrid).GetComponent<VRelicUI>().Initialize(relicData);
                        }
                    }
                }
            }

            // 2. Inheritable Relics
            var inheritableRelicReward = VGameManager.Instance.ReincarnationConfiguration.relicRewards.Find(x => e.EventID == x.eventID);
            if (inheritableRelicReward != null)
            {
                foreach (var relicId in inheritableRelicReward.relicIDs)
                {
                    VRelic relicData = VDataManager.Instance.CreateRelicByID(relicId);
                    if (relicData != null)
                    {
                        Instantiate(relicUiPrefab, relicsGrid).GetComponent<VRelicUI>().Initialize(relicData);
                    }
                }
            }
        }
        
        

        public void Unselect()
        {
            backgroundImage.color = Color.white;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _menu.SelectOption(this);
            backgroundImage.color = Color.cyan;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }
    }
}