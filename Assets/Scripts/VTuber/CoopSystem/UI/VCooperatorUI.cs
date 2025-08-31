using System;
using PrimeTween;
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
        [SerializeField] private RectTransform slotHidePos;
        [SerializeField] private RectTransform slotShowPos;
        [SerializeField] private Button showHideButton;
        [SerializeField] private RectTransform showHideSymbol;
        
        
        public VCooperator Cooperator => _cooperator;
        private VCooperator _cooperator;
        private Action<VCooperatorUI> _onClicked;
        
        private VEventUI _upgradeEventUI;
        private bool _slotShowable;
        private bool _isSlotShowing = false;
        private bool _shouldRecoverSlot = false;
        private bool _selected = false;

        protected override void Awake()
        {
            base.Awake();
            (upgradeEventScheduleSlot.transform as RectTransform).anchoredPosition = slotHidePos.anchoredPosition;
            showHideButton.onClick.AddListener((() =>
            {
                if (_isSlotShowing)
                {
                    HideSlot(false);
                }
                else
                {
                    ShowSlot();
                }
            }));
        }

        public void SetCooperator(VCooperator cooperator, Action<VCooperatorUI> onClicked)
        {
            Id = cooperator.Id;
            _cooperator = cooperator;
            _onClicked = onClicked;
            pfp.sprite = cooperator.configuration.Icon;
            cooperatorName.text = cooperator.configuration.Name;
            coopLevel.text = cooperator.CurrentCoopLevel.levelName;
            SetBaseCoopEvent(VDataManager.Instance.GetAllEventConfigurations()
                .Find(x => x.id == cooperator.configuration.BaseCoopEvent));
            _slotShowable = false;
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
            _slotShowable = true;
            showHideButton.gameObject.SetActive(true);
            _upgradeEventUI = Instantiate(upgradeEventUIPrefab, upgradeEventScheduleSlot.transform).GetComponent<VEventUI>();
            _upgradeEventUI.Initialize(scheduleEvent, upgradeEventScheduleSlot, false, upgradeEventScheduleSlot.transform);
            upgradeEventScheduleSlot.SetPlaceable(false, false, (int)_upgradeEventUI.Event.EventID);
            upgradeEventScheduleSlot.SetUseThisTransformAsParent(true);
            ShowSlot();
        }                                                     
        
        public void ClearUpgradeEvent()
        {
            showHideButton.gameObject.SetActive(false);
            HideSlot(false);
            _slotShowable = false;
            if(upgradeEventScheduleSlot.Item is not null)
                Destroy(upgradeEventScheduleSlot.Item.gameObject); 
        }

        public void ShowSlot()
        {
            if (!_slotShowable)
                return;
            _isSlotShowing = true;
            showHideSymbol.localScale = new Vector3(-1, 1, 1);
            Tween.UIAnchoredPosition(upgradeEventScheduleSlot.transform as RectTransform, slotShowPos.anchoredPosition, 0.3f);
        }

        public Tween HideSlot(bool shouldRecover)
        {
            if (!_isSlotShowing)
                return Tween.Delay(0.01f);
            _shouldRecoverSlot = shouldRecover;
            _isSlotShowing = false;
            showHideSymbol.localScale = new Vector3(1, 1, 1);
            return Tween.UIAnchoredPosition(upgradeEventScheduleSlot.transform as RectTransform, slotHidePos.anchoredPosition, 0.3f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            background.color = Color.cyan;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(!_selected)
                background.color = Color.white;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_selected)
            {
                _selected = true;
            }
            _onClicked?.Invoke(this);
        }

        public void SetSlotShowable(bool value)
        {
            _slotShowable = value;
        }

        public void RestoreSlot()
        {
            if (_shouldRecoverSlot)
            {
                _shouldRecoverSlot = false;
                ShowSlot();
            }
        }

        public void Unselect()
        {
            _selected = false;
            background.color = Color.white;
        }
    }
}