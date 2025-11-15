using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;
using VTuber.Core.SE;
using VTuber.ScheduleSystem.UI;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.RaisingAnimationSystem.Animations.SelectFrom3ConsumableAnimation
{
    public class VSelectFrom3ConsumablesMenu : VRaisingAnimation
    {
        [SerializeField] private GameObject consumablePrefab;

        [SerializeField] public Button confirmButton;
        [SerializeField] private VConsumableSlotsUI consumableSlotsUI;

        public List<Transform> positions;
        public Transform spawnPosition;
        private List<VSelectConsumableUI> _consumableUIs;
        private VSelectConsumableUI _selectedConsumableUI;
        private Action _onComplete;
        

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

        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLast)
        {
            base.BeginAnimation(request, onComplete, isLast);
            
            _onComplete = onComplete;
            Initialize(request.consumableIDs.Select(id => VDataManager.Instance.CreateConsumableByID(id)).ToList());
        }

        public void Initialize(List<VConsumable> consumables)
        {
            confirmButton.interactable = false;
            var i = 0;
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
            
            var selectable = consumableSlotsUI.AreThereAvailableSlots();

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
            VRaisingAnimationSystem.Instance.EnqueueAnimationRequest(VAnimationRequestFactory.
                CreateAddConsumableRequest(_selectedConsumableUI.Consumable, false), true);
            _onComplete?.Invoke();
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