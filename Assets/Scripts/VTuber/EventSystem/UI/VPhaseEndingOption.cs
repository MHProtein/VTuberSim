using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.EventSystem.UI;
using VTuber.Relic;
using VTuber.Relic.UI;
using VTuber.ScheduleSystem.Events;

namespace VTuber.Dialogue.UI
{
    public class VPhaseEndingOption : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public VStreamEvent E => e;
        private VStreamEvent e;
        // --- NEW FIELDS: Add these for relic display ---
        [Header("Relic Display")]
        [SerializeField] private GameObject relicUiPrefab; // The prefab for a single relic icon
        [SerializeField] private Transform characterRelicsGrid;   // 用于角色遗物
        [SerializeField] private Transform inheritableRelicsGrid; // 用于可继承遗物
        
        [SerializeField] private Image eventIcon;
        
        
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private GameObject conditionPrefab;
        [SerializeField] private Transform grids;
        private VPhaseEndingSelectionMenu _menu;
        private List<TMP_Text> conditionDescriptions;
        
        public void Initialize(VStreamEvent streamEvent, VPhaseEndingSelectionMenu menu)
        {;
            e = streamEvent;
            _menu = menu;
            conditionDescriptions = new List<TMP_Text>();
            titleText.text = streamEvent.EventName;
            
            var kpis = streamEvent.Kpis;
            if (eventIcon != null)
            {
                // VScheduleEvent (父类) 中已经有了 Icon 属性
                eventIcon.sprite = streamEvent.Icon;
                
                // 可选：如果图标为空，可能需要隐藏 Image 防止显示白色方块
                eventIcon.gameObject.SetActive(streamEvent.Icon != null);
            }
            
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

            // 1. 角色遗物 (Character Relics)
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
                            
                            Debug.Log($"Character Relic Found: {relicData}");
                            // --- 核心修改点 2：实例化到 characterRelicsGrid ---
                            // Instantiate the prefab
                            GameObject relicGo = Instantiate(relicUiPrefab, characterRelicsGrid);
                            // Get the VRelicSlotUI component and initialize it
                            relicGo.GetComponent<VRelicSlotUI>().Initialize(relicData, false); // displayValue is false as we don't need the layer text
                            // --- END OF CHANGE ---
                        }
                    }
                }
            }

            // 2. 可继承遗物 (Inheritable Relics)
            var inheritableRelicReward = VGameManager.Instance.ReincarnationConfiguration.relicRewards.Find(x => e.EventID == x.eventID);
            if (inheritableRelicReward != null)
            {
                foreach (var relicId in inheritableRelicReward.relicIDs)
                {
                    VRelic relicData = VDataManager.Instance.CreateRelicByID(relicId);
                    if (relicData != null)
                    {
                        Debug.Log($"Inheritable Relic Found: {relicData}");
                        // --- 核心修改点 3：实例化到 inheritableRelicsGrid ---
                        // Instantiate the prefab
                        GameObject relicGo = Instantiate(relicUiPrefab, inheritableRelicsGrid);
                        // Get the VRelicSlotUI component and initialize it
                        relicGo.GetComponent<VRelicSlotUI>().Initialize(relicData, false);
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