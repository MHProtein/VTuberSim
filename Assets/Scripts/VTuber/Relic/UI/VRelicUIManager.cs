using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.Relic.UI
{
    public class VRelicUIManager : VUIBehaviour
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private Transform content;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private bool areStreamingRelics;
        [SerializeField] private VRelicMenu relicMenu;
        private List<VRelicSlotUI> _slotUIs;

        protected override void Awake()
        {
            base.Awake();

            _slotUIs = content.GetComponentsInChildren<VRelicSlotUI>().ToList();
            foreach (var slotUI in _slotUIs)
            {
                slotUI.SetShouldShowDescription(false);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRelicAdded, OnRelicAdded);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRelicRemoved, OnRelicRemoved);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRelicValueChanged,
                OnRelicValueChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRelicAdded, OnRelicAdded);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRelicRemoved, OnRelicRemoved);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRelicValueChanged, OnRelicValueChanged);
        }

        public void Show(bool show)
        {
            ui.SetActive(show);
        }

        private void OnRelicRemoved(Dictionary<string, object> messagedict)
        {
            if ((bool)messagedict["IsStreamRelic"] != areStreamingRelics)
                return;

            var relic = (VRelic)messagedict["Relic"];
            var slot = _slotUIs.FirstOrDefault(slot => slot.HasRelic() && slot.Relic.Id == relic.Id);

            if (!slot)
                return;
            slot.Clear();
            if (slot.IsAdditional)
                Destroy(slot);
            else
                slot.transform.SetAsLastSibling();
        }

        private void OnRelicAdded(Dictionary<string, object> messagedict)
        {
            if ((bool)messagedict["IsStreamRelic"] != areStreamingRelics)
                return;

            if (TryGetEmptySlot(out var slot))
            {
                slot.Initialize((VRelic)messagedict["Relic"], areStreamingRelics, relicMenu);
            }
            else
            {
                var go = Instantiate(slotPrefab, content);
                slot = go.GetComponent<VRelicSlotUI>();
                slot.Initialize((VRelic)messagedict["Relic"], areStreamingRelics, relicMenu);
                slot.SetIsAdditional(true);
                slot.SetShouldShowDescription(false);
                
                _slotUIs.Add(slot);
            }
        }

        private void OnRelicValueChanged(Dictionary<string, object> messagedict)
        {
            if ((bool)messagedict["IsStreamRelic"] != areStreamingRelics)
                return;
            var relic = (VRelic)messagedict["Relic"];
            var slot = _slotUIs.FirstOrDefault(slot => slot.HasRelic() && slot.Relic.Id == relic.Id);
            slot.UpdateValue();
        }

        private bool TryGetEmptySlot(out VRelicSlotUI slot)
        {
            foreach (var slotUI in _slotUIs)
                if (!slotUI.HasRelic())
                {
                    slot = slotUI;
                    return true;
                }

            slot = null;
            return false;
        }
    }
}