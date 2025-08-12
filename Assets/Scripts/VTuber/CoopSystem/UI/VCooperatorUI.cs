using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.UI;

namespace VTuber.CoopSystem.UI
{
    public class VCooperatorUI : VUIBehaviour
    {
        public uint Id { get; private set; }
        [SerializeField] private Image pfp;
        [SerializeField] private TMP_Text cooperatorName;
        [SerializeField] private TMP_Text coopLevel;
        [SerializeField] private VScheduleSlot upgradeEventScheduleSlot;
        [SerializeField] private GameObject upgradeEventUIPrefab;
        
        private VEventUI _upgradeEventUI;
        
        public void SetCooperator(VCooperator cooperator)
        {
            Id = cooperator.Id;
            pfp.sprite = cooperator.configuration.Icon;
            cooperatorName.text = cooperator.configuration.Name;
            coopLevel.text = cooperator.CurrentCoopLevel.levelName;
        }

        public void UpdateValue(VCooperator cooperator)
        {
            coopLevel.text = cooperator.CurrentCoopLevel.levelName;
        }
        
        public void SetUpgradeEvent(VScheduleEvent scheduleEvent)
        {
            upgradeEventScheduleSlot.gameObject.SetActive(true);
            upgradeEventScheduleSlot.SetPlaceable(true, false);
            
            _upgradeEventUI = Instantiate(upgradeEventUIPrefab, upgradeEventScheduleSlot.transform).GetComponent<VEventUI>();
            _upgradeEventUI.Initialize(scheduleEvent, upgradeEventScheduleSlot);
            upgradeEventScheduleSlot.SetPlaceable(false, false);
        }
        
        public void ClearUpgradeEvent()
        {
            upgradeEventScheduleSlot.gameObject.SetActive(false);
            if(upgradeEventScheduleSlot.Item is not null)
                Destroy(upgradeEventScheduleSlot.Item.gameObject); 
        }
    }
}