using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Consumable;
using VTuber.Core.Foundation;
using VTuber.Core.UI;
using VTuber.Relic;
using VTuber.Relic.UI;

namespace VTuber.BattleSystem.UI
{
    public class VRelicGroupUI : VUIBehaviour
    {
        [SerializeField] private int displayingRelicCount = 5;
        [SerializeField] private GameObject subMenuButtonPrefab;
        [SerializeField] private GameObject displayingRelicSlotPrefab;
        [SerializeField] private GameObject submenuRelicSlotPrefab;
        [SerializeField] private Transform displayGroup;
        [SerializeField] private Transform submenuRelicGroup;
        [SerializeField] private GameObject submenuObject;
        [SerializeField] private VClickDetectionPanel detectionPanel;
        private readonly VAnimationQueue _animationQueue = new();

        private GameObject _ellipsisObject;
        private bool _isSubmenuOpen;

        private List<VRelicSlotUI> displayingRelics;
        private List<VRelicSlotUI> hiddenRelics;

        protected override void Awake()
        {
            base.Awake();
            displayingRelics = new List<VRelicSlotUI>();
            hiddenRelics = new List<VRelicSlotUI>();
            detectionPanel.onClick = OnEllipsisButtonClicked;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnRelicAdded, OnRelicAdded);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnRelicRemoved, OnRelicRemoved);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnRelicValueChanged, OnRelicValueUpdated);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleUIInitialize, OnBattleUIInitialize);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnRelicAdded, OnRelicAdded);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnRelicRemoved, OnRelicRemoved);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnRelicValueChanged, OnRelicValueUpdated);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleUIInitialize, OnBattleUIInitialize);
        }

        public void OnEllipsisButtonClicked()
        {
            _isSubmenuOpen = !_isSubmenuOpen;
            submenuObject.SetActive(_isSubmenuOpen);
            detectionPanel.gameObject.SetActive(_isSubmenuOpen);
        }

        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            Clear();
        }
        
        private void OnBattleUIInitialize(Dictionary<string, object> messagedict)
        {
            Clear();
        }
        
        public void Clear()
        {
            foreach (var ui in hiddenRelics) Destroy(ui.gameObject);
            foreach (var ui in displayingRelics) Destroy(ui.gameObject);
            hiddenRelics.Clear();
            displayingRelics.Clear();
        }

        private VRelicSlotUI SpawnRelicSlot(GameObject prefab, Transform parent)
        {
            var go = Instantiate(prefab, parent);
            return go.GetComponent<VRelicSlotUI>();
        }

        private void OnRelicAdded(Dictionary<string, object> msg)
        {
            var relic = (VRelic)msg["Relic"];

            if (displayingRelics.Count <= displayingRelicCount)
            {
                var relicUI = SpawnRelicSlot(displayingRelicSlotPrefab, displayGroup);
                displayingRelics.Add(relicUI);
                relicUI.Initialize(relic, true);
                return;
            }

            if (hiddenRelics.Count == 0)
            {
                _ellipsisObject = Instantiate(subMenuButtonPrefab, displayGroup);
                var button = _ellipsisObject.GetComponent<Button>();
                button.onClick.AddListener(OnEllipsisButtonClicked);
            }

            var newRelicUI = SpawnRelicSlot(submenuRelicSlotPrefab, submenuRelicGroup);
            hiddenRelics.Add(newRelicUI);
            newRelicUI.Initialize(relic, true);
        }

        private void OnRelicValueUpdated(Dictionary<string, object> msg)
        {
            var id = (uint)msg["Id"];

            var relic = displayingRelics.Find(ui => ui.BattleID == id);
            if (relic == null) relic = hiddenRelics.Find(ui => ui.BattleID == id);

            if(relic is not null) relic.UpdateValue();
        }

        private void OnRelicRemoved(Dictionary<string, object> msg)
        {
            var id = (uint)msg["Id"];

            var relic = displayingRelics.Find(ui => ui.BattleID == id);
            if (relic != null)
            {
                if (hiddenRelics.Count == 0)
                {
                    Tween.Alpha(relic.Icon, 0.5f, 0.5f);
                    relic.transform.SetAsLastSibling();
                    relic.DisplayValue(false);
                }
                else
                {
                    var hiddenRelic = hiddenRelics[0];
                    hiddenRelics.Remove(hiddenRelic);
                    
                    displayingRelics.Remove(relic);
                    hiddenRelics.Add(relic);
                    var newRelic = SpawnRelicSlot(submenuRelicSlotPrefab, submenuRelicGroup);
                    newRelic.Initialize(relic.Relic, true);
                    Destroy(relic.gameObject);
                    hiddenRelics.Add(newRelic);
                    Tween.Alpha(newRelic.Icon, 0.5f, 0.5f);
                    relic.DisplayValue(false);

                    _ellipsisObject.gameObject.transform.SetParent(null);
                    
                    var newDisplayingRelic = SpawnRelicSlot(displayingRelicSlotPrefab, displayGroup);
                    newDisplayingRelic.Initialize(hiddenRelic.Relic, true);
                    displayingRelics.Add(newDisplayingRelic);
                    VUIUtils.SetImageAlpha(newDisplayingRelic.Icon, hiddenRelic.Icon.color.a);
                    newDisplayingRelic.DisplayValue(hiddenRelic.IsDisplayValue);
                    Destroy(hiddenRelic.gameObject);

                    if (_ellipsisObject != null)
                        _ellipsisObject.gameObject.transform.SetParent(displayGroup);
                }
            }
            else
            {
                var hiddenRelic = hiddenRelics.Find(ui => ui.BattleID == id);
                if (hiddenRelic is null) return;
                
                Tween.Alpha(hiddenRelic.Icon, 0.5f, 0.5f);
                hiddenRelic.transform.SetAsLastSibling();
                hiddenRelic.DisplayValue(false);
            }
        }
    }
}