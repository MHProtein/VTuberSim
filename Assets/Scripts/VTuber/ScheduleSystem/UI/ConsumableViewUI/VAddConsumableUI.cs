using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VAddConsumableUI : VUIBehaviour
    {
        [SerializeField] private GameObject consumablePrefab;
        public Transform spawnPosition;
        private VSelectcConsumableUI _consumableUI;
        
        [SerializeField] public Button confirmButton;
        [SerializeField] public Button returnButton;
        private Action<VConsumable> _confirmAction;
        private VCharacter _character;
        
        public void Initialize(VCharacter character, VConsumable consumable, Action<VConsumable> confirmAction)
        {
            confirmButton.interactable = false;
            _confirmAction = confirmAction;
            confirmButton.onClick.AddListener(Confirm);
            
            _character = character;
            
            var item = Instantiate(consumablePrefab, spawnPosition);
            _consumableUI = item.AddComponent<VSelectcConsumableUI>();
            var consumableUI = _consumableUI.GetComponent<VConsumableUI>();
            consumableUI.SetConsumable(consumable);
            
            _consumableUI.Initialize(consumableUI, false, null);
            
            consumableUI.transform.localScale = Vector3.zero;
            consumableUI.transform.position = spawnPosition.position;
            Tween.Scale(consumableUI.transform, Vector3.one * 3.0f, 0.5f, Ease.OutBounce).OnComplete((() =>
            {
                _consumableUI.SetSelectable(true);
            }));
            

            bool areSlotsFull = character.ConsumableManager.CanAddConsumable();
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
        
            confirmButton.interactable = !areSlotsFull;
        }

        private void OnAddConsumable(Dictionary<string, object> messagedict)
        {
            bool areSlotsFull = (bool)messagedict["AreSlotsFull"];
            
            confirmButton.interactable = !areSlotsFull;
        }
        
        public void Confirm()
        {
            _confirmAction?.Invoke(_consumableUI.Card);
            
            Destroy(_consumableUI.gameObject);
            
            _consumableUI = null;
        }
    }
}