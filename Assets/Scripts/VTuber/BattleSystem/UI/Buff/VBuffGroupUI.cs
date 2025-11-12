using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Core;
using VTuber.Consumable;
using VTuber.Core.Foundation;
using Tween = PrimeTween.Tween;

namespace VTuber.BattleSystem.UI
{
    public class VBuffGroupUI : VUIBehaviour
    {
        [SerializeField] private GameObject buffDetailsObject;
        [SerializeField] private VClickDetectionPanel clickDetectionPanel;
        [SerializeField] private Transform buffDetailsParent;
        [SerializeField] private GameObject buffDetailsPrefab;
        [SerializeField] private GameObject ellipsisPrefab;
        [SerializeField] private GameObject buffCellPrefab;
        [SerializeField] private int displayingBuffCount = 6;
        private readonly VAnimationQueue _animationQueue = new();

        private List<VBuffUI> _buffUIs;
        private GameObject _ellipsisObject;
        private bool _isDetailsOpen;

        protected override void Awake()
        {
            base.Awake();
            _buffUIs = new List<VBuffUI>();
            clickDetectionPanel.onClick += OpenBuffDetails;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBuffAdded, OnBuffAdded);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBuffRemoved, OnBuffRemoved);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBuffValueUpdated, OnBuffValueUpdated);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBuffAdded, OnBuffAdded);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBuffRemoved, OnBuffRemoved);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBuffValueUpdated, OnBuffValueUpdated);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
        }

        public void OpenBuffDetails()
        {
            _isDetailsOpen = !_isDetailsOpen;
            buffDetailsObject.SetActive(_isDetailsOpen);
            clickDetectionPanel.gameObject.SetActive(_isDetailsOpen);
        }

        private void OnBattleEnd(Dictionary<string, object> msg)
        {
            foreach (var ui in _buffUIs)
            {
                ui.Clear();
                Destroy(ui.gameObject);
            }

            _buffUIs.Clear();
            if (_ellipsisObject)
            {
                Destroy(_ellipsisObject);
                _ellipsisObject = null;
            }
        }

        private void OnBuffAdded(Dictionary<string, object> msg)
        {
            var id = (uint)msg["Id"];
            var isFromCard = msg["IsFromCard"] as bool? ?? false;
            var shouldTwice = msg["ShouldPlayTwice"] as bool? ?? false;
            var buff = (VBuffItem)msg["Buff"];
            var value = (int)msg["Value"];

            var go = Instantiate(buffCellPrefab, transform);
            var details = Instantiate(buffDetailsPrefab, buffDetailsParent);
            var ui = go.GetComponent<VBuffUI>();
            go.transform.localScale = Vector3.zero;
            ui.onClick += () => OpenBuffDetails();
            ui.SetBuff(buff, details.GetComponent<VBuffDetailsUI>());
            ui.SetText(value);


            _buffUIs.Add(ui);
            _animationQueue.Enqueue(Tween.Scale(ui.transform, Vector3.one, 0.3f).OnComplete(() =>
            {
                RaiseEvents(msg["IsFromCard"] as bool? ?? false,
                    msg["ShouldPlayTwice"] as bool? ?? false);
            }));
            RefreshBuffDisplay();
            RaiseEvents(isFromCard, shouldTwice);
        }

        private void OnBuffValueUpdated(Dictionary<string, object> msg)
        {
            var id = (uint)msg["Id"];

            var buff = _buffUIs.Find(ui => ui.id == id);
            if (buff is not null)
            {
                buff.SetText((int)msg["Value"]);

                if (IsVisible(buff))
                    _animationQueue.Enqueue(Tween.PunchScale(buff.transform, Vector3.one * 1.3f, 0.3f).OnComplete(() =>
                    {
                        RaiseEvents(msg["IsFromCard"] as bool? ?? false,
                            msg["ShouldPlayTwice"] as bool? ?? false);
                    }));
            }
            else
            {
                RaiseEvents(false, false);
            }
        }

        private void OnBuffRemoved(Dictionary<string, object> msg)
        {
            var id = (uint)msg["Id"];

            var buff = _buffUIs.Find(ui => ui.id == id);
            if (buff is not null)
            {
                buff.Clear();
                _buffUIs.Remove(buff);
                Destroy(buff.gameObject);
                RefreshBuffDisplay();
            }
        }

        private void RefreshBuffDisplay()
        {
            var ordered = _buffUIs.OrderBy(ui => ui.ConfigID).ToList();

            var n = displayingBuffCount;
            if (ordered.Count > n) n--;

            for (var i = 0; i < ordered.Count; i++)
            {
                var ui = ordered[i];
                if (i < n)
                {
                    ui.gameObject.SetActive(true);
                    ui.transform.SetSiblingIndex(i);
                }
                else
                {
                    ui.gameObject.SetActive(false);
                }
            }

            if (ordered.Count > displayingBuffCount)
            {
                if (_ellipsisObject == null)
                {
                    _ellipsisObject = Instantiate(ellipsisPrefab, transform);
                    _ellipsisObject.transform.SetAsLastSibling();
                    _ellipsisObject.GetComponent<VEllipsisUI>().onClick = OpenBuffDetails;
                }
            }
            else
            {
                if (_ellipsisObject != null)
                {
                    Destroy(_ellipsisObject);
                    _ellipsisObject = null;
                }
            }
        }

        private bool IsVisible(VBuffUI ui)
        {
            return ui.gameObject.activeSelf;
        }

        private void RaiseEvents(bool isFromCard, bool shouldPlayTwice)
        {
            if (shouldPlayTwice)
            {
                VBattleRootEventCenter.Instance.Raise(
                    VBattleEventKey.OnPlayTheSecondTime,
                    new Dictionary<string, object>()
                );
                return;
            }

            if (isFromCard)
                VBattleRootEventCenter.Instance.Raise(
                    VBattleEventKey.OnNotifyBeginDisposeCard,
                    new Dictionary<string, object>()
                );
        }
    }
}