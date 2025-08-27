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

            var coopEvents = cooperator.CoopEvents;
            for (int i = 0; i < coopEvents.Count; i++)
            {
                var ui = Instantiate(eventUIPrefab, eventContainer);
                var eventUI = ui.GetComponent<VCoopEventUI>();
                eventUI.Initialize(coopEvents[i], cooperator.CurrentLevel, cooperator.CurrentCoopLevel.levelName);
                eventUIs.Add(eventUI);
            }
        }

        public override void Clear()
        {
            foreach (var ui in eventUIs)
            {
                Destroy(ui.gameObject);
            }
            eventUIs.Clear();
        }
    }
}