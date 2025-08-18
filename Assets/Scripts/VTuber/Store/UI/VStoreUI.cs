using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Store.UI
{
    public class VStoreUI : VUIBehaviour
    {
        [SerializeField] private Button refreshButton;
        [SerializeField] private TMP_Text refreshCountText;
        [SerializeField] private VStoreButtonUI discardButton;
        [SerializeField] private VStoreButtonUI upgradeButton;
        [SerializeField] private List<VStoreItemUI> storeCardItemUIs = new List<VStoreItemUI>();
        [SerializeField] private List<VStoreItemUI> storeConsumableItemUIs = new List<VStoreItemUI>();
        VCharacter _character;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEnterStore, OnEnterStore);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnMoneyChanged, OnMoneyChanged);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnStoreEndRefresh, OnStoreEndRefresh);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnAddConsumable, OnAddConsumable);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRemoveConsumable, OnRemoveConsumable);
        }

        private void OnRemoveConsumable(Dictionary<string, object> messagedict)
        {
            storeConsumableItemUIs.ForEach(storeConsumableItemUI => (storeConsumableItemUI as VStoreConsumableItemUI).AreSlotsFull(false));
        }

        private void OnAddConsumable(Dictionary<string, object> messagedict)
        {
            storeConsumableItemUIs.ForEach(storeConsumableItemUI =>
                (storeConsumableItemUI as VStoreConsumableItemUI).AreSlotsFull((bool)messagedict["AreSlotsFull"]));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEnterStore, OnEnterStore);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnMoneyChanged, OnMoneyChanged);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnStoreEndRefresh, OnStoreEndRefresh);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnAddConsumable, OnAddConsumable);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRemoveConsumable, OnRemoveConsumable);
        }
        
        private void OnMoneyChanged(Dictionary<string, object> messagedict)
        {
            storeCardItemUIs.ForEach(storeCardItemUI => storeCardItemUI.SetCanAfford());
            storeConsumableItemUIs.ForEach(storeConsumableItemUI => storeConsumableItemUI.SetCanAfford());
            
            upgradeButton.SetCanAfford();
            discardButton.SetCanAfford();
        }
        
        private void OnStoreEndRefresh(Dictionary<string, object> messagedict)
        {
            _character = messagedict["Character"] as VCharacter;
            var cardSlots = messagedict["CardSlots"] as List<VStoreCardSlot>;
            for (int i = 0; i < cardSlots.Count; i++)
            {
                storeCardItemUIs[i].SetSlot(cardSlots[i], _character);
            }
            
            var consumableSlots = messagedict["ConsumableSlots"] as List<VStoreConsumableSlot>;
            for (int i = 0; i < consumableSlots.Count; i++)
            {
                storeConsumableItemUIs[i].SetSlot(consumableSlots[i], _character);
            }
            
            discardButton.SetButton(messagedict["DiscardButton"] as VStoreButton, _character);
            upgradeButton.SetButton(messagedict["UpgradeButton"] as VStoreButton, _character);
            
            refreshCountText.text = messagedict["RefreshCount"].ToString();
            refreshButton.interactable = refreshCountText.text != "0";
        }
        
        private void OnEnterStore(Dictionary<string, object> messagedict)
        {
            _character = messagedict["Character"] as VCharacter;
            var cardSlots = messagedict["CardSlots"] as List<VStoreCardSlot>;
            for (int i = 0; i < cardSlots.Count; i++)
            {
                storeCardItemUIs[i].SetSlot(cardSlots[i], _character);
            }
            
            var consumableSlots = messagedict["ConsumableSlots"] as List<VStoreConsumableSlot>;
            for (int i = 0; i < consumableSlots.Count; i++)
            {
                storeConsumableItemUIs[i].SetSlot(consumableSlots[i], _character);
            }
            
            discardButton.SetButton(messagedict["DiscardButton"] as VStoreButton, _character);
            upgradeButton.SetButton(messagedict["UpgradeButton"] as VStoreButton, _character);
            
            refreshCountText.text = messagedict["RefreshCount"].ToString();
            refreshButton.interactable = refreshCountText.text != "0";
        }
        
        public void NotifyStoreBeginRefresh()
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreBeginRefresh, new Dictionary<string, object>()
            {
            });
        }
    }
}