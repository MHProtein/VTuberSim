using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VTuber.CoopSystem.UI.DetailsUI
{
    public class VCoopEventTab : VCoopTab
    {
        [SerializeField] private TMP_Text description;
        [SerializeField] private GameObject eventUIPrefab;
        [SerializeField] private Transform eventContainer;
        
        private List<VCoopEventUI> eventUIs;
        
        public override void SetTab(VCooperator cooperator)
        {
            eventUIs = new List<VCoopEventUI>();
            description.text = cooperator.configuration.Description;
            var coopEvents = cooperator.CoopEvents;
            for (int i = 0; i < coopEvents.Count; i++)
            {
                var ui = Instantiate(eventUIPrefab, eventContainer);
                var eventUI = ui.GetComponent<VCoopEventUI>();
                eventUI.Initialize(coopEvents[i], cooperator);
                eventUIs.Add(eventUI);
            }
        }

        public override void Clear()
        {
            if (eventUIs != null)
            {
                foreach (var ui in eventUIs)
                {
                    Destroy(ui.gameObject);
                }
                eventUIs.Clear();
            }
        }
    }
}