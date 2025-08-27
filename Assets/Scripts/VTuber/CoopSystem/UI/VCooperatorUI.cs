using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.UI;

namespace VTuber.CoopSystem.UI
{
    public class VCooperatorUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public uint Id { get; private set; }
        [SerializeField] private Image pfp;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text cooperatorName;
        [SerializeField] private TMP_Text coopLevel;
        [SerializeField] private VScheduleSlot upgradeEventScheduleSlot;
        [SerializeField] private GameObject upgradeEventUIPrefab;
        [SerializeField] protected GameObject itemDataPrefab;
        [SerializeField] protected VScheduleCreatorSlot creatorSlot;
        private VCooperator _cooperator;
        private Action<VCooperator> _onClicked;
        
        private VEventUI _upgradeEventUI;
        
        public void SetCooperator(VCooperator cooperator, Action<VCooperator> onClicked)
        {
            Id = cooperator.Id;
            _cooperator = cooperator;
            _onClicked = onClicked;
            pfp.sprite = cooperator.configuration.Icon;
            cooperatorName.text = cooperator.configuration.Name;
            coopLevel.text = cooperator.CurrentCoopLevel.levelName;
            SetBaseCoopEvent(VDataManager.Instance.GetAllEventConfigurations()
                .Find(x => x.id == cooperator.configuration.BaseCoopEvent));
        }

        public void SetBaseCoopEvent(VScheduleEventConfiguration eventData)
        {
            var eventObj = Instantiate(itemDataPrefab, creatorSlot.transform);
            eventObj.transform.localPosition = Vector3.zero;
            var eventUI = eventObj.GetComponent<VEventDataUI>();
            eventUI.Initialize(eventData);
            creatorSlot.SetItem(eventUI);
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            background.color = Color.cyan;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            background.color = Color.white;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClicked?.Invoke(_cooperator);
        }
    }
}