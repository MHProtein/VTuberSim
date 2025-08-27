using System.Collections.Generic;
using UnityEngine;
using VTuber.CoopSystem.UI.DetailsUI;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.CoopSystem.UI
{
    public class VCoopUI : VUIBehaviour
    {
        private List<VCooperatorUI> uis;

        [SerializeField] private GameObject cooperatorPrefab;
        [SerializeField] private VCoopDetails detailsTab;
        
        protected override void Awake()
        {
            base.Awake();
            uis = new List<VCooperatorUI>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnCooperatorAdded, OnCooperatorAdded);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnCooperatorRemoved, OnCooperatorRemoved);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnCooperatorValueUpdated, OnCooperatorValueUpdated);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSetCoopUpgradeEvent, OnSetCoopUpgradeEvent);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnFinishScheduleCreationOrModification, OnFinishScheduleCreationOrModification);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnCooperatorAdded, OnCooperatorAdded);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnCooperatorRemoved, OnCooperatorRemoved);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnCooperatorValueUpdated, OnCooperatorValueUpdated);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSetCoopUpgradeEvent, OnSetCoopUpgradeEvent);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnFinishScheduleCreationOrModification, OnFinishScheduleCreationOrModification);
        }
        
        
        private void OnFinishScheduleCreationOrModification(Dictionary<string, object> messagedict)
        {
            foreach (var ui in uis)
            {
                ui.ClearUpgradeEvent();
            }
        }
        
        private void OnSetCoopUpgradeEvent(Dictionary<string, object> messagedict)
        {
            var cooperator = messagedict["Cooperator"] as VCooperator;
            var ui = uis.Find(x => x.Id == cooperator.Id);
            ui.SetUpgradeEvent(cooperator.UpgradeEvent);
        }

        private void OnCooperatorRemoved(Dictionary<string, object> messagedict)
        {
            var cooperator = messagedict["Cooperator"] as VCooperator;
            var ui = uis.Find(x => x.Id == cooperator.Id);
            uis.Remove(ui);
            Destroy(ui.gameObject);
        }

        private void OnCooperatorValueUpdated(Dictionary<string, object> messagedict)
        {
            var cooperator = messagedict["Cooperator"] as VCooperator;
            var ui = uis.Find(x => x.Id == cooperator.Id);
            ui.UpdateValue(cooperator);
        }

        private void OnCooperatorAdded(Dictionary<string, object> messagedict)
        {
            VDebug.Log("OnCooperatorAdded");;
            var cooperator = messagedict["Cooperator"] as VCooperator;
            GameObject cooperatorGo = Instantiate(cooperatorPrefab, transform);
            var ui = cooperatorGo.GetComponent<VCooperatorUI>();
            ui.SetCooperator(cooperator, CooperatorClicked);
            uis.Add(ui);
        }

        public void CooperatorClicked(VCooperator cooperator)
        {
            detailsTab.SetCooperator(cooperator);
            detailsTab.Show();
        }
    }
}