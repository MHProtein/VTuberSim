using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.EventSystem.UI;
using VTuber.ScheduleSystem.Events;

namespace VTuber.Dialogue.UI
{
    public class VPhaseEndingOption : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public VStreamEvent E => e;
        private VStreamEvent e;
        VPhaseEndingSelectionMenu _menu;
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