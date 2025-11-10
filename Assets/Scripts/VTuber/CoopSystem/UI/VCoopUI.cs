using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using VTuber.CoopSystem.UI.DetailsUI;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.CoopSystem.UI
{
    public class VCoopUI : VUIBehaviour
    {
        [SerializeField] private GameObject cooperatorPrefab;
        [SerializeField] private VCoopDetails detailsTab;
        [SerializeField] private VCooperatorUI _selectedCooperator;
        private List<VCooperatorUI> uis;


        protected override void Awake()
        {
            base.Awake();
            uis = new List<VCooperatorUI>();
            detailsTab.onHide += OnDetailsHide;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnCooperatorAdded, OnCooperatorAdded);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnCooperatorRemoved,
                OnCooperatorRemoved);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnCooperatorValueUpdated,
                OnCooperatorValueUpdated);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSetCoopUpgradeEvent,
                OnSetCoopUpgradeEvent);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnFinishScheduleCreationOrModification,
                OnFinishScheduleCreationOrModification);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEndRun, OnEndRun);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSwitchToScheduleCreation,
                OnSwitchToScheduleCreationModify);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSwitchToModifySchedule,
                OnSwitchToScheduleCreationModify);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnReset, OnSwitchToMainMenu);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnCooperatorAdded, OnCooperatorAdded);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnCooperatorRemoved, OnCooperatorRemoved);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnCooperatorValueUpdated,
                OnCooperatorValueUpdated);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSetCoopUpgradeEvent,
                OnSetCoopUpgradeEvent);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnFinishScheduleCreationOrModification,
                OnFinishScheduleCreationOrModification);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEndRun, OnEndRun);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSwitchToScheduleCreation,
                OnSwitchToScheduleCreationModify);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSwitchToModifySchedule,
                OnSwitchToScheduleCreationModify);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnReset, OnSwitchToMainMenu);
        }

        private void OnSwitchToMainMenu(Dictionary<string, object> messagedict)
        {
            Clear();
        }

        private void OnEndRun(Dictionary<string, object> messagedict)
        {
            Clear();
        }

        public void Clear()
        {
            foreach (var ui in uis)
            {
                ui.OnFinishScheduleCreationOrModification();
                Destroy(ui.gameObject);
            }

            uis.Clear();
        }

        private void OnSwitchToScheduleCreationModify(Dictionary<string, object> messagedict)
        {
            foreach (var ui in uis) ui.OnSwitchToScheduleCreationModify();
        }

        private void OnFinishScheduleCreationOrModification(Dictionary<string, object> messagedict)
        {
            foreach (var ui in uis) ui.OnFinishScheduleCreationOrModification();
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
            VDebug.Log("OnCooperatorAdded");
            ;
            var cooperator = messagedict["Cooperator"] as VCooperator;
            var cooperatorGo = Instantiate(cooperatorPrefab, transform);
            var ui = cooperatorGo.GetComponent<VCooperatorUI>();
            ui.SetCooperator(cooperator, CooperatorClicked);
            uis.Add(ui);
        }

        public void OnDetailsHide()
        {
            _selectedCooperator.Unselect();
            _selectedCooperator = null;
            foreach (var ui in uis)
            {
                ui.SetSlotShowable(true);
                ui.RestoreSlot();
            }
        }

        public void CooperatorClicked(VCooperatorUI cooperator)
        {
            if (cooperator == _selectedCooperator)
            {
                detailsTab.Hide();
                return;
            }

            var tween = Tween.Delay(0.05f);
            foreach (var ui in uis)
            {
                ui.SetSlotShowable(false);
                tween = ui.HideSlot(true);
            }

            if (_selectedCooperator)
                _selectedCooperator.Unselect();
            _selectedCooperator = cooperator;
            tween.OnComplete(() => detailsTab.Show());
            detailsTab.SetCooperator(cooperator.Cooperator);
        }
    }
}