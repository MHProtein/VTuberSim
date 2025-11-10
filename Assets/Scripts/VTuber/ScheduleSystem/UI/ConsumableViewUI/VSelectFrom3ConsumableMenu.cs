using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.SE;

namespace VTuber.ScheduleSystem.UI.ConsumableViewUI
{
    public class VSelectFrom3ConsumablesMenu : VUIBehaviour
    {
        [SerializeField] private GameObject consumablePrefab;

        [SerializeField] public Button confirmButton;

        public List<Transform> positions;
        public Transform spawnPosition;
        private VCharacter _character;
        private Action<VConsumable> _confirmAction;
        private List<VSelectConsumableUI> _consumableUIs;
        private VSelectConsumableUI _selectedConsumableUI;

        protected override void Awake()
        {
            base.Awake();
            confirmButton.onClick.AddListener(Confirm);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnAddConsumable, OnAddConsumable);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRemoveConsumable, OnRemoveConsumable);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnAddConsumable, OnAddConsumable);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRemoveConsumable, OnRemoveConsumable);
        }

        public void Initialize(VCharacter character, List<VConsumable> consumables, Action<VConsumable> confirmAction)
        {
            confirmButton.interactable = false;
            _confirmAction = confirmAction;
            var i = 0;
            _character = character;
            _consumableUIs = new List<VSelectConsumableUI>();
            foreach (var consumable in consumables)
            {
                var item = Instantiate(consumablePrefab, transform);
                var consumableItem = item.AddComponent<VSelectConsumableUI>();
                var consumableUI = consumableItem.GetComponent<VConsumableUI>();
                consumableUI.SetConsumable(consumable);

                consumableItem.Initialize(consumableUI, false, Select);
                _consumableUIs.Add(consumableItem);

                consumableUI.transform.localScale = Vector3.zero;
                consumableUI.transform.position = spawnPosition.position;
                Tween.Position(consumableUI.transform, positions[i].position, 0.5f);
                Tween.Scale(consumableUI.transform, Vector3.one * 2.0f, 0.5f, Ease.OutBounce);
                i++;
            }

            var selectable = character.ConsumableManager.CanAddConsumable();

            foreach (var consumableUI in _consumableUIs) consumableUI.SetSelectable(selectable);
            confirmButton.interactable = false;
        }

        private void OnRemoveConsumable(Dictionary<string, object> messagedict)
        {
            foreach (var consumableUI in _consumableUIs) consumableUI.SetSelectable(true);
        }

        private void OnAddConsumable(Dictionary<string, object> messagedict)
        {
            var areSlotsFull = (bool)messagedict["AreSlotsFull"];

            foreach (var consumableUI in _consumableUIs) consumableUI.SetSelectable(!areSlotsFull);
        }

        public void Confirm()
        {
            _confirmAction?.Invoke(_selectedConsumableUI.Consumable);

            foreach (var consumableUI in _consumableUIs) Destroy(consumableUI.gameObject);
            _consumableUIs.Clear();
            _selectedConsumableUI = null;
        }

        public void Select(VSelectConsumableUI consumableUI)
        {
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
            confirmButton.interactable = true;
            if (_selectedConsumableUI != null && _selectedConsumableUI == consumableUI)
                return;

            if (_selectedConsumableUI is not null)
                _selectedConsumableUI.UnSelect();
            _selectedConsumableUI = consumableUI;
        }
    }
}