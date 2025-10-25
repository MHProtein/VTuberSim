using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.EventSystem.UI;
using VTuber.ScheduleSystem.Events;

namespace VTuber.Dialogue.UI
{
    public class VPhaseEndingOption : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private GameObject conditionPrefab;
        [SerializeField] private Transform grids;
        private VPhaseEndingSelectionMenu _menu;
        private List<TMP_Text> conditionDescriptions;
        public VStreamEvent E { get; private set; }

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

        public void Initialize(VStreamEvent streamEvent, VPhaseEndingSelectionMenu menu)
        {
            ;
            E = streamEvent;
            _menu = menu;
            conditionDescriptions = new List<TMP_Text>();
            titleText.text = streamEvent.EventName;
            var kpis = streamEvent.Kpis;
            foreach (var kpi in kpis)
            {
                var conditionGo = Instantiate(conditionPrefab, grids);
                var text = conditionGo.GetComponent<TMP_Text>();
                text.text = $"每周至少需完成 {kpi.RequiredAmount} 次 <color=red>{kpi.AbilityName}{kpi.EventName}</color>";
            }
        }

        public void Unselect()
        {
            backgroundImage.color = Color.white;
        }
    }
}