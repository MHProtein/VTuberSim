using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI {
    public class VBuffGroupUI : VUIBehaviour {
        [SerializeField] private GameObject ellipsisPrefab;
        [SerializeField] private GameObject buffCellPrefab;
        [SerializeField] private int displayingBuffCount = 6;

        private List<VBuffUI> _displayingBuffUIs;
        private List<VBuffUI> _hiddenBuffUIs;
        private VAnimationQueue _animationQueue = new VAnimationQueue();

        private GameObject _ellipsisObject;

        protected override void Awake() {
            base.Awake();
            _displayingBuffUIs = new List<VBuffUI>();
            _hiddenBuffUIs = new List<VBuffUI>();
        }

        protected override void OnEnable() {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBuffAdded, OnBuffAdded);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBuffRemoved, OnBuffRemoved);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBuffValueUpdated, OnBuffValueUpdated);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
        }

        protected override void OnDisable() {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBuffAdded, OnBuffAdded);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBuffRemoved, OnBuffRemoved);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBuffValueUpdated, OnBuffValueUpdated);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
        }

        private void OnBattleEnd(Dictionary<string, object> messagedict) {
            foreach (var ui in _hiddenBuffUIs) {
                Destroy(ui.gameObject);
            }
            foreach (var ui in _displayingBuffUIs) {
                Destroy(ui.gameObject);
            }
            _hiddenBuffUIs.Clear();
            _displayingBuffUIs.Clear();
            if (_ellipsisObject) {
                Destroy(_ellipsisObject);
                _ellipsisObject = null;
            }
        }

        private void OnBuffAdded(Dictionary<string, object> msg) {
            uint id = (uint)msg["Id"];
            bool isFromCard = msg["IsFromCard"] as bool? ?? false;
            bool shouldTwice = msg["ShouldPlayTwice"] as bool? ?? false;
            bool isPermanent = (bool)msg["IsPermanent"];
            string buffName = (string)msg["BuffName"];
            int value = (int)msg["Value"];
            int latency = (int)msg["Latency"];

            // Always create one buffCell
            var go = Instantiate(buffCellPrefab, transform);
            var ui = go.GetComponent<VBuffUI>();
            ui.SetBuff((VBuffItem)msg["Buff"]);
            ui.SetText(value);

            // Default hidden
            go.transform.localScale = Vector3.zero;

            int n = _displayingBuffUIs.Count + (_ellipsisObject ? 1 : 0);

            if (n < displayingBuffCount) {
                // Add to display list
                _displayingBuffUIs.Add(ui);
                _animationQueue.Enqueue(
                    Tween.Scale(go.transform, Vector3.one, 0.4f).OnComplete(() => {
                        RaiseEvents(isFromCard, shouldTwice);
                    })
                );
            } else {
                // Hide the last displayed and show ellipsis
                if (_hiddenBuffUIs.Count == 0) {
                    var lastUI = _displayingBuffUIs[_displayingBuffUIs.Count - 1];
                    lastUI.gameObject.SetActive(false);
                    _hiddenBuffUIs.Add(lastUI);
                    _ellipsisObject = Instantiate(ellipsisPrefab, transform);
                }

                go.SetActive(false);
                _hiddenBuffUIs.Add(ui);
            }
        }

        private void OnBuffValueUpdated(Dictionary<string, object> msg) {
            uint id = (uint)msg["Id"];
            var relic = _displayingBuffUIs.Find(ui => ui.id == id);
            if (relic == null) {
                relic = _hiddenBuffUIs.Find(ui => ui.id == id);
            }

            if (relic == null) {
                // Buff not found, still raise for consistency
                RaiseEvents(false, false);
                return;
            }

            relic.SetText((int)msg["Value"]);

            if (_displayingBuffUIs.Contains(relic)) {
                // Punch only if base scale > 0 to avoid NaN
                if (relic.transform.localScale.sqrMagnitude > 0.0001f) {
                    _animationQueue.Enqueue(
                        Tween.PunchScale(relic.transform, Vector3.one * 0.3f, 0.4f).OnComplete(() => {
                            RaiseEvents(msg["IsFromCard"] as bool? ?? false,
                                msg["ShouldPlayTwice"] as bool? ?? false);
                        })
                    );
                } else {
                    RaiseEvents(msg["IsFromCard"] as bool? ?? false,
                        msg["ShouldPlayTwice"] as bool? ?? false);
                }
            }
        }

        private void OnBuffRemoved(Dictionary<string, object> msg) {
            uint id = (uint)msg["Id"];

            var relic = _displayingBuffUIs.Find(ui => ui.id == id);
            if (relic != null) {
                _displayingBuffUIs.Remove(relic);
                Destroy(relic.gameObject);

                if (_hiddenBuffUIs.Count > 0) {
                    var hiddenRelic = _hiddenBuffUIs[0];
                    _hiddenBuffUIs.RemoveAt(0);
                    hiddenRelic.gameObject.SetActive(true);

                    if (_hiddenBuffUIs.Count == 0) {
                        if (_ellipsisObject) {
                            Destroy(_ellipsisObject);
                            _ellipsisObject = null;
                        }
                    } else {
                        _ellipsisObject.transform.SetParent(null);
                        _ellipsisObject.transform.SetParent(transform);
                    }
                    _displayingBuffUIs.Add(hiddenRelic);
                }
            } else {
                var hiddenRelic = _hiddenBuffUIs.Find(ui => ui.id == id);
                if (hiddenRelic != null) {
                    _hiddenBuffUIs.Remove(hiddenRelic);
                    Destroy(hiddenRelic.gameObject);

                    if (_hiddenBuffUIs.Count == 0 && _ellipsisObject) {
                        Destroy(_ellipsisObject);
                        _ellipsisObject = null;
                    }
                }
            }
        }

        private void RaiseEvents(bool isFromCard, bool shouldPlayTwice) {
            if (shouldPlayTwice) {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnPlayTheSecondTime,
                    new Dictionary<string, object>());
                return;
            }
            if (isFromCard) {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                    new Dictionary<string, object>());
            }
        }
    }
}
