using System.Collections.Generic;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.Consumable
{
    public class VConsumableSlotsUI : VUIBehaviour
    {
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private VClickDetectionPanel clickDetectionPanel;
        private readonly List<VConsumableSlotUI> _slots = new();
        public bool IsSubMenuActive { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            for (var i = 0; i < 3; i++) AddSlot();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            clickDetectionPanel.onClick += CloseSubMenu;

            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnAddConsumable, OnAddConsumable);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRemoveConsumable, OnRemoveConsumable);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            clickDetectionPanel.onClick -= CloseSubMenu;
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnAddConsumable, OnAddConsumable);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRemoveConsumable, OnRemoveConsumable);
        }

        private void OnRemoveConsumable(Dictionary<string, object> messagedict)
        {
            var consumable = messagedict["Consumable"] as VConsumable;
            var slot = _slots.Find(slot => slot.GetConsumable() == consumable);
            slot.Clear();
        }

        private void OnAddConsumable(Dictionary<string, object> messagedict)
        {
            var consumable = messagedict["Consumable"] as VConsumable;

            var slot = _slots.Find(slot => slot.HasConsumable() == false);
            slot.SetConsumable(consumable);
        }

        public void CloseSubMenu()
        {
            clickDetectionPanel.gameObject.SetActive(false);
            IsSubMenuActive = false;
            _slots.ForEach(slot => slot.SetSubMenuInactive());
        }

        public void AddSlot()
        {
            var slotGo = Instantiate(slotPrefab, transform);
            var slot = slotGo.GetComponent<VConsumableSlotUI>();
            slot.Init(this);
            _slots.Add(slot);
        }

        public void OnSubMenuOn()
        {
            clickDetectionPanel.gameObject.SetActive(true);
            IsSubMenuActive = true;
        }
    }
}