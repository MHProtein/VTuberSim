using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core.KPIs;
using VTuber.BattleSystem.Core.KPIs.UI;
using VTuber.Core.Foundation;

namespace VTuber.Core.KPIs.UI
{
    public class VKPIUIManager : VSingletonMonobehaviour<VKPIUIManager>
    {
        [SerializeField] private GameObject kpiUIPrefab;
        List<VKPIUI> kpiUIs = new List<VKPIUI>();

        public void AddKPIUI(VKPI kpi)
        {
            var go = Instantiate(kpiUIPrefab, transform, false);
            go.GetComponent<VKPIUI>().Initialize(kpi);
            kpiUIs.Add(go.GetComponent<VKPIUI>());
        }
        
        public void RemoveKPIUI(VKPI kpi)
        {
            var kpiUI = kpiUIs.Find(kpiUI => kpiUI.ID == kpi.ID);
            kpiUIs.Remove(kpiUI);
            Destroy(kpiUI.gameObject);
        }
        
        public void UpdateKPIUI(uint id, int count, bool satisfied)
        {
            var kpiUI = kpiUIs.Find(kpiUI => kpiUI.ID == id);
            kpiUI.SetText(count, satisfied);
        }
        
        public void ResetKPIUIs()
        {
            foreach (var kpiUI in kpiUIs)
            {
                kpiUI.ResetText();
            }
        }
    }
}