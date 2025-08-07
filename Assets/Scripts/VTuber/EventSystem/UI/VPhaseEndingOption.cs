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
        public VStreamEvent E => e;
        private VStreamEvent e;
        VPhaseEndingSelectionMenu _menu;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private GameObject conditionPrefab;
        [SerializeField] private Transform grids;
        private List<TMP_Text> conditionDescriptions;
        private bool _selectable = true;
        
        public void Initialize(VStreamEvent streamEvent, List<bool> conditionsMet, VPhaseEndingSelectionMenu menu)
        {
            e = streamEvent;
            _menu = menu;
            conditionDescriptions = new List<TMP_Text>();
            titleText.text = streamEvent.EventName;
            var conditions = streamEvent.PhaseEndingConditions;
            for (int i = 0; i < conditions.Count; i++)
            {
                GameObject conditionGo = Instantiate(conditionPrefab, grids);
                var text = conditionGo.GetComponent<TMP_Text>();
                text.text = streamEvent.PhaseEndingConditions[i].GetDescription();
                if (conditionsMet[i])
                {
                    text.color = Color.green;
                }
                else
                {
                    text.color = Color.red;
                    _selectable = false;
                }
                conditionDescriptions.Add(text);
            }
            if(!_selectable)
                backgroundImage.color = Color.grey;
        }

        public void Unselect()
        {
            backgroundImage.color = Color.white;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_selectable)
                return;
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