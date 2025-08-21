using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Dialogue.UI;

namespace VTuber.ScheduleSystem.UI.ConsumableViewUI
{
    public class VSelectFrom3ConsumablesMenu : VUIBehaviour
    {
        [SerializeField] private GameObject consumablePrefab;
        private List<VSelectcConsumableUI> _consumableUIs;
        private VSelectcConsumableUI _selectedConsumableUI;
        
        [SerializeField] public Button confirmButton;
        private Action<VConsumable> _confirmAction;
        private VCharacter _character;
        
        public List<Transform> positions;
        public Transform spawnPosition;
        
        public void Initialize(VCharacter character, List<VConsumable> consumables, Action<VConsumable> confirmAction)
        {
            confirmButton.interactable = false;
            _confirmAction = confirmAction;
            confirmButton.onClick.AddListener(Confirm);
            int i = 0;
            _character = character;
            _consumableUIs = new List<VSelectcConsumableUI>();
            foreach (var card in consumables)
            {
                var item = Instantiate(consumablePrefab, transform);
                var consumableItem = item.AddComponent<VSelectcConsumableUI>();
                var consumableUI = consumableItem.GetComponent<VConsumableUI>();
                consumableUI.SetConsumable(card);
                
                consumableItem.Initialize(consumableUI, this, Select);
                _consumableUIs.Add(consumableItem);
                
                consumableUI.transform.localScale = Vector3.zero;
                consumableUI.transform.position = spawnPosition.position;
                Tween.Position(consumableUI.transform, positions[i].position, 0.5f);
                Tween.Scale(consumableUI.transform, Vector3.one, 0.5f, Ease.OutBounce).OnComplete((() =>
                {
                    consumableItem.SetSelectable(true);
                }));
                i++;
            }

            bool areSlotsFull = character.ConsumableManager.CanAddConsumable();
                
            foreach (var consumableUI in _consumableUIs)
            {
                consumableUI.SetSelectable(areSlotsFull);
            }
            confirmButton.interactable = !areSlotsFull;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnAddConsumable, OnAddConsumable);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRemoveConsumable, OnRemoveConsumable);
        }

        private void OnRemoveConsumable(Dictionary<string, object> messagedict)
        {
            bool areSlotsFull = (bool)messagedict["AreSlotsFull"];
                
            foreach (var consumableUI in _consumableUIs)
            {
                consumableUI.SetSelectable(areSlotsFull);
            }
            confirmButton.interactable = !areSlotsFull;
        }

        private void OnAddConsumable(Dictionary<string, object> messagedict)
        {
            bool areSlotsFull = (bool)messagedict["AreSlotsFull"];
                
            foreach (var consumableUI in _consumableUIs)
            {
                consumableUI.SetSelectable(areSlotsFull);
            }
            confirmButton.interactable = !areSlotsFull;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnAddConsumable, OnAddConsumable);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRemoveConsumable, OnRemoveConsumable);
        }

        public void Confirm()
        {
            _confirmAction?.Invoke(_selectedConsumableUI.Card);
            
            foreach (var consumableUI in _consumableUIs)
            {
                Destroy(consumableUI.gameObject);
            }
            _consumableUIs.Clear();
            _selectedConsumableUI = null;
            VEventSystemUI.Instance.CloseSelectFrom3Menu();
        }
        
        public void Select(VSelectcConsumableUI consumableUI)
        {
            confirmButton.interactable = true;
            if (_selectedConsumableUI != null && _selectedConsumableUI == consumableUI)
                return;
            
            if(_selectedConsumableUI is not null)
                _selectedConsumableUI.UnSelect();
            _selectedConsumableUI = consumableUI;
        }
    }
}