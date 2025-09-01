using System.Collections.Generic;
using VTuber.BattleSystem.Core.KPIs.UI;
using VTuber.Core.KPIs.UI;
using VTuber.ScheduleSystem.Core;

namespace VTuber.BattleSystem.Core.KPIs
{
    public class VKPIManager
    {
        protected List<VKPI> permanentKPIs = new List<VKPI>();
        protected List<VKPI> phaseKPIs = new List<VKPI>();
        public uint idDistributor = 0;
        
        public void AddPermanentKPI(VKPI kpi)
        {
            permanentKPIs.Add(kpi);
            kpi.OnAdded(idDistributor++);
        }
        
        public void AddPermanentKPI(List<VKPI> kpis)
        {
            foreach (var kpi in kpis)
            {
                AddPermanentKPI(kpi);
            }
        }
        
        public void AddPhaseKPI(List<VKPI> kpis)
        {
            foreach (var kpi in kpis)
            {
                AddPhaseKPI(kpi);
            }
        }
        
        public void AddPhaseKPI(VKPI kpi)
        {
            phaseKPIs.Add(kpi);
            kpi.OnAdded(idDistributor++);
        }
        
        public void RemovePermanentKPI(VKPI kpi)
        {
            kpi.OnRemoved();
            permanentKPIs.Remove(kpi);
        }
        
        public void RemovePhaseKPI(VKPI kpi)
        {
            kpi.OnRemoved();
            phaseKPIs.Remove(kpi);
        }

        public void ClearPhaseKPIs()
        {
            foreach (var kpi in phaseKPIs)
            {
                kpi.OnRemoved();
            }
            phaseKPIs.Clear();
        }
        
        public bool CheckKPIs(Dictionary<VEventType, int> events, List<int> streamEvents)
        {
            bool satisfied = true;
            foreach (var kpi in permanentKPIs)
            {
                if (!kpi.Check(events, streamEvents))
                {
                    satisfied = false;
                }
            }
            foreach (var kpi in phaseKPIs)
            {
                if (!kpi.Check(events, streamEvents))
                {
                    satisfied = false;
                }
            }
            return satisfied;
        }
        
        public void ResetKPIUIs()
        {
            VKPIUIManager.Instance.ResetKPIUIs();
        }

        public bool HasKPIs()
        {
            return permanentKPIs.Count > 0 || phaseKPIs.Count > 0;
        }

        public void ClearKPIs()
        {
            foreach (var kpi in permanentKPIs)
            {
                kpi.OnRemoved();
            }
            permanentKPIs.Clear();
            ClearPhaseKPIs();
        }
    }
}